import { getCLIArgs } from "./getCLIArgs";
import { repoRepoReadmeRewrite } from "./repoRepoReadmeRewrite";

export async function runProcessArgs(args: string[]): Promise<number> {
  try {
    return await repoRepoReadmeRewrite(getCLIArgs(args));
  } catch (e) {
    const msg = e instanceof Error ? e.message : String(e);
    console.error(msg);
    return 1;
  }
}
