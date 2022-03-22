using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Item
{
    internal interface IDisplayNameCache
    {
        public string? ReadDisplayName(string path);
        public void SaveDisplayName(string path, string? displayName);
    }
}
