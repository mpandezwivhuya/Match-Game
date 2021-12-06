using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MatchString
{
    class Program
    {
        static void Main(string[] args)
        {

            // enter first string
            string fstring = "";
            string str1 = "";
            while (fstring != null)
            {
                Console.WriteLine("Enter first Name1:");
                str1 = Console.ReadLine();
                if (!Regex.IsMatch(str1, @"^[a-zA-Z]+$"))
                {
                    Console.WriteLine("Invalid input, please enter the correct name");
                    continue;
                }
                else
                {
                    fstring = null;
                }

            }

            string lstring = "";
            string str2 = "";
            while (lstring != null)
            {
                Console.WriteLine("Enter first Name2:");
                str2 = Console.ReadLine();
                if (!Regex.IsMatch(str2, @"^[a-zA-Z]+$"))
                {
                    Console.WriteLine("Invalid input, please enter the correct name");
                    continue;
                }
                else
                {
                    lstring = null;
                }
            }

            int percentage = calculateMatchPercentage(str1, str2);  // calcutate and return match percentage
            string result = percentage >= 80 ? "Good match" : "Not good match";
            Console.WriteLine("[" + str1 + "," + " " + str2 + "]: " + percentage + "% :" + result);
            string pstring = "";
            string path = "";
            while (pstring != null)
            {
                Console.WriteLine("Enter csv file location:");
                path = Console.ReadLine(); // path to csv file
                try
                {
                    Dictionary<string, List<string>> dict = readCSV(path);

                    List<string[]> lines = runMatch(dict);
                    generateOutputFile(lines);
                    readOutputFile();
                    pstring = null;
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
        }

        static int calculateMatchPercentage(string str1, string str2)
        {
            // receive two strings, compute and return percentage 

            string smallStr; // hold string with lesser number of characters
            string bigStr; // hold string with higher number of characters

            // determine which string (between str1 and str2) is bigStr and smallStr
            if (str1.Length >= str2.Length)
            {
                bigStr = str1;
                smallStr = str2;
            }
            else
            {
                bigStr = str2;
                smallStr = str1;
            }
            int matches = 0;
            int i = 0;
            // compare characters from smaller string against characters from bigStr (this is indexed based)
            foreach (char str in smallStr)
            {
                if (char.ToUpperInvariant(str) == char.ToUpperInvariant(bigStr[i]))
                    matches++; // coout matches

                i++;
            }
            // compute avergae out of hundred
            double matchAverage = ((double)matches / (double)bigStr.Length) * 100;
            return (int)matchAverage;

        }

        static Dictionary<string, List<string>> readCSV(string path)
        {
            List<string> Males = new List<string>(); // store males
            List<string> Females = new List<string>(); // store females
            // read csv data and store it to into lists
            var reader = new StreamReader(File.OpenRead(path));

            while (!reader.EndOfStream)
            {
                // read cvs line by line
                var line = reader.ReadLine().Replace("\"", "").Trim(); // remove double quotes from line
                var lineArray = line.Split(','); // split line by comma
                string name = lineArray[0].Trim(); // get name
                string gender = lineArray[1].Trim(); // get indicator (f/m)
                if (gender.ToUpper() == "F" && !Females.Contains(name))
                {
                    // add to females list 
                    Females.Add(name);
                }
                else if (gender.ToUpper() == "M" && !Males.Contains(name))
                {
                    // add to males list 
                    Males.Add(name);
                }

            }
            Dictionary<string, List<string>> hash = new Dictionary<string, List<string>>();
            hash.Add("males", Males);
            hash.Add("females", Females);
            return hash; // return dictionary
        }
        static List<string[]> runMatch(Dictionary<string, List<string>> hash)
        {
            // compare names from the two lists (males and females)
            List<string> Males = hash["males"]; // read males list from dictionary
            List<string> Females = hash["females"]; // read females list from dictionary
            List<string[]> lines = new List<string[]>(); // store match results

            foreach (string male in Males)
            {
                foreach (string female in Females)
                {
                    int percentage = calculateMatchPercentage(male, female);
                    lines.Add(new string[] { male, female, percentage.ToString() });

                }
            }
            List<string[]> SortedLines = lines.OrderByDescending(x => Int32.Parse(x[2])).ToList(); // order results by percentage
            return SortedLines;

        }
        static void generateOutputFile(List<string[]> lines)
        {
            string path = @"output.txt";

            if (!File.Exists(path))
            {

                File.Create(path).Dispose();
                using (TextWriter tw = new StreamWriter(path))
                {
                    foreach (string[] line in lines)
                    {
                        string result = Int32.Parse(line[2]) >= 80 ? "Good match" : "Not good match";
                        tw.WriteLine("[" + line[0] + "," + " " + line[1] + "]: " + line[2] + "% :" + result);
                    }
                }

            }
            else if (File.Exists(path))
            {
                // clear file before writing to it
                File.WriteAllText(path, String.Empty);
                TextWriter tw = new StreamWriter(path, true);
                foreach (string[] line in lines)
                {
                    string result = Int32.Parse(line[2]) >= 80 ? "Good match" : "Not good match";
                    tw.WriteLine("[" + line[0] + "," + " " + line[1] + "]: " + line[2] + "% :" + result);
                }
                tw.Close();

            }


        }
        static void readOutputFile(string path = @"output.txt")
        {

            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(path))
            {
                string line;

                // Read and display lines from the file until 
                // the end of the file is reached. 
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }

        }
    }
}
