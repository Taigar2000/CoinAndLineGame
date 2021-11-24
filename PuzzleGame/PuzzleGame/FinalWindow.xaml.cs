using System.Windows.Controls;
using DataAnalysis;


namespace PuzzleGame
{
    /// <summary>
    /// Логика взаимодействия для FinalWindow.xaml
    /// </summary>
    public partial class FinalWindow : ContentControl
    {
        public FinalWindow()
        {
            InitializeComponent();
            DataAnalysis.DataWriter.AnalizeAndWrite();
        }
    }
}
