import { getHelpCLIArgs } from "./getHelpCLIArgs";
import { getCLIArgsFromJson } from "./getCLIArgsFromJson";

export function getCLIArgs(args: string[]): string[] {
  return getHelpCLIArgs(args) ?? getCLIArgsFromJson();
}
