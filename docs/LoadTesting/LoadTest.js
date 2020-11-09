import http from "k6/http";
import { check, group, sleep } from 'k6';
import { Counter } from 'k6/metrics';

const vus = 5;
const iterations = 3;

const scenarios = {
  createCustomer: "CreateCustomer",
  orderMobile: "OrderMobile",
  completeProvision: "CompleteProvision",
  activateMobile: "ActivateMobile",
  completeActivate: "CompleteActivate"
}

const SLEEP_DURATION = 0.1;
const SLEEP_DURATION_BEFORE_ORDER_COMPLETION = 40.0;
const SLEEP_DURATION_FOR_ORDER_COMPLETION_CHECK = 20.0;
const RETRIES_FOR_ORDER_COMPLETION_CHECK = 10;
const baseUrlMobiles = "http://localhost:5000/api";
const baseUrlExternalSimCards = "http://localhost:5001/api";
const baseUrlExternalTelecomsNetwork= "http://localhost:5002/api";
const dataWebServiceBaseUrl = "http://localhost:5099";

let errorMetrics = {
  orderMobile: new Counter("orderMobileErrors"),
  completeProvision: new Counter("completeProvisionErrors"),
  activateMobile: new Counter("activateMobileErrors"),
  completeActivate: new Counter("completeActivateErrors")
};

let dataFile = loadDataFile();

export let options = {
  scenarios: {
    createCustomerTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'createCustomer'
    },
    orderMobileTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'orderMobile'
    },
    completeProvisionTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'completeProvision'
    },
    activateMobileTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'activateMobile'
    },
    completeActivateTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'completeActivate',
    }
  }
};

export function createCustomer() {

  group('Create a Customer', (_) => {
    let scenarioName = scenarios.createCustomer;
    let user = getUser(scenarioName, __VU, __ITER);
    
    beginScenarioLog(scenarioName, user, __ITER);
    let customerName = `Armstrong-${__VU}-${__ITER} Corporation`;
    let createCustomerBody = JSON.stringify({
      Name: customerName
    });
    let url;

    url = `${baseUrlMobiles}/customers`;
    scenarioLog(scenarioName, user, __ITER, `Sending CreateCustomer to Mobiles API, customerName: ${customerName} [${url}]`);
    let createCustomerResponse = http.post(url, createCustomerBody, getHttpParams(scenarioName));

    let createCustomerSuccess = check(createCustomerResponse, {
      [`when ${scenarioName}, is status 200`]: (r) => r.status === 200,
      [`when ${scenarioName}, is customerId present`]: (r) => r.json().hasOwnProperty('globalId'),
    });
    scenarioLogCheck(scenarioName, user, __ITER, createCustomerSuccess, createCustomerResponse);
    let customerId = createCustomerResponse.json()['globalId'];
    
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);
    
    url = `${baseUrlMobiles}/customers/${customerId}`;
    scenarioLog(scenarioName, user, __ITER, `Check whether the Customer has been created, customerId: ${customerId} [${url}]`);
    let getCustomerResponse = httpGetWithRetry(url, getHttpParams(`${scenarioName}-getCustomer`));

    let getCustomerSuccess = check(getCustomerResponse, {
      'is status 200': (r) => r.status === 200
    });
    scenarioLogCheck(scenarioName, user, __ITER, getCustomerSuccess, getCustomerResponse);
  });
}

export function orderMobile() {

  group('Order a Mobile', (_) => {    
    let scenarioName = scenarios.orderMobile;
    let user = getUser(scenarioName, __VU, __ITER);

    beginScenarioLog(scenarioName, user, __ITER, user);
    let scenarioData = getScenarioDataForUser(dataFile, scenarioName, user, __ITER);      
    let customerId = scenarioData.customerId;
    let phoneNumber = scenarioData.phoneNumber;
    let contactName = scenarioData.contactName;
    let contactPhoneNumber = scenarioData.contactPhoneNumbers;
    let orderMobileBody = JSON.stringify({
      PhoneNumber: phoneNumber,
      Name: contactName,
      ContactPhoneNumber: contactPhoneNumber
    });    
    let url;

    url = `${baseUrlMobiles}/customers/${customerId}/provision`;
    scenarioLog(scenarioName, user, __ITER, `Sending Order to Mobiles API, CustomerId: ${customerId}, PhoneNumber: ${phoneNumber} [${url}]`);
    let orderMobileResponse = http.post(url, orderMobileBody, getHttpParams(scenarioName));    
    
    let orderMobileSuccess = check(orderMobileResponse, {
      [`when ${scenarioName}, is status 200`]: (r) => r.status === 200,
      [`when ${scenarioName}, is mobileId present`]: (r) => r.json().hasOwnProperty('globalId'),
    });
    scenarioLogCheck(scenarioName, user, __ITER, orderMobileSuccess, orderMobileResponse);
    logCheckFailure(scenarioName, orderMobileSuccess, orderMobileResponse);
    let mobileId = orderMobileResponse.json()['globalId'];
    sleep(SLEEP_DURATION);

    url = `${baseUrlMobiles}/mobiles/${mobileId}`;
    scenarioLog(scenarioName, user, __ITER, `Getting Mobile from Mobiles API, MobileId: ${mobileId} [${url}]`);
    let getMobileResponse = http.get(url, getHttpParams(`${scenarioName}-getMobile`));

    let getMobileSuccess = check(getMobileResponse, {
      [`when ${scenarioName} getMobile, is status 200`]: (r) => r.status === 200,
      [`when ${scenarioName} getMobile, is provisionOrderId present`]: (r) => r.json('orderHistory.0').hasOwnProperty('globalId'),
    });
    scenarioLogCheck(scenarioName, user, __ITER, getMobileSuccess, getMobileResponse);
    logCheckFailure(scenarioName, getMobileSuccess, getMobileResponse);
    let provisionOrderId = getMobileResponse.json('orderHistory.0')['globalId'];    
    
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    url = `${baseUrlExternalSimCards}/orders/${provisionOrderId}`;
    scenarioLog(scenarioName, user, __ITER, `Check whether the Order has been received by the External Service, ProvisionOrderId: ${provisionOrderId} [${url}]`);
    let provisionOrderReceivedResponse = httpGetWithRetry(url, getHttpParams(`${scenarioName}-provisionOrderReceived`));
    let provisionOrderReceivedSuccess = check(provisionOrderReceivedResponse, {
      [`when ${scenarioName} provisionOrderReceived, is status 200`]: (r) => r.status === 200,
    });
    scenarioLogCheck(scenarioName, user, __ITER, provisionOrderReceivedSuccess, provisionOrderReceivedResponse);
    logCheckFailure(scenarioName, provisionOrderReceivedSuccess, provisionOrderReceivedResponse);
  });
}

export function completeProvision() {

  group('The External Service has completed a Mobile Provision Order', (_) => {
    let scenarioName = scenarios.completeProvision;
    let user = getUser(scenarioName, __VU, __ITER);
    
    beginScenarioLog(scenarioName, user, __ITER);    
    let scenarioData = getScenarioDataForUser(dataFile, scenarioName, user, __ITER);  
    let mobileId = scenarioData.mobileId;
    let mobileOrderId = scenarioData.mobileOrderId;
    let url;

    url = `${baseUrlExternalSimCards}/orders/${mobileOrderId}/complete`;
    scenarioLog(scenarioName, user, __ITER, `Sending CompleteProvisionOrder to External Service, ProvisionOrderId: ${mobileOrderId} [${url}]`);
    let completeProvisionResponse = http.post(url, "", getHttpParams(scenarioName));
    
    let completeProvisionResponseSuccess = check(completeProvisionResponse, {
      [`when ${scenarioName}, is status 200`]: (r) => r.status === 200,
      [`when ${scenarioName}, is activationCode present`]: (r) => r.json().hasOwnProperty('activationCode'),
    });
    scenarioLogCheck(scenarioName, user, __ITER, completeProvisionResponseSuccess, completeProvisionResponse);
    logCheckFailure(scenarioName, completeProvisionResponseSuccess, completeProvisionResponse);
    
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    url = `${baseUrlMobiles}/mobiles/${mobileId}`;
    scenarioLog(scenarioName, user, __ITER, `Check whether the Mobile has been updated to WaitingForActivate, mobileId: ${mobileId} [${url}]`);
    let waitingForActivateCheckResponse = waitUntilMobileInState(url, getHttpParams(`${scenarioName}-WaitingForActivate`), 'WaitingForActivate');

    if (!waitingForActivateCheckResponse)
      console.log(`FAILED - waitUntilMobileInState ${baseUrlMobiles}/mobiles/${mobileId} with state 'WaitingForActivate'`);

    let waitingForActivateSuccess = check(waitingForActivateCheckResponse, {
      [`when ${scenarioName} waitingForActivate, is status 200`]: (r) => r.status === 200
    });
    scenarioLogCheck(scenarioName, user, __ITER, waitingForActivateSuccess, waitingForActivateCheckResponse);
    logCheckFailure(scenarioName, waitingForActivateSuccess, waitingForActivateCheckResponse);
  });

}

export function activateMobile() {  

  group('Activate a Mobile', (_) => {
    const scenarioName = scenarios.activateMobile;
    let user = getUser(scenarioName, __VU, __ITER);
    
    beginScenarioLog(scenarioName, user, __ITER);
    let scenarioData = getScenarioDataForUser(dataFile, scenarioName, user, __ITER);
    let mobileId = scenarioData.mobileId;
    let activationCode = scenarioData.activationCode;
    let url;

    let activateMobileBody = JSON.stringify({
      ActivationCode: activationCode
    });

    url = `${baseUrlMobiles}/mobiles/${mobileId}/activate`;
    scenarioLog(scenarioName, user, __ITER, `Sending ActivateOrder to Mobiles API, MobileId: ${mobileId}, ActivationCode: ${activationCode} [${url}]`);
    let activateMobileResponse = http.post(url, activateMobileBody, getHttpParams(scenarioName));

    let activateMobileSuccess = check(activateMobileResponse, {
      [`when ${scenarioName}, is status 200`]: (r) => r.status === 200,
      [`when ${scenarioName}, is activateOrderId present`]: (r) => r.json().hasOwnProperty('globalId'),
    });
    scenarioLogCheck(scenarioName, user, __ITER, activateMobileSuccess, activateMobileResponse);
    logCheckFailure(scenarioName, activateMobileSuccess, activateMobileResponse);

    let activateOrderId = activateMobileResponse.json()['globalId'];

    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    scenarioLog(scenarioName, user, __ITER, `Check whether the Order has been received by the External Service, ActivateOrderId: ${activateOrderId} [${url}]`);
    let activateOrderReceivedResponse = httpGetWithRetry(`${baseUrlExternalTelecomsNetwork}/orders/${activateOrderId}`, getHttpParams(`${scenarioName}-activateOrderReceived`));
    let activateOrderReceivedSuccess = check(activateOrderReceivedResponse, {
      ['when ${scenarioName}-activateOrderReceived, is status 200']: (r) => r.status === 200
    });
    scenarioLogCheck(scenarioName, user, __ITER, activateOrderReceivedSuccess, activateOrderReceivedResponse);
    logCheckFailure(scenarioName, activateOrderReceivedSuccess, activateOrderReceivedResponse);
  });

}

export function completeActivate() {

  group('The External Service has completed a Mobile Activate Order', (_) => {
    const scenarioName = scenarios.completeActivate;
    let user = getUser(scenarioName, __VU, __ITER);

    beginScenarioLog(scenarioName, user, __ITER);
    let scenarioData = getScenarioDataForUser(dataFile, scenarioName, user, __ITER);
    let mobileId = scenarioData.mobileId;
    let mobileOrderId = scenarioData.mobileOrderId;
    let url;

    url = `${baseUrlExternalTelecomsNetwork}/orders/${mobileOrderId}/complete`;
    scenarioLog(scenarioName, user, __ITER, `Sending CompleteActivateOrder to External Service, ActivateOrderId: ${mobileOrderId} [${url}]`);
    let completeActivateResponse = http.post(url, "", getHttpParams(scenarioName));

    let completeActivateSuccess = check(completeActivateResponse, {
      [`when ${scenarioName}, is status 200`]: (r) => r.status === 200
    });
    scenarioLogCheck(scenarioName, user, __ITER, completeActivateSuccess, completeActivateResponse);
    logCheckFailure(scenarioName, completeActivateSuccess, completeActivateResponse);
    
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);
    
    url = `${baseUrlMobiles}/mobiles/${mobileId}`;
    scenarioLog(scenarioName, user, __ITER, `Check whether the Mobile has been updated to Live, MobileId: ${mobileId} [${url}]`);
    let waitingForLiveCheckResponse = waitUntilMobileInState(url, getHttpParams(`${scenarioName}-waitingForLive`), 'Live');

    if (!waitingForLiveCheckResponse)
      console.log(`FAILED - waitUntilMobileInState ${baseUrlMobiles}/mobiles/${mobileId} with state 'Live'`);
    
    let waitingForLiveCheckSuccess = check(waitingForLiveCheckResponse, {
      [`when ${scenarioName} waitingForLive, is Mobile in state 'Live'`]: (r) => r !== null,
      [`when ${scenarioName} waitingForLive, is status 200`]: (r) => r.status === 200
    });
    scenarioLogCheck(scenarioName, user, __ITER, waitingForLiveCheckSuccess, waitingForLiveCheckResponse);
    logCheckFailure(scenarioName, waitingForLiveCheckSuccess, waitingForLiveCheckResponse);
  });
}

function loadDataFile() {
  return JSON.parse(open("./TestRuns/data.json"));
}

function getUser(scenarioKey, vuId, iteration){
  let body = JSON.stringify({
    ScenarioKey: scenarioKey,
    VirtualUserId: vuId,
    Iteration: iteration
  });
  let response = http.post(`${dataWebServiceBaseUrl}/data`, body, getHttpParams());
  return response.json();
}

function getScenarioDataForUser(dataFile, scenarioKey, user, iteration) {
  return dataFile.filter(x => x.scenarioName===scenarioKey)[0].data
    .filter(y => y.userId === user.globalId)[0].data[iteration];
}

function httpGetWithRetry(url, params) {
  var res;
  for (var retries = RETRIES_FOR_ORDER_COMPLETION_CHECK; retries > 0; retries--) {
    res = http.get(url, params)
    if (res.status != 404 && res.status != 408 && res.status < 500) {
      return res;
    }
    sleep(SLEEP_DURATION_FOR_ORDER_COMPLETION_CHECK);
  }
  return res;
}

function waitUntilMobileInState(url, params, state) {
  let res;
  for (var retries = RETRIES_FOR_ORDER_COMPLETION_CHECK; retries > 0; retries--) {
    res = http.get(url, params)
    if (res.status != 404 && res.status != 408 && res.status < 500 && res.json()['state'] === state) {
      return res;
    }
    sleep(SLEEP_DURATION_FOR_ORDER_COMPLETION_CHECK);
  }
  return null;
}

function getHttpParams(tagName){
  let params = {
    headers: {
      'Content-Type': 'application/json',
    },
    tags: {}
  };

  if (tagName)
    params.tags.name = tagName;

  return params;
}

function beginScenarioLog(scenarioName, user, iteration) {
  let body = JSON.stringify({
    ScenarioKey: scenarioName,    
    UserGlobalId: user.globalId,
    VirtualUserId: `${user.virtualUserId}`,
    Iteration: `${iteration}`,
    Message: ""
  });

  http.post(`${dataWebServiceBaseUrl}/scenarios/beginlog`, body, getHttpParams());
}

function scenarioLog(scenarioName, user, iteration, message){
  let body = JSON.stringify({
    ScenarioKey: scenarioName,
    UserGlobalId: user.globalId,
    VirtualUserId: `${user.virtualUserId}`,
    Iteration: `${iteration}`,
    Message: message
  });

  http.post(`${dataWebServiceBaseUrl}/scenarios/log`, body, getHttpParams());
}

function scenarioLogCheck(scenarioName, user, iteration, success, response) {
  let message;
  if (!success) {
    let status = response ? response.status : "undefined";
    message = `FAILED - Request to ${response.request.url} with returned ${status}`;
  } else {
    message=  "Succeeded";
  }

  let body = JSON.stringify({
    ScenarioKey: scenarioName,
    UserGlobalId: user.globalId,
    VirtualUserId: `${user.virtualUserId}`,
    Iteration: `${iteration}`,
    Message: message
  });

  http.post(`${dataWebServiceBaseUrl}/scenarios/log`, body, getHttpParams());
}

function logCheckFailure(scenarioName, success, response) {
  if (!success) {
    let status = response ? response.status : "undefined";
    console.log(`FAILED - Request to ${response.request.url} with returned ${status}`);
    errorMetrics[scenarioName].add(1, { url: response.request.url });
  }
}