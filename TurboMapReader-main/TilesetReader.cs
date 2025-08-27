using System;
using Newtonsoft.Json;

namespace TurboMapReader
{
	internal class TilesetReader
	{
		public static TilesetFile? LoadTileSetFromFile(string filename)
		{
			if (File.Exists(filename) == false)
			{
				Console.Write("No such tile set file: ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(filename);
				Console.ResetColor();
				return null;
			}

			string[] nameAndType = filename.Split(".");
			string fileType = "";
			if (nameAndType.Length == 2 )
			{
				fileType = nameAndType[1];
			}
			else
			{
				Console.WriteLine("Error: tileset file has no type information");
				return null;
			}

			if (fileType == "tsx" || fileType == "xml")
			{
				return LoadFromXML(filename);
			}
			else if (fileType == "tsj" || fileType == "json")
			{
				return LoadFromJSON(filename);
			}
			else
			{
				Console.WriteLine("Error: Unsupported tileset file type or no type information found");
				return null;
			}
		}

		private static TilesetFile ConvertToTileset(TiledTileSetXML tilesetData)
		{
			TilesetFile loadedData = new TilesetFile();
			if (tilesetData == null)
			{
				return loadedData;
			}
			loadedData.version = tilesetData.version;
			loadedData.tiledversion = tilesetData.tiledversion;
			loadedData.name = tilesetData.name;
			loadedData.tilewidth = Convert.ToInt32(tilesetData.tilewidth);
			loadedData.tileheight = Convert.ToInt32(tilesetData.tileheight);
			loadedData.tilecount = Convert.ToInt32(tilesetData.tilecount);
			loadedData.columns = Convert.ToInt32(tilesetData.columns);
			if (tilesetData.image != null && tilesetData.image.source != null)
			{
				loadedData.image = tilesetData.image.source;
				int lastSlash = loadedData.image.LastIndexOf('/');
				loadedData.imageWoPath = tilesetData.image.source.Substring(lastSlash + 1);
				loadedData.imagewidth = Convert.ToInt32(tilesetData.image.width);
				loadedData.imageheight = Convert.ToInt32(tilesetData.image.height);
			}
			return loadedData;
		}
		private static TilesetFile ConvertToTileset(TiledTileSetJSON tilesetData)
		{
			TilesetFile loadedData = new TilesetFile();
			loadedData.version = tilesetData.version;
			loadedData.tiledversion = tilesetData.tiledversion;
			loadedData.name = tilesetData.name;
			loadedData.tilewidth = (tilesetData.tilewidth);
			loadedData.tileheight = (tilesetData.tileheight);
			loadedData.tilecount = (tilesetData.tilecount);
			loadedData.columns = (tilesetData.columns);
			loadedData.image = tilesetData.image;
			int lastSlash = loadedData.image.LastIndexOf('/');
			loadedData.imageWoPath = tilesetData.image.Substring(lastSlash + 1);
			loadedData.imagewidth = (tilesetData.imagewidth);
			loadedData.imageheight = (tilesetData.imageheight);
			return loadedData;
		}

		private static TilesetFile? LoadFromXML(string filename)
		{
			// Read data to nicer format
			try
			{
				TiledTileSetXML? tilesetData = ReadXml.ReadXmlTo<TiledTileSetXML>(filename);
				if (tilesetData != null)
				{
					return ConvertToTileset(tilesetData);
				}
				return null;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error when reading tileset file :{filename} >>>> {ex.Message}");
				return null;
			}
		}
		private static TilesetFile? LoadFromJSON(string filename)
		{
			try
			{
				StreamReader fileReader = new StreamReader(filename);
				string fileContents = fileReader.ReadToEnd();
				fileReader.Close();
				TiledTileSetJSON? tilesetData = JsonConvert.DeserializeObject<TiledTileSetJSON>(fileContents);
				if (tilesetData != null)
				{
					return ConvertToTileset(tilesetData);
				}
				else
				{
					throw new NullReferenceException();
				}
			}
			catch (Exception e)
			{
				Console.Write("Could not read tileset file: ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(filename);
				Console.ResetColor();
				Console.WriteLine("Error message: " + e.Message);
				return null;
			}
		}
	}
}
