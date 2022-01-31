using MVVMMathProblemsBase.Model;
using MVVMMathProblemsBase.ViewModel;
using MVVMMathProblemsBase.ViewModel.Helpers;
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

    //POŽADAVKY V ZADÁNÍ BAKALÁŘSKÉ PRÁCE:
    // 2 režimy - student (procvičování úloh) a učitel (editace a správa úloh)
    // volba obtížnosti procvičování
    //  == (ne)možnost zobrazit kroky s návodem k řešení
    // zaznamenávání historie učení a sledování pokroků
    //  == statistika + zařazování příkladů s chybou na konec kurzu
    // interní editor musí podporovat export a import úloh
    //  == asi budu zazipovávat?

    //HLAVNÍ SEZNAM VĚCÍ NEZBYTNÝCH, KTERÉ MUSÍM DODĚLAT:
    // 1) najít způsob, jak serializovat kurzy a příklady (asi udělám helper třídy) - nějakým způsobem hotovo
    // 2) dořešit přepínání mezi příklady v editoru
    // 3) rozchodit studentský mód

    //Podtrhování a přeškrtávání pohromadě asi fungovat prostě nebude... Pokud mám mít jen jedno, bude to podtrhávání.

    //Když chci něco skrýt v design módu, tak tomu v .xaml přihodím d:IsHidden="True"

    public partial class MainMenuWindow : Window
    {
        MainMenuVM vM;

        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private Dictionary<string, RichTextBox> dicTeacherContentLoad;
        private Dictionary<string, RichTextBox> dicStudentContentLoad;
        private RichTextBox RTBWithFcs;

        public MainMenuWindow()
        {
            InitializeComponent();
            vM = Resources["vm"] as MainMenuVM;
            if (vM.IsInStudentMode == false)
            {
                vM.TeacherNotNullCurrentMathProblemAboutToChange += ViewModelTeacher_CurrentMathProblemAboutToChange;
                vM.TeacherCurrentMathProblemChanged += ViewModel_TeacherCurrentMathProblemChanged;
            }
            if (vM.IsInStudentMode == true)
            {
                //vM.TeacherNotNullCurrentMathProblemAboutToChange += ViewModelTeacher_CurrentMathProblemAboutToChange;
                vM.StudentCurrentMathProblemChanged += ViewModel_StudentCurrentMathProblemChanged;
            }

            RTBWithFcs = new RichTextBox { Visibility = Visibility.Collapsed };

            dicStudentContentLoad = new Dictionary<string, RichTextBox>()
            {
                { "ProblemText", rtbProblemTextStudentMode},
                { "Question", rtbQuestionStudentMode }
            };

            dicTeacherContentLoad = new Dictionary<string, RichTextBox>()
            {
                { "ProblemText", rtbProblemText},
                { "Question", rtbQuestion }
            };

            var initFontFamily = new FontFamily("Cambria Math");
            var initFontSize = 16;

            rtbProblemText.FontFamily = initFontFamily;
            rtbProblemText.FontSize = initFontSize;

            rtbQuestion.FontFamily = initFontFamily;
            rtbQuestion.FontSize = initFontSize;

            rtbCodeMode.FontFamily = initFontFamily;
            rtbCodeMode.FontSize = initFontSize;

            var p = new Paragraph();
            p.FontFamily = initFontFamily;
            p.FontSize = initFontSize;
            p.Margin = new Thickness(0);

            rtbProblemText.Document.Blocks.Clear();
            rtbProblemText.Document.Blocks.Add(p);

            rtbQuestion.Document.Blocks.Clear();
            rtbQuestion.Document.Blocks.Add(p);

            rtbCodeMode.Document.Blocks.Clear();
            rtbCodeMode.Document.Blocks.Add(p);

            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cbEditorFontFamily.ItemsSource = fontFamilies;
            cbEditorFontFamily.SelectedItem = initFontFamily;

            List<double> fontSizes = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 36, 40, 44, 48, 72 };
            cbEditorFontSize.ItemsSource = fontSizes;
            cbEditorFontSize.Text = initFontSize.ToString();

            lvUsers.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
        }
        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb senderAsThumb = e.OriginalSource as Thumb;
            GridViewColumnHeader header = senderAsThumb.TemplatedParent as GridViewColumnHeader;
            if (!(header is null))
            {
                if (header.Column.ActualWidth < MinWidth)
                    header.Column.Width = MinWidth;
                if (header.Column.ActualWidth > MaxWidth)
                    header.Column.Width = MaxWidth;
            }
        }
        private void ListViewColumnHeader_Click(object sender, RoutedEventArgs e, ListView listView)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                listView.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            listView.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
        private void lvUsersColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            ListViewColumnHeader_Click(sender, e, lvUsers);
        }
        private void lvContinuableCoursesColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            ListViewColumnHeader_Click(sender, e, lvContinuableCourses);
        }

        private void lvStartableNewCoursesColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            ListViewColumnHeader_Click(sender, e, lvStartableNewCourses);
        }

        private void lvStudentCoursesInProgressColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            ListViewColumnHeader_Click(sender, e, lvStudentCoursesInProgress);
        }

        private void ResetCourseEditor(object sender, RoutedEventArgs e)
        {
            btnCodeMode.IsChecked = false;
        }

        private void ViewModelTeacher_CurrentMathProblemAboutToChange(object sender, EventArgs e)
        {
            if (vM.Settings.AutosaveProblemWhenSwitching && App.WhereInApp == WhereInApp.CourseEditor)
            {
                btnSaveCourse_Click(sender, new RoutedEventArgs());
            }
        }

        private void ViewModel_StudentCurrentMathProblemChanged(object sender, EventArgs e)
        {
            rtbProblemTextStudentMode.Document.Blocks.Clear();
            rtbQuestionStudentMode.Document.Blocks.Clear();
            tbStudentsAnswer.Clear();

            if (vM.CurrentMathProblem != null)
            {
                if (!string.IsNullOrEmpty(vM.CurrentMathProblem.FilePath) && File.Exists(vM.CurrentMathProblem.FilePath))
                {
                    switch (App.WhereInApp)
                    {
                        case WhereInApp.ModeSelection:
                        case WhereInApp.MainMenu:
                        case WhereInApp.CourseEditor:
                            break;
                        case WhereInApp.CourseForStudent:
                            LoadMathProblemIntoRTBs(dicStudentContentLoad);
                            tbStudentsAnswer.Text = "";
                            break;
                    }
                }
            }
        }

        private void ViewModel_TeacherCurrentMathProblemChanged(object sender, EventArgs e)
        {
            if (vM.CurrentMathProblem != null)
            {
                if (!string.IsNullOrEmpty(vM.CurrentMathProblem.FilePath) && File.Exists(vM.CurrentMathProblem.FilePath))
                {
                    switch (App.WhereInApp)
                    {
                        case WhereInApp.ModeSelection:
                        case WhereInApp.MainMenu:
                            break;
                        case WhereInApp.CourseEditor:
                            rtbCodeMode.Document.Blocks.Clear();
                            rtbProblemText.Document.Blocks.Clear();
                            rtbQuestion.Document.Blocks.Clear();
                            LoadMathProblemIntoRTBs(dicTeacherContentLoad);
                            break;
                    }
                }
            }
        }

        private void btnBold_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonChecked)
            { RTBWithFcs.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold); }
            else
            { RTBWithFcs.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal); }

        }

        private void btnItalics_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonChecked)
            { RTBWithFcs.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic); }
            else
            { RTBWithFcs.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal); }
        }

        private void btnUnderline_Click(object sender, RoutedEventArgs e)
        {
            //var textDecorations = new TextDecorationCollection();
            //if (!RTBWithFocus.Selection.IsEmpty)
            //{
            //    var tpFirst = RTBWithFocus.Selection.Start;
            //    var tpLast = RTBWithFocus.Selection.End;
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
                textDecorations.Add(RTBWithFcs.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection);
                if (isButtonChecked)
                { textDecorations.Add(TextDecorations.Underline); }
                else { textDecorations.TryRemove(TextDecorations.Underline, out textDecorations); }
                RTBWithFcs.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
            catch (Exception)
            {
                textDecorations = new TextDecorationCollection();
                if (!RTBWithFcs.Selection.IsEmpty)
                {
                    var tpFirst = RTBWithFcs.Selection.Start;
                    var tpLast = RTBWithFcs.Selection.End;
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
            //if (!RTBWithFocus.Selection.IsEmpty)
            //{
            //    var tpFirst = RTBWithFocus.Selection.Start;
            //    var tpLast = RTBWithFocus.Selection.End;
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
                textDecorations.Add(RTBWithFcs.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection);
                if (isButtonChecked)
                { textDecorations.Add(TextDecorations.Strikethrough); }
                else { textDecorations.TryRemove(TextDecorations.Strikethrough, out textDecorations); }
                RTBWithFcs.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
            catch (Exception)
            {
                textDecorations = new TextDecorationCollection();
                if (!RTBWithFcs.Selection.IsEmpty)
                {
                    var tpFirst = RTBWithFcs.Selection.Start;
                    var tpLast = RTBWithFcs.Selection.End;
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

        private void RTBWithFcs_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedWeight = RTBWithFcs.Selection.GetPropertyValue(FontWeightProperty);
            btnBold.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && (selectedWeight.Equals(FontWeights.Bold));

            var selectedStyle = RTBWithFcs.Selection.GetPropertyValue(FontStyleProperty);
            btnItalics.IsChecked = (selectedStyle != DependencyProperty.UnsetValue) && (selectedStyle.Equals(FontStyles.Italic));

            var selectedDecoration = RTBWithFcs.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;

            //if (selectedDecoration != null && selectedDecoration != DependencyProperty.UnsetValue)
            //{
            //    btnUnderline.IsChecked = selectedDecoration.Contains(TextDecorations.Underline[0]);
            //    btnStrikethrough.IsChecked = selectedDecoration.Contains(TextDecorations.Strikethrough[0]);
            //}
            //else
            //{
            //    var tpFirst = RTBWithFocus.Selection.Start;
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

            cbEditorFontFamily.SelectedItem = RTBWithFcs.Selection.GetPropertyValue(Inline.FontFamilyProperty);

            if (RTBWithFcs.Selection.GetPropertyValue(Inline.FontSizeProperty) != DependencyProperty.UnsetValue)
            { cbEditorFontSize.Text = (RTBWithFcs.Selection.GetPropertyValue(Inline.FontSizeProperty)).ToString(); }
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
            if (RTBWithFcs.CaretPosition.IsAtInsertionPosition)
            { RTBWithFcs.CaretPosition.InsertTextInRun(text); }
        }

        private void InsertNewParagraphOrRun()
        {
            // Check to see if we are at the start of the textbox and nothing has been added yet
            if (RTBWithFcs.Selection.Start.Paragraph == null)
            {
                // Add a new paragraph object to the richtextbox with the fontsize
                RTBWithFcs.Document.Blocks.Add(NewParagraphWhenSelectionIsEmpty());
            }
            else
            {
                // Get current position of cursor
                TextPointer curCaret = RTBWithFcs.CaretPosition;
                // Get the current block object that the cursor is in
                Block curBlock = RTBWithFcs.Document.Blocks.Where
                    (x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
                if (curBlock != null)
                {
                    Paragraph curParagraph = curBlock as Paragraph;
                    // Create a new run object with the fontsize, and add it to the current block
                    Run newRun = MakeNewRunWithProperties();
                    curParagraph.Inlines.Add(newRun);
                    // Reset the cursor into the new block. 
                    // If we don't do this, the font size will default again when you start typing.
                    RTBWithFcs.CaretPosition = newRun.ElementStart;
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
            if (RTBWithFcs != null)
            {
                if (!RTBWithFcs.Selection.IsEmpty)
                { RTBWithFcs.Selection.ApplyPropertyValue(Inline.ForegroundProperty, colourBrush); }
                else
                { NewParagraphWhenSelectionIsEmpty(); }
            }
        }

        private void cbEditorFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbEditorFontFamily.SelectedItem != null)
            {
                if (RTBWithFcs != null)
                {
                    if (!RTBWithFcs.Selection.IsEmpty)
                    { RTBWithFcs.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cbEditorFontFamily.SelectedItem); }
                    else
                    { NewParagraphWhenSelectionIsEmpty(); }
                    RTBWithFcs.Focus();
                }
            }
        }

        private void cbEditorFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(cbEditorFontSize.Text, out double d))
            { cbEditorFontSize.Text = String.Empty; }
            else
            {
                if (RTBWithFcs != null)
                {
                    if (!RTBWithFcs.Selection.IsEmpty)
                    { RTBWithFcs.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cbEditorFontSize.Text); }
                    else
                    { NewParagraphWhenSelectionIsEmpty(); }
                }
            }
            RTBWithFcs.Focus();
        }

        private void cbEditorFontFamily_LostFocus(object sender, RoutedEventArgs e)
        {
            RTBWithFcs.Focus();
        }

        private void cbEditorFontSize_LosFocus(object sender, RoutedEventArgs e)
        {
            RTBWithFcs.Focus();
        }

        private void SetRTBWithFocus(object sender, RoutedEventArgs e)
        {
            RTBWithFcs = sender as RichTextBox;
        }

        private void btnCodeMode_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonChecked)
            {
                rtbCodeMode.Document.Blocks.Clear();
                rtbCodeMode.Document = FlowDocumentOfCurrentMathProblem();
            }
        }

        private FlowDocument FlowDocumentOfCurrentMathProblem()
        {
            FlowDocument newdocument = new FlowDocument();
            newdocument.Blocks.Add(new Paragraph { Margin = new Thickness(0) });

            newdocument.Blocks.OfType<Paragraph>().Last().Inlines.Add(new Run("<ProblemText>"));
            AddDocument(rtbProblemText.Document, newdocument);
            newdocument.Blocks.OfType<Paragraph>().Last().Inlines.Add(new Run("</ProblemText>\n"));
            newdocument.Blocks.Add(new Paragraph { Margin = new Thickness(0) });
            newdocument.Blocks.OfType<Paragraph>().Last().Inlines.Add(new Run("<Question>"));
            AddDocument(rtbQuestion.Document, newdocument);
            newdocument.Blocks.OfType<Paragraph>().Last().Inlines.Add(new Run("</Question>"));
            return newdocument;
        }

        public static void AddDocument(FlowDocument from, FlowDocument to)
        {
            TextRange range = new TextRange(from.ContentStart, from.ContentEnd);
            MemoryStream stream = new MemoryStream();
            System.Windows.Markup.XamlWriter.Save(range, stream);
            range.Save(stream, DataFormats.XamlPackage);

            TextRange range2 = new TextRange(to.ContentEnd, to.ContentEnd);
            range2.Load(stream, DataFormats.XamlPackage);
        }

        private void AllRTBToRTF(string dirpath, int fileNbr) //only used when debugging
        {
            var problemPath = System.IO.Path.Combine(dirpath, $"{fileNbr}problem.rtf");
            var questionPath = System.IO.Path.Combine(dirpath, $"{fileNbr}question.rtf");
            var codeModePath = System.IO.Path.Combine(dirpath, $"{fileNbr}codemode.rtf");
            SaveRTF(problemPath, new TextRange(rtbProblemText.Document.ContentStart, rtbProblemText.Document.ContentEnd));
            SaveRTF(questionPath, new TextRange(rtbQuestion.Document.ContentStart, rtbQuestion.Document.ContentEnd));
            SaveRTF(codeModePath, new TextRange(rtbCodeMode.Document.ContentStart, rtbCodeMode.Document.ContentEnd));
        }
        private static void SaveRTF(string path, TextRange tr) //only used when debugging
        {
            try
            {
                FileStream fileStream = new FileStream(path, FileMode.Create);
                tr.Save(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void BackToModeSelection(object sender, RoutedEventArgs e)
        {
            App.WhereInApp = WhereInApp.ModeSelection;
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
            if (!RTBWithFcs.Selection.IsEmpty)
            {
                RTBWithFcs.Selection.Text = textToInsert;
            }
            else
            {
                NewParagraphWhenSelectionIsEmpty(textToInsert);
            }
            RTBWithFcs.CaretPosition = RTBWithFcs.CaretPosition.GetPositionAtOffset(textToInsert.Length);
        }

        private void LoadMathProblemIntoRTBs(Dictionary<string, RichTextBox> tagRTBDictionary)
        {
            FileStream fileStream = new FileStream(vM.CurrentMathProblem.FilePath, FileMode.Open);
            var trCodeMode = new TextRange(rtbCodeMode.Document.ContentStart, rtbCodeMode.Document.ContentEnd);
            trCodeMode.Load(fileStream, DataFormats.Rtf);
            fileStream.Close();

            var tpContentStart = rtbCodeMode.Document.ContentStart;

            for (int i = 0; i < dicTeacherContentLoad.Count; i++)
            {
                var openTag = $"<{tagRTBDictionary.ElementAt(i).Key}>";
                var closeTag = $"</{tagRTBDictionary.ElementAt(i).Key}>";
                var rtbTarget = tagRTBDictionary.ElementAt(i).Value;

                var textOpenTagStartIndex = trCodeMode.Text.IndexOf(openTag, StringComparison.OrdinalIgnoreCase);
                var textCloseTagStartIndex = trCodeMode.Text.IndexOf(closeTag, StringComparison.OrdinalIgnoreCase);

                if (textOpenTagStartIndex < 0 || textCloseTagStartIndex < 0)
                {
                    MessageBoxResult result = MessageBox.Show("Při načítání příkladu ze souboru byla zjištěna chyba.\n" +
                                                                "Nouzovým načtením bude celý obsah souboru načten do textového pole \"Zadání\".\n" +
                                                                "Chcete se pokusit příklad načíst nouzově?",
                                                                "Nezmatematika - chyba načtení příkladu",
                                                                MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        tagRTBDictionary.ElementAt(0).Value.Document.Blocks.Clear();
                        AddDocument(rtbCodeMode.Document, tagRTBDictionary.ElementAt(0).Value.Document);
                    }
                    return;
                }

                var tpStart = tpContentStart.GetPositionVisibleCharactersAway(textOpenTagStartIndex + openTag.Length);
                var tpEnd = tpContentStart.GetPositionVisibleCharactersAway(textCloseTagStartIndex);

                using (var stream = new MemoryStream())
                {
                    new TextRange(tpStart, tpEnd).Save(stream, DataFormats.XamlPackage);
                    new TextRange(rtbTarget.Document.ContentEnd, rtbTarget.Document.ContentEnd).Load(stream, DataFormats.XamlPackage);
                }
            }
        }

        private void btnPublishCourse_Click(object sender, RoutedEventArgs e)
        {
            if (vM.CurrentCourse != null)
            {
                if (vM.Settings.AutosaveCourseBeforePublishing)
                    btnSaveCourse_Click(sender, e);

                vM.PublishCurrentCourse();
            }
        }

        private void btnSaveCourse_Click(object sender, RoutedEventArgs e)
        {
            if (vM.CurrentCourse != null && vM.CurrentMathProblem != null)
            {
                vM.CurrentMathProblem.CorrectAnswers = vM.TempAnswers;

                rtbCodeMode.Document.Blocks.Clear();
                rtbCodeMode.Document = FlowDocumentOfCurrentMathProblem();

                var problem = new TextRange(rtbProblemText.Document.ContentStart, rtbProblemText.Document.ContentEnd);
                vM.CurrentMathProblem.ProblemText = problem.Text.Trim();
                var question = new TextRange(rtbQuestion.Document.ContentStart, rtbQuestion.Document.ContentEnd);
                vM.CurrentMathProblem.ProblemQuestion = question.Text.Trim();
                var contents = new TextRange(rtbCodeMode.Document.ContentStart, rtbCodeMode.Document.ContentEnd);

                vM.SaveCurrentCourse();
                vM.SaveCurrentMathProblem(contents);
            }
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
