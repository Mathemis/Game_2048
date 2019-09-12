using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Navigation;


namespace Game2048
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Data members

        public int[,] board = new int[4, 4];
        public int[,] preBoard = new int[4, 4];
        public int preScore;
        public int score;
        public int bestScore;
        bool high;

        #endregion


        #region Sub functions

        /// <summary>
        /// Genertate random position for 2 tile in new game
        /// </summary>
        /// <returns></returns>
        private Tuple<byte, byte> generateNewPos()
        {
            bool occupied = true;
            byte r = 0, c = 0;
            Random rnd = new Random();
            while (occupied)
            {
                r = (byte)(rnd.Next(1000) % 4);
                c = (byte)(rnd.Next(1000) % 4);
                if (board[r, c] == 0)
                    occupied = false;
            }
            return Tuple.Create(r, c);
        }


        /// <summary>
        /// Check if board is full or not
        /// </summary>
        /// <returns></returns>
        private bool isFull()
        {
            for (var i = 0; i < 4; ++i)
                for (var j = 0; j < 4; ++j)
                    if (board[i, j] == 0) return false;
            return true;
        }


        /// <summary>
        /// Check game over: 0: not over, 1: you win, 2: game over
        /// </summary>
        /// <returns></returns>
        private byte IsOver()
        {
            for (var i = 0; i < 4; ++i)
                for (var j = 0; j < 4; ++j)
                    if (board[i, j] == 2048)
                        return 1;
            for (var r = 0; r < 4; ++r)
                for (var i = 0; i < 3; ++i)
                    if (board[r, i] == board[r, i + 1])
                        return 0;
            for (var c = 0; c < 4; ++c)
                for (var i = 0; i < 3; ++i)
                    if (board[i, c] == board[i + 1, c])
                        return 0;
            return 2;
        }


        /// <summary>
        /// Check if preBoard is the same as board or not
        /// </summary>
        /// <returns></returns>
        private bool SameBoard()
        {
            for (var i = 0; i < 4; ++i)
                for (var j = 0; j < 4; ++j)
                    if (preBoard[i, j] != board[i, j])
                        return false;
            return true;
        }

        #endregion


        #region Primary functions

        /// <summary>
        /// Prepare for a new game
        /// </summary>
        public void NewGame()
        {
            score = 0;
            preScore = 0;
            high = false;

            for (var i = 0; i < 4; ++i)
                for (var j = 0; j < 4; ++j)
                    board[i, j] = 0;

            Tuple<byte, byte> pos = generateNewPos();
            Random rnd = new Random();
            var x = rnd.Next(1000) % 20;
            if (x != 4) x = 2;
            board[pos.Item1, pos.Item2] = x;
            pos = generateNewPos();
            x = rnd.Next(1000) % 20;
            if (x != 4) x = 2;
            board[pos.Item1, pos.Item2] = x;

            for (var i = 0; i < 4; ++i)
                for (var j = 0; j < 4; ++j)
                    preBoard[i, j] = board[i, j];
        }


        /// <summary>
        /// Save best score if close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // Write best score to file
            using (TextWriter sr = File.CreateText("BestScore.txt"))
            {
                sr.WriteLine(bestScore.ToString());
                sr.Close();
            }
        }


        /// <summary>
        /// Handle arrow keys pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_OnKeyDown(object sender, KeyEventArgs e)
        {
            // Save to the preBoard before making any change
            for (var i = 0; i < 4; ++i)
                for (var j = 0; j < 4; ++j)
                    preBoard[i, j] = board[i, j];
            preScore = score;

            switch (e.Key)
            {
                case Key.Up:
                    {
                        List<int> tmp = new List<int>();
                        for (var i = 0; i < 4; ++i)
                        {
                            tmp.Clear();
                            for (var j = 0; j < 4; ++j) if (board[j, i] != 0)
                                {
                                    tmp.Add(board[j, i]);
                                    board[j, i] = 0;
                                }
                            if (tmp.Count == 0) continue;
                            if (tmp.Count == 1)
                            {
                                board[0, i] = tmp[0];
                                continue;
                            }
                            for (var j = 0; j < tmp.Count - 1; ++j) if (tmp[j] == tmp[j + 1])
                                {
                                    tmp[j] *= 2;
                                    score += tmp[j];
                                    tmp.RemoveAt(j + 1);
                                }
                            for (var j = 0; j < tmp.Count; ++j)
                                board[j, i] = tmp[j];
                        }
                        break;
                    }
                case Key.Down:
                    {
                        List<int> tmp = new List<int>();
                        for (var i = 0; i < 4; ++i)
                        {
                            tmp.Clear();
                            for (int j = 3; j >= 0; --j) if (board[j, i] != 0)
                                {
                                    tmp.Add(board[j, i]);
                                    board[j, i] = 0;
                                }
                            if (tmp.Count == 0) continue;
                            if (tmp.Count == 1)
                            {
                                board[3, i] = tmp[0];
                                continue;
                            }
                            for (var j = 0; j < tmp.Count - 1; ++j) if (tmp[j] == tmp[j + 1])
                                {
                                    tmp[j] *= 2;
                                    score += tmp[j];
                                    tmp.RemoveAt(j + 1);
                                }
                            for (var j = 0; j < tmp.Count; ++j)
                                board[3 - j, i] = tmp[j];
                        }
                        break;
                    }
                case Key.Left:
                    {
                        List<int> tmp = new List<int>();
                        for (var i = 0; i < 4; ++i)
                        {
                            tmp.Clear();
                            for (var j = 0; j < 4; ++j) if (board[i, j] != 0)
                                {
                                    tmp.Add(board[i, j]);
                                    board[i, j] = 0;
                                }
                            if (tmp.Count == 0) continue;
                            if (tmp.Count == 1)
                            {
                                board[i, 0] = tmp[0];
                                continue;
                            }
                            for (var j = 0; j < tmp.Count - 1; ++j) if (tmp[j] == tmp[j + 1])
                                {
                                    tmp[j] *= 2;
                                    score += tmp[j];
                                    tmp.RemoveAt(j + 1);
                                }
                            for (var j = 0; j < tmp.Count; ++j)
                                board[i, j] = tmp[j];
                        }
                        break;
                    }
                case Key.Right:
                    {
                        List<int> tmp = new List<int>();
                        for (var i = 0; i < 4; ++i)
                        {
                            tmp.Clear();
                            for (int j = 3; j >= 0; --j) if (board[i, j] != 0)
                                {
                                    tmp.Add(board[i, j]);
                                    board[i, j] = 0;
                                }
                            if (tmp.Count == 0) continue;
                            if (tmp.Count == 1)
                            {
                                board[i, 3] = tmp[0];
                                continue;
                            }
                            for (var j = 0; j < tmp.Count - 1; ++j) if (tmp[j] == tmp[j + 1])
                                {
                                    tmp[j] *= 2;
                                    score += tmp[j];
                                    tmp.RemoveAt(j + 1);
                                }
                            for (var j = 0; j < tmp.Count; ++j)
                                board[i, 3 - j] = tmp[j];
                        }
                        break;
                    }
            }

            byte c = IsOver();
            if (isFull() && c != 0)
            {
                ShowZone.Content = new OverPage();
                NewGame();
            }
            else if (c == 1 && high == false)
            {
                ShowZone.Content = new WinPage();
                high = true;
            }
            else if (!SameBoard())
            {
                Tuple<byte, byte> pos = generateNewPos();
                Random rnd = new Random();
                var x = rnd.Next(1000) % 20;
                if (x != 4) x = 2;
                board[pos.Item1, pos.Item2] = x;
                ShowZone.Content = new BoardPage();
            }
        }


        /// <summary>
        /// Handle the button event
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The events of the click</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var button = (Button)sender;
            var col = Grid.GetColumn(button);
            if (col == 0) NewGame();
            else 
            {
                for (var i = 0; i < 4; ++i)
                    for (var j = 0; j < 4; ++j)
                        board[i, j] = preBoard[i, j];
                score = preScore;
            }
            ShowZone.Content = new BoardPage();
        }

        #endregion


        public MainWindow()
        {
            InitializeComponent();
            bestScore = 0;
            // Read best score from file
            try
            {
                using (TextReader inFile = File.OpenText("BestScore.txt"))
                {
                    if (inFile != null)
                        bestScore = int.Parse(inFile.ReadLine());
                }
            }
            catch (FileNotFoundException) { }

            BestText.Text = bestScore.ToString();
            NewGame();
            ShowZone.Content = new BoardPage();
            this.KeyDown += new KeyEventHandler(Window_OnKeyDown);
            this.Closed += new EventHandler(MainWindow_Closed);
        }
    }
}
