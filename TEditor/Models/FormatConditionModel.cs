using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Models
{
    public class FormatConditionModel
    {
        public const string DEFAULT_NAME = "默认";

        public string Name { get; set; }
        public string Condition { get; set; }
        public Dictionary<string, bool> LayersVisable { get; set; } = new Dictionary<string, bool>();

        public override string ToString()
        {
            return Name;
        }
    }
}
