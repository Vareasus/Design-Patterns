// ✅ GOOD — Decorator Pattern: stackable wrappers
using System;

namespace Decorator.Good
{
    // Base interface
    public interface IBeverage
    {
        string GetDescription();
        decimal GetCost();
    }

    // Base component
    public class Coffee : IBeverage
    {
        public string GetDescription() => "Coffee";
        public decimal GetCost() => 3.00m;
    }

    public class Tea : IBeverage
    {
        public string GetDescription() => "Tea";
        public decimal GetCost() => 2.50m;
    }

    // Base decorator
    public abstract class BeverageDecorator : IBeverage
    {
        protected readonly IBeverage _beverage;
        protected BeverageDecorator(IBeverage beverage) { _beverage = beverage; }
        public abstract string GetDescription();
        public abstract decimal GetCost();
    }

    // Concrete decorators — each adds ONE thing
    public class MilkDecorator : BeverageDecorator
    {
        public MilkDecorator(IBeverage beverage) : base(beverage) { }
        public override string GetDescription() => _beverage.GetDescription() + " + Milk";
        public override decimal GetCost() => _beverage.GetCost() + 0.50m;
    }

    public class SugarDecorator : BeverageDecorator
    {
        public SugarDecorator(IBeverage beverage) : base(beverage) { }
        public override string GetDescription() => _beverage.GetDescription() + " + Sugar";
        public override decimal GetCost() => _beverage.GetCost() + 0.20m;
    }

    public class WhippedCreamDecorator : BeverageDecorator
    {
        public WhippedCreamDecorator(IBeverage beverage) : base(beverage) { }
        public override string GetDescription() => _beverage.GetDescription() + " + Whipped Cream";
        public override decimal GetCost() => _beverage.GetCost() + 0.70m;
    }

    public class CaramelDecorator : BeverageDecorator
    {
        public CaramelDecorator(IBeverage beverage) : base(beverage) { }
        public override string GetDescription() => _beverage.GetDescription() + " + Caramel";
        public override decimal GetCost() => _beverage.GetCost() + 0.60m;
    }

    public class ExtraShotDecorator : BeverageDecorator
    {
        public ExtraShotDecorator(IBeverage beverage) : base(beverage) { }
        public override string GetDescription() => _beverage.GetDescription() + " + Extra Shot";
        public override decimal GetCost() => _beverage.GetCost() + 0.80m;
    }

    class Program
    {
        static void PrintOrder(IBeverage beverage)
        {
            Console.WriteLine($"  ☕ {beverage.GetDescription()}");
            Console.WriteLine($"     ${beverage.GetCost():F2}\n");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("☕ Coffee Shop — Decorator Pattern\n");

            // Simple coffee
            IBeverage order1 = new Coffee();
            PrintOrder(order1);

            // Coffee + milk + sugar (stack decorators!)
            IBeverage order2 = new SugarDecorator(new MilkDecorator(new Coffee()));
            PrintOrder(order2);

            // Full loaded: coffee + milk + whip + caramel + extra shot
            IBeverage order3 = new ExtraShotDecorator(
                new CaramelDecorator(
                    new WhippedCreamDecorator(
                        new MilkDecorator(new Coffee()))));
            PrintOrder(order3);

            // Works with tea too!
            IBeverage order4 = new MilkDecorator(new SugarDecorator(new Tea()));
            PrintOrder(order4);

            Console.WriteLine("✨ 5 decorators × 2 base drinks = infinite combinations.");
            Console.WriteLine("✨ No class explosion. Stack freely. Add new decorators anytime.");
        }
    }
}
