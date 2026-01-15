import * as path from "node:path";
import { getOptionsFromConfig } from "../../src/getOptionsFromConfig";
import { loadAndParseConfigFile } from "../../src/loadAndParseConfigFile";
import { CONFIG_FILE_NAME } from "../../src/getOptionsFromConfig";

jest.mock("../../src/loadAndParseConfigFile");

describe("getOptionsFromConfig", () => {
  const mockedLoadAndParseConfigFile =
    loadAndParseConfigFile as jest.MockedFunction<
      typeof loadAndParseConfigFile
    >;

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("returns options when config file contains an object", () => {
    const packageJsonPath = path.join("/workspace", "package.json");
    const configPath = path.join("/workspace", CONFIG_FILE_NAME);
    const options = { errorOnHtml: true };

    mockedLoadAndParseConfigFile.mockReturnValueOnce(options);

    const result = getOptionsFromConfig(packageJsonPath);

    expect(result).toEqual(options);
    expect(mockedLoadAndParseConfigFile).toHaveBeenCalledWith(configPath);
  });

  it("throws when config file does not contain an object", () => {
    const packageJsonPath = path.join("/workspace", "package.json");
    mockedLoadAndParseConfigFile.mockReturnValueOnce(null);

    expect(() => getOptionsFromConfig(packageJsonPath)).toThrow(
      `${CONFIG_FILE_NAME} must contain an object of repoReadmeRewriter options`
    );
  });

  it("returns undefined when config file is missing", () => {
    const packageJsonPath = path.join("/workspace", "package.json");
    mockedLoadAndParseConfigFile.mockReturnValueOnce(undefined);

    const result = getOptionsFromConfig(packageJsonPath);

    expect(result).toBeUndefined();
  });
});
