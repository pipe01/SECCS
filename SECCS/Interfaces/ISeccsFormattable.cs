namespace SECCS.Interfaces
{
    public interface ISeccsWriteable<TWriter>
    {
        void Write(TWriter writer);
    }

    public interface ISeccsReadable<TReader>
    {
        void Read(TReader reader);
    }

    public interface ISeccsFormattable<TBuffer> : ISeccsWriteable<TBuffer>, ISeccsReadable<TBuffer>
    {
    }
}
