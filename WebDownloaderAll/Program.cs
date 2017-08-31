using System;
using System.Linq;
using WebDownloaderAll.Common;

namespace WebDownloaderAll
{
    internal static class Program
    {
        private static void Main(string[] commandLineParameters)
        {

            var root = new Root();

            AutoConfigProvider.ProcessConfig = commandLineParameters;

            if (AutoConfigProvider.AutoConfigDataSet != null && AutoConfigProvider.AutoConfigDataSet.Count() > 0)
            {
                root.DoStackCall();
            }
            else
            {
                while (root.DoStackCall())
                {
                    Console.WriteLine(Resource.enterToContinue);
                    Console.ReadLine();
                    Console.Clear();
                }
                Console.Clear();
                Console.WriteLine(Resource.longProcess);
                Console.ReadLine();
            }


            //Console.ReadLine();
            /*
            var root = new Root();
            while (root.DoStackCall()) {
                Console.WriteLine(Resource.enterToContinue);
                Console.ReadLine();
                Console.Clear();
            }
            Console.Clear();
            Console.WriteLine(Resource.longProcess);
            Console.ReadLine();
             */
        }
    }
}
