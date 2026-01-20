using System;
using System.IO;
using Newtonsoft.Json;
using TurboMapReader;

namespace Rogue
{
    internal class Kartanlukija
    {
       
        public Kartta LoadTestMap()
        {
            var testMap = new Kartta
            {
                mapWidth = 8,
                layers = new Layeri[3]
            };

            // Maa-layer, kovakoodatut arvot
            testMap.layers[0] = new Layeri
            {
                mapTiles = new int[]
                {
                    2, 2, 2, 2, 2, 2, 2, 2,
                    2, 1, 1, 2, 1, 1, 1, 2,
                    2, 1, 1, 2, 1, 1, 1, 2,
                    2, 1, 1, 1, 1, 1, 2, 2,
                    2, 2, 2, 2, 1, 1, 1, 2,
                    2, 1, 1, 1, 1, 1, 1, 2,
                    2, 2, 2, 2, 2, 2, 2, 2
                }
            };

            return testMap;
        }

       
        public Kartta ReadMapFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Varoitus: tiedostoa '{fileName}' ei löytynyt. Ladataan testikartta.");
                return LoadTestMap();
            }

            string json;
            using (var reader = new StreamReader(fileName))
            {
                json = reader.ReadToEnd();
            }

            // Deserialisoi JSON-tiedoston Map-olioksi
            var loadedMap = JsonConvert.DeserializeObject<Kartta>(json,
                new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                });

            return loadedMap;
        }

      
        public Kartta ReadTiledMapFromFile(string filename)
        {
            TiledMap tileMap = TurboMapReader.MapReader.LoadMapFromFile(filename);

            int width = tileMap.width;

            // Haetaan eri layerit Tiled-mallista
            MapLayer ground = tileMap.GetLayerByName("Ground");
            MapLayer enemies = tileMap.GetLayerByName("Enemy");
            MapLayer items = tileMap.GetLayerByName("Items");

            int[] groundTiles = ground.data;
            int[] enemyTiles = enemies.data;
            int[] itemTiles = items.data;

            var map = new Kartta
            {
                mapWidth = width,
                layers = new Layeri[3]
            };

            // Asetetaan layerit Map-olioon
            map.layers[0] = new Layeri { mapTiles = groundTiles };
            map.layers[1] = new Layeri { mapTiles = itemTiles };
            map.layers[2] = new Layeri { mapTiles = enemyTiles };

            return map;
        }
    }
}
