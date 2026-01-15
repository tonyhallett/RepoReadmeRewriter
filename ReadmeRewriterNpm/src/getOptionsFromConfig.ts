import * as path from "node:path";
import { RepoReadmeRewriterOptions } from "./RepoReadmeRewriterOptions";
import { loadAndParseConfigFile } from "./loadAndParseConfigFile";

export const CONFIG_FILE_NAME = "repoReadmeRewriter.config.json";

function asRepoReadmeRewriterOptions(
  value: unknown
): RepoReadmeRewriterOptions {
  if (value == null || typeof value !== "object" || Array.isArray(value)) {
    throw new Error(
      `${CONFIG_FILE_NAME} must contain an object of repoReadmeRewriter options`
    );
  }

  return value as RepoReadmeRewriterOptions;
}

export function getOptionsFromConfig(
  packageJsonPath: string
): RepoReadmeRewriterOptions | undefined {
  const configPath = path.join(path.dirname(packageJsonPath), CONFIG_FILE_NAME);
  const parsed = loadAndParseConfigFile(configPath);
  if (parsed === undefined) {
    return undefined;
  }

  return asRepoReadmeRewriterOptions(parsed);
}
