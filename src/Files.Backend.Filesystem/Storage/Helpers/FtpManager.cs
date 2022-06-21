using System.Collections.Generic;
using System.Net;

namespace Files.Backend.Filesystem.Storage
{
    public static class FtpManager
    {
        public static Dictionary<string, NetworkCredential> Credentials = new();

        public static readonly NetworkCredential Anonymous = new("anonymous", "anonymous");
    }
}