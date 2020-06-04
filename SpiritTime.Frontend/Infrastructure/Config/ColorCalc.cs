using System.Drawing;

namespace SpiritTime.Frontend.Infrastructure.Config
{
    public static class ColorCalc
    {
        public static Color ContrastColor(string colorHash)
        {
            var colConvert = new ColorConverter();
            return ContrastColor((Color)colConvert.ConvertFromString(colorHash));
        }

        public static string ContrastColorFromString(string colorHash)
        {
            var color = ContrastColor(colorHash);
            return "rgba(" + color.R + ", " + color.G + ", " + color.B + ", " + color.A + ")";
            
            //var color     = ContrastColor(converter.ConvertToString(colorHash));
            // var converter = TypeDescriptor.GetConverter(color);
            // return converter.ConvertToString(color);
        }

        public static Color ContrastColor(Color color)
        {
            int d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

            if (luminance > 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return Color.FromArgb(d, d, d);
        }
    }
}