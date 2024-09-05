using System;

namespace Agritracer.Domain
{
    public class DomainException : Exception
    {
        public DomainException(string bussinessMessage) : base(bussinessMessage) { }
    }
}
