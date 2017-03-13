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
        public static string apikey { get; private set; }
        public static string inputfile { get; private set; }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Okta Api Key to Use");
                apikey = Console.ReadLine();
                Console.WriteLine("Input File to Use.");
                inputfile = Console.ReadLine();
            }
            else
            {
                 apikey = args[0];
                 inputfile = args[1];
            }


            try
            {
                foreach (string line in File.ReadLines(inputfile))
                {

                    StringBuilder commandargs = new StringBuilder();
                    commandargs.Append("- X PUT - H \"Accept: application / json\" - H \"Content - Type: application / json\" - H \"Authorization: SSWS ");
                    commandargs.Append(apikey);
                    commandargs.AppendFormat("\" - H \"Cache-Control: no-cache\" - d \'{" +
                    "\"credentials\": {" +
                     "\"password\" : { \"value\": \"{2}\" }" +
                    "}" +
                    "}\' \"https://amwaysso-qa.oktapreview.com/api/v1/users/{1}\"", line.Split(','));

                    try
                    {
                        Process.Start("curl.exe", commandargs.ToString());
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
