// ❌ BAD EXAMPLE — Violating Liskov Substitution Principle
// Subtypes that CAN'T truly replace their parent types.

using System;
using System.Collections.Generic;

namespace LSP.Bad
{
    // ══════════════════════════════════════════════════════
    // VIOLATION 1: The Classic Rectangle-Square Problem
    // ══════════════════════════════════════════════════════

    public class Rectangle
    {
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public int CalculateArea() => Width * Height;
    }

    // 💥 Square "IS-A" Rectangle mathematically, but NOT in OOP
    public class Square : Rectangle
    {
        // Overriding to maintain the square invariant
        // This CHANGES the expected behavior of the parent!
        public override int Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                base.Height = value; // 💥 Side effect! Parent doesn't do this.
            }
        }

        public override int Height
        {
            get => base.Height;
            set
            {
                base.Height = value;
                base.Width = value; // 💥 Side effect! Parent doesn't do this.
            }
        }
    }

    // ══════════════════════════════════════════════════════
    // VIOLATION 2: The Bird-Penguin Problem
    // ══════════════════════════════════════════════════════

    public class Bird
    {
        public string Name { get; set; }

        public virtual void Fly()
        {
            Console.WriteLine($"  {Name} is flying through the sky! 🦅");
        }

        public virtual void MakeSound()
        {
            Console.WriteLine($"  {Name} makes a sound.");
        }
    }

    public class Eagle : Bird
    {
        public override void Fly()
        {
            Console.WriteLine($"  {Name} soars majestically at 10,000 feet! 🦅");
        }

        public override void MakeSound()
        {
            Console.WriteLine($"  {Name} screeches! 🗣️");
        }
    }

    // 💥 Penguins can't fly! This violates LSP.
    public class Penguin : Bird
    {
        public override void Fly()
        {
            // Option A: Throw exception — breaks callers
            throw new NotSupportedException($"{Name} can't fly! I'm a penguin! 🐧");

            // Option B: Do nothing — silently violates the contract
            // (commented out, but equally bad)
            // Console.WriteLine($"  {Name} flaps uselessly...");
        }

        public override void MakeSound()
        {
            Console.WriteLine($"  {Name} honks! 🐧");
        }
    }

    // 💥 Ostriches can't fly either!
    public class Ostrich : Bird
    {
        public override void Fly()
        {
            throw new NotSupportedException($"{Name} can't fly! Too heavy! 🦤");
        }
    }

    // ══════════════════════════════════════════════════════
    // VIOLATION 3: ReadOnlyCollection extending Collection
    // ══════════════════════════════════════════════════════

    public class MyCollection<T>
    {
        protected List<T> Items = new();

        public virtual void Add(T item)
        {
            Items.Add(item);
        }

        public virtual void Remove(T item)
        {
            Items.Remove(item);
        }

        public int Count => Items.Count;

        public T Get(int index) => Items[index];
    }

    // 💥 A ReadOnlyCollection that extends a mutable Collection
    public class MyReadOnlyCollection<T> : MyCollection<T>
    {
        public MyReadOnlyCollection(IEnumerable<T> items)
        {
            Items.AddRange(items);
        }

        public override void Add(T item)
        {
            throw new NotSupportedException("Cannot add to a read-only collection! 💥");
        }

        public override void Remove(T item)
        {
            throw new NotSupportedException("Cannot remove from a read-only collection! 💥");
        }
    }

    // ══════════════════════════════════════════════════════
    // DEMONSTRATION — Everything breaks!
    // ══════════════════════════════════════════════════════

    class Program
    {
        // This method expects a Rectangle. It should work with ANY Rectangle.
        static void TestRectangle(Rectangle rect)
        {
            rect.Width = 5;
            rect.Height = 10;

            var expectedArea = 50; // 5 × 10 = 50
            var actualArea = rect.CalculateArea();

            Console.WriteLine($"  Expected area: {expectedArea}");
            Console.WriteLine($"  Actual area:   {actualArea}");

            if (actualArea != expectedArea)
                Console.WriteLine("  💥 LSP VIOLATED! Substituting Square for Rectangle broke the math!\n");
            else
                Console.WriteLine("  ✅ Working correctly.\n");
        }

        // This method expects a Bird. It should work with ANY Bird.
        static void MakeBirdFly(Bird bird)
        {
            try
            {
                bird.Fly();
                Console.WriteLine($"  ✅ {bird.Name} flew successfully.\n");
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"  💥 LSP VIOLATED! {ex.Message}\n");
            }
        }

        // This method expects a Collection. It should work with ANY Collection.
        static void AddItemToCollection(MyCollection<string> collection, string item)
        {
            try
            {
                collection.Add(item);
                Console.WriteLine($"  ✅ Added '{item}'. Count: {collection.Count}\n");
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"  💥 LSP VIOLATED! {ex.Message}\n");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║   LSP VIOLATIONS — Everything Breaks!   ║");
            Console.WriteLine("╚══════════════════════════════════════════╝\n");

            // Test 1: Rectangle vs Square
            Console.WriteLine("── Test 1: Rectangle ──");
            TestRectangle(new Rectangle());

            Console.WriteLine("── Test 1: Square (substituted) ──");
            TestRectangle(new Square()); // 💥 BREAKS!

            // Test 2: Birds
            Console.WriteLine("── Test 2: Eagle ──");
            MakeBirdFly(new Eagle { Name = "Golden Eagle" });

            Console.WriteLine("── Test 2: Penguin (substituted) ──");
            MakeBirdFly(new Penguin { Name = "Emperor Penguin" }); // 💥 BREAKS!

            Console.WriteLine("── Test 2: Ostrich (substituted) ──");
            MakeBirdFly(new Ostrich { Name = "African Ostrich" }); // 💥 BREAKS!

            // Test 3: Collections
            Console.WriteLine("── Test 3: Regular Collection ──");
            AddItemToCollection(new MyCollection<string>(), "Hello");

            Console.WriteLine("── Test 3: ReadOnlyCollection (substituted) ──");
            AddItemToCollection(
                new MyReadOnlyCollection<string>(new[] { "existing" }),
                "new item"); // 💥 BREAKS!
        }
    }
}
