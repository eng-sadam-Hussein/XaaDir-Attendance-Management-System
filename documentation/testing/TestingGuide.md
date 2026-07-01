# XaaDir Testing Guide

## 1. Database Test

Open SSMS and run:

```sql
USE XaaDirDB;
SELECT COUNT(*) AS UsersCount FROM Users;
SELECT COUNT(*) AS ClassesCount FROM Classes;
SELECT COUNT(*) AS SubjectsCount FROM Subjects;
SELECT COUNT(*) AS StudentsCount FROM Students;
SELECT COUNT(*) AS AttendanceCount FROM Attendance;
```

Expected basic data:

- Users: Admin + teachers
- Classes: Form 1, Form 2, Form 3, Form 4
- Subjects: 12 subjects
- Students: 200 students
- Attendance: sample records or class attendance records after testing

## 2. Backend Test

Run backend:

```bash
cd D:\Projects\XaaDir-Attendance-Management-System\backend\XaaDirApi
dotnet run
```

Open:

```text
http://localhost:5168/api/Classes
```

Expected: JSON list of classes.

## 3. Swagger Test

Open:

```text
http://localhost:5168/swagger
```

Test login:

```json
{
  "username": "admin",
  "password": "admin123"
}
```

Teacher login:

```json
{
  "username": "sadam",
  "password": "teacher123"
}
```

## 4. Frontend Test

Run frontend:

```bash
cd D:\Projects\XaaDir-Attendance-Management-System\frontend\xaadir-client
npm install
npm run dev
```

Open:

```text
http://localhost:5173
```

## 5. Role Test

Admin should see:

- Dashboard
- Users
- Classes
- Subjects
- Students
- Attendance
- Reports
- About

Teacher should see:

- Teacher Dashboard
- My Subjects
- Students
- Mark Attendance
- My Reports
- About

## 6. Class Attendance Test

1. Login as teacher.
2. Open Attendance.
3. Select Class.
4. Select assigned Subject.
5. Select Date.
6. Click Load Students.
7. Tick Present students.
8. Tick Late students.
9. Leave missing students unticked.
10. Click Save Class Attendance.

Expected:

- Present students become `Present`.
- Late students become `Late`.
- Unticked students become `Absent`.
- Summary cards update automatically.
- Reports show percentage information.

## 7. GitHub Final Check

Run:

```bash
cd /d/Projects/XaaDir-Attendance-Management-System
git status
```

Expected:

```text
nothing to commit, working tree clean
```
