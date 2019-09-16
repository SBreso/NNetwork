using Accord.IO;
using Accord.Math;
using NNetwork.AI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NNetwork
{
    enum CommandInterfaceEnum
    {
        EXIT = 0,
        LOAD = 1,
        CHECK = 2,
        CLEAR = 10
    }

    internal class CommandInterface
    {
        static Dictionary<CommandInterfaceEnum, Func<bool>> commandCast = new Dictionary<CommandInterfaceEnum, Func<bool>>()
        {
            {CommandInterfaceEnum.EXIT,Exit },
            {CommandInterfaceEnum.CHECK,Check },
            {CommandInterfaceEnum.LOAD,Load },
            {CommandInterfaceEnum.CLEAR,Clear },
        };
        private static Brain _brain;

        private static bool Clear()
        {
            System.Console.Clear();
            return true;
        }

        private static bool Exit()
        {
            Environment.Exit(0);
            return true;
        }

        private static bool Check()
        {
            if (_brain == null)
            {
                Console.WriteLine("First load brain, thank you.");
                return true;
            }
            Console.WriteLine("X?");
            double x = 0, y = 0;
            Double.TryParse(Console.ReadLine(), out x);
            Console.WriteLine("Y?");
            Double.TryParse(Console.ReadLine(), out y);
            var t = new double[1][];
            t[0] = new double[4] { x, y, Math.Pow(x,2),Math.Pow(y,2) };
            var output = _brain.Check(t);
            foreach (var item in output)
            {
                Console.WriteLine($"Answer: {item}");
            }
            for (int i = 0; i < t.Length; i++)
            {
                Console.WriteLine($"Input: [{t[i][0]}, {t[i][1]}]->");
                for (int j = 0; j < output[i].Length; j++)
                {
                    Console.Write($"{output[i][j]}\t");
                }
                
            }
            
            return true;
        }

        private static bool Load()
        {
            Console.WriteLine("What is brain´s name?");
            var name = Console.ReadLine();
            var exist = Brain.ActivationNetworkExists(name);

            if (exist)
            {
                _brain = new Brain(name);
                _brain.Run().Wait();
                Console.WriteLine("Ok, this brain alredy exists.");
            }
            else
            {
                Console.WriteLine("Ok, this brain must learn.");
                //Datos de entrada dispuestos en 6 columnas, las cuatro primeras son inputs y las dos ultimas outputs
                DataTable table = new ExcelReader("examples.xls").GetWorksheet("Classification - Circle");
                // Convert the DataTable to input and output vectors
                var inputs = table.ToJagged<double>("X", "Y", "X2", "Y2");
                var outputs = table.ToJagged<double>("G1", "G2");
                //Distribucion de las 'caja negra', capas y neuronas por capa
                int[] neuronsCount = new int[3] { 5, 4, 2 };
                //Tolerancia, hasta que el error no sea menor a 1e-5 no se acaba el aprendizaje
                int tol = -5;
                double alpha = .5;
                var activationFunction = new Accord.Neuro.BipolarSigmoidFunction();
                _brain = new Brain(name, inputs, outputs, neuronsCount, activationFunction, tol);
                Console.WriteLine("Learning...");
                _brain.Run().Wait();
                Console.WriteLine("I just know kung-foo");
            }

            return _brain!=null;
        }

        public static void Run()
        {
            CommandInterface.PrintHelp();

            String chainWrittedByUser = null;
            bool continueFlag = true;

            while (continueFlag)
            {
                chainWrittedByUser = System.Console.ReadLine();

                if (!int.TryParse(chainWrittedByUser.ToLower(), out int op))
                {
                    op = -1;
                }
                Func<bool> command = null;
                if (!commandCast.TryGetValue((CommandInterfaceEnum)op, out command))
                {
                    Console.WriteLine("You´re dump!!!!");
                    continueFlag = true;
                    continue;
                }
                continueFlag = command();

                //System.Console.Clear();
                CommandInterface.PrintHelp();
            }
        }

        private static void PrintHelp()
        {
            System.Console.WriteLine("========== AI ==========");
            System.Console.WriteLine("\nPlease enter a command:");

            foreach (var item in Enum.GetNames(typeof(CommandInterfaceEnum)))
            {
                CommandInterfaceEnum value = CommandInterfaceEnum.EXIT;
                if (Enum.TryParse(item, out value))
                {
                    System.Console.WriteLine($"{(int)value} > {item.ToUpper()}");
                }
            }
        }
    }
}
