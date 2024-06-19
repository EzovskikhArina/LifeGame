using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        // Размеры поля
        private const int rows = 50;
        private const int cols = 50;

        // Основной массив для хранения состояния клеток
        private bool[,] cells = new bool[rows, cols];

        // Буферный массив для расчета следующего поколения
        private bool[,] buffer = new bool[rows, cols];

        // Текущий номер поколения
        private int generation = 0;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Устанавливаем размер клиентской области формы
            this.ClientSize = new Size(800, 600);

            // Запрещаем изменение размеров окна
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Включаем двойную буферизацию для уменьшения мерцания
            this.DoubleBuffered = true;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            // Вычисляем размеры каждой клетки
            int cellWidth = this.ClientSize.Width / cols;
            int cellHeight = this.ClientSize.Height / rows;

            // Определяем координаты клетки, на которую нажали
            int x = e.X / cellWidth;
            int y = e.Y / cellHeight;

            // Если координаты в пределах поля, меняем состояние клетки
            if (x >= 0 && x < cols && y >= 0 && y < rows)
            {
                cells[y, x] = !cells[y, x];
                this.Invalidate(); // Перерисовываем форму
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Вычисляем размеры каждой клетки
            int cellWidth = this.ClientSize.Width / cols;
            int cellHeight = this.ClientSize.Height / rows;

            // Проходим по всем клеткам и рисуем их
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (cells[y, x])
                    {
                        // Заполняем живую клетку черным цветом
                        e.Graphics.FillRectangle(Brushes.Black, x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                    }
                    // Рисуем серую сетку
                    e.Graphics.DrawRectangle(Pens.Gray, x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                }
            }
        }

        private void StartMenuItem_Click(object sender, EventArgs e)
        {
            timer.Start(); // Запускаем таймер
        }

        private void ClearMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop(); // Останавливаем таймер
            Array.Clear(cells, 0, cells.Length); // Очищаем массив клеток
            generation = 0; // Сбрасываем счетчик поколений
            generationLabel.Text = "Поколение: 0"; // Обновляем текст в статусной строке
            this.Invalidate(); // Перерисовываем форму
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); // Закрываем приложение
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CalculateNextGeneration(); // Вычисляем следующее поколение
            this.Invalidate(); // Перерисовываем форму
            generation++; // Увеличиваем номер поколения
            generationLabel.Text = $"Поколение: {generation}"; // Обновляем текст в статусной строке
        }

        private void CalculateNextGeneration()
        {
            Array.Clear(buffer, 0, buffer.Length); // Очищаем буферный массив

            // Проходим по всем клеткам
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int liveNeighbors = GetLiveNeighborsCount(x, y); // Считаем количество живых соседей

                    if (cells[y, x])
                    {
                        // Живая клетка остается живой, если у нее 2 или 3 живых соседа
                        if (liveNeighbors < 2 || liveNeighbors > 3)
                        {
                            buffer[y, x] = false; // Иначе умирает
                        }
                        else
                        {
                            buffer[y, x] = true;
                        }
                    }
                    else
                    {
                        // Мертвая клетка становится живой, если у нее ровно 3 живых соседа
                        if (liveNeighbors == 3)
                        {
                            buffer[y, x] = true;
                        }
                    }
                }
            }

            // Меняем местами основной массив и буфер
            var temp = cells;
            cells = buffer;
            buffer = temp;
        }

        private int GetLiveNeighborsCount(int x, int y)
        {
            int count = 0;

            // Проверяем всех 8 соседей
            for (int row = -1; row <= 1; row++)
            {
                for (int col = -1; col <= 1; col++)
                {
                    if (row == 0 && col == 0)
                    {
                        continue; // Пропускаем саму клетку
                    }

                    int newRow = (y + row + rows) % rows; // Обеспечиваем тороидальное поле
                    int newCol = (x + col + cols) % cols; // Обеспечиваем тороидальное поле

                    if (cells[newRow, newCol])
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}