using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSTestExtractFailedTestNames
{
    class Program
    {
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
            Environment.SetEnvironmentVariable("ListOfFailedTestsToReRun", listOfFailedTests);




            // otherwise we need to find list of failed tests names and send it back as env param
            Console.WriteLine("Setting ListOfFailedTestsToReRun variable to: " + "Quick1_Test");
            Console.WriteLine("##teamcity[setParameter name='env.ListOfFailedTestsToReRun' value='Quick1_Test']");
            Environment.SetEnvironmentVariable("ListOfFailedTestsToReRun", "GamEffective.QuickTests.Quick1_Test");

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

            return parseTRXFile(myFile);
        }

        private static string parseTRXFile(FileInfo myFile)
        {
            string fileContent = File.ReadAllText(myFile.FullName);

            return fileContent;

        }

        private static string readFailedTestsFromTRXFile(string latestTRXFileName)
        {
            throw new NotImplementedException();
        }

        private static void showErrorAndFinish(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(-1);
        }
    }
}