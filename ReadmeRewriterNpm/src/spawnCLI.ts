import { spawn } from "node:child_process";

export function spawnCLI(
  dllPath: string,
  cliArgs: string[],
  resolve: (exitCode: number) => void
): void {
  const child = spawn("dotnet", [dllPath, ...cliArgs], {
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
}
