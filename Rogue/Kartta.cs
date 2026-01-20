using Newtonsoft.Json;
using System.Numerics;
using System.IO;
using System.Collections.Generic;
using ZeroElectric.Vinculum;

namespace Rogue
{
    /// <summary>
    /// Kartta-luokka hallitsee ruudukon, tilet, viholliset ja esineet.
    /// </summary>
    internal class Kartta
    {
        public int mapWidth;
        public Layeri[] layers;

        private Texture image1;
        private int imagePixelX, imagePixelY;
        private int imagesPerRow;

        public List<Vihollinen> enemies;
        public List<Item> items;
        private List<Vihollinen> enemyTypes;

        public enum MapTile : int
        {
            Floor = 1,
            Wall = 15,
            Enemy = 110
        }

        /// <summary>
        /// Hakee ruudun ID:n annetusta sijainnista.
        /// </summary>
        public int getTileAt(Vector2 pos) => layers[0].mapTiles[(int)pos.X + (int)pos.Y * mapWidth];

        /// <summary>
        /// Palauttaa ground-layerin tile enum-arvona.
        /// </summary>
        public MapTile GetTileAtGround(int x, int y) => (MapTile)layers[0].mapTiles[x + y * mapWidth];

        /// <summary>
        /// Palauttaa enemy-layerin tile enum-arvona.
        /// </summary>
        public MapTile GetTileAtEnemy(int x, int y) => (MapTile)layers[2].mapTiles[x + y * mapWidth];

        /// <summary>
        /// Asettaa tekstuurin ja kuvan indeksin kartalle.
        /// </summary>
        public void SetImageAndIndex(Texture atlasImage, int imagesPerRow, int index)
        {
            image1 = atlasImage;
            this.imagesPerRow = imagesPerRow;
        }

        /// <summary>
        /// Piirtää kartan sekä viholliset ja esineet.
        /// </summary>
        public void DrawMap()
        {
            Layeri ground = layers[0];
            Console.ForegroundColor = ConsoleColor.Gray;
            int height = ground.mapTiles.Length / mapWidth;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    int tileId = ground.mapTiles[y * mapWidth + x] - 1;

                    imagePixelX = (tileId % imagesPerRow) * Peli.tileSize;
                    imagePixelY = (tileId / imagesPerRow) * Peli.tileSize;

                    int px = x * Peli.tileSize;
                    int py = y * Peli.tileSize;

                    Raylib.DrawTextureRec(image1,
                        new Rectangle(imagePixelX, imagePixelY, Peli.tileSize, Peli.tileSize),
                        new Vector2(px, py),
                        Raylib.WHITE);
                }
            }

            if (enemies != null) foreach (var e in enemies) e.DrawEnemy();
            if (items != null) foreach (var i in items) i.DrawItem();
        }

        /// <summary>
        /// Lataa vihollistyypit JSON-tiedostosta.
        /// </summary>
        public void LoadEnemyTypes(string filename)
        {
            enemyTypes = new List<Vihollinen>();
            if (!File.Exists(filename)) return;

            string json = File.ReadAllText(filename);
            enemyTypes = JsonConvert.DeserializeObject<List<Vihollinen>>(json);
        }

        private Vihollinen CreateEnemyBySpriteId(int spriteId)
        {
            foreach (var template in enemyTypes)
            {
                if (template.tileId == spriteId)
                    return new Vihollinen(template);
            }

            Console.WriteLine($"Error: no enemy with id {spriteId}");
            return null;
        }

        /// <summary>
        /// Lataa viholliset ja esineet kartan layerien mukaan.
        /// </summary>
        public void LoadEnemiesAndItems()
        {
            LoadEnemyTypes("enemies.json");
            enemies = new List<Vihollinen>();

            Layeri enemyLayer = layers[2];
            int height = enemyLayer.mapTiles.Length / mapWidth;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    int tileId = enemyLayer.mapTiles[y * mapWidth + x];
                    if (tileId == 0) continue;

                    Vihollinen e = CreateEnemyBySpriteId(tileId);
                    if (e == null) continue;

                    e.SetImageAndIndex(image1, imagesPerRow, tileId);
                    e.position = new Vector2(x, y);
                    enemies.Add(e);
                }
            }

            items = new List<Item>();
            Layeri itemLayer = layers[1];
            int itemHeight = itemLayer.mapTiles.Length / mapWidth;

            for (int y = 0; y < itemHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    int tileId = itemLayer.mapTiles[y * mapWidth + x];
                    Vector2 pos = new Vector2(x, y);
                    if (tileId == 0) continue;

                    Item newItem = tileId switch
                    {
                        90 => new Item("chest", pos, Raylib.WHITE),
                        119 => new Item("axe", pos, Raylib.WHITE),
                        104 => new Item("knife", pos, Raylib.WHITE),
                        _ => null
                    };

                    if (newItem != null)
                    {
                        newItem.SetImageAndIndex(image1, imagesPerRow, tileId - 1);
                        items.Add(newItem);
                    }
                }
            }
        }
    }
}
