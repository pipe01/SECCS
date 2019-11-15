using Moq;
using NUnit.Framework;
using SECCS.DefaultFormats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SECCS.Tests.Formats
{
    [TestFixture]
    public class CollectionFormatTest
    {
        private static readonly object[] SerializeObjects =
        {
            new[] { 1, 2, 3 },
            new[] { 1, 2, 3 },
            new[] { "asd", "hello" },
            new[] { null, "asd", "foo" },
            new[] { "asd", null, "foo" },
            new[] { "asd", "foo", null },
            new object[] { 123, "foo", 42, "nice" },
            new object[] { 123, "foo", null, "nice" },
            new string[0],
            new int[0],

            new Dictionary<int, int>(),
            new Dictionary<int, int> { [1] = 2, [3] = 4 },
            new Dictionary<int, string> { [1] = "foo", [3] = "bar" },
            new Dictionary<int, string> { [1] = "", [3] = "" },
            new Dictionary<int, string> { [1] = null, [3] = "bar" },
            new Dictionary<int, string> { [1] = null, [3] = "" },
            new Dictionary<int, string> { [1] = "foo", [3] = null },
            new Dictionary<int, string> { [1] = "", [3] = null },
            new Dictionary<int, object> { [1] = new { my = "object", @is = "great" }, [3] = null },
            new Dictionary<int, object> { [1] = new { my = 123, @is = "great" }, [3] = null },
            new Dictionary<int, object> { [1] = new { my = "object", @is = 123 }, [3] = null },
            new Dictionary<int, object> { [1] = new { my = 123, @is = 123 }, [3] = null },
            new Dictionary<int, object> { [1] = (1, "asd", 3), [3] = null },

            new Dictionary<string, int> { ["one"] = 12, ["two"] = 32 },
            new Dictionary<string, int> { ["one"] = 12, [""] = 32 },
            new Dictionary<string, int> { [""] = 12, ["two"] = 32 },
        };

        [TestCaseSource(nameof(SerializeObjects))]
        public void Serialize(IEnumerable obj)
        {
            int objCount = obj.Cast<object>().Count();
            var serializedValues = new List<object>();

            var genericFormatMock = new Mock<ITypeFormat>();
            genericFormatMock.Setup(o => o.CanFormat(It.IsAny<Type>())).Returns(true);
            genericFormatMock.Setup(o => o.Serialize(It.IsAny<FormatContextWithValue>()))
                .Returns<FormatContextWithValue>(o => Expression.Call(
                    Expression.Constant((Action<object>)serializedValues.Add), "Invoke", null, Expression.Convert(o.Value, typeof(object))));

            var context = new FormatContextWithValue(
                    formats: new TypeFormatCollection<object>(new[] { genericFormatMock.Object }),
                    type: obj.GetType(),
                    bufferType: typeof(void),
                    buffer: Expression.Constant(null),
                    value: Expression.Constant(obj));

            var serializeExpr = new CollectionFormat().Serialize(context);

            Expression.Lambda<Action>(serializeExpr).Compile()();

            Assert.AreEqual(objCount, serializedValues[0]);
            CollectionAssert.AreEqual(obj, serializedValues.Skip(1));
        }
    }
}
