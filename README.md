# AuctionWebApp

# Please run these database script in MSSQL

CREATE DATABASE AuctionDB

USE AuctionDB

CREATE TABLE AuctionItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX),
    StartingPrice DECIMAL(18,0) NOT NULL,
    CurrentPrice DECIMAL(18,0) NOT NULL,
    EndTime DATETIME NOT NULL,
	ImageFileName NVARCHAR(MAX) NOT NULL,
    UpdatedBy NVARCHAR(50) NOT NULL,
    UpdatedAt DATETIME DEFAULT GETDATE()
)

CREATE TABLE Bids (
    Id INT PRIMARY KEY IDENTITY(1,1),
	AuctionItemId INT FOREIGN KEY (AuctionItemId) REFERENCES AuctionItems(Id) NOT NULL,
	BidderName NVARCHAR(100) NOT NULL,
    BidAmount DECIMAL(18, 2) NOT NULL,
    BidTime DATETIME DEFAULT GETDATE() NOT NULL,
    IsAutoBid BIT NOT NULL,
)
