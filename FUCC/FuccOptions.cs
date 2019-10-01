namespace FUCC
{
    /// <summary>
    /// Options for <see cref="FuccFormatter{TBuffer}"/>.
    /// </summary>
    public sealed class FuccOptions
    {
        /// <summary>
        /// If a field with a class type is found when (de)serializing and this option is set to true, the field
        /// will be serialized field-by-field. If it's set to false, an exception will be thrown
        /// </summary>
        public bool SerializeUnknownTypes { get; set; } = true;

        /// <summary>
        /// Write a byte with value 243 (or 244 if <see cref="CheckStructureSignature"/> is true) at the beginning.
        /// </summary>
        public bool WriteHeader { get; set; } = true;

        /// <summary>
        /// Check for a byte with value 243 (or 244 if <see cref="CheckStructureSignature"/> is true) at the beginning.
        /// </summary>
        public bool CheckHeader { get; set; } = true;

        /// <summary>
        /// Writes an MD5 checksum at the beginning of the file generated from the serialized object's field types and names.
        /// </summary>
        public bool WriteStructureSignature { get; set; } = true;

        /// <summary>
        /// Checks for the class signature written by <see cref="WriteStructureSignature"/>.
        /// </summary>
        public bool CheckStructureSignature { get; set; } = true;
    }
}
