# Internationalization (i18n) Implementation Guide for .NET

## Overview

This project uses ASP.NET Core's built-in localization features to provide internationalization support. This guide focuses on how to manage and use translations within the API, which can then be consumed by any client-side application.

## Features

-   **Supported Languages**: English (en-US) and Spanish (es-ES).
-   **Server-side Translations**: Managed in `.resx` resource files.
-   **API-driven Localization**: The API can return translated strings based on the client's request headers.
-   **Interpolation Support**: Dynamic values can be inserted into translated strings.
-   **Locale Detection**: The user's preferred language is determined from the `Accept-Language` HTTP header.

## File Structure

Translations are stored in `.resx` files within the project, typically in a `Resources` folder.

```text
/Diquis.WebApi
├── /Resources
│   ├── Controllers.HomeController.en-US.resx
│   └── Controllers.HomeController.es-ES.resx
├── /Controllers
│   └── HomeController.cs
```

A common pattern is to name the resource file after the class that will use it (e.g., `HomeController.cs` uses `Controllers.HomeController.resx`).

## Usage in the Backend (API)

In ASP.NET Core, we use the `IStringLocalizer<T>` service to access translations.

### 1. Inject the Localizer

Inject `IStringLocalizer<T>` into your controller or service. The `T` is the type that the resource file is named after.

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Diquis.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IStringLocalizer<HomeController> _localizer;

    public HomeController(IStringLocalizer<HomeController> localizer)
    {
        _localizer = localizer;
    }

    [HttpGet("welcome")]
    public IActionResult GetWelcomeMessage()
    {
        // Basic usage
        string title = _localizer["DashboardTitle"]; 
        // Returns "Dashboard" (en-US) or "Tablero" (es-ES)

        // Usage with interpolation
        string userName = "John Doe";
        string welcomeMessage = _localizer["WelcomeMessage", userName];
        // Returns "Welcome back, John Doe!" or "¡Bienvenido de nuevo, John Doe!"

        return Ok(new { title, welcomeMessage });
    }
}
```

### 2. Adding New Translations

**a. Add to `.resx` files:**

Open the resource files in your IDE (like Visual Studio) or a text editor.

**`Controllers.HomeController.en-US.resx`:**
| Name | Value |
|---|---|
| `DashboardTitle` | Dashboard |
| `WelcomeMessage` | Welcome back, {0}! |

**`Controllers.HomeController.es-ES.resx`:**
| Name | Value |
|---|---|
| `DashboardTitle` | Tablero |
| `WelcomeMessage` | ¡Bienvenido de nuevo, {0}! |

**b. Use in your C# code:**
```csharp
string title = _localizer["DashboardTitle"];
string welcome = _localizer["WelcomeMessage", "Maria"];
```

## How the API Determines the Language

The locale is determined by the `Accept-Language` header sent by the client in the HTTP request. The ASP.NET Core localization middleware automatically negotiates the best-matching language.

**Example Request Header:**
To get Spanish translations, the client would send:
```http
GET /api/home/welcome
Accept-Language: es-ES
```

To get English translations:
```http
GET /api/home/welcome
Accept-Language: en-US
```

If the header is omitted, the application will fall back to the default language (configured as English).

## Consuming Translations in a Frontend Client

Since this is an API-only backend, the frontend is responsible for managing its own i18n library (e.g., `react-i18next`). The frontend can:

1.  **Fetch translations from the API**: Create an endpoint that returns all necessary translations as a JSON object for the client to use.
2.  **Handle translations entirely on the client**: Maintain separate JSON translation files in the frontend project.

For Diquis, where the frontend is a separate repository, handling translations on the client is often the more scalable approach. The API can still return localized error messages or data based on the `Accept-Language` header.

## Best Practices

1.  **Use Descriptive Keys**: Use clear, descriptive names for your resource keys (e.g., `DashboardTitle` instead of `title`).
2.  **Organize by Feature**: Create separate resource files for different features or controllers to keep translations organized.
3.  **Use Interpolation**: Use placeholders like `{0}`, `{1}` for dynamic content.
4.  **Provide a Default Language**: Ensure you have a default resource file (e.g., `MyResources.resx`) that the application can fall back to.
5.  **Test All Languages**: Ensure that all supported languages are tested.
