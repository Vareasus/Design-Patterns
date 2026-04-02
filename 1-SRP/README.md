# S — Single Responsibility Principle (SRP)

> ***"A class should have one, and only one, reason to change."***
> — Robert C. Martin

---

## 📖 What Is SRP?

The Single Responsibility Principle is the first and arguably most foundational of the SOLID principles. It states that **every class, module, or function should have responsibility over a single part of the functionality** provided by the software.

But here's the key insight that most tutorials miss:

### SRP is NOT about doing "one thing"

A class that "calculates tax" does one thing, right? But what if it calculates income tax, sales tax, AND property tax for 5 different countries? That's "one thing" but it has **many reasons to change**.

**SRP is about having one reason to change — one actor, one stakeholder.**

---

## 🧠 Deep Dive: What Does "Reason to Change" Mean?

Robert C. Martin clarifies this in *Clean Architecture*:

> *"A module should be responsible to one, and only one, actor."*

An **actor** is a group of people who would request a change. Consider an `Employee` class:

```csharp
public class Employee
{
    public decimal CalculatePay()    { /* ... */ }  // CFO's team requests changes
    public string GenerateReport()   { /* ... */ }  // COO's team requests changes  
    public void SaveToDatabase()     { /* ... */ }  // CTO's team requests changes
}
```

Three different departments. Three different reasons to change. **Three responsibilities crammed into one class.**

When the CFO's team changes the pay calculation formula, there's a risk of accidentally breaking the COO's report or the CTO's database logic.

---

## 📁 Files in This Directory

| File | Description |
|------|-------------|
| [bad-example.cs](./bad-example.cs) | ❌ A single class doing too much — violating SRP |
| [good-example.cs](./good-example.cs) | ✅ Responsibilities separated into focused classes |
| [real-world-scenario.cs](./real-world-scenario.cs) | 🏗️ A production-realistic e-commerce order processing example |

---

## ❌ The Violation — What Goes Wrong

When you violate SRP, you get classes that are:

1. **Hard to test** — You can't test payment logic without setting up email and database dependencies
2. **Hard to understand** — A 500-line class doing 5 things is cognitive overload
3. **Fragile** — Changing one feature breaks another feature in the same class
4. **Hard to reuse** — You want just the email logic? Too bad, it comes bundled with payment and logging

### The Merge Conflict Nightmare

With SRP violations, **two developers working on different features** will constantly step on each other's toes because their changes touch the same file.

---

## ✅ The Solution — How to Apply SRP

### Step 1: Identify the Actors
Ask: "Who are the different people/teams that might request changes to this class?"

### Step 2: Group by Actor
Group methods by which actor would request changes to them.

### Step 3: Extract Classes
Create a new class for each group. Give it a name that clearly describes its single responsibility.

### Step 4: Compose
Use the extracted classes together through composition, not inheritance.

---

## 🔍 How to Spot SRP Violations

### Red Flags 🚩

| Smell | Example |
|-------|---------|
| Class name includes "And" | `UserValidatorAndSaver` |
| Class name includes "Manager" | `OrderManager` (manages what exactly?) |
| Class has 10+ methods | Probably doing too much |
| Class imports from many different domains | `using System.Net.Mail; using System.Data; using System.IO;` |
| Methods don't use the same fields | `Calculate()` uses `_price`, `Print()` uses `_printer` |
| Difficult to write a one-sentence description | "This class handles users and also sends emails and also..." |

### The Newspaper Test

Imagine describing the class to a non-technical person. If you can't do it in one sentence without using "and", it violates SRP.

- ✅ "InvoiceCalculator calculates invoice totals."
- ❌ "InvoiceManager calculates totals **and** prints invoices **and** saves to database."

---

## 🤔 Common Misconceptions

### "SRP means a class should have only one method"
**No.** A class can have many methods — they just need to serve the same responsibility. An `InvoiceCalculator` might have `CalculateSubtotal()`, `CalculateTax()`, `CalculateDiscount()`, and `CalculateTotal()`. All are part of "calculation."

### "SRP makes too many small classes"
You'll have more classes, yes. But each one is **simple, testable, and focused**. Would you rather read 5 files of 50 lines each, or 1 file of 500 lines that does 5 things?

### "SRP is just about separation of concerns"
They're related but different. Separation of concerns is about **architecture** (UI vs. business logic vs. data). SRP is about **who requests changes** to a specific class.

---

## 💡 Real-World Applications

| Domain | SRP Applied |
|--------|-------------|
| **E-commerce** | `Order`, `OrderValidator`, `OrderPricer`, `OrderRepository`, `OrderNotifier` |
| **Authentication** | `User`, `PasswordHasher`, `TokenGenerator`, `AuthenticationService` |
| **Logging** | `LogFormatter`, `FileLogWriter`, `ConsoleLogWriter`, `LogRouter` |
| **Payment** | `PaymentProcessor`, `PaymentValidator`, `PaymentGateway`, `PaymentReceipt` |

---

## 🎯 Key Takeaways

1. **SRP = one reason to change**, not "one thing to do"
2. **Identify actors** — who requests changes to this code?
3. **Extract responsibilities** into focused classes
4. **Compose** classes together instead of cramming everything into one
5. **Don't over-apply** — a `User` class with `FirstName` and `LastName` doesn't need to be split further
