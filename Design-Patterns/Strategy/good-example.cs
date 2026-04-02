// ✅ GOOD — Strategy Pattern: Swap algorithms at runtime
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategy.Good
{
    // The Strategy interface
    public interface ISortStrategy
    {
        string Name { get; }
        List<int> Sort(List<int> data);
    }

    // Concrete strategies
    public class BubbleSort : ISortStrategy
    {
        public string Name => "Bubble Sort";
        public List<int> Sort(List<int> data)
        {
            var arr = new List<int>(data);
            for (int i = 0; i < arr.Count - 1; i++)
                for (int j = 0; j < arr.Count - i - 1; j++)
                    if (arr[j] > arr[j + 1])
                        (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
            return arr;
        }
    }

    public class QuickSort : ISortStrategy
    {
        public string Name => "Quick Sort";
        public List<int> Sort(List<int> data)
        {
            var arr = new List<int>(data);
            arr.Sort();
            return arr;
        }
    }

    public class InsertionSort : ISortStrategy
    {
        public string Name => "Insertion Sort";
        public List<int> Sort(List<int> data)
        {
            var arr = new List<int>(data);
            for (int i = 1; i < arr.Count; i++)
            {
                int key = arr[i], j = i - 1;
                while (j >= 0 && arr[j] > key) { arr[j + 1] = arr[j]; j--; }
                arr[j + 1] = key;
            }
            return arr;
        }
    }

    // Context: uses strategy without knowing which algorithm
    public class SortingService
    {
        private ISortStrategy _strategy;

        public SortingService(ISortStrategy strategy) { _strategy = strategy; }

        // Swap strategy at runtime!
        public void SetStrategy(ISortStrategy strategy) { _strategy = strategy; }

        public List<int> Sort(List<int> data)
        {
            Console.WriteLine($"  📊 Sorting with {_strategy.Name}...");
            return _strategy.Sort(data);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var data = new List<int> { 64, 25, 12, 22, 11 };
            Console.WriteLine("Original: " + string.Join(", ", data));

            var service = new SortingService(new BubbleSort());
            Console.WriteLine("Result:   " + string.Join(", ", service.Sort(data)));

            // Swap at runtime!
            service.SetStrategy(new QuickSort());
            Console.WriteLine("Result:   " + string.Join(", ", service.Sort(data)));

            service.SetStrategy(new InsertionSort());
            Console.WriteLine("Result:   " + string.Join(", ", service.Sort(data)));

            Console.WriteLine("\n✨ Same SortingService, 3 different algorithms, zero code changes.");
        }
    }
}
