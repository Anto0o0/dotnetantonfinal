using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TurboMapReader
{

	/// <summary>
	/// This is the interface to this library.
	/// Contains a static function LoadMapFromFile
	/// </summary>
	public class MapReader
	{
		/// <summary>
		/// Attempts to load a Tiled Map file
		/// from given file.
		/// On success returns a TiledMap instance
		/// that is populated with the data.
		/// If reading fails a Null is returned instead.
		/// </summary>
		/// <param name="filename">Filename of a file. The file must be a .json file that has been exported from Tiled without compression.</param>
		/// <returns>The data in the file wrapped in a TiledMap instance or null if reading fails.</returns>
		public static TiledMap? LoadMapFromFile(string filename)
		{
			StreamReader? fileReader;
			try
			{
				fileReader = new StreamReader(filename);
			}
			catch(FileNotFoundException e)
			{
				Console.Write("Could not find Tiled map file: ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(filename);
				Console.ResetColor();
				Console.WriteLine("Error message: " + e.Message);
				return null;
			}

			string fileContents = fileReader.ReadToEnd();
			fileReader.Close();

			try
			{
				TiledMap? mapData = JsonConvert.DeserializeObject<TiledMap>(fileContents);
				if (mapData != null && mapData.tilesets != null && mapData.tilesets.Count > 0)
				{
					mapData.tilesetFiles = new List<TilesetFile>();
					foreach (Tileset tileset in mapData.tilesets)
					{
						TilesetFile? tilesetFile = new TilesetFile();
						tilesetFile = TilesetReader.LoadTileSetFromFile(tileset.source);
						if (tilesetFile != null)
						{
							mapData.tilesetFiles.Add(tilesetFile);
						}
						else 
						{
							Console.Write("Could not read tileset from ");
							Console.ForegroundColor = ConsoleColor.Green;
							Console.WriteLine(tileset.source);
							Console.ResetColor();
						}
					}
				}
				return mapData;
			}
			catch (JsonReaderException e)
			{
				Console.Write("Could not read Tiled map file: ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(filename);
				Console.ResetColor();
				Console.WriteLine("Error message: " + e.Message);
				return null;
			}
		}

	}
}