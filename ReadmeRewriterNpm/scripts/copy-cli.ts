import AdmZip from "adm-zip";
import * as fs from "fs";
import * as path from "path";

function getNuPkgCliPath(): string {
  const nupkgDir = path.join(
    __dirname,
    "..",
    "..",
    "ReadmeRewriterCLI",
    "nupkg"
  );
  const files = fs.readdirSync(nupkgDir);
  const nupkgFiles = files.filter((file) => file.endsWith(".nupkg"));
  if (nupkgFiles.length === 0) {
    throw new Error(`No .nupkg files found in directory: ${nupkgDir}`);
  }
  // get the latest by version number
  nupkgFiles.sort((a, b) => {
    const versionA = a.match(
      /RepoReadmeRewriter\.CLI\.(\d+\.\d+\.\d+)\.nupkg/
    )?.[1];
    const versionB = b.match(
      /RepoReadmeRewriter\.CLI\.(\d+\.\d+\.\d+)\.nupkg/
    )?.[1];
    if (!versionA || !versionB) {
      return 0;
    }
    const partsA = versionA.split(".").map(Number);
    const partsB = versionB.split(".").map(Number);
    for (let i = 0; i < partsA.length; i++) {
      if (partsA[i] > partsB[i]) return -1;
      if (partsA[i] < partsB[i]) return 1;
    }
    return 0;
  });
  const latestNuPkg = nupkgFiles[0];
  return path.join(nupkgDir, latestNuPkg);
}

function unzipIntoBin(nuPkgPath: string): void {
  const binDir = path.join(__dirname, "..", "bin");
  const binTmpDir = path.join(binDir, "tmp");
  const binCLIDir = path.join(binDir, "cli");

  try {
    makeDirs();
    extractToBinTmp();
    copyToolsAnyToBinCli();
  } catch (e) {
    console.error("Error during unzip and copy:", e);
  }
  // delete tmp dir
  fs.rmSync(binTmpDir, { recursive: true, force: true });

  function makeDirs() {
    fs.rmSync(binCLIDir, { recursive: true, force: true });
    fs.mkdirSync(binCLIDir, { recursive: true });
    fs.mkdirSync(binTmpDir, { recursive: true });
  }

  function extractToBinTmp(): void {
    const zip = new AdmZip(nuPkgPath);
    zip.extractAllTo(binTmpDir, true);
  }

  function copyToolsAnyToBinCli(): void {
    const toolsAnyDir = path.join(binTmpDir, "tools", "net8.0", "any");
    const files = fs.readdirSync(toolsAnyDir);
    files.forEach((file) => {
      const srcPath = path.join(toolsAnyDir, file);
      const destPath = path.join(binCLIDir, file);
      fs.cpSync(srcPath, destPath, { recursive: true });
    });
  }
}

unzipIntoBin(getNuPkgCliPath());
