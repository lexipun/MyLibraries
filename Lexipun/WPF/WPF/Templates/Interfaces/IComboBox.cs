
namespace Lexipun.Templates.Interfaces
{
    public interface IComboBox
    {
        object ChosenItem { get; set; }
        object[] GetArray();
    }
}
