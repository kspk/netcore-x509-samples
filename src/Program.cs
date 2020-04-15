using System;
using System.IO;

namespace X509Samples
{
    partial class X509Sample
    {
        const string PfxFile = "ymclient.pfx";
        const string CerFile = "";
        const string PEMFile = "";
        static void Main(string[] args)
        {
            var cert = ReadFromFile(PfxFile);
            CheckIfCertificateAuthority(cert);
            ReadKeysFromCertificate(PfxFile);
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
    }
}
