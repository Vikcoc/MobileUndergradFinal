using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Response
{
    public class NetworkResponse<T>
    {
        public ErrorType ErrorType { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
    }
}
