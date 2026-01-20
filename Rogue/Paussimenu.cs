using System;
using ZeroElectric.Vinculum;

namespace Rogue
{
    
    
   
    class Paussimenu
    {
      
        
   
        public event EventHandler<Peli.GameState> BackButtonPressedEvent;

    
        /// Piirtää pause-valikon ja käsittelee käyttäjän painikkeet.
    
        public void DrawMenu()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Perusmitat
            int w = 220;
            int h = 45;

            // Keskitys näytölle
            int cx = Raylib.GetScreenWidth() / 2 - w / 2;
            int cy = Raylib.GetScreenHeight() / 2 - h / 2;

            // Otsikko
            Rectangle labelBox = new(cx, cy - h * 3, w, h);
            RayGui.GuiLabel(labelBox, "Peli pausella");

            // Paluu peliin
            Rectangle backBtn = new(cx, cy, w, h);
            if (RayGui.GuiButton(backBtn, "Jatka") == 1)
            {
                BackButtonPressedEvent?.Invoke(this, Peli.GameState.GameLoop);
            }

            // Päävalikkoon
            Rectangle mainBtn = new(cx, cy + h + 10, w, h);
            if (RayGui.GuiButton(mainBtn, "Päävalikko") == 1)
            {
                BackButtonPressedEvent?.Invoke(this, Peli.GameState.MainMenu);
            }

            Raylib.EndDrawing();
        }
    }
}
