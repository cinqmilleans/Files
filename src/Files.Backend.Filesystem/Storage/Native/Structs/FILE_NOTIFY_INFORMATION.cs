namespace Files.Backend.Filesystem.Storage
{
    public unsafe struct FILE_NOTIFY_INFORMATION
    {
        public uint NextEntryOffset;
        public uint Action;
        public uint FileNameLength;
        public fixed char FileName[1];
    }
}
