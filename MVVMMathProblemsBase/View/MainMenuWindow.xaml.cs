using MVVMMathProblemsBase.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MVVMMathProblemsBase.View
{
    /// <summary>
    /// Interakční logika pro MainMenuWindow.xaml
    /// </summary>

    //HLAVNÍ SEZNAM VĚCÍ NEZBYTNÝCH, KTERÉ MUSÍM DODĚLAT:
    // 1) najít způsob, jak serializovat kurzy a příklady (asi udělám helper třídy) - nějakým způsobem hotovo
    // 2) dořešit přepínání mezi příklady v editoru
    // 3) rozchodit studentský mód

    //Podtrhování a přeškrtávání pohromadě asi fungovat prostě nebude... Pokud mám mít jen jedno, bude to podtrhávání.
    //Dořešit scrollování na listview (bude potřeba nastudovat z internetu)
    //Ribbon, pokud to půjde nějak rozchodit


    public partial class MainMenuWindow : Window
    {
        MainMenuVM vM;

        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        public MainMenuWindow()
        {
            InitializeComponent();
            vM = Resources["vm"] as MainMenuVM;
            vM.CurrentMathProblemAboutToChangeToNotNull += ViewModel_CurrentMathProblemAboutToChange;
            vM.CurrentMathProblemChanged += ViewModel_CurrentMathProblemChanged;

            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cbEditorFontFamily.ItemsSource = fontFamilies;
            cbEditorFontFamily.SelectedItem = new FontFamily("Cambria Math");

            contentRichTextBox.FontFamily = cbEditorFontFamily.SelectedItem as FontFamily;
            contentRichTextBox.FontSize = Convert.ToDouble(cbEditorFontSize.Text);

            List<double> fontSizes = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 36, 40, 44, 48, 72 };
            cbEditorFontSize.ItemsSource = fontSizes;

            lvUsers.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
        }
        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb senderAsThumb = e.OriginalSource as Thumb;
            GridViewColumnHeader header = senderAsThumb.TemplatedParent as GridViewColumnHeader;
            if (header.Column.ActualWidth < MinWidth)
                header.Column.Width = MinWidth;
            if (header.Column.ActualWidth > MaxWidth)
                header.Column.Width = MaxWidth;
        }
        private void lvUsersColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lvUsers.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lvUsers.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
        private void lvContinuableCoursesColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lvContinuableCourses.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lvContinuableCourses.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private void ViewModel_CurrentMathProblemAboutToChange(object sender, EventArgs e)
        {
            if (vM.Settings.AutosaveProblemWhenSwitching)
            {
                btnSaveCourse_Click(sender, new RoutedEventArgs());
            }
        }
        private void ViewModel_CurrentMathProblemChanged(object sender, EventArgs e)
        {
            contentRichTextBox.Document.Blocks.Clear();
            if (vM.CurrentMathProblem != null)
            {
                if (!string.IsNullOrEmpty(vM.CurrentMathProblem.FilePath))
                {
                    FileStream fileStream = new FileStream(vM.CurrentMathProblem.FilePath, FileMode.Open);
                    var contents = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);
                    contents.Load(fileStream, DataFormats.Rtf);
                    fileStream.Close();
                }
            }
        }

        private void btnBold_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonChecked)
            { contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold); }
            else
            { contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal); }

        }

        private void btnItalics_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonChecked)
            { contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic); }
            else
            { contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal); }
        }

        private void btnUnderline_Click(object sender, RoutedEventArgs e)
        {
            //var textDecorations = new TextDecorationCollection();
            //if (!contentRichTextBox.Selection.IsEmpty)
            //{
            //    var tpFirst = contentRichTextBox.Selection.Start;
            //    var tpLast = contentRichTextBox.Selection.End;
            //    var textRange = new TextRange(tpFirst, tpFirst.GetPositionAtOffset(1));
            //    var underlined = (textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).Contains(TextDecorations.Underline[0]);
            //    for (TextPointer t = tpFirst; t.CompareTo(tpLast) < 0; t = t.GetPositionAtOffset(1))
            //    {
            //        textDecorations.Clear();
            //        textRange = new TextRange(t, t.GetPositionAtOffset(1));
            //        textDecorations = textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;
            //        if (!underlined)
            //        { textDecorations.Add(TextDecorations.Underline[0]); }
            //        else { textDecorations.TryRemove(TextDecorations.Underline, out textDecorations); }
            //        textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            //    }
            //}
            //else
            //{
            //    NewParagraphWhenSelectionIsEmpty();
            //}

            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            TextDecorationCollection textDecorations = new TextDecorationCollection();
            try
            {
                textDecorations.Add(contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection);
                if (isButtonChecked)
                { textDecorations.Add(TextDecorations.Underline); }
                else { textDecorations.TryRemove(TextDecorations.Underline, out textDecorations); }
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
            catch (Exception)
            {
                textDecorations = new TextDecorationCollection();
                if (!contentRichTextBox.Selection.IsEmpty)
                {
                    var tpFirst = contentRichTextBox.Selection.Start;
                    var tpLast = contentRichTextBox.Selection.End;
                    var textRange = new TextRange(tpFirst, tpFirst.GetPositionAtOffset(1));
                    var underlined = (textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).Contains(TextDecorations.Underline[0]);
                    for (TextPointer t = tpFirst; t.CompareTo(tpLast.GetPositionAtOffset(-1)) < 0; t = t.GetPositionAtOffset(1))
                    {
                        textDecorations.Clear();
                        textRange = new TextRange(t, t.GetPositionAtOffset(1));
                        textDecorations = textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;
                        if (!underlined)
                        { textDecorations.Add(TextDecorations.Underline[0]); }
                        else { textDecorations.TryRemove(TextDecorations.Underline, out textDecorations); }
                        textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
                    }
                }
                else
                {
                    InsertNewParagraphOrRun();
                }
            }
        }
        private void btnStrikethrough_Click(object sender, RoutedEventArgs e)
        {
            //var textDecorations = new TextDecorationCollection();
            //if (!contentRichTextBox.Selection.IsEmpty)
            //{
            //    var tpFirst = contentRichTextBox.Selection.Start;
            //    var tpLast = contentRichTextBox.Selection.End;
            //    var textRange = new TextRange(tpFirst, tpFirst.GetPositionAtOffset(1));
            //    var strikedthrough = (textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).Contains(TextDecorations.Strikethrough[0]);
            //    for (TextPointer t = tpFirst; t.CompareTo(tpLast) < 0; t = t.GetPositionAtOffset(1))
            //    {
            //        textDecorations.Clear();
            //        textRange = new TextRange(t, t.GetPositionAtOffset(1));
            //        textDecorations = textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;
            //        if (!strikedthrough)
            //        { textDecorations.Add(TextDecorations.Strikethrough[0]); }
            //        else { textDecorations.TryRemove(TextDecorations.Strikethrough, out textDecorations); }
            //        textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            //    }
            //}
            //else
            //{
            //    NewParagraphWhenSelectionIsEmpty();
            //}

            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            TextDecorationCollection textDecorations = new TextDecorationCollection();
            try
            {
                textDecorations.Add(contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection);
                if (isButtonChecked)
                { textDecorations.Add(TextDecorations.Strikethrough); }
                else { textDecorations.TryRemove(TextDecorations.Strikethrough, out textDecorations); }
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
            catch (Exception)
            {
                textDecorations = new TextDecorationCollection();
                if (!contentRichTextBox.Selection.IsEmpty)
                {
                    var tpFirst = contentRichTextBox.Selection.Start;
                    var tpLast = contentRichTextBox.Selection.End;
                    var textRange = new TextRange(tpFirst, tpFirst.GetPositionAtOffset(1));
                    var strikedthrough = (textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).Contains(TextDecorations.Strikethrough[0]);
                    for (TextPointer t = tpFirst; t.CompareTo(tpLast) < 0; t = t.GetPositionAtOffset(1))
                    {
                        textDecorations.Clear();
                        textRange = new TextRange(t, t.GetPositionAtOffset(1));
                        textDecorations = textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;
                        if (!strikedthrough)
                        { textDecorations.Add(TextDecorations.Strikethrough[0]); }
                        else { textDecorations.TryRemove(TextDecorations.Strikethrough, out textDecorations); }
                        textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
                    }
                }
                else
                {
                    InsertNewParagraphOrRun();
                }
            }
        }

        private void contentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedWeight = contentRichTextBox.Selection.GetPropertyValue(FontWeightProperty);
            btnBold.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && (selectedWeight.Equals(FontWeights.Bold));

            var selectedStyle = contentRichTextBox.Selection.GetPropertyValue(FontStyleProperty);
            btnItalics.IsChecked = (selectedStyle != DependencyProperty.UnsetValue) && (selectedStyle.Equals(FontStyles.Italic));

            var selectedDecoration = contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;

            //if (selectedDecoration != null && selectedDecoration != DependencyProperty.UnsetValue)
            //{
            //    btnUnderline.IsChecked = selectedDecoration.Contains(TextDecorations.Underline[0]);
            //    btnStrikethrough.IsChecked = selectedDecoration.Contains(TextDecorations.Strikethrough[0]);
            //}
            //else
            //{
            //    var tpFirst = contentRichTextBox.Selection.Start;
            //    var textRange = new TextRange(tpFirst, tpFirst.GetPositionAtOffset(1));
            //    btnStrikethrough.IsChecked = (textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).Contains(TextDecorations.Strikethrough[0]);
            //    btnUnderline.IsChecked = (textRange.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).Contains(TextDecorations.Underline[0]);
            //}

            btnUnderline.IsChecked = (selectedDecoration != null)
                && (selectedDecoration != DependencyProperty.UnsetValue)
                && selectedDecoration.Contains(TextDecorations.Underline[0]);

            btnStrikethrough.IsChecked = (selectedDecoration != null)
                && (selectedDecoration != DependencyProperty.UnsetValue)
                && selectedDecoration.Contains(TextDecorations.Strikethrough[0]);

            cbEditorFontFamily.SelectedItem = contentRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);

            if (contentRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty) != DependencyProperty.UnsetValue)
            { cbEditorFontSize.Text = (contentRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty)).ToString(); }
        }

        private Run MakeNewRunWithProperties()
        {
            var fontFamily = new FontFamily(cbEditorFontFamily.SelectedItem.ToString());
            var fontSize = Convert.ToDouble(cbEditorFontSize.Text);
            var fontStyle = (btnItalics.IsChecked ?? false) ? FontStyles.Italic : FontStyles.Normal;
            var fontWeight = (btnItalics.IsChecked ?? false) ? FontWeights.Bold : FontWeights.Normal;

            var textDecorations = new TextDecorationCollection();
            if (btnUnderline.IsChecked ?? false)
            { textDecorations.Add(TextDecorations.Underline[0]); }
            if (btnStrikethrough.IsChecked ?? false)
            { textDecorations.Add(TextDecorations.Strikethrough[0]); }

            Run newRun = new Run();
            newRun.FontFamily = fontFamily;
            newRun.FontSize = fontSize;
            newRun.FontStyle = fontStyle;
            newRun.FontWeight = fontWeight;
            newRun.Foreground = new SolidColorBrush((Color)wpfcpEditorFontColour.SelectedColor);
            newRun.TextDecorations = textDecorations;

            return newRun;
        }

        private Paragraph NewParagraphWhenSelectionIsEmpty()
        {
            var fontFamily = new FontFamily(cbEditorFontFamily.SelectedItem.ToString());
            var fontSize = Convert.ToDouble(cbEditorFontSize.Text);
            var fontStyle = (btnItalics.IsChecked ?? false) ? FontStyles.Italic : FontStyles.Normal;
            var fontWeight = (btnItalics.IsChecked ?? false) ? FontWeights.Bold : FontWeights.Normal;

            var textDecorations = new TextDecorationCollection();
            if (btnUnderline.IsChecked ?? false)
            { textDecorations.Add(TextDecorations.Underline[0]); }
            if (btnStrikethrough.IsChecked ?? false)
            { textDecorations.Add(TextDecorations.Strikethrough[0]); }

            Paragraph p = new Paragraph();
            p.FontFamily = fontFamily;
            p.FontSize = fontSize;
            p.FontStyle = fontStyle;
            p.FontWeight = fontWeight;
            p.TextDecorations = textDecorations;
            return p;
        }
        private void NewParagraphWhenSelectionIsEmpty(string text)
        {
            NewParagraphWhenSelectionIsEmpty();
            if (contentRichTextBox.CaretPosition.IsAtInsertionPosition)
            { contentRichTextBox.CaretPosition.InsertTextInRun(text); }
        }

        private void InsertNewParagraphOrRun()
        {
            // Check to see if we are at the start of the textbox and nothing has been added yet
            if (contentRichTextBox.Selection.Start.Paragraph == null)
            {
                // Add a new paragraph object to the richtextbox with the fontsize
                contentRichTextBox.Document.Blocks.Add(NewParagraphWhenSelectionIsEmpty());
            }
            else
            {
                // Get current position of cursor
                TextPointer curCaret = contentRichTextBox.CaretPosition;
                // Get the current block object that the cursor is in
                Block curBlock = contentRichTextBox.Document.Blocks.Where
                    (x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
                if (curBlock != null)
                {
                    Paragraph curParagraph = curBlock as Paragraph;
                    // Create a new run object with the fontsize, and add it to the current block
                    Run newRun = MakeNewRunWithProperties();
                    curParagraph.Inlines.Add(newRun);
                    // Reset the cursor into the new block. 
                    // If we don't do this, the font size will default again when you start typing.
                    contentRichTextBox.CaretPosition = newRun.ElementStart;
                }
            }
        }

        private void btnTextColour_Click(object sender, RoutedEventArgs e)
        {
            ApplyColourToText(colourPreviewTextBlock.Background);
        }

        private void colorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            var newColour = wpfcpEditorFontColour.SelectedColor;
            if (newColour != null)
            { ApplyColourToText(new SolidColorBrush((Color)newColour)); }
        }
        private void ApplyColourToText(Brush colourBrush)
        {
            if (contentRichTextBox != null)
            {
                if (!contentRichTextBox.Selection.IsEmpty)
                { contentRichTextBox.Selection.ApplyPropertyValue(Inline.ForegroundProperty, colourBrush); }
                else
                { NewParagraphWhenSelectionIsEmpty(); }
            }
        }

        private void cbEditorFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbEditorFontFamily.SelectedItem != null)
            {
                if (contentRichTextBox != null)
                {
                    if (!contentRichTextBox.Selection.IsEmpty)
                    { contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cbEditorFontFamily.SelectedItem); }
                    else
                    { NewParagraphWhenSelectionIsEmpty(); }
                    contentRichTextBox.Focus();
                }
            }
        }

        private void cbEditorFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(cbEditorFontSize.Text, out double d))
            { cbEditorFontSize.Text = String.Empty; }
            else
            {
                if (contentRichTextBox != null)
                {
                    if (!contentRichTextBox.Selection.IsEmpty)
                    { contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cbEditorFontSize.Text); }
                    else
                    { NewParagraphWhenSelectionIsEmpty(); }
                }
            }
            contentRichTextBox.Focus();
        }

        private void cbEditorFontFamily_LostFocus(object sender, RoutedEventArgs e)
        {
            contentRichTextBox.Focus();
        }
        private void cbEditorFontSize_LosFocus(object sender, RoutedEventArgs e)
        {
            contentRichTextBox.Focus();
        }

        private void BackToModeSelection(object sender, RoutedEventArgs e)
        {
            App.IsInStudentMode = null;
            UserTypeSelectWindow userTypeSelectWindow = new UserTypeSelectWindow();

            userTypeSelectWindow.Show();
            this.Close();
        }
        private void AppCurrentShutdown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnPi_Click(object sender, RoutedEventArgs e)
        {
            InsertTextFromButtonClick("π");
        }

        private void InsertTextFromButtonClick(string textToInsert)
        {
            if (!contentRichTextBox.Selection.IsEmpty)
            {
                contentRichTextBox.Selection.Text = textToInsert;
            }
            else
            {
                NewParagraphWhenSelectionIsEmpty(textToInsert);
            }
            contentRichTextBox.CaretPosition = contentRichTextBox.CaretPosition.GetPositionAtOffset(textToInsert.Length);
        }

        private void btnSaveCourse_Click(object sender, RoutedEventArgs e)
        {
            //string coursePath = System.IO.Path.Combine(courseDir, $"{vM.CurrentCourse.Id}{vM.CourseFilename}"); //PROČ se nejde dostat ke konstantám?!
            //vM.CurrentCourse.Save(coursePath);

            //JAK SMAZAT OBSAH CELÉHO ADRESÁŘE?

            //string rtfFile;
            //FileStream fileStream;

            if (vM.CurrentCourse != null)
            {
                vM.SaveCurrentCourse();
                if (vM.CurrentMathProblem != null)
                {
                    var contents = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);
                    vM.SaveMathProblem(vM.CurrentMathProblem, contents);
                }
            }

            //for (int i = 0; i < vM.CurrentCourse.Problems.Count; i++)
            //{
            //    //rtfFile = System.IO.Path.Combine(courseDir, String.Join(Convert.ToString(vM.CurrentCourse.Problems[i]), ".rtf"));
            //    //fileStream = new FileStream(rtfFile, FileMode.Create);
            //    //vM
            //    var contents = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);
            //    vM.SaveMathProblem(vM.CurrentCourse.Problems[i], contents);
            //}

            //vM.SelectedNote.FileLocation = rtfFile;
            //DatabaseHelper.Update(viewModel.SelectedNote);


            //var contents = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);
            //contents.Save(fileStream, DataFormats.Rtf);
        }
    }

    public static class GridColumn
    {
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.RegisterAttached("MinWidth", typeof(double), typeof(GridColumn), new PropertyMetadata(75d, (s, e) =>
            {
                if (s is GridViewColumn gridColumn)
                {
                    SetMinWidth(gridColumn);
                    ((System.ComponentModel.INotifyPropertyChanged)gridColumn).PropertyChanged += (cs, ce) =>
                    {
                        if (ce.PropertyName == nameof(GridViewColumn.ActualWidth))
                        {
                            SetMinWidth(gridColumn);
                        }
                    };
                }
            }));

        private static void SetMinWidth(GridViewColumn column)
        {
            double minWidth = (double)column.GetValue(MinWidthProperty);

            if (column.Width < minWidth)
                column.Width = minWidth;
        }

        public static double GetMinWidth(DependencyObject obj) => (double)obj.GetValue(MinWidthProperty);

        public static void SetMinWidth(DependencyObject obj, double value) => obj.SetValue(MinWidthProperty, value);
    }
    public class SortAdorner : Adorner
    {
        private static Geometry ascGeometry =
            Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

        private static Geometry descGeometry =
            Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

        public ListSortDirection Direction { get; private set; }

        public SortAdorner(UIElement element, ListSortDirection dir)
            : base(element)
        {
            this.Direction = dir;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            TranslateTransform transform = new TranslateTransform
                (
                    AdornedElement.RenderSize.Width - 15,
                    (AdornedElement.RenderSize.Height - 5) / 2
                );
            drawingContext.PushTransform(transform);

            Geometry geometry = ascGeometry;
            if (this.Direction == ListSortDirection.Descending)
                geometry = descGeometry;
            drawingContext.DrawGeometry(Brushes.Black, null, geometry);

            drawingContext.Pop();
        }
    }
}
