using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace brick_game_3._0
{
    public class GameBase : Form
    {
        protected bool[] keyStates = new bool[256]; // Все возможные клавиши

        protected override void OnKeyDown(KeyEventArgs e)
        {
            keyStates[(int)e.KeyCode] = true;
            e.Handled = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            keyStates[(int)e.KeyCode] = false;
            e.Handled = true;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return true; // Разрешаем обработку всех клавиш
        }
    }
}
