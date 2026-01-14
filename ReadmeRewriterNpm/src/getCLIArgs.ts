import { getCLIArgsFromPackageJson } from "./getCLIArgsFromPackageJson";
import { getHelpCLIArgs } from "./getHelpCLIArgs";

export function getCLIArgs(args: string[]): string[] {
  return getHelpCLIArgs(args) ?? getCLIArgsFromPackageJson();
}
