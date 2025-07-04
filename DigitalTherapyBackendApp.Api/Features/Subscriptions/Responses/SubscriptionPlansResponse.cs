﻿using DigitalTherapyBackendApp.Application.Dtos.Subscription;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses
{
    public class SubscriptionPlansResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<SubscriptionDetailsDto> Data { get; set; }
    }
}
