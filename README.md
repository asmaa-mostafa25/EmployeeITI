# 👨‍💼 Employee Management System (ASP.NET Core MVC)

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)
![jQuery](https://img.shields.io/badge/jQuery-Validation-0769AD?style=for-the-badge&logo=jquery&logoColor=white)
![License](https://img.shields.io/badge/License-Educational-lightgrey?style=for-the-badge)

A full-featured **Employee & Department Management System** built with ASP.NET Core MVC (.NET 8), Entity Framework Core, and SQL Server — covering CRUD operations, custom middleware, cookies, sessions, validation, and a live statistics dashboard.

## ✨ Features

### 🧩 Custom Middleware
- **Request Logger** — logs path, HTTP method, and timestamp for every request.
- **Maintenance Mode** — blocks all requests with a friendly message when enabled.
- **Request Counter** — tracks total requests handled since app startup.
- **Response Timer** — measures and logs how long each request takes.

### 🍪 Cookies
- Remember the user's name and greet them on the Home page (`Welcome, {Name}` / `Welcome, Guest`).
- Save and apply a **Light/Dark theme** preference across the whole app.
- Employee search text and selected page size persist across page refreshes.
- Last login date is remembered and shown at the next login.

### 🔐 Sessions
- **Visit counter** on the Home page.
- **Recently visited departments** (last 5), with the most recent one highlighted in the Departments table.
- **Login/Logout simulation** — logged-in username shown in the navbar.
- Employee & Department pages require login.

### 👥 Employee Module
- List with Id, Name, Age, Salary, Job Title, Department.
- Add / Edit / Delete.
- Search by name + adjustable page size (5 / 10 / 20) with pagination.

### 🏢 Department Module
- List with Id, Name, Manager, Employee Count.
- Add / Edit / Delete / Show Employees / Department Details.

### ✅ Validation
- ViewModels + Data Annotations.
- Client-side (jQuery Unobtrusive) and server-side (`ModelState.IsValid`) validation.
- Custom validation attributes (unique department name, no-numbers name, age/salary rule).

### 📊 Dashboard
- Total Employees, Total Departments, Average / Highest / Lowest Salary — all loaded asynchronously.

### ⚠️ Error Handling
- Custom friendly error page (`/Home/Error`) via `UseExceptionHandler`.

## 🛠️ Tech Stack
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (SQL Server)
- Bootstrap 5
- jQuery Validation (Unobtrusive)

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server / SQL Server Express

### Setup
```bash
git clone <your-repo-url>
cd EmployeeDep
```

1. Update the connection string in `Models/AppDbContext.cs` to match your local SQL Server instance.
2. Apply migrations:
```bash
   dotnet ef database update
```
3. Run the app:
```bash
   dotnet run
```
4. Open the URL shown in the console (e.g. `https://localhost:xxxx`).

## 📁 Project Structure
