# Creating Certificates
Creating a certificate happens in 2 steps. We create a _Certificate Request_ and then generate the certificate based on that. A certificate request may be signed - or more appropriately executed - by another certificate which acts as a Certificate Authority, or self-signed in which case the generated certificate becomes the root certificate. 

## Create a certificate requests
Create a basic certificate request
```csharp
// A friendly name (canonical/distinguished). 
// There are a few other prefixes available, at least one is required. 
var subjectName = new X500DistinguishedName("CN=Test Certificate");

// Create a key for creating the certificate - specify key algorithm and key size. 
var ecdsa = ECDsa.Create("ECDsa");
ecdsa.KeySize = 256;

// Create a certificate request object that will be used to generate the certificate.
var req = new CertificateRequest(
    subjectName,    
    ecdsa,
    HashAlgorithmName.SHA256
);
```

Add Basic Constraints extension - Basic constraints of a certificate define whether it is a certificate authority or not. 
```csharp
req.CertificateExtensions.Add(
    new X509BasicConstraintsExtension(
        // certificateAuthority: Is the certificate an authority, 
        // aka. can it sign other certificates. 
        true, 

        // hasPathLengthConstraint: Does it have any path length limit, 
        // aka. is there a limit on the number of levels in certificate chain under it. 
        true, 

        // pathLengthConstraint: How many levels are allowed in the child hierarchy. 
        2, 

        // critical: Is this extension critical, for basic constraints it is typically yes. 
        true)
);
```

Add a subject key identifier extension - Subject keys are unique keys (smaller than the actual certificate key) that help the host platform to speed up certificate lookups from the certificate store.
```csharp
// The subject key is typically a hash of the public key, 
// sometimes it is also combined with the subject and few other properties 
// to compute the hash. Nevertheless it is expected to be globally unique.
var subjectKeyExtension = new X509SubjectKeyIdentifierExtension(req.PublicKey, false);
req.CertificateExtensions.Add(subjectKeyExtension);
```

Add an authority key identifier extension - Authority Keys are subject keys for the signing certificates. These help in creation of a certificate chain by allowing quick lookups of parent or signing certificates. 
```csharp
// Get the bytes from signer's subject key. 
// In case the certificate is self-signed root, then we can either skip this, 
// or set the authority key to the certificate's own subject key. 
byte[] authorityKeyData = signerSubjectKeyIdentifier;

var skisegment = new ArraySegment<byte>(authorityKeyData, 2, authorityKeyData.Length - 2);
var authorityKeyIdentifier = new byte[skisegment.Count + 4];
// These bytes define the KeyID part of the AuthorityKeyIdentifier
authorityKeyIdentifier[0] = 0x30;
authorityKeyIdentifier[1] = 0x16;
authorityKeyIdentifier[2] = 0x80;
authorityKeyIdentifier[3] = 0x14;
skisegment.CopyTo(authorityKeyIdentifier, 4);

req.CertificateExtensions.Add(
    new X509Extension(
        "2.5.29.35", 
        authorityKeyIdentifier, 
        false)
);
```
> This looks a little more complicated than it should, which is to ensure the correct format for Authority key is specified in the form of - "KeyID=\<signer-subject-key\>"

Add a custom extension - any custom extension can be added to a certificate. Extensions usually need an [OID](./CertificateExtensions.md) as a key. 
```csharp
req.CertificateExtensions.Add(
    new X509Extension(
        // A custom extension OID for Microsoft - '1.3.6.1.4.1.311' represents Microsoft Organization
        "1.3.6.1.4.1.311.100.1",

        // Bytes representing the value of the extension
        extBytes,

        // Custom extensions must not be specified as critical to maintain backward compatibility
        false
    )
);
```

>OIDs are hierarchical representation of identifier tree, and the higher order ints are reserved, so for a public distribution an OID must be secured to avoid conflicts. For internal use, any OID may be used as long as its use is understood and documented. 

## Create the certificate

Create a self signed certificate
```csharp
X509Certificate2 cert = req.CreateSelfSigned(
    // notBefore: Validity start time 
    DateTimeOffset.UtcNow,

    // notAfter: Validity end time
    DateTimeOffset.UtcNow.AddYears(1)
);
```

Create a child certificate, signed by a Certificate Authority
```csharp
X509Certificate2 cert = req.Create(
    // issuerCertificate: Signing certificate authority 
    signer,

    // notBefore: Validity start time
    DateTimeOffset.UtcNow,

    // notAfter: Validity end time
    DateTimeOffset.UtcNow.AddYears(1),

    // serialNumber: A serial number for the certificate. 
    // This is specific to the CA and used to track the list of certificates.
    Guid.NewGuid().ToByteArray()
);
```

Create a child certificate with the issuer name.
```csharp

// Create the issuer name for the new certificate
var issuerName = new X500DistinguishedName("CN=Test Issuer");

// Create a signature generator for the certificate key
var sigen = X509SignatureGenerator.CreateForECDsa(issuerEcdsa);

X509Certificate2 cert = req.Create(
    // issuerName : Distinguished name of the issuer
    issuerName,

    // generator: Signature generator that represents the issuing key
    sigen,

    // notBefore: Validity start time
    DateTimeOffset.UtcNow,

    // notAfter: Validity end time
    DateTimeOffset.UtcNow.AddYears(1),

    // serialNumber: A serial number for the certificate. 
    // This is specific to the CA and used to track the list of certificates.
    Guid.NewGuid().ToByteArray()
);
```