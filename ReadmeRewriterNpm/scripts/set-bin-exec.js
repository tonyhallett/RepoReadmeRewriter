const fs = require("fs");
const path = require("path");

const target = path.join(__dirname, "..", "bin", "rewriter.js");

try {
  if (!fs.existsSync(target)) {
    console.warn(`set-bin-exec: skip (missing ${target})`);
    process.exit(0);
  }

  fs.chmodSync(target, 0o755);
} catch (err) {
  console.warn(`set-bin-exec: unable to set executable bit on ${target}`);
  console.warn(String(err));
}
