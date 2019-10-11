namespace SECCS
{
    public interface ISeccsSerializable<TBuffer>
    {
        void Serialize(TBuffer buffer);
    }
}
