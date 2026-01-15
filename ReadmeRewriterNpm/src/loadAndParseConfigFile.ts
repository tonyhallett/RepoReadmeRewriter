import * as fs from "fs";
import { CONFIG_FILE_NAME } from "./getOptionsFromConfig";

export function loadAndParseConfigFile(
  configPath: string
): unknown | undefined {
  if (!fs.existsSync(configPath)) {
    return undefined;
  }

  let fileContent: string;
  try {
    fileContent = fs.readFileSync(configPath, "utf-8");
  } catch (err) {
    const message = err instanceof Error ? err.message : String(err);
    throw new Error(`Failed to read ${CONFIG_FILE_NAME}: ${message}`);
  }

  let parsed: unknown;
  try {
    parsed = JSON.parse(fileContent);
  } catch (err) {
    const message = err instanceof Error ? err.message : String(err);
    throw new Error(`Failed to parse ${CONFIG_FILE_NAME}: ${message}`);
  }
  return parsed;
}
