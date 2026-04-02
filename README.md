# 🏗️ SOLID Principles & Design Patterns — The Ultimate Guide

<div align="center">

![SOLID](https://img.shields.io/badge/SOLID-Principles-blueviolet?style=for-the-badge&logo=dotnet&logoColor=white)
![Design Patterns](https://img.shields.io/badge/Design-Patterns-ff6b6b?style=for-the-badge&logo=buffer&logoColor=white)
![C#](https://img.shields.io/badge/C%23-Examples-239120?style=for-the-badge&logo=csharp&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**The only SOLID & Design Patterns guide you'll ever need. Period.**

*If you can't explain it after reading this, the problem isn't the concept — it's every other guide you've read.*

🌐 [**Live Demo**](https://vareasus.github.io/Design-Patterns/) · [📖 Interactive Guide](./index.html) · [🧠 Take the Quiz](./quiz.html) · [🏗️ Design Patterns](./Design-Patterns/) · [🎯 Cheat Sheet](#-interview-cheat-sheet)

</div>

---

## 📑 Table of Contents

| # | Principle | One-Liner | Folder |
|---|-----------|-----------|--------|
| **S** | [Single Responsibility](#s---single-responsibility-principle-srp) | *"A class should have only ONE reason to change."* | [`/1-SRP`](./1-SRP/) |
| **O** | [Open/Closed](#o---openclosed-principle-ocp) | *"Open for extension, closed for modification."* | [`/2-OCP`](./2-OCP/) |
| **L** | [Liskov Substitution](#l---liskov-substitution-principle-lsp) | *"Subtypes must be substitutable for their base types."* | [`/3-LSP`](./3-LSP/) |
| **I** | [Interface Segregation](#i---interface-segregation-principle-isp) | *"No client should be forced to depend on methods it doesn't use."* | [`/4-ISP`](./4-ISP/) |
| **D** | [Dependency Inversion](#d---dependency-inversion-principle-dip) | *"Depend on abstractions, not concretions."* | [`/5-DIP`](./5-DIP/) |

---

## 🧠 Why SOLID Matters

Imagine you're building a house. You *could* wire the electricity directly into the walls with no junction boxes, glue the pipes together permanently, and nail everything shut. It'll work... **until something breaks.** Then you tear down the whole wall to fix a leaky pipe.

**SOLID principles are the junction boxes, the access panels, and the modular wiring of software.** They don't make your code work — your code already works. They make your code **survivable**.

### The Real-World Cost of Ignoring SOLID

```
Monday:    "Just add this one feature..." 
Tuesday:   "Why did that break 3 other things?"
Wednesday: "Who wrote this? Oh wait... I did."
Thursday:  "We need to rewrite everything."
Friday:    *Updates LinkedIn*
```

### Who Coined SOLID?

The SOLID principles were promoted by **Robert C. Martin** (Uncle Bob) in the early 2000s. The acronym itself was coined by **Michael Feathers**. These aren't arbitrary rules — they emerged from **decades of painful experience** in software engineering.

---

## S — Single Responsibility Principle (SRP)

> ***"A class should have one, and only one, reason to change."***
> — Robert C. Martin

### 🎯 The Core Idea

SRP doesn't mean a class should only do "one thing." It means a class should only be **responsible to one actor** — one stakeholder, one source of change.

### 🍕 The Pizza Analogy

Think of a pizzeria:

- The **chef** makes pizzas
- The **cashier** handles payments  
- The **delivery driver** delivers orders

Now imagine ONE person doing all three. When the menu changes, the chef-cashier-driver needs retraining. When tax laws change, the chef-cashier-driver needs retraining. When delivery routes change... you get it.

**Each role change affects the same person.** That's a violation of SRP.

### ❌ BAD — Violating SRP

```csharp
// This class has THREE reasons to change:
// 1. If invoice calculation logic changes
// 2. If printing format changes  
// 3. If database storage changes
public class Invoice
{
    public decimal Amount { get; set; }
    public string CustomerName { get; set; }
    public DateTime Date { get; set; }

    // Reason 1: Business logic
    public decimal CalculateTotal(decimal taxRate)
    {
        return Amount + (Amount * taxRate);
    }

    // Reason 2: Presentation logic
    public string PrintInvoice()
    {
        return $"Invoice for {CustomerName}\n" +
               $"Date: {Date}\n" +
               $"Total: ${CalculateTotal(0.18m)}";
    }

    // Reason 3: Persistence logic
    public void SaveToDatabase()
    {
        // Direct database access inside the model
        var connection = new SqlConnection("...");
        connection.Open();
        // INSERT INTO Invoices ...
    }
}
```

### ✅ GOOD — Following SRP

```csharp
// Each class has exactly ONE reason to change

// Only changes if the invoice DATA structure changes
public class Invoice
{
    public decimal Amount { get; set; }
    public string CustomerName { get; set; }
    public DateTime Date { get; set; }
}

// Only changes if CALCULATION rules change
public class InvoiceCalculator
{
    public decimal CalculateTotal(Invoice invoice, decimal taxRate)
    {
        return invoice.Amount + (invoice.Amount * taxRate);
    }
}

// Only changes if PRINT FORMAT changes
public class InvoicePrinter
{
    private readonly InvoiceCalculator _calculator;

    public InvoicePrinter(InvoiceCalculator calculator)
    {
        _calculator = calculator;
    }

    public string Print(Invoice invoice, decimal taxRate)
    {
        var total = _calculator.CalculateTotal(invoice, taxRate);
        return $"Invoice for {invoice.CustomerName}\n" +
               $"Date: {invoice.Date}\n" +
               $"Total: ${total}";
    }
}

// Only changes if STORAGE mechanism changes
public class InvoiceRepository
{
    public void Save(Invoice invoice)
    {
        // Database logic here
    }
}
```

### 🗣️ Interview Answer Template

> *"SRP states that a class should have only one reason to change, meaning it should be responsible to only one actor or stakeholder. For example, if I have an Invoice class that calculates totals, prints reports, AND saves to the database, then a change in tax calculation, print format, OR database schema would all require modifying the same class. Instead, I separate these into InvoiceCalculator, InvoicePrinter, and InvoiceRepository — each with a single, focused responsibility."*

📁 **[See full code examples →](./1-SRP/)**

---

## O — Open/Closed Principle (OCP)

> ***"Software entities should be open for extension, but closed for modification."***
> — Bertrand Meyer

### 🎯 The Core Idea

You should be able to **add new behavior** without **changing existing code**. When you need a new feature, you **extend** — you don't go back and **edit** what already works.

### 🔌 The Power Outlet Analogy

A wall power outlet is **closed for modification** (you don't rewire it every time you buy a new device) but **open for extension** (you can plug in ANY device that fits the socket). 

The outlet doesn't know or care if it's powering a lamp, a laptop, or an electric guitar amp. It just provides power through a standard interface.

### ❌ BAD — Violating OCP

```csharp
// Every time we add a new shape, we MODIFY this class
public class AreaCalculator
{
    public double CalculateArea(object shape)
    {
        if (shape is Circle circle)
        {
            return Math.PI * circle.Radius * circle.Radius;
        }
        else if (shape is Rectangle rectangle)
        {
            return rectangle.Width * rectangle.Height;
        }
        else if (shape is Triangle triangle)  // ← Had to MODIFY existing code!
        {
            return 0.5 * triangle.Base * triangle.Height;
        }
        // What about Pentagon? Hexagon? 
        // This method grows FOREVER.
        
        throw new ArgumentException("Unknown shape");
    }
}
```

### ✅ GOOD — Following OCP

```csharp
// The abstraction — our "power outlet"
public interface IShape
{
    double CalculateArea();
}

// Each shape EXTENDS the system without modifying existing code
public class Circle : IShape
{
    public double Radius { get; set; }
    
    public double CalculateArea()
        => Math.PI * Radius * Radius;
}

public class Rectangle : IShape
{
    public double Width { get; set; }
    public double Height { get; set; }
    
    public double CalculateArea()
        => Width * Height;
}

// NEW shape? Just ADD a new class. Zero changes to existing code!
public class Triangle : IShape
{
    public double Base { get; set; }
    public double Height { get; set; }
    
    public double CalculateArea()
        => 0.5 * Base * Height;
}

// This class NEVER needs to change
public class AreaCalculator
{
    public double CalculateTotalArea(IEnumerable<IShape> shapes)
    {
        return shapes.Sum(shape => shape.CalculateArea());
    }
}
```

### 🗣️ Interview Answer Template

> *"OCP means we should be able to extend a system's behavior without modifying its existing source code. I achieve this through abstractions like interfaces. For example, instead of a giant if-else chain in an AreaCalculator, I define an IShape interface with a CalculateArea method. Each shape implements this interface. When I need to add a Pentagon, I just create a new class — the AreaCalculator never changes. This makes the system extensible and dramatically reduces the risk of introducing bugs in working code."*

📁 **[See full code examples →](./2-OCP/)**

---

## L — Liskov Substitution Principle (LSP)

> ***"Objects of a superclass should be replaceable with objects of a subclass without affecting the correctness of the program."***
> — Barbara Liskov, 1987

### 🎯 The Core Idea

If class B extends class A, then you should be able to use B **everywhere** you use A without anything breaking. Subtypes must honor the **contract** of their parent type — not just the method signatures, but the **behavior**.

### 🦆 The Duck Analogy

> *"If it looks like a duck, swims like a duck, and quacks like a duck, then it probably is a duck."*

But what if you create a RubberDuck that extends Duck? It looks like a duck... but it can't fly, it can't quack (it squeaks), and it doesn't swim (it floats). **It violates the contract of what a Duck should do.** Code expecting a Duck will break when it gets a RubberDuck.

### ❌ BAD — Violating LSP (The Classic Rectangle-Square Problem)

```csharp
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }

    public int CalculateArea() => Width * Height;
}

// Mathematically, a square IS a rectangle. 
// But in OOP? This is DANGEROUS.
public class Square : Rectangle
{
    public override int Width
    {
        get => base.Width;
        set { base.Width = value; base.Height = value; } // Forces both!
    }

    public override int Height
    {
        get => base.Height;
        set { base.Height = value; base.Width = value; } // Forces both!
    }
}

// 💥 THIS BREAKS
public void TestArea(Rectangle rect)
{
    rect.Width = 5;
    rect.Height = 10;
    
    // We EXPECT area = 50... 
    Console.WriteLine(rect.CalculateArea()); 
    // But if rect is a Square, area = 100! (both became 10)
    // The substitution BROKE the expected behavior.
}
```

### ✅ GOOD — Following LSP

```csharp
// Don't force an "is-a" relationship. Use a common abstraction.
public interface IShape
{
    int CalculateArea();
}

public class Rectangle : IShape
{
    public int Width { get; set; }
    public int Height { get; set; }
    
    public int CalculateArea() => Width * Height;
}

public class Square : IShape
{
    public int Side { get; set; }
    
    public int CalculateArea() => Side * Side;
}

// Now both work correctly through the interface
public void PrintArea(IShape shape)
{
    Console.WriteLine($"Area: {shape.CalculateArea()}");
    // No surprises. Ever.
}
```

### 🔑 The LSP Checklist

When creating a subtype, ask yourself:
1. ✅ Does it honor all contracts of the parent? (preconditions, postconditions)
2. ✅ Does it NOT throw unexpected exceptions?
3. ✅ Can it truly be used as a drop-in replacement?
4. ✅ Does it NOT change the expected behavior of inherited methods?

If any answer is **NO** → Don't use inheritance. Use composition or a shared interface.

### 🗣️ Interview Answer Template

> *"LSP says that a subclass must be usable wherever its parent class is used without breaking the program. The classic example is Rectangle vs Square — mathematically a square is a rectangle, but in code, making Square extend Rectangle causes bugs because setting width independently of height violates the square's invariant. The fix is to use a shared interface like IShape instead of forced inheritance. The key test: can I swap the subclass in and have all existing tests still pass?"*

📁 **[See full code examples →](./3-LSP/)**

---

## I — Interface Segregation Principle (ISP)

> ***"No client should be forced to depend on methods it does not use."***
> — Robert C. Martin

### 🎯 The Core Idea

Don't create **fat** interfaces. If a class only needs 2 out of 10 methods, it shouldn't be forced to implement the other 8. Split large interfaces into smaller, focused ones.

### 🍽️ The Restaurant Menu Analogy

Imagine a restaurant that hands you a 200-page menu covering breakfast, lunch, dinner, desserts, drinks, catering, and party packages. You came in at noon for a sandwich. You don't need the party planner section.

**ISP says: give the lunch customer a lunch menu.** Small, focused, relevant.

### ❌ BAD — Violating ISP (Fat Interface)

```csharp
// This interface is a MONSTER
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
    void AttendMeeting();
    void WriteReport();
    void FixBugs();
}

// A robot worker has to implement eating and sleeping?!
public class RobotWorker : IWorker
{
    public void Work() { /* ✅ Sure */ }
    
    public void Eat() 
    { 
        throw new NotImplementedException(); // 💥 Robots don't eat!
    }
    
    public void Sleep() 
    { 
        throw new NotImplementedException(); // 💥 Robots don't sleep!
    }
    
    public void AttendMeeting() { /* ✅ Sure */ }
    
    public void WriteReport() 
    { 
        throw new NotImplementedException(); // 💥 This robot just builds cars
    }
    
    public void FixBugs() 
    { 
        throw new NotImplementedException(); // 💥 Not a software robot
    }
}
```

### ✅ GOOD — Following ISP

```csharp
// Small, focused interfaces
public interface IWorkable
{
    void Work();
}

public interface IFeedable
{
    void Eat();
}

public interface ISleepable
{
    void Sleep();
}

public interface IReportable
{
    void AttendMeeting();
    void WriteReport();
}

// Human implements what humans do
public class HumanWorker : IWorkable, IFeedable, ISleepable, IReportable
{
    public void Work() { Console.WriteLine("Working..."); }
    public void Eat() { Console.WriteLine("Eating lunch..."); }
    public void Sleep() { Console.WriteLine("Sleeping..."); }
    public void AttendMeeting() { Console.WriteLine("In a meeting..."); }
    public void WriteReport() { Console.WriteLine("Writing report..."); }
}

// Robot ONLY implements what it can actually do
public class RobotWorker : IWorkable
{
    public void Work() { Console.WriteLine("Assembling parts 24/7..."); }
    // No forced implementation of Eat, Sleep, etc.!
}
```

### 🗣️ Interview Answer Template

> *"ISP states that clients shouldn't be forced to depend on interfaces they don't use. Instead of one large IWorker interface with Work, Eat, Sleep, and WriteReport methods, I'd split it into focused interfaces: IWorkable, IFeedable, ISleepable, and IReportable. A RobotWorker only implements IWorkable — it's not forced to throw NotImplementedException for Eat and Sleep. This makes the code more maintainable and prevents classes from carrying dead weight."*

📁 **[See full code examples →](./4-ISP/)**

---

## D — Dependency Inversion Principle (DIP)

> ***"High-level modules should not depend on low-level modules. Both should depend on abstractions."***
> — Robert C. Martin

### 🎯 The Core Idea

Your business logic (high-level) should NOT directly reference your infrastructure code (low-level). Instead, both should depend on **interfaces** (abstractions). This inverts the traditional dependency direction.

### 🔌 The Charger Analogy

Your phone (high-level module) doesn't have a wire permanently soldered to a specific wall outlet. Instead:

- Your phone depends on a **USB-C port** (abstraction)
- Your charger provides a **USB-C plug** (implementation)
- You can swap chargers, use power banks, car chargers — anything that speaks USB-C

The phone doesn't know or care WHERE the power comes from. It just knows the interface.

### ❌ BAD — Violating DIP

```csharp
// High-level module DIRECTLY depends on low-level module
public class MySqlDatabase
{
    public void Save(string data)
    {
        Console.WriteLine($"Saving '{data}' to MySQL...");
    }
}

public class UserService
{
    // 💥 Tightly coupled to MySQL!
    private readonly MySqlDatabase _database = new MySqlDatabase();

    public void CreateUser(string name)
    {
        // Business logic...
        _database.Save(name);
        // Want to switch to PostgreSQL? You have to CHANGE this class.
        // Want to unit test? You're hitting a real database.
    }
}
```

### ✅ GOOD — Following DIP

```csharp
// The abstraction (interface) — owned by the HIGH-level layer
public interface IDatabase
{
    void Save(string data);
}

// Low-level module implements the abstraction
public class MySqlDatabase : IDatabase
{
    public void Save(string data)
    {
        Console.WriteLine($"Saving '{data}' to MySQL...");
    }
}

public class PostgreSqlDatabase : IDatabase
{
    public void Save(string data)
    {
        Console.WriteLine($"Saving '{data}' to PostgreSQL...");
    }
}

public class MongoDatabase : IDatabase
{
    public void Save(string data)
    {
        Console.WriteLine($"Saving '{data}' to MongoDB...");
    }
}

// High-level module depends on ABSTRACTION, not a concrete class
public class UserService
{
    private readonly IDatabase _database;

    // Inject the dependency — any IDatabase works!
    public UserService(IDatabase database)
    {
        _database = database;
    }

    public void CreateUser(string name)
    {
        // Business logic...
        _database.Save(name);
        // 🎉 No changes needed to switch databases!
        // 🎉 Easy to unit test with a mock!
    }
}

// Usage
var service1 = new UserService(new MySqlDatabase());
var service2 = new UserService(new PostgreSqlDatabase());
var service3 = new UserService(new MongoDatabase());
// Same UserService, different databases. Zero code changes.
```

### 🗣️ Interview Answer Template

> *"DIP says that high-level business logic shouldn't directly depend on low-level infrastructure details. Both should depend on abstractions. For example, instead of UserService directly creating a MySqlDatabase instance, I define an IDatabase interface. UserService receives an IDatabase through its constructor — it doesn't know or care if it's MySQL, PostgreSQL, or a mock for testing. This makes the system flexible, testable, and easy to evolve without touching business logic."*

📁 **[See full code examples →](./5-DIP/)**

---

## 🎯 Interview Cheat Sheet

### Quick-Fire Q&A

| Question | Answer |
|----------|--------|
| What does S in SOLID stand for? | **Single Responsibility** — a class should have only one reason to change |
| What does O stand for? | **Open/Closed** — open for extension, closed for modification |
| What does L stand for? | **Liskov Substitution** — subtypes must be substitutable for their base types |
| What does I stand for? | **Interface Segregation** — no client should depend on methods it doesn't use |
| What does D stand for? | **Dependency Inversion** — depend on abstractions, not concretions |

### The 30-Second Elevator Pitch

> *"SOLID is a set of five design principles that make software easier to understand, maintain, and extend. **S** keeps classes focused, **O** lets you add features without changing existing code, **L** ensures inheritance doesn't break things, **I** prevents bloated interfaces, and **D** decouples high-level logic from infrastructure details. Together, they produce code that's flexible, testable, and resilient to change."*

### Common Follow-Up Questions

**Q: "Can you over-apply SOLID?"**
> Yes! Over-engineering is a real risk. If you have a simple CRUD app, creating 5 interfaces and 10 classes for a single entity is overkill. SOLID is a **guideline**, not a religion. Apply it when complexity justifies it.

**Q: "Which principle is most important?"**
> They're all interconnected, but if I had to pick one: **Single Responsibility**. If you get SRP right, the others tend to follow naturally. A focused class is easier to extend (OCP), safer to substitute (LSP), produces leaner interfaces (ISP), and is naturally more decoupled (DIP).

**Q: "How does SOLID relate to Design Patterns?"**
> Design patterns are **implementations** of SOLID principles. Strategy Pattern = OCP + DIP. Observer Pattern = SRP + OCP. Factory Pattern = DIP. Understanding SOLID helps you understand **why** patterns exist, not just **how** to use them.

**Q: "What's the difference between DIP and Dependency Injection?"**
> DIP is a **principle** (depend on abstractions). Dependency Injection is a **technique** (passing dependencies through constructors/parameters). DI is one way to achieve DIP, but not the only way.

---

## 📂 Repository Structure

```
Design-Patterns/
├── README.md                    ← You are here
├── index.html                   ← Interactive visual guide
├── quiz.html                    ← SOLID & Patterns quiz
│
├── 1-SRP/                       ← Single Responsibility Principle
│   ├── README.md
│   ├── bad-example.cs
│   ├── good-example.cs
│   └── real-world-scenario.cs
│
├── 2-OCP/                       ← Open/Closed Principle
│   ├── README.md
│   ├── bad-example.cs
│   ├── good-example.cs
│   └── real-world-scenario.cs
│
├── 3-LSP/                       ← Liskov Substitution Principle
│   ├── README.md
│   ├── bad-example.cs
│   ├── good-example.cs
│   └── real-world-scenario.cs
│
├── 4-ISP/                       ← Interface Segregation Principle
│   ├── README.md
│   ├── bad-example.cs
│   ├── good-example.cs
│   └── real-world-scenario.cs
│
├── 5-DIP/                       ← Dependency Inversion Principle
│   ├── README.md
│   ├── bad-example.cs
│   ├── good-example.cs
│   └── real-world-scenario.cs
│
└── Design-Patterns/             ← GoF Design Patterns
    ├── README.md
    ├── Strategy/
    │   ├── bad-example.cs
    │   └── good-example.cs
    ├── Observer/
    │   ├── bad-example.cs
    │   └── good-example.cs
    ├── Factory/
    │   ├── bad-example.cs
    │   └── good-example.cs
    ├── Singleton/
    │   ├── bad-example.cs
    │   └── good-example.cs
    └── Decorator/
        ├── bad-example.cs
        └── good-example.cs
```

---

## 🔗 Additional Resources

- 📘 [Clean Architecture — Robert C. Martin](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164)
- 📘 [Design Patterns: Elements of Reusable OO Software — Gang of Four](https://www.amazon.com/Design-Patterns-Elements-Reusable-Object-Oriented/dp/0201633612)
- 🎥 [SOLID Principles in 100 Seconds — Fireship](https://www.youtube.com/watch?v=q1qKv5TBaOA)
- 📝 [Martin Fowler's Blog](https://martinfowler.com/)

---

## ⭐ Support

If this helped you nail that interview, give it a ⭐ and share it with someone who's struggling with SOLID.

---

<div align="center">

**Anıl Ceylan** · Software Engineer

[![GitHub](https://img.shields.io/badge/GitHub-Vareasus-181717?style=flat-square&logo=github)](https://github.com/Vareasus)

*"Any fool can write code that a computer can understand. Good programmers write code that humans can understand."* — Martin Fowler

© 2026 Anıl Ceylan. All rights reserved.

</div>
