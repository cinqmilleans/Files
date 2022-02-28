using System.ComponentModel;

namespace Files.Backend.Models.Item
{
    public interface IItem : INotifyPropertyChanged
    {
        string Path { get; }
        string Name { get; }
    }
}
