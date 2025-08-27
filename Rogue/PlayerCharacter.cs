using System;
using System.Numerics;
using ZeroElectric.Vinculum;

namespace Rogue
{
    /// <summary>
    /// Hahmon rotu.
    /// </summary>
    public enum Race
    {
        Human,
        Elf,
        Dwarf
    }

    /// <summary>
    /// Hahmon hahmoluokka.
    /// </summary>
    public enum Class
    {
        Warrior,
        Mage,
        Rogue
    }

    /// <summary>
    /// Pelaajahahmo, joka sisältää tiedot ja toiminnot pelissä liikkumiseen ja piirtämiseen.
    /// </summary>
    internal class PlayerCharacter
    {
        public string nimi;                 // Pelaajan nimi
        public Race rotu;                  // Pelaajan rotu
        public Class hahmoluokka;          // Pelaajan hahmoluokka
        public Vector2 position;           // Pelaajan sijainti pelimaailmassa

        // Tekstuuritiedot
        Texture image1;
        int imagePixelX;
        int imagePixelY;

        private char image;                // Hahmon ASCII-kuva konsolissa
        ZeroElectric.Vinculum.Color color; // Hahmon väri (ei-konsoli)
        ConsoleColor color1;              // Konsoliväri

        /// <summary>
        /// Asettaa hahmon kuvan ja tekstuurin indeksin.
        /// </summary>
        public void SetImageAndIndex(Texture atlasImage, int imagesPerRow, int index)
        {
            image1 = atlasImage;
            imagePixelX = (index % imagesPerRow) * Game.tileSize - 16;
            imagePixelY = (int)(index / imagesPerRow) * Game.tileSize;
        }

        /// <summary>
        /// Pelaajahahmon konstruktori.
        /// </summary>
        public PlayerCharacter(char image, ZeroElectric.Vinculum.Color color, ConsoleColor color1)
        {
            this.image = image;
            this.color1 = color1;
            this.color = color;
        }

        /// <summary>
        /// Liikuttaa hahmoa haluttuun suuntaan.
        /// </summary>
        public void Move(int x_move, int y_move)
        {
            position.X += x_move;
            position.Y += y_move;

            // Varmistetaan, ettei pelaaja mene ruudun ulkopuolelle
            position.X = Math.Clamp(position.X, 0, Console.WindowWidth - 1);
            position.Y = Math.Clamp(position.Y, 0, Console.WindowHeight - 1);
        }

        /// <summary>
        /// Piirtää pelaajahahmon näytölle.
        /// </summary>
        public void Draw()
        {
            int pixelX = (int)(position.X * Game.tileSize);
            int pixelY = (int)(position.Y * Game.tileSize);

            // Piirretään konsolikuvake
            Console.ForegroundColor = color1;
            Console.SetCursorPosition((int)position.X, (int)position.Y);
            Console.Write(image);

            // Piirretään grafiikka tekstuurista
            Rectangle imageRect = new Rectangle(imagePixelX, imagePixelY, Game.tileSize, Game.tileSize);
            Raylib.DrawTextureRec(image1, imageRect, new Vector2(pixelX, pixelY), Raylib.WHITE);
        }
    }
}
