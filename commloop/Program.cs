using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace commloop
{
    class Program
    {
        public static string command { get; private set; }
        public static string arguments { get; private set; }
        public static string inputfile { get; private set; }
        public static int exitcode { get; private set; }


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
            //we should exit with a code of 0  if all goes as planned. 
            exitcode = 0;
            try
            {
                foreach (string line in File.ReadLines(inputfile))
                {

                    StringBuilder commandargs = new StringBuilder();

                    commandargs.AppendFormat(arguments, line.Split(','));
                    try
                    {
                        Process.Start(command, commandargs.ToString()).WaitForExit();                        
                    }
                    catch (Exception e)
                    {
                        if (!EventLog.SourceExists("commloop.exe")) EventLog.CreateEventSource("commloop.exe", "Application");
                        EventLog.WriteEntry("commloop.exe", "Failure To Execute Line: " + line + " /n Exception: " + e, EventLogEntryType.Error);
                        //The exit code should count the number of failures we got from running our command
                        exitcode++;
                        continue;
                    }
                }                
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
                exitcode = -100;
                          
            }  
            finally
            {
                Console.WriteLine("commloop complete press any key...");
                Console.ReadKey(false);
                Environment.ExitCode = exitcode;
            }        
            
        }
    }
}
