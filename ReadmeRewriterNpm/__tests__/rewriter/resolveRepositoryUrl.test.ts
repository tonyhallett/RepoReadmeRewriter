import { resolveRepositoryUrl } from "../../src/resolveRepositoryUrl";

describe("resolveRepositoryUrl", () => {
  it("should work for git suffixed URLs", () => {
    const url = resolveRepositoryUrl(
      "https://github.com/monatheoctocat/my_package.git"
    );

    expect(url).toBe("https://github.com/monatheoctocat/my_package");
  });

  it("should work for git prefixed urls", () => {
    const url = resolveRepositoryUrl({
      type: "git",
      url: "git+https://github.com/npm/cli.git",
    });

    expect(url).toBe("https://github.com/npm/cli");
  });

  it("should work for shortcuts", () => {
    const url = resolveRepositoryUrl("gitlab:user/repo");

    expect(url).toBe("https://gitlab.com/user/repo");
  });

  it("should throw for unsupported hosts", () => {
    expect(() => {
      resolveRepositoryUrl("bitbucket:user/repo");
    }).toThrow("Only GitHub and GitLab repositories are supported.");
  });
});
