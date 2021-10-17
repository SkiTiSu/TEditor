using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TEditor.Models
{
    public class FormatConditionGroupModel
    {
        public FormatConditionGroupModel()
        {
            FormatConditionModels.Add(new FormatConditionModel() { Name = FormatConditionModel.DEFAULT_NAME });
            Random r = new Random();
            Color = Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255));
        }

        public string Name { get; set; }
        public Color Color { get; set; }
        public HashSet<string> EffctiveLayers { get; set; } = new HashSet<string>();
        public ObservableCollection<FormatConditionModel> FormatConditionModels { get; set; } = new ObservableCollection<FormatConditionModel>();
    }

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
