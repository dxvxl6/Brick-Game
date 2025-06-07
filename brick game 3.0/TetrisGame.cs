using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClassicGamesLauncher
{
    public partial class TetrisGame : Form
    {
        // Игровое поле
        private int[,] grid = new int[20, 10];
        private int[,] currentPiece;
        private int[,] nextPiece;
        private int currentPieceX, currentPieceY;
        private Random random = new Random();

        // Управление
        private bool[] keyStates = new bool[256];
        private DateTime lastInputTime;
        private const int RepeatDelay = 30;

        // Игровые параметры
        private int score = 0;
        private int level = 1;
        private int baseSpeed;
        private int currentSpeed;
        private bool isPaused = false;
        private Timer gameTimer;
        private Timer inputTimer;

        public TetrisGame(string difficulty)
        {
            InitializeGame(difficulty);
            InitializeComponents();
        }

        private void InitializeGame(string difficulty)
        {
            switch (difficulty)
            {
                case "Легкий": baseSpeed = 500; break;
                case "Средний": baseSpeed = 300; break;
                case "Сложный": baseSpeed = 150; break;
                default: baseSpeed = 300; break;
            }
            currentSpeed = baseSpeed;

            currentPiece = GetRandomPiece();
            nextPiece = GetRandomPiece();
            SpawnNewPiece();
        }

        private void InitializeComponents()
        {
            this.Text = "Тетрис";
            this.ClientSize = new Size(400, 600);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.BackColor = Color.LightGray;

            gameTimer = new Timer();
            gameTimer.Interval = currentSpeed;
            gameTimer.Tick += GameLoop;

            inputTimer = new Timer();
            inputTimer.Interval = 16;
            inputTimer.Tick += ProcessInput;

            gameTimer.Start();
            inputTimer.Start();
        }

        private void SpawnNewPiece()
        {
            currentPiece = nextPiece;
            nextPiece = GetRandomPiece();
            currentPieceX = 4;
            currentPieceY = 0;

            if (!CanMove(0, 0))
            {
                gameTimer.Stop();
                inputTimer.Stop();
                MessageBox.Show($"Игра окончена! Ваш счет: {score}");
                this.Close();
            }
        }

        private bool CanMove(int offsetX, int offsetY)
        {
            for (int y = 0; y < currentPiece.GetLength(0); y++)
            {
                for (int x = 0; x < currentPiece.GetLength(1); x++)
                {
                    if (currentPiece[y, x] == 1)
                    {
                        int newX = currentPieceX + x + offsetX;
                        int newY = currentPieceY + y + offsetY;

                        if (newX < 0 || newX >= 10 || newY >= 20 || (newY >= 0 && grid[newY, newX] == 1))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void UpdateGame()
        {
            if (CanMove(0, 1))
            {
                currentPieceY++;
            }
            else
            {
                MergePieceToGrid();
                ClearLines();
                SpawnNewPiece();
            }
        }

        private void MergePieceToGrid()
        {
            for (int y = 0; y < currentPiece.GetLength(0); y++)
            {
                for (int x = 0; x < currentPiece.GetLength(1); x++)
                {
                    if (currentPiece[y, x] == 1 && currentPieceY + y >= 0)
                    {
                        grid[currentPieceY + y, currentPieceX + x] = 1;
                    }
                }
            }
        }

        private void ClearLines()
        {
            int linesCleared = 0;

            for (int y = 19; y >= 0; y--)
            {
                bool lineComplete = true;
                for (int x = 0; x < 10; x++)
                {
                    if (grid[y, x] == 0)
                    {
                        lineComplete = false;
                        break;
                    }
                }

                if (lineComplete)
                {
                    linesCleared++;
                    for (int y2 = y; y2 > 0; y2--)
                    {
                        for (int x = 0; x < 10; x++)
                        {
                            grid[y2, x] = grid[y2 - 1, x];
                        }
                    }
                    y++; // Проверяем ту же строку снова
                }
            }

            if (linesCleared > 0)
            {
                score += linesCleared * 100 * level;
                if (score / 2000 > level - 1)
                {
                    level = score / 2000 + 1;
                }
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                UpdateGame();
                this.Invalidate();
            }
        }

        private void ProcessInput(object sender, EventArgs e)
        {
            if (isPaused) return;

            bool moved = false;

            if (keyStates[(int)Keys.Left] && CanMove(-1, 0))
            {
                if ((DateTime.Now - lastInputTime).TotalMilliseconds > RepeatDelay)
                {
                    currentPieceX--;
                    moved = true;
                }
            }

            if (keyStates[(int)Keys.Right] && CanMove(1, 0))
            {
                if ((DateTime.Now - lastInputTime).TotalMilliseconds > RepeatDelay)
                {
                    currentPieceX++;
                    moved = true;
                }
            }

            if (moved)
            {
                lastInputTime = DateTime.Now;
                this.Invalidate();
            }

            gameTimer.Interval = keyStates[(int)Keys.Down] ? baseSpeed / 5 : currentSpeed;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            keyStates[(int)e.KeyCode] = true;

            if ((DateTime.Now - lastInputTime).TotalMilliseconds > RepeatDelay)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left when CanMove(-1, 0):
                        currentPieceX--;
                        lastInputTime = DateTime.Now;
                        break;

                    case Keys.Right when CanMove(1, 0):
                        currentPieceX++;
                        lastInputTime = DateTime.Now;
                        break;
                }
            }

            switch (e.KeyCode)
            {
                case Keys.Up:
                    RotatePiece();
                    break;

                case Keys.Space:
                    HardDrop();
                    break;

                case Keys.P:
                    isPaused = !isPaused;
                    break;

                case Keys.Escape:
                    this.Close();
                    break;
            }

            this.Invalidate();
            e.Handled = true;
        }

        private void RotatePiece()
        {
            int[,] rotated = RotateMatrix(currentPiece);
            if (CanRotate(rotated))
            {
                currentPiece = rotated;
            }
        }

        private void HardDrop()
        {
            while (CanMove(0, 1))
            {
                currentPieceY++;
            }
            MergePieceToGrid();
            ClearLines();
            SpawnNewPiece();
        }

        private int[,] RotateMatrix(int[,] matrix)
        {
            int[,] rotated = new int[matrix.GetLength(1), matrix.GetLength(0)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    rotated[j, matrix.GetLength(0) - 1 - i] = matrix[i, j];
                }
            }
            return rotated;
        }

        private bool CanRotate(int[,] rotatedPiece)
        {
            for (int y = 0; y < rotatedPiece.GetLength(0); y++)
            {
                for (int x = 0; x < rotatedPiece.GetLength(1); x++)
                {
                    if (rotatedPiece[y, x] == 1)
                    {
                        int newX = currentPieceX + x;
                        int newY = currentPieceY + y;

                        if (newX < 0 || newX >= 10 || newY >= 20 || (newY >= 0 && grid[newY, newX] == 1))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            keyStates[(int)e.KeyCode] = false;
            e.Handled = true;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData == Keys.Up || keyData == Keys.Down ||
                   keyData == Keys.Left || keyData == Keys.Right ||
                   base.IsInputKey(keyData);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawGame(e.Graphics);
        }

        private void DrawGame(Graphics g)
        {
            g.FillRectangle(Brushes.White, 0, 0, 300, 600);

            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (grid[y, x] == 1)
                    {
                        g.FillRectangle(Brushes.Blue, x * 30, y * 30, 30, 30);
                        g.DrawRectangle(Pens.Black, x * 30, y * 30, 30, 30);
                    }
                }
            }

            for (int y = 0; y < currentPiece.GetLength(0); y++)
            {
                for (int x = 0; x < currentPiece.GetLength(1); x++)
                {
                    if (currentPiece[y, x] == 1)
                    {
                        g.FillRectangle(Brushes.Red,
                            (currentPieceX + x) * 30,
                            (currentPieceY + y) * 30,
                            30, 30);
                        g.DrawRectangle(Pens.Black,
                            (currentPieceX + x) * 30,
                            (currentPieceY + y) * 30,
                            30, 30);
                    }
                }
            }

            g.DrawString("Следующая:", new Font("Arial", 12), Brushes.Black, 320, 50);
            for (int y = 0; y < nextPiece.GetLength(0); y++)
            {
                for (int x = 0; x < nextPiece.GetLength(1); x++)
                {
                    if (nextPiece[y, x] == 1)
                    {
                        g.FillRectangle(Brushes.Green,
                            320 + x * 30,
                            80 + y * 30,
                            30, 30);
                        g.DrawRectangle(Pens.Black,
                            320 + x * 30,
                            80 + y * 30,
                            30, 30);
                    }
                }
            }

            g.DrawString($"Счет: {score}", new Font("Arial", 12), Brushes.Black, 320, 200);
            g.DrawString($"Уровень: {level}", new Font("Arial", 12), Brushes.Black, 320, 230);

            if (isPaused)
            {
                g.DrawString("ПАУЗА", new Font("Arial", 24, FontStyle.Bold),
                    Brushes.Red, 80, 250);
            }
        }

        private int[,] GetRandomPiece()
        {
            int[][,] pieces = new int[][,]
            {
                new int[,] { {1,1,1,1} }, // I
                new int[,] { {1,1}, {1,1} }, // O
                new int[,] { {0,1,0}, {1,1,1} }, // T
                new int[,] { {1,1,0}, {0,1,1} }, // Z
                new int[,] { {0,1,1}, {1,1,0} } // S
            };
            return pieces[random.Next(pieces.Length)];
        }
    }
}