using System;

namespace SECCS.Exceptions
{
    public sealed class FormattingException : Exception
    {
        public string Path { get; private set; }

        private readonly string BaseMessage;

        public override string Message => BaseMessage + (Path == null ? null : $" at {Path}");

        public FormattingException() : base()
        {
        }

        public FormattingException(string message)
        {
            this.BaseMessage = message;
        }

        public FormattingException(string message, Exception innerException) : base(null, innerException)
        {
            this.BaseMessage = message;
        }

        public FormattingException AppendPath(string path)
        {
            this.Path = $".{path}{this.Path}";
            return this;
        }
    }
}
