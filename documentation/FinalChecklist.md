# XaaDir App - Final Checklist

## Completed

- Database: SQL Server `XaaDirDB`
- Backend: ASP.NET Core Web API + ADO.NET
- Frontend: React JS + Tailwind CSS
- Class Attendance Workflow: class/session based
- Roles: Admin, Teacher, TeacherAdmin
- Reports: status charts + student percentages
- Documentation: final report + final presentation

## Before Final Submission

1. Run SQL Server and confirm `XaaDirDB` exists.
2. Run backend:

```bash
cd D:\Projects\XaaDir-Attendance-Management-System\backend\XaaDirApi
dotnet run
```

3. Test backend:

```text
http://localhost:5168/api/Classes
```

4. Run frontend:

```bash
cd D:\Projects\XaaDir-Attendance-Management-System\frontend\xaadir-client
npm install
npm run dev
```

5. Test frontend:

```text
http://localhost:5173
```

6. Login:

```text
admin / admin123
sadam / teacher123
```

7. Test class attendance: select class, subject, date, load students, mark Present/Late, save.
8. Push latest files to GitHub.
9. Optional: deploy if instructor requires live URL.

## Final Git Push

```bash
cd /d/Projects/XaaDir-Attendance-Management-System
git add backend/XaaDirApi frontend/xaadir-client database documentation
git commit -m "Finalize XaaDir attendance system documentation"
git push
```
