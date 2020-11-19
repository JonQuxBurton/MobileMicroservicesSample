# Mobile Microservices Sample

# Overview

A sample of a Microservices architecture for the domain of a simple Mobile Phone/Telecoms domain.
To explore some of the different aspects of a microservices architecture.

(Warning: This code is not suitable for Production use)

## Walkthrough

You can try out the system by executing a Walkthrough of the Sceanrio - Order a Mobile, as follows:

Prerequissites:
Install and launch Docker
Install Visual Studio Code
Install REST Client Visual Studio Code plugin

1. Launch the system using docker: `$ docker-compose up`
1. Wait a minute
1. Verify the test system has successfully launched by  executing the checks in the file `\docs\ManualTesting\CheckSystem.http`
1. Connect to the SQL Server database using the Admin account: `localhost,5433`, Login: `SA`, Password: `Pass@word`
1. Verify the database is running by running the SQL script `\docs\ManualTesting\CheckDatabase.sql`
1. Open the file `docs\ManualTesting\ExecuteScenarios.http`
1. Create a Customer - Click `Send Request` under `### Step 1 - Create a Customer`
1. Observe the Customer was created with the SQL: `select * from Mobiles.Customers`
1. Order a Mobile - Click `Send Request` under `### Step 2 - Order a Mobile`
1. Observe the Mobile was created with the SQL: `select * from Mobiles.Mobiles`
1. Observe the Mobile Order was created with the SQL: `select * from Mobiles.Orders`
1. Simulate the External SIM Card System completing the Mobile Provision Order - Click `Send Request` under `### Step 3 - The External Service has completed the Mobile Provision Order`
1. Observe that the Mobile is `WaitingForActivate` with the SQL: `select * from Mobiles.Mobiles`
1. When the External SIM Card System has processed the Order the Activation Code would then be mailed to the Customer to complete the Activaion

# Background
## What are Microservices?
Microservices - also known as the microservice architecture - is an architectural style that structures an application as a collection of services that are

* Highly maintainable and testable
* Loosely coupled
* Independently deployable
* Organized around business capabilities
* Owned by a small team

The microservice architecture enables the rapid, frequent and reliable delivery of large, complex applications. It also enables an organization to evolve its technology stack.   
From [microservices.io](#microservices.io)

## Aspects of Microservices
| Aspect        |               	| Implementation
| ------------- | ------------------|------------- |
|Data			| 					|SQL Server, Dapper, Entity Framework
|				|Sovereignty		|Seperate database schemas adn logins
|Communication	|					|
|				|Event Bus			|AWS SNQ, SQS. JustSaying. Minimal Event Bus. GoAws
|				|Outbox Pattern		|In Mobiles.API, EventPublisherService and Checkers
|API Gateway	|					|Not Implemented
|Resiliency		|					|
|				|Retry				|Polly/HttpClientFactory
|				|Health Monitoring	|ASP.NET Core Health Checks Middleware
|				|Observability		|Promethues, Grafana
|				|Structured Logging	|Serilog, Seq
|				|Distributed Tracing|Not Implemented
|Security		|					|Not Implemented
|Testing		|					|
|				|Manual				|Walkthroughs using the [REST Client Visual Studio Code plugin](#REST-client)
|				|Unit tests			|xUnit, Moq, Fluent assertions
|				|End-to-end tests	|Scenario Scripts in EndToEndApiLevelTests
|				|Load tests			|k6 Load Testing tool
|Deployment		|					|
|				|Containers			|docker-compose
|Documentation	|					|Markdown, Readme.md (this document)
|				|Domain				|Sequence diagrams (sequencediagram.org), Statecharts (smcat)
|				|Architecture		|[4+1 Model](#4+1-Model), [C4 Model](#C4-Model), [C4 PlantUML Visual Studio Code plugin](#C4-PlantUML), LADRs (TODO)

## Descriptions of the Aspects of Microservices

### Data
Each microservice should be independent from all the others, so it should own it's own data, this is known as Data Soveriengty. This allows the schema of the data to evolve without impacting any other services.

### Communication
As the services should be loosley coupled, we avoid long chains of service calls which could result in temporal coupling. Instead services use the Publish/Subscribe pattern to communicate asynchronously using an Event Bus (also known as Message Bus).

### API Gateway
If the Front end apps call the microservices directly, then they are coupled and changes to the back end will also require updates to the front end. 
An API Gateway (also known as a Back end-for-Front end (BFF)) can sit inbetween and act as a facade for the front end. It can perform re-routing of inbound requests and aggregating together the responses into a single response. The back end microservices can then evolve independently from the front end.

### Resiliency
To keep the system resilient we should provide:
* Retry mechnism - retry failed requests, so that temporary or intermittent faults do not cause downtime
* Health checks - a way to check on the health of services
* Structured logging - log the details of what the service has done, in a format which ca be easily searched
* Metrics - to observe the functioning of the system as a whole

### Security
Microservices should perform the standard security practices of Authentication (verifying  identify) and Authroization (verifying the principal has the correct claims). Performance shoud also be considered since multiple microservices may be involved in processing a request, each needing to do security checks. Also microservices should not be abe to access each others data.

## Testing
Indivudal microservices can be tested with automated, fast running unit tests. These can keep the code from breaking and also help improve the design and keep components loosely coupled. 
Testing multiple services interacting with each other is more complicated. There are multiple ideas about how to do this, such as:
* End-to-end tests driven through a UI automation tool
* Testing services using faked dependencies such as the EF Core In-Memory database
* Running tests againat a simulated system running in Docker containers. For example using GoAws to simulate AWS SNS/SQS

Ultimately whatever approach is taken the goal is to have tests which can give confidence that the system as a whole works (and has not regressed) and are automated so they reduce the need for manual testing.

### Deployment
Microservices can be deployed in many ways. One way is by using containers, which allow each instance of a service to run in isolation from all others. The container can also be restarted if it crashes, by an Orchestrator. Another benefit of this approach is that test systems can be easily created and deployed using the same techniques.

### Documentation
Not directly related to microservices, but documentation is essential for future developers to quickly familiarise themselves with the system and for long term maintainability.
We should document:
* The domain - record the business processes so future developers can see what problem the system is solving
* The architecutre - record the high level structure of the system so future developers can see how the pieces fit together

# Documentation
## The Domain

The system is a Mobile Mobile Phone/Telecoms which supports the ordering and cancelling of Mobile Phones. The Ordering process is illustrated in the following diagram and User Stories:

![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/Order_a_Mobile.png)

```
User Story - Order a Mobile

As a SalesAgent I want to Order a Mobile  
Given: The Order details are captured  
When: The Order is placed  
Then: A SimCard Order is sent to the External Sim Cards Provider  
Then: The Mobile is saved in state WaitingForActivation  
```

```
User Story - Activate a Mobile

As a Sales Agent I want to Activate a Mobile  
Given: The Mobile is in state AwaitingActivation
When: The Activation Code is supplied  
Then: The Activation Code is sent to the External Mobile Telecoms Network  
Then: The Mobile is in state Live  
```

### Mobile States
During it's lifecycle a Mobile can transition between a number of states. These states such as New, ProcessingProvision, Live, etc. are shown in the Mobile States statechart.

In order to transition to a new state an Order must be sent to an External system. This In-flight order then progresses through it's own states of New, Processing, Sent and Completed. Adding these into the diagram results in the Mobile States with In-flight Order States statechart.

The greyed out states have not been implemented at present.

For information on statecharts, see [Welcome to Statecharts](#Welcome-to-Statecharts) and [Statecharts](#Statecharts).

#### Mobile States
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/MobileOverviewStatechart.png)

### Mobile States with In-flight Order States
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/MobileDetailedStatechart.png)


## Architecture
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/MobileMsSampleDiagram1.png)
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/MobileMsSampleDiagram2.png)

### Visualisation
The following diagrams map the architecture of the system.
They following the [4+1 Model](#4+1-Model), the [C4 Model](#C4-Model) and us the [C4 PlantUML Visual Studio Code plugin](#C4-PlantUML).

#### System Context Diagram

![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/C4Diagrams/System%20Context%20diagram%20for%20the%20Mobile%20system.png)

Source: \docs\MobileC4Context.puml

#### Containers Diagram

![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/C4Diagrams/Containers%20diagram%20for%20the%20Mobile%20system.png)

Source: \docs\MobileC4Containers.puml

#### Components Diagram

![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/C4Diagrams/Components%20diagram%20for%20the%20Mobile%20system%20-%20Mobiles%20Microservice.png)

Source: \docs\MobileC4Components.puml

# Testing

## Testing Goals

The goals of the tests are to ensure that the system is Reliable, Robust, Modifiable, Understandable and has acceptable Performance.
* Reliable - the users can use the system as intended and without encountering bugs.
* Robust - the system operates for an extended period of time without crashing.
* Modifiable - the system can be adapted to meet future requirements.
* Understandable - the system design is not more complicated than it needs to be.
* Performance - the system has acceptable performance.

## Testing Strategy

To achieve these goals, my plan is to test the system from the bottom up, with unit tests and from the top down with manual and automated tests. 
These are illustrated on the standard testing pyramid:

![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/TestingStrategy.png)

The rough positions of the three types of tests are shown with the numbers:
1. Unit Tests
2. API level Automated End-to-end Tests
3. API level Manual Tests
4. Load Tests

### 1. Unit Tests

These are standard unit tests which test individual units in isolation and are fast running. 

These support the testing goals as follows:
* Reliable - ensure each unit is reliable when run in isolation. 
* Modifiable - they are fast to run and so can be run after every code change to prevent regressions.
* Understandable - the code can be refactored and simplified, then the tests can be re-run to check for and prevent regressions.

### 2. API level Automated End-to-end Tests
These test each of the most important Scenarios which the system can perform. 

These are not true "End-to-end" tests since they are at the API level rather than the UI. I choose to use the API level as a seam to test against as this should provide confidence that the back end system has not regressed.

The tests are executed against a test system which is started using docker-compose. This launches the following:
* each service running in a seperate container
* the SQL Server database running in a container
* GoAws - a simulation of AWS SNS/SQS running in a container

The tests are then executed against this test system and verified by querying the database.

These support the testing goals as follows:
* Reliable - they match the primary use cases of the system, and so verify that the system works as intended.
* Modifiable - after the system is modified, the tests can be re-run to check for and prevent regressions.
* Understandable - the system can be refactored and simplified,, then the tests can be re-run to check for and prevent regressions.

### 3. API level Manual Tests
These also test the most important Scenarios which the system can perform.

They are run against the same test system as above, launched through docker-compose.
Once the system is running they are executed manually by using the [REST Client Visual Studio Code plugin](#REST-client) and executing the Scenarios in the file:
\docs\ManualTesting\ExecuteScenarios.http

These support the testing goals as follows:
* Reliable - they can be run manaully to aid in troubleshooting and diagnosing.
* Understandable - they allow each step of a Scenario can be run and checked.

### 4. Load Tests
The Load test are performed using [k6](#k6), which is a command line Load testing tool. The tests to be executed by k6 are defined in a JavaScript file (/docs/LoadTesting/LoadTest.js). This script details the actions to be performed (the Scenarios) and defines the number of Virtual Users and iterations.
It is currently set to launch 5 simultaneous Virtual Users each performing 3 iterations of one of 5 scenarios. So 5 * 5 * 3 = 75 tests in total.

During the test run, the Virtual User needs data to use for the current test iteration. This is pre-generated into a JSON file by the LoadTestingWebService, which I created. This also allows each Virtual User to request an Identifier, which it can use to ensure it gets it's own specific data for each test iterations that it runs.

The Load Tests support the testing goals as follows:
* Reliable - the Load Test Scenarios match the primary use cases of the system, and so verify that the system works as intended.
* Robust - they simulate a number of users simultaneously using the system and verify that it does not crash.
* Modifiable - after the system is modified, the tests can be re-run to check for and prevent regressions.
* Understandable - the system can be refactored and simplified, then the tests can be re-run to check for, and prevent regressions.
* Performance - check and benchamrk the performace of the system with a number of simultaneous users.

## Executing the Tests

### 1. Unit Tests

Execute using the Visual Studio test runner.

### 2. API level End-to-end Tests
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

### 3. API level Manual Tests
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

* Open \docs\ManualTesting\ExecuteScenarios.http in the REST Client Visual Studio Code plugin
* Click 'Send Request' against the first Step of the first Scenario
* Verify the changes in the database by running the SQL script: \docs\ManualTesting\CheckDatabase.sql
* Repeat for the other Steps and Scenarios

### 4. Load Tests
* Install k6 and Cmder (optional)
* Start Docker Desktop
* Launch the test system
```
λ docker-compose -f docker-compose-test.yml -f docker-compose-test.override.yml up
```
* Launch the LoadTestingWebApp
```
λ cd docs\LoadTesting\LoadTestingWebService\LoadTestingWebService
λ dotnet run
```
* Run the k6 Load Tests (will take some time). Open a new Cmder tab:
```
λ cd docs\LoadTesting
λ k6 run LoadTest.js
```

#### Output
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MobileMicroservicesSample/master/docs/K6-Output.png)

## The Scenarios

1. Create a Customer
1. Order a Mobile
1. Mobile Order Completed
1. Activate a Mobile
1. Activate Order Completed

### 1. Create a Customer
Inputs:
* POST Customer to the Mobiles Web Service

Outputs:
* Updates Mobiles database
	* Creates a Customer

### 2. Order a Mobile
Inputs:
* POST Order to the Mobiles Web Service

Outputs:
* Updates Mobiles database
	* Mobile State to ProcessingProvisioning
	* Order State to Sent
* Updates SIM Cards database
	* Order State to Sent
* Calls External SIM Card system

### 3. Mobile Order Completed
Inputs:
* Complete the Mobile Order in the External SIM Card system

Outputs:
* Updates SIM Cards database
	* Order State to Completed
* Updates Mobiles database
	* Mobile State to WaitingForActivation
	* Order State to Completed
	
### 4. Activate a Mobile
Inputs:
* POST Activate Order to the Mobiles Web Service

Outputs:
* Updates Mobiles database
	* Mobile State to ProcessingActivation
	* Order State to Sent 
* Updates Mobile Telecoms Network database
	* Order State to Sent
* Calls External Mobile Telecoms Network system

### 5. Activate Order Completed
Inputs:
* Complete the Activate Order in the External Mobile Telecoms Network system

Outputs:
* Updates Mobile Telecoms Network database
	* Order State to Completed
* Updates Mobiles database
	* Mobile State to Live
	* Order State to Completed

# Resiliency

## Logging

Logging in the system uses the Serilog library. This supports strutured logging in which log entries include data, rather than being just plain text. This allows the logs to be more easily searched, filtered and analysed to assist in diagnosing problems.

Theses logs are also pushed to Seq which produces a dashboard where they can be viewed and queried.

The Seq Dashboard can be viewed at the address:
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

### Mobiles System

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

### External SIM Cards Provider
| PromQL | Description |
| ---- | ---- |
| simcard_orders_sent | Total number of SIM Card orders sent |
| simcard_orders_completed | Total number of SIM Card orders completed |
| simcard_orders_inprogress | Current number of SIM Card orders in progress |
| simcard_orders_failed| Total number of SIM Card orders which failed |

### External Mobile Telecoms Network
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

# References

<a name="microservices.io">[microservices.io]</a>  
**Microservice Architecture by Chris Richardson**
https://microservices.io

<a name="4+1-Model">[4+1 Model]</a>  
**The "4+1" View Model of Software Architecture**  
Architectural Blueprints - The "4+1" View Model of Software Architecture by Kruchten, Philippe (1995)     
https://www.cs.ubc.ca/~gregor/teaching/papers/4+1view-architecture.pdf  
(15 pages)  
https://en.wikipedia.org/wiki/4%2B1_architectural_view_model

<a name="C4-Model">[C4 Model]</a>    
**The C4 model for visualising software architecture**  
https://c4model.com/

<a name="C4-PlantUML">[C4 PlantUML]</a>  
**The C4 model using PlantUML**  
C4-PlantUML  
https://github.com/RicardoNiepel/C4-PlantUML

<a name="REST-Client">[REST Client]</a>  
**REST Client Visual Studio plugin**
by Huachao Mao  
https://marketplace.visualstudio.com/items?itemName=humao.rest-client

<a name="Welcome-to-Statecharts">[Welcome to Statecharts]</a>  
**Welcome to the world of Statecharts**  
https://statecharts.github.io/

<a name="Statecharts">[Statecharts]</a>  
**Statecharts: A visual formalism for complex systems by David Harel (1986)**  
http://www.inf.ed.ac.uk/teaching/courses/seoc/2005_2006/resources/statecharts.pdf  
(44 pages)

<a name="k6">[k6]</a>  
**k6 - Load Testing tool**  
https://k6.io
