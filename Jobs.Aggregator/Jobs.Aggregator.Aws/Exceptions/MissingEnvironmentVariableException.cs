using System;

namespace Jobs.Aggregator.Aws.Exceptions
{
    public class MissingEnvironmentVariableException : Exception
    {
        public MissingEnvironmentVariableException(string variableName) : base($"Missing environment variable : {variableName}")
        {
            
        }
    }
}