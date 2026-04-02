// ❌ BAD — Class explosion with inheritance for every combination
using System;

namespace Decorator.Bad
{
    // 💥 Need a class for EVERY combination
    public class Coffee { public virtual string GetDescription() => "Coffee"; public virtual decimal GetCost() => 3.00m; }
    public class CoffeeWithMilk : Coffee { public override string GetDescription() => "Coffee + Milk"; public override decimal GetCost() => 3.50m; }
    public class CoffeeWithMilkAndSugar : Coffee { public override string GetDescription() => "Coffee + Milk + Sugar"; public override decimal GetCost() => 3.70m; }
    public class CoffeeWithMilkAndWhipAndCaramel : Coffee { public override string GetDescription() => "Coffee + Milk + Whip + Caramel"; public override decimal GetCost() => 4.80m; }
    // 💥 CoffeeWithSoyAndVanillaAndExtraShot...???
    // 💥 Combinatorial explosion! 50+ classes needed.

    class Program
    {
        static void Main(string[] args)
        {
            var order = new CoffeeWithMilkAndWhipAndCaramel();
            Console.WriteLine($"{order.GetDescription()} — ${order.GetCost()}");
            Console.WriteLine("\n💥 Need a new class for EVERY combination. Impossible to maintain.");
        }
    }
}
