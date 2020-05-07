using System;
using System.IO;
using System.Collections;
using System.Linq;

namespace X509Samples
{
    partial class X509Sample
    {
        const string PfxFile = "ymclient.pfx";
        const string CerFile = "";
        const string PEMFile = "";
        static void Main(string[] args)
        {
            // var cert = ReadFromFile(PfxFile);
            // CheckIfCertificateAuthority(cert);

            // ReadKeysFromCertificate(PfxFile);
            // ReadExtensions(PfxFile);


            // var cert = CreateCertificateWithECDsa(
            //     "CN=Test Certificate",
            //     true, 
            //     2,
            //     null
            // );

            var cert = CreateBasicECDsaCertificateWithIssuerName(
                "CN=Test Certificate",
                "CN=Test Issuer"
            );
            ReadFromCertificate(cert);
        }

        internal static byte[] ReadFile(string filename)
        {
            byte[] data = new byte[] {};
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                int size = (int)fs.Length;
                data = new byte[size];
                size = fs.Read(data, 0, size);
            }

            return data;
        }

        internal static void WriteFile(string filename, byte[] content)
        {
            using(FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(content, 0, content.Length);
            }
        }

        internal static uint[] _Lookup32 = Enumerable.Range(0, 256).Select(i => {
            string s = i.ToString("X2");
            return ((uint)s[0]) + ((uint)s[1] << 16);
        }).ToArray();
        /// <summary>
        /// Derived from http://stackoverflow.com/a/24343727/48700
        /// </summary>
        internal static string ByteArrayToHexString(byte[] bytes) {
            var result = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++) {
                var val = _Lookup32[bytes[i]];
                result[2*i] = (char)val;
                result[2*i + 1] = (char) (val >> 16);
            }
            return new string(result);
        }
    }
}
