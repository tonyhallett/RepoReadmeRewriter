import { getCLIArgs } from "./getCLIArgs";
import { getCLIDllPath } from "./getCLIDllPath";
import { spawnCLI } from "./spawnCLI";

export function runProcessArgs(args: string[]): Promise<number> {
  return new Promise((resolve) => {
    let cliArgs: string[];

    try {
      cliArgs = getCLIArgs(args);
    } catch (e) {
      const msg = e instanceof Error ? e.message : String(e);
      console.error(msg);
      return resolve(1);
    }

    spawnCLI(getCLIDllPath(), cliArgs, resolve);
  });
}
