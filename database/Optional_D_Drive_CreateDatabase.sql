/*
Optional D Drive Create Database Version

Use only if you want SQL Server .mdf/.ldf files on D drive.
Before running, create:
D:\SQLData\XaaDirDB
*/

USE master;
GO

IF DB_ID('XaaDirDB') IS NOT NULL
BEGIN
    ALTER DATABASE XaaDirDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE XaaDirDB;
END
GO

CREATE DATABASE XaaDirDB
ON PRIMARY
(
    NAME = N'XaaDirDB',
    FILENAME = N'D:\SQLData\XaaDirDB\XaaDirDB.mdf',
    SIZE = 20MB,
    MAXSIZE = 1024MB,
    FILEGROWTH = 10MB
)
LOG ON
(
    NAME = N'XaaDirDB_Log',
    FILENAME = N'D:\SQLData\XaaDirDB\XaaDirDB_Log.ldf',
    SIZE = 10MB,
    MAXSIZE = 512MB,
    FILEGROWTH = 10MB
);
GO
