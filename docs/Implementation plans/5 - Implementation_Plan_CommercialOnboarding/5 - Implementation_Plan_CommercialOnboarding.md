# Commercial Onboarding: Implementation & Testing Plan

## 1. Executive Summary

This document outlines the implementation and testing strategy for the Commercial Onboarding module. This is a mission-critical module that directly connects our business model to the underlying infrastructure by automating the sign-up, payment, and tenant provisioning process.

The plan details the necessary backend extensions to the `Tenant` entity, the implementation of a `StripeService` to handle payment webhooks, the creation of a frontend "Subscription" page in React, and a robust integration testing strategy to validate the entire sign-up and provisioning flow.

## 2. Backend Extension

The backend will be extended to manage subscription state and handle the core logic of processing payments and triggering provisioning jobs.

### 2.1. Extend the `Tenant` Entity

As specified in the technical guide, we will add subscription-related properties to the `Tenant` entity. This makes the subscription state a core part of the tenant's identity in the management database (`BaseDbContext`).

**Action:** Modify the `Tenant.cs` entity in the `Diquis.Domain` project.

**File:** `Diquis.Domain/Entities/Multitenancy/Tenant.cs`
```csharp
using Diquis.Domain.Enums; // Assuming SubscriptionStatus enum is defined

namespace Diquis.Domain.Entities.Multitenancy;

public class Tenant : BaseEntity
{
    // ... existing properties: Name, ConnectionString, etc.

    /// <summary>
    /// The subscription tier (e.g., Grassroots, Professional) for the tenant.
    /// Determines feature access and infrastructure allocation.
    /// </summary>
    public SubscriptionTier SubscriptionTier { get; set; }

    /// <summary>
    /// The current billing status of the subscription.
    /// </summary>
    public SubscriptionStatus SubscriptionStatus { get; set; }

    /// <summary>
    /// The unique customer ID from the Stripe payment gateway.
    /// Used for all billing and subscription management operations.
    /// </summary>
    public string? StripeCustomerId { get; set; }
    
    /// <summary>
    /// A flag to indicate if the tenant is in a read-only state,
    /// typically used during data migrations or maintenance.
    /// </summary>
    public bool IsReadOnly { get; set; }

    // ... other properties like ContractId
}

// And the corresponding Enum
public enum SubscriptionStatus
{
    Active,
    Trial,
    Provisioning,
    ProvisioningFailed,
    PastDue,
    Canceled
}
```

### 2.2. Implement `StripeService` and Webhook Logic

The core of the onboarding process is handling the `checkout.session.completed` event from Stripe. This is handled by a dedicated webhook controller that passes the event to an application service for processing.

**Action:** Create the `StripeWebhookController` and the services that orchestrate the sign-up and provisioning trigger. The implementation will follow the technical guide's specification.

**File:** `Diquis.WebApi/Controllers/StripeWebhookController.cs` (Refined from Technical Guide)
```csharp
[ApiController]
[Route("api/webhooks")]
public class StripeWebhookController : ControllerBase
{
    private readonly IOnboardingService _onboardingService;
    private readonly IConfiguration _config;
    private readonly ILogger<StripeWebhookController> _logger;

    public StripeWebhookController(IOnboardingService onboardingService, IConfiguration config, ILogger<StripeWebhookController> logger)
    {
        _onboardingService = onboardingService;
        _config = config;
        _logger = logger;
    }

    [HttpPost("stripe")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"],
                _config["Stripe:WebhookSecret"]);

            // Handle the specific event we care about for new sign-ups
            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                if (session?.Status == "complete")
                {
                    // This service method contains the core logic:
                    // 1. Create Tenant and User.
                    // 2. Set Status to "Provisioning".
                    // 3. Enqueue the provisioning background job.
                    await _onboardingService.HandleSuccessfulSignUpAsync(session);
                }
            }

            return Ok();
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Stripe webhook signature validation failed.");
            return BadRequest();
        }
    }
}
```
The `IOnboardingService` would then use the session details to create the `Tenant` and `ApplicationUser`, and critically, enqueue a background job for the `IProvisioningService` to perform the infrastructure setup, as detailed in the technical guide.

## 3. Frontend Extension (React)

The frontend needs a dedicated area for users to view and manage their subscription plans.

### 3.1. Create Subscription Page and Pricing Cards

An `Academy Owner` should be able to see their current plan and options to upgrade. This will be implemented in a new "Subscription" page within the settings area.

**Action:** Create a `SubscriptionPage.tsx` and a reusable `PricingCard.tsx` component.

**File:** `src/components/subscriptions/PricingCard.tsx`
```tsx
import { Card, Button, ListGroup } from 'react-bootstrap';

interface PricingCardProps {
  tierName: string;
  price: string;
  features: string[];
  isCurrentPlan: boolean;
  onSelect: () => void;
}

export const PricingCard = ({ tierName, price, features, isCurrentPlan, onSelect }: PricingCardProps) => {
  return (
    <Card className={isCurrentPlan ? 'border-primary' : ''}>
      <Card.Body>
        <Card.Title>{tierName}</Card.Title>
        <Card.Subtitle className="mb-2 text-muted">{price}</Card.Subtitle>
        <ListGroup variant="flush">
          {features.map(feature => <ListGroup.Item key={feature}>{feature}</ListGroup.Item>)}
        </ListGroup>
        <Button 
          variant={isCurrentPlan ? 'secondary' : 'primary'}
          onClick={onSelect}
          disabled={isCurrentPlan}
          className="mt-3"
        >
          {isCurrentPlan ? 'Current Plan' : 'Select Plan'}
        </Button>
      </Card.Body>
    </Card>
  );
};
```

**File:** `src/pages/settings/SubscriptionPage.tsx`
```tsx
import { useState, useEffect } from 'react';
import { Container, Row, Col, Alert } from 'react-bootstrap';
import { PricingCard } from '@/components/subscriptions/PricingCard';
import { useSubscriptionApi } from '@/api/subscriptionApi'; // Custom hook for API calls

export const SubscriptionPage = () => {
  const [currentSubscription, setCurrentSubscription] = useState<any>(null);
  const { getCurrentSubscription, createCheckoutSession } = useSubscriptionApi();

  useEffect(() => {
    getCurrentSubscription().then(setCurrentSubscription);
  }, []);

  const handleSelectPlan = async (tier: 'Grassroots' | 'Professional') => {
    try {
      // This API call returns a Stripe checkout URL
      const { url } = await createCheckoutSession(tier);
      // Redirect the user to Stripe to complete the payment
      window.location.href = url;
    } catch (error) {
      console.error('Failed to create checkout session', error);
    }
  };

  return (
    <Container>
      <h2 className="my-4">Subscription Management</h2>
      {currentSubscription && (
        <Alert variant="info">
          Your current plan is: <strong>{currentSubscription.tierName}</strong>. Status: <strong>{currentSubscription.status}</strong>.
        </Alert>
      )}
      <Row>
        <Col md={6}>
          <PricingCard
            tierName="Grassroots"
            price="$49/mo"
            features={['Up to 50 Players', 'Shared Infrastructure', 'Basic Analytics']}
            isCurrentPlan={currentSubscription?.tierName === 'Grassroots'}
            onSelect={() => handleSelectPlan('Grassroots')}
          />
        </Col>
        <Col md={6}>
          <PricingCard
            tierName="Professional"
            price="$199/mo"
            features={['Unlimited Players', 'Dedicated Database', 'Advanced Analytics', 'Sports Medicine Module']}
            isCurrentPlan={currentSubscription?.tierName === 'Professional'}
            onSelect={() => handleSelectPlan('Professional')}
          />
        </Col>
      </Row>
    </Container>
  );
};
```

## 4. Testing Strategy

The most critical test is the end-to-end integration of the self-service sign-up flow, from a successful payment webhook to the successful enqueuing of a provisioning job.

### 4.1. Backend Integration Test: Sign Up -> Provision Flow

This test will validate that the `StripeWebhookController` correctly processes a simulated event and that the `OnboardingService` creates the necessary database records and enqueues the correct background job.

**Action:** Create an integration test in `Diquis.Infrastructure.Tests`.

**File:** `Diquis.Infrastructure.Tests/Onboarding/OnboardingFlowTests.cs`
```csharp
using Diquis.Application.Common;
using Diquis.Application.Services.Onboarding;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Stripe;
using Stripe.Checkout;

namespace Diquis.Infrastructure.Tests.Onboarding;

[TestFixture]
public class OnboardingFlowTests
{
    private ServiceProvider _sp;
    private Mock<IBackgroundJobService> _mockBackgroundJobService;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        
        _mockBackgroundJobService = new Mock<IBackgroundJobService>();

        services.AddDbContext<BaseDbContext>(options => 
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
        
        services.AddScoped<IOnboardingService, OnboardingService>();
        services.AddSingleton(_mockBackgroundJobService.Object);
        
        _sp = services.BuildServiceProvider();
    }

    [Test]
    public async Task HandleSuccessfulSignUpAsync_WithValidStripeSession_ShouldCreateTenantAndEnqueueProvisioning()
    {
        // Arrange
        var onboardingService = _sp.GetRequiredService<IOnboardingService>();
        var dbContext = _sp.GetRequiredService<BaseDbContext>();

        var newTenantName = "Test Academy";
        var newOwnerEmail = "owner@test.com";

        // Simulate a completed Stripe Checkout Session
        var session = new Session
        {
            Id = "cs_test_123",
            Status = "complete",
            CustomerDetails = new SessionCustomerDetails { Email = newOwnerEmail },
            Metadata = new Dictionary<string, string>
            {
                { "TenantName", newTenantName },
                { "SubscriptionTier", "Professional" }
            }
        };

        // Act
        await onboardingService.HandleSuccessfulSignUpAsync(session);

        // Assert
        // 1. Verify a Tenant was created in the database with the correct status
        var newTenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == newTenantName);
        newTenant.Should().NotBeNull();
        newTenant!.SubscriptionStatus.Should().Be(SubscriptionStatus.Provisioning);
        newTenant.SubscriptionTier.Should().Be(Diquis.Domain.Enums.SubscriptionTier.Professional);

        // 2. Verify the background job for provisioning was correctly enqueued
        _mockBackgroundJobService.Verify(
            jobService => jobService.Enqueue<IProvisioningService>(
                service => service.ProvisionTenantAsync(newTenant.Id)
            ),
            Times.Once // Ensure it was called exactly one time
        );
    }
}
```
This test provides high confidence that the most critical part of the commercial onboarding flow—the hand-off from a successful payment to the start of the infrastructure provisioning process—is working correctly.
