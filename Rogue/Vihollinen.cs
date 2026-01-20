using System.Numerics;
using ZeroElectric.Vinculum;

namespace Rogue
{
    public class Vihollinen
    {
        //nimet 
        public string Name;
        public Vector2 Position;
        public int HitPoints;
        public int TileIndex;

        // Tekstuurin hallintaan liittyvät muuttujat
        private Texture tileAtlas;
        private int atlasCols;
        private int sourceX;
        private int sourceY;
        private Color tint;

 
        public int tileId
        {
            get => TileIndex;
            set => TileIndex = value;
        }

        public Vector2 position
        {
            get => Position;
            set => Position = value;
        }

        public void SetImageAndIndex(Texture atlasImage, int imagesPerRow, int index)
        {
            ConfigureTexture(atlasImage, imagesPerRow, index);
        }

        public void DrawEnemy()
        {
            Render();
        }
       

        public Vihollinen() { }

        public Vihollinen(string name, int tileIndex, int health)
        {
            Name = name;
            TileIndex = tileIndex;
            HitPoints = health;
        }

        public Vihollinen(string name, Vector2 startPos, Color colorTint)
        {
            Name = name;
            Position = startPos;
            tint = colorTint;
        }

        //konstruktori
        public Vihollinen(Vihollinen src)
        {
            Name = src.Name;
            Position = src.Position;
            tint = src.tint;

            atlasCols = src.atlasCols;
            tileAtlas = src.tileAtlas;

            HitPoints = src.HitPoints;
            TileIndex = src.TileIndex;
        }

        // Tekstuuritiedot käyttöön
        public void ConfigureTexture(Texture texture, int columns, int index)
        {
            tileAtlas = texture;
            atlasCols = columns;

            // Laske tekstuurin otto-alue
            sourceX = (index % columns) * Peli.tileSize;
            sourceY = (index / columns) * Peli.tileSize;
        }

        // Piirtometodi
        public void Render()
        {
            int px = (int)(Position.X * Peli.tileSize);
            int py = (int)(Position.Y * Peli.tileSize);

            Rectangle region = new Rectangle(sourceX, sourceY, Peli.tileSize, Peli.tileSize);
            Raylib.DrawTextureRec(tileAtlas, region, new Vector2(px, py), Raylib.WHITE);
        }
    }
}
