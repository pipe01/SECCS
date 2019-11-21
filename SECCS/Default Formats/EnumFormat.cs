using System;
using System.Linq.Expressions;

namespace SECCS.DefaultFormats
{
    using static Expression;

    internal class EnumFormat : ITypeFormat
    {
        public bool CanFormat(Type type) => type.IsEnum;

        public Expression Deserialize(FormatContext context)
        {
            var enumType = context.Type.GetEnumUnderlyingType();
            return Convert(context.Read(enumType, reason: "enum"), context.Type);
        }

        public Expression Serialize(FormatContextWithValue context)
        {
            var enumType = context.Type.GetEnumUnderlyingType();
            return context.Write("enum", enumType, Convert(context.Value, enumType));
        }
    }
}
