import {
  mkdtempSync,
  writeFileSync,
  rmSync,
  readFileSync,
  mkdirSync,
} from "fs";
import * as os from "os";
import * as path from "path";
import { spawnSync, SpawnSyncReturns } from "child_process";
import pkg from "../package.json";
import {
  RepoReadmeRewriterOptions,
  packageJsonOptionsConfigKey,
} from "../src/RepoReadmeRewriterOptions";
import { NoOptionsErrorMessage } from "../src/getCLIArgsFromJson";

describe("RepoReadmeRewriter CLI integration", () => {
  const repoRoot = process.cwd();
  let dependentDirectory: string;

  function createDependentProject(options?: RepoReadmeRewriterOptions): void {
    const packageJsonContent: Record<string, any> = {
      name: "tmp-proj",
      version: "1.0.0",
    };

    if (options) {
      packageJsonContent[packageJsonOptionsConfigKey] = options;
    }

    dependentDirectory = mkdtempSync(path.join(os.tmpdir(), "rrw-"));
    writeDependentProectFile(
      JSON.stringify(packageJsonContent, null, 2),
      "package.json"
    );
    // create a .git directory to simulate a git repo
    const gitDir = path.join(dependentDirectory, ".git");
    mkdirSync(gitDir);
  }

  function installTarball() {
    const tarball = path.join(repoRoot, `${pkg.name}-${pkg.version}.tgz`);
    // Install the packed tarball into the temp project
    const npmCmd = process.platform === "win32" ? "npm.cmd" : "npm";
    const install = spawnSync(npmCmd, ["i", tarball], {
      cwd: dependentDirectory,
      encoding: "utf8",
      shell: process.platform === "win32",
    });
    if (install.error) {
      throw install.error;
    }
    expect(install.status).toBe(0);
  }

  function executeBin(args: string[] = []) {
    // Run the installed bin via the local .bin shim
    const binName = pkg.name; // npm creates shim matching package name for single-bin packages
    const binPath = path.join(
      dependentDirectory,
      "node_modules",
      ".bin",
      process.platform === "win32" ? `${binName}.cmd` : binName
    );

    return spawnSync(binPath, args, {
      cwd: dependentDirectory,
      encoding: "utf8",
      shell: process.platform === "win32",
    });
  }

  it("shows help with exit 0", () => {
    const result = execute(undefined, undefined, ["--help"]);

    expect(result.status).toBe(0);
    const combined = (result.stdout || "") + (result.stderr || "");
    expect(combined).toMatch(/Usage|help/i);
  });

  it("errors when no options", () => {
    const result = execute();
    expect(result.status).toBe(1);
    const combined = (result.stdout || "") + (result.stderr || "");
    // debug info printed
    expect(combined).toContain(NoOptionsErrorMessage);
  });

  function writeDependentProectFile(contents: string, filename: string) {
    const filePath = path.join(dependentDirectory, filename);
    writeFileSync(filePath, contents, "utf8");
  }

  it("processes a real repo and outputs modified README", () => {
    const options: RepoReadmeRewriterOptions = {
      repoUrl: "https://github.com/tonyhallett/RepoReadmeRewriter",
      ref: "master",
      output: "OUTPUT.md",
    };

    const readmeContents = "Before [Title](.gitignore).";
    const result = execute(() => {
      writeDependentProectFile(readmeContents, "README.md");
      // error if assets are not present
      writeDependentProectFile("", ".gitignore");
    }, options);

    expect(result.status).toBe(0);

    const outputContents = readFileSync(
      path.join(dependentDirectory, "OUTPUT.md"),
      "utf8"
    );
    expect(outputContents).toEqual(
      "Before [Title](https://github.com/tonyhallett/RepoReadmeRewriter/blob/master/.gitignore)."
    );
  });

  afterEach(() => {
    if (dependentDirectory) {
      rmSync(dependentDirectory, { recursive: true, force: true });
    }
  });

  function execute(
    dependentProjectCreated?: () => void,
    options?: RepoReadmeRewriterOptions,
    args: string[] = []
  ): SpawnSyncReturns<string> {
    createDependentProject(options);
    dependentProjectCreated && dependentProjectCreated();
    installTarball();

    return executeBin(args);
  }
});
