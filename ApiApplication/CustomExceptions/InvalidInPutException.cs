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
    /*******************************************************************************************************************/

    public class MappingException<TSource, TTarget> : Exception
    {
        public MappingException()
            : base() { }

        public MappingException(string message)
            : base(message) { }

        public MappingException(string message, Exception innerException)
            : base(message, innerException) { }

        // Override ToString to include the custom message
        public override string ToString()
        {
            string originalMessage = base.ToString();
            string customMessage = $"Unable to map from {typeof(TSource).Name} to {typeof(TTarget).Name}.";
            return $"{customMessage}\n{originalMessage}";
        }
    }

    public class DataRetrieveException<TSource> : Exception
    {
        public DataRetrieveException(string message) : base(message) { }
        public DataRetrieveException(string message, Exception innerException) : base(message, innerException) { }

        public override string ToString()
        {
            string originalMessage = base.ToString();
            string customMessage = $"Unable to retrieve data from {typeof(TSource).Name} .";
            return $"{customMessage}\n{originalMessage}";
        }
    }


    public class DataSaveException<TDestination> : Exception
    {
        public DataSaveException(string message) : base(message) { }
        public DataSaveException(string message, Exception innerException) : base(message, innerException) { }

        public override string ToString()
        {
            string originalMessage = base.ToString();
            string customMessage = $"Unable to save data to  {typeof(TDestination).Name}.";
            return $"{customMessage}\n{originalMessage}";
        }
    }

}
