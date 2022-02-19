using System.ComponentModel;

namespace Files.BackEnd
{
    public interface IItem : INotifyPropertyChanged
    {
        string Path { get; }
        string Name { get; }
    }
}
