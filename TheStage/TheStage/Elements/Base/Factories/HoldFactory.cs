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
    class HoldFactory : ElementFactory
    {
        private Dictionary<PrimitiveType, BitmapSource> placeholders;
        private Dictionary<PrimitiveType, BitmapSource> primitives;
        public override Dictionary<PrimitiveType, BitmapSource> Placeholders { get { return placeholders; } protected set { placeholders = value; } }
        public override Dictionary<PrimitiveType, BitmapSource> Primitives { get { return primitives; } protected set { primitives = value; } }

        public HoldFactory()
        {
            Primitives = new Dictionary<PrimitiveType, BitmapSource>();
            Placeholders = new Dictionary<PrimitiveType, BitmapSource>();

            string path = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Elements\HoldElement\Placeholders";
            foreach (string file in Directory.GetFiles(path))
            {
                PrimitiveType type = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), Path.GetFileNameWithoutExtension(file), true);
                Placeholders.Add(type, GetBitmapSource(file));
            }

            path = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Elements\HoldElement\Primitives";
            foreach (string file in Directory.GetFiles(path))
            {
                PrimitiveType type = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), Path.GetFileNameWithoutExtension(file), true);
                Primitives.Add(type, GetBitmapSource(file));
            }
        }
    }
}
