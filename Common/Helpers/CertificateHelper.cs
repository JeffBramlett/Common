using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    /// <summary>
    /// Static class to help with any Certificate operations
    /// </summary>
    public static class CertificateHelper
    {
        /// <summary>
        /// Determine if the Certificate by subject is found.  (Matches on a partial subject, does not require the full subject)
        /// </summary>
        /// <param name="partialSubject">the partial subject of the certificate</param>
        /// <param name="storeLocation">which certificate store to use (defaults to Local Machine)</param>
        /// <returns>true if the certificate is found</returns>
        public static bool HasCertificateInStore(string partialSubject, StoreLocation storeLocation = StoreLocation.LocalMachine)
        {
            partialSubject.ThrowIfNull();

            X509Store store = new X509Store(StoreName.My, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            foreach (X509Certificate2 certificate in store.Certificates)
            {
                if (DoesCertificateMatch(partialSubject, certificate))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool DoesCertificateMatch(string subject, X509Certificate certificate)
        {
            string[] delimitedStrings = subject.Split(',');

            string certificateSubject = certificate.Subject;

            foreach (string delimited in delimitedStrings)
            {
                if (!certificateSubject.Contains(delimited.Trim()))
                {
                    return false;
                }
            }

            return true;
        }
    }}
