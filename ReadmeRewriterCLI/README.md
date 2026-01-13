# RepoReadmeRewriter CLI

A CLI tool that rewrites relative README asset links (GitHub/GitLab) to absolute URLs.

## Install

`dotnet tool install -g RepoReadmeRewriter.CLI`

## Usage
Run the tool (binary name `reporeadmerewriter`) from your repository root. Use `--help` to see required arguments and available options.


## Notes
- Outputs a detailed help table with required/optional options, aliases, and default/completion values when present.
- Use the CLI to prepare README assets before publishing packages so links remain valid off-repo.