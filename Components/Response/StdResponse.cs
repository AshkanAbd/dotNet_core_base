using System;

namespace dotNet_base.Components.Response
{
    public class StdResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        protected internal StdResponse(string status = null, string message = null, object data = null)
        {
            Status = status;
            Message = message;
            Data = data;
        }

        public string GetStatus()
        {
            return Status;
        }

        public string GetMessage()
        {
            return Message;
        }

        public object GetData()
        {
            return Data;
        }
    }
}