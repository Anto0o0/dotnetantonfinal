using System;
using System.Numerics;
using ZeroElectric.Vinculum;

namespace Rogue
{
    internal class Item
    {
        public string name;              
        public Vector2 position;       

        private Color color;             
        private Texture atlasTexture;    // Koko sprite-atlas
        private int atlasX;              // Kuva-atlaksen X-koordinaatti
        private int atlasY;              // Kuva-atlaksen Y-koordinaatti

        public Item(string name, Vector2 position, Color color)
        {
            this.name = name;
            this.position = position;
            this.color = color;
        }

        public void SetImageAndIndex(Texture texture, int imagesPerRow, int index)
        {
            atlasTexture = texture;

            // Selvitetään missä kohtaa atlas-kuvaa oikea kuvake sijaitsee
            atlasX = (index % imagesPerRow) * Peli.tileSize;
            atlasY = (index / imagesPerRow) * Peli.tileSize;
        }

        public void DrawItem()
        {
            // Muutetaan karttakoordinaatit pikseleiksi
            int drawX = (int)(position.X * Peli.tileSize);
            int drawY = (int)(position.Y * Peli.tileSize);

            // Otetaan oikea osa atlas-kuvasta
            Rectangle srcRect = new Rectangle(atlasX, atlasY, Peli.tileSize, Peli.tileSize);

            // Piirretään esine oikeaan paikkaan
            Raylib.DrawTextureRec(atlasTexture, srcRect, new Vector2(drawX, drawY), Raylib.WHITE);
        }
    }
}
