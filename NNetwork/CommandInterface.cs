using NNetwork.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace NNetwork
{
    enum CommandInterfaceEnum
    {
        EXIT = 0,
        READ_VALUES = 1,
        LEARN = 2,
        CHECK = 3,
        SAVE = 4,
        LOAD = 5,
        CLEAR = 10
    }

    internal class CommandInterface
    {
        static Dictionary<CommandInterfaceEnum, Func<bool>> commandCast = new Dictionary<CommandInterfaceEnum, Func<bool>>()
        {
            {CommandInterfaceEnum.EXIT,Exit },
            {CommandInterfaceEnum.READ_VALUES,ReadValues },
            {CommandInterfaceEnum.LEARN,Learn },
            {CommandInterfaceEnum.CHECK,Check },
            {CommandInterfaceEnum.SAVE,Save },
            {CommandInterfaceEnum.LOAD,Load },
            {CommandInterfaceEnum.CLEAR,Clear },
        };

        private static bool Clear()
        {
            System.Console.Clear();
            return true;
        }

        private static bool ReadValues()
        {
            _brain.SetValuesToLearn();
            return true;
        }

        public static Brain _brain { get; private set; }

        private static bool Exit()
        {
            Environment.Exit(0);
            return true;
        }

        private static bool Learn()
        {
            _brain.Learn();
            return true;
        }

        private static bool Check()
        {
            Console.WriteLine("X?");
            double x = 0,y=0;
            Double.TryParse(Console.ReadLine(), out x);
            Console.WriteLine("Y?");
            Double.TryParse(Console.ReadLine(), out y);
            _brain.Check(x,y);
            return true;
        }

        private static bool Save()
        {
            _brain.Save();
            return true;
        }

        private static bool Load()
        {
            _brain.Load();
            return true;
        }

        

        public static void Run()
        {
            _brain = new Brain();
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
