# Accessibility Report

I wanted accessibility to be part of the build instead of something I only talked about in the README. This report captures the checks I focused on, the tooling I used, and the issues I watched most closely in an image-heavy React app.

## Accessibility goals

For this project, my baseline accessibility goals were:

- semantic page structure
- keyboard-usable navigation and forms
- descriptive labels on interactive controls
- inline validation messaging
- meaningful alternative text for imagery
- responsive layouts that stay usable on smaller screens

## What I reviewed

I focused on the pages most likely to be used in a demo and the components most likely to create accessibility regressions.

### Key screens

- landing/login flow
- games listing
- game detail page
- my collection page
- live activity page

### Key reusable components

- `RequireAuth`
- `CommentEditorForm`
- `CollectionStatusForm`
- navigation inside `AppLayout`
- live activity list rendering

## Automated checks

I added Playwright-based accessibility coverage using axe so I could scan rendered pages instead of relying only on code review.

### Automated scan coverage

- public landing page
- public games page
- signed-in collection page using the development test-login helper

### Result summary

My current automated accessibility scan is set to fail on **critical** axe violations. During the audit, axe also surfaced additional **serious** color-contrast findings on some screens. I fixed the shared shell contrast issues that were directly in front of the auth and footer controls, and I documented the remaining contrast work as follow-up polish instead of pretending the UI is perfect.

## Manual review areas

I also did manual checks because not every real usability problem is caught by automated tooling.

### Manual checks I focused on

- heading order and section clarity
- keyboard access to buttons, links, and form submission
- visible validation feedback near the edited field
- clarity of button labels such as Edit, Delete, Save status, and Logout
- image alternative text on box art and platform imagery
- mobile readability on detail and member pages

## Project strengths

### 1. Route and content structure

The route structure is simple and predictable, which helps both navigation and mental mapping.

### 2. Reusable forms with inline validation

The comment and collection forms surface validation messages in context instead of silently failing.

### 3. Protected-route messaging

`RequireAuth` has explicit loading and redirect behavior rather than leaving the user on a blank screen.

### 4. Responsive layout work

The main views are designed to remain usable on desktop and mobile-sized screens.

## Known watch areas

These are the parts of the app I would keep watching as the UI evolves.

1. **Image-heavy pages:** the game detail page uses a lot of art, so alt text quality matters.
2. **Contrast drift during polish:** design tweaks can accidentally reduce readability if I am not careful, and axe already caught a few serious color-contrast cases that I still want to tighten.
3. **Realtime updates:** the live feed is useful, but I would like to add more explicit announcement patterns if I extend it further.
4. **Future charts or trend visualizations:** richer data views often need extra accessibility attention.

## Next improvements I would make

If I keep iterating, I would add:

- a skip link in the main layout
- more explicit focus styling tokens across custom buttons
- an ARIA live-region strategy for the SignalR feed
- broader automated accessibility coverage across more routes

## Bottom line

The app is in a good place for baseline accessibility: it uses semantic structure, labels, inline validation, and keyboard-friendly flows, and I now have automated axe coverage guarding against serious regressions on key routes. I would still continue improving realtime announcements and focus treatment if I keep building on it.
