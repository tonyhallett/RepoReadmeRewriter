# RepoReadmeRewriter (npm)

RepoReadmeRewriter packages the existing [.NET ReadmeRewriter CLI](https://www.nuget.org/packages/RepoReadmeRewriter.CLI) as an npm module so it can be driven directly from JavaScript tooling. It rewrites README content by turning repository-relative links and images into absolute URLs and offers several cleanup utilities that the underlying CLI already supports.

## Prerequisites

- Node.js (18 or newer recommended)
- The .NET runtime available on your PATH (the CLI runs as `dotnet ReadmeRewriterCLI.dll`). You can verify with `dotnet --info`.

## Installation

```bash
npm install --save-dev reporeadmerewriter
```

After installation you can invoke the binary with npm scripts, `npx`, or the per-project shim under `node_modules/.bin/reporeadmerewriter`.

## Configuration and option resolution

RepoReadmeRewriter operates relative to the directory you execute it from (the .NET tool defaults to the current working directory). Relative paths for `readme`, `output`, `config` are resolved against that folder.

RepoReadmeRewriter resolves CLI arguments for you before spawning the .NET executable. Resolution happens in the following order:

1. Look for a `repoReadmeRewriter` block inside the nearest `package.json`.
2. If that block is missing we try `repoReadmeRewriter.config.json` next to the same `package.json`.

If neither source is found the command exits with an error explaining that no configuration was detected.

The configuration object maps directly to CLI switches. Each camelCase property becomes a `--kebab-case` flag when passed to the executable (for example `repoUrl` becomes `--repo-url`). Refer to `reporeadmerewriter --help` for the authoritative meaning of each option exposed by the .NET tool.

### Minimal examples

`package.json`:

```json
{
  "name": "example",
  "repoReadmeRewriter": {
    "repoUrl": "https://github.com/owner/repo",
    "ref": "main",
    "output": "OUTPUT.md"
  }
}
```

`repoReadmeRewriter.config.json` (same directory as `package.json`):

```json
{
  "repoUrl": "https://github.com/owner/repo",
  "ref": "main",
  "output": "OUTPUT.md"
}
```

If `repoUrl` is omitted we fall back to the repository URL declared in your `package.json` `repository` field (GitHub or GitLab). Other Boolean or string properties in the configuration object are forwarded verbatim to the CLI.

## Usage

Once your configuration is in place you can run:

```bash
npx reporeadmerewriter
```

Passing a help flag (`--help`, `-h`, `-?`, `/h`, or `/?`) bypasses configuration and shows the CLI's built-in help text.

```bash
npx reporeadmerewriter --help
```

## Programmatic API

The default export mirrors the CLI entry point so you can invoke the bundled .NET executable yourself:

```ts
import repoRepoReadmeRewrite from "reporeadmerewriter";

// Launch the .NET CLI directly when you already have the final argv array
const exitCode = await repoRepoReadmeRewrite([
  "--repo-url",
  "https://github.com/owner/repo",
  "--output",
  "outputreadme.md",
]);
```

`repoRepoReadmeRewrite` simply executes the embedded .NET binary with whatever arguments you supply.

## License

ISC © Tony Hallett
