import http from "k6/http";
import { check, group, sleep } from 'k6';
import { Counter } from 'k6/metrics';

const vus = 2;//5;
const iterations = 1;//3;

const scenarios = {
  createCustomer: "createCustomer",
  orderMobile: "orderMobile",
  completeProvision: "completeProvision",
  activateMobile: "activateMobile",
  completeActivate: "completeActivate"
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
      exec: 'createCustomer',
    },
    orderMobileTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'orderMobile',
    },
    completeProvisionTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'completeProvision',
    },
    activateMobileTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'activateMobile',
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
    let createCustomerBody = JSON.stringify({
      Name: `Armstrong-${__VU}-${__ITER} Corporation`
    });

    let createCustomerResponse = http.post(`${baseUrlMobiles}/customers`, createCustomerBody, getHttpParams(scenarioName));

    check(createCustomerResponse, {
      [`when ${scenarioName}, is status 200`]: (r) => r.status === 200,
      [`when ${scenarioName}, is customerId present`]: (r) => r.json().hasOwnProperty('globalId'),
    });
    let customerId = createCustomerResponse.json()['globalId'];
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);
    
    let getCustomerResponse = httpGetWithRetry(`${baseUrlMobiles}/customers/${customerId}`, getHttpParams(`${scenarioName}-getCustomer`));

    check(getCustomerResponse, {
      'is status 200': (r) => r.status === 200
    });
  });
}

function logCheckFailure(scenarioName, success, response) {
  if (!success) {
    let status = response ? response.status : "undefined";
    console.log(`FAILED - Request to ${response.request.url} with returned ${status}`);
    errorMetrics[scenarioName].add(1, { url: response.request.url });
  }
}

export function orderMobile() {

  group('Order a Mobile', (_) => {
    let scenarioName = scenarios.orderMobile;
    let indexes = getDataIndexes(scenarioName, __VU, __ITER);
    let scenarioData = getScenarioDataForVirtualUser(dataFile, scenarioName, indexes);
    let customerId = scenarioData.customerId;
    let phoneNumber = scenarioData.phoneNumber;
    let contactName = scenarioData.contactName;
    let contactPhoneNumber = scenarioData.contactPhoneNumbers;
    let orderMobileBody = JSON.stringify({
      PhoneNumber: phoneNumber,
      Name: contactName,
      ContactPhoneNumber: contactPhoneNumber
    });

    let orderMobileResponse = http.post(`${baseUrlMobiles}/customers/${customerId}/provision`, orderMobileBody, getHttpParams(scenarioName));

    let orderMobileSuccess = check(orderMobileResponse, {
      [`when ${scenarioName}, is status 200`]: (r) => r.status === 200,
      [`when ${scenarioName}, is mobileId present`]: (r) => r.json().hasOwnProperty('globalId'),
    });
    logCheckFailure(scenarioName, orderMobileSuccess, orderMobileResponse);
    let mobileId = orderMobileResponse.json()['globalId'];
    sleep(SLEEP_DURATION);
    
    let getMobileResponse = http.get(`${baseUrlMobiles}/mobiles/${mobileId}`, getHttpParams(`${scenarioName}-getMobile`));

    let getMobileSuccess = check(getMobileResponse, {
      [`when ${scenarioName} getMobile, is status 200`]: (r) => r.status === 200,
      [`when ${scenarioName} getMobile, is provisionOrderId present`]: (r) => r.json('orderHistory.0').hasOwnProperty('globalId'),
    });
    logCheckFailure(scenarioName, getMobileSuccess, getMobileResponse);
    let provisionOrderId = getMobileResponse.json('orderHistory.0')['globalId'];    
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    // Check whether the Order has been received by the External Service
    let provisionOrderReceivedResponse = httpGetWithRetry(`${baseUrlExternalSimCards}/orders/${provisionOrderId}`, getHttpParams(`${scenarioName}-provisionOrderReceived`));
    let provisionOrderReceivedSuccess = check(provisionOrderReceivedResponse, {
      [`when ${scenarioName} provisionOrderReceived, is status 200`]: (r) => r.status === 200,
    });
    logCheckFailure(scenarioName, provisionOrderReceivedSuccess, provisionOrderReceivedResponse);
  });
}

export function completeProvision() {

  group('The External Service has completed a Mobile Provision Order', (_) => {
    let scenarioName = scenarios.completeProvision;
    let indexes = getDataIndexes(scenarioName, __VU, __ITER);
    let scenarioData = getScenarioDataForVirtualUser(dataFile, scenarioName, indexes);
    let mobileId = scenarioData.mobileId;
    let provisionOrderId = scenarioData.provisionOrderId;

    let completeProvisionResponse = http.post(`${baseUrlExternalSimCards}/orders/${provisionOrderId}/complete`, "", getHttpParams(scenarioName));
    sleep(SLEEP_DURATION);

    let completeProvisionResponseSuccess = check(completeProvisionResponse, {
      [`when ${scenarioName}, is status 200`]: (r) => r.status === 200,
      [`when ${scenarioName}, is activationCode present`]: (r) => r.json().hasOwnProperty('activationCode'),
    });
    logCheckFailure(scenarioName, completeProvisionResponseSuccess, completeProvisionResponse);
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    // Wait unitl the Order has been processed
    let waitingForActivateCheckResponse = waitUntilMobileInState(`${baseUrlMobiles}/mobiles/${mobileId}`, getHttpParams(`${scenarioName}-WaitingForActivate`), 'WaitingForActivate');

    if (!waitingForActivateCheckResponse)
      console.log(`FAILED - waitUntilMobileInState ${baseUrlMobiles}/mobiles/${mobileId} with state 'WaitingForActivate'`);

    let waitingForActivateSuccess = check(waitingForActivateCheckResponse, {
      [`when ${scenarioName} waitingForActivate, is status 200`]: (r) => r.status === 200
    });
    logCheckFailure(scenarioName, waitingForActivateSuccess, waitingForActivateCheckResponse);
  });

}

export function activateMobile() {  

  group('Activate a Mobile', (_) => {
    const scenarioName = scenarios.activateMobile;
    let indexes = getDataIndexes(scenarioName, __VU, __ITER);
    let scenarioData = getScenarioDataForVirtualUser(dataFile, scenarioName, indexes);
    let mobileId = scenarioData.mobileId;
    let activationCode = scenarioData.activationCode;

    let activateMobileBody = JSON.stringify({
      ActivationCode: activationCode
    });

    let activateMobileResponse = http.post(`${baseUrlMobiles}/mobiles/${mobileId}/activate`, activateMobileBody, getHttpParams(scenarioName));

    let activateMobileSuccess = check(activateMobileResponse, {
      [`when ${scenarioName}, is status 200`]: (r) => r.status === 200,
      [`when ${scenarioName}, is activateOrderId present`]: (r) => r.json().hasOwnProperty('globalId'),
    });
    logCheckFailure(scenarioName, activateMobileSuccess, activateMobileResponse);

    let activateOrderId = activateMobileResponse.json()['globalId'];

    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    // Check whether the Order has been received by the External Service
    let activateOrderReceivedResponse = httpGetWithRetry(`${baseUrlExternalTelecomsNetwork}/orders/${activateOrderId}`, getHttpParams(`${scenarioName}-activateOrderReceived`));
    let activateOrderReceivedSuccess = check(activateOrderReceivedResponse, {
      ['when ${scenarioName}-activateOrderReceived, is status 200']: (r) => r.status === 200
    });
    logCheckFailure(scenarioName, activateOrderReceivedSuccess, activateOrderReceivedResponse);
  });

}

export function completeActivate() {

  group('The External Service has completed a Mobile Activate Order', (_) => {
    const scenarioName = scenarios.completeActivate;
    let indexes = getDataIndexes(scenarioName, __VU, __ITER);    
    let scenarioData = getScenarioDataForVirtualUser(dataFile, scenarioName, indexes);
    let mobileId = scenarioData.mobileId;
    let activateOrderId = scenarioData.activateOrderId;

    let completeActivateResponse = http.post(`${baseUrlExternalTelecomsNetwork}/orders/${activateOrderId}/complete`, "", getHttpParams(scenarioName));

    let completeActivateSuccess = check(completeActivateResponse, {
      [`when ${scenarioName}, is status 200`]: (r) => r.status === 200
    });
    logCheckFailure(scenarioName, completeActivateSuccess, completeActivateResponse);
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);
    
    // Wait unitl the Order has been processed
    let waitingForLiveCheckResponse = waitUntilMobileInState(`${baseUrlMobiles}/mobiles/${mobileId}`, getHttpParams(`${scenarioName}-waitingForLive`), 'Live');

    if (!waitingForLiveCheckResponse)
      console.log(`FAILED - waitUntilMobileInState ${baseUrlMobiles}/mobiles/${mobileId} with state 'Live'`);
    
    let waitingForLiveCheckSuccess = check(waitingForLiveCheckResponse, {
      [`when ${scenarioName} waitingForLive, is Mobile in state 'Live'`]: (r) => r !== null,
      [`when ${scenarioName} waitingForLive, is status 200`]: (r) => r.status === 200
    });
    logCheckFailure(scenarioName, waitingForLiveCheckSuccess, waitingForLiveCheckResponse);
  });
}

function loadDataFile() {
  return JSON.parse(open("./data.json"));
}

function getDataIndexes(scenarioKey, vuId, iteration){
  let body = JSON.stringify({
    ScenarioKey: scenarioKey,
    VuId: vuId,
    Iteration: iteration
  });
  let response = http.post(`${dataWebServiceBaseUrl}/data`, body, getHttpParams());
  let index0 = response.body;

  return {
    index0: index0,
    index1: iteration
  }
}

function getScenarioDataForVirtualUser(dataFile, scenarioKey, indexes) {
  return dataFile[scenarioKey][indexes.index0][indexes.index1];
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