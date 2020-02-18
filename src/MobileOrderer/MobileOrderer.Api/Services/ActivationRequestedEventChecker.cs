﻿using MobileOrderer.Api.Data;

namespace MobileOrderer.Api.Services
{
    public class ActivationRequestedEventChecker : IMobileEventsChecker
    {
        private readonly IGetNewActivationsQuery getNewActivationsQuery;
        private readonly IMobileCommand command;

        public ActivationRequestedEventChecker(
            IGetNewActivationsQuery getNewActivationsQuery,
            IMobileCommand command
            )
        {
            this.command = command;
            this.getNewActivationsQuery = getNewActivationsQuery;
        }

        public void Check()
        {
            var newMobiles = this.getNewActivationsQuery.Get();

            foreach (var newMobile in newMobiles)
            {
                this.command.Execute(newMobile);
            }
        }
    }
}
