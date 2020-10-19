import http from "k6/http";
import { check, group, sleep } from 'k6';

let orderMobileData;
let completeProvisionData;
let activateMobileData;
let completeActivateData;

loadData();

let vus = 5;
let iterations = 2;

export let options = {
  vus: vus,
  iterations: vus * iterations
};

const SLEEP_DURATION = 0.1;
const SLEEP_DURATION_FOR_ORDER_COMPLETION_CHECK = 10.0;
const RETRIES_FOR_ORDER_COMPLETION_CHECK = 12;

let counters = {
  orderMobile: 0,
  completeProvision: 0,
  activateMobile: 0,
  completeActivate: 0,
  createCustomer: 0
};

export default function () {
  let params = {
    headers: {
      'Content-Type': 'application/json',
    },
    tags: {}
  };

  if (__VU == 2)
    orderMobile(params, orderMobileData[counters.orderMobile++]);
  else if (__VU == 3)
    completeProvision(params, completeProvisionData[counters.completeProvision++]);
  else if (__VU == 4)
     activateMobile(params, activateMobileData[counters.activateMobile++]);
  else if (__VU == 5)
     completeActivate(params, completeActivateData[counters.completeActivate++]);
  else
    createCustomer(params);
};

function createCustomer(params) {
  group('Create a Customer', (_) => {
    params.tags.name = 'create-customer';
    let customersUrl = "http://localhost:5000/api/customers";
    let createCustomerBody = JSON.stringify({
      Name: `Armstrong-${__VU}-${__ITER} Corporation`
    });

    let createCustomerResponse = http.post(customersUrl, createCustomerBody, params);

    check(createCustomerResponse, {
      'is status 200': (r) => r.status === 200,
      'is customerId present': (r) => r.json().hasOwnProperty('globalId'),
    });
    sleep(SLEEP_DURATION);

    let customerId = createCustomerResponse.json()['globalId'];
    let getCustomerResponse = httpGetWithRetry(`http://localhost:5000/api/customers/${customerId}`, params);

    check(getCustomerResponse, {
      'is status 200': (r) => r.status === 200
    });
  });
}

function orderMobile(params, orderMobileData) {
  group('Order a Mobile', (_) => {
    params.tags.name = 'order-mobile';
    let customerId = orderMobileData.customerId;
    let phoneNumber = orderMobileData.phoneNumber;
    let contactName = orderMobileData.contactName;
    let contactPhoneNumber = orderMobileData.contactPhoneNumbers;

    let orderMobileUrl = `http://localhost:5000/api/customers/${customerId}/provision`;
    let orderMobileBody = JSON.stringify({
      PhoneNumber: phoneNumber,
      Name: contactName,
      ContactPhoneNumber: contactPhoneNumber
    });

    let orderMobileResponse = http.post(orderMobileUrl, orderMobileBody, params);

    check(orderMobileResponse, {
      'is status 200': (r) => r.status === 200,
      'is mobileId present': (r) => r.json().hasOwnProperty('globalId'),
    });
    sleep(SLEEP_DURATION);

    params.tags.name = 'get-mobile';
    let mobileId = orderMobileResponse.json()['globalId'];
    let getMobileUrl = `http://localhost:5000/api/mobiles/${mobileId}`;
    let getMobileResponse = http.get(getMobileUrl, params);

    check(getMobileResponse, {
      'is status 200': (r) => r.status === 200,
      'is provisionOrderId present': (r) => r.json('orderHistory.0').hasOwnProperty('globalId'),
    });

    let provisionOrderId = getMobileResponse.json('orderHistory.0')['globalId'];

    // Check whether the Order has been received by the External Service
    let provisionOrderReceivedResponse = httpGetWithRetry(`http://localhost:5001/api/orders/${provisionOrderId}`, params);
    check(provisionOrderReceivedResponse, {
      'is status 200': (r) => r.status === 200,
    });
  });
}

function completeProvision(params, completeProvisionData) {
  group('The External Service has completed a Mobile Provision Order', (_) => {
    // Step 3 - The External Service has completed the Mobile Provision Order
    params.tags.name = 'complete-provision';
    let mobileId = completeProvisionData.mobileId;
    let provisionOrderId = completeProvisionData.provisionOrderId;
    let completeProvisionUrl = `http://localhost:5001/api/orders/${provisionOrderId}/complete`;

    let completeProvisionResponse = http.post(completeProvisionUrl, "", params);
    sleep(SLEEP_DURATION);

    check(completeProvisionResponse, {
      'is status 200': (r) => r.status === 200,
      'is activationCode present': (r) => r.json().hasOwnProperty('activationCode'),
    });

    // Wait unitl the Order has been processed
    let waitingForActivateCheckResponse = waitUntilMobileInState(`http://localhost:5000/api/mobiles/${mobileId}`, params, 'WaitingForActivate');

    check(waitingForActivateCheckResponse, {
      'is status 200': (r) => r.status === 200
    });
  });

}

function activateMobile(params, activateMobileData) {
  group('Activate a Mobile', (_) => {
    // Step 4 - Activate a Mobile
    params.tags.name = 'activate-mobile';
    let mobileId = activateMobileData.mobileId;
    let activationCode = activateMobileData.activationCode;

    let activateMobileUrl = `http://localhost:5000/api/mobiles/${mobileId}/activate`;
    let activateMobileBody = JSON.stringify({
      ActivationCode: activationCode
    });

    let activateMobileResponse = http.post(activateMobileUrl, activateMobileBody, params);

    check(activateMobileResponse, {
      'is status 200': (r) => r.status === 200,
      'is activateOrderId present': (r) => r.json().hasOwnProperty('globalId'),
    });
    let activateOrderId = activateMobileResponse.json()['globalId'];

    // Check whether the Order has been received by the External Service
    let activateOrderReceivedResponse = httpGetWithRetry(`http://localhost:5002/api/orders/${activateOrderId}`, params);
    check(activateOrderReceivedResponse, {
      'is status 200 2': (r) => r.status === 200
    });
  });

}

function completeActivate(params, completeActivateData) {
  group('The External Service has completed a Mobile Activate Order', (_) => {

    // Step  5 - The External Service has completed the Mobile Activate Order
    params.tags.name = 'complete-activate';
    let mobileId = completeActivateData.mobileId;
    let activateOrderId = completeActivateData.activateOrderId;

    let completeActivateUrl = `http://localhost:5002/api/orders/${activateOrderId}/complete`;

    let completeActivateResponse = http.post(completeActivateUrl, null, params);

    check(completeActivateResponse, {
      'is status 200': (r) => r.status === 200
    });

    // Wait unitl the Order has been processed
    let waitingForLiveCheckResponse = waitUntilMobileInState(`http://localhost:5000/api/mobiles/${mobileId}`, params, 'Live');

    check(waitingForLiveCheckResponse, {
      "is Mobile in state 'Live'": (r) => r !== null,
      'is status 200': (r) => r.status === 200
    });
  });
}


function loadData() {
  const data = JSON.parse(open("./data.json"));
  
  orderMobileData = data.orderMobile;
  completeProvisionData = data.completeProvision;
  activateMobileData = data.activateMobile;
  completeActivateData = data.completeActivate;
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