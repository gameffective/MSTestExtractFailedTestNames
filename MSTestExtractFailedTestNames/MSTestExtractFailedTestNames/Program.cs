using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MSTestExtractFailedTestNames
{
    class Program
    {
        private const string SEARCH_REGEX = "<UnitTestResult.*testName=\"([^\\\"]*)\".*outcome=\"Failed\".*\">";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            // read param value
            string runOnlyFailedTests = Environment.GetEnvironmentVariable("RunOnlyFailedTests");
            if (runOnlyFailedTests == null)
            {
                showErrorAndFinish("Environment variable 'runOnlyFailedTests' not found");
            }
            Console.WriteLine("RunOnlyFailedTests variable value is: " + runOnlyFailedTests);

            // if no need to run only failed tests - terminate now
            if (!runOnlyFailedTests.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("As not asked to run only failed tests, terminating now..");
                Environment.Exit(0);
            }
                       
            // get TRX files directory path
            if (args.Length != 1)
            {
                showErrorAndFinish("Error: missing input argument of TRX files folder path");
            }

            string folderPath = args[0];
            string latestTRXFileName = findLatestTRXFile(folderPath);
            string listOfFailedTests = readFailedTestsFromTRXFile(latestTRXFileName);

            // output is sent via environment variable as well as console output for teamCity
            Console.WriteLine("Setting ListOfFailedTestsToReRun variable to: " + listOfFailedTests);
            Console.WriteLine("##teamcity[setParameter name='env.ListOfFailedTestsToReRun' value='" + listOfFailedTests + "']");
            Environment.SetEnvironmentVariable("ListOfFailedTestsToReRun", listOfFailedTests);

            Console.WriteLine("Done!");
        }

        private static string findLatestTRXFile(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                showErrorAndFinish("Provided folder path for TRX result files wasn't found: " + folderPath);
            }

            // find latest file
            var directory = new DirectoryInfo(folderPath);
            var myFile = directory.GetFiles()
             .OrderByDescending(f => f.LastWriteTime)
             .First();

            if (myFile == null)
            {
                showErrorAndFinish("No TRX results file found in folder " + folderPath);
            }

            Console.WriteLine("Working on latest TRX file: " + myFile.FullName);
            return myFile.FullName;
        }

        private static string readFailedTestsFromTRXFile(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);
            StringBuilder result = new StringBuilder();

            foreach (Match m in Regex.Matches(fileContent, SEARCH_REGEX))
            {
                // extract test name from current match - m.Groups[1].Value
                // in order to remove additional problematic text of (Data Row 0) - .Split(' ')[0]
                string testName = m.Groups[1].Value.Split(' ')[0];

                Console.WriteLine("'{0}' found at index {1}.",
                                 testName, m.Index);

                result.Append("|Name=" + testName);
            }
            
            return result.ToString();
        }


        private static void showErrorAndFinish(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(-1);
        }
    }
}