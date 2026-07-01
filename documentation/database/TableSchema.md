# XaaDirDB Table Schema

## Users
UserId PK, FullName, Username, Email, Password, Role, IsActive, CreatedAt

## Classes
ClassId PK, ClassName, Section, Description, CreatedAt

## Subjects
SubjectId PK, SubjectName, TeacherUserId FK -> Users.UserId, Description, CreatedAt

## Students
StudentId PK, FullName, Gender, Phone, Email, ClassId FK -> Classes.ClassId, Status, CreatedAt

## Attendance
AttendanceId PK, StudentId FK, ClassId FK, SubjectId FK, AttendanceDate, Status, Remarks, MarkedByUserId FK, CreatedAt
