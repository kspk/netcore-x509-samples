# Certificate Extensions
Certificate properties and extensions in a X509 certificate are stored in a form of Key-Value pairs within the certificate file. The key's used for such pairs are represented with **Object Identifiers**. 

[Object Identifiers](https://en.wikipedia.org/wiki/Object_identifier) are uniqiely assigned identifiers depending on the standards defined by ISO. OIDs are a series of numbers delimited with a period '.' that are akin to a tree like representation. For example the OID for Microsoft is defined as such:

`1 ..................... ISO`  
`1.3 ................... identified-organization`  
`1.3.6 ................. dod`  
`1.3.6.1 ............... internet`  
`1.3.6.1.4 ............. private`  
`1.3.6.1.4.1 ........... IANA enterprise numbers`  
`1.3.6.1.4.1.311 ....... Microsoft`  

Certificates use similar format to represent all their properties. Below is a compilation of common OIDs used in a certificates. We will use these values in our samples to access certificate properties. 

## List of common OIDs for Certificates

`2.5.29 ................ Certificate Extensions Root OID`  

All standard properties/extensions in a certificate use OIDs derived from a base OID, to store associated values. Custom certificate extensions with different OIDs can also be included at the time of certificate creation. This is leveraged by most organizations to include their private data within a certificate. 

---

`2.5.29.19 ............. Basic Constraints`  

Defines the basic constraints of a certificate and includes these properties:  
- Whether it is a Certificate Authority
- Whether it has a constraint on the path length on its child certificate chain
- The allowed length of its child certificate chain

---

`2.5.29.14 ............. Subject Key Identifier`

This represents a unique key to identify the certificate. Typically this is derieved from the Public Key of the certificate, but it can be optionally be set to a custom value which can be guranteed to be unique value. The _Subject Key Identifier_ is used as _Authority Key Identifier_ for any child certificates signed with a CA certificate. These hashed values help in faster lookups for building  certificate chains. 

---

`2.5.29.35 ............. Authority Key Identifier`

This value represents the _Subject Key Identifier_ of the signing CA certificate. Certificate chains are created based on these references to parent certificates.

---

`2.5.29.15 ............. Key Usage`

This value represents the valid usages of a certificate. This extension is typically used to allow limited usage, and applications that rely on certificates typically rely on this property to ensure they're using the right one.   

List of allowed values:
- Digital Signature
- Non-repudiation
- Key Encipherment 
- Data Encipherment
- Key Agreement
- Key Cert Sign
- Certificate Revocation List Sign
- Encipher Only
- Decipher Only

---

`2.5.29.37 ............. Extended Key Usage`

This property is used in addition to the _Key Usage_ property to define purposes for which the certificate can be used. 

List of allowed values:
- Server Authentication
- Client Authentication
- Code Signing
- Email
- Timestamping

---

`2.5.29.17 ............. Subject Alternate Name`

This property includes a set of alternate names that represent the identity defined by the CA.  

List of possible values:
- Email addresses
- IP addresses
- URIs
- DNS Names
- Distinguished Names

---

## References

[Wikipedia: Object Identifier](https://en.wikipedia.org/wiki/Object_identifier)  
[Certificate Extension OID Tree](http://oid-info.com/get/2.5.29)   