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
    public class Enemy
    {
        public string name;               // Vihollisen nimi
        public Vector2 position;          // Vihollisen sijainti kartalla (ruutukoordinaatit)
        private Color color;              // Vihollisen väri (ei käytössä piirrossa)
        int imagesPerRow;                 // Kuinka monta kuvaa rivissä on tekstuurissa
        Texture image1;                   // Tekstuurikuva (atlas)
        int imagePixelX;                  // Vihollisen kuva-atlaksen X-koordinaatti
        int imagePixelY;                  // Vihollisen kuva-atlaksen Y-koordinaatti

        public int tileId;                // ID kuva-atlaksessa
        public int hp;                    // Vihollisen elämäpisteet

        public Enemy() { }

        public Enemy(Enemy copyFrom)
        {
            // Luo uuden vihollisen kopiona annetusta pohjasta (esim. kun kartalta ladataan vihollinen)
            this.name = copyFrom.name;
            this.position = copyFrom.position;
            this.color = copyFrom.color;
            this.imagesPerRow = copyFrom.imagesPerRow;
            this.image1 = copyFrom.image1;
            this.hp = copyFrom.hp;
            this.tileId = copyFrom.tileId;
        }

        public void SetImageAndIndex(Texture atlasImage, int imagesPerRow, int index)
        {
            // Asetetaan kuva ja lasketaan sijainti kuva-atlaksessa
            image1 = atlasImage;
            imagePixelX = (index % imagesPerRow) * Game.tileSize;
            imagePixelY = (int)(index / imagesPerRow) * Game.tileSize;
        }

        public Enemy(string name, Vector2 position, Color color)
        {
            // Vihollinen sijainti- ja väritiedolla (ei yleensä käytössä pelissä)
            this.name = name;
            this.position = position;
            this.color = color;
        }

        public Enemy(string name, int tileID, int hitPoints)
        {
            // Vihollinen luodaan pelitilastoilla (esim. editorista tai JSON:sta)
            this.name = name;
            this.tileId = tileID;
            this.hp = hitPoints;
        }

        public void DrawEnemy()
        {
            // Piirtää vihollisen ruudulle
            int pixelX = (int)(position.X * Game.tileSize);
            int pixelY = (int)(position.Y * Game.tileSize);

            Rectangle imageRect = new Rectangle(imagePixelX, imagePixelY, Game.tileSize, Game.tileSize);

            Raylib.DrawTextureRec(image1, imageRect, new Vector2(pixelX, pixelY), Raylib.WHITE);
        }
    }
}
