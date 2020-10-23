USE [Mobile]
GO

/*
Setup data for test: The External Service has completed a Mobile Provision Order
*/
drop PROCEDURE SetupDataForCompleteProvision;
go
CREATE PROCEDURE SetupDataForCompleteProvision 
@customerId uniqueidentifier, 
@mobileId uniqueidentifier, 
@mobileOrderId uniqueidentifier, 
@phoneNumber varchar(50), 
@contactName varchar(50),
@newMobileId int output
AS
begin
begin transaction

declare 
@mobileDbId int
;

delete from ExternalSimCardsProvider.Orders where Reference=@mobileOrderId;
delete from SimCards.Orders where MobileOrderId=@mobileOrderId;
delete from Mobiles.Orders where GlobalId=@mobileOrderId;
delete from Mobiles.Mobiles where GlobalId=@mobileId;

-- Create Mobile
insert into Mobiles.Mobiles ([GlobalId],[CustomerId],[State],[PhoneNumber]) 
values (@mobileId, @customerId, 'ProcessingProvision', @phoneNumber);

set @newMobileId = SCOPE_IDENTITY();

-- Create MobileOrder
insert into Mobiles.Orders ([MobileId],[GlobalId],[Name],[ContactPhoneNumber],[State],[Type]) 
values (@newMobileId, @mobileOrderId, @contactName, '00123456789', 'Sent', 'Provision');

-- Create SimCard Order
insert into SimCards.Orders ([PhoneNumber],[MobileId],[MobileOrderId],[Name],[Status]) 
values (@phoneNumber, @mobileId, @mobileOrderId, @contactName, 'Sent' );

-- Create External SimCard Order
insert into ExternalSimCardsProvider.Orders ([PhoneNumber],[Reference],[Status]) 
values (@phoneNumber, @mobileOrderId, 'New');

commit;
end;
go
