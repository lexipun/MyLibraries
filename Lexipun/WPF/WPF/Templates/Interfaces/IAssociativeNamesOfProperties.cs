using System.Collections.Generic;

namespace Lexipun.Templates.Interfaces
{
    public interface IAssociativeNamesOfProperties
    {
        Dictionary<string, string> GetListOfFieldsNames();
    }
}
