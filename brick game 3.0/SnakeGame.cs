using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClassicGamesLauncher
{
    public partial class SnakeGame : Form
    {
        private List<Point> snake = new List<Point>();
        private Point food;
        private Direction currentDirection = Direction.Right;
        private int cellSize = 20;
        private int score = 0;
        private int speed;
        private Random random = new Random();

        public SnakeGame(string difficulty)
        {
            this.Text = "Змейка";
            this.ClientSize = new Size(400, 400);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            SetDifficulty(difficulty);

            InitializeGame();
            SetupTimer();
        }

        private void SetDifficulty(string difficulty)
        {
            switch (difficulty)
            {
                case "Легкий": speed = 150; break;
                case "Средний": speed = 100; break;
                case "Сложный": speed = 50; break;
                default: speed = 100; break;
            }
        }

        private void InitializeGame()
        {
            snake.Clear();
            snake.Add(new Point(5, 5));
            snake.Add(new Point(4, 5));
            snake.Add(new Point(3, 5));
            SpawnFood();
        }

        private void SetupTimer()
        {
            gameTimer = new Timer();
            gameTimer.Interval = speed;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            MoveSnake();
            CheckCollision();
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawSnake(e.Graphics);
            DrawFood(e.Graphics);
            DrawScore(e.Graphics);
        }

        private void DrawSnake(Graphics g)
        {
            foreach (var segment in snake)
            {
                g.FillRectangle(Brushes.Green, segment.X * cellSize, segment.Y * cellSize, cellSize, cellSize);
                g.DrawRectangle(Pens.Black, segment.X * cellSize, segment.Y * cellSize, cellSize, cellSize);
            }
        }

        private void DrawFood(Graphics g)
        {
            g.FillEllipse(Brushes.Red, food.X * cellSize, food.Y * cellSize, cellSize, cellSize);
        }

        private void DrawScore(Graphics g)
        {
            g.DrawString($"Счет: {score}", new Font("Arial", 12), Brushes.Black, 10, 10);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (currentDirection != Direction.Down) currentDirection = Direction.Up;
                    break;
                case Keys.Down:
                    if (currentDirection != Direction.Up) currentDirection = Direction.Down;
                    break;
                case Keys.Left:
                    if (currentDirection != Direction.Right) currentDirection = Direction.Left;
                    break;
                case Keys.Right:
                    if (currentDirection != Direction.Left) currentDirection = Direction.Right;
                    break;
            }
        }

        private void MoveSnake()
        {
            Point head = snake[0];
            Point newHead = head;

            switch (currentDirection)
            {
                case Direction.Up:
                    newHead.Y--;
                    break;
                case Direction.Down:
                    newHead.Y++;
                    break;
                case Direction.Left:
                    newHead.X--;
                    break;
                case Direction.Right:
                    newHead.X++;
                    break;
            }

            snake.Insert(0, newHead);

            if (newHead == food)
            {
                score += 10;
                SpawnFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        private void SpawnFood()
        {
            int maxX = this.ClientSize.Width / cellSize;
            int maxY = this.ClientSize.Height / cellSize;

            do
            {
                food = new Point(random.Next(0, maxX), random.Next(0, maxY));
            } while (snake.Contains(food));
        }

        private void CheckCollision()
        {
            Point head = snake[0];

            // Проверка столкновения со стенами
            if (head.X < 0 || head.X >= this.ClientSize.Width / cellSize ||
                head.Y < 0 || head.Y >= this.ClientSize.Height / cellSize)
            {
                GameOver();
                return;
            }

            // Проверка столкновения с собой
            for (int i = 1; i < snake.Count; i++)
            {
                if (head == snake[i])
                {
                    GameOver();
                    return;
                }
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show($"Игра окончена! Ваш счет: {score}");
            this.Close();
        }
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}