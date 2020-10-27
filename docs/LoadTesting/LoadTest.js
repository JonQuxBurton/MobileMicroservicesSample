import http from "k6/http";
import { check, group, sleep } from 'k6';
import { Counter } from 'k6/metrics';

const vus = 2;//5;
const iterations = 1;//3;

const SLEEP_DURATION = 0.1;
const SLEEP_DURATION_BEFORE_ORDER_COMPLETION = 40.0;
const SLEEP_DURATION_FOR_ORDER_COMPLETION_CHECK = 20.0;
const RETRIES_FOR_ORDER_COMPLETION_CHECK = 10;
const baseUrlMobiles = "http://localhost:5000/api";
const baseUrlExternalSimCards = "http://localhost:5001/api";
const baseUrlExternalTelecomsNetwork= "http://localhost:5002/api";
const dataWebServiceBaseUrl = "http://localhost:5099";

let orderMobileErrorMetrics = new Counter("orderMobileErrors");
let completeProvisionErrorMetrics = new Counter("completeProvisionErrors");
let activateMobileErrorMetrics = new Counter("activateMobileErrors");
let completeActivateErrorMetrics = new Counter("completeActivateErrors");

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
    let createCustomerBody = JSON.stringify({
      Name: `Armstrong-${__VU}-${__ITER} Corporation`
    });

    let createCustomerResponse = http.post(`${baseUrlMobiles}/customers`, createCustomerBody, getHttpParams('create-customer'));

    check(createCustomerResponse, {
      'is status 200': (r) => r.status === 200,
      'is customerId present': (r) => r.json().hasOwnProperty('globalId'),
    });
    let customerId = createCustomerResponse.json()['globalId'];
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);
    
    let getCustomerResponse = httpGetWithRetry(`${baseUrlMobiles}/customers/${customerId}`, getHttpParams('create-customer-getCustomer'));

    check(getCustomerResponse, {
      'is status 200': (r) => r.status === 200
    });
  });
}

export function orderMobile() {

  group('Order a Mobile', (_) => {
    let indexes = getDataIndexes("order-mobile", __VU, __ITER);
    let scenarioData = getScenarioDataForVirtualUser(dataFile, "order-mobile", indexes);
    let customerId = scenarioData.customerId;
    let phoneNumber = scenarioData.phoneNumber;
    let contactName = scenarioData.contactName;
    let contactPhoneNumber = scenarioData.contactPhoneNumbers;
    let orderMobileBody = JSON.stringify({
      PhoneNumber: phoneNumber,
      Name: contactName,
      ContactPhoneNumber: contactPhoneNumber
    });

    let orderMobileResponse = http.post(`${baseUrlMobiles}/customers/${customerId}/provision`, orderMobileBody, getHttpParams('order-mobile'));

    let orderMobileSuccess = check(orderMobileResponse, {
      'when orderMobile, is status 200': (r) => r.status === 200,
      'when orderMobile, is mobileId present': (r) => r.json().hasOwnProperty('globalId'),
    });
    if (!orderMobileSuccess) {
      let status = orderMobileResponse ? orderMobileResponse.status : "undefined";
      console.log(`FAILED - Request to ${orderMobileResponse.request.url} with returned ${status}`);
      orderMobileErrorMetrics.add(1, { url: orderMobileResponse.request.url });
    }
    let mobileId = orderMobileResponse.json()['globalId'];
    sleep(SLEEP_DURATION);
    
    let getMobileResponse = http.get(`${baseUrlMobiles}/mobiles/${mobileId}`, getHttpParams('order-mobile-getMobile'));

    let getMobileSuccess = check(getMobileResponse, {
      'when getMobile, is status 200': (r) => r.status === 200,
      'when getMobile, is provisionOrderId present': (r) => r.json('orderHistory.0').hasOwnProperty('globalId'),
    });
    if (!getMobileSuccess) {
      let status = getMobileResponse ? getMobileResponse.status : "undefined";
      console.log(`FAILED - Request to ${getMobileResponse.request.method} ${getMobileResponse.request.url} returned status ${status}`);
      orderMobileErrorMetrics.add(1, { url: getMobileResponse.request.url });
    }
    let provisionOrderId = getMobileResponse.json('orderHistory.0')['globalId'];    
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    // Check whether the Order has been received by the External Service
    let provisionOrderReceivedResponse = httpGetWithRetry(`${baseUrlExternalSimCards}/orders/${provisionOrderId}`, getHttpParams('order-mobile-provisionOrderReceived'));
    let provisionOrderReceivedSuccess = check(provisionOrderReceivedResponse, {
      'when checking provisionOrderReceived, is status 200': (r) => r.status === 200,
    });
    if (!provisionOrderReceivedSuccess) {
      let status = provisionOrderReceivedResponse ? provisionOrderReceivedResponse.status : "undefined";
      console.log(`FAILED - Request to ${provisionOrderReceivedResponse.request.method} ${provisionOrderReceivedResponse.request.url} returned status ${status}`);
      orderMobileErrorMetrics.add(1, { url: provisionOrderReceivedResponse.request.url });
    }
  });
}

export function completeProvision() {

  group('The External Service has completed a Mobile Provision Order', (_) => { 
    let indexes = getDataIndexes("complete-provision", __VU, __ITER);
    let scenarioData = getScenarioDataForVirtualUser(dataFile, "complete-provision", indexes);
    //console.log(`complete-provision vuId: ${scenarioData.vuId}`);
    //console.log(`complete-provision iteration: ${__ITER}`);
    //console.log(`complete-provision data: ${JSON.stringify(scenarioData.data)}`);
    
    let mobileId = scenarioData.mobileId;
    let provisionOrderId = scenarioData.provisionOrderId;

    let completeProvisionResponse = http.post(`${baseUrlExternalSimCards}/orders/${provisionOrderId}/complete`, "", getHttpParams('complete-provision'));
    sleep(SLEEP_DURATION);

    let completeProvisionResponseSuccess = check(completeProvisionResponse, {
      'when checking completeProvision, is status 200': (r) => r.status === 200,
      'when checking completeProvision, is activationCode present': (r) => r.json().hasOwnProperty('activationCode'),
    });
    if (!completeProvisionResponseSuccess) {
      let status = completeProvisionResponse ? completeProvisionResponse.status : "undefined";
      console.log(`FAILED - Request to ${completeProvisionResponseSuccess.request.method} ${completeProvisionResponseSuccess.request.url} returned status ${status}`);
      completeProvisionErrorMetrics.add(1, { url: completeProvisionResponseSuccess.request.url });
    }

    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    // Wait unitl the Order has been processed
    let waitingForActivateCheckResponse = waitUntilMobileInState(`${baseUrlMobiles}/mobiles/${mobileId}`, getHttpParams('complete-provision-WaitingForActivate'), 'WaitingForActivate');

    if (!waitingForActivateCheckResponse){
      console.log(`FAILED - waitUntilMobileInState ${baseUrlMobiles}/mobiles/${mobileId} with state 'WaitingForActivate'`);
    }

    let waitingForActivateSuccess = check(waitingForActivateCheckResponse, {
      'when checking is waitingForActivate, is status 200': (r) => r.status === 200
    });
    if (!waitingForActivateSuccess) {
      let status = waitingForActivateCheckResponse ? waitingForActivateCheckResponse.status : "undefined";
      console.log(`FAILED - Request to ${waitingForActivateCheckResponse.request.method} ${waitingForActivateCheckResponse.request.url} returned status ${status}`);
      completeProvisionErrorMetrics.add(1, { url: waitingForActivateCheckResponse.request.url });
    }
  });

}

export function activateMobile() {  

  group('Activate a Mobile', (_) => {
    let indexes = getDataIndexes("activate-mobile", __VU, __ITER);
    //console.log(`activate-mobile __VU: ${__VU}, __ITER: ${__ITER}, indexes: [${indexes.index0}, ${indexes.index1}]`);  
    let scenarioData = getScenarioDataForVirtualUser(dataFile, "activate-mobile", indexes);
    let mobileId = scenarioData.mobileId;
    let activationCode = scenarioData.activationCode;

    let activateMobileBody = JSON.stringify({
      ActivationCode: activationCode
    });

    let activateMobileResponse = http.post(`${baseUrlMobiles}/mobiles/${mobileId}/activate`, activateMobileBody, getHttpParams('activate-mobile'));

    let activateMobileSuccess = check(activateMobileResponse, {
      'when activateMobile, is status 200': (r) => r.status === 200,
      'when activateMobile, is activateOrderId present': (r) => r.json().hasOwnProperty('globalId'),
    });
    if (!activateMobileSuccess) {
      let status = activateMobileResponse ? activateMobileResponse.status : "undefined";
      console.log(`FAILED - Request to ${activateMobileResponse.request.method} ${activateMobileResponse.request.url} returned status ${status}`);
      activateMobileErrorMetrics.add(1, { url: activateMobileResponse.request.url });
    }

    let activateOrderId = activateMobileResponse.json()['globalId'];

    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    // Check whether the Order has been received by the External Service
    let activateOrderReceivedResponse = httpGetWithRetry(`${baseUrlExternalTelecomsNetwork}/orders/${activateOrderId}`, getHttpParams('activate-mobile-activateOrderReceived'));
    let activateOrderReceivedSuccess = check(activateOrderReceivedResponse, {
      'when activateOrderReceived, is status 200 2': (r) => r.status === 200
    });
    if (!activateOrderReceivedSuccess) {
      let status = activateOrderReceivedResponse ? activateOrderReceivedResponse.status : "undefined";
      console.log(`FAILED - Request to ${activateOrderReceivedResponse.request.method} ${activateOrderReceivedResponse.request.url} returned status ${status}`);
      activateMobileErrorMetrics.add(1, { url: activateOrderReceivedResponse.request.url });
    }
  });

}

export function completeActivate() {

  group('The External Service has completed a Mobile Activate Order', (_) => {
    let indexes = getDataIndexes("complete-activate", __VU, __ITER);
    //console.log(`complete-activate __VU: ${__VU}, __ITER: ${__ITER}, indexes: [${indexes.index0}, ${indexes.index1}]`);
    let scenarioData = getScenarioDataForVirtualUser(dataFile, "complete-activate", indexes);
    // console.log(`complete-activate vuId: ${scenarioData.vuId}`);
    // console.log(`complete-activate iteration: ${__ITER}`);
    // console.log(`complete-activate data: ${JSON.stringify(scenarioData.data)}`);

    let mobileId = scenarioData.mobileId;
    let activateOrderId = scenarioData.activateOrderId;

    let completeActivateResponse = http.post(`${baseUrlExternalTelecomsNetwork}/orders/${activateOrderId}/complete`, "", getHttpParams('complete-activate'));

    let completeActivateSuccess = check(completeActivateResponse, {
      'when checking completeActivate, is status 200': (r) => r.status === 200
    });
    if (!completeActivateSuccess) {
      let status = completeActivateResponse ? completeActivateResponse.status : "undefined";
      console.log(`FAILED - Request to ${completeActivateResponse.request.method} ${completeActivateResponse.request.url} returned status ${status}`);
      completeActivateErrorMetrics.add(1, { url: completeActivateResponse.request.url });
    }

    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);
    
    // Wait unitl the Order has been processed
    let waitingForLiveCheckResponse = waitUntilMobileInState(`${baseUrlMobiles}/mobiles/${mobileId}`, getHttpParams('complete-activate-waitingForLive'), 'Live');

    if (!waitingForLiveCheckResponse){
      console.log(`FAILED - waitUntilMobileInState ${baseUrlMobiles}/mobiles/${mobileId} with state 'Live'`);
    }

    let waitingForLiveCheckSuccess = check(waitingForLiveCheckResponse, {
      "when checking waitingForLive, is Mobile in state 'Live'": (r) => r !== null,
      'when checking waitingForLive, is status 200': (r) => r.status === 200
    });
    if (!waitingForLiveCheckSuccess) {
      let status = waitingForLiveCheckResponse ? waitingForLiveCheckResponse.status : "undefined";
      console.log(`FAILED - Request to ${waitingForLiveCheckResponse.request.method} ${waitingForLiveCheckResponse.request.url} returned status ${status}`);
      completeActivateErrorMetrics.add(1, { url: waitingForLiveCheckResponse.request.url });
    }
  });
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

function loadDataFile() {
  return JSON.parse(open("./data.json"));
}

function getScenarioDataForVirtualUser(dataFile, scenarioKey, indexes) {
  if (scenarioKey == "order-mobile")
    return dataFile.orderMobile[indexes.index0][indexes.index1];
  
  if (scenarioKey == "complete-provision")
    return dataFile.completeProvision[indexes.index0][indexes.index1];

  if (scenarioKey == "activate-mobile")
    return dataFile.activateMobile[indexes.index0][indexes.index1];

  if (scenarioKey == "complete-activate")
    return dataFile.completeActivate[indexes.index0][indexes.index1];

  return;
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