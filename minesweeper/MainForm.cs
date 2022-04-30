using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minesweeper
{
    public partial class MainForm : Form
    {
        Random myRandomNumberGenerator = new Random();
        Bitmap[] labels = new Bitmap[9];
        Bitmap boom, cover, flag, unkown, win1, win2, lose;
        SoundPlayer start, win, booom;
        PictureBox[,] board;
        int row, col, boomNumber, time, coverNumber;
        int width, windowWidth, windowHeight;

        private void PlayAgain(object sender, EventArgs e)
        {
            start.Play();
            timer2.Enabled = false;
            pictureBox1.Visible = button1.Visible = false;
            ClearBoard();
            InitializeGame();
        }

        private void ClearBoard()
        {
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    Controls.Remove(board[r, c]);
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            pictureBox1.Image = pictureBox1.Image == win1 ? win2 : win1;
        }

        public MainForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            labels[0] = Properties.Resources._0;
            labels[1] = Properties.Resources._1;
            labels[2] = Properties.Resources._2;
            labels[3] = Properties.Resources._3;
            labels[4] = Properties.Resources._4;
            labels[5] = Properties.Resources._5;
            labels[6] = Properties.Resources._6;
            labels[7] = Properties.Resources._7;
            labels[8] = Properties.Resources._8;
            boom = Properties.Resources.boom1;
            cover = Properties.Resources.cover;
            flag = Properties.Resources.flag;
            unkown = Properties.Resources.unkown;
            win1 = Properties.Resources.win1;
            win2 = Properties.Resources.win2;
            lose = Properties.Resources.lose;
            start = new SoundPlayer(Properties.Resources.start);
            win = new SoundPlayer(Properties.Resources.win);
            booom = new SoundPlayer(Properties.Resources.boom2);
            row = 16;
            col = 32;
            board = new PictureBox[row, col];
            boomNumber = row * col / 9;
            windowWidth = System.Windows.Forms.SystemInformation.WorkingArea.Width;
            windowHeight = System.Windows.Forms.SystemInformation.WorkingArea.Height;
            width = windowWidth / (col + 2);
            pictureBox1.Left = (windowWidth - pictureBox1.Width) / 2;
            pictureBox1.Top = (windowHeight - pictureBox1.Height) / 3;
            button1.Left = (windowWidth - button1.Width) / 2;
            button1.Top = (windowHeight - button1.Height) / 3 * 2;
            InitializeGame();
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void InitializeGame()
        {
            label1.Text = "00:00";
            time = 0;
            coverNumber = row * col;
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    board[r, c] = new PictureBox
                    {
                        Width = width,
                        Height = width,
                        Left = width + c * width,
                        Top = 90 + r * width,
                        Image = cover,
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                    board[r, c].MouseDown += Click;
                    Controls.Add(board[r, c]);
                }
            }

            for (int i = 0; i < boomNumber; i++)
            {
                SetBoom();
            }
            Mark();
        }

        private void SetBoom()
        {
            int r = myRandomNumberGenerator.Next(0, row);
            int c = myRandomNumberGenerator.Next(0, col);
            if (board[r, c].Tag != boom)
            {
                board[r, c].Tag = boom;
            } else
            {
                SetBoom();
            }
        }

        private void Mark()
        {
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    int count = 0;
                    if (board[r, c].Tag != boom)
                    {
                        for (int i = r - 1; i <= r + 1; i++)
                        {
                            for (int j = c - 1; j <= c + 1; j++)
                            {
                                if (i >= 0 && i < row && j >=0 && j < col && board[i, j].Tag == boom)
                                {
                                    count++;
                                }
                            }
                        }
                        board[r, c].Tag = labels[count];
                    }
                }
            }
        }

        private void Open(int r, int c)
        {
            if (board[r, c].Tag != boom && board[r, c].Image == cover)
            {
                coverNumber--;
                board[r, c].Enabled = false;
                if (board[r, c].Tag == labels[0])
                {
                    board[r, c].Image = labels[0];
                    for (int i = r - 1; i <= r + 1; i++)
                    {
                        for (int j = c - 1; j <= c + 1; j++)
                        {
                            if (i >= 0 && i < row && j >= 0 && j < col)
                            {
                                Open(i, j);
                            }
                        }
                    }
                }
                else
                {
                    board[r, c].Image = (Bitmap)board[r, c].Tag;
                }
            }
        }

        private new void Click(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled) timer1.Enabled = true;
            PictureBox cell = (PictureBox)sender;

            if (e.Button == MouseButtons.Left)
            {
                if (cell.Image != cover) return;

                if (cell.Tag == boom)
                {
                    // game over
                    booom.Play();
                    timer1.Enabled = false;
                    for (int r = 0; r < row; r++)
                    {
                        for (int c = 0; c < col; c++)
                        {
                            board[r, c].Image = (Bitmap)board[r, c].Tag;
                        }
                    }
                    pictureBox1.Image = lose;
                    pictureBox1.Visible = button1.Visible = true;
                } else if (cell.Tag != labels[0])
                {
                    coverNumber--;
                    cell.Enabled = false;
                    cell.Image = (Bitmap)cell.Tag;
                } else
                {
                    Open((cell.Top - 90) / width, (cell.Left - width) / width);
                }
            } else
            {
                cell.Image = cell.Image == cover ? flag : cell.Image == flag ? unkown : cover;
            }

            if (coverNumber == boomNumber)
            {
                // win
                win.Play();
                timer1.Enabled = false;
                timer2.Enabled = true;
                pictureBox1.Image = win1;
                pictureBox1.Visible = button1.Visible = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time++;
            label1.Text = $"{(time / 60).ToString().PadLeft(2, '0')}:{(time % 60).ToString().PadLeft(2, '0')}";
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
        }
    }
}
