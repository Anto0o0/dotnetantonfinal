using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurboMapReader
{
	/// <summary>
	/// Represents a tileset exported in JSON format
	/// </summary>
	internal class TiledTileSetJSON
	{
		public int columns = 0;
		public string image = "";
		public int imageheight = 0;
		public int imagewidth = 0;
		public int margin = 0;
		public string name = "";
		public int spacing = 0;
		public int tilecount = 0;
		public string tiledversion = "";
		public int tileheight = 0;
		public int tilewidth = 0;
		public string type = "";
		public string version = "";
	}
}
