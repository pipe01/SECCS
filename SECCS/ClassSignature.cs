using System;
using System.Security.Cryptography;
using System.Text;

namespace SECCS
{
    internal static class ClassSignature
    {
        public static string Get(Type type)
        {
            using (var md5 = MD5.Create())
            {
                md5.Initialize();

                foreach (var field in type.GetFields())
                {
                    Transform(field.FieldType.FullName + " " + field.Name);
                }

                md5.TransformFinalBlock(new byte[0], 0, 0);

                return BitConverter.ToString(md5.Hash).Replace("-", "");

                void Transform(string str)
                {
                    var bytes = Encoding.UTF8.GetBytes(str);
                    md5.TransformBlock(bytes, 0, bytes.Length, null, 0);
                }
            }
        }
    }
}
