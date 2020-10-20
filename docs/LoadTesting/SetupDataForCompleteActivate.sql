/*
Setup data for test
*/
drop PROCEDURE SetupDataForCompleteActivate;
go
CREATE PROCEDURE SetupDataForCompleteActivate 
@customerId uniqueidentifier, 
@mobileId uniqueidentifier, 
@mobileOrderId uniqueidentifier,
@phoneNumber  varchar(50)
AS
begin
begin transaction

declare 
@mobileDbId int,
@contactName varchar(50)
;

set @contactName = 'Neil Armstrong';

delete from ExternalMobileTelecomsNetwork.Orders where Reference=@mobileOrderId;
delete from MobileTelecomsNetwork.Orders where MobileOrderId=@mobileOrderId;
delete from Mobiles.Orders where GlobalId=@mobileOrderId;
delete from Mobiles.Mobiles where GlobalId=@mobileId;

-- Create Mobile
insert into Mobiles.Mobiles ([GlobalId],[CustomerId],[State],[PhoneNumber]) 
values (@mobileId, @customerId, 'ProcessingActivate', @phoneNumber);

set @mobileDbId = SCOPE_IDENTITY();

-- Create MobileOrder
insert into Mobiles.Orders ([MobileId],[GlobalId],[Name],[ContactPhoneNumber],[State],[Type]) 
values (@mobileDbId, @mobileOrderId, @contactName, '00123456789', 'Sent', 'Activate');

-- Create MobileTelecomsNetwork Order
insert into MobileTelecomsNetwork.Orders ([Type],[MobileId],[MobileOrderId],[PhoneNumber],[Name],[Status]) 
values ('Activate', @mobileId, @mobileOrderId, @phoneNumber, @contactName, 'Sent' );

-- Create External ExternalMobileTelecomsNetwork Order
insert into ExternalMobileTelecomsNetwork.Orders ([PhoneNumber],[Reference],[Status],[Type]) 
values (@phoneNumber, @mobileOrderId, 'New', 'Activate');

commit;
end;
go
