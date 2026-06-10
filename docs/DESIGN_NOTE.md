# Design Note

I wanted The Hyrule Archives to feel like a finished Zelda fan product instead of a generic assignment dashboard. My design goal was to blend three ideas together:

1. a readable archival layout
2. a community-driven interface
3. a visual identity that clearly belongs to the Zelda space without copying Nintendo's official sites

## Product feel

I treated the app as a community archive first and a CRUD app second. That meant the interface needed to communicate:

- history and curation
- activity and momentum
- ownership for signed-in users

I used public-facing pages to make the product feel populated and then used protected member pages to make the signed-in experience feel personal.

## Layout strategy

I kept the layout structure simple on purpose.

- A shared shell gives the app consistent navigation and auth-aware actions.
- Public routes focus on discovery, trends, and game detail pages.
- Protected routes focus on the user's own collection, favorites, activity, and live updates.

This split helped me avoid mixing public browsing with private account workflows in the same screens.

## Visual system

My styling choices aimed for a polished but readable presentation.

### Color and contrast

I used a consistent dark-on-light content treatment so long summaries, metadata blocks, and community cards remain easy to scan. I also tightened text contrast in the game detail page so the title, summary, and section labels read more confidently.

### Spacing and hierarchy

I used repeated spacing tokens and grouped content into cards, sections, and callout blocks. That gave the app a cleaner sense of rhythm and made it easier to distinguish:

- title and summary content
- metadata and release details
- community actions
- recent comments and activity

### Imagery

The game detail page is the most image-heavy screen, so I spent the most time there. I standardized the image sizing around the stronger Wind Waker reference layout and then increased the image footprint so the art carries more visual weight. I also removed the decorative frame treatment around the hero art because it distracted from the actual box art.

## Route design decisions

I wanted route names to be plain and predictable.

### Public routes

- `/games` for discovery
- `/games/:gameSlug` for detail pages
- `/trends` for community-wide metrics

### Protected routes

- `/dashboard` for signed-in summary
- `/my-collection` for collection management
- `/my-favorites` for favorite-only filtering
- `/my-activity` for personal history
- `/community/live` for the SignalR feed

These routes read naturally and map closely to user intent.

## Reusable UI patterns

I made reuse a design priority because it improves both consistency and maintenance.

### Reused forms

- `CommentEditorForm` handles both comment creation and comment editing
- `CollectionStatusForm` handles collection status changes, favorite toggles, and delete actions

### Reused content blocks

- `LiveActivityFeed`
- auth guard behavior through `RequireAuth`
- shared member page styling in `MemberPages.css`

This kept the protected experience visually unified.

## Accessibility-minded design choices

I tried to make accessibility part of the design instead of a cleanup step.

- Semantic headings and section structure
- Buttons and links with clear labels
- Form validation surfaced inline near the field
- Alt text for image-heavy views
- Mobile-friendly layouts that avoid forcing horizontal scroll
- Keyboard-friendly actions for forms and navigation

## What I changed late in the project

Late in the build, I made a focused polish pass on the game detail page because it is the page that best sells the product.

I updated it to:

- enlarge the hero and supporting images
- center the “Secondary Consoles” and “Downloaded Ports” section headings
- remove image borders so the art stands on its own
- darken important text so the content has stronger visual hierarchy

That pass made the detail page feel much more intentional.

## Design tradeoffs

- I chose readability over flashy animation.
- I preferred simple custom CSS over introducing a heavy component framework late in the project.
- I kept the visual language cohesive across public and protected pages instead of treating each route like its own mini app.
- I prioritized the game detail page, dashboard, and collection views because those are the pages most likely to be shown during evaluation.

## If I keep iterating

If I continue this project, my next design steps would be:

- add a stronger empty-state illustration system
- improve chart-style visualizations on the trends page
- add a theme token layer for easier palette tuning
- build a more explicit mobile nav treatment for smaller viewports
