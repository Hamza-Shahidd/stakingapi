using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stakingapi.Models
{
    public class UserAuthRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}