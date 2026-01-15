import { runProcessArgs } from "../../src/runProcessArgs";
import { getCLIArgs } from "../../src/getCLIArgs";
import { repoRepoReadmeRewrite } from "../../src/repoRepoReadmeRewrite";

jest.mock("../../src/getCLIArgs");
jest.mock("../../src/repoRepoReadmeRewrite", () => ({
  repoRepoReadmeRewrite: jest.fn(),
}));

describe("runProcessArgs", () => {
  const mockedGetCLIArgs = getCLIArgs as jest.MockedFunction<typeof getCLIArgs>;
  const mockedRepoReadmeRewrite = repoRepoReadmeRewrite as jest.MockedFunction<
    typeof repoRepoReadmeRewrite
  >;

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("returns the exit code from repoRepoReadmeRewrite", async () => {
    const processArgs = ["processarg"];
    const cliArgs = ["--repo-url", "https://example"];
    mockedGetCLIArgs.mockReturnValue(cliArgs);
    mockedRepoReadmeRewrite.mockResolvedValue(123);

    const exitCode = await runProcessArgs(processArgs);

    expect(mockedGetCLIArgs).toHaveBeenCalledWith(processArgs);
    expect(mockedRepoReadmeRewrite).toHaveBeenCalledWith(cliArgs);
    expect(exitCode).toBe(123);
  });

  it("logs and returns 1 when getCLIArgs throws", async () => {
    mockedGetCLIArgs.mockImplementation(() => {
      throw new Error("CLI args failure");
    });
    const errorSpy = jest.spyOn(console, "error").mockImplementation(() => {
      return;
    });

    const exitCode = await runProcessArgs(["--repo-url", "https://example"]);

    expect(errorSpy).toHaveBeenCalledWith("CLI args failure");
    expect(mockedRepoReadmeRewrite).not.toHaveBeenCalled();
    expect(exitCode).toBe(1);

    errorSpy.mockRestore();
  });

  it("logs and returns 1 when repoRepoReadmeRewrite rejects", async () => {
    mockedGetCLIArgs.mockReturnValue(["--repo-url", "https://example"]);
    mockedRepoReadmeRewrite.mockRejectedValue(new Error("spawn failed"));
    const errorSpy = jest.spyOn(console, "error").mockImplementation(() => {
      return;
    });

    const exitCode = await runProcessArgs(["--repo-url", "https://example"]);

    expect(errorSpy).toHaveBeenCalledWith("spawn failed");
    expect(exitCode).toBe(1);

    errorSpy.mockRestore();
  });
});
