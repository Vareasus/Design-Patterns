# I — Interface Segregation Principle (ISP)

> ***"No client should be forced to depend on methods it does not use."***
> — Robert C. Martin

---

## 📖 What Is ISP?

ISP is about **keeping interfaces small, focused, and relevant**. Instead of creating one massive interface that tries to cover everything, break it into smaller interfaces that each serve a specific client need.

Think of it this way: **An interface is a contract.** If you force someone to sign a contract with 50 clauses but they only need 3, that's bad design. Give them a contract with just the 3 clauses they need.

---

## 🧠 Deep Dive: Fat Interfaces Kill Flexibility

### What's a "Fat Interface"?

A fat interface is one that has too many methods — methods that not all implementors actually need.

```csharp
// This interface FORCES every implementor to deal with 8 methods
public interface IMultiFunctionDevice
{
    void Print(Document doc);
    void Scan(Document doc);
    void Fax(Document doc);
    void Staple(Document doc);
    void PhotoCopy(Document doc);
    void Email(Document doc);
    void SaveToDisk(Document doc);
    void OCR(Document doc);
}
```

A simple printer that just prints? It has to implement `Fax()`, `Staple()`, `OCR()`, and 5 other methods it can't actually do. That's ISP violation.

### The Ripple Effect

When a fat interface changes (adds a method, changes a signature), **every single implementor** must be updated — even if the change doesn't apply to them. With 20 implementors, that's 20 files to change for one new method.

---

## 📁 Files in This Directory

| File | Description |
|------|-------------|
| [bad-example.cs](./bad-example.cs) | ❌ Fat interface forcing unnecessary implementations |
| [good-example.cs](./good-example.cs) | ✅ Small, focused interfaces that clients can pick from |
| [real-world-scenario.cs](./real-world-scenario.cs) | 🏗️ A real-world user management system with role-based interfaces |

---

## 🔍 How to Spot ISP Violations

### Red Flags 🚩

| Smell | Why It's Bad |
|-------|-------------|
| `throw new NotImplementedException()` | Class can't fulfill the interface contract |
| Empty method bodies | Silent LSP + ISP violation |
| Interface with 10+ methods | Probably too fat |
| Interface name is too generic | `IManager`, `IHandler`, `IProcessor` |
| Class implements interface but only uses 2 of 8 methods | 6 methods are dead weight |

### The Dependency Test

Ask: *"If I add a method to this interface, how many implementors would need to change?"*

- If the answer is "all of them need it" → Interface is fine ✅
- If the answer is "only some need it" → Split the interface 🔪

---

## ⚖️ ISP vs SRP

They're related but different:

| | SRP | ISP |
|---|-----|-----|
| **Focus** | Class responsibilities | Interface size |
| **Question** | "How many reasons does this class have to change?" | "Are clients forced to depend on methods they don't use?" |
| **Solution** | Split class into focused classes | Split interface into focused interfaces |

A class can follow SRP but still implement a fat interface (ISP violation). They complement each other.

---

## 💡 Benefits of Small Interfaces

1. **Easier to implement** — New classes only implement what they need
2. **Easier to test** — Mock only the methods you care about
3. **Less coupling** — Clients depend on fewer methods
4. **Better documentation** — Interface name tells you exactly what it does
5. **Easier to compose** — Mix and match interfaces for different needs

---

## 🎯 Key Takeaways

1. **Small interfaces > Fat interfaces** — always
2. **Clients dictate interface design** — what does the CLIENT need?
3. **It's fine for a class to implement multiple interfaces** — that's the point
4. **Watch for `NotImplementedException`** — it's a giant red flag
5. **Interface names should be specific** — `IPrintable`, not `IEverything`
