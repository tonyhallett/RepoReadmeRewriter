import hostedGitInfo from "hosted-git-info";

export type RepositoryField =
  | string
  | { url: string; type?: string; directory?: string };

export function resolveRepositoryUrl(repo: RepositoryField): string {
  const raw = typeof repo === "string" ? repo : repo.url;
  if (!raw) {
    throw new Error("Repository field is empty or invalid.");
  }

  const info = hostedGitInfo.fromUrl(raw);
  if (!info) {
    throw new Error(`Unable to parse repository URL: ${raw}`);
  }

  throwIfNotSupportedHost(info);

  // Clean HTTPS URL without git+ prefix or .git suffix
  return info.browse({ noGitPlus: true });
}

function throwIfNotSupportedHost(info: hostedGitInfo): void {
  const supportedHosts: hostedGitInfo.Hosts[] = ["github", "gitlab"];
  const supportedHost = supportedHosts.includes(info.type);
  if (!supportedHost) {
    throw new Error("Only GitHub and GitLab repositories are supported.");
  }
}
