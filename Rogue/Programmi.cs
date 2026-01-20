namespace Rogue
{
    internal static class Programmi
    {
        static void Main(string[] args)
        {
           
            Console.WriteLine("Huomenta!");

            // Luodaan peli-instanssi ja käynnistetään se
            Peli peli = new();
            peli.Run();
        }
    }
}
