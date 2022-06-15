namespace Files.Shared.Cloud
{
    public class CloudProvider : ICloudProvider
    {
        public CloudProviders ID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string SyncFolder { get; set; } = string.Empty;

        public byte[] IconData { get; set; } = new byte[0];

        public override int GetHashCode()
        {
            return $"{ID}|{SyncFolder}".GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ICloudProvider other)
            {
                return Equals(other);
            }
            return base.Equals(obj);
        }

        public bool Equals(ICloudProvider other)
        {
            return other != null && other.ID == ID && other.SyncFolder == SyncFolder;
        }
    }
}