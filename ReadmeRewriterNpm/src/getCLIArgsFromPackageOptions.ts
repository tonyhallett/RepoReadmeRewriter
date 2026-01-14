import { RepoReadmeRewriterOptions } from "./RepoReadmeRewriterOptions";
import { RepositoryField, resolveRepositoryUrl } from "./resolveRepositoryUrl";

function getOption(
  name: string,
  value: string | boolean | undefined
): string[] {
  if (value === undefined) {
    return [];
  }
  if (typeof value === "boolean") {
    return value ? [`--${name}`] : [];
  }
  return [`--${name}`, value];
}

export interface PackageJsonOptions {
  repoReadmeRewriter: RepoReadmeRewriterOptions;
  repository: RepositoryField | undefined;
}

export function getCLIArgsFromPackageOptions(
  packageOptions: PackageJsonOptions
): string[] {
  const options = packageOptions.repoReadmeRewriter;
  let repoUrl = options.repoUrl;
  if (!repoUrl && packageOptions.repository) {
    repoUrl = resolveRepositoryUrl(packageOptions.repository);
  }

  return [
    ...getOption("repo-url", repoUrl),
    ...getOption("error-on-html", options.errorOnHtml),
    ...getOption("remove-html", options.removeHtml),
    ...getOption("extract-details-summary", options.extractDetailsSummary),
    ...getOption("config", options.config),
    ...getOption("ref", options.ref),
    ...getOption("gh", options.gh),
    ...getOption("readme", options.readme),
    ...getOption("output", options.output),
  ];
}
