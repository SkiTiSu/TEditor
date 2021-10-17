using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TEditor.Models;

namespace TEditor.Layers
{
    public class ImageLayer : LayerInner, ILayerWithVar
    {
        public override string Type { get; } = "图片";
        public override string Key { get; } = LayerType.Image;
        public override FrameworkElement LayerControl { get; protected set; } = new ImageLayerControl();
        public override object Model
        {
            get => model;
            set
            {
                // TODO 重写保存逻辑
                model = value as ImageLayerModel;
                if (model != null && model.Image != null)
                {
                    string imageBase64 = model.Image.Substring(model.Image.IndexOf(",") + 1);
                    fileByte = Convert.FromBase64String(imageBase64);
                }
                Init();
            }
        }

        public ImageLayer(Canvas canvasLayout, Layer canvasContent)
            : this(canvasLayout, canvasContent, new ImageLayerModel())
        {
            image = new BitmapImage(new Uri("pack://application:,,,/Resources/media_offline.png"));
            this.Width = Image.PixelWidth;
            this.Height = Image.PixelHeight;
        }

        public ImageLayer(Canvas canvasLayout, Layer canvasContent, object model)
            : base(canvasLayout, canvasContent)
        {
            Model = model;
        }

        private ImageLayerModel model;
        private MemoryStream inMemoryCopy;
        private byte[] fileByte;
        private BitmapImage image;

        public string ImageUrl
        {
            get => model.ImageUrl;
            set
            {
                model.ImageUrl = value;
                ReInit();
            }
        }

        public bool VariableEnable
        {
            get => model.VariableEnable;
            set
            {
                model.VariableEnable = value;
                EmbedImage = false;
            }
        }

        public string VariableImageUrl
        {
            get => model.VariableImageUrl;
            set => model.VariableImageUrl = value;
        }
        public bool EmbedImage
        {
            get => model.EmbedImage;
            set => model.EmbedImage = value;
        }

        public override double Width
        {
            get => model.Width;
            set
            {
                base.Width = value;
                model.Width = value;
            }
        }

        public override double Height
        {
            get => model.Height;
            set
            {
                base.Height = value;
                model.Height = value;
            }
        }

        private ICommand _chooseCommand;
        public ICommand ChooseCommand
        {
            get
            {
                return _chooseCommand ?? (_chooseCommand = new CommandHandler(() => ChooseAction(), () => CanExecute));
            }
        }
        public bool CanExecute
        {
            get => !VariableEnable;
        }

        public void ChooseAction()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            var imageExtensions = string.Join(";", ImageCodecInfo.GetImageDecoders().Select(ici => ici.FilenameExtension));
            dlg.Filter = $"图片文件|{imageExtensions}|所有文件|*.*";
            if (dlg.ShowDialog() == true)
            {
                ImageUrl = dlg.FileName;
            }
        }

        private void Init()
        {
            // TODO 是否换成有fileByte
            if (EmbedImage)
            {
                inMemoryCopy = new MemoryStream(fileByte);
                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = inMemoryCopy;
                image.EndInit();
            }
            else
            {
                ReInit();
            }
        }

        private void ReInit()
        {

            if (string.IsNullOrEmpty(ImageUrl) || !File.Exists(ImageUrl))
            {
                image = new BitmapImage(new Uri("pack://application:,,,/Resources/media_offline.png"));
            }
            else
            {
                fileByte = File.ReadAllBytes(ImageUrl);
                model.Image = $"data:{MimeMapping.MimeUtility.GetMimeMapping(ImageUrl)};base64,{Convert.ToBase64String(fileByte)}";
                inMemoryCopy = new MemoryStream(fileByte);
                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = inMemoryCopy;
                image.EndInit();
            }

            this.InvalidateVisual();
        }

        public BitmapImage Image
        {
            get
            {
                return image;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (image == null)
            {
                return;
            }
            drawingContext.DrawImage(image, new Rect(0, 0, Width, Height));
        }

        public void UpdateVar(Dictionary<string, string> keyValues)
        {
            if (VariableEnable)
            {
                string realImageUrl = VariableImageUrl;
                if (string.IsNullOrEmpty(realImageUrl)) { return; }
                foreach (var keyValue in keyValues)
                {
                    realImageUrl = realImageUrl.Replace("{" + keyValue.Key + "}", keyValue.Value);
                }
                ImageUrl = realImageUrl;
            }
        }
    }
}
