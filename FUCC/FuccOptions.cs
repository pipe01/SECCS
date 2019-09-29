namespace FUCC
{
    public sealed class FuccOptions
    {
        public bool SerializeUnknownTypes { get; set; } = true;

        public bool WriteHeader { get; set; } = true;

        public bool CheckHeader { get; set; } = true;

        public bool WriteStructureSignature { get; set; } = true;

        public bool CheckStructureSignature { get; set; } = true;
    }
}
