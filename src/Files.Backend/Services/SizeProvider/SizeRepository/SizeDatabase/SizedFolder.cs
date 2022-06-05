﻿using LiteDB;

namespace Files.Backend.Services.SizeProvider
{
    internal class SizedFolder
    {
        [BsonId] public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public ulong Size { get; set; }
    }
}
