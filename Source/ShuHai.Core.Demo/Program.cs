using System;
using System.Collections.Generic;

namespace ShuHai.Core.Demo
{
    static class Program
    {
        private static void Main(string[] args)
        {
            var demos = new List<Demo> { new DeadlineTimerDemo() };
            Console.WriteLine("Select a demo:");
            for (int i = 0; i < demos.Count; ++i)
                Console.WriteLine($"{i + 1}: {demos[i].Name}");

            var input = Console.ReadLine();
            if (!int.TryParse(input, out var n))
            {
                Console.WriteLine("Only number is allowed.");
                return;
            }

            var index = n - 1;
            if (!Index.IsValid(index, demos.Count))
            {
                Console.WriteLine("Invalid demo number.");
                return;
            }

            var demo = demos[index];
            Console.WriteLine($"---- Running demo {demo.Name} ----");
            demo.Run();
        }
    }
}
