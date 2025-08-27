using System.Xml.Serialization;

namespace TurboMapReader
{
	/// <summary>
	/// This class can read a given XML file to any type of object
	/// </summary>
	internal class ReadXml
	{
		public static T? ReadXmlTo<T>(string filename)
		{
			// Contents of the XML file
			T? contents;
			var serializer = new XmlSerializer(typeof(T));
			using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				try
				{
					contents = (T?)serializer.Deserialize(fileStream);
					return contents;
				}
				catch (InvalidOperationException e)
				{
					Console.Write("Error in .tsx file: ");
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine(filename);
					Console.ResetColor();
					Console.WriteLine("Error message: " + e.Message);
					return default(T);

				}
			}
		}
	}
}
