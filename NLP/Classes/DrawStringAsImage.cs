using PDFManipulator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PDFManipulator
{
    public  class DrawStringAsImage
    {


        public  System.Drawing.Image DrawTextImage(string s, String currencyCode, System.Drawing.Font font, System.Drawing.Color textColor, System.Drawing.Color backColor)
        {
            return DrawTextImage(s, currencyCode, font, textColor, backColor, System.Drawing.Size.Empty);
        }
        private static System.Drawing.Image DrawTextImage(string s, String currencyCode, System.Drawing.Font font, System.Drawing.Color textColor, System.Drawing.Color backColor, System.Drawing.Size minSize)
        {
            //first, create a dummy bitmap just to get a graphics object
            System.Drawing.SizeF textSize;
            using (System.Drawing.Image img = new Bitmap(1, 1, PixelFormat.Format32bppArgb))
            {
                using (System.Drawing.Graphics drawing = Graphics.FromImage(img))
                {
                    //measure the string to see how big the image needs to be
                    SizeF layoutSize = new SizeF(200.0F, 20.0f);
 
                    textSize = new SizeF();
                    textSize = drawing.MeasureString(s, font, layoutSize);

                   // textSize.Width = textSize.Width > minSize.Width ? textSize.Width : minSize.Width;
                   // textSize.Height = 18f;// textSize.Height > minSize.Height ? textSize.Height : minSize.Height;

                }
            }

            //create a new image of the right size
            System.Drawing.Image retImg = new Bitmap((int)textSize.Width, (int)textSize.Height, PixelFormat.Format32bppArgb);
            using (var drawing = Graphics.FromImage(retImg))
            {
                //paint the background
                drawing.Clear(System.Drawing.Color.Transparent);
                drawing.TextRenderingHint = TextRenderingHint.AntiAlias;
                //create a brush for the text
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    drawing.DrawString(s, font, textBrush, 3, -2);
                    drawing.Save();
                }
            }
            return retImg;
        }



    }
}
