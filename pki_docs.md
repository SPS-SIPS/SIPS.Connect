## Certificate Generation and Validation Process
#### Overview
understanding of the below commands is a mandatory requirement for the below procedure.
#### Pre-requisites
1. **OpenSSL**  
   Ensure that OpenSSL is installed on your machine.  
   ```bash
   # For macOS, use the following command
   brew install openssl

   # For Windows, download the installer from https://slproweb.com/products/Win32OpenSSL.html

   # For Linux, use the following command
   sudo apt-get install openssl
   ```
```bash
2. **Certificate Authority (CA)**
    Contact the SPS CA team to get a user account and access to the CA server.
```
1. **Generate a Certificate Signing Request (CSR) and Private Key**  
   Execute the following command to create a CSR and a private key:  
   ```bash
   openssl req -new -newkey rsa:2048 -nodes -keyout private.key -out request.csr -subj "/CN=<your-bic>"
   ```
### Procedure for Generating and Verifying Certificates Using OpenSSL
2. **Submit the CSR to the Certificate Authority (CA)**  
   Copy the CSR content and submit it to the CA server:  
   ```bash
   # For macOS, use the following command
   cat request.csr | pbcopy 

   # For Windows, use the following command
   type request.csr | clip

   # For Linux, use the following command
   xclip -sel clip < request.csr

   # just in case, you can also use the following command to copy the content
   cat request.csr

   ```
   Paste the content into the CA's CSR submission form.

3. **Retrieve the Certificate**  
   After the CA administrator approves the CSR, download the `certnew.p7b` file in PEM format.

4. **Extract the Certificate Chain**  
   Convert the `certnew.p7b` file into a PEM format chain:  
   ```bash
   openssl pkcs7 -print_certs -in certnew.p7b -out chain.pem

   # Extract the certificate from the chain file it's the first certificate in the chain
    openssl x509 -in chain.pem -out certificate.cer
    
    # You can also normally cut the certificate from the chain file using the following command
    cat chain.pem | sed -n '/-----BEGIN CERTIFICATE-----/,/-----END CERTIFICATE-----/p' > certificate.cer

    # Also you can use normal text editor to cut the certificate from the chain file and save it as certificate.cer file

    # Remove the certificate from the chain file
    sed -i '/-----BEGIN CERTIFICATE-----/,/-----END CERTIFICATE-----/d' chain.pem

    # Now the chain.pem file contains the chain of certificates (intermediate and root certificates)
   ```

5. **Validate Modulus Consistency**  
   Ensure the modulus of the private key, the certificate, and the CSR match:  
   ```bash
   openssl req -noout -modulus -in request.csr | openssl md5
   openssl x509 -noout -modulus -in certificate.cer | openssl md5
   openssl rsa -noout -modulus -in private.key | openssl md5
   ```

6. **Validate Certificate and Chain Modulus Consistency**  
   Verify that the modulus of the certificate matches the chain:  
   ```bash
   openssl x509 -noout -modulus -in certificate.cer | openssl md5
   openssl x509 -noout -modulus -in chain.pem | openssl md5
   ```

7. **Verify Certificate Validity**  
   Confirm the certificate's validity against the provided CA chain:  
   ```bash
   openssl verify -verbose -CAfile chain.pem certificate.cer
   ```

8. **Verify Certificate and Chain Completeness**  
   Check the certificate's validity and completeness of the chain:  
   ```bash
   openssl verify -verbose -CAfile chain.pem -untrusted chain.pem certificate.cer
   ```

9. **Install both intermediate and root certificates in your machine**  
   ```bash
    # For macOS, use the following command
    sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain intermediate.crt

    # For Windows, use the following command
    certutil -addstore -f "ROOT" intermediate.crt

    # For Linux, use the following command
    sudo cp intermediate.crt /usr/local/share/ca-certificates/intermediate.crt
    sudo update-ca-certificates
   ```

This structured process ensures the secure generation, verification, and validation of certificates.

## I do not use windows or linux, so the commands are not tested on those platforms. Please test them on your own before using them, or simply use macOS 😬.
## Certification Disclaimer
This document is intended only to provide general guidance to the certificate generation and validation process. It is not intended to provide legal advice or to be a comprehensive guide to the certificate generation and validation process. It is not intended to be a substitute for professional advice. You should not act upon information contained in this document without seeking professional advice. The information contained in this document is provided on an "as is" basis with no guarantees of completeness, accuracy, usefulness, or timeliness. The information contained in this document is subject to change without notice. The author disclaims all warranties, express or implied, including, but not limited to, the warranties of merchantability, fitness for a particular purpose, and non-infringement. In no event shall the author be liable for any direct, indirect, incidental, special, exemplary, or consequential damages (including, but not limited to, procurement of substitute goods or services; loss of use, data, or profits; or business interruption) however caused and on any theory of liability, whether in contract, strict liability, or tort (including negligence or otherwise) arising in any way out of the use of this document, even if advised of the possibility of such damage.
