// ❌ BAD — Hardcoded sorting algorithms with if-else
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategy.Bad
{
    public class SortingService
    {
        // 💥 Every new algorithm = modify this method
        public List<int> Sort(List<int> data, string algorithm)
        {
            switch (algorithm)
            {
                case "bubble":
                    Console.WriteLine("  Using Bubble Sort...");
                    for (int i = 0; i < data.Count - 1; i++)
                        for (int j = 0; j < data.Count - i - 1; j++)
                            if (data[j] > data[j + 1])
                                (data[j], data[j + 1]) = (data[j + 1], data[j]);
                    return data;

                case "selection":
                    Console.WriteLine("  Using Selection Sort...");
                    for (int i = 0; i < data.Count - 1; i++)
                    {
                        int minIdx = i;
                        for (int j = i + 1; j < data.Count; j++)
                            if (data[j] < data[minIdx]) minIdx = j;
                        (data[i], data[minIdx]) = (data[minIdx], data[i]);
                    }
                    return data;

                case "quick":
                    Console.WriteLine("  Using Quick Sort...");
                    data.Sort(); // simplified
                    return data;

                // 💥 Want merge sort? Heap sort? Radix sort?
                // Keep modifying this method FOREVER.
                default:
                    throw new ArgumentException($"Unknown algorithm: {algorithm}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var service = new SortingService();
            var data = new List<int> { 64, 25, 12, 22, 11 };

            Console.WriteLine("Original: " + string.Join(", ", data));
            var sorted = service.Sort(new List<int>(data), "bubble");
            Console.WriteLine("Sorted:   " + string.Join(", ", sorted));

            Console.WriteLine("\n💥 Adding a new algorithm requires modifying SortingService.");
        }
    }
}
