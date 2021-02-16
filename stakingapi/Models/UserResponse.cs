using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace stakingapi.Models
{
    public class UserResponse
    {
        public HttpResponseMessage responseMsg { get; set; }

    }
    public class AdminKeys 
    {
        public string publicAddress { get; set; }
        public string privateKey { get; set; }
        public double? rewards { get; set; }
    }
}