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

namespace MVVMMathProblemsBase.ViewModel.Helpers
{
    public static class TextRangeHelper
    {
        /// <summary>
        /// Returns the position of the specified offset in the text specified by the inlines.
        /// </summary>
        /// <param name="inlines">The inlines which specifies the text.</param>
        /// <param name="offset">The offset within the text to get the position of.</param>
        /// <param name="contentStartPosition">The position where the content starts. If null, the position before the start of the first inline will be used. If null and there are no inlines, an exception is thrown.</param>
        /// <returns>A <see cref="TextPointer"/> indicating the position of the specified offset.</returns>
        public static TextPointer GetPositionAtOffset(this InlineCollection inlines, int offset, TextPointer contentStartPosition = null)
        {
            if (inlines == null)
                throw new ArgumentNullException(nameof(inlines));
            if (!inlines.Any() && contentStartPosition == null)//if no inlines, can't determine start of content
                throw new ArgumentException("A content start position has to be specified if the inlines collection is empty.", nameof(contentStartPosition));

            if (contentStartPosition == null)
                contentStartPosition = inlines.First().ContentStart.DocumentStart;//if no content start specified, gets it
            int offsetWithInlineBorders = 0;//collects the value of offset (with inline borders)
            foreach (var inline in inlines)
            {
                int inlineLength = (inline as Run)?.Text.Length ?? (inline is LineBreak ? 1 : 0);//gets the length of the inline (length of a Run is the lengts of its text, length of a LineBreak is 1, other types are ignored)

                if (inlineLength < offset)//if position specified by the offset is beyond this inline...
                    offsetWithInlineBorders += inlineLength + 2;//...then the whole length is added with the two borders
                else if (inlineLength == offset)//if position specified by the offset is at the end of this inline...
                    offsetWithInlineBorders += inlineLength + 1;//...then the whole length is added with only the opening border
                else //inlineLength > value, if the position specified by the offset is within this inline
                {
                    offsetWithInlineBorders += offset + 1;//...then adds the remaining length (the offset itself), plus the opening border
                    break;//the inlines beyond are not needed
                }
                offset -= inlineLength;//substracts the added inline length
            }

            return contentStartPosition.GetPositionAtOffset(
                Math.Min(Math.Max(offsetWithInlineBorders, 0), contentStartPosition.GetOffsetToPosition(contentStartPosition.DocumentEnd)));//if the value is not within the boundaries of the text, returns the start or the end of the text
        }

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

        public static TextRange GetTextRangeBetweenTwoStrings(this TextPointer tp1, TextRange tr, string openTag, string closeTag)
        {
            MemoryStream stream = new MemoryStream();
            System.Windows.Markup.XamlWriter.Save(tr, stream);
            tr.Save(stream, DataFormats.XamlPackage);

            var flowDoc = new FlowDocument();
            var backupTR = new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd);
            backupTR.Load(stream, DataFormats.XamlPackage);

            var textToReduce = backupTR.Text;
            var textOpenTagStartIndex = textToReduce.IndexOf(openTag, StringComparison.OrdinalIgnoreCase);
            var textCloseTagStartIndex = textToReduce.IndexOf(closeTag, StringComparison.OrdinalIgnoreCase);

            while (textOpenTagStartIndex > -1)
            {
                
                textToReduce = backupTR.Text;
                textOpenTagStartIndex = textToReduce.IndexOf(openTag, StringComparison.OrdinalIgnoreCase);
            }

            //TODO: all the logic...

            return backupTR;
        }

        /// <summary>
        /// Returns the position of the specified offset in the text specified by the inlines.
        /// </summary>
        /// <param name="inlines">The inlines which specifies the text.</param>
        /// <param name="offset">The offset within the text to get the position of.</param>
        /// <param name="contentStartPosition">The position where the content starts. If null, the position before the start of the first inline will be used. If null and there are no inlines, an exception is thrown.</param>
        /// <returns>A <see cref="TextPointer"/> indicating the position of the specified offset.</returns>
        public static TextPointer MYGetPositionAtOffset(this InlineCollection inlines, int offset, TextPointer contentStartPosition = null)
        {
            if (inlines == null)
                throw new ArgumentNullException(nameof(inlines));
            if (!inlines.Any() && contentStartPosition == null)//if no inlines, can't determine start of content
                throw new ArgumentException("A content start position has to be specified if the inlines collection is empty.", nameof(contentStartPosition));

            if (contentStartPosition == null)
                contentStartPosition = inlines.First().ContentStart.DocumentStart;//if no content start specified, gets it
            int offsetWithInlineBorders = 0;//collects the value of offset (with inline borders)
            foreach (var inline in inlines)
            {
                int inlineLength = (inline as Run)?.Text.Length ?? (inline is LineBreak ? 1 : 0);//gets the length of the inline (length of a Run is the lengts of its text, length of a LineBreak is 1, other types are ignored)

                if (inlineLength < offset)//if position specified by the offset is beyond this inline...
                    offsetWithInlineBorders += inlineLength + 2;//...then the whole length is added with the two borders
                else if (inlineLength == offset)//if position specified by the offset is at the end of this inline...
                    offsetWithInlineBorders += inlineLength + 1;//...then the whole length is added with only the opening border
                else //inlineLength > value, if the position specified by the offset is within this inline
                {
                    offsetWithInlineBorders += offset + 1;//...then adds the remaining length (the offset itself), plus the opening border
                    break;//the inlines beyond are not needed
                }
                offset -= inlineLength;//substracts the added inline length
            }

            return contentStartPosition.GetPositionAtOffset(
                Math.Min(Math.Max(offsetWithInlineBorders, 0), contentStartPosition.GetOffsetToPosition(contentStartPosition.DocumentEnd)));//if the value is not within the boundaries of the text, returns the start or the end of the text
        }

        public static InlineCollection GetInlinesOfRTB(RichTextBox rtb)
        {
            TextBlock textBlock = new TextBlock();

            foreach (Paragraph paragraph in rtb.Document.Blocks)
            {
                foreach (var inline in paragraph.Inlines)
                {
                    textBlock.Inlines.Add(inline.Clone());
                }
            }
            return textBlock.Inlines;
        }

        public static Inline Clone(this Inline inline)
        {
            string text = XamlWriter.Save(inline);
            Stream s = new MemoryStream(ASCIIEncoding.Default.GetBytes(text));
            Inline clone = XamlReader.Load(s) as Inline;
            return clone;
        }
    }
}
