using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using NDesk.Options;
using System.Collections.Generic;


namespace commloop
{
    class Program
    {
        public static string command { get; private set; }
        public static string arguments { get; private set; }
        public static string inputfile { get; private set; }
        public static int exitcode { get; private set; }
        public static string argfilePath { get; private set; }
        public static char delimiter { get; private set; }


        static void Main(string[] args)
        {
        
            bool show_help = false;
            delimiter = ',';
            var p = new OptionSet()
            {
                {"c|command=", "the {COMMAND} to execute during the loop.",
                (string v)=>command = v},
                {"a|arg=", "the {ARGUMENTS} to append to the command for each line item.",
                (string v)=>arguments = v},
                {"f|file=", "the {FILE} containing the lines of data to be used for each iteration usually a CSV.",
                (string v)=>inputfile = v},
                {"d|delimiter =", "the {delimiter} (char)  to be used with the input file to seperate each line into chunks of data. The default is comma (,)",
                (char v)=> delimiter= v},
                {"A|argfile=", "the File to extract the {ARGUMENTS} from. Will only treat the first line as the Argument.",
                (string v)=>argfilePath =v},
                {"h|help", "show this message and exit",
                v => show_help = v !=null }
            };

            List<string> cArgs;
            try
            {
                cArgs = p.Parse(args);

                if(show_help)
                {
                    ShowHelp(p);
                    return;
                }
                else if( args.Length < 3)
                {
                    Console.WriteLine("Not Enough paramaters detected!");
                    ShowHelp(p);
                    Environment.ExitCode = -200;
                    return;
                }
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                ShowHelp(p);
                Environment.ExitCode = -200;
                return;            
            }

            exitcode = 0;
            try
            {
                if(string.IsNullOrEmpty(arguments))
                {
                    try
                    {
                        arguments = File.ReadAllText(argfilePath);
                    }
                    catch (Exception x)
                    {

                        Console.WriteLine(x);
                        exitcode = -100;
                        ShowHelp(p);
                        return;
                    }
                }
                foreach (string line in File.ReadLines(inputfile))
                {

                    StringBuilder commandargs = new StringBuilder();

                    
                    try
                    {
                        string[] linex = line.Split(delimiter);
                        for (int i = 0; i < linex.Length; i++)
                        {  
                           arguments = arguments.Replace("{{" + i.ToString() + "}}", linex[i]);
                        }

                        commandargs.Append(arguments); 
                        Process.Start(command, commandargs.ToString()).WaitForExit();
                        if (!EventLog.SourceExists("commloop.exe")) EventLog.CreateEventSource("commloop.exe", "Application");
                        EventLog.WriteEntry("commloop.exe", "Executed Command: " + command + "  Arguments Passesd:" + commandargs.ToString() , EventLogEntryType.Information);


                    }
                    catch (Exception e)
                    {
                        if (!EventLog.SourceExists("commloop.exe")) EventLog.CreateEventSource("commloop.exe", "Application");
                        EventLog.WriteEntry("commloop.exe", "Failure To Execute Line: " + line + "  Arguments Passesd:" + arguments + "  Exception: " + e, EventLogEntryType.Error);
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
                ShowHelp(p);
                

            }  
            finally
            {
                Console.WriteLine("commloop complete press any key...");
                //Console.ReadKey(false);
                Environment.ExitCode = exitcode;
            }        
            
        }

        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: commloop [Options]");
            Console.WriteLine("The main purpose of this applicaiton is to execute a command repeatedly ");
            Console.WriteLine("over a given set of data.  Expects a Command, Data File, and Arguments or argument");
            Console.WriteLine(" depending on complexity. Argument should be formatted with {{n}} fields where data");
            Console.WriteLine(" needs to be injected. n is determined by the number of columns extracted from the data file in a 0");
            Console.WriteLine("based array.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
