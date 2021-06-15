using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // for ObservableCollection
using System.ComponentModel; // for INotifyPropertyChanged
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMMathProblemsBase.Model;
using MVVMMathProblemsBase.ViewModel.Commands;
using MVVMMathProblemsBase.ViewModel.ValueConverters;

namespace MVVMMathProblemsBase.ViewModel
{
    public class MathProblemVM : INotifyPropertyChanged
    {
        private string userAnswer;

        public string UserAnswer
        {
            get { return userAnswer; }
            set
            {
                userAnswer = value;
                OnPropertyChanged("UserAnswer");
            }
        }

        private MathProblem currentMathProblem;

        public MathProblem CurrentMathProblem
        {
            get { return currentMathProblem; }
            set
            { 
                currentMathProblem = value;
                OnPropertyChanged("CurrentMathProblem");
            }
        }
        private ObservableCollection<SolutionStep> visibleSteps;

        public ObservableCollection<SolutionStep> VisibleSteps
        {
            get { return visibleSteps; }
            set
            { 
                visibleSteps = value;
                OnPropertyChanged("VisibleSteps");
            }
        }

        private bool? solved;

        public bool? Solved
        {
            get { return solved; }
            set
            {
                solved = value;
                OnPropertyChanged("Solved");
            }
        }
        public MakeStepVisibleCommand ChangeStepVisibilityCommand { get; set; }
        public CheckIfAnswerIsCorrectCommand CheckIfAnswerIsCorrectCommand { get; set; }
        public MathProblemVM()
        {
            bool isInDesignMode = DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
/*            if (isInDesignMode)
            {*/
                CurrentMathProblem = new MathProblem("Adélka má 5 nevlastních sourozenců. Její nevlastní sestra Míša má 2 vlastní bratry, " +
                        "1 nevlastní sestru a 2 přiženěné bratry. Míšin přiženěný bratra Jakub má 1 vlastního bratra, 1 nevlastní sestru a " +
                        "3 přiženěné sourozence.",
                    "Kolik nejméně lidí bude na Adélčině oslavě narozenin, kde s ní budou oba její rodiče a všichni její nevlastní sourozenci?",
                    new List<string> { "8" },
                    new List<SolutionStep> {
                        new SolutionStep("Přečti si první větu.",
                            BoolToVisibilityConverter.Convert(isInDesignMode)),
                        new SolutionStep("Znovu si přečti první větu.",
                            BoolToVisibilityConverter.Convert(isInDesignMode)),
                        new SolutionStep("Adélka (první účastník oslavy) má 5 nevlastních sourozenců, takže už z první věty máme 6 lidí.",
                            BoolToVisibilityConverter.Convert(isInDesignMode)),
                        new SolutionStep("Na oslavě budou i oba její rodiče, takže je musíme přičíst.",
                            BoolToVisibilityConverter.Convert(isInDesignMode)),
                        new SolutionStep("Adélka + 5 nevlastních sourozenců + 2 rodiče = 8 lidí na oslavě." ,
                            BoolToVisibilityConverter.Convert(isInDesignMode))});

            //            }
            Solved = null;
            VisibleSteps = new ObservableCollection<SolutionStep>();
            ChangeStepVisibilityCommand = new MakeStepVisibleCommand(this);
            CheckIfAnswerIsCorrectCommand = new CheckIfAnswerIsCorrectCommand(this);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsAnswerCorrect()
        {
            Solved = CurrentMathProblem.CorrectAnswers.Contains(UserAnswer);
            return Solved == true;
        }
    }
}
