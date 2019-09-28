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
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/MobileMsSampleDiagram1.png)
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/MobileMsSampleDiagram2.png)

## Technologies:
Cloud: [Localstack](https://github.com/localstack/localstack) - local AWS cloud stack

WebApi: ASP.NET Core

Data access: Dapper

Event-based collaboration: AWS SNS and SQS. JustSaying message bus

Unit tests: Moq, xUnit, FluentAssertions

Containers: Docker

## Launching
Launch the system using docker:
```shell
$ docker-compose up
```

## Walkthrough
(See Walkthrough.http)

### Order a Mobile:
```http
POST http://localhost:5000/api/provisioner
content-type: application/json

{
	"Name" : "Neil Armstrong",
	"ContactPhoneNumber" : "30123456789"
}
```

### Verify that the Order was created:
```shell
$ docker exec -it localstack-s3 bash
```
```shell
`$ /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "Pass@word"
```
```sql
select * from SimCardWholesaler.Orders;
go
```