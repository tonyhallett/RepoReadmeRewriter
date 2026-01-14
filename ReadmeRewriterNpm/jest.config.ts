import { createDefaultPreset, JestConfigWithTsJest } from "ts-jest";

const config: JestConfigWithTsJest = {
  ...createDefaultPreset({
    tsconfig: "<rootDir>/tsconfig.jest.json",
  }),
  testEnvironment: "node",
};

export default config;
