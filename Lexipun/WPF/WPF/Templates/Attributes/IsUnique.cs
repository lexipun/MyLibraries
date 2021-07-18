using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexipun.Templates.Atributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IsUnique : Attribute
    {
        public int Id { get; set; }

        public IsUnique()
        {
            Id = -1;
        }

        public IsUnique(int id)
        {
            if (id >= 0)
            {
                Id = id;
                return;
            }

            Id = -1;
        }
    }
}
