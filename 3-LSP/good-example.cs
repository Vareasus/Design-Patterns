// ✅ GOOD EXAMPLE — Following Liskov Substitution Principle
// All subtypes can be substituted for their base type without breaking anything.

using System;
using System.Collections.Generic;

namespace LSP.Good
{
    // ══════════════════════════════════════════════════════
    // FIX 1: Rectangle-Square — Use a shared interface
    // Don't force "is-a" inheritance when behavior differs.
    // ══════════════════════════════════════════════════════

    public interface IShape
    {
        double CalculateArea();
        string Describe();
    }

    // Rectangle has its own behavior — width and height are independent
    public class Rectangle : IShape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public double CalculateArea() => Width * Height;

        public string Describe()
            => $"Rectangle ({Width} × {Height}), Area = {CalculateArea()}";
    }

    // Square has its own behavior — one side, no confusion
    public class Square : IShape
    {
        public double Side { get; set; }

        public double CalculateArea() => Side * Side;

        public string Describe()
            => $"Square ({Side} × {Side}), Area = {CalculateArea()}";
    }

    // Circle — works perfectly through the same interface
    public class Circle : IShape
    {
        public double Radius { get; set; }

        public double CalculateArea() => Math.PI * Radius * Radius;

        public string Describe()
            => $"Circle (r = {Radius}), Area = {CalculateArea():F2}";
    }

    // ══════════════════════════════════════════════════════
    // FIX 2: Bird-Penguin — Separate WHAT birds can do
    // Not all birds fly. Model capabilities, not taxonomy.
    // ══════════════════════════════════════════════════════

    // Base: All birds can do these things
    public interface IBird
    {
        string Name { get; }
        void MakeSound();
        void Move(); // All birds can move (walk, swim, or fly)
    }

    // Capability: Only some birds can fly
    public interface IFlyable
    {
        void Fly();
        double MaxAltitudeFeet { get; }
    }

    // Capability: Only some birds can swim
    public interface ISwimmable
    {
        void Swim();
        double MaxDepthMeters { get; }
    }

    // Eagle: Can fly ✅, Can't swim ❌
    public class Eagle : IBird, IFlyable
    {
        public string Name => "Golden Eagle";

        public void MakeSound()
            => Console.WriteLine($"  🦅 {Name} screeches!");

        public void Move()
            => Console.WriteLine($"  🦅 {Name} glides through the air.");

        public void Fly()
            => Console.WriteLine($"  🦅 {Name} soars at {MaxAltitudeFeet:N0} feet!");

        public double MaxAltitudeFeet => 10000;
    }

    // Penguin: Can't fly ❌, Can swim ✅
    public class Penguin : IBird, ISwimmable
    {
        public string Name => "Emperor Penguin";

        public void MakeSound()
            => Console.WriteLine($"  🐧 {Name} honks!");

        public void Move()
            => Console.WriteLine($"  🐧 {Name} waddles along the ice.");

        public void Swim()
            => Console.WriteLine($"  🐧 {Name} dives to {MaxDepthMeters}m depth!");

        public double MaxDepthMeters => 500;
    }

    // Duck: Can fly ✅ AND swim ✅
    public class Duck : IBird, IFlyable, ISwimmable
    {
        public string Name => "Mallard Duck";

        public void MakeSound()
            => Console.WriteLine($"  🦆 {Name} quacks!");

        public void Move()
            => Console.WriteLine($"  🦆 {Name} waddles or flies.");

        public void Fly()
            => Console.WriteLine($"  🦆 {Name} flies at {MaxAltitudeFeet:N0} feet!");

        public double MaxAltitudeFeet => 4000;

        public void Swim()
            => Console.WriteLine($"  🦆 {Name} paddles on the pond.");

        public double MaxDepthMeters => 2;
    }

    // ══════════════════════════════════════════════════════
    // FIX 3: Collections — Proper hierarchy
    // ReadOnlyCollection is NOT a mutable collection.
    // They share a read contract, not a write contract.
    // ══════════════════════════════════════════════════════

    // Read-only contract — safe for everyone
    public interface IReadableCollection<T>
    {
        int Count { get; }
        T Get(int index);
        bool Contains(T item);
    }

    // Write contract — extends read contract
    public interface IWritableCollection<T> : IReadableCollection<T>
    {
        void Add(T item);
        void Remove(T item);
    }

    // Mutable collection implements BOTH read and write
    public class MutableCollection<T> : IWritableCollection<T>
    {
        private readonly List<T> _items = new();

        public int Count => _items.Count;
        public T Get(int index) => _items[index];
        public bool Contains(T item) => _items.Contains(item);

        public void Add(T item)
        {
            _items.Add(item);
            Console.WriteLine($"  ✅ Added item. Count: {Count}");
        }

        public void Remove(T item) => _items.Remove(item);
    }

    // Read-only collection only promises READ — no broken contracts!
    public class ReadOnlyCollection<T> : IReadableCollection<T>
    {
        private readonly List<T> _items;

        public ReadOnlyCollection(IEnumerable<T> items)
        {
            _items = new List<T>(items);
        }

        public int Count => _items.Count;
        public T Get(int index) => _items[index];
        public bool Contains(T item) => _items.Contains(item);

        // No Add() or Remove() — they were never promised!
    }

    // ══════════════════════════════════════════════════════
    // DEMONSTRATION — Everything works perfectly!
    // ══════════════════════════════════════════════════════

    class Program
    {
        // Works with ANY shape — no surprises
        static void PrintShapeArea(IShape shape)
        {
            Console.WriteLine($"  {shape.Describe()}");
        }

        // Works with ANY bird — no exceptions
        static void InteractWithBird(IBird bird)
        {
            bird.MakeSound();
            bird.Move();

            // Check capabilities safely — no forced behavior
            if (bird is IFlyable flyer)
                flyer.Fly();

            if (bird is ISwimmable swimmer)
                swimmer.Swim();
        }

        // Works with any READABLE collection — never throws
        static void PrintCollection(IReadableCollection<string> collection)
        {
            Console.WriteLine($"  Collection has {collection.Count} items:");
            for (var i = 0; i < collection.Count; i++)
                Console.WriteLine($"    [{i}] {collection.Get(i)}");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║   LSP CORRECT — Everything Works!       ║");
            Console.WriteLine("╚══════════════════════════════════════════╝\n");

            // Test 1: All shapes work through IShape
            Console.WriteLine("── Shapes (all substitutable via IShape) ──");
            var shapes = new List<IShape>
            {
                new Rectangle { Width = 5, Height = 10 },
                new Square { Side = 7 },
                new Circle { Radius = 4 }
            };

            foreach (var shape in shapes)
                PrintShapeArea(shape); // ✅ No surprises!

            // Test 2: All birds work through IBird
            Console.WriteLine("\n── Birds (all substitutable via IBird) ──");
            var birds = new List<IBird>
            {
                new Eagle(),
                new Penguin(),
                new Duck()
            };

            foreach (var bird in birds)
            {
                Console.WriteLine($"\n  ── {bird.Name} ──");
                InteractWithBird(bird); // ✅ No exceptions!
            }

            // Test 3: Collections
            Console.WriteLine("\n── Collections ──");

            var mutable = new MutableCollection<string>();
            mutable.Add("Hello");
            mutable.Add("World");
            PrintCollection(mutable); // ✅ Works!

            var readOnly = new ReadOnlyCollection<string>(new[] { "A", "B", "C" });
            PrintCollection(readOnly); // ✅ Works! No Add() to break.

            Console.WriteLine("\n✨ Every substitution worked perfectly.");
            Console.WriteLine("✨ No exceptions. No surprises. That's LSP.");
        }
    }
}
