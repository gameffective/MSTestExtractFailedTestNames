using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSTestExtractFailedTestNames
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            // read param value
            string runOnlyFailedTests = Environment.GetEnvironmentVariable("RunOnlyFailedTests");
            if (runOnlyFailedTests == null)
            {
                Console.WriteLine("Environment variable 'runOnlyFailedTests' not found");
                Environment.Exit(-1);
            }
            Console.WriteLine("RunOnlyFailedTests variable value is: " + runOnlyFailedTests);

            // if no need to run only failed tests - terminate now
            if (!runOnlyFailedTests.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("As not asked to run only failed tests, terminating now..");
                Environment.Exit(0);
            }

            // otherwise we need to find list of failed tests names and send it back as env param
            Console.WriteLine("Setting ListOfFailedTestsToReRun variable to: " + "GamEffective.QuickTests.Quick1_Test");
            Environment.SetEnvironmentVariable("ListOfFailedTestsToReRun", "GamEffective.QuickTests.Quick1_Test");

                /*string latestTRXFileName = findLatestTRXFile();
                string listOfFailedTests = readFailedTestsFromTRXFile(latestTRXFileName);
                Environment.SetEnvironmentVariable("ListOfFailedTestsToReRun", listOfFailedTests);*/
            
            
        }

        private static string findLatestTRXFile()
        {
            throw new NotImplementedException();
        }

        private static string readFailedTestsFromTRXFile(string latestTRXFileName)
        {
            throw new NotImplementedException();
        }
    }
}