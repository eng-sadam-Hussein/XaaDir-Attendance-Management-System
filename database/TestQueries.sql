/*
XaaDirDB Test Queries
Run this after XaaDirDB.sql.
*/

USE XaaDirDB;
GO

SELECT 'Users' AS TableName, COUNT(*) AS TotalRecords FROM Users
UNION ALL
SELECT 'Classes', COUNT(*) FROM Classes
UNION ALL
SELECT 'Subjects', COUNT(*) FROM Subjects
UNION ALL
SELECT 'Students', COUNT(*) FROM Students
UNION ALL
SELECT 'Attendance', COUNT(*) FROM Attendance;
GO

SELECT UserId, FullName, Username, Email, Role, IsActive
FROM Users
WHERE Username = 'admin' AND Password = 'admin123' AND IsActive = 1;
GO

SELECT UserId, FullName, Username, Email, Role, IsActive
FROM Users
WHERE Username = 'sadam' AND Password = 'teacher123' AND Role = 'Teacher' AND IsActive = 1;
GO

SELECT TOP 50
    s.StudentId,
    s.FullName,
    s.Gender,
    s.Phone,
    s.Email,
    c.ClassName,
    c.Section,
    s.Status
FROM Students s
INNER JOIN Classes c ON s.ClassId = c.ClassId
ORDER BY s.StudentId;
GO

SELECT
    sub.SubjectId,
    sub.SubjectName,
    u.FullName AS TeacherName,
    u.Username AS TeacherUsername
FROM Subjects sub
INNER JOIN Users u ON sub.TeacherUserId = u.UserId
ORDER BY u.FullName, sub.SubjectName;
GO

DECLARE @TeacherUserId INT = 2;

SELECT
    sub.SubjectId,
    sub.SubjectName,
    u.FullName AS TeacherName
FROM Subjects sub
INNER JOIN Users u ON sub.TeacherUserId = u.UserId
WHERE sub.TeacherUserId = @TeacherUserId;
GO

DECLARE @TeacherUserId2 INT = 2;

SELECT
    a.AttendanceId,
    st.FullName AS StudentName,
    c.ClassName,
    sub.SubjectName,
    a.AttendanceDate,
    a.Status,
    a.Remarks,
    u.FullName AS MarkedBy
FROM Attendance a
INNER JOIN Students st ON a.StudentId = st.StudentId
INNER JOIN Classes c ON a.ClassId = c.ClassId
INNER JOIN Subjects sub ON a.SubjectId = sub.SubjectId
INNER JOIN Users u ON a.MarkedByUserId = u.UserId
WHERE sub.TeacherUserId = @TeacherUserId2
ORDER BY a.AttendanceDate DESC, st.FullName;
GO

SELECT
    Status,
    COUNT(*) AS Total
FROM Attendance
GROUP BY Status;
GO
