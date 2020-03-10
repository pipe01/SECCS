using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SECCS.Internal
{
    using static Expression;

    internal static class PositionGetter
    {
        private static readonly IDictionary<object, int> PositionStacks = new Dictionary<object, int>();
        private static readonly IDictionary<Type, Func<object, long>> Getters = new Dictionary<Type, Func<object, long>>();

        public static MethodInfo PushPositionMethod { get; } = typeof(PositionGetter).GetMethod(nameof(PushPosition));

        public static void PushPosition(this object buffer, string reason)
        {

            if (!PositionStacks.TryGetValue(buffer, out int stack))
            {
                stack = 1;
            }
            else
            {
                stack++;
            }
            PositionStacks[buffer] = stack;

            Debug.WriteLine($"{new string('│', stack - 1)}┌\t '{reason}' @ {buffer.GetPosition()}");
        }

        public static MethodInfo PopPositionMethod { get; } = typeof(PositionGetter).GetMethod(nameof(PopPosition));

        public static void PopPosition(this object buffer, string reason)
        {
            if (!PositionStacks.TryGetValue(buffer, out var stack))
            {
                throw new InvalidOperationException("Imbalanced push and pops");
            }

            stack--;

            if (stack == 0)
                PositionStacks.Remove(buffer);
            else
                PositionStacks[buffer] = stack;

            Debug.WriteLine($"{new string('│', stack)}└\t '{reason}' @ {buffer.GetPosition()}");
        }

        public static long GetPosition(this object buffer)
        {
            var t = buffer.GetType();

            if (!Getters.TryGetValue(t, out var getter))
            {
                var prop = t.GetProperty("Position");

                if (prop == null || (prop.PropertyType != typeof(int) && prop.PropertyType != typeof(long)))
                    throw new MissingMemberException("No property named 'Position' of type int or long has been found");

                Getters[t] = getter = CreateGetter(t, prop);
            }

            return getter(buffer);
        }

        private static Func<object, long> CreateGetter(Type t, PropertyInfo positionProp)
        {
            var objParam = Parameter(typeof(object), "obj");
            Expression valueGetter = Property(Convert(objParam, t), positionProp);

            if (positionProp.PropertyType == typeof(int))
                valueGetter = Convert(valueGetter, typeof(long));

            return Lambda<Func<object, long>>(valueGetter, objParam).Compile();
        }
    }
}
