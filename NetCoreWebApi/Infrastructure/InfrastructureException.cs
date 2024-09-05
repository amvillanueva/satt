using System;

namespace Infrastructure
{
    public class InfrastructureException: Exception
    {
        internal InfrastructureException(string businessMessage) : base(businessMessage){}
    }
}
