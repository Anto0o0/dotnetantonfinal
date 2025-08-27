using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ZeroElectric.Vinculum;
using static System.Net.Mime.MediaTypeNames;

namespace Rogue
{
    internal class Item
    {
        public string name;                 // Esineen nimi (esim. "chest", "axe")
        public Vector2 position;           // Esineen sijainti kartalla
        private Color color;               // Väri (tällä hetkellä ei käytetä piirtoon)
        int imagePixelX;                   // Kuva-atlaksen X-koordinaatti
        int imagePixelY;                   // Kuva-atlaksen Y-koordinaatti
        Texture image1;                    // Koko kuva-atlas

        public void SetImageAndIndex(Texture atlasImage, int imagesPerRow, int index)
        {
            image1 = atlasImage; // Asetetaan esineen käyttämä tekstuurikuva (atlas)

            // Lasketaan kuvake (tile) sijainti atlasissa ruudun koon mukaan
            imagePixelX = (index % imagesPerRow) * Game.tileSize;
            imagePixelY = (int)(index / imagesPerRow) * Game.tileSize;
        }

        public Item(string name, Vector2 position, Color color)
        {
            this.name = name;             // Esineen nimi
            this.position = position;     // Sijainti
            this.color = color;           // Väri (ei käytetä tällä hetkellä)
        }

        public void DrawItem()
        {
            // Lasketaan esineen piirtopaikka pikseleinä
            int pixelX = (int)(position.X * Game.tileSize);
            int pixelY = (int)(position.Y * Game.tileSize);

            // Rajauskuva (rectangle) tekstuurista
            Rectangle imageRect = new Rectangle(imagePixelX, imagePixelY, Game.tileSize, Game.tileSize);

            // Piirretään oikea osa tekstuurista oikeaan kohtaan
            Raylib.DrawTextureRec(image1, imageRect, new Vector2(pixelX, pixelY), Raylib.WHITE);
        }
    }
}
