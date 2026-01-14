import * as path from "node:path";

export function getCLIDllPath(): string {
  return path.join(__dirname, "cli", "ReadmeRewriterCLI.dll");
}
