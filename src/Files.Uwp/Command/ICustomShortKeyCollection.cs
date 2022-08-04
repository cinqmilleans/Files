using System.Collections.Generic;
using System.Collections.Immutable;

namespace Files.Uwp.Command
{
    public interface ICustomShortKeyCollection : ICollection<ShortKey>
    {
        string Label { get; }

        ImmutableArray<ShortKey> DefaultShortKeys { get; }
    }
}
