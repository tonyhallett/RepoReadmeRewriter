import { findPackageJson } from "./findPackageJson";
import {
  getCLIArgsFromPackageOptions,
  PackageJsonOptions,
} from "./getCLIArgsFromPackageOptions";
import {
  repoReadmeRewriterConfigKey,
  RepoReadmeRewriterOptions,
} from "./RepoReadmeRewriterOptions";
import { RepositoryField } from "./resolveRepositoryUrl";

export function getOptionsFromPackageJson(): PackageJsonOptions {
  const pkg = findPackageJson();

  if (pkg[repoReadmeRewriterConfigKey] == undefined) {
    throw new Error("No repoReadmeRewriter config found in package.json");
  }

  return {
    repoReadmeRewriter: pkg[
      repoReadmeRewriterConfigKey
    ] as RepoReadmeRewriterOptions,
    repository: pkg.repository as RepositoryField | undefined,
  };
}

export function getCLIArgsFromPackageJson(): string[] {
  const packageOptions = getOptionsFromPackageJson();
  return getCLIArgsFromPackageOptions(packageOptions);
}
