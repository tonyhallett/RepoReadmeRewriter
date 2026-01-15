export interface RepoReadmeRewriterOptions {
  errorOnHtml?: boolean;
  removeHtml?: boolean;
  extractDetailsSummary?: boolean;
  config?: string;
  ref?: string;
  gh?: string;
  readme?: string;
  output?: string;
  repoUrl?: string;
}

export const packageJsonOptionsConfigKey = "repoReadmeRewriter";
