using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexipun.Templates.Atributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NonGenerateByTemplate: Attribute
    {
        public bool IsGeneration { get; set; }

        public NonGenerateByTemplate()
        {
            IsGeneration = false;
        }

        public NonGenerateByTemplate(bool isGenerate)
        {
            IsGeneration = isGenerate;
        }
    }
}
