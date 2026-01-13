# RepoReadmeRewriter CLI

A CLI tool that rewrites relative README asset links (GitHub/GitLab) to absolute URLs.

## Install

`dotnet tool install -g RepoReadmeRewriter.CLI`

## Usage
Run the tool (binary name `reporeadmerewriter`) from your repository root. Use `--help` to see required arguments and available options. Supply `--config <path>` when you want to remove/replace sections or words before rewriting links.[^config]

## Notes
- Outputs a detailed help table with required/optional options, aliases, and default/completion values when present.
- Use the CLI to prepare README assets before publishing packages so links remain valid off-repo.

[^config]:
    `--config` is a JSON file that is deserialized into `RemoveReplaceSettings` (the same structure described in `NugetRepoReadme/README.md`, but consumed here by the CLI). The file can define any combination of:
    ```json
    {
      "removeCommentIdentifiers": { "start": "remove-start", "end": "remove-end" },
      "removalsOrReplacements": [
        { "commentOrRegex": "Comment", "start": "remove-start 1", "end": "remove-end 1", "replacementText": "visible text" },
        { "commentOrRegex": "Regex", "start": "# Remove", "replacementFromFile": true, "replacementText": "replacements/block.md" }
      ],
      "removeReplaceWordsFilePaths": [ "remove-replace-words.txt" ]
    }
    ```
    - `removeCommentIdentifiers` maps to `RemoveCommentIdentifiers` and removes everything between matching HTML comments (end is optional; missing end removes to EOF).
    - Each entry in `removalsOrReplacements` becomes a `RemovalOrReplacement`. `commentOrRegex` is `Comment` (HTML comment markers) or `Regex` (line-based regex). `start` is required; `end` is optional; `replacementText` is optional. If `replacementFromFile` is `true`, `replacementText` must be a file path; the loader resolves it relative to the config file first, then the project directory.
    - `removeReplaceWordsFilePaths` lists files parsed with the same “Removals / Replacements” format shown in `NugetRepoReadme/README.md` (headers `Removals` / `Replacements` separated by `---`; odd lines are patterns, even lines are replacements).
    - If replacement text contains `{readme_marker}`, it is substituted with the absolute URL of the repository README being processed.
    - The loader fails with `Config contained no configuration` when all sections are empty.