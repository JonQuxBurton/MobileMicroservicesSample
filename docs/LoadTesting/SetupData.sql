
-- SetupDataForCompleteProvision

declare 
@customerId uniqueidentifier,
@mobileId uniqueidentifier,
@mobileOrderId uniqueidentifier,
@phoneNumber varchar(50),
@contactName varchar(50)
;

set @customerId = 'C5C04D13-25B2-4EC2-97E0-99737673287F';
set @mobileId =  '0D070AAD-2897-4B2D-B03C-7D7894777856';
set @mobileOrderId = 'EE918282-2940-4453-9298-EE361FEDFB1B';
set @phoneNumber = '07001000001';
set @contactName = 'Neil One Armstrong';

exec SetupDataForCompleteProvision @customerId, @mobileId, @mobileOrderId, @phoneNumber, @contactName;


set @mobileId =  '08027DE6-C655-474C-8BD7-08D4A9186225';
set @mobileOrderId = 'A5463012-862C-490A-AC32-D52B63531328';
set @phoneNumber = '07001000002';
set @contactName = 'Neil Two Armstrong';

exec SetupDataForCompleteProvision @customerId, @mobileId, @mobileOrderId, @phoneNumber, @contactName;

go

-- SetupDataForActivate

declare 
@customerId uniqueidentifier,
@mobileId uniqueidentifier,
@activationCode varchar(50),
@phoneNumber varchar(50)
;
set @customerId = 'C5C04D13-25B2-4EC2-97E0-99737673287F';
set @mobileId =  'A12F2B7E-170E-408E-9451-85DC796A9C07';
set @phoneNumber = '07001000005';
set @activationCode = 'AAA005';

exec SetupDataForActivate @customerId, @mobileId, @phoneNumber, @activationCode;


set @mobileId =  '66606739-E37A-452A-A6A5-831D960C4AD8';
set @phoneNumber = '07001000006';
set @activationCode = 'AAA006';

exec SetupDataForActivate @customerId, @mobileId, @phoneNumber, @activationCode;

go

-- SetupDataForCompleteActivate 

declare 
@customerId uniqueidentifier,
@mobileId uniqueidentifier,
@mobileOrderId uniqueidentifier,
@phoneNumber varchar(50)
;

set @customerId = 'C5C04D13-25B2-4EC2-97E0-99737673287F';
set @mobileId =  '422DD8B1-BD6E-4594-8958-D8DC60D495B1';
set @mobileOrderId = '499FA140-310A-433A-ACF5-B3BAAD206553';
set @phoneNumber = '07001000007';

exec SetupDataForCompleteActivate @customerId, @mobileId, @mobileOrderId, @phoneNumber;


set @mobileId =  '9A479AB1-FD6C-4004-B0AA-A7D6BCF98344';
set @mobileOrderId = '2A894D82-F14A-489C-8E30-983A1FA7A358';
set @phoneNumber = '07001000008';

exec SetupDataForCompleteActivate @customerId, @mobileId, @mobileOrderId, @phoneNumber;

go
