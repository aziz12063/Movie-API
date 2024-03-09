using System;

namespace ApiApplication.CustomExceptions
{
    public class InvalidInPutException : Exception
    {
        public InvalidInPutException() : base("Invalid auditorium Id") 
        {
            
        }

        public InvalidInPutException(string message) : base(message) { }
        
    }
}
