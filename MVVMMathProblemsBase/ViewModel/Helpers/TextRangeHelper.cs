using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Nezmatematika.ViewModel.Helpers
{
    public static class TextRangeHelper
    {
        public static TextPointer GetPositionVisibleCharactersAway(this TextPointer tp1, int length)
        {
            int charCount = 0;
            var nextTP = tp1;
            while (charCount < length)
            {
                nextTP = nextTP.GetNextContextPosition(LogicalDirection.Forward);
                charCount = new TextRange(tp1, nextTP).Text.Length;
            }
            return nextTP;
        }

        public static string GetTextRangeSubstringWithoutLineBreaks(TextRange range, int resultLength)
        {
            var blurb = range.Text.Trim();
            if (blurb.Contains("\r\n"))
                blurb = blurb.Substring(0, blurb.IndexOf("\r\n"));
            if (blurb.Contains("\n"))
                blurb = blurb.Substring(0, blurb.IndexOf("\n"));
            if (blurb.Length > resultLength)
                blurb = blurb.Substring(0, resultLength - 3) + "...";
            return blurb;
        }
    }
}
