using System.ComponentModel;

namespace Files.BackEnd
{
    public interface IArticle : INotifyPropertyChanged
    {
        string Path { get; }
        string Name { get; }
    }
}
