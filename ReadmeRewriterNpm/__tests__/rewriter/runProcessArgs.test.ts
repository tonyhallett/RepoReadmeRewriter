import { runProcessArgs } from "../../src/runProcessArgs";
import { getCLIArgs } from "../../src/getCLIArgs";
import { getCLIDllPath } from "../../src/getCLIDllPath";
import { spawnCLI } from "../../src/spawnCLI";

jest.mock("../../src/getCLIArgs");
jest.mock("../../src/getCLIDllPath");
jest.mock("../../src/spawnCLI");

describe("runProcessArgs", () => {
  const mockedGetCLIArgs = getCLIArgs as jest.MockedFunction<typeof getCLIArgs>;
  const mockedGetCLIDllPath = getCLIDllPath as jest.MockedFunction<
    typeof getCLIDllPath
  >;
  const mockedSpawnCLI = spawnCLI as jest.MockedFunction<typeof spawnCLI>;

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("resolves with the exit code returned by spawnCLI", async () => {
    const processArgs = ["processarg"];
    const cliArgs = ["--repo-url", "https://example"];
    mockedGetCLIArgs.mockReturnValue(cliArgs);
    mockedGetCLIDllPath.mockReturnValue("/path/to/cli.dll");
    mockedSpawnCLI.mockImplementation((_dllPath, _cliArgs, resolve) => {
      resolve(123);
    });

    const exitCode = await runProcessArgs(processArgs);

    expect(mockedGetCLIArgs).toHaveBeenCalledWith(processArgs);
    expect(mockedSpawnCLI).toHaveBeenCalledWith(
      "/path/to/cli.dll",
      cliArgs,
      expect.any(Function)
    );
    expect(exitCode).toBe(123);
  });

  it("logs the error and resolves with exit code 1 when getCLIArgs throws", async () => {
    mockedGetCLIArgs.mockImplementation(() => {
      throw new Error("CLI args failure");
    });

    const errorSpy = jest.spyOn(console, "error").mockImplementation(() => {
      return;
    });

    const exitCode = await runProcessArgs(["--repo-url", "https://example"]);

    expect(errorSpy).toHaveBeenCalledWith("CLI args failure");
    expect(mockedSpawnCLI).not.toHaveBeenCalled();
    expect(exitCode).toBe(1);

    errorSpy.mockRestore();
  });
});
