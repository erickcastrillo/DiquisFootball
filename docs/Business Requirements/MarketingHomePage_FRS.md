## Web Platform: Marketing Site (diquis.com)

### 1. Strategic Goals
- **Primary Goal (Self-Service Conversion):** Convert academies and clubs with fewer than 1,000 players to begin a self-service free trial or paid plan for Tiers 1 and 2.
- **Secondary Goal (Enterprise Funnel):** Funnel large clubs, leagues, and federations with over 1,000 players into a scheduled sales demo for a Tier 3 (Enterprise) solution.

### 2. Functional Sections (The Scroll Journey)

#### Section A: The Hero (Above the Fold)
This section must immediately capture **Attention** and establish the core value proposition for both primary personas.

- **Content:**
    - **H1 Headline:** `The All-in-One Platform for Football Excellence.`
    - **Sub-headline:** `From local clubs to national federations, Diquis provides the tools to manage, develop, and scale your talent.`
- **Interactive Element: "Persona Toggle"**
    - A simple, two-option toggle will be displayed prominently under the sub-headline:
        - **Option 1 (Default):** "For Academies & Clubs"
        - **Option 2:** "For Leagues & Federations"
    - **UX Logic:** When the user selects an option, the background hero image and CTAs subtly adjust.
        - *Academies & Clubs:* Shows a background image of a coach on a training pitch using a tablet. The "Start Free Trial" CTA is emphasized.
        - *Leagues & Federations:* Shows a background image of a modern stadium operations center with data analytics on screens. The "Book Enterprise Demo" CTA is emphasized.
- **CTAs (Call to Action):**
    - **Primary CTA:** A solid, high-contrast button labeled `Start Your Free Trial`. This links directly to the Tier 1/2 self-service signup page.
    - **Secondary CTA:** A ghost/outline button labeled `Book Enterprise Demo`. This links to the sales team's calendar booking tool (e.g., Calendly, HubSpot Meetings).

#### Section B: The "Chaos vs. Control" Visual
This section is designed to create **Interest** by visualizing the core problem and solution.

- **Concept:** A full-width visual element split into two halves.
    - **Left Side ("Chaos"):** A collage of disorganized elements representing the "old way": spreadsheet icons, chaotic WhatsApp message bubbles, scattered paper forms, and a tangled mess of stopwatches.
    - **Right Side ("Control"):** A clean, elegant screenshot of the main Diquis dashboard, showing organized player profiles, a clear calendar, and insightful analytics charts.
- **UX Requirement:**
    - The two halves are separated by a vertical bar with a draggable slider handle.
    - The user can click and drag this handle left or right. As they drag it to the right, it "wipes" away the chaos, revealing the organized Diquis dashboard underneath. This provides a satisfying, interactive demonstration of the product's value.

#### Section C: The Module Grid (Feature Discovery)
This section builds **Desire** by showcasing the breadth and depth of the platform's capabilities.

- **Logic:** A 3x3 grid of interactive cards, each representing a core product module.
    - Examples: `Academy Operations`, `Player Management`, `Sports Medicine`, `Scouting & Recruitment`, `Analytics & Reporting`, `Team Organization`, etc.
- **Interactivity:**
    - On mouse hover, the static icon on the card is replaced by a 5-second, auto-playing, silent micro-loop video that shows the corresponding feature in action (e.g., hovering on "Sports Medicine" shows a user updating a player's return-to-play status).
- **Requirement:**
    - To drive upsells and manage expectations, specific cards must be tagged with a small, non-intrusive badge.
        - `Sports Medicine & Bio-Passport` card gets a "Professional Tier" badge.
        - `Scouting & Recruitment` card gets a "Professional Tier" badge.
        - A future "API Access" or "Custom Integrations" card would get an "Enterprise Tier" badge.

#### Section D: The "Right-Size" Pricing Calculator
This section is designed to transparently guide users to the correct tier and CTA, solidifying **Desire** and leading to **Action**.

- **Logic:** An interactive pricing section, not a static table.
    - **Input:** A prominent slider element labeled: `How many players do you manage?`
        - The slider has defined steps: `1-50`, `51-250`, `251-500`, `501-1,000`, `1,000+`.
    - **Output:** Below the slider, three cards represent the `Grassroots (Tier 1)`, `Professional (Tier 2)`, and `Enterprise (Tier 3)` plans. As the user adjusts the slider, the cards dynamically update:
        - **`1-50`:** The "Grassroots" card is highlighted with a "Recommended" badge, showing its monthly price.
        - **`51-1,000`:** The "Professional" card is highlighted, showing its monthly price.
        - **`1,000+`:** The "Enterprise" card is highlighted. The price is replaced with the text "Custom Pricing," and its CTA changes from "Get Started" to `Contact Sales`.

#### Section E: Trust & Compliance (The "FIFA Standard")
This final section builds trust and credibility, especially for the enterprise persona.

- **Content:** A clean, professional footer section with a heading like "The Professional Standard in Football Technology". It must contain:
    - **Logos:** A row of "Trusted By" logos (initially using placeholder partner logos like "National Soccer Coaches Association," "American Youth Soccer Organization").
    - **Compliance Badges:** A set of high-quality vector icons for `GDPR Ready` and `HIPAA Compliant`, reassuring enterprise clients about data security, particularly for the Sports Medicine module.
    - **Social Proof:** A quote from a key figure, e.g., *"Diquis has become the central nervous system of our academy." - Jane Doe, Academy Director, Premier League Youth Team.*

### 3. Non-Functional Requirements
- **SEO & Performance:** The page must achieve a Google PageSpeed Insights score of 90+ for Mobile. All images must be compressed and served in modern formats (e.g., WebP). Total page weight should not exceed 1.5MB.
- **Mobile Responsiveness:** The "Right-Size" Pricing Calculator must be specifically optimized for mobile. The slider will be at the top, and the three tier cards will stack vertically beneath it. As the slider is adjusted, the view will automatically scroll to the highlighted, recommended tier card.
- **Analytics:** The site must be configured with event tracking for all key user interactions. This includes:
    - Clicks on the Primary vs. Secondary hero CTAs.
    - Engagement with the "Chaos vs. Control" slider (tracked as a conversion event).
    - Final position of the pricing calculator slider upon CTA click.
    - Scroll depth tracking (via Hotjar or Google Analytics) to identify user drop-off points.

### 4. Acceptance Criteria
- [ ] The "Persona Toggle" in the hero section correctly adjusts the background image.
- [ ] The Primary CTA ("Start Your Free Trial") and Secondary CTA ("Book Enterprise Demo") are visually distinct and link to the correct URLs.
- [ ] The "Chaos vs. Control" interactive slider is functional and smooth.
- [ ] All module cards in the 3x3 grid show a video on hover.
- [ ] The "Sports Medicine" module card correctly displays its "Professional Tier" badge.
- [ ] The pricing calculator slider correctly highlights the "Enterprise" card and shows the "Contact Sales" button when moved to "1,000+".
- [ ] The mobile view of the pricing calculator stacks the cards vertically and scrolls to the correct card.
- [ ] The page achieves a production Google PageSpeed score of > 90 for mobile.
- [ ] All specified analytics events (CTA clicks, slider engagement) are verified as firing correctly in Google Analytics.
