User Management API
📋 Project Overview
A complete ASP.NET Core Web API for managing users with full CRUD operations, input validation, and custom middleware implementation. Developed as part of a back-end development project with assistance from GitHub Copilot.

✨ Features
✅ Full CRUD Operations: Create, Read, Update, Delete users
✅ Data Validation: Comprehensive input validation using DataAnnotations
✅ Custom Middleware Pipeline: Logging, Authentication, and Error Handling middleware
✅ Token-based Authentication: Secure API endpoints with Bearer token
✅ Consistent Error Responses: Standardized JSON error format
✅ Request/Response Logging: Detailed logging of all API interactions

API starts at: https://localhost:7188
Test with Postman using

GET /api/users - Get all users
POST /api/users - Create user
PUT /api/users/{id} - Update user (requires auth)
DELETE /api/users/{id} - Delete user (requires auth)

📁 Project Structure
text
UserManagementAPI/
├── Controllers/UsersController.cs
├── Models/User.cs
├── Middleware/ (3 custom middleware)
├── Services/UserService.cs
├── Program.cs
└── README.md

