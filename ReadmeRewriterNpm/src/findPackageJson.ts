import * as fs from "fs";
import { findPackageJsonPath } from "./findPackageJsonPath";

export function findPackageJson(): Record<string, unknown> {
  const pkgPath = findPackageJsonPath();
  const pkgContent = fs.readFileSync(pkgPath, "utf-8");
  return JSON.parse(pkgContent);
}
