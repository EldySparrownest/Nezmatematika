using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Nezmatematika.Model
{
    public class TextStyling
    {
        public FontFamily FontFamily { get; set; }
        public double  FontSize { get; set; }
        public FontStyle FontStyle { get; set; }
        public FontWeight FontWeight { get; set; }
        public TextDecorationCollection TextDecorationsCol { get; set; }
        public SolidColorBrush ForegroundColour { get; set; }

        TextStyling()
        {
            FontFamily = new FontFamily("Cambria Math");
            FontSize = 16;
            FontStyle = FontStyles.Normal;
            FontWeight = FontWeights.Normal;
            TextDecorationsCol = new TextDecorationCollection();
            ForegroundColour = new SolidColorBrush(Colors.Black);
        }

        TextStyling(string fontName = "Cambria Math", double fontSize = 16, string fontStyleName = "Normal", string fontWeightName = "Normal",
            bool strikethrough = false, bool underline = false, string colorName = "Black")
        {
            try 
            {
                FontFamily = new FontFamily(fontName);
                FontSize = fontSize;
                FontStyle = (FontStyle)new FontStyleConverter().ConvertFrom(fontStyleName);
                FontWeight = (FontWeight)new FontWeightConverter().ConvertFrom(fontWeightName);
                TextDecorationsCol = new TextDecorationCollection();
                if (strikethrough)
                { TextDecorationsCol.Add(TextDecorations.Strikethrough[0]); }
                if (underline)
                { TextDecorationsCol.Add(TextDecorations.Underline[0]); }
                ForegroundColour = new SolidColorBrush((Color)new ColorConverter().ConvertFrom(colorName));
            }
            catch 
            {
               throw new Exception(
                   "Chyba při předání stylovacích parametrů.");
            }
        }
    }
}
