import { defineConfig } from '@playwright/test';

// I use Playwright to verify the critical signed-in flows against the real frontend and backend running together.
export default defineConfig({
  testDir: './tests/e2e',
  fullyParallel: false,
  retries: 0,
  use: {
    baseURL: 'http://localhost:3000',
    trace: 'on-first-retry',
  },
  webServer: [
    {
      command: 'dotnet run --project ../backend/backend.csproj --urls http://localhost:5154',
      env: {
        ASPNETCORE_ENVIRONMENT: 'Testing',
        Frontend__BaseUrl: 'http://localhost:3000',
      },
      port: 5154,
      reuseExistingServer: false,
      timeout: 120000,
    },
    {
      command: 'npm run dev -- --host localhost',
      port: 3000,
      reuseExistingServer: false,
      timeout: 120000,
    },
  ],
  workers: 1,
});
