namespace SECCS.Tests
{
    public class BaseFormatTest<T> where T : new()
    {
        protected T Format => new T();
    }
}
