import { getCLIArgsFromJson } from "../../src/getCLIArgsFromJson";
import { findPackageJson } from "../../src/findPackageJson";
import {
  getCLIArgsFromPackageOptions,
  PackageJsonOptions,
} from "../../src/getCLIArgsFromPackageOptions";
import { getOptionsFromConfig } from "../../src/getOptionsFromConfig";

jest.mock("../../src/findPackageJson");
jest.mock("../../src/getCLIArgsFromPackageOptions");
jest.mock("../../src/getOptionsFromConfig");

describe("getCLIArgsFromJson", () => {
  const mockedFindPackageJson = findPackageJson as jest.MockedFunction<
    typeof findPackageJson
  >;
  const mockedGetCLIArgsFromPackageOptions =
    getCLIArgsFromPackageOptions as jest.MockedFunction<
      typeof getCLIArgsFromPackageOptions
    >;
  const mockedGetOptionsFromConfig =
    getOptionsFromConfig as jest.MockedFunction<typeof getOptionsFromConfig>;

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("builds args from package.json options when present", () => {
    const repoOptions = { errorOnHtml: true };
    const repository = { url: "git+https://github.com/owner/repo" };
    const cliArgsFromPackageOptions = ["--error-on-html"];

    mockedFindPackageJson.mockReturnValue({
      content: {
        repoReadmeRewriter: repoOptions,
        repository,
      },
      path: "/workspace/package.json",
    });

    mockedGetCLIArgsFromPackageOptions.mockImplementation((options) => {
      expect(options).toEqual({
        repoReadmeRewriter: repoOptions,
        repository,
      } satisfies PackageJsonOptions);
      return cliArgsFromPackageOptions;
    });
    const args = getCLIArgsFromJson();

    expect(args).toBe(cliArgsFromPackageOptions);
    expect(mockedGetOptionsFromConfig).not.toHaveBeenCalled();
  });

  it("loads options from config file when package.json is missing them", () => {
    const repoOptions = { removeHtml: true };
    const repository = { url: "git+https://github.com/owner/other" };
    const cliArgsFromPackageOptions = ["--remove-html"];
    const packageJsonPath = "/workspace/package.json";

    mockedFindPackageJson.mockReturnValue({
      content: {
        repository,
      },
      path: packageJsonPath,
    });

    mockedGetOptionsFromConfig.mockImplementation((path) => {
      expect(path).toBe(packageJsonPath);
      return repoOptions;
    });

    mockedGetCLIArgsFromPackageOptions.mockImplementation((options) => {
      expect(options).toEqual({
        repoReadmeRewriter: repoOptions,
        repository,
      } satisfies PackageJsonOptions);
      return cliArgsFromPackageOptions;
    });

    const args = getCLIArgsFromJson();

    expect(args).toBe(cliArgsFromPackageOptions);
  });

  it("throws when neither package.json nor config provide options", () => {
    mockedFindPackageJson.mockReturnValue({
      content: {},
      path: "/workspace/package.json",
    });
    mockedGetOptionsFromConfig.mockReturnValue(undefined);

    expect(() => getCLIArgsFromJson()).toThrow(
      "No repoReadmeRewriter package.json field or repoReadmeRewriter.config.json"
    );
    expect(mockedGetCLIArgsFromPackageOptions).not.toHaveBeenCalled();
  });
});
