## Module 16-A: 🤖 External AI Agents (Sales & Support)

### 1. Executive Summary
This document outlines the requirements for an AI-powered chatbot deployed on the public Diquis marketing site. This AI agent will serve as a "first responder" to visitor inquiries, providing instant, 24/7, multi-lingual support with the primary goals of qualifying leads for the sales team and guiding prospective customers to the correct self-service channels.

### 2. Key Actors
- **`Website Visitor`**: Any anonymous user browsing the public `diquis.com` website.
- **`Prospective Client`**: A website visitor who has demonstrated intent to purchase by engaging with the AI agent about pricing or features.
- **`Sales Agent`**: An internal Diquis team member who receives qualified leads and conversation transcripts from the AI agent.

### 3. Functional Capabilities

#### Feature: The Context-Aware Sales Concierge
- **User Story:** "As a Visitor exploring the Diquis website, I want to get immediate, accurate answers about pricing and features without having to read through the entire FAQ or wait for a human."
- **Detailed Business Logic:**
    - **Knowledge Base Constraint:** The AI agent's responses **must** be strictly constrained to an approved knowledge base. This knowledge base will consist of the content from the official "Pricing" page, the "Features" page, and the public-facing FRS documents (Modules 1-10). The AI's system prompt must explicitly forbid it from answering general knowledge questions or any questions unrelated to the Diquis product.
        - *Example Refusal:* If asked, "Who is the best football player in the world?", the AI must respond with a polite and firm refusal, such as: "I can only provide information about the Diquis platform. How can I help you with our features or pricing?"
    - **Lead Routing Logic:** The AI must be programmed to detect user intent and route the conversation accordingly.
        - **Enterprise Path:** If a user's query contains keywords like `Enterprise`, `Custom`, `Federation`, `API`, `HIPAA`, `dedicated`, or mentions managing a large number of players (e.g., "more than 1,000"), the AI's primary goal is to capture the lead. It will respond with: *"It sounds like our Enterprise plan is the right fit for your needs. A specialist can walk you through our security, compliance, and custom features. What is the best email to reach you at to schedule a demo?"*
        - **Self-Service Path:** If a user's query contains keywords like `price`, `cost`, `start now`, `free trial`, or mentions a small number of players, the AI's goal is to drive self-service conversion. It will answer the question using the knowledge base and provide a direct, clickable link to the self-service signup page.
    - **Tone:** The AI agent's persona must be professional, helpful, and concise. It should project expertise and confidence without using overly casual language, slang, or emojis.

#### Feature: Real-Time Language Auto-Switching
- **User Story:** "As a Spanish-speaking Academy Director from Mexico, I want to be able to ask questions in my native language on the website and receive immediate, fluent answers in Spanish, even if it's outside of normal business hours."
- **Detailed Business Logic:**
    - **Detection:** The AI system must have a language detection layer that analyzes the user's initial input. When it receives a prompt such as, *"Hola, ¿cuánto cuesta el plan Profesional?"*, it must identify the language as Spanish (`es`).
    - **Action:** Once the language is detected, the AI Agent **must** immediately switch its response language to match. It will conduct the entire remainder of the conversation in Spanish, while still adhering to the same knowledge base and lead routing logic. The tone and persona remain consistent, just localized.
    - **Handover:** If the conversation needs to be escalated to a human `Sales Agent` (either by user request or because the AI cannot fulfill the query), the system must perform the following actions:
        1.  Capture the user's contact information (if not already done).
        2.  Create a ticket in the sales CRM (e.g., Salesforce, HubSpot).
        3.  Attach the **full conversation transcript** to the ticket in two formats: the original Spanish version for accuracy, and an automated, machine-translated English version for the convenience of the English-speaking sales agent.

### 4. Acceptance Criteria
- [ ] The bot successfully and politely refuses to answer off-topic questions, such as "What is the weather today?".
- [ ] The bot correctly identifies the keyword "Federation" in a user query and responds by asking for an email address to book a demo.
- [ ] The bot correctly answers a question about the price of the "Grassroots" plan and provides a direct link to the self-service signup page.
- [ ] If a user starts a conversation in English and then asks a question in Spanish, the bot seamlessly switches to responding in Spanish for the remainder of the chat.
- [ ] When a Spanish conversation is escalated, the generated sales ticket contains both the original Spanish transcript and its English machine translation.
