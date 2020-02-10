
CREATE DATABASE Mobile;
GO

USE Mobile;
GO

/*----------*/

CREATE SCHEMA [MobileOrderer]
GO

CREATE LOGIN MobileOrdererMicroservice WITH PASSWORD = 'MobileOrderer@123';
GO  
CREATE USER MobileOrdererMicroservice FOR LOGIN MobileOrdererMicroservice
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: MobileOrderer TO MobileOrdererMicroservice;
GO

CREATE TABLE [MobileOrderer].[Mobiles](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[GlobalId] [uniqueidentifier] NOT NULL,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[State] [nvarchar](100) NOT NULL
)
GO

CREATE TABLE [MobileOrderer].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[MobileId] [int] NOT NULL,
	[GlobalId] [uniqueidentifier] NOT NULL,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[Name] [nvarchar](100) NOT NULL,
	[ContactPhoneNumber] [nvarchar](50) NULL,
	[State] [nvarchar](100) NOT NULL,
	CONSTRAINT FK_Orders_Mobiles FOREIGN KEY (MobileId) REFERENCES [MobileOrderer].[Mobiles](Id)
)
GO

/*----------*/

CREATE SCHEMA [SimCards]
GO

CREATE LOGIN SimCardsMicroservice WITH PASSWORD = 'SimCards@123';
GO  
CREATE USER SimCardsMicroservice FOR LOGIN SimCardsMicroservice
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: SimCards TO SimCardsMicroservice;
GO

CREATE TABLE [SimCards].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[MobileOrderId] [uniqueidentifier] NOT NULL,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[Name] [nvarchar](100) NOT NULL,
	[Status] [nvarchar](100) NOT NULL
)
GO

/*----------*/

CREATE SCHEMA [SimCardWholesaler]
GO

CREATE LOGIN SimCardWholesalerWebService WITH PASSWORD = 'SimCardWholesaler@123';
GO  
CREATE USER SimCardWholesalerWebService FOR LOGIN SimCardWholesalerWebService
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: SimCardWholesaler TO SimCardWholesalerWebService;
GO

CREATE TABLE [SimCardWholesaler].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[Reference] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](100) NOT NULL
)
GO

