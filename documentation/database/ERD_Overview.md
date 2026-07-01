# XaaDirDB ERD Overview

Users 1 -> many Subjects
Users 1 -> many Attendance
Classes 1 -> many Students
Classes 1 -> many Attendance
Subjects 1 -> many Attendance
Students 1 -> many Attendance

Teacher-specific permission:
Subjects.TeacherUserId = LoggedInTeacherUserId
