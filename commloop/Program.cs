using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace commloop
{
    class Program
    {
        static void Main(string[] args)
        {

            string command;

            string inputfile;

            Console.WriteLine("Command to execute.");
            command = Console.ReadLine();

            Console.WriteLine("Input File to Use.");
            inputfile = Console.ReadLine();

            foreach (string line in File.ReadLines(inputfile))
            {
                
            }
            File.ReadLines(inputfile);



        }
    }
}
