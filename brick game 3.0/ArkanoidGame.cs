using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClassicGamesLauncher
{
    public partial class ArkanoidGame : Form
    {
        private int paddleX;
        private int paddleWidth = 80;
        private int ballX, ballY;
        private int ballSize = 15;
        private int ballSpeedX, ballSpeedY;
        private int[,] bricks;
        private int brickWidth = 50;
        private int brickHeight = 20;
        private int score = 0;
        private int speed;
        private Random random = new Random();

        public ArkanoidGame(string difficulty)
        {
            this.Text = "Арканоид";
            this.ClientSize = new Size(500, 600);
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
                case "Легкий": speed = 5; break;
                case "Средний": speed = 8; break;
                case "Сложный": speed = 12; break;
                default: speed = 8; break;
            }
        }

        private void InitializeGame()
        {
            paddleX = this.ClientSize.Width / 2 - paddleWidth / 2;
            ballX = this.ClientSize.Width / 2;
            ballY = this.ClientSize.Height - 100;
            ballSpeedX = speed;
            ballSpeedY = -speed;

            InitializeBricks();
        }

        private void InitializeBricks()
        {
            bricks = new int[5, 8];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bricks[i, j] = 1;
                }
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
            MoveBall();
            CheckCollisions();
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawPaddle(e.Graphics);
            DrawBall(e.Graphics);
            DrawBricks(e.Graphics);
            DrawScore(e.Graphics);
        }

        private void DrawPaddle(Graphics g)
        {
            g.FillRectangle(Brushes.Blue, paddleX, this.ClientSize.Height - 30, paddleWidth, 15);
        }

        private void DrawBall(Graphics g)
        {
            g.FillEllipse(Brushes.Red, ballX, ballY, ballSize, ballSize);
        }

        private void DrawBricks(Graphics g)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (bricks[i, j] == 1)
                    {
                        Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                        g.FillRectangle(new SolidBrush(color), j * (brickWidth + 5) + 30, i * (brickHeight + 5) + 30, brickWidth, brickHeight);
                    }
                }
            }
        }

        private void DrawScore(Graphics g)
        {
            g.DrawString($"Счет: {score}", new Font("Arial", 12), Brushes.Black, 10, 10);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Left && paddleX > 0)
            {
                paddleX -= 20;
            }
            else if (e.KeyCode == Keys.Right && paddleX < this.ClientSize.Width - paddleWidth)
            {
                paddleX += 20;
            }
        }

        private void MoveBall()
        {
            ballX += ballSpeedX;
            ballY += ballSpeedY;
        }

        private void CheckCollisions()
        {
            // Столкновение со стенами
            if (ballX <= 0 || ballX >= this.ClientSize.Width - ballSize)
            {
                ballSpeedX = -ballSpeedX;
            }

            if (ballY <= 0)
            {
                ballSpeedY = -ballSpeedY;
            }

            // Столкновение с платформой
            if (ballY + ballSize >= this.ClientSize.Height - 30 &&
                ballY + ballSize <= this.ClientSize.Height - 15 &&
                ballX + ballSize >= paddleX &&
                ballX <= paddleX + paddleWidth)
            {
                ballSpeedY = -ballSpeedY;
            }

            // Столкновение с кирпичами
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (bricks[i, j] == 1)
                    {
                        int brickX = j * (brickWidth + 5) + 30;
                        int brickY = i * (brickHeight + 5) + 30;

                        if (ballX + ballSize >= brickX &&
                            ballX <= brickX + brickWidth &&
                            ballY + ballSize >= brickY &&
                            ballY <= brickY + brickHeight)
                        {
                            bricks[i, j] = 0;
                            ballSpeedY = -ballSpeedY;
                            score += 10;
                        }
                    }
                }
            }

            // Проверка на проигрыш
            if (ballY >= this.ClientSize.Height)
            {
                GameOver();
                return;
            }

            // Проверка на выигрыш
            bool allBricksDestroyed = true;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (bricks[i, j] == 1)
                    {
                        allBricksDestroyed = false;
                        break;
                    }
                }
                if (!allBricksDestroyed) break;
            }

            if (allBricksDestroyed)
            {
                GameWon();
            }
        }

        private void ArkanoidGame_Load(object sender, EventArgs e)
        {

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
            MessageBox.Show($"Поздравляем! Вы выиграли! Ваш счет: {score}");
            this.Close();
        }
    }

}