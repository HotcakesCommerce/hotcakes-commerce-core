using System.Collections.Generic;

namespace Hotcakes.Shipping.Ups.Models
{
    public class ErrorResponse
    {
        public ResponseDetails Response { get; set; }

        public class ResponseDetails
        {
            public List<Error> Errors { get; set; }
        }

        public class Error
        {
            public string Code { get; set; }
            public string Message { get; set; }
        }
    }
}
