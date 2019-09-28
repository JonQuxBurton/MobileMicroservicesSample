# Mobile Microservices Sample
A small sample of a Microservices architecture for a simple Mobile Phone/Telecoms domain.

(Warning: This code is not suitable for Production use)


## Domain
User Story #1
As a SalesAgent I want to Order a Mobile.
Given: The Order details are captured
When: The Order is placed
Then: A SimCard Order is sent to the SimCardWholesaler

## Architecture
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/MobileMsSampleDiagram2.png)
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/MobileMsSampleDiagram1.png)

## Technologies:
Cloud: Localstack - a local AWS cloud stack
WebApi: ASP.NET Core
Data access: Dapper
Event-based collaboration: AWS SNS and SQS. JustSaying message bus
Unit tests: Moq, xUnit, FluentAsertions
Containers: Docker

## Launching
Launch the system using docker:
$ docker-compose up

## Walkthrough
(See Walkthrough.http)

### Order a Mobile:
POST http://localhost:5000/api/provisioner
content-type: application/json

{
	"Name" : "Neil Armstrong",
	"ContactPhoneNumber" : "30123456789"
}

### Verify that the Order was created:
$ docker exec -it localstack-s3 bash
$ /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "Pass@word"
 
select * from SimCardWholesaler.Orders;
go