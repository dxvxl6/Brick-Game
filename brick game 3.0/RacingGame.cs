using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClassicGamesLauncher
{
    public partial class RacingGame : Form
    {
        private int playerCarX = 200;
        private int playerCarY = 400;
        private List<Point> obstacles = new List<Point>();
        private Random random = new Random();
        private int score = 0;
        private int speed;
        private int obstacleSpeed;
        private int obstacleFrequency;

        public RacingGame(string difficulty)
        {
            this.Text = "Гонки";
            this.ClientSize = new Size(400, 500);
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
                case "Легкий":
                    speed = 5;
                    obstacleSpeed = 5;
                    obstacleFrequency = 20;
                    break;
                case "Средний":
                    speed = 7;
                    obstacleSpeed = 7;
                    obstacleFrequency = 15;
                    break;
                case "Сложный":
                    speed = 10;
                    obstacleSpeed = 10;
                    obstacleFrequency = 10;
                    break;
                default:
                    speed = 7;
                    obstacleSpeed = 7;
                    obstacleFrequency = 15;
                    break;
            }
        }

        private void InitializeGame()
        {
            // Начальные препятствия
            for (int i = 0; i < 3; i++)
            {
                SpawnObstacle();
            }
        }

        private void SetupTimer()
        {
            gameTimer = new Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            MoveObstacles();
            SpawnNewObstacles();
            CheckCollisions();
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawRoad(e.Graphics);
            DrawPlayerCar(e.Graphics);
            DrawObstacles(e.Graphics);
            DrawScore(e.Graphics);
        }

        private void DrawRoad(Graphics g)
        {
            // Дорога
            g.FillRectangle(Brushes.Gray, 100, 0, 200, this.ClientSize.Height);

            // Разметка
            for (int i = 0; i < this.ClientSize.Height; i += 40)
            {
                g.FillRectangle(Brushes.White, 195, i, 10, 20);
            }

            // Боковые обочины
            g.FillRectangle(Brushes.Green, 0, 0, 100, this.ClientSize.Height);
            g.FillRectangle(Brushes.Green, 300, 0, 100, this.ClientSize.Height);
        }

        private void DrawPlayerCar(Graphics g)
        {
            g.FillRectangle(Brushes.Red, playerCarX - 15, playerCarY - 30, 30, 50);
            g.FillRectangle(Brushes.DarkRed, playerCarX - 10, playerCarY - 35, 20, 10);
        }

        private void DrawObstacles(Graphics g)
        {
            foreach (var obstacle in obstacles)
            {
                g.FillRectangle(Brushes.Blue, obstacle.X - 20, obstacle.Y - 25, 40, 50);
            }
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
                case Keys.Left:
                    if (playerCarX > 120) playerCarX -= speed;
                    break;
                case Keys.Right:
                    if (playerCarX < 280) playerCarX += speed;
                    break;
            }
        }

        private void MoveObstacles()
        {
            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                obstacles[i] = new Point(obstacles[i].X, obstacles[i].Y + obstacleSpeed);

                // Удаление препятствий, уехавших за экран
                if (obstacles[i].Y > this.ClientSize.Height + 50)
                {
                    obstacles.RemoveAt(i);
                    score += 10;
                }
            }
        }

        private void SpawnNewObstacles()
        {
            if (random.Next(obstacleFrequency) == 0)
            {
                SpawnObstacle();
            }
        }

        private void SpawnObstacle()
        {
            int x = random.Next(120, 280);
            obstacles.Add(new Point(x, -50));
        }

        private void CheckCollisions()
        {
            foreach (var obstacle in obstacles)
            {
                if (Math.Abs(obstacle.X - playerCarX) < 25 && Math.Abs(obstacle.Y - playerCarY) < 40)
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

        private void RacingGame_Load(object sender, EventArgs e)
        {

        }
    }
}