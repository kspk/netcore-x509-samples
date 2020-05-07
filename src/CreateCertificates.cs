using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


namespace X509Samples
{
    partial class X509Sample
    {
        static X509Certificate2 CreateCertificateWithECDsa(
            string subject, 
            bool isCA,
            int pathLength,
            X509Certificate2 signer = null)
        {
            // Create the key for this certificate and initialize the keysiez
            var ecdsa = ECDsa.Create("ECDsa");
            ecdsa.KeySize = 256;
            var hash = HashAlgorithmName.SHA256;

            // Create the subject name for the new certificate
            var subjectName = new X500DistinguishedName(subject);

            // Create a certificate request - 
            // this is codified form of the ASN certificate request format. 
            var req = new CertificateRequest(
                subjectName,
                ecdsa,
                hash
            );

            // Create basic extension to represent if the certificate is a CA.
            req.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(isCA, pathLength != 0, pathLength, true)
            );

            // Create subject key identifier extension, helps speed up creating the certificate chain.
            var subjectKeyExtension = new X509SubjectKeyIdentifierExtension(req.PublicKey, false);
            req.CertificateExtensions.Add(subjectKeyExtension);

            // Create authority key identifier extension, helps find the signing cert when forming chain. 
            byte[] authorityKeyData = signer != null 
                ? signer.Extensions[CertificateExtensionOids.AuthorityKeyIdentifier].RawData 
                : subjectKeyExtension.RawData;

            var skisegment = new ArraySegment<byte>(authorityKeyData, 2, authorityKeyData.Length - 2);
            var authorityKeyIdentifier = new byte[skisegment.Count + 4];
            // These bytes define the KeyID part of the AuthorityKeyIdentifier
            authorityKeyIdentifier[0] = 0x30;
            authorityKeyIdentifier[1] = 0x16;
            authorityKeyIdentifier[2] = 0x80;
            authorityKeyIdentifier[3] = 0x14;
            skisegment.CopyTo(authorityKeyIdentifier, 4);

            req.CertificateExtensions.Add(
                new X509Extension(CertificateExtensionOids.AuthorityKeyIdentifier, authorityKeyIdentifier, false)
            );

            // Create the certificate, either selfsigned or signed by the signer. 
            X509Certificate2 cert = signer == null
                ? req.CreateSelfSigned(
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow.AddYears(1))
                : req.Create(
                    signer,
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow.AddYears(1),
                    Guid.NewGuid().ToByteArray());
            
            // When a certificate is signed by a CA, then its private key 
            // is not included in the new certificate by default. 
            // Include the private key explicitly.
            if(signer != null)
            {
                cert = cert.CopyWithPrivateKey(ecdsa);
            }

            return cert;
        }

        static X509Certificate2 CreateBasicECDsaCertificateWithIssuerName(
            string subject, 
            string issuer)
        {
            // Create the key for this certificate and initialize the keysiez
            var ecdsa = ECDsa.Create("ECDsa");
            ecdsa.KeySize = 256;
            var hash = HashAlgorithmName.SHA256;
            var sigen = X509SignatureGenerator.CreateForECDsa(ecdsa);

            // Create the subject name for the new certificate
            var subjectName = new X500DistinguishedName(subject);

            // Create a certificate request - 
            // this is codified form of the ASN certificate request format. 
            var req = new CertificateRequest(
                subjectName,
                ecdsa,
                hash
            );

            var issuerName = new X500DistinguishedName(issuer);

            X509Certificate2 cert = req.Create(
                issuerName,
                sigen,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddYears(1),
                Guid.NewGuid().ToByteArray()
            );

            cert = cert.CopyWithPrivateKey(ecdsa);

            return cert;
        }
    }
}