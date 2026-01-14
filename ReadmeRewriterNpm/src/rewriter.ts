#!/usr/bin/env node
import { runProcessArgs } from "./runProcessArgs";

function run(): Promise<number> {
  return runProcessArgs(process.argv.slice(2));
}

// Only execute when run directly as a CLI, not when imported.
if (require.main === module) {
  run().then((code) => process.exit(code));
}
