// Factory Method Design Pattern - Creational Category
// Source: salihcantekin/youtube_DesignPatterns_Builder

interface IPizza
{
    void Prepare();
    void Bake();
    void Cut();
}

// === Concrete Products ===

class CheesePizza : IPizza
{
    public void Prepare() => Console.WriteLine("Cheese Pizza Prepared");
    public void Bake() => Console.WriteLine("Cheese Pizza Baked");
    public void Cut() => Console.WriteLine("Cheese Pizza Cut");
}

class VeggiePizza : IPizza
{
    public void Prepare() => Console.WriteLine("Veggie Pizza Prepared");
    public void Bake() => Console.WriteLine("Veggie Pizza Baked");
    public void Cut() => Console.WriteLine("Veggie Pizza Cut");
}

// === Creator (Abstract Factory) ===

abstract class PizzaStore
{
    protected abstract IPizza CreatePizza(string type);

    public IPizza OrderPizza(string type)
    {
        IPizza pizza = CreatePizza(type);

        pizza.Prepare();
        pizza.Bake();
        pizza.Cut();

        return pizza;
    }
}

// === Concrete Creators ===

class AnkaraPizzaStore : PizzaStore
{
    protected override IPizza CreatePizza(string type)
    {
        return type switch
        {
            "cheese" => new CheesePizza(),
            "veggie" => new VeggiePizza(),
            _ => throw new ArgumentException("Invalid pizza type", nameof(type))
        };
    }
}

class IstanbulPizzaStore : PizzaStore
{
    protected override IPizza CreatePizza(string type)
    {
        return type switch
        {
            "cheese" => new CheesePizza(),
            "veggie" => new VeggiePizza(),
            _ => throw new ArgumentException("Invalid pizza type", nameof(type))
        };
    }
}

// === Usage ===
// PizzaStore store = new AnkaraPizzaStore();
// IPizza pizza = store.OrderPizza("cheese");
//
// Yeni pizza türü? Yeni class yaz. Yeni şehir? Yeni store yaz.
// Hiçbir mevcut kod değişmez → OCP
