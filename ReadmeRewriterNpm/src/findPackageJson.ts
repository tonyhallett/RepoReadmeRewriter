import * as fs from "fs";
import { findPackageJsonPath } from "./findPackageJsonPath";

interface PackageJsonInfo {
  content: Record<string, unknown>;
  path: string;
}

export function findPackageJson(): PackageJsonInfo {
  const pkgPath = findPackageJsonPath();
  const pkgContent = fs.readFileSync(pkgPath, "utf-8");
  return {
    content: JSON.parse(pkgContent) as Record<string, unknown>,
    path: pkgPath,
  };
}
