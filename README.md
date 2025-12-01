# Diquis SaaS Platform

![Build Status](https://img.shields.io/azure-devops/build/diquis/diquis/1?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge)
![C#](https://img.shields.io/badge/C%23-12-239120?style=for-the-badge)
![License](https://img.shields.io/badge/License-Proprietary-red?style=for-the-badge)

## Executive Summary

Diquis is a next-generation, AI-enhanced, multi-tenant SaaS platform engineered for elite football academies. Built on a robust and scalable **Hybrid Tenancy Architecture**, it offers tailored infrastructure solutions from shared databases for grassroots academies to fully isolated enterprise stacks. Our integrated AI suite provides unprecedented data-driven insights and operational efficiency, governed by a strict ethical framework.

## Commercial Architecture

Diquis employs a flexible, three-tier infrastructure model to meet the diverse security, performance, and budgetary needs of our clients.

| Tier         | Infrastructure Model         | Ideal For                                  |
|--------------|------------------------------|--------------------------------------------|
| **Grassroots** | Shared Database, Shared App  | Small academies, startups, and trials.     |
| **Professional** | Dedicated Database, Shared App | Growing academies requiring data isolation. |
| **Enterprise** | Fully Isolated Stack         | Large organizations with maximum security needs. |

## The Module Ecosystem

The Diquis platform is a comprehensive ecosystem of interconnected modules designed to manage every facet of a modern football academy.

### Cluster A: Core Operations
-   **Module 1:** Academy Operations (Finance, Inventory, Staffing)
-   **Module 2:** Player Management (Profiles, Registration, Documentation)
-   **Module 3:** Team Organization (Squads, Rosters, Formations)
-   **Module 4:** Training Sessions (Planning, Attendance, Performance Tracking)

### Cluster B: Professional Suite
-   **Module 5:** Analytics & Reporting (Advanced Dashboards, Player Progression)
-   **Module 6:** Asset Management (Kit & Equipment Tracking)
-   **Module 7:** Sports Medicine (Bio-Passport, Injury Tracking - HIPAA/GDPR Compliant)
-   **Module 8:** Scouting & Recruitment (Talent Identification, Network Management)
-   **Module 9:** Facility & Resource Operations (Booking, Maintenance)

### Cluster C: Artificial Intelligence Suite
-   **Module 16-A:** External AI Sales Agent (Automated Lead Engagement)
-   **Module 16-B:** Operational AI Assistant Coach (Training Plan Generation)
-   **Module 16-C:** Predictive AI Revenue Guardian (Financial Forecasting)
-   **Module 16-D:** AI Governance & Safety Protocols (Ethical Oversight)

### Cluster D: Platform & Utility Services
-   **Module 10:** Commercial Onboarding & Billing (Tenant Provisioning, Subscriptions)
-   **Module 11:** Internationalization (i18n) & Localization
-   **Module 12:** Security & Compliance (GDPR, Audit Logs)
-   **Module 13:** Communication & Notification Engine
-   **Module 14:** Data Portability & Migration Tools
-   **Module 15:** Digital Signatures & Legal Documents

## Tech Stack

-   **Backend:** .NET 10, C# 12, ASP.NET Core
-   **Architecture:** Clean Architecture, Domain-Driven Design
-   **Database:** PostgreSQL
-   **DevOps:** Docker, Kubernetes (K8s)

## Quick Start

1.  **Clone the repository:**
    ```sh
    git clone https://github.com/diquis/diquis.git
    cd diquis/Diquis.WebApi
    ```

2.  **Set User Secrets:**
    *Set up your database connection string and other secrets. This is required for local development.*
    ```sh
    dotnet user-secrets set "DatabaseSettings:ConnectionString" "Your_PostgreSQL_Connection_String"
    dotnet user-secrets set "Security.Key" "Your_Super_Secret_Key_For_JWT"
    ```

3.  **Run Migrations:**
    *Apply the latest database migrations to create the schema.*
    ```sh
    dotnet ef database update --project ../Diquis.Infrastructure/Diquis.Infrastructure.csproj
    ```

4.  **Start the API:**
    ```sh
    dotnet run
    ```
    The API will be available at `https://localhost:5001`.

## License

This is a proprietary commercial software product.

Copyright (c) 2025 Diquis. All Rights Reserved.

See the [LICENSE](LICENSE) file for more details.