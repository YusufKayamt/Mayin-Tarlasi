using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MayinTarlasi
{
    public partial class Form1 : Form
    {
        private const int GridSize = 20; // 20x20 grid boyutu
        private const int ButtonSize = 25; // Buton boyutu (25x25 px)
        private const int TotalMines = 40; // Toplam mayın sayısı
        private Button[,] buttons; // Buton matrisi
        private bool[,] mines; // Mayın yerleşim matrisi
        private bool gameOver = false; // Oyun durumu

        public Form1()
        {
            InitializeComponent();
            this.Text = "Mayın Tarlası"; // Form başlığı
            this.Size = new Size(GridSize * ButtonSize + 20, GridSize * ButtonSize + 40); // Form boyutu
            InitializeGrid();
            PlaceMines();
        }

        // Oyun alanını oluşturur
        private void InitializeGrid()
        {
            buttons = new Button[GridSize, GridSize];
            mines = new bool[GridSize, GridSize];

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(ButtonSize, ButtonSize);
                    btn.Location = new Point(x * ButtonSize, y * ButtonSize);
                    btn.Click += Button_Click;
                    btn.Tag = new Point(x, y); // Butona koordinat bilgisini ata
                    this.Controls.Add(btn);
                    buttons[x, y] = btn;
                }
            }
        }

        // Rastgele mayın yerleştirir
        private void PlaceMines()
        {
            Random rand = new Random();
            int placedMines = 0;

            while (placedMines < TotalMines)
            {
                int x = rand.Next(GridSize);
                int y = rand.Next(GridSize);

                if (!mines[x, y]) // Eğer mayın yoksa
                {
                    mines[x, y] = true;
                    placedMines++;
                }
            }
        }

        // Butona tıklandığında çalışır
        private void Button_Click(object sender, EventArgs e)
        {
            if (gameOver) return;

            Button clickedButton = sender as Button;
            Point coordinates = (Point)clickedButton.Tag;

            int x = coordinates.X;
            int y = coordinates.Y;

            if (mines[x, y]) // Eğer mayın varsa
            {
                clickedButton.Text = "💣";
                clickedButton.BackColor = Color.Red;
                GameOver();
            }
            else
            {
                int surroundingMines = CountSurroundingMines(x, y);
                clickedButton.Text = surroundingMines.ToString();
                clickedButton.Enabled = false;

                if (surroundingMines == 0)
                {
                    RevealEmptyCells(x, y);
                }

                CheckWin();
            }
        }

        // Etrafındaki mayınları sayar
        private int CountSurroundingMines(int x, int y)
        {
            int count = 0;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && ny >= 0 && nx < GridSize && ny < GridSize && mines[nx, ny])
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        // Boş hücreleri otomatik açar
        private void RevealEmptyCells(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && ny >= 0 && nx < GridSize && ny < GridSize && buttons[nx, ny].Enabled)
                    {
                        buttons[nx, ny].PerformClick();
                    }
                }
            }
        }

        // Kazanma durumunu kontrol eder
        private void CheckWin()
        {
            foreach (Button btn in buttons)
            {
                if (btn.Enabled && !mines[(Point)btn.Tag.X, (Point)btn.Tag.Y])
                {
                    return; // Açılmamış güvenli hücre var
                }
            }

            MessageBox.Show("Tebrikler, kazandınız!");
            gameOver = true;
        }

        // Oyunu bitirir
        private void GameOver()
        {
            gameOver = true;
            MessageBox.Show("Mayına bastınız! Oyun bitti.");

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    if (mines[x, y])
                    {
                        buttons[x, y].Text = "💣";
                        buttons[x, y].BackColor = Color.Gray;
                    }
                    buttons[x, y].Enabled = false;
                }
            }
        }
    }
}