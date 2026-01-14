import {
  getCLIArgsFromPackageOptions,
  PackageJsonOptions,
} from "../../src/getCLIArgsFromPackageOptions";
import * as repoUrl from "../../src/resolveRepositoryUrl";

jest.mock("../../src/resolveRepositoryUrl");

describe("getCLIArgsFromPackageOptions", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("uses resolveRepositoryUrl to build args when not on options", () => {
    const mockedResolve = repoUrl.resolveRepositoryUrl as jest.MockedFunction<
      typeof repoUrl.resolveRepositoryUrl
    >;
    mockedResolve.mockReturnValue("https://github.com/owner/repo");

    const packageRepositoryField: repoUrl.RepositoryField = {
      url: "git+...",
    };
    const packageOptions: PackageJsonOptions = {
      repository: packageRepositoryField,
      repoReadmeRewriter: {},
    };
    const args = getCLIArgsFromPackageOptions(packageOptions);

    expect(mockedResolve).toHaveBeenCalledWith(packageRepositoryField);
    expect(args).toEqual(["--repo-url", "https://github.com/owner/repo"]);
  });

  it("should have correct args for boolean options", () => {
    const packageOptions: PackageJsonOptions = {
      repoReadmeRewriter: {
        errorOnHtml: true,
      },
      repository: undefined,
    };
    const args = getCLIArgsFromPackageOptions(packageOptions);

    expect(args).toEqual(["--error-on-html"]);
  });
});
