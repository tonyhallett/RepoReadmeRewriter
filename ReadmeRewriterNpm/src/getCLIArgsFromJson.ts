import { findPackageJson } from "./findPackageJson";
import { getCLIArgsFromPackageOptions } from "./getCLIArgsFromPackageOptions";
import { CONFIG_FILE_NAME, getOptionsFromConfig } from "./getOptionsFromConfig";
import {
  packageJsonOptionsConfigKey,
  RepoReadmeRewriterOptions,
} from "./RepoReadmeRewriterOptions";
import { RepositoryField } from "./resolveRepositoryUrl";

export const NoOptionsErrorMessage = `No ${packageJsonOptionsConfigKey} package.json field or ${CONFIG_FILE_NAME}`;

export function getCLIArgsFromJson(): string[] {
  const { content: packageJson, path: packageJsonPath } = findPackageJson();

  let options: RepoReadmeRewriterOptions | undefined;

  if (packageJson["repoReadmeRewriter"] == undefined) {
    options = getOptionsFromConfig(packageJsonPath);
  } else {
    options = packageJson["repoReadmeRewriter"] as RepoReadmeRewriterOptions;
  }

  if (options == undefined) {
    throw new Error(NoOptionsErrorMessage);
  }
  return getCLIArgsFromPackageOptions({
    repoReadmeRewriter: options,
    repository: packageJson.repository as RepositoryField | undefined,
  });
}
