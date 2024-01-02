// ========== COPYRIGHT ==========
// Copyright (c) Systems Lab 21 GmbH, 2023 - 2023
// ========== COPYRIGHT ==========
module.exports = {
    "extends": [
      "eslint:recommended",
      "plugin:@typescript-eslint/recommended",
      "plugin:react/recommended",
      "airbnb",
      "airbnb-typescript"
    ],
    "ignorePatterns": [
      "src/setupProxy.js",
      "**/*/proto/*"
    ],
    "parser": "@typescript-eslint/parser",
    "parserOptions": { "project": ["./tsconfig.json"] },
    "plugins": [
      "@typescript-eslint",
      "react",
      "jsx-a11y"
    ],
    "rules": {
      "import/prefer-default-export": "off",
      "react/jsx-indent": ["error", 2],
      "import/no-cycle": "off",
      "max-len": ["warn", {
        "code": 180,
        "comments": 350,
        "ignoreTemplateLiterals": true,
        "ignorePattern": "^(import.*)|(.*className.*)|(d=.*)|(.*ClassName.*)",
        "ignoreUrls": true
      }],
      "jsx-a11y/label-has-associated-control": "off",
      "react/react-in-jsx-scope": "off",
      "no-unused-vars": "off",
      "@typescript-eslint/no-unused-vars": ["error", { "vars": "all", "varsIgnorePattern": "^_", "argsIgnorePattern": "^_" }],
      "no-underscore-dangle": "off",
      "@typescript-eslint/naming-convention": ["error", {
        "trailingUnderscore": "allow",
        "leadingUnderscore": "allow",
        "selector": "variable",
        "format": [
            "camelCase",
            "UPPER_CASE",
            "PascalCase"
        ]
      }],
      "import/no-extraneous-dependencies": "off",
      "react/require-default-props": "off",
      "linebreak-style": ["error", (process.platform === "win32" ? "windows" : "unix")], // https://stackoverflow.com/q/39114446/2771889
      "import/extensions": "off",
      "no-empty-pattern": "off",
      "jsx-a11y/control-has-associated-label": "off"
    }
  }

