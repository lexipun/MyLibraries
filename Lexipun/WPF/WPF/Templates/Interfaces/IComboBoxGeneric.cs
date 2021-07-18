
namespace Lexipun.Templates.Interfaces
{
    public interface IComboBox<T>
    {
        T ChosenItem { get; set; }
        T[] GetArray();
    }
}
