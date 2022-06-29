using System.Collections.Generic;
using System.Net;

namespace Files.Backend.Filesystem.Storage
{
    public static class FtpManager
    {
        public static readonly NetworkCredential Anonymous = new("anonymous", "anonymous");

        public static readonly IDictionary<string, NetworkCredential> Credentials = new Dictionary<string, NetworkCredential>();
    }
}
