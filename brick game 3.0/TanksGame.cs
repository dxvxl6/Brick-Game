using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClassicGamesLauncher
{
    public partial class TanksGame : Form
    {
        private Point playerTank = new Point(100, 300);
        private Direction playerDirection = Direction.Up;
        private List<Point> enemyTanks = new List<Point>();
        private List<Direction> enemyDirections = new List<Direction>();
        private List<Point> playerBullets = new List<Point>();
        private List<Point> enemyBullets = new List<Point>();
        private Random random = new Random();
        private int score = 0;
        private int enemyCount;
        private int enemySpeed;

        public TanksGame(string difficulty)
        {
            this.Text = "Танчики";
            this.ClientSize = new Size(500, 500);
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
                case "Легкий": enemyCount = 3; enemySpeed = 5; break;
                case "Средний": enemyCount = 5; enemySpeed = 7; break;
                case "Сложный": enemyCount = 8; enemySpeed = 10; break;
                default: enemyCount = 5; enemySpeed = 7; break;
            }
        }

        private void InitializeGame()
        {
            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemyTank();
            }
        }

        private void SpawnEnemyTank()
        {
            int x = random.Next(50, this.ClientSize.Width - 50);
            int y = random.Next(50, this.ClientSize.Height / 2);
            enemyTanks.Add(new Point(x, y));
            enemyDirections.Add((Direction)random.Next(4));
        }

        private void SetupTimer()
        {
            gameTimer = new Timer();
            gameTimer.Interval = 50;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            MoveEnemies();
            MoveBullets();
            CheckCollisions();
            EnemyAI();
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawPlayerTank(e.Graphics);
            DrawEnemyTanks(e.Graphics);
            DrawBullets(e.Graphics);
            DrawScore(e.Graphics);
        }

        private void DrawPlayerTank(Graphics g)
        {
            Brush tankBrush = Brushes.Green;
            int tankSize = 30;

            g.FillRectangle(tankBrush, playerTank.X - tankSize / 2, playerTank.Y - tankSize / 2, tankSize, tankSize);

            // Рисуем дуло танка
            switch (playerDirection)
            {
                case Direction.Up:
                    g.FillRectangle(Brushes.DarkGreen, playerTank.X - 5, playerTank.Y - tankSize / 2 - 15, 10, 15);
                    break;
                case Direction.Down:
                    g.FillRectangle(Brushes.DarkGreen, playerTank.X - 5, playerTank.Y + tankSize / 2, 10, 15);
                    break;
                case Direction.Left:
                    g.FillRectangle(Brushes.DarkGreen, playerTank.X - tankSize / 2 - 15, playerTank.Y - 5, 15, 10);
                    break;
                case Direction.Right:
                    g.FillRectangle(Brushes.DarkGreen, playerTank.X + tankSize / 2, playerTank.Y - 5, 15, 10);
                    break;
            }
        }

        private void DrawEnemyTanks(Graphics g)
        {
            Brush tankBrush = Brushes.Red;
            int tankSize = 30;

            for (int i = 0; i < enemyTanks.Count; i++)
            {
                Point tank = enemyTanks[i];
                g.FillRectangle(tankBrush, tank.X - tankSize / 2, tank.Y - tankSize / 2, tankSize, tankSize);

                // Рисуем дуло танка
                switch (enemyDirections[i])
                {
                    case Direction.Up:
                        g.FillRectangle(Brushes.DarkRed, tank.X - 5, tank.Y - tankSize / 2 - 15, 10, 15);
                        break;
                    case Direction.Down:
                        g.FillRectangle(Brushes.DarkRed, tank.X - 5, tank.Y + tankSize / 2, 10, 15);
                        break;
                    case Direction.Left:
                        g.FillRectangle(Brushes.DarkRed, tank.X - tankSize / 2 - 15, tank.Y - 5, 15, 10);
                        break;
                    case Direction.Right:
                        g.FillRectangle(Brushes.DarkRed, tank.X + tankSize / 2, tank.Y - 5, 15, 10);
                        break;
                }
            }
        }

        private void DrawBullets(Graphics g)
        {
            int bulletSize = 6;

            foreach (var bullet in playerBullets)
            {
                g.FillEllipse(Brushes.Blue, bullet.X - bulletSize / 2, bullet.Y - bulletSize / 2, bulletSize, bulletSize);
            }

            foreach (var bullet in enemyBullets)
            {
                g.FillEllipse(Brushes.Red, bullet.X - bulletSize / 2, bullet.Y - bulletSize / 2, bulletSize, bulletSize);
            }
        }

        private void DrawScore(Graphics g)
        {
            g.DrawString($"Счет: {score}", new Font("Arial", 12), Brushes.Black, 10, 10);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            int moveStep = 5;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    playerDirection = Direction.Up;
                    if (playerTank.Y - moveStep > 30) playerTank.Y -= moveStep;
                    break;
                case Keys.Down:
                    playerDirection = Direction.Down;
                    if (playerTank.Y + moveStep < this.ClientSize.Height - 30) playerTank.Y += moveStep;
                    break;
                case Keys.Left:
                    playerDirection = Direction.Left;
                    if (playerTank.X - moveStep > 30) playerTank.X -= moveStep;
                    break;
                case Keys.Right:
                    playerDirection = Direction.Right;
                    if (playerTank.X + moveStep < this.ClientSize.Width - 30) playerTank.X += moveStep;
                    break;
                case Keys.Space:
                    ShootPlayer();
                    break;
            }
        }

        private void ShootPlayer()
        {
            Point bullet = new Point(playerTank.X, playerTank.Y);
            playerBullets.Add(bullet);
        }

        private void MoveEnemies()
        {
            for (int i = 0; i < enemyTanks.Count; i++)
            {
                Point tank = enemyTanks[i];
                Direction dir = enemyDirections[i];

                switch (dir)
                {
                    case Direction.Up:
                        enemyTanks[i] = new Point(tank.X, tank.Y - enemySpeed);
                        break;
                    case Direction.Down:
                        enemyTanks[i] = new Point(tank.X, tank.Y + enemySpeed);
                        break;
                    case Direction.Left:
                        enemyTanks[i] = new Point(tank.X - enemySpeed, tank.Y);
                        break;
                    case Direction.Right:
                        enemyTanks[i] = new Point(tank.X + enemySpeed, tank.Y);
                        break;
                }

                // Проверка на выход за границы
                if (enemyTanks[i].X < 30 || enemyTanks[i].X > this.ClientSize.Width - 30 ||
                    enemyTanks[i].Y < 30 || enemyTanks[i].Y > this.ClientSize.Height - 30)
                {
                    enemyDirections[i] = (Direction)random.Next(4);
                    enemyTanks[i] = tank; // Возвращаем на предыдущую позицию
                }
            }
        }

        private void MoveBullets()
        {
            // Движение пуль игрока
            for (int i = playerBullets.Count - 1; i >= 0; i--)
            {
                Point bullet = playerBullets[i];
                int newX = bullet.X;
                int newY = bullet.Y;

                switch (playerDirection)
                {
                    case Direction.Up:
                        newY -= 10;
                        break;
                    case Direction.Down:
                        newY += 10;
                        break;
                    case Direction.Left:
                        newX -= 10;
                        break;
                    case Direction.Right:
                        newX += 10;
                        break;
                }

                playerBullets[i] = new Point(newX, newY);

                // Удаление пуль за пределами экрана
                if (newX < 0 || newX > this.ClientSize.Width || newY < 0 || newY > this.ClientSize.Height)
                {
                    playerBullets.RemoveAt(i);
                }
            }

            // Движение пуль врагов
            for (int i = enemyBullets.Count - 1; i >= 0; i--)
            {
                Point bullet = enemyBullets[i];
                int newX = bullet.X;
                int newY = bullet.Y;

                switch (enemyDirections[i % enemyDirections.Count])
                {
                    case Direction.Up:
                        newY -= 8;
                        break;
                    case Direction.Down:
                        newY += 8;
                        break;
                    case Direction.Left:
                        newX -= 8;
                        break;
                    case Direction.Right:
                        newX += 8;
                        break;
                }

                enemyBullets[i] = new Point(newX, newY);

                // Удаление пуль за пределами экрана
                if (newX < 0 || newX > this.ClientSize.Width || newY < 0 || newY > this.ClientSize.Height)
                {
                    enemyBullets.RemoveAt(i);
                }
            }
        }

        private void CheckCollisions()
        {
            // Проверка попадания пуль игрока во врагов
            for (int i = playerBullets.Count - 1; i >= 0; i--)
            {
                Point bullet = playerBullets[i];
                for (int j = enemyTanks.Count - 1; j >= 0; j--)
                {
                    Point enemy = enemyTanks[j];
                    if (Math.Abs(bullet.X - enemy.X) < 20 && Math.Abs(bullet.Y - enemy.Y) < 20)
                    {
                        playerBullets.RemoveAt(i);
                        enemyTanks.RemoveAt(j);
                        enemyDirections.RemoveAt(j);
                        score += 100;
                        break;
                    }
                }
            }

            // Проверка попадания пуль врагов в игрока
            for (int i = enemyBullets.Count - 1; i >= 0; i--)
            {
                Point bullet = enemyBullets[i];
                if (Math.Abs(bullet.X - playerTank.X) < 20 && Math.Abs(bullet.Y - playerTank.Y) < 20)
                {
                    GameOver();
                    return;
                }
            }

            // Проверка столкновения игрока с врагами
            foreach (var enemy in enemyTanks)
            {
                if (Math.Abs(enemy.X - playerTank.X) < 30 && Math.Abs(enemy.Y - playerTank.Y) < 30)
                {
                    GameOver();
                    return;
                }
            }

            // Проверка на победу
            if (enemyTanks.Count == 0)
            {
                GameWon();
            }
        }

        private void EnemyAI()
        {
            // Враги стреляют случайным образом
            if (random.Next(100) < 2 && enemyTanks.Count > 0)
            {
                int tankIndex = random.Next(enemyTanks.Count);
                Point bullet = new Point(enemyTanks[tankIndex].X, enemyTanks[tankIndex].Y);
                enemyBullets.Add(bullet);
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show($"Игра окончена! Ваш счет: {score}");
            this.Close();
        }

        private void GameWon()
        {
            gameTimer.Stop();
            MessageBox.Show($"Поздравляем! Вы победили! Ваш счет: {score}");
            this.Close();
        }
    }
}