using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAspCore
{
    public static class ArgumentParser
    {
        public static Dictionary<ArgType, string> Parse(this string[] args)
        {
            var result = new Dictionary<ArgType, string>();
            foreach (var arg in args)
            {
                if (arg == "-h" || arg == "--help" || arg == "help")
                    ShowHelp();

                var splitedArg = arg.Split("=");
                Enum.TryParse(typeof(ArgType), splitedArg[0], out object? argTypeObj);
                if (argTypeObj is { })
                {
                    var argType = (ArgType)argTypeObj;
                    result.Add(argType, splitedArg[1]);
                }
            }
            return result;
        }

        public static void ShowHelp()
        {
            foreach (var item in Enum.GetNames(typeof(ArgType)))
            {
                Console.WriteLine(item);
            }
            Environment.Exit(0);
        }
    }
}
