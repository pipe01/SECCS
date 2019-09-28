using System.Reflection;

namespace FUCC
{
    public interface IBuffer
    {
        MethodInfo[] GetWriteMethods();
        MethodInfo[] GetReadMethods();
    }
}
