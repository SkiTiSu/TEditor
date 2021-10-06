using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TEditor.Models
{
    public class ImageLayerModel : BaseLayerModel
    {
        public string ImageUrl { get; set; }
        public bool VariableEnable { get; set; }
        public string VariableImageUrl { get; set; }
        public bool EmbedImage { get; set; } = false;
        public string Image { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
