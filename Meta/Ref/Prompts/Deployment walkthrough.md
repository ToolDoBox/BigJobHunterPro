We are working on getting our website published and deployed with Azure for the first time. It is a very small full stack application, we don't want to have to pay for every month if we can avoid it. We want to use the cheaper/free tiers. Here is the stack:

Deploy Big Job Hunter Pro MVP to Azure with:

- Backend: .NET 8 API on Azure App Service

- Frontend: React SPA on Azure Static Web Apps

- Database: Existing Azure SQL (bjhp-dev-sql-cgta.database.windows.net)

- Secrets: Azure Key Vault with Managed Identity

- CI/CD: GitHub Actions pipelines

The website repo that has both the front end and back end is located at: https://github.com/ToolDoBox/BigJobHunterPro

We already have an SQL server and SQL database set up. We need to do the following:

1: Azure Resource Provisioning with Azure Key Vault to store secrets
(DB Connection, JWT secret, Anthropic API Key)
2: Create App Service (backend)
3: Enable managed identity and grant key vault access 4. Configure Azure SQL firewall 5. Create Static Web App (frontend) using our `.github/workflows/azure-static-web-apps-*.yml`
6: Create Application Insights

Walk me through this process. I have minimal experience with Azure's interface and services, so I will need thorough explanations of what we are doing.
