using System.Collections.Generic;
using System.Linq.Expressions;

namespace SECCS.Internal
{
    using static Expression;

    internal static class WrappedExpressionExtensions
    {
        public static Expression Wrapped(this Expression expr, FormatContext context)
        {
            return Block(
                Call(PositionGetter.PushPositionMethod, context.Buffer, Constant(context.Reason)),
                expr,
                Call(PositionGetter.PopPositionMethod, context.Buffer, Constant(context.Reason)));
        }
    }
}
