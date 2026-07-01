/*
============================================================
 XaaDir Attendance Management System
 Complete SQL Server Database Script
 Database: XaaDirDB

 Guideline Compliance:
 - SQL Server Database
 - 5 Tables only: Users, Classes, Subjects, Students, Attendance
 - Primary Keys, Foreign Keys, Relationships
 - Sample Records
 - Admin + Teacher Login
 - Teacher-specific permission design
============================================================
*/

USE master;
GO

IF DB_ID('XaaDirDB') IS NOT NULL
BEGIN
    ALTER DATABASE XaaDirDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE XaaDirDB;
END
GO

CREATE DATABASE XaaDirDB;
GO

USE XaaDirDB;
GO

CREATE TABLE Users
(
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(120) NOT NULL,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(120) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin', 'Teacher'))
);
GO

CREATE TABLE Classes
(
    ClassId INT IDENTITY(1,1) PRIMARY KEY,
    ClassName NVARCHAR(100) NOT NULL,
    Section NVARCHAR(20) NULL,
    Description NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Classes_ClassName_Section UNIQUE (ClassName, Section)
);
GO

CREATE TABLE Subjects
(
    SubjectId INT IDENTITY(1,1) PRIMARY KEY,
    SubjectName NVARCHAR(120) NOT NULL,
    TeacherUserId INT NOT NULL,
    Description NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Subjects_Teachers FOREIGN KEY (TeacherUserId) REFERENCES Users(UserId),
    CONSTRAINT UQ_Subjects_SubjectName UNIQUE (SubjectName)
);
GO

CREATE TABLE Students
(
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(120) NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    Phone NVARCHAR(30) NOT NULL UNIQUE,
    Email NVARCHAR(120) NOT NULL UNIQUE,
    ClassId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Students_Classes FOREIGN KEY (ClassId) REFERENCES Classes(ClassId),
    CONSTRAINT CK_Students_Gender CHECK (Gender IN ('Male', 'Female')),
    CONSTRAINT CK_Students_Status CHECK (Status IN ('Active', 'Inactive'))
);
GO

CREATE TABLE Attendance
(
    AttendanceId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    ClassId INT NOT NULL,
    SubjectId INT NOT NULL,
    AttendanceDate DATE NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Remarks NVARCHAR(255) NULL,
    MarkedByUserId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Attendance_Students FOREIGN KEY (StudentId) REFERENCES Students(StudentId),
    CONSTRAINT FK_Attendance_Classes FOREIGN KEY (ClassId) REFERENCES Classes(ClassId),
    CONSTRAINT FK_Attendance_Subjects FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId),
    CONSTRAINT FK_Attendance_Users FOREIGN KEY (MarkedByUserId) REFERENCES Users(UserId),
    CONSTRAINT CK_Attendance_Status CHECK (Status IN ('Present', 'Absent', 'Late')),
    CONSTRAINT UQ_Attendance_Student_Subject_Date UNIQUE (StudentId, SubjectId, AttendanceDate)
);
GO

INSERT INTO Users (FullName, Username, Email, Password, Role, IsActive)
VALUES
('System Administrator', 'admin', 'admin@xaadir.com', 'admin123', 'Admin', 1),
('Ustaad Sadam Hussein', 'sadam', 'sadam.teacher@xaadir.com', 'teacher123', 'Teacher', 1),
('Ustaad Farhan Shukri', 'farhan', 'farhan.teacher@xaadir.com', 'teacher123', 'Teacher', 1),
('Ustaad Liban', 'liban', 'liban.teacher@xaadir.com', 'teacher123', 'Teacher', 1),
('Ustaad Ahmed Shafi', 'ahmed', 'ahmed.teacher@xaadir.com', 'teacher123', 'Teacher', 1),
('Ustaad Miqdaad', 'miqdaad', 'miqdaad.teacher@xaadir.com', 'teacher123', 'Teacher', 1);
GO

INSERT INTO Classes (ClassName, Section, Description)
VALUES
('Form 1', 'A', 'Form 1 secondary class'),
('Form 2', 'A', 'Form 2 secondary class'),
('Form 3', 'A', 'Form 3 secondary class'),
('Form 4', 'A', 'Form 4 secondary class');
GO

INSERT INTO Subjects (SubjectName, TeacherUserId, Description)
VALUES
('Technology', 2, 'Technology assigned to Ustaad Sadam Hussein'),
('English', 2, 'English assigned to Ustaad Sadam Hussein'),
('Somali', 2, 'Somali assigned to Ustaad Sadam Hussein'),
('Physics', 3, 'Physics assigned to Ustaad Farhan Shukri'),
('Chemistry', 3, 'Chemistry assigned to Ustaad Farhan Shukri'),
('Biology', 3, 'Biology assigned to Ustaad Farhan Shukri'),
('History', 4, 'History assigned to Ustaad Liban'),
('Geography', 4, 'Geography assigned to Ustaad Liban'),
('Islamic Studies / Tarbiya', 5, 'Islamic Studies assigned to Ustaad Ahmed Shafi'),
('Arabic', 5, 'Arabic assigned to Ustaad Ahmed Shafi'),
('Physical Education', 6, 'Physical Education assigned to Ustaad Miqdaad'),
('Arts and Crafts', 6, 'Arts and Crafts assigned to Ustaad Miqdaad');
GO

DECLARE @i INT = 1;
DECLARE @classId INT;
DECLARE @gender NVARCHAR(10);
DECLARE @fullName NVARCHAR(120);
DECLARE @phone NVARCHAR(30);
DECLARE @email NVARCHAR(120);

WHILE @i <= 200
BEGIN
    SET @classId = CASE
        WHEN @i BETWEEN 1 AND 50 THEN 1
        WHEN @i BETWEEN 51 AND 100 THEN 2
        WHEN @i BETWEEN 101 AND 150 THEN 3
        ELSE 4
    END;

    SET @gender = CASE WHEN @i % 2 = 0 THEN 'Female' ELSE 'Male' END;
    SET @fullName = CONCAT('Student ', FORMAT(@i, '000'), CASE WHEN @gender = 'Male' THEN ' Mohamed' ELSE ' Ahmed' END);
    SET @phone = CONCAT('061', FORMAT(@i, '0000000'));
    SET @email = CONCAT('student', FORMAT(@i, '000'), '@xaadir.com');

    INSERT INTO Students (FullName, Gender, Phone, Email, ClassId, Status)
    VALUES (@fullName, @gender, @phone, @email, @classId, 'Active');

    SET @i = @i + 1;
END
GO

DECLARE @studentId INT = 1;
DECLARE @subjectId INT;
DECLARE @teacherId INT;
DECLARE @status NVARCHAR(20);

WHILE @studentId <= 40
BEGIN
    SET @subjectId =
        CASE
            WHEN @studentId % 5 = 1 THEN 1
            WHEN @studentId % 5 = 2 THEN 4
            WHEN @studentId % 5 = 3 THEN 7
            WHEN @studentId % 5 = 4 THEN 9
            ELSE 11
        END;

    SELECT @teacherId = TeacherUserId FROM Subjects WHERE SubjectId = @subjectId;

    SET @status =
        CASE
            WHEN @studentId % 10 = 0 THEN 'Absent'
            WHEN @studentId % 6 = 0 THEN 'Late'
            ELSE 'Present'
        END;

    INSERT INTO Attendance (StudentId, ClassId, SubjectId, AttendanceDate, Status, Remarks, MarkedByUserId)
    SELECT
        s.StudentId,
        s.ClassId,
        @subjectId,
        '2026-07-04',
        @status,
        CASE
            WHEN @status = 'Present' THEN 'On time'
            WHEN @status = 'Late' THEN 'Arrived late'
            ELSE 'No reason provided'
        END,
        @teacherId
    FROM Students s
    WHERE s.StudentId = @studentId;

    SET @studentId = @studentId + 1;
END
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

SELECT TOP 20
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

SELECT TOP 50
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
ORDER BY a.AttendanceDate DESC, st.FullName;
GO

PRINT 'XaaDirDB database created successfully.';
GO
