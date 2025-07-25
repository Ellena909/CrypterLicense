# Crypter License Backend

A comprehensive licensing system for the Crypter application with tiered subscription plans and automatic stub cleaning.

## Features

- **Multi-tier Licensing System**
  - Free Tier: 3 crypts per month
  - Basic Plan ($75/month): 40 crypts per month
  - Pro Plan ($200/month): Unlimited crypts per month
  - Enterprise Plan ($500/3 months): Unlimited crypts for 3 months

- **Security Features**
  - JWT Authentication
  - Hardware ID binding
  - License validation
  - Usage tracking

- **Automatic Stub Management**
  - Polymorphic stub generation every 6 hours
  - Anti-analysis features
  - Evasion techniques
  - Version control and cleanup

## Tech Stack

- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Swagger/OpenAPI

## Quick Deploy to Railway

### Prerequisites

1. GitHub account
2. Railway account (https://railway.app)
3. Git installed locally

### Deployment Steps

1. **Push to GitHub**
   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   git branch -M main
   git remote add origin https://github.com/yourusername/crypter-license.git
   git push -u origin main
   ```

2. **Deploy on Railway**
   - Go to https://railway.app
   - Click "New Project"
   - Select "Deploy from GitHub repo"
   - Choose your repository
   - Railway will automatically detect the .NET project

3. **Add Environment Variables**
   In Railway dashboard, go to Variables tab and add:
   ```
   JWT_KEY=your-super-secret-jwt-key-here-make-it-long-and-secure-at-least-32-characters
   ASPNETCORE_ENVIRONMENT=Production
   ```

4. **Add PostgreSQL Database**
   - In Railway dashboard, click "New"
   - Select "Database" → "PostgreSQL"
   - Railway will automatically set DATABASE_URL

5. **Deploy**
   - Railway will automatically build and deploy
   - Your API will be available at the provided Railway URL

## Local Development

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL (or use Docker)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/crypter-license.git
   cd crypter-license
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Setup database**
   ```bash
   # Update connection string in appsettings.json
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**
   Open https://localhost:7000/swagger

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `GET /api/auth/me` - Get current user info
- `POST /api/auth/validate-token` - Validate JWT token

### License Management
- `POST /api/license/validate` - Validate license key
- `POST /api/license/create` - Create new license
- `POST /api/license/crypt` - Process crypt request
- `GET /api/license/user/{userId}` - Get user licenses
- `GET /api/license/{key}` - Get license details
- `DELETE /api/license/{key}` - Deactivate license
- `GET /api/license/stub/info` - Get stub information
- `GET /api/license/stub/check` - Check for stub updates

### Admin (Protected)
- `GET /api/admin/stats` - System usage statistics
- `GET /api/admin/stubs/history` - Stub version history
- `GET /api/admin/stubs/active` - Active stub information
- `POST /api/admin/stubs/generate` - Generate new stub
- `POST /api/admin/stubs/update` - Update active stub
- `POST /api/admin/stubs/clean` - Clean old stubs
- `GET /api/admin/health` - System health check

## License Plans

| Plan | Price | Duration | Crypts | Features |
|------|-------|----------|--------|----------|
| Free | $0 | 1 month | 3 | Basic encryption |
| Basic | $75 | 1 month | 40 | Standard features |
| Pro | $200 | 1 month | Unlimited | All features |
| Enterprise | $500 | 3 months | Unlimited | All features + extended support |

## Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `DATABASE_URL` | PostgreSQL connection string | Yes |
| `JWT_KEY` | JWT signing key (min 32 chars) | Yes |
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | No |

## Database Schema

The application uses Entity Framework Code First migrations. Key entities:

- **Users**: User accounts and authentication
- **Licenses**: License keys and subscription details
- **CryptUsage**: Usage tracking and analytics
- **StubVersions**: Polymorphic stub management

## Security Considerations

- All API endpoints use HTTPS in production
- JWT tokens expire after 30 days
- Hardware ID binding prevents license sharing
- Rate limiting on authentication endpoints
- Input validation and sanitization
- Secure password hashing with PBKDF2

## Monitoring and Logging

- Structured logging with Serilog
- Usage analytics and reporting
- Error tracking and alerting

## Support

For issues and questions:
1. Check the API documentation at `/swagger`
2. Review the logs in Railway dashboard
3. Check database connectivity and migrations

## License

This project is open source and available under the MIT License.