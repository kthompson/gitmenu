using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace GitMenu
{
    public static class StatusIcons
    {
        public static ImageList CreateStatusImageList()
        {
            using (Stream images = typeof(StatusIcons).Assembly.GetManifestResourceStream("GitMenu.Resources.StatusIcons.bmp"))
            {
                if (images == null)
                    return null;

                Bitmap bitmap = (Bitmap)Image.FromStream(images, true);

                ImageList imageList = new ImageList();
                imageList.ImageSize = new Size(8, bitmap.Height);
                bitmap.MakeTransparent(bitmap.GetPixel(0, 0));

                imageList.Images.AddStrip(bitmap);

                return imageList;
            }
        }
    }
}
