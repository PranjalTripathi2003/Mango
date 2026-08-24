# 🥭 Mango - Microservices E-Commerce Platform

A production-ready, cloud-native E-Commerce solution built with **.NET 8**, **Microservices Architecture**, **Ocelot API Gateway**, **Azure Service Bus**, **Azure SQL Database**, and **Azure App Services**.

---

## 🏗️ Architecture Overview

The application follows a distributed microservices pattern with asynchronous event-driven messaging, centralized authentication, and an API Gateway acting as a single entry point for client requests.

```
                         ┌──────────────────┐
                         │   Mango.Web UI   │
                         │    (MVC Client)  │
                         └────────┬─────────┘
                                  │
                                  ▼
                         ┌──────────────────┐
                         │  Ocelot Gateway  │
                         │ (Mango.Gateway)  │
                         └────────┬─────────┘
                                  │
      ┌──────────────┬────────────┼────────────┬──────────────┐
      │              │            │            │              │
      ▼              ▼            ▼            ▼              ▼
┌───────────┐  ┌───────────┐ ┌──────────┐ ┌──────────┐ ┌─────────────┐
│  AuthAPI  │  │ CouponAPI │ │ProductAPI│ │ CartAPI  │ │  OrderAPI   │
└─────┬─────┘  └───────────┘ └──────────┘ └────┬─────┘ └──────┬──────┘
      │                                        │              │
      └──────────────────┐ ┌───────────────────┘              │
                         │ │                                  │
                         ▼ ▼                                  ▼
                 ┌─────────────────────────────────────────────────┐
                 │         Azure Service Bus (Topic / Queues)      │
                 └──────────────┬──────────────────┬───────────────┘
                                │                  │
                                ▼                  ▼
                         ┌─────────────┐    ┌─────────────┐
                         │  EmailAPI   │    │  RewardAPI  │
                         │  (Consumer) │    │  (Consumer) │
                         └─────────────┘    └─────────────┘
```

---

## 🚀 Microservices Breakdown

| Service | Technology | Description |
|---|---|---|
| **`Mango.Web`** | ASP.NET Core 8 MVC | User-facing responsive web application with JWT authentication, cart, checkout, and order management. |
| **`Mango.GatewaySolution`** | Ocelot API Gateway | Central reverse proxy routing requests to appropriate backend services with unified endpoints. |
| **`Mango.Services.AuthAPI`** | ASP.NET Core Identity + EF Core | User registration, login, JWT token generation, role management (Admin/Customer), and Service Bus integration. |
| **`Mango.Services.CouponAPI`** | REST API + EF Core | Promo code creation, validation, percentage discounts, and min-amount rules. |
| **`Mango.Services.ProductAPI`** | REST API + EF Core | Product catalog CRUD, image uploads with local/blob storage support, and category filtering. |
| **`Mango.Services.ShoppingCartAPI`**| REST API + EF Core | Shopping cart operations, coupon application/removal, and cart checkout message publishing. |
| **`Mango.Services.OrderAPI`** | REST API + EF Core | Order processing, Razorpay payment gateway integration, order lifecycle & status management. |
| **`Mango.Services.EmailAPI`** | Background Consumer + EF Core | Azure Service Bus topic listener for registration emails, cart recovery, and order confirmation logging. |
| **`Mango.Services.RewardAPI`** | Background Consumer + EF Core | Azure Service Bus consumer rewarding customer loyalty points upon order placement. |
| **`Mango.MessageBus`** | Azure Service Bus SDK | Shared messaging abstraction library for publishing topics and events across services. |

---

## 🛠️ Tech Stack & Tools

- **Framework**: .NET 8 (C# 12)
- **Architecture**: Microservices, Event-Driven, Clean Architecture
- **API Gateway**: Ocelot
- **Database**: Azure SQL Serverless / SQL Server with Entity Framework Core & Migrations
- **Messaging / Event Bus**: Azure Service Bus (Topics & Subscriptions)
- **Security & Identity**: ASP.NET Core Identity, JWT Bearer Authentication, Role-Based Authorization
- **Payments**: Razorpay Payment Gateway integration
- **Frontend**: ASP.NET Core MVC, Bootstrap, HTML5, JavaScript, Toastr Notifications
- **CI/CD**: GitHub Actions workflows with path-based deployment triggers
- **Cloud Hosting**: Azure App Services (Linux / Windows) with OIDC Managed Identity authentication

---

## ⚡ Getting Started (Local Development)

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or Azure SQL Database
- [Azure Service Bus](https://azure.microsoft.com/en-us/products/service-bus) (or local connection string)

### 1. Clone the repository
```bash
git clone https://github.com/PranjalTripathi2003/Mango.git
cd Mango
```

### 2. Configure Connection Strings
Update `appsettings.json` in each microservice project with your SQL connection strings and Azure Service Bus details:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Mango_<Service>;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Apply Database Migrations
Run migrations for each service:
```bash
dotnet ef database update --project Mango.Services.AuthAPI
dotnet ef database update --project Mango.Services.CouponAPI
dotnet ef database update --project Mango.Services.ProductAPI
dotnet ef database update --project Mango.Services.ShoppingCartAPI
dotnet ef database update --project Mango.Services.OrderAPI
dotnet ef database update --project Mango.Services.EmailAPI
dotnet ef database update --project Mango.Services.RewardAPI
```

### 4. Run the Solution
Open `Mango.sln` in Visual Studio or launch via CLI / Multiple Startup Projects (start the APIs, Gateway, and `Mango.Web`).

---

## 🚢 CI/CD & Cloud Deployment

Each microservice has an independent **GitHub Actions workflow** configured with `paths:` triggers under `.github/workflows/`. 

- Pushing changes to a specific service folder (e.g. `Mango.Services.ProductAPI/**`) will **only** build and deploy that specific microservice to Azure App Service.
- Workflows use **GitHub OIDC federated credentials** with an Azure User-Assigned Managed Identity (`oidc-msi-a856`) for secure passwordless deployments.
- Includes `/health` keep-alive endpoints across all microservices for automated warming.

---

## 📜 License

This project is licensed under the MIT License.
