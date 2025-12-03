# Task Context
Configure the ASP.NET Core backend to support multiple cultures (English and Spanish). This involves setting up the `RequestLocalizationMiddleware` to detect the client's preferred language and configuring the `IStringLocalizer` service to provide localized strings for server-side validation and messages.

# Core References
- **Plan:** [3 - Implementation_Plan_i18n.md](./3%20-%20Implementation_Plan_i18n.md)
- **Tech Guide:** [InternationalizationSecurity_TechnicalGuide.md](../../Technical%20documentation/InternationalizationSecurity_TechnicalGuide/InternationalizationSecurity_TechnicalGuide.md)

# Step-by-Step Instructions
1.  **Open `Diquis.WebApi/Program.cs`:**
2.  **Define Supported Cultures:**
    *   Create an array: `var supportedCultures = new[] { "en-US", "es-CR" };`
3.  **Configure Options:**
    *   Create `RequestLocalizationOptions`.
    *   Set Default Culture to `en-US`.
    *   Add Supported Cultures and UI Cultures.
4.  **Add Services:**
    *   Call `builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");`.
5.  **Use Middleware:**
    *   Call `app.UseRequestLocalization(localizationOptions);`.
    *   **Crucial:** Ensure this is placed *before* `app.UseAuthorization()` and `app.MapControllers()`.
6.  **Create Resource Directory:**
    *   Create a folder `Resources` in `Diquis.WebApi`.
    *   (Optional) Add a dummy resource file to verify path resolution.

# Acceptance Criteria
- [ ] `Program.cs` contains the localization configuration code.
- [ ] Middleware is inserted in the correct pipeline order.
- [ ] `Resources` directory exists.
- [ ] Application starts without errors.
