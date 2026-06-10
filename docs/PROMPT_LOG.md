# Prompt Log

I used GitHub Copilot throughout this project as a pair-programming assistant, but I kept the final engineering decisions on my side. This log captures representative prompts and the kinds of tasks I used AI for.

## How I used AI

I mostly used AI for four kinds of work:

1. speeding up routine implementation
2. exploring refactor options
3. generating first drafts of tests and docs
4. helping me reason through UI polish and requirement coverage

I did **not** treat AI output as automatically correct. I still read the code, edited it, and ran tests before keeping changes.

## Representative prompts

### 1. Assignment audit

**Prompt:** Review the project and tell me what is still missing relative to the assignment requirements.

**Why I used it:** I wanted a structured gap analysis before I started my final completion push.

**What I kept:** The audit was useful because it highlighted missing protected routes, missing docs, missing CI, and missing frontend testing coverage.

**What I changed:** I did not accept the audit blindly. I verified the route counts, endpoint counts, and missing files against the repository before I started implementing changes.

---

### 2. Protected member pages

**Prompt:** Add the remaining protected React routes and build pages for my collection, favorites, activity, and a live feed.

**Why I used it:** This helped me move quickly through a large amount of boilerplate while keeping the route structure coherent.

**What I kept:** The overall direction and the component split were useful.

**What I changed:** I rewrote parts of the page logic so the routes matched my own naming, voice, and UI goals.

---

### 3. Reusable forms and validation

**Prompt:** Refactor comments and collection actions into reusable form components with inline validation.

**Why I used it:** The assignment required reusable components and client-side validation, so this was a good place to combine both.

**What I kept:** The initial component extraction idea.

**What I changed:** I adjusted the props, validation behavior, and wording so the forms matched my app instead of feeling auto-generated.

---

### 4. SignalR frontend integration

**Prompt:** Wire a React hook and display component to the existing SignalR activity hub.

**Why I used it:** I already had the backend hub, so I used AI to speed up the client integration.

**What I kept:** The basic connection pattern and reconnect logic.

**What I changed:** I made sure the feed stayed capped, deduplicated, and aligned with my page structure.

---

### 5. Testing setup

**Prompt:** Set up Vitest, Testing Library, Playwright, and a small set of tests that cover protected flows and form validation.

**Why I used it:** I needed to close a testing gap quickly but still meet the assignment's behavioral testing requirements.

**What I kept:** The general setup pattern and the first-draft tests.

**What I changed:** I fixed configuration issues, separated unit tests from E2E tests, and re-ran everything until the suite passed.

---

### 6. Documentation drafting

**Prompt:** Write the missing Markdown files in my voice and keep them accurate to the codebase.

**Why I used it:** Documentation is important, but writing six project docs from scratch is time-consuming.

**What I kept:** The structure and first-draft wording.

**What I changed:** I edited the docs to stay honest about what is complete, what is configurable, and how I actually used AI.

## Times I rejected or revised AI output

I had multiple moments where I had to step in and correct the output.

- A styling change initially targeted too many images instead of just the game detail page assets.
- One patch to the game detail page conflicted with the existing file and had to be rewritten cleanly.
- My first frontend test run failed because the Playwright spec was being included in the Vitest run. I fixed the config and re-ran the suite.
- I corrected outdated product-brief wording that still referenced RAWG instead of MobyGames.

## Verification steps I took after AI-generated changes

After major AI-assisted edits, I verified the result by:

- reading the changed files directly
- checking for editor/build errors
- running frontend unit tests
- running backend tests
- building the frontend production bundle
- running end-to-end tests on critical signed-in flows
- reviewing accessibility and dependency-security results

## My takeaway

AI helped me move faster, especially on repetitive implementation and first drafts, but the project only held together because I kept validating, trimming, rewriting, and testing the output. I used Copilot as a force multiplier, not as a substitute for engineering judgment.
