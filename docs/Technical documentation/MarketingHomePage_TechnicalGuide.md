# Technical Implementation Guide: Marketing Home Page

This document provides a detailed technical guide for implementing the public-facing Marketing Home Page as outlined in the corresponding Functional Requirement Specification (FRS).

## 1. Architectural Analysis

The Marketing Home Page is a distinct entity from the core Diquis SaaS application. Its primary goals are SEO, high performance, and rich, anonymous user interaction to drive conversions. The existing Nano ASP.NET Boilerplate is optimized for a data-driven, authenticated SPA and is not the appropriate architecture for a public marketing site.

Therefore, the marketing site will be developed as a **separate, dedicated Next.js (React) application**.

**Justification:**
-   **SEO & Performance:** Next.js provides Server-Side Rendering (SSR) and Static Site Generation (SSG) out-of-the-box, which are critical for achieving the Google PageSpeed score of 90+ and ensuring fast initial load times as required by the FRS.
-   **Development Experience:** It allows for a modern, component-based frontend architecture using React, which aligns with the technology stack of the core application.
-   **Decoupling:** A separate application ensures that marketing content can be updated and deployed independently of the core SaaS platform, increasing agility and reducing risk.

### Domain Entities
-   **None.** This is a stateless presentation-layer application. All content is static or managed within the frontend project.

### Multi-Tenancy Scope
-   **Not Applicable.** The marketing site is for anonymous, pre-conversion users and exists entirely outside of the multi-tenancy context.

### Permissions
-   **Not Applicable.** All pages and components are public and do not require authorization.

## 2. Scaffolding Strategy (CLI)

The .NET scaffolding tools are not applicable here. A new Next.js project will be initialized.

1.  **Initialize Next.js Project:**
    *From the root of the main repository, a new `marketing-site` directory should be created.*
    ```bash
    npx create-next-app@latest marketing-site --typescript --tailwind --eslint
    ```
    This command sets up a new Next.js project with TypeScript and Tailwind CSS for styling, which is ideal for rapid, modern UI development.

## 3. Implementation Plan (Agile Breakdown)

### User Story: Hero Section & Persona Targeting
**As a** potential customer, **I want to** immediately understand the product's value for my specific context (Club vs. Federation), **so that** I feel the platform is tailored to my needs.

**Technical Tasks:**
1.  **Component Structure (React):**
    -   `src/components/sections/HeroSection.tsx`: The main container for this section.
    -   `src/components/ui/PersonaToggle.tsx`: The "For Academies & Clubs" / "For Leagues & Federations" toggle component.
    -   `src/components/ui/CtaButton.tsx`: A reusable button component for the primary and secondary CTAs.
2.  **State Management:**
    -   Within `HeroSection.tsx`, use `useState` to manage the currently selected persona: `const [persona, setPersona] = useState('club');`
3.  **Interactivity:**
    -   The `PersonaToggle` component will take `persona` and `setPersona` as props and update the state `onClick`.
    -   The `HeroSection` will conditionally render CSS classes or background image components based on the `persona` state.
4.  **API:** No backend API calls are required.
5.  **UI (React/Client):**
    -   Implement two distinct background images, conditionally applied based on the persona state.
    -   The primary CTA (`Start Your Free Trial`) will be visually dominant (solid background).
    -   The secondary CTA (`Book Enterprise Demo`) will be an outline/ghost button.
    -   Analytics: Fire a tracking event in the `onClick` handler for each CTA button to differentiate clicks.

### User Story: "Chaos vs. Control" Interactive Visualization
**As a** visitor, **I want to** interactively see the "before and after" of using Diquis, **so that** I can grasp the value proposition in a memorable way.

**Technical Tasks:**
1.  **Component Structure (React):**
    -   `src/components/sections/ChaosControlSlider.tsx`: The main component for this section.
2.  **Library Integration:** To achieve the draggable slider effect, integrate a specialized third-party library like `react-compare-slider`.
    -   *Installation:* `npm install react-compare-slider`
3.  **Content:**
    -   The "Chaos" image will be a pre-designed static asset (`.webp` format).
    -   The "Control" image will be a high-quality screenshot of the Diquis application dashboard.
4.  **Interactivity:** The `react-compare-slider` library handles the drag-and-wipe logic.
5.  **Analytics:** Configure analytics to fire an event when the user has engaged with the slider (e.g., on `onDragEnd` or a similar callback).

### User Story: Feature Discovery via Module Grid
**As a** potential customer, **I want to** quickly explore the key features of the platform, **so that** I can see if it meets my needs.

**Technical Tasks:**
1.  **Component Structure (React):**
    -   `src/components/sections/ModuleGrid.tsx`: The container for the 3x3 grid.
    -   `src/components/ui/ModuleCard.tsx`: An individual card for a single module.
2.  **Content:**
    -   Each `ModuleCard` will receive props for its `title`, `icon`, `microLoopVideoUrl`, and an optional `tierBadge` (e.g., "Professional Tier").
3.  **State Management & Interactivity:**
    -   Within `ModuleCard.tsx`, use a `useState` hook (`const [isHovering, setIsHovering] = useState(false);`) controlled by `onMouseEnter` and `onMouseLeave` events.
    -   Conditionally render the static icon or the auto-playing `<video>` element based on the `isHovering` state. The video tag should include the `autoPlay`, `loop`, and `muted` attributes.
4.  **UI (React/Client):**
    -   The card for "Sports Medicine" must receive the `tierBadge="Professional Tier"` prop to render the badge correctly.

### User Story: Interactive Pricing Calculator
**As a** prospective buyer, **I want to** get a clear pricing recommendation based on my size, **so that** I can make a quick decision.

**Technical Tasks:**
1.  **Component Structure (React):**
    -   `src/components/sections/PricingCalculator.tsx`: The main component holding the slider and cards.
    -   `src/components/ui/PlayerSlider.tsx`: The slider input component.
    -   `src/components/ui/PricingCard.tsx`: A card representing one of the three tiers.
2.  **State Management:**
    -   In `PricingCalculator.tsx`, use a `useState` hook to manage the player count: `const [playerCount, setPlayerCount] = useState(50);`
3.  **Logic:**
    -   The `PlayerSlider`'s `onChange` event will update the `playerCount` state.
    -   The `PricingCalculator` component will contain logic that determines the recommended tier based on the `playerCount`.
    -   Each `PricingCard` will receive an `isRecommended={true/false}` prop based on this logic, which will control its styling (e.g., adding a border or a "Recommended" badge).
    -   The "Enterprise" `PricingCard` will conditionally render "Custom Pricing" and a "Contact Sales" button when the `playerCount` is `1000+`.
4.  **UI (React/Client):**
    -   Use Tailwind CSS's responsive modifiers (`md:`, `lg:`) to ensure the cards stack vertically on mobile and are in a row on desktop.
    -   Implement the "scroll-to-highlight" feature on mobile using `useEffect` and `element.scrollIntoView()` when the `playerCount` state changes.

## 4. Code Specifications (Key Logic)

### `PersonaToggle.tsx` - State and Conditional Styling

```typescript
// Simplified pseudo-code for the hero section
import { useState } from 'react';

type Persona = 'club' | 'federation';

export default function HeroSection() {
    const [persona, setPersona] = useState<Persona>('club');

    const isClub = persona === 'club';

    return (
        <div className={isClub ? 'bg-hero-club' : 'bg-hero-federation'}>
            {/* Persona Toggle Buttons */}
            <div>
                <button onClick={() => setPersona('club')} className={isClub ? 'active' : ''}>
                    For Academies & Clubs
                </button>
                <button onClick={() => setPersona('federation')} className={!isClub ? 'active' : ''}>
                    For Leagues & Federations
                </button>
            </div>

            {/* Conditional CTAs */}
            <div>
                <CtaButton primary={isClub} href="/signup">Start Free Trial</CtaButton>
                <CtaButton primary={!isClub} href="/book-demo">Book Enterprise Demo</CtaButton>
            </div>
        </div>
    );
}
```

### `PricingCalculator.tsx` - Slider Logic

```typescript
// Simplified pseudo-code for the pricing calculator
import { useState, useMemo } from 'react';

export default function PricingCalculator() {
    // Value could represent steps: 0=50, 1=250, 2=500, 3=1000, 4=1000+
    const [sliderValue, setSliderValue] = useState(0); 

    const recommendedTier = useMemo(() => {
        if (sliderValue >= 4) return 'Enterprise';
        if (sliderValue >= 1) return 'Professional';
        return 'Grassroots';
    }, [sliderValue]);

    return (
        <div>
            <input 
                type="range" 
                min="0" 
                max="4" 
                step="1"
                value={sliderValue}
                onChange={(e) => setSliderValue(Number(e.target.value))}
            />

            <div className="flex flex-col md:flex-row">
                <PricingCard 
                    title="Grassroots" 
                    isRecommended={recommendedTier === 'Grassroots'} 
                />
                <PricingCard 
                    title="Professional" 
                    isRecommended={recommendedTier === 'Professional'} 
                />
                <PricingCard 
                    title="Enterprise" 
                    isRecommended={recommendedTier === 'Enterprise'}
                    isEnterprise={true} // To handle special text/CTA logic
                />
            </div>
        </div>
    );
}
```

### `ModuleCard.tsx` - Hover Video Effect

```typescript
import { useState } from 'react';

interface ModuleCardProps {
    title: string;
    iconUrl: string;
    videoUrl: string;
    tierBadge?: string;
}

export default function ModuleCard({ title, iconUrl, videoUrl, tierBadge }: ModuleCardProps) {
    const [isHovering, setIsHovering] = useState(false);

    return (
        <div 
            className="relative"
            onMouseEnter={() => setIsHovering(true)}
            onMouseLeave={() => setIsHovering(false)}
        >
            {isHovering ? (
                <video src={videoUrl} autoPlay loop muted className="absolute inset-0 w-full h-full object-cover" />
            ) : (
                <img src={iconUrl} alt={`${title} icon`} className="w-full h-full object-contain" />
            )}
            
            <h3 className="absolute bottom-4 left-4 text-white">{title}</h3>

            {tierBadge && (
                <span className="absolute top-2 right-2 bg-blue-500 text-white text-xs px-2 py-1 rounded">
                    {tierBadge}
                </span>
            )}
        </div>
    );
}
```
