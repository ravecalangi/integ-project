# Fuel Price Management System

## Description
A C#-based Fuel Price Management System designed to manage fuel pricing, user authentication, notifications, and dashboard operations. The system includes admin and user interfaces with session handling and database integration.

## Features
- User login and registration system
- Admin dashboard management
- User dashboard interface
- Fuel price management
- Real-time notifications system
- Session handling
- Splash screen interface
- Database helper integration
- In-memory data storage
- User account management

## Technologies Used
- C#
- .NET Framework
- Windows Forms (WinForms)
- SQL Database
- Object-Oriented Programming (OOP)

## How to Run
1. Download or clone the project.
2. Open the project in Visual Studio.
3. Restore required dependencies if needed.
4. Configure the database connection inside `DatabaseHelper.cs`.
5. Build and run the project using Visual Studio.

## Usage
1. Launch the application.
2. Register a new account or login using existing credentials.
3. Access the dashboard based on your account role.
4. Manage fuel prices and view notifications through the system interface.

### Example

**Input:**
```text
Username: admin
Password: admin123
```

**Output:**
```text
Login Successful
Opening Admin Dashboard...
```

## Project Structure
```text
AdminDashboard.cs
DatabaseHelper.cs
FuelPrice.cs
InMemoryStore.cs
LoginForms.cs
Notification.cs
PriceForm.cs
Program.cs
Registerform.cs
Session.cs
SplashForm.cs
User.cs
UserDashboard.cs
```

## License
MIT License
