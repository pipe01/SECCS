using System;
using System.Collections.Generic;
using System.Reflection;

namespace SECCS.Internal
{
    internal interface IFormatFinder<TFormat> where TFormat : IFormat
    {
        IEnumerable<TFormat> FindAll(Type bufferType);
    }

    internal class FormatFinder<TFormat> : IFormatFinder<TFormat> where TFormat : IFormat
    {
        public IEnumerable<TFormat> FindAll(Type bufferType)
        {
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (item.Namespace.StartsWith($"{nameof(SECCS)}.{nameof(Formats)}") && item.GetInterface(typeof(TFormat).Name) != null)
                {
                    var specialized = item.MakeGenericType(bufferType);

                    yield return (TFormat)Activator.CreateInstance(specialized);
                }
            }
        }
    }
}
