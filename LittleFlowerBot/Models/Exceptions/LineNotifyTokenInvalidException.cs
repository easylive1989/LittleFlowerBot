using System;

namespace LittleFlowerBot.Models.Exceptions
{
    public class LineNotifyTokenInvalidException : Exception
    {
        private readonly string _message;

        public LineNotifyTokenInvalidException(string message)
        {
            _message = message;
        }

        public override string Message => _message;
    }
}