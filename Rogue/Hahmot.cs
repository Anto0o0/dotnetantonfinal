using System;
using System.Numerics;
using ZeroElectric.Vinculum;

namespace Rogue
{
    public enum Hahmot
    {
        Piru,
        Tonttu,
        Hobitti
    }

    public enum Class
    {
        Megaknigt,
        Koira,
        Kissa
    }

    internal class PlayerCharacter
    {
        public string nimi;
        public Hahmot rotu;
        public Class hahmoluokka;
        public Vector2 position;

        // Raylib-textuuri
        private Texture image1;
        private int imagePixelX;
        private int imagePixelY;

        // Konsolipiirto
        private char image;
        private Color color;
        private ConsoleColor color1;

        public PlayerCharacter(char image, Color color, ConsoleColor color1)
        {
            this.image = image;
            this.color = color;
            this.color1 = color1;
        }

        public void SetImageAndIndex(Texture atlasImage, int imagesPerRow, int index)
        {
            image1 = atlasImage;
            imagePixelX = (index % imagesPerRow) * Peli.tileSize - 16;
            imagePixelY = (index / imagesPerRow) * Peli.tileSize;
        }

        public void Move(int x_move, int y_move)
        {
            position.X += x_move;
            position.Y += y_move;

            position.X = Math.Clamp(position.X, 0, Console.WindowWidth - 1);
            position.Y = Math.Clamp(position.Y, 0, Console.WindowHeight - 1);
        }

        public void Draw()
        {
            int pixelX = (int)(position.X * Peli.tileSize);
            int pixelY = (int)(position.Y * Peli.tileSize);

            // Konsoli
            Console.ForegroundColor = color1;
            Console.SetCursorPosition((int)position.X, (int)position.Y);
            Console.Write(image);

            // Raylib
            Rectangle imageRect = new Rectangle(imagePixelX, imagePixelY, Peli.tileSize, Peli.tileSize);
            Raylib.DrawTextureRec(image1, imageRect, new Vector2(pixelX, pixelY), Raylib.WHITE);
        }
    }
}
