using System;
using RC.Models.Enums;

namespace RC.Models
{
    public class ResponseMessage
    {
        public ResponseMessage()
        {

        }

        public ResponseMessage(ResponseStatus.Status status)
        {
            Status = status;
        }

        public ResponseMessage(ResponseStatus.Status status, Guid id)
        {
            Status = status;
            Id = id;
        }

        public ResponseMessage(ResponseStatus.Status status, string message)
        {
            Status = status;
            Message = message;
        }

        public ResponseStatus.Status Status { get; set; }

        public string Message { get; set; }

        public Guid? Id { get; set; }
    }
}
