# Expense Tracker API

Expense Tracker API is a personal finance management system that allows users to record, track, and manage their expenses efficiently. Supports filtering by time period (e.g., last week, last three months) and allows users to select custom date ranges for detailed expense tracking.  
You can view the project details on link here: [Project_URL](https://roadmap.sh/projects/expense-tracker-api).

## 🚀 Technologies Used
- **.NET 8** – Main framework for API development
- **Entity Framework Core** – ORM for database operations
- **Microsoft SQL Server** – Database management system
- **JWT (JSON Web Token)** – User authentication and authorization
- **AutoMapper** – Data mapping between layers
- **FluentValidation** – Input validation
- **Repository Pattern & Unit of Work** – Decoupled data access and transaction management

## 📌 Key Features
- **User Management:** Register, login, and authenticate with JWT
- **Expense Management:** Add, update, delete, and view expense records
- **User Authorization:** Restrict actions based on user roles

## 📂 Project Structure
```
ExpenseTrackerAPI/
│── Attributes/        # Custom attributes
│── Controllers/       # API controllers
│── Data/              # Database context and configurations
│── DTOs/              # Data Transfer Objects
│── Filters/           # Custom action filters
│── Mapper/            # AutoMapper profiles
│── Migrations/        # Entity Framework migrations
│── Models/            # Data models
│── Repositories/      # Repository pattern implementation
│── Services/          # Business logic services
│── Validators/        # FluentValidation rules
│── Program.cs         # Application configuration
│── appsettings.json   # Database and JWT settings
```

## 📚 Design Principles
This project follows **SOLID principles**

## 🔥 Key API Endpoints
![image](https://github.com/user-attachments/assets/748ac52d-d5aa-48f6-84bd-8779c42bd12e)


