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
            using (var images = typeof(StatusIcons).Assembly.GetManifestResourceStream("GitMenu.Resources.StatusIcons.bmp"))
            {
                if (images == null)
                    return null;

                var bitmap = (Bitmap)Image.FromStream(images, true);

                var imageList = new ImageList();
                imageList.ImageSize = new Size(8, bitmap.Height);
                bitmap.MakeTransparent(bitmap.GetPixel(0, 0));

                imageList.Images.AddStrip(bitmap);

                return imageList;
            }
        }
    }
}
