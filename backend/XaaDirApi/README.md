# XaaDirApi - Complete Backend

ASP.NET Core Web API backend for XaaDir Attendance Management System.

## Stack

- ASP.NET Core Web API
- C#
- ADO.NET
- Microsoft.Data.SqlClient
- SQL Server XaaDirDB
- Swagger

## Features

- Login API
- Admin dashboard
- Teacher dashboard
- Users CRUD
- Classes CRUD
- Subjects CRUD
- Students CRUD
- Attendance CRUD
- Reports API
- Search endpoints
- Backend validation
- Teacher-specific permission rules
- Swagger testing

## Run in Visual Studio 2026

1. Copy `XaaDirApi` into:

   `D:\Projects\XaaDir-Attendance-Management-System\backend\XaaDirApi`

2. Open Visual Studio 2026.
3. Choose **Open a project or solution**.
4. Open:

   `backend\XaaDirApi\XaaDirApi.csproj`

5. Make sure SQL Server is running and database `XaaDirDB` exists.
6. Press the green **https** run button.
7. Swagger opens automatically.

Swagger URLs:

- `https://localhost:7168/swagger`
- `http://localhost:5168/swagger`

## Connection String

`appsettings.json`

```json
"Server=localhost;Database=XaaDirDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

## Test Login

POST `/api/auth/login`

Admin:

```json
{
  "username": "admin",
  "password": "admin123"
}
```

Teacher:

```json
{
  "username": "sadam",
  "password": "teacher123"
}
```

## Important Endpoints

- GET `/api/classes`
- GET `/api/users`
- GET `/api/students`
- GET `/api/subjects`
- GET `/api/subjects/teacher/2`
- GET `/api/attendance`
- GET `/api/attendance/teacher/2`
- GET `/api/reports/admin/summary`
- GET `/api/reports/teacher/2/summary`
- POST `/api/attendance/mark`

## Teacher Permission

A teacher can only access records connected to their own subjects.

Permission SQL rule:

```sql
WHERE Subjects.TeacherUserId = @TeacherUserId
```

If a teacher tries to mark attendance for another teacher's subject, backend returns `403 Forbidden`.

## GitHub Commands After Copying

```bash
cd /d/Projects/XaaDir-Attendance-Management-System
git status
git add backend/XaaDirApi
git commit -m "Add completed ASP.NET Core Web API backend"
git push
```
