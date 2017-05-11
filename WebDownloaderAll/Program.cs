using System;
using WebDownloaderAll.Common;

namespace WebDownloaderAll
{
    internal static class Program
    {
        private static void Main() {

            //Console.BackgroundColor = ConsoleColor.White;
            //Console.ForegroundColor = ConsoleColor.Black;
            var root = new Root();
            while (root.DoStackCall()) {
                Console.WriteLine(Resource.enterToContinue);
                Console.ReadLine();
                Console.Clear();
            }
            Console.Clear();
            Console.WriteLine(Resource.longProcess);
            Console.ReadLine();
        }
    }
}
