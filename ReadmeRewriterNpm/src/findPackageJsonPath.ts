import * as fs from "fs";
import * as path from "node:path";

export function findPackageJsonPath(): string {
  let dir = process.cwd();
  while (true) {
    const pkgPath = path.join(dir, "package.json");
    if (fs.existsSync(pkgPath)) {
      return pkgPath;
    }
    const parentDir = path.dirname(dir);
    if (parentDir === dir) {
      throw new Error("Could not find package.json");
    }
    dir = parentDir;
  }
}
