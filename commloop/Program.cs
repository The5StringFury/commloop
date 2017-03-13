using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;

namespace commloop
{
    class Program
    {
        public static string command { get; private set; }
        public static string arguments { get; private set; }
        public static string inputfile { get; private set; }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Command to Execute");
                command = Console.ReadLine();
                Console.WriteLine("Arguments to use");
                arguments = Console.ReadLine();
                Console.WriteLine("Input File to Use.");
                inputfile = Console.ReadLine();
            }
            else
            {
                command = args[0];
                 arguments = args[1];
                 inputfile = args[2];
            }


            try
            {
                foreach (string line in File.ReadLines(inputfile))
                {

                    StringBuilder commandargs = new StringBuilder();

                    commandargs.AppendFormat(arguments, line.Split(','));
                    try
                    {
                        Process.Start(command, commandargs.ToString());
                    }
                    catch (Exception e)
                    {
                        if (!EventLog.SourceExists("commloop.exe")) EventLog.CreateEventSource("commloop.exe", "Application");
                        EventLog.WriteEntry("commloop.exe", "Failure To Execute Line: " + line + " /n Exception: " + e, EventLogEntryType.Error);
                        continue;

                    }

                }
                
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
                Console.ReadKey(false);
                
            }          
            
        }
    }
}
