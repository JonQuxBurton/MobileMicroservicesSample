import http from "k6/http";
import { check, group, sleep } from 'k6';

let customerIds = [];
let mobileIds = [];
let provisionOrderIds = [];
let phoneNumbers = [];
let contactNames = [];
let contactPhoneNumbers = [];
let activateMobileMobileIds = [];
let activateMobileActivationCodes = [];
let completeActivateMobileIds = [];
let completeActivateActivateOrderIds = [];

loadData();

export let options = {
  vus: 5,
  iterations: 5
  //vus: 2,
  //duration: '3s'
};

const SLEEP_DURATION = 0.1;
const SLEEP_DURATION_FOR_ORDER_COMPLETION_CHECK = 10.0;
const RETRIES_FOR_ORDER_COMPLETION_CHECK = 12;

export default function () {
  let params = {
    headers: {
      'Content-Type': 'application/json',
    },
    tags: {}
  };

  //completeProvision(params);

  if (__VU == 2)
    orderMobile(params);
  else if (__VU == 3)
    completeProvision(params);
  else if (__VU == 4)
    activateMobile(params);
  else if (__VU == 5)
    completeActivate(params);
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

function orderMobile(params) {
  group('Order a Mobile', (_) => {
    params.tags.name = 'order-mobile';
    let customerId = customerIds[0];
    let phoneNumber = phoneNumbers[0];
    let contactName = contactNames[0];
    let contactPhoneNumber = contactPhoneNumbers[0];


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

function completeProvision(params) {
  group('The External Service has completed a Mobile Provision Order', (_) => {
    // Step 3 - The External Service has completed the Mobile Provision Order
    params.tags.name = 'complete-provision';
    let mobileId = mobileIds[0];
    let provisionOrderId = provisionOrderIds[0];
    let completeProvisionUrl = `http://localhost:5001/api/orders/${provisionOrderId}/complete`;

    let completeProvisionResponse = http.post(completeProvisionUrl, "", params);
    sleep(SLEEP_DURATION);

    check(completeProvisionResponse, {
      'is status 200': (r) => r.status === 200,
      'is activationCode present': (r) => r.json().hasOwnProperty('activationCode'),
    });
    //activationCode = completeProvisionResponse.json()['activationCode'];

    // Wait unitl the Order has been processed
    let waitingForActivateCheckResponse = waitUntilMobileInState(`http://localhost:5000/api/mobiles/${mobileId}`, params, 'WaitingForActivate');

    check(waitingForActivateCheckResponse, {
      'is status 200': (r) => r.status === 200
    });
  });

}

function activateMobile(params) {
  group('Activate a Mobile', (_) => {
    // Step 4 - Activate a Mobile
    params.tags.name = 'activate-mobile';
    let mobileId = activateMobileMobileIds[0];
    let activationCode = activateMobileActivationCodes[0];

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


function completeActivate(params) {
  group('The External Service has completed a Mobile Activate Order', (_) => {

    // Step  5 - The External Service has completed the Mobile Activate Order
    params.tags.name = 'complete-activate';
    let mobileId = completeActivateMobileIds[0];
    let activateOrderId = completeActivateActivateOrderIds[0];

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
  customerIds = data.customerIds;
  mobileIds = data.mobileIds;
  provisionOrderIds = data.provisionOrderIds;
  phoneNumbers = data.phoneNumbers;
  contactNames = data.contactNames;
  contactPhoneNumbers = data.contactPhoneNumbers;
  activateMobileMobileIds = data.activateMobileMobileIds;
  activateMobileActivationCodes = data.activateMobileActivationCodes;
  completeActivateMobileIds = data.completeActivateMobileIds;
  completeActivateActivateOrderIds = data.completeActivateActivateOrderIds;
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