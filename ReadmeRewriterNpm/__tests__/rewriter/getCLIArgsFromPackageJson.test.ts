import { getCLIArgsFromPackageJson } from "../../src/getCLIArgsFromPackageJson";
import { findPackageJson } from "../../src/findPackageJson";
import { getCLIArgsFromPackageOptions } from "../../src/getCLIArgsFromPackageOptions";

jest.mock("../../src/findPackageJson");
jest.mock("../../src/getCLIArgsFromPackageOptions");

describe("getCLIArgsFromPackageJson", () => {
  const mockedFindPackageJson = findPackageJson as jest.MockedFunction<
    typeof findPackageJson
  >;
  const mockedGetCLIArgsFromPackageOptions =
    getCLIArgsFromPackageOptions as jest.MockedFunction<
      typeof getCLIArgsFromPackageOptions
    >;

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("returns CLI args produced from package options", () => {
    const cliArgsFromPackageOptions = ["--error-on-html"];
    const expectedOptions = {
      repoReadmeRewriter: {
        errorOnHtml: true,
      },
      repository: { url: "git+https://github.com/owner/repo" },
    };
    const packageJson = {
      ...expectedOptions,
      name: "test-package",
    };
    mockedFindPackageJson.mockReturnValue(packageJson);
    mockedGetCLIArgsFromPackageOptions.mockImplementation((options) => {
      expect(options).toEqual(expectedOptions);
      return cliArgsFromPackageOptions;
    });

    const cliArgs = getCLIArgsFromPackageJson();

    expect(cliArgs).toBe(cliArgsFromPackageOptions);
  });

  it("throws when repoReadmeRewriter configuration is missing", () => {
    mockedFindPackageJson.mockReturnValue({});

    expect(() => getCLIArgsFromPackageJson()).toThrow(
      "No repoReadmeRewriter config found in package.json"
    );
    expect(mockedGetCLIArgsFromPackageOptions).not.toHaveBeenCalled();
  });
});
