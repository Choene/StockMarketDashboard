# Stock Market Dashboard

Stock Market Dashboard is a web application designed to provide users with real-time stock data in a visual and interactive format. It consists of an Angular frontend, an ASP.NET Core API backend, an Azure SQL database, and Redis caching for efficient data retrieval. The project is deployed using a CI/CD pipeline on GitHub Actions and is monitored with Azure Application Insights.

---

## Table of Contents
- [Project Overview](#project-overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Setup and Configuration](#setup-and-configuration)
- [API Documentation](#api-documentation)
- [Future Improvements](#future-improvements)

---

## Project Overview
Stock Market Dashboard is built to provide users with up-to-date stock market information. It supports secure login with JWT, efficient data caching with Redis, and data visualization using the Syncfusion Chart Library.

**LIVE PREVIEW**

- [Visit Preview Site](https://stockmarketdashboard.azurewebsites.net)

**Login Credentials**
- Username: user1
- Password: user123

## Features
- **Real-time Stock Data Visualization:** Display stock market data in interactive charts.
- **Authentication and Authorization:** Secure API endpoints with JWT.
- **Caching Mechanism:** Use of Redis and In-Memory cache to minimize API calls and improve performance.
- **Continuous Deployment:** CI/CD pipeline for seamless deployment using GitHub Actions.
- **Monitoring and Logging:** Azure Application Insights for real-time monitoring.

---

## Technologies Used
- **Backend:** ASP.NET Core API (.NET 8.x) hosted on Azure App Service.
- **Frontend:** Angular 18.x with Node.js 20.x, hosted on Azure App Service.
- **Database:** Azure SQL Database.
- **Data Visualization:** Syncfusion Chart Library.
- **Caching:** Azure Redis Cache & Fallback to In-Memory Cache.
- **CI/CD Pipeline:** GitHub Actions for continuous integration and deployment.
- **Monitoring:** Azure Application Insights for API monitoring and performance insights.

---

## Architecture

```plaintext
                    
+----------+          +--------+--------+        +----------------+         
|          |          |                 |        |                |        +----------------------+
|  Client  |<-------->|  Angular Front  |<------>| ASP.NET        |<------>|   Azure SQL Database |
| (Browser)|          |    End          |        | Core API(Azure |        +----------^-----------+
|          |          |  (Azure App     |        | App Service)   |
+----------+          |   Service)      |        +----------------+
                      |                 |
                      +-----------------+
```
## Key Components
- **Client (Browser):** Users interact with the Angular frontend through their web browsers.
- **Frontend (Angular):** Serves the UI, sends HTTP requests to the backend, and uses Syncfusion Chart Library for data visualization.
- **Backend (ASP.NET Core API):** Provides RESTful API endpoints, uses JWT for security, and integrates with Redis for caching.
- **Database (Azure SQL Database):** Stores user and application data.
- **Redis Cache (Azure Redis Cache & In-Memory Cache):** Caches frequently requested stock data to minimize API calls.
- **CI/CD Pipeline (GitHub Actions):** Automates deployment of the API and frontend.
- **Monitoring (Azure Application Insights):** Monitors API performance and usage.


## Getting Started

Prerequisites
• Backend: .NET 8.x SDK, Visual Studio, Azure App Service for hosting.
• Frontend: Node.js 20.x, Angular CLI 18.x, Azure App Service for hosting.
• Database: Azure SQL Database.
• Other Tools: GitHub account for CI/CD setup, Azure Subscription, Azure Redis Cache.

**Installation**

1. Clone the Repository:
```
git clone https://github.com/Choene/StockMarketDashboard.git
```

2. Configure Backend (API):
Add Azure SQL Database and Redis connection strings to secrets.json or environment variables.


```json
{
  "ConnectionStrings": {
    "DefaultConnection": "",
    "RedisConnection": ""
  }
}
```
• Set up the environment variables for `JwtSettings` (key, issuer, and audience).


3. Configure Frontend (Angular):
• Update the `apiUrl` in `src/environments/environment.ts` to point to your deployed backend.
• Run `npm install` to install dependencies.


4. Run Locally:

• Backend: Run the ASP.NET Core API with `dotnet run`.
• Frontend: Run Angular with `ng serve`.


## Setup and Configuration

1. Redis Caching & In-Memory Cache
• Redis is configured in `StockService` to cache stock data and reduce API calls.
• Ensure `RedisConnection` is added to your environment variables or `secrets.json` and mapped in Azure App Service's Connection Strings.
• In-Memory Cache is there as a fallback for when Redis fails.

3. JWT Authentication
• Secure API endpoints are configured with JWT for authentication and authorization.
• CORS policies allow the Angular frontend to securely access the API.

4. Monitoring
• Azure Application Insights is configured for monitoring API performance and error logging.

5. CI/CD Pipeline
• GitHub Actions is used to automate deployment to Azure.\
• See `.github/workflows/deploy.yml` for the complete CI/CD pipeline configuration.


## API Documentation

**Authentication**

**POST** `/api/auth/login`
• Description: Authenticates a user and returns a JWT token.
• Request Body: `{ "username": "string", "password": "string" }`

**POST** `/api/auth/register`
Description: Registers a new user with a username and password.
Request Body: `{ "username": "string", "password": "string", "role": "string" }`

**Stock Data**

**GET** `/api/stocks/{symbol}`
• Description: Fetches stock data for the given symbol, cached in Redis.
• Parameters: `symbol` - stock ticker symbol (e.g., "AAPL").

**GET** `/api/stocks`
• Description: Fetches data for multiple stocks (Admin only).

## Future Improvements
**Infrastructure as Code (IaC) with Bicep**: Automate Azure resource setup for better scalability and management.
**Enhanced Security with Azure Key Vault**: Secure sensitive data such as API keys and connection strings.
**Improved Data Refresh**: Implement advanced caching with Redis and In-Memory Cache for automated data refresh to ensure real-time accuracy.


![Untitled video (1)](https://github.com/user-attachments/assets/229698c2-3547-4040-904a-5e1cdfafd53d)








