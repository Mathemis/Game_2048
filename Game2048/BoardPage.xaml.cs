using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Game2048
{
    /// <summary>
    /// Interaction logic for BoardPage.xaml
    /// </summary>
    public partial class BoardPage : Page
    {
        public BoardPage()
        {
            InitializeComponent();
            ShowBoard();
        }


        /// <summary>
        /// Trick to file controls in WPF Window by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }



        /// <summary>
        /// Show new board after key event
        /// </summary>
        public void ShowBoard()
        {
            var wnd = (MainWindow)Application.Current.MainWindow;
            var bc = new BrushConverter();
            foreach (Label tb in FindVisualChildren<Label>(BoardShow))
            {
                var r = Grid.GetRow(tb);
                var c = Grid.GetColumn(tb);
                if (wnd.board[r, c] == 0)
                {
                    tb.Content = string.Empty;
                    tb.Background = (Brush)bc.ConvertFrom("#CDC1B3");
                }
                else
                {
                    tb.Content = wnd.board[r, c].ToString();
                    tb.Background = (SolidColorBrush)FindResource("N" + tb.Content);
                    if (wnd.board[r, c] < 8)
                        tb.Foreground = (Brush)bc.ConvertFrom("#776E65");
                    else tb.Foreground = (Brush)bc.ConvertFrom("#FDEBE8");
                }
            }
            wnd.ScoreText.Text = wnd.score.ToString();
            if (wnd.score > wnd.bestScore)
            {
                wnd.bestScore = wnd.score;
                wnd.BestText.Text = wnd.score.ToString();
            }
        }

    }
}
