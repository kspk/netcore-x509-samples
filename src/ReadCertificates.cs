using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace X509Samples
{
    partial class X509Sample
    {
        // Read the certificate from a file and display its basic information.
        static X509Certificate2 ReadFromFile(string filename)
        {
            // Create the certificate object with a byte stream of the file. 
            // File could optionally be specified directly as well. 
            X509Certificate2 cert = new X509Certificate2(ReadFile(filename));

            Console.WriteLine($"Subject: {cert.Subject}");
            Console.WriteLine($"Issuer name: {cert.Issuer}");
            Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
            Console.WriteLine($"Version: {cert.Version}");
            Console.WriteLine($"Serial Number: {cert.SerialNumber}");
            Console.WriteLine($"Key Algorithm: {(cert.PublicKey.Key.SignatureAlgorithm)}");
            Console.WriteLine($"Key size: {cert.PublicKey.Key.KeySize}");
            Console.WriteLine($"Has Private Key: {cert.HasPrivateKey}");

            return cert;
        }

        // Check if a certificate is a Certificate Authority 
        static bool CheckIfCertificateAuthority(X509Certificate2 cert)
        {
            // Get the BasicConstraints extension, it contains the flag to denote if the certificate is a CA. 
            // 2.5.29.19 is the OID for BasicConstraints
            bool isca = (cert.Extensions[CertificateExtensionOids.BasicConstraints] as X509BasicConstraintsExtension).CertificateAuthority;
            Console.WriteLine($"Certificate Authority: {isca}");

            return isca;
        }

        static X509Certificate2 ReadKeysFromCertificate(string filename)
        {
            // Create the certificate object from a byte stream.
            // Set X509KeyStorageFlags to Exportable, to be able to print the private key.
            X509Certificate2 cert = new X509Certificate2(ReadFile(filename), "", X509KeyStorageFlags.Exportable);

            Console.WriteLine($"Key Algorithm: {(cert.PublicKey.Key.SignatureAlgorithm)}");
            Console.WriteLine($"Key size: {cert.PublicKey.Key.KeySize}");
            Console.WriteLine($"Has Private Key: {cert.HasPrivateKey}");

            // Get the public key from the utility function. This gives us a HEX encoding of the bytes of public key. 
            // Public key can optionally be derieved from the Public Key property. 
            Console.WriteLine($"Public Key: {ByteArrayToHexString(cert.GetPublicKey())}");

            // There is no utility method to get a string representation of the private key, 
            // So we export it to a byte array, and Base64 encode to print on the console. 
            Console.WriteLine($"Base64 Encoded Private Key: {ByteArrayToHexString(cert.PrivateKey.ExportPkcs8PrivateKey())}");

            return cert;
        }

        static X509Certificate2 ReadExtensions(string filename)
        {
            // Create the certificate object with a byte stream of the file. 
            // File could optionally be specified directly as well. 
            X509Certificate2 cert = new X509Certificate2(ReadFile(filename));

            foreach(var ext in cert.Extensions)
            {
                Console.WriteLine($"OID: {ext.Oid.Value}, Friendly name: {ext.Oid.FriendlyName}, Value: {ByteArrayToHexString(ext.RawData)}");
            }

            return cert;
        }
    }
}