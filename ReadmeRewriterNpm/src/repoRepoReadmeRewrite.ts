import { spawn } from "child_process";
import path from "path";

export function repoRepoReadmeRewrite(args: string[]): Promise<number> {
  return new Promise((resolve) => {
    const dllPath = path.join(__dirname, "cli", "ReadmeRewriterCLI.dll");
    const child = spawn("dotnet", [dllPath, ...args], {
      stdio: "inherit",
    });

    child.on("error", (err) => {
      console.error(
        [
          "Failed to launch 'dotnet' (spawn error).",
          "Ensure .NET is installed and available on PATH (try 'dotnet --info').",
          String(err),
        ].join("\n")
      );
      resolve(1);
    });

    child.on("close", (code) => {
      // Non-zero exits are emitted by the CLI itself (e.g., missing args).
      // We inherit stdio so the CLI already printed context; just propagate the code.
      resolve(code ?? 1);
    });
  });
}
