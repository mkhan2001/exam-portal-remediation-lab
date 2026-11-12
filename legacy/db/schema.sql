-- ExamPortal legacy sample database (SQL Server / LocalDB).
-- Run against a new database named ExamPortalLegacy:
--   sqllocaldb start MSSQLLocalDB
--   sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE ExamPortalLegacy"
--   sqlcmd -S "(localdb)\MSSQLLocalDB" -d ExamPortalLegacy -i schema.sql
-- Seed logins: alice / Passw0rd!   bob / Winter2024   admin / admin123

IF OBJECT_ID('dbo.Candidates') IS NOT NULL DROP TABLE dbo.Candidates;
IF OBJECT_ID('dbo.Users') IS NOT NULL DROP TABLE dbo.Users;

CREATE TABLE dbo.Users (
    Id INT NOT NULL PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(200) NOT NULL,
    Role NVARCHAR(20) NOT NULL
);

CREATE TABLE dbo.Candidates (
    Id INT NOT NULL PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Notes NVARCHAR(2000) NULL,
    OwnerUserId INT NOT NULL
);

INSERT INTO dbo.Users (Id, Username, PasswordHash, Role) VALUES
    (1, 'alice', '47b7bfb65fa83ac9a71dcb0f6296bb6e', 'Scheduler'),
    (2, 'bob',   '2a28bf27d9b2f12a7213bf4635f3f1c6', 'Scheduler'),
    (3, 'admin', '0192023a7bbd73250516f069df18b500', 'Admin');

INSERT INTO dbo.Candidates (Id, FirstName, LastName, Email, Notes, OwnerUserId) VALUES
    (1, 'Maria',  'Alvarez',  'maria.alvarez@example.com',  'Needs extended time.', 1),
    (2, 'Devon',  'Brooks',   'devon.brooks@example.com',   NULL, 1),
    (3, 'Priya',  'Chandra',  'priya.chandra@example.com',  'Rescheduled twice.', 2),
    (4, 'Samuel', 'O''Brien', 'samuel.obrien@example.com',  NULL, 2);
