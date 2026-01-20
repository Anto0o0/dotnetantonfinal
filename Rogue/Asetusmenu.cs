using System;
using ZeroElectric.Vinculum;

namespace Rogue
{
   
    class OptionsMenu
    {
        // Tapahtuma, joka kutsutaan kun käyttäjä painaa takaisin
        public event EventHandler BackButtonPressedEvent;

        
        public void DrawMenu()
        {
            // Aloita piirto
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Painikkeen koko ja sijainti keskelle ruutua
            int width = 120;
            int height = 25;
            int xPos = (Raylib.GetScreenWidth() - width) / 2;
            int yPos = (Raylib.GetScreenHeight() - height) / 2;

            // Näytä valikon otsikko
            RayGui.GuiLabel(new Rectangle(xPos, yPos - 2 * height, width, height), "Peliasetukset");

            // Takaisin-painike
            if (RayGui.GuiButton(new Rectangle(xPos, yPos, width, height), "Takaisin") == 1)
            {
                BackButtonPressedEvent?.Invoke(this, EventArgs.Empty);
            }

            // Lopeta piirto
            Raylib.EndDrawing();
        }
    }
}
