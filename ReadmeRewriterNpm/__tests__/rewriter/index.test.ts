import { repoRepoReadmeRewrite } from "../../src/repoRepoReadmeRewrite";
import entryPoint from "../../src";

describe("package entry point", () => {
  it("exports repoRepoReadmeRewrite as default", () => {
    expect(entryPoint).toBe(repoRepoReadmeRewrite);
  });
});
