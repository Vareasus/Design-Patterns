// Observer Design Pattern - Behavioral Category
// Source: salihcantekin/youtube_DesignPatterns_Builder

class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}

interface IObserver
{
    string FullName { get; set; }
    void Notify(Product product);
}

// === Concrete Observers ===

class SalihObserver : IObserver
{
    public string FullName { get; set; }

    public SalihObserver(string fullName)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
    }

    public void Notify(Product product)
    {
        Console.WriteLine($"{FullName}, Product {product.Name} in stock now!");
    }
}

class CantekinObserver : IObserver
{
    public string FullName { get; set; }

    public CantekinObserver(string fullName)
    {
        FullName = fullName;
    }

    public void Notify(Product product)
    {
        Console.WriteLine($"{FullName}, Product {product.Name} in stock now!");
    }
}

// === Subject (Observable) ===

class Amazon
{
    private Dictionary<IObserver, Product> observers = new();

    public void Register(IObserver observer, Product product)
    {
        observers.TryAdd(observer, product);
    }

    public void UnRegister(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyAll()
    {
        foreach (var kv in observers)
        {
            kv.Key.Notify(kv.Value);
        }
    }

    public void NotifyForProductName(string productName)
    {
        foreach (var kv in observers)
        {
            if (kv.Value.Name == productName)
                kv.Key.Notify(kv.Value);
        }
    }
}

// === Usage ===
// var samsung = new Product("Samsung S23", 1000);
// var amazon = new Amazon();
// var salih = new SalihObserver("Salih Cantekin");
// amazon.Register(salih, samsung);
// amazon.NotifyAll();
//
// Yeni observer eklemek? Yeni class yaz, Register et. Amazon class'ı değişmez.
