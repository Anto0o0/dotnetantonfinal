using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroElectric.Vinculum;

namespace Rogue
{
    /// <summary>
    /// Asetusvalikko, joka näyttää "Go Back" -napin.
    /// Käyttää RayGui-kirjastoa käyttöliittymän piirtämiseen.
    /// </summary>
    class OptionsMenu
    {
        /// <summary>
        /// Tapahtuma, joka aktivoituu kun käyttäjä painaa "Go Back" -painiketta.
        /// </summary>
        public event EventHandler BackButtonPressedEvent;

        /// <summary>
        /// Piirtää asetusvalikon ruudulle ja käsittelee napin painalluksen.
        /// </summary>
        public void DrawMenu()
        {
            // Tyhjennä ruutu ja aloita piirtäminen
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Napin koko ja sijainti
            int button_width = 100;
            int button_height = 20;
            int button_x = Raylib.GetScreenWidth() / 2 - button_width / 2;
            int button_y = Raylib.GetScreenHeight() / 2 - button_height / 2;

            // Piirrä valikon otsikko
            RayGui.GuiLabel(new Rectangle(button_x, button_y - button_height * 2, button_width, button_height), "Rogue");

            // "Go Back" -painike
            if (RayGui.GuiButton(new Rectangle(button_x, button_y, button_width, button_height), "Go Back") == 1)
            {
                BackButtonPressedEvent?.Invoke(this, EventArgs.Empty);
            }

            // Lopeta piirtäminen
            Raylib.EndDrawing();
        }
    }
}
