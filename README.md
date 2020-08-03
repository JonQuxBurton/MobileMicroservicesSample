# Mobile Microservices Sample
A small sample of a Microservices architecture for a simple Mobile Phone/Telecoms domain.

(Warning: This code is not suitable for Production use)

## Domain
User Story #1

As a SalesAgent I want to Order a Mobile.

Given: The Order details are captured

When: The Order is placed

Then: A SimCard Order is sent to the ExternalSimCardsProvider

## Architecture
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/MobileMsSampleDiagram1.png)
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/MobileMsSampleDiagram2.png)

## Technologies:
|               |               |
| ------------- | ------------- |
| Cloud         | [GoAWS](https://github.com/p4tin/goaws) - local AWS SNS/SQS system |
| WebApi        | ASP.NET Core  |
| Data access   | Dapper        |
| Event-based collaboration | AWS SNS and SQS. JustSaying message bus |
| Unit tests    | Moq, xUnit, FluentAssertions |
| Containers    | Docker        |

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
$ docker exec -it goaws bash
```
```shell
$ /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "Pass@word"
```
```sql
select * from ExternalSimCardsProvider.Orders;
go
```

## Testing Strategy

My plan is to test the system from the bottom with unit tests and from the top with manual and automated tests. 
These are illustrated on the standard testing pyramid:

![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/TestingStrategy.png)

The rough positions of the three types of tests are shown with the numbers:
1. Unit Tests
2. API level End-to-end Tests
3. API level Manual Tests


#### 1. Unit Tests

These are standard unit tests which test individual units in isolation and are fast running. 

#### 2. API level End-to-end Tests
These test each of the most important Scenarios which the system can perform. 

These are not true "End-to-end" tests since they are at the API level rather than the UI. I choose to use the API level as a seam to test against as this should provide confidence that the back-end system has not regressed.

The tests are executed against a test system which is started using docker-compose. This launches the following:
* each service in a seperate containers
* the SQL Server database in a container
* GoAws - a simulation of AWS SNS/SQS in a container

The tests are then executed against this test system and verified by querying the database.

#### 3. API level Manual Tests
These also test the most important Scenarios which the system can perform.

They are run against the same test system as above, launched through docker-compose.
Once the system is running they are executed manually by using the Visual Studio Code [REST client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) and executing the Scenarios in the file:
\docs\ManualTesting\ExecuteScenarios.http


### Executing the Tests

#### 1. Unit Tests

Execute using the Visual Studio test runner.

#### 2. API level End-to-end Tests
* Start Docker Desktop
* Build the test system: 
```
docker-compose -f docker-compose-test.yml -f docker-compose-test.override.yml build
```
* Launch the test system 
```
docker-compose -f docker-compose-test.yml -f docker-compose-test.override.yml up
```
* Wait a minute
* Verify the test system has successfully launched by  executing the checks in the file \docs\ManualTesting\CheckSystem.http
* Verify the database is running by running the SQL script \docs\ManualTesting\CheckDatabase.sql

Execute the tests in the EndToEndApiLevelTests project using the Visual Studio test runner.

#### 3. API level Manual Tests
* Start Docker Desktop
* Build the test system: 
```
docker-compose -f docker-compose-test.yml -f docker-compose-testoverride.yml build
```
* Launch the test system 
```
docker-compose -f docker-compose-test.yml -f docker-compose-testoverride.yml up
```
* Wait a minute
* Verify the test system has successfully launched (see above)

* Open \docs\ManualTesting\ExecuteScenarios.http in Visual Studio Code
* Click 'Send Request' against the first Scenario
* Verify the changes in the database by running the SQL script: \docs\ManualTesting\ClearDatabase.sql
* Repeat for the other Scenarios

### The Scenarios

1. Order a Mobile	
2. Mobile Order Completed
3. Activate a Mobile
4. Activate Order Completed


#### 1. Order a Mobile
Inputs:
* POST Order to the Mobile Orderer Web Service

Outputs:
* Updates Mobiles database
	* Mobile State to ProcessingProvisioning
	* Order State to Sent
* Updates SIM Cards database
	* Order State to Sent
* Calls External SIM Card system

#### 2. Mobile Order Completed
Inputs:
* Complete the Mobile Order in the External SIM Card system

Outputs:
* Updates SIM Cards database
	* Order State to Completed
* Updates Mobiles database
	* Mobile State to WaitingForActivation
	* Order State to Completed
	
#### 3. Activate a Mobile
Inputs:
* POST Activate Order to the Mobile Orderer Web Service

Outputs:
* Updates Mobiles database
	* Mobile State to ProcessingActivation
	* Order State to Sent 
* Updates Mobile Telecoms Network database
	* Order State to Sent
* Calls External Mobile Telecoms Network system

#### 4. Activate Order Completed
Inputs:
* Complete the Activate Order in the External Mobile Telecoms Network system

Outputs:
* Updates Mobile Telecoms Network database
	* Order State to Completed
* Updates Mobiles database
	* Mobile State to Live
	* Order State to Completed

## Logging

Logging in the system uses the Serilog library. This supports strutured logging in which log entries include data, rather than being just plain text. This allows the logs to be more easily searched, filtered and analysed to assist in diagnosing problems.

Theses logs are also push ed to Seq which produces a dashboard where they can be viewed and queried.

The Seq Dashboard can be viewed at:
```
http://localhost:5341
```

![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/SeqDashboard.png)

## Monitoring

The system is monitored, so we can verify that it is functioning correctly and prevent problems before they escalate. System Metrics are gathered using Prometheus and displayed in Grafana Dashboards.

To test the system and generate metrics, run the End-to-end tests, then observe in the Prometheus Control Panel and Grafana Dashboard.

### Prometheus Control Panel
The Prometheus Control Panel can be viewed at:
```
http://localhost:9090
```

View the metrics using the following PromQL queries:

#### Mobiles System

| PromQL | Description |
| ---- | ---- |
| mobile_provisions | Total number of Mobile Provisions requested |
| mobile_provisions_completed | Total number of Mobile Provisions completed |
| mobile_provisions_inprogress | Current number of Mobile Provisions in progress |
| mobile_activates | Total number of Mobile Activates requested |
| mobile_activates_completed | Total number of Mobile Activates requested |
| mobile_activates_inprogress | Current number of Mobile Activates in progress |
| mobile_ceases | Total number of Mobile Ceases requested |
| mobile_ceases_completed | Total number of Mobile Ceases requested |
| mobile_ceases_inprogress | Current number of Mobile Ceases in progress |

#### External SIM Cards Provider
| PromQL | Description |
| ---- | ---- |
| simcard_orders_sent | Total number of SIM Card orders sent |
| simcard_orders_completed | Total number of SIM Card orders completed |
| simcard_orders_inprogress | Current number of SIM Card orders in progress |
| simcard_orders_failed| Total number of SIM Card orders which failed |

#### External Mobile Telecoms Network
| PromQL | Description |
| ---- | ---- |
| mobiletelecomsnetwork_activate_orders_sent | Total number of Activate orders sent |
| mobiletelecomsnetwork_activate_orders_completed | Total number of Activate orders completed |
| mobiletelecomsnetwork_activate_orders_inprogress | Current number of Activate orders in progress |
| mobiletelecomsnetwork_activate_orders_failed | Total number of Activate orders which failed|
| mobiletelecomsnetwork_cease_orders_sent | Total number of Cease orders sent |
| mobiletelecomsnetwork_cease_orders_completed | Total number of Cease orders completed |
| mobiletelecomsnetwork_cease_orders_inprogress | Current number of Cease orders in progress |
| mobiletelecomsnetwork_cease_orders_failed | Total number of Cease orders which failed |

### Grafana Dashboard
The Grafana Dashboard can be viewed at:
```
http://localhost:3000
```
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/GrafanaDashboard.png)





