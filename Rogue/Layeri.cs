using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue
{
    internal class Layeri
    {
        
        public string name;
        public int[] mapTiles;

        // Oletuskonstruktori
        public Layeri() { }

        // Vaihtoehtoinen konstruktori helppoon alustukseen
        public Layeri(string layerName, int[] tiles)
        {
            name = layerName;
            mapTiles = tiles;
        }

        // apumetodi
        public int GetTileCount()
        {
            return mapTiles != null ? mapTiles.Length : 0;
        }
    }
}

