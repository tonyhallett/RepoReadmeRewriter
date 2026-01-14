export function getHelpCLIArgs(args: string[]): string[] | undefined {
  const helpFlags = new Set(["--help", "-h", "/h", "-?", "/?"]);
  const requestedHelp = args.some((a) => helpFlags.has(a));
  if (requestedHelp) {
    return args;
  }
  return undefined;
}
