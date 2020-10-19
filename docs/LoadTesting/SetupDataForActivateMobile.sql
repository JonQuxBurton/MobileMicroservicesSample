/*
Setup data for test: The External Service has completed a Mobile Provision Order
*/
drop PROCEDURE SetupDataForActivate;
go
CREATE PROCEDURE SetupDataForActivate 
@customerId uniqueidentifier, 
@mobileId uniqueidentifier,
@phoneNumber  varchar(50),
@activationCode varchar(50)
AS
begin
begin transaction

delete from ExternalMobileTelecomsNetwork.ActivationCodes where PhoneNumber=@phoneNumber;
--delete from Mobiles.Orders where GlobalId=@mobileOrderId;
delete from Mobiles.Mobiles where GlobalId=@mobileId;

-- Create Mobile
insert into Mobiles.Mobiles ([GlobalId],[CustomerId],[State],[PhoneNumber]) 
values (@mobileId, @customerId, 'WaitingForActivate', @phoneNumber);

-- Create External MobileTelecomsNetwork ActivationCode
insert into ExternalMobileTelecomsNetwork.ActivationCodes ([PhoneNumber],[Code]) 
values (@phoneNumber, @activationCode);

commit;
end;
go

declare 
@customerId uniqueidentifier,
@mobileId uniqueidentifier,
@activationCode varchar(50),
@phoneNumber varchar(50)
;

set @customerId = 'C5C04D13-25B2-4EC2-97E0-99737673287F';
set @mobileId =  'A12F2B7E-170E-408E-9451-85DC796A9C07';
set @phoneNumber = '07001000001';
set @activationCode = 'CMP238';

exec SetupDataForActivate @customerId, @mobileId, @phoneNumber, @activationCode;