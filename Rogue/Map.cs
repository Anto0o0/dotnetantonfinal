using Newtonsoft.Json;
using System.Numerics;
using System.Text.RegularExpressions;
using ZeroElectric.Vinculum;

namespace Rogue
{
    internal class Map
    {
        public int mapWidth; // Kartan leveys ruuduissa
        public MapLayer[] layers; // Kolme layeria: ground, item ja enemy

        Texture image1; // Kartan tekstuurikuva
        int imagePixelX; // Piirrettävän tile-kuvan X-koordinaatti atlasissa
        int imagePixelY; // Piirrettävän tile-kuvan Y-koordinaatti atlasissa

        int imagesPerRow; // Kuinka monta kuvaa yhdessä rivissä tekstuurissa

        public List<Enemy> enemies; // Kartalla olevat viholliset
        public List<Item> items; // Kartalla olevat esineet

        public enum MapTile : int
        {
            Floor = 1,
            Wall = 15,
            Enemy = 110,
        }

        List<Enemy> enemyTypes; // Ladatut vihollistyypit (mallitiedot)

        public int getTileAt(Vector2 newPlace)
        {
            MapLayer ground = layers[0]; // Haetaan ground-layer
            return (ground.mapTiles[(int)newPlace.X + (int)newPlace.Y * mapWidth]); // Palautetaan ruudun ID
        }

        public MapTile GetTileAtGround(int x, int y)
        {
            MapLayer ground = layers[0]; // Haetaan ground-layer
            return (MapTile)(ground.mapTiles[x + y * mapWidth]); // Palautetaan tile enum-arvona
        }

        public MapTile GetTileAtEnemy(int x, int y)
        {
            MapLayer ground = layers[2]; // Haetaan enemy-layer
            return (MapTile)(ground.mapTiles[x + y * mapWidth]); // Palautetaan tile enum-arvona
        }

        public void SetImageAndIndex(Texture atlasImage, int imagesPerRow, int index)
        {
            this.imagesPerRow = imagesPerRow; // Montako kuvaa per rivi
            image1 = atlasImage; // Asetetaan kuvatiedosto
        }

        public void DrawMap()
        {
            MapLayer ground = layers[0]; // Haetaan ground-layer
            Console.ForegroundColor = ConsoleColor.Gray;

            for (int row = 0; row < ground.mapTiles.Length / mapWidth; row++)
            {
                for (int col = 0; col < mapWidth; col++)
                {
                    int tileId = ground.mapTiles[row * mapWidth + col]; // Nykyinen tile ID
                    tileId--;

                    Console.SetCursorPosition(col, row);

                    // Lasketaan piirrettävän kuvan sijainti tekstuurissa
                    imagePixelX = (tileId % imagesPerRow) * Game.tileSize;
                    imagePixelY = (int)(tileId / imagesPerRow) * Game.tileSize;
                    int pixelX = (int)(col * Game.tileSize);
                    int pixelY = (int)(row * Game.tileSize);

                    Rectangle imageRect = new Rectangle(imagePixelX, imagePixelY, Game.tileSize, Game.tileSize);
                    Raylib.DrawTextureRec(image1, imageRect, new Vector2(pixelX, pixelY), Raylib.WHITE); // Piirretään tile
                }
            }

            foreach (var item in enemies)
            {
                item.DrawEnemy(); // Piirrä vihollinen
            }

            foreach (var item in items)
            {
                item.DrawItem(); // Piirrä esine
            }
        }

        public void LoadEnemyTypes(string filename)
        {
            enemyTypes = new List<Enemy>();

            if (File.Exists(filename))
            {
                string fileContents;

                using (StreamReader reader = File.OpenText(filename))
                {
                    fileContents = reader.ReadToEnd(); // Lue koko tiedosto
                }

                enemyTypes = JsonConvert.DeserializeObject<List<Enemy>>(fileContents); // Deserialize JSON listaksi
            }
        }

        private Enemy CreateEnemyBySpriteId(int spriteId)
        {
            foreach (Enemy template in enemyTypes)
            {
                if (template.tileId == spriteId) // Jos ID täsmää, luodaan kopio
                {
                    return new Enemy(template); // Palautetaan klooni
                }
            }

            Console.WriteLine($"Error, no enemy found with id: {spriteId}"); // Jos ei löydy
            return null;
        }

        public void LoadEnemiesAndItems()
        {
            LoadEnemyTypes("enemies.json"); // Ladataan vihollistyypit
            enemies = new List<Enemy>();

            MapLayer enemyLayer = layers[2]; // Haetaan vihollis-layer

            int[] enemyTiles = enemyLayer.mapTiles;
            int mapHeight = enemyTiles.Length / mapWidth;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Vector2 position = new Vector2(x, y);
                    int index = x + y * mapWidth;
                    int tileId = enemyTiles[index];

                    if (tileId != 0) // Jos ruudussa on vihollinen
                    {
                        Enemy newenemy = CreateEnemyBySpriteId((int)tileId);
                        if (newenemy != null)
                        {
                            newenemy.SetImageAndIndex(image1, imagesPerRow, tileId); // Asetetaan kuva
                            newenemy.position = position;
                            enemies.Add(newenemy); // Lisätään listaan
                        }
                    }
                }
            }

            // Esineiden lataus
            items = new List<Item>();

            MapLayer itemLayers = layers[1]; // Haetaan item-layer

            int[] itemTiles = itemLayers.mapTiles;
            int mapHeight1 = itemTiles.Length / mapWidth;

            for (int y = 0; y < mapHeight1; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Vector2 position = new Vector2(x, y);
                    int index = x + y * mapWidth;
                    int tileId = itemTiles[index];

                    switch (tileId)
                    {
                        case 0:
                            // Tyhjä ruutu, ei esinettä
                            break;
                        case 90:
                            Item i = new Item("chest", position, Raylib.WHITE);
                            i.SetImageAndIndex(image1, imagesPerRow, tileId -= 1); // Aseta kuva
                            items.Add(i);
                            break;
                        case 119:
                            Item a = new Item("axe", position, Raylib.WHITE);
                            a.SetImageAndIndex(image1, imagesPerRow, tileId -= 1);
                            items.Add(a);
                            break;
                        case 104:
                            Item b = new Item("knife", position, Raylib.WHITE);
                            b.SetImageAndIndex(image1, imagesPerRow, tileId -= 1);
                            items.Add(b);
                            break;
                    }
                }
            }
        }
    }
}
