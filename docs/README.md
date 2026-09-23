# BookManager Documentation

Welcome to the **BookManager** project documentation! This directory contains comprehensive documentation for understanding, developing, deploying, and contributing to the BookManager RESTful API system.

## 📚 Documentation Index

### [Architecture Documentation](ARCHITECTURE.md)
Deep dive into the system architecture, design patterns, and technical decisions.

**Topics covered**:
- Clean Architecture and layered design
- Thin Controller - Fat Service pattern
- Repository Pattern implementation
- Communication protocols and data flow
- Security architecture (JWT Dual-Token authentication, BCrypt)
- Database design with PostgreSQL and EF Core
- Scalability considerations, caching, and background queues
- Design patterns and technical decisions

**Read this if you want to**:
- Understand system design and layer responsibilities
- Learn about architectural patterns and principles
- Understand request lifecycles and data flow
- Explore security and authentication mechanics

---

### [API Documentation](API.md)
Complete API reference for all endpoints with request/response examples and client snippets.

**Topics covered**:
- Base URLs and environment configurations
- Authentication and authorization flow (Access Token & Refresh Token)
- Authentication API (`/api/auth/register`, `/api/auth/login`, `/api/auth/refresh`)
- Books Management API (`/api/books` - CRUD, search, sorting, filtering)
- Error handling format (RFC 7807 Problem Details) and HTTP status codes
- Client integration examples (cURL, JavaScript Fetch, C# HttpClient, Postman)

**Read this if you want to**:
- Integrate frontend (React/Vue/Angular) or mobile apps with the API
- Test endpoints using Postman or cURL
- Understand request schemas and response formats
- Handle API errors consistently

---

### [Development Guide](DEVELOPMENT.md)
Step-by-step onboarding guide for developers working on the codebase.

**Topics covered**:
- Development environment setup (Prerequisites: .NET 10 SDK, Docker, PostgreSQL)
- Quick start instructions (run within 15 minutes)
- Detailed project structure and file map
- Coding standards and C# naming conventions
- Database migrations with Entity Framework Core CLI
- Step-by-step tutorial for adding new features
- Testing, debugging, and troubleshooting common errors

**Read this if you want to**:
- Set up your local development environment
- Understand the codebase structure and rules
- Add new endpoints, entities, or services
- Run migrations and debug local issues

---

### [Deployment Guide](DEPLOYMENT.md)
Instructions for deploying BookManager to Staging and Production environments.

**Topics covered**:
- Hardware and operating system requirements
- Complete environment variables reference (`.env`)
- Docker & Docker Compose deployment
- Kubernetes (K8s) deployment architecture, manifests, and configs
- Database setup, automated migrations, and backup/restore procedures
- Monitoring, logging, and health check configurations
- Troubleshooting production issues and rollback strategies

**Read this if you want to**:
- Deploy the system using Docker Compose or Kubernetes
- Configure environment variables and secrets
- Set up database backup and recovery plans
- Maintain and scale the production environment

---

### [Contributing Guide](CONTRIBUTING.md)
Guidelines for collaborating and contributing code to the project.

**Topics covered**:
- Code of conduct and team values
- Git workflow (Git Flow, branch naming conventions)
- Commit message standards (Conventional Commits: `feat:`, `fix:`, `refactor:`)
- Pull Request (PR) checklist and acceptance criteria
- Code review guidelines and quality standards
- Bug reporting and feature suggestion templates

**Read this if you want to**:
- Create feature branches and submit Pull Requests
- Follow team Git standards to prevent merge conflicts
- Participate in code reviews

---

## 🚀 Quick Links by Role

### For New Developers (Onboarding)
1. Follow the [Development Guide](DEVELOPMENT.md) to set up your environment.
2. Read the [Contributing Guide](CONTRIBUTING.md) for Git branch and commit rules.
3. Review the [Architecture Documentation](ARCHITECTURE.md) to understand data flow.

### For Frontend / Mobile Developers
1. Start with the [API Documentation](API.md) for endpoint specifications.
2. Review authentication flow in [API Documentation](API.md#authentication).
3. Import `BookManager.postman_collection.json` to test endpoints immediately.

### For DevOps & System Administrators
1. Review the [Deployment Guide](DEPLOYMENT.md) for Docker and Kubernetes setup.
2. Check environment variables in [Deployment Guide](DEPLOYMENT.md#environment-configuration-reference).
3. Set up database backup cronjobs as described in [Deployment Guide](DEPLOYMENT.md#database-backup-and-recovery).

---

## 📖 Documentation Structure

```
docs/
├── README.md              # This file - Documentation hub & index
├── ARCHITECTURE.md        # System architecture, patterns, and design
├── API.md                 # Full API reference with examples
├── DEVELOPMENT.md         # Developer setup, coding standards, and workflow
├── DEPLOYMENT.md          # Docker, Kubernetes, and operations guide
└── CONTRIBUTING.md        # Git flow, commit conventions, and PR process
```

---

## 🔍 Frequently Asked Questions (FAQ)

**Q: How do I run the project locally with Docker?**  
A: Run `docker compose up --build -d`. The API will be available at `http://localhost:8080` (Swagger at `http://localhost:8080/swagger`). See [Development Guide](DEVELOPMENT.md#quick-start-in-15-minutes).

**Q: How does the JWT authentication work?**  
A: The API issues an Access Token (valid for 15 minutes) and a Refresh Token (valid for 7 days) signed with separate secret keys. See [Architecture Documentation](ARCHITECTURE.md#security-architecture) and [API Documentation](API.md#authentication).

**Q: Where can I find pre-configured Postman requests?**  
A: Import the file `BookManager.postman_collection.json` located at the root directory into Postman. See [API Documentation](API.md#postman-collection).

**Q: How do I add a new Database Migration?**  
A: Run `dotnet ef migrations add <MigrationName>`. See [Development Guide](DEVELOPMENT.md#database-migrations).

---

## 📝 Documentation Metadata

- **Project**: BookManager REST API
- **Framework**: .NET 10.0 / ASP.NET Core
- **Database**: PostgreSQL 17
- **Architecture**: Layered Clean Architecture (Thin Controller - Fat Service, Repository Pattern)
- **Status**: Production-Ready Documentation

---

**Happy Coding! 🎉**
