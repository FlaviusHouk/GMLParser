using System;

using GMLParser.CLI;

namespace GMLParser
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine cl = new CommandLine(args);

            cl.ProcessFiles();   
        }
    }
}
