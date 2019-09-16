using System;

namespace NNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CommandInterface.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                CommandInterface.Run();
            }
            
        }
    }
}
