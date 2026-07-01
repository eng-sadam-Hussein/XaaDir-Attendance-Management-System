# XaaDir Attendance Management System - Documentation

This folder contains the final documentation files for the XaaDir Attendance Management System.

## Contents

| File/Folder | Purpose |
|---|---|
| `ProjectReport.docx` | Final editable project report for submission |
| `ProjectReport.pdf` | PDF version of the final report |
| `Presentation.pptx` | Final PowerPoint presentation for demo/viva |
| `FinalChecklist.md` | Final project completion checklist |
| `database/ERD_Overview.md` | Database relationship overview |
| `database/TableSchema.md` | Table structure and schema description |
| `testing/TestingGuide.md` | How to test database, backend, frontend, login, roles, and attendance |

## Project Title

**XaaDir Attendance Management System**

## Slogan

**Smart Attendance Made Simple**

## Technology Stack

- Frontend: React JS, Tailwind CSS, React Router, Context API, Axios
- Backend: ASP.NET Core Web API, C#, ADO.NET
- Database: SQL Server
- Tools: Visual Studio, VS Code, SSMS, Git, GitHub

## Final Attendance Workflow

1. Teacher/Admin selects class.
2. Teacher/Admin selects subject.
3. Teacher/Admin selects date.
4. System loads all students in the selected class.
5. Teacher ticks students who are Present.
6. Teacher marks students who are Late.
7. Unticked students are automatically Absent.
8. System saves the attendance for the whole class.
9. Reports show Present, Absent, Late, and percentage summaries.

## Role Rules

- `Admin`: full system access; cannot be converted to another role.
- `Teacher`: teacher access only.
- `TeacherAdmin`: teacher account with admin access.

## Main Login Accounts

| Role | Username | Password |
|---|---|---|
| Admin | `admin` | `admin123` |
| Teacher | `sadam` | `teacher123` |
| Teacher | `farhan` | `teacher123` |
| Teacher | `liban` | `teacher123` |
| Teacher | `ahmed` | `teacher123` |
| Teacher | `miqdaad` | `teacher123` |
