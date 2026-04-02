// Decorator Design Pattern - Structural Category
// Source: salihcantekin/youtube_DesignPatterns_Builder

// === Base Component (1950s) ===
public class Computer
{
    public void Start()
    {
        Console.WriteLine($"{GetType().Name} is starting");
    }

    public void ShutDown()
    {
        Console.WriteLine($"{GetType().Name} is shutting down");
    }
}

// === Concrete Component (1970s) ===
public class Laptop : Computer
{
    public void OpenLid()
    {
        Console.WriteLine($"{GetType().Name}'s lid is opening");
    }

    public void CloseLid()
    {
        Console.WriteLine($"{GetType().Name}'s lid is closing");
    }
}

// === Decorator (1990s) ===
public class LaptopDecorator : Laptop
{
    public virtual void OpenLid()      // virtual → alt class'lar override edebilir
    {
        // do something before
        base.OpenLid();
    }

    public virtual void CloseLid()     // virtual → alt class'lar override edebilir
    {
        base.CloseLid();
        // do something after
    }
}

// === Concrete Decorators ===
public class DellLaptop : LaptopDecorator
{
    public override void CloseLid()
    {
        base.CloseLid();
        Console.WriteLine("Dell Laptop is sleeping");   // Davranış EKLİYOR
    }

    public override void OpenLid()
    {
        Console.WriteLine("Dell Laptop is waking up");  // Davranış EKLİYOR
        base.OpenLid();
    }
}

public class AppleLaptop : Laptop
{
    // Apple kendi davranışını eklememiş → base davranış aynı kalır
}

// === Usage ===
// AppleLaptop al = new AppleLaptop();
// al.CloseLid();  // → "AppleLaptop's lid is closing"
//
// DellLaptop dl = new DellLaptop();
// dl.CloseLid();  // → "DellLaptop's lid is closing"
//                 //   "Dell Laptop is sleeping"  ← EK DAVRANIŞ
//
// virtual sayesinde DellLaptop, CloseLid'i override ederek
// base davranışa yeni özellik ekledi. Bu Decorator pattern'dir.
