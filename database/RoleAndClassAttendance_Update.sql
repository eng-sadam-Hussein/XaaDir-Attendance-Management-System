USE XaaDirDB;
GO

IF EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_Users_Role'
)
BEGIN
    ALTER TABLE Users DROP CONSTRAINT CK_Users_Role;
END
GO

ALTER TABLE Users
ADD CONSTRAINT CK_Users_Role
CHECK (Role IN ('Admin', 'Teacher', 'TeacherAdmin'));
GO

SELECT UserId, FullName, Username, Role, IsActive
FROM Users
ORDER BY UserId;
GO
