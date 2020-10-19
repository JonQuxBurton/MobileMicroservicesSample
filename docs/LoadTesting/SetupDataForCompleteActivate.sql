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

declare 
@customerId uniqueidentifier,
@mobileId uniqueidentifier,
@mobileOrderId uniqueidentifier,
@phoneNumber varchar(50)
;

set @customerId = 'C5C04D13-25B2-4EC2-97E0-99737673287F';
set @mobileId =  '422DD8B1-BD6E-4594-8958-D8DC60D495B1';
set @mobileOrderId = '499FA140-310A-433A-ACF5-B3BAAD206553';
set @phoneNumber = '07001000001';

exec SetupDataForCompleteActivate @customerId, @mobileId, @mobileOrderId, @phoneNumber;