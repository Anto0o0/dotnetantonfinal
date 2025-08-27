using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroElectric.Vinculum;

namespace Rogue
{
    /// <summary>
    /// Pause-valikko, joka näyttää kaksi painiketta: "Go Back" ja "Main Menu".
    /// Käyttää RayGui-kirjastoa käyttöliittymän piirtämiseen.
    /// </summary>
    class PauseMenu
    {
        /// <summary>
        /// Tapahtuma, joka aktivoituu kun jompikumpi painikkeista painetaan.
        /// Parametrina annetaan uusi pelitila.
        /// </summary>
        public event EventHandler<Game.GameState> BackButtonPressedEvent;

        /// <summary>
        /// Piirtää pause-valikon ruudulle ja käsittelee napin painallukset.
        /// </summary>
        public void DrawMenu()
        {
            // Tyhjennä ruutu ja aloita piirtäminen
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Laske napin sijainti ruudulla
            int button_width = 200;
            int button_height = 40;
            int button_x = Raylib.GetScreenWidth() / 2 - button_width / 2;
            int button_y = Raylib.GetScreenHeight() / 2 - button_height / 2;

            // Piirrä otsikko
            RayGui.GuiLabel(new Rectangle(button_x, button_y - button_height * 2, button_width, button_height), "Rogue");

            // "Go Back" -painike
            if (RayGui.GuiButton(new Rectangle(button_x, button_y, button_width, button_height), "Go Back") == 1)
            {
                BackButtonPressedEvent?.Invoke(this, Game.GameState.GameLoop);
            }

            button_y += button_height;

            // "Main menu" -painike
            if (RayGui.GuiButton(new Rectangle(button_x, button_y, button_width, button_height), "Main menu") == 1)
            {
                BackButtonPressedEvent?.Invoke(this, Game.GameState.MainMenu);
            }

            // Lopeta piirtäminen
            Raylib.EndDrawing();
        }
    }
}
