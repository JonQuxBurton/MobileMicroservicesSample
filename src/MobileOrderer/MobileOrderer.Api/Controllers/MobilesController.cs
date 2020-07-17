﻿using Microsoft.AspNetCore.Mvc;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;
using System;
using Utils.DomainDrivenDesign;
using Utils.Enums;
using Utils.Guids;

namespace MobileOrderer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobilesController : ControllerBase
    {
        private readonly IRepository<Mobile> mobileRepository;
        private readonly IGuidCreator guidCreator;
        private readonly IMonitoring monitoring;

        public MobilesController(IRepository<Mobile> mobileRepository, IGuidCreator guidCreator, IMonitoring monitoring)
        {
            this.mobileRepository = mobileRepository;
            this.guidCreator = guidCreator;
            this.monitoring = monitoring;
        }

        [HttpPost("{id}/activate")]
        public IActionResult Post(Guid id, [FromBody] OrderToAdd orderToAdd)
        {
            var mobile = this.mobileRepository.GetById(id);

            if (mobile == null)
                return NotFound();

            var newStateName = new EnumConverter().ToName<Order.State>(Order.State.New);
            var orderType = new EnumConverter().ToName<Order.OrderType>(Order.OrderType.Activate);
            var dataEntity = new OrderDataEntity()
            {
                GlobalId = this.guidCreator.Create(),
                Name = orderToAdd.Name,
                ContactPhoneNumber = orderToAdd.ContactPhoneNumber,
                State = newStateName,
                Type = orderType
            };
            var inFlightOrder = new Order(dataEntity);

            mobile.Activate(inFlightOrder);
            mobileRepository.Update(mobile);

            monitoring.Activate();

            return new OkObjectResult(new OrderResource
            {
                GlobalId = dataEntity.GlobalId,
                Name = dataEntity.Name,
                ContactPhoneNumber = dataEntity.ContactPhoneNumber,
                State = dataEntity.State,
                Type = dataEntity.Type,
                CreatedAt = dataEntity.CreatedAt,
                UpdatedAt = dataEntity.UpdatedAt
            });
        }

        [HttpGet("{id}")]
        public ActionResult<Mobile> Get(Guid id)
        {
            var mobile = this.mobileRepository.GetById(id);

            if (mobile == null)
                return NotFound();

            return new OkObjectResult(mobile);
        }

        [HttpDelete("{id}")]
        public IActionResult Cease(Guid id)
        {
            var mobile = this.mobileRepository.GetById(id);

            if (mobile == null)
                return NotFound();

            var newStateName = new EnumConverter().ToName<Order.State>(Order.State.New);
            var orderType = new EnumConverter().ToName<Order.OrderType>(Order.OrderType.Cease);
            var dataEntity = new OrderDataEntity()
            {
                GlobalId = this.guidCreator.Create(),
                State = newStateName,
                Type = orderType
            };
            var inFlightOrder = new Order(dataEntity); 
            mobile.Cease(inFlightOrder);
            mobileRepository.Update(mobile);

            monitoring.Cease();

            return Accepted();
        }
    }
}