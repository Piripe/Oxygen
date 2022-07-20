using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Oxygen.Data.JS
{
    public class Gradient
    {
        public GradientDirection Direction { get; set; }
        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
        public float Point1 { get; set; }
        public float Point2 { get; set; }
        public Gradient? SourceGradient { get; set; }

        public Gradient(GradientDirection direction, Color color1, Color color2, float point1 = -1, float point2 = -1, Gradient? sourceGradient = null)
        {
            Direction = direction;
            Color1 = color1;
            Color2 = color2;
            Point1 = point1;
            Point2 = point2;
            SourceGradient = sourceGradient;
        }

        static public Gradient Horizontal(Color color1, Color color2, float point1 = -1, float point2 = -1)
        {
            return new Gradient(GradientDirection.Horizontal, color1, color2, point1, point2);
        }
        static public Gradient Horizontal(Gradient gradient, Color color1, Color color2, float point1 = -1, float point2 = -1)
        {
            return new Gradient(GradientDirection.Horizontal, color1, color2, point1, point2, gradient);
        }
        static public Gradient Vertical(Color color1, Color color2, float point1 = -1, float point2 = -1)
        {
            return new Gradient(GradientDirection.Vertical, color1, color2, point1, point2);
        }
        static public Gradient Vertical(Gradient gradient, Color color1, Color color2, float point1 = -1, float point2 = -1)
        {
            return new Gradient(GradientDirection.Vertical, color1, color2, point1, point2, gradient);
        }

        internal Image Apply(Image image)
        {
            Bitmap resultImage = new Bitmap(image.Width, image.Height);
            Graphics g = Graphics.FromImage(resultImage);

            int gradientWidth = Direction == GradientDirection.Horizontal ? image.Width : image.Height;

            Bitmap gradientColors = new Bitmap(gradientWidth, 1);
            Graphics gradientG = Graphics.FromImage(gradientColors);

            if (SourceGradient != null)
            {
                image = SourceGradient.Apply(image);
            }

            if (Point1 == -1 && Point2 == -1)
            {
                LinearGradientBrush gradientBrush = new LinearGradientBrush(new Point(0, 0), new Point(gradientWidth, 0), Color1, Color2);
                gradientBrush.GammaCorrection = true;
                gradientG.FillRectangle(gradientBrush, 0, 0, gradientWidth, 1);
            }
            else {
                Point point1 = new Point((int)Point1, 0);
                Point point2 = new Point((int)Point2, 0);
                if (Point1 - Point2 == 0)
                {
                    gradientG.FillRectangle(new SolidBrush(Color1), 0, 0, point1.X, 1);
                    gradientG.FillRectangle(new SolidBrush(Color2), Point2, 0, gradientWidth - Point2, 1);
                }
                else
                {
                    LinearGradientBrush gradientBrush = new LinearGradientBrush(point1, point2, Color1, Color2);
                    gradientBrush.GammaCorrection = true;
                    gradientG.FillRectangle(gradientBrush, Point1, 0, Point2 - Point1 + 1, 1);
                    gradientG.FillRectangle(new SolidBrush(Color1), 0, 0, Point1, 1);
                    gradientG.FillRectangle(new SolidBrush(Color2), Point2 + 1, 0, gradientWidth - Point2, 1);
                } 
            }
            for (int i = 0; i < gradientWidth; i++)
            {
                Color color = gradientColors.GetPixel(i, 0);

                ImageAttributes imageAttributes = new ImageAttributes();

                ColorMatrix colorMatrix = new ColorMatrix(getColorMatrix(color));

                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                if (Direction == GradientDirection.Horizontal)
                    g.DrawImage(image, new Rectangle(i, 0, 1, image.Height), i, 0, 1, image.Height, GraphicsUnit.Pixel, imageAttributes);
                else
                    g.DrawImage(image, new Rectangle(0, i, image.Width, 1), 0, i, image.Width, 1, GraphicsUnit.Pixel, imageAttributes);
            }

            return resultImage;
        }

        /// <summary>
        /// Return the color matrix used by <see cref="ApplyGradient"/>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static float[][] getColorMatrix(Color color)
        {
            float[][] colorMatrixElements = {
                                new float[] {(float)(color.R) / 255,  0,  0,  0, 0},
                                new float[] {0,  (float)(color.G) / 255,  0,  0, 0},
                                new float[] {0,  0,  (float)(color.B) / 255,  0, 0},
                                new float[] {0,  0,  0,  (float)(color.A) / 255, 0},
                                new float[] {0,  0,  0,  0, 1}};
            return colorMatrixElements;
        }

        public enum GradientDirection
        {
            Horizontal,
            Vertical,
        }
    }
}
