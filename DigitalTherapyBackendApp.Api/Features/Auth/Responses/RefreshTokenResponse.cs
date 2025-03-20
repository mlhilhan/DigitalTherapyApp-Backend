﻿namespace DigitalTherapyBackendApp.Api.Features.Auth.Responses
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
