
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

CREATE SCHEMA [SimCards]
GO

CREATE LOGIN SimCardsMicroservice WITH PASSWORD = 'SimCards@123';
GO  
CREATE USER SimCardsMicroservice FOR LOGIN SimCardsMicroservice
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: SimCards TO SimCardsMicroservice;
GO

CREATE SCHEMA [ExternalSimCardsProvider]
GO

CREATE LOGIN ExternalSimCardsProviderWebService WITH PASSWORD = 'ExternalSimCardsProvider@123';
GO  
CREATE USER ExternalSimCardsProviderWebService FOR LOGIN ExternalSimCardsProviderWebService
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: ExternalSimCardsProvider TO ExternalSimCardsProviderWebService;
GO

CREATE SCHEMA [MobileTelecomsNetwork]
GO

CREATE LOGIN MobileTelecomsNetworkMicroservice WITH PASSWORD = 'MobileTelecomsNetwork@123';
GO  
CREATE USER MobileTelecomsNetworkMicroservice FOR LOGIN MobileTelecomsNetworkMicroservice
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: MobileTelecomsNetwork TO MobileTelecomsNetworkMicroservice;
GO

CREATE SCHEMA [ExternalMobileTelecomsNetwork]
GO

CREATE LOGIN ExternalMobileTelecomsNetworkWebService WITH PASSWORD = 'ExternalMobileTelecomsNetwork@123';
GO  
CREATE USER ExternalMobileTelecomsNetworkWebService FOR LOGIN ExternalMobileTelecomsNetworkWebService
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: ExternalMobileTelecomsNetwork TO ExternalMobileTelecomsNetworkWebService;
GO

/*----------*/

CREATE TABLE [MobileOrderer].[Customers](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[GlobalId] [uniqueidentifier] NOT NULL,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[Name] [nvarchar](100) NULL
)
GO

CREATE TABLE [MobileOrderer].[Mobiles](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[GlobalId] [uniqueidentifier] NOT NULL,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[CustomerId] [uniqueidentifier] not null,
	[State] [nvarchar](100) NOT NULL,
	[PhoneNumber] [nvarchar](50) NOT NULL
)
GO

CREATE TABLE [MobileOrderer].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[MobileId] [int] NOT NULL,
	[GlobalId] [uniqueidentifier] NOT NULL,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[Name] [nvarchar](100) NULL,
	[ContactPhoneNumber] [nvarchar](50) NULL,
	[ActivationCode] [nvarchar](50) NULL,
	[State] [nvarchar](100) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	CONSTRAINT FK_Orders_Mobiles FOREIGN KEY (MobileId) REFERENCES [MobileOrderer].[Mobiles](Id)
)
GO

CREATE TABLE [SimCards].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PhoneNumber] [nvarchar](50) NOT NULL,
	[MobileId] [uniqueidentifier] NOT NULL,
	[MobileOrderId] [uniqueidentifier] NOT NULL,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[Name] [nvarchar](100) NOT NULL,
	[Status] [nvarchar](100) NOT NULL
)
GO

CREATE TABLE [ExternalSimCardsProvider].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[PhoneNumber] [nvarchar](50) NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](100) NOT NULL,
	[ActivationCode] [nvarchar](50) NULL
)
GO

CREATE TABLE [MobileTelecomsNetwork].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Type]  [nvarchar](100) NOT NULL,
	[MobileId] [uniqueidentifier] NOT NULL,
	[MobileOrderId] [uniqueidentifier] NOT NULL,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[PhoneNumber] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Status] [nvarchar](100) NOT NULL
)
GO

CREATE TABLE [ExternalMobileTelecomsNetwork].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[PhoneNumber] [nvarchar](50) NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](100) NOT NULL,
	[Type]  [nvarchar](100) NOT NULL,
	[ActivationCode] [nvarchar](50) NULL,
	[Reason] [nvarchar](100) NULL
)
GO

CREATE TABLE [ExternalMobileTelecomsNetwork].[ActivationCodes](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CreatedAt] datetime DEFAULT GETDATE() NOT NULL,
	[UpdatedAt] datetime,
	[PhoneNumber] [nvarchar](50) NOT NULL,
	[Code] [nvarchar](50) NULL
)
GO

/*----------*/