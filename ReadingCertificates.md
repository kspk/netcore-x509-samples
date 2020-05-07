# Reading Certificates
A certificate can be read in multiple ways, depending on where it is coming from, and what system the code is running on. We can read certificates directly from certificate files, as a _byte stream_ input, from certificate stores on a host, or created directly in code. 

## Reading from files or byte streams
Read from a certificate file directly: 
```cs
X509Certificate2 cert = new X509Certificate2(filename);
```

Or, read from a byte stream:
```csharp
X509Certificate2 cert = new X509Certificate2(certificateBytes);
```

Read a password protected file, and specify additional options (using [X509KeyStorageFlags](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509keystorageflags)):

```csharp
// Password can be plain text, or 
X509Certificate2 cert = new X509Certificate2(filename, "password", X509KeyStorageFlags.Exportable);
```

> .Net core doesn't yet support PEM format out of the box. A simple implementation to support reading and writing certificates in PEM format is discussed in [PEM Format](PemFormat.md) document. 

Once we have a certificate object, we use it to access its properties. Basic Certificate identifiers:
```csharp
Console.WriteLine($"Subject: {cert.Subject}");
Console.WriteLine($"Issuer name: {cert.Issuer}");
Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
Console.WriteLine($"Version: {cert.Version}");
```

Check if a certificate is a Certificate Authority:
```csharp
// BasicConstraintsExtension is a utility class to represent the BasicConstraints extension in the certificate
bool isca = (cert.Extensions[CertificateExtensionOids.BasicConstraints] as X509BasicConstraintsExtension).CertificateAuthority;
Console.WriteLine($"Certificate Authority: {isca}");
``` 

Get information about the keys in the certificate:
```csharp
Console.WriteLine($"Key Algorithm: {(cert.PublicKey.Key.SignatureAlgorithm)}");
Console.WriteLine($"Key size: {cert.PublicKey.Key.KeySize}");
Console.WriteLine($"Has Private Key: {cert.HasPrivateKey}");
```

Keys in the certificate can be exported externally:
```csharp
// Public Key
Console.WriteLine($"Public Key: {ByteArrayToHexString(cert.GetPublicKey())}");

// Private Key
Console.WriteLine($"Base64 Encoded Private Key: {ByteArrayToHexString(cert.PrivateKey.ExportPkcs8PrivateKey())}");
```

> To enable Private Key to be exportable, the `X509KeyStorageFlags.Exportable` must be specified when opening the certificate file.

Read certificate extensions - standard or custom:

```csharp
foreach(var ext in cert.Extensions)
{
    Console.WriteLine($"OID: {ext.Oid.Value}, Friendly name: {ext.Oid.FriendlyName}, Value: {ByteArrayToHexString(ext.RawData)}");
}
```
> OIDs are delimited numerical representation for extension keys. Some of the extensions have a friendly name, may vary across platforms. Although friendly names may be used to lookup specific extensions, we must uses OIDs as key to ensure reliability across different platforms. 

---
