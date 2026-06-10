import { expect, test } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

async function loginAs(page, subjectId, returnPath = '/dashboard') {
  await page.goto(
    `http://localhost:5154/auth/dev/login?subjectId=${encodeURIComponent(subjectId)}&displayName=${encodeURIComponent(`A11y ${subjectId}`)}&email=${encodeURIComponent(`${subjectId}@tests.local`)}&returnTo=${encodeURIComponent(`http://localhost:3000${returnPath}`)}`,
  );
}

async function expectNoCriticalAccessibilityIssues(page) {
  const { violations } = await new AxeBuilder({ page }).analyze();
  const criticalViolations = violations.filter((violation) => violation.impact === 'critical');

  expect(criticalViolations).toEqual([]);
}

test('public pages avoid critical accessibility regressions', async ({ page }) => {
  await page.goto('/');
  await expectNoCriticalAccessibilityIssues(page);

  await page.goto('/games');
  await expectNoCriticalAccessibilityIssues(page);
});

test('the signed-in collection page avoids critical accessibility regressions', async ({ page }) => {
  await loginAs(page, 'e2e-a11y-user', '/my-collection');

  await expect(page).toHaveURL(/\/my-collection$/);
  await expectNoCriticalAccessibilityIssues(page);
});
