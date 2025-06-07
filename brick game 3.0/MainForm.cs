using System;
using System.Windows.Forms;

namespace ClassicGamesLauncher
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            string selectedGame = cmbGame.SelectedItem?.ToString();
            string difficulty = cmbDifficulty.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedGame) || string.IsNullOrEmpty(difficulty))
            {
                MessageBox.Show("Пожалуйста, выберите игру и уровень сложности!");
                return;
            }

            Form gameForm = null;

            switch (selectedGame)
            {
                case "Тетрис":
                    gameForm = new TetrisGame(difficulty);
                    break;
                case "Змейка":
                    gameForm = new SnakeGame(difficulty);
                    break;
                case "Арканоид":
                    gameForm = new ArkanoidGame(difficulty);
                    break;
                case "Танчики":
                    gameForm = new TanksGame(difficulty);
                    break;
                case "Гонки":
                    gameForm = new RacingGame(difficulty);
                    break;
            }

            if (gameForm != null)
            {
                this.Hide();
                gameForm.FormClosed += (s, args) => this.Show();
                gameForm.Show();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            cmbGame.Items.AddRange(new object[] { "Тетрис", "Змейка", "Арканоид", "Танчики", "Гонки" });
            cmbDifficulty.Items.AddRange(new object[] { "Легкий", "Средний", "Сложный" });
        }
    }
}