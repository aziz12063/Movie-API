using System;

namespace ApiApplication.CustomExceptions
{
    public class InvalidInPutException : Exception
    {
        public InvalidInPutException() : base() 
        {
            
        }

        public InvalidInPutException(string message) : base(message) { }
        
    }
}
