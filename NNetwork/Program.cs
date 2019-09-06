using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            }
            
        }
    }
}
