namespace X509Samples
{
    public class CertificateExtensionOids
    {
        // Basic constraints of the certificate, whether it is a CA & allowed child path length
        public const string BasicConstraints = "2.5.29.19";

        // A unique hash used as an identifier for the certificate
        public const string SubjectKeyIdentifier = "2.5.29.14";

        // The Subject Key Identifier for the signing CA certificate
        public const string AuthorityKeyIdentifier = "2.5.29.35";

        // Allowed purposes for the certificate
        public const string KeyUsage = "2.5.29.15";

        // Additional allowed purposes for the certificate
        public const string ExtendedKeyUsage = "2.5.29.37";

        // Additional identifing names that can be included within the certificate
        public const string SubjectAlternateName = "2.5.29.17";
    }
}