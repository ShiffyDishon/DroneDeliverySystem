using System;

namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome4228();
            Welcome1070();

        }
        static partial void Welcome4228();
        private static void Welcome1070()
        {
            Console.WriteLine("Enter your name: ");
            string userName = Console.ReadLine();
            Console.WriteLine($"{ userName}, welcome to my first console application");
            Console.ReadLine();
        }

    }
}
