using System;

namespace SECCS
{
    public interface IFormat
    {
        bool CanFormat(Type type);
    }
}
