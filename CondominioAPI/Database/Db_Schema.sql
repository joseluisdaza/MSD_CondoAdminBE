-- Database Schema for Condominio Management System
-- This script creates the necessary tables and relationships for managing users, roles, properties, expenses, and payments.
-- It ensures that the database and tables are created only if they do not already exist.
-- v1.2
CREATE DATABASE
IF NOT EXISTS Condominio;
USE Condominio;

CREATE TABLE IF NOT EXISTS Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Login VARCHAR(30) NOT NULL UNIQUE,
    User_Name VARCHAR(100) NOT NULL,
    Last_Name VARCHAR(100) NOT NULL,
    Legal_Id VARCHAR(1000) NULL,
    Start_Date DATETIME NOT NULL,
    End_Date DATETIME NULL,
    PASSWORD VARCHAR(1000) NOT NULL
);

CREATE TABLE IF NOT EXISTS Roles
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Rol_Name VARCHAR(50) NOT NULL,
  Description VARCHAR(1000)
);

CREATE TABLE IF NOT EXISTS User_Roles
(
  Role_Id INT NOT NULL,
  User_Id INT NOT NULL,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,

  PRIMARY KEY (Role_Id, User_Id),
  FOREIGN KEY (Role_Id) REFERENCES Roles(Id),
  FOREIGN KEY (User_Id) REFERENCES Users(Id)
);

CREATE TABLE IF NOT EXISTS Property_Type
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Type VARCHAR (100) NOT NULL,
  Description VARCHAR (500) NOT NULL,
  Rooms INT DEFAULT 0,
  Bathrooms INT DEFAULT 0,
  Water_Service BOOLEAN DEFAULT FALSE,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL
);

CREATE TABLE IF NOT EXISTS Property
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Legal_Id VARCHAR(100) NULL,
  Tower VARCHAR(20) NOT NULL,
  Floor INT NOT NULL,
  Code VARCHAR(2) NOT NULL,
  Property_Type INT NOT NULL,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,

  FOREIGN KEY (Property_Type) REFERENCES Property_Type (Id)
);

CREATE TABLE IF NOT EXISTS Property_Owners
(
  Property_Id INT NOT NULL,
  User_Id INT NOT NULL,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,

  PRIMARY KEY (Property_Id, User_Id),
  FOREIGN KEY (Property_Id) REFERENCES Property (Id),
  FOREIGN KEY (User_Id) REFERENCES Users (Id)
);

CREATE TABLE IF NOT EXISTS Expense_Categories
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Category VARCHAR(100) NOT NULL,
  Description VARCHAR(500) NOT NULL
);

CREATE TABLE IF NOT EXISTS Expenses
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Receive_Number VARCHAR(100) NOT NULL,
  Category_Id INT NOT NULL,
  Property_Id INT,
  Start_Date DATETIME NOT NULL,
  Payment_Limit_Date DATETIME NOT NULL,
  Amount DECIMAL(10,2) NOT NULL,
  Interest_Amount DECIMAL(10,2) DEFAULT 0,
  Interest_Rate DECIMAL(5,2) DEFAULT 0,
  Description VARCHAR(500) NOT NULL,
  Status INT NOT NULL DEFAULT 1,  -- 1: Pending, 2: Paid, 3: Overdue, 4: Cancelled
  Expense_Date DATETIME NOT NULL,
  
  FOREIGN KEY (Category_Id) REFERENCES Expense_Categories (Id),
  FOREIGN KEY (Property_Id) REFERENCES Property (Id)
);

CREATE TABLE IF NOT EXISTS Payments
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Expense_Id INT NOT NULL,
  Payment_Date DATETIME NOT NULL,
  Amount DECIMAL(10,2) NOT NULL,
  Description VARCHAR(500) NULL,
  Receive_Photo VARCHAR(1000) NOT NULL,

  FOREIGN KEY (Expense_Id) REFERENCES Expenses (Id)
);

