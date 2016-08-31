using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TheStage.Elements.Base.Factories
{
    public abstract class ElementFactory
    {
        public abstract Dictionary<PrimitiveType, BitmapSource> Placeholders { get; protected set; }
        public abstract Dictionary<PrimitiveType, BitmapSource> Primitives { get; protected set; }

        internal static BitmapSource GetBitmapSource(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Bitmap bitmap = new Bitmap(Image.FromStream(stream));
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                    return bitmapimage;
                }
            }
        }
    }
}
