import { LogLevel } from "@azure/msal-browser";

export const environment = {
  production: false,
  name: "development",
  settings: {
    auth: {
      clientId: "75b6c518-ca71-410a-8ed6-d8e8f320e035",
      authority: "https://login.microsoftonline.com/87b57f75-016d-4e16-bc51-f63021e6d7d9/",
      redirectUri: "https://localhost:44348/",
      postLogoutRedirectUri: "https://localhost:44348/"
    },
    system: {
      loggerOptions: {
        logLevel: LogLevel.Info
      }
    },
    resources: {
      graphMe: {
        endpoint: "https://graph.microsoft.com/v1.0/me"
      },
      api: {
        scopePrefix: "api://7ba0ef35-3759-4726-a194-28b2e2e2912a",
        endpoint: "https://localhost:44313/api/"
      }
    }
  }
};