/*
Setup data for test: The External Service has completed a Mobile Provision Order
*/
drop PROCEDURE SetupDataForCompleteProvision;
go
CREATE PROCEDURE SetupDataForCompleteProvision @customerId uniqueidentifier, @mobileId uniqueidentifier, @mobileOrderId uniqueidentifier
AS
begin
begin transaction

declare 
@phoneNumber varchar(50),
@mobileDbId int,
@contactName varchar(50)
;

set @phoneNumber = '07001000001';
set @contactName = 'Neil Armstrong';

delete from ExternalSimCardsProvider.Orders where Reference=@mobileOrderId;
delete from SimCards.Orders where MobileOrderId=@mobileOrderId;
delete from Mobiles.Orders where GlobalId=@mobileOrderId;
delete from Mobiles.Mobiles where GlobalId=@mobileId;

-- Create Mobile
insert into Mobiles.Mobiles ([GlobalId],[CustomerId],[State],[PhoneNumber]) 
values (@mobileId, @customerId, 'ProcessingProvision', @phoneNumber);

set @mobileDbId = SCOPE_IDENTITY();

-- Create MobileOrder
insert into Mobiles.Orders ([MobileId],[GlobalId],[Name],[ContactPhoneNumber],[State],[Type]) 
values (@mobileDbId, @mobileOrderId, @contactName, '00123456789', 'Sent', 'Provision');

-- Create SimCard Order
insert into SimCards.Orders ([PhoneNumber],[MobileId],[MobileOrderId],[Name],[Status]) 
values (@phoneNumber, @mobileId, @mobileOrderId, @contactName, 'Sent' );

-- Create External SimCard Order
insert into ExternalSimCardsProvider.Orders ([PhoneNumber],[Reference],[Status]) 
values (@phoneNumber, @mobileOrderId, 'New');

commit;
end;
go

declare 
@customerId uniqueidentifier,
@mobileId uniqueidentifier,
@mobileOrderId uniqueidentifier
;

set @customerId = 'C5C04D13-25B2-4EC2-97E0-99737673287F';
set @mobileId =  '0D070AAD-2897-4B2D-B03C-7D7894777856';
set @mobileOrderId = 'EE918282-2940-4453-9298-EE361FEDFB1B';

exec SetupDataForCompleteProvision @customerId, @mobileId, @mobileOrderId;