
using System;

namespace STOApi.Models
{
    public class TokenResponse
    {
        public int? ExpirationTime { set; get; }
        public string Token { set; get; }
    }
}
