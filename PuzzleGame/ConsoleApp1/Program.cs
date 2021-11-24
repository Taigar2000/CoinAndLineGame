using System;
using FileReading;
using DataAnalysis;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main()
        {
            try
            {
                Console.Write("Please inpet path to data file: ");
                string s = Console.ReadLine();
                //Console.Write("|" + s + "|");
                if (s.Length == 0)
                {
                    s = "../../../../working_result.txt";
                }
                Console.Write("Please inpet path to result file: ");
                string sr = Console.ReadLine();
                if(sr.Length == 0)
                {
                    sr = "../../../../result.csv";
                }
                DataAnalysis.DataWriter.AnalizeAndWrite(s, sr);
            }
            catch(Exception e)
            {
                Console.WriteLine("Program ends with some problem: " + e.Message + "\n\n\tAdditional information:\n" + e.ToString() + "\n");
            }
        }
    }
}
