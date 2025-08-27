using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TurboMapReader
{
	/// <summary>
	/// Represents a tileset exported in XML format
	/// </summary>
	[Serializable]
	public class TiledTileSetXML
	{
		[XmlAttribute]
		public string version { get; set; } = "";
		[XmlAttribute]
		public string tiledversion { get; set; } = "";
		[XmlAttribute]
		public string name { get; set; } = "";
		[XmlAttribute]
		public string tilewidth { get; set; } = "0";
		[XmlAttribute]
		public string tileheight { get; set; } = "0";
		[XmlAttribute]
		public string tilecount { get; set; } = "0";
		[XmlAttribute]
		public string columns { get; set; } = "0";
		[XmlElement]
		public image? image { get; set; }
	}
	[Serializable]
	public class image
	{
		[XmlAttribute]
		public string source { get; set; } = "";
		[XmlAttribute]
		public string width { get; set; } = "0";
		[XmlAttribute]
		public string height { get; set; } = "0";
	}
}
