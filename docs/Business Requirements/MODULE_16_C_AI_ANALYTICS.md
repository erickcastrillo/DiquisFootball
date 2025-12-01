## Module 16-C: 📊 Predictive AI (The Revenue Guardian)

### 1. Executive Summary
This document specifies the requirements for a predictive analytics module that leverages machine learning to provide actionable financial insights. By analyzing historical data patterns, the "Revenue Guardian" proactively identifies players at risk of churning and provides a 90-day cash flow forecast to help owners mitigate risk and optimize financial planning.

### 2. Key Actors
- **`Academy Owner`**: The primary consumer of the predictive insights, using the data to make strategic decisions about retention and spending.
- **`Financial Admin`**: Uses the cash flow forecast to manage budgets and anticipate future financial positions.

### 3. Functional Capabilities

#### Feature: Player Churn Prediction Engine
- **User Story:** "As an Academy Owner, I'm often surprised when a family decides to leave. I want the system to alert me which players are likely to quit *before* they make the decision, so I have a chance to intervene and improve their experience."
- **Detailed Business Logic:**
    - **Data Signals:** The AI model runs as a nightly batch process, analyzing every active player against a set of key behavioral and financial indicators. The model is trained on historical data to correlate these signals with actual churn events. The three primary signals are:
        1.  **Attendance Signal (from Module 4):** The system calculates the player's attendance rate over the last 30 days. A rate below 50% is a strong negative signal.
        2.  **Financial Signal (from Module 1):** The system checks the status of the family's latest invoice. A payment that is more than 5 days overdue is a moderate negative signal.
        3.  **Engagement Signal (from User Logs):** The system checks the `last_login_date` for all associated `Parent` accounts. If no parent has logged into the web or mobile app in the last 60 days, it is a weak negative signal.
    - **Output:** The results are displayed in a dashboard widget titled "Players at Risk of Churn". The model assigns a simple, interpretable "Risk Score" to each player who flags on at least one signal:
        - **High Risk:** Player flags on 2 or more signals (e.g., poor attendance AND late payment).
        - **Medium Risk:** Player flags on 1 strong or moderate signal (e.g., poor attendance).
        - **Low Risk:** Players with no flags are not shown in this widget.
    - **Action:** For each player listed in the widget, a "Take Action" button is available. Clicking this button opens a modal window containing an AI-generated "Re-engagement Script" that is empathetic and designed to open a conversation. The owner can easily copy this script for use in WhatsApp, SMS, or email.
        - *Example Script:* `"Hi [Parent's Name], this is [Owner's Name] from the academy. I was reviewing our records and wanted to check in to make sure everything is going well for [Player's Name]. We really value having you as part of our community and would love to hear any feedback you have. Hope to see you at the pitch soon!"`

#### Feature: Cash Flow Forecaster
- **User Story:** "As an Academy Owner, my cash flow is unpredictable, especially around the holidays. I need a tool that can forecast my likely bank balance over the next three months so I know if I can afford to purchase new team uniforms in December."
- **Detailed Business Logic:**
    - **Input:** The forecasting model ingests the academy's historical payment data from **Module 1 (Financial Records)**. To ensure accuracy, the model requires at least 12 months of transaction history. It analyzes the date and amount of every recorded payment, membership fee, and expense.
    - **Process:** The AI uses time-series analysis to identify critical patterns:
        - **Seasonality:** It automatically detects recurring annual trends (e.g., "Revenue consistently drops by 40% in December and January" or "Revenue peaks by 50% in August during annual registration").
        - **Growth Trend:** It calculates the underlying month-over-month growth or decline rate.
        - **Payment Cadence:** It learns the typical timing of payments within a month (e.g., most payments arrive in the first 5 days).
    - **Output:** The feature is presented as a "Cash Flow Forecast" page.
        - **Visual Graph:** A clear line graph shows the projected bank balance over the next 90 days. A solid line indicates the historical balance up to the current day, which then transitions to a dotted line representing the AI's projection. The graph clearly visualizes expected dips and rises.
        - **Insight Summary:** A plain-language text box below the graph provides a summary of the forecast's key assumption, for example: `"Based on last year's data, our forecast predicts a seasonal cash-flow dip of approximately 40% in December. Plan expenditures accordingly."`

### 4. Acceptance Criteria
- [ ] The Churn Prediction Engine correctly flags a player with a 40% attendance rate in the last 30 days as "Medium Risk" (assuming no other negative signals are present).
- [ ] A player with both a late payment and a 65-day parent login gap is correctly flagged as "High Risk".
- [ ] The Cash Flow Forecasting tool, when analyzing an academy with a known revenue drop every December, shows a clear projected decline in the graph for the upcoming December.
- [ ] The "Re-engagement Script" for an at-risk player correctly and dynamically inserts the parent's first name and the player's first name into the text.
