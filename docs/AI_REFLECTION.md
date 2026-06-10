# AI Reflection

GitHub Copilot made this project materially faster to build, but it did not remove the need for engineering judgment. The best way I can describe my workflow is that AI helped me reach a decent first draft quickly, and then I had to do the real work of shaping, verifying, and correcting that draft.

## Where AI helped the most

### 1. Scaffolding repetitive code

AI was most helpful when I already knew the destination and wanted help getting through the repetitive middle.

Examples:

- route scaffolding
- basic DTO and endpoint patterns
- form extraction into reusable components
- test boilerplate
- Markdown first drafts

That kind of work still required review, but it saved me time.

### 2. Requirement coverage

The final project has a long checklist. AI was useful for turning that checklist into concrete implementation tasks. It helped me identify where the project was already strong and where it was still thin, especially around:

- protected routes
- frontend tests
- end-to-end coverage
- CI/CD setup
- documentation

### 3. Refactoring support

AI was also useful when I wanted to improve reuse without losing momentum. For example, splitting comment handling into `CommentEditorForm` and collection management into `CollectionStatusForm` gave me cleaner code and helped satisfy the assignment's reuse requirements.

## Where AI was weak

### 1. Context drift

The most obvious weakness was that AI sometimes generalized too aggressively. A good example was the image-sizing request on the game detail page. The intent was specific, but the first pass was broader than I wanted.

### 2. Accuracy under changing code

When files were changing quickly, AI sometimes produced patches that did not fit the exact file state. I had to catch that, stop, and rewrite sections carefully.

### 3. Configuration details

Tooling setup still required human attention. My first frontend test run failed because Playwright specs were being discovered by Vitest. AI got me close, but I still had to debug the configuration and fix it.

## How I kept AI use responsible

I tried to be disciplined about verification.

I did not keep AI-generated changes unless I could support them with one or more of the following:

- direct file review
- a passing test
- a passing build
- a working browser flow
- a security or accessibility check

I also kept secrets out of the repo and made sure protected flows were enforced server-side, not just in React.

## Concrete examples of human oversight

- I corrected stale product language so the docs matched the actual MobyGames integration.
- I verified that protected routes and protected endpoints actually met the assignment minimums.
- I reran backend tests after adding new auth and authorization coverage.
- I separated unit and E2E test tooling correctly after the first combined run failed.
- I reviewed vulnerability output instead of ignoring the package-manager warning.

## What I learned

The biggest lesson for me is that AI is strongest when I give it a narrow, well-framed job. It is much less reliable when I let it make assumptions about the product on its own.

For this project, the best pattern was:

1. I define the goal clearly.
2. AI gives me a draft.
3. I review and reshape it.
4. I validate it with tooling.
5. I keep only the parts that survive that validation.

## Final reflection

I do think AI made me more productive on this project. It helped me cover more ground, especially late in the assignment when there were still multiple missing pieces to close. At the same time, the project is only coherent because I treated every AI response as something to inspect, not something to trust automatically.
