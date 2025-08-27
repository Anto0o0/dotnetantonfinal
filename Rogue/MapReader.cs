using System;
using System.IO;
using Newtonsoft.Json;
using TurboMapReader;

namespace Rogue
{
    /// <summary>
    /// Vastaa pelin karttojen lataamisesta joko testikarttana, tiedostosta tai Tiled-editorista.
    /// </summary>
    internal class MapReader
    {
        /// <summary>
        /// Palauttaa kovakoodatun testikartan.
        /// </summary>
        /// <returns>Testikartta Map-oliona</returns>
        public Map LoadTestMap()
        {
            Map testi = new Map();
            testi.mapWidth = 8;
            testi.layers = new MapLayer[3];
            testi.layers[0].mapTiles = new int[]
            {
                2, 2, 2, 2, 2, 2, 2, 2,
                2, 1, 1, 2, 1, 1, 1, 2,
                2, 1, 1, 2, 1, 1, 1, 2,
                2, 1, 1, 1, 1, 1, 2, 2,
                2, 2, 2, 2, 1, 1, 1, 2,
                2, 1, 1, 1, 1, 1, 1, 2,
                2, 2, 2, 2, 2, 2, 2, 2
            };

            return testi;
        }

        /// <summary>
        /// Lukee karttatiedoston .json-muodossa ja palauttaa sen Map-oliona.
        /// </summary>
        /// <param name="fileName">Karttatiedoston nimi</param>
        /// <returns>Map-olio tiedoston sisällöstä tai testikartta jos tiedostoa ei löydy</returns>
        public Map ReadMapFromFile(string fileName)
        {
            bool exists = File.Exists(fileName);

            if (!exists)
            {
                Console.WriteLine($"File {fileName} not found");
                return LoadTestMap();
            }

            string fileContents;

            using (StreamReader reader = File.OpenText(fileName))
            {
                fileContents = reader.ReadToEnd();
            }

            Map loadedMap = JsonConvert.DeserializeObject<Map>(fileContents,
                new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                });

            return loadedMap;
        }

        /// <summary>
        /// Lukee kartan Tiled-editorin tuottamasta tiedostosta ja rakentaa siitä pelin sisäisen Map-olion.
        /// </summary>
        /// <param name="filename">Tiled-muotoisen tiedoston nimi</param>
        /// <returns>Map-olio, joka sisältää eri kerrokset (maa, viholliset, esineet)</returns>
        public Map ReadTiledMapFromFile(string filename)
        {
            TiledMap loadedTileMap = TurboMapReader.MapReader.LoadMapFromFile(filename);

            int mapWidth = loadedTileMap.width;

            TurboMapReader.MapLayer groundLayer = loadedTileMap.GetLayerByName("Ground");
            TurboMapReader.MapLayer enemyLayer = loadedTileMap.GetLayerByName("Enemy");
            TurboMapReader.MapLayer itemLayer = loadedTileMap.GetLayerByName("Items");

            int[] groundTiles = groundLayer.data;
            int[] enemyTiles = enemyLayer.data;
            int[] itemTiles = itemLayer.data;

            Map tiledMap = new Map();
            tiledMap.mapWidth = mapWidth;
            tiledMap.layers = new MapLayer[3];

            tiledMap.layers[0] = new MapLayer { mapTiles = groundTiles };
            tiledMap.layers[1] = new MapLayer { mapTiles = itemTiles };
            tiledMap.layers[2] = new MapLayer { mapTiles = enemyTiles };

            return tiledMap;
        }
    }
}
