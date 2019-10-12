using System;

namespace SECCS.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class SeccsObjectAttribute : Attribute
    {
        public SeccsMemberSerializing MemberSerializing { get; set; } = SeccsMemberSerializing.OptOut;
    }

    public enum SeccsMemberSerializing
    {
        OptIn,
        OptOut
    }
}
