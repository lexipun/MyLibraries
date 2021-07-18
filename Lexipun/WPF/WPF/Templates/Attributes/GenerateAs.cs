using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Lexipun.Templates.Atributes
{
    public enum TypeOfGeneration
    {
        Path,
        Password,
        NumericUpDown,
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class GenerateAs : Attribute
    {
        public TypeOfGeneration Generated { get; set; }
        public string WhichFiles { get; set; }

        public GenerateAs(TypeOfGeneration typeOfGeneration)
        {
            Generated = typeOfGeneration;

            if(Generated == TypeOfGeneration.Path)
            {
                WhichFiles = "(*.*)|*.*";
            }
        }

        public GenerateAs(TypeOfGeneration typeOfGeneration, string whichFiles)
        {
            if(typeOfGeneration != TypeOfGeneration.Path)
            {
                throw new ArgumentException("Constructor for generation files cannot get another type of generation");
            }

            Generated = typeOfGeneration;
            WhichFiles = whichFiles;
        }

        public GenerateAs(string whichFiles)
        {
            Generated = TypeOfGeneration.Path;
            WhichFiles = whichFiles;
        }

    }
}
