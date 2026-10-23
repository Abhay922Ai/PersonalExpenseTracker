-- ========================================================
-- Database Creation Script for PersonalExpenseTracker
-- ========================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'PersonalExpenseTrackerDB')
BEGIN
    CREATE DATABASE PersonalExpenseTrackerDB;
END
GO

USE PersonalExpenseTrackerDB;
GO

-- 1. Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        Password NVARCHAR(255) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 2. Income Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Income')
BEGIN
    CREATE TABLE Income (
        IncomeId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Source NVARCHAR(100) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        IncomeDate DATETIME2 NOT NULL,
        Description NVARCHAR(500) NULL,
        CONSTRAINT FK_Income_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
    );
END
GO

-- 3. Expenses Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Expenses')
BEGIN
    CREATE TABLE Expenses (
        ExpenseId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Category NVARCHAR(100) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        ExpenseDate DATETIME2 NOT NULL,
        Description NVARCHAR(500) NULL,
        CONSTRAINT FK_Expenses_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
    );
END
GO

-- 4. SavingsGoals Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'SavingsGoals')
BEGIN
    CREATE TABLE SavingsGoals (
        GoalId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        GoalName NVARCHAR(100) NOT NULL,
        TargetAmount DECIMAL(18,2) NOT NULL,
        SavedAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        TargetDate DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_SavingsGoals_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
    );
END
GO
