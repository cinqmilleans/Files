using System.Collections.Generic;
using System.ComponentModel;

namespace Files.BackEnd
{
    public interface IItem : INotifyPropertyChanged
    {
        string Path { get; }
        string Name { get; }
    }

    public interface IItemProvider
    {
        IEnumerable<IItem> EnumerateItems();
    }

    internal interface IFactory<T>
    {
        T Build();
    }
}
