import { expect, test } from '@playwright/test';

async function loginAs(page, subjectId, returnPath = '/dashboard') {
  await page.goto(
    `http://localhost:5154/auth/dev/login?subjectId=${encodeURIComponent(subjectId)}&displayName=${encodeURIComponent(`E2E ${subjectId}`)}&email=${encodeURIComponent(`${subjectId}@tests.local`)}&returnTo=${encodeURIComponent(`http://localhost:3000${returnPath}`)}`,
  );
}

test('signed-in user can log in and log out', async ({ page }) => {
  await loginAs(page, `e2e-login-user-${Date.now()}`);

  await expect(page).toHaveURL(/\/dashboard$/);
  await expect(page.getByRole('heading', { name: 'Your Zelda activity at a glance' })).toBeVisible();

  await page.getByRole('button', { name: 'Logout' }).click();

  await expect(page).toHaveURL(/\/login$/);
  await expect(page.getByRole('heading', { name: 'Sign in with Google' })).toBeVisible();
});

test('signed-in user can add a game to the collection and update its status', async ({ page }) => {
  await loginAs(page, `e2e-collection-user-${Date.now()}`, '/games');

  await expect(page).toHaveURL(/\/games$/);
  await page.goto('/games/the-legend-of-zelda');
  await expect(page).toHaveURL(/\/games\/the-legend-of-zelda$/);
  await page.getByRole('button', { name: 'Add to my collection' }).click();
  await page.getByRole('link', { name: 'Open my collection' }).click();

  await expect(page).toHaveURL(/\/my-collection$/);
  await expect(page.getByRole('heading', { name: 'The Legend of Zelda' })).toBeVisible();

  const collectionCard = page.locator('.member-page__item-card').filter({
    has: page.getByRole('heading', { name: 'The Legend of Zelda' }),
  });

  await collectionCard.getByRole('combobox', { name: 'Status' }).selectOption('Completed');
  await collectionCard.getByRole('button', { name: 'Save status' }).click();

  await expect(page.getByText('Current status: Completed')).toBeVisible();
});

test('signed-in user can create, edit, and delete a comment', async ({ page }) => {
  await loginAs(page, `e2e-comment-user-${Date.now()}`, '/games');

  await page.getByRole('link', { name: /The Legend of Zelda/i }).first().click();

  const originalComment = `E2E comment ${Date.now()}`;
  const updatedComment = `${originalComment} updated`;

  await page.getByRole('textbox').fill(originalComment);
  await page.getByRole('button', { name: 'Post' }).click();
  await expect(page.getByText(originalComment)).toBeVisible();

  await page.getByRole('button', { name: 'Edit' }).first().click();
  await page.getByRole('textbox').last().fill(updatedComment);
  await page.getByRole('button', { name: 'Save edit' }).click();
  await expect(page.getByText(updatedComment)).toBeVisible();

  await page.getByRole('button', { name: 'Delete' }).first().click();
  await expect(page.getByText(updatedComment)).not.toBeVisible();
});
