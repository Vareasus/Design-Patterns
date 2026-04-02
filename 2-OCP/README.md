# O — Open/Closed Principle (OCP)

> ***"Software entities (classes, modules, functions) should be open for extension, but closed for modification."***
> — Bertrand Meyer, 1988

---

## 📖 What Is OCP?

The Open/Closed Principle states that you should be able to **add new functionality** to a system **without changing the existing code** that already works.

- **Open for extension** → You can add new behavior
- **Closed for modification** → You don't touch existing, tested code

This sounds paradoxical at first. How do you add behavior without changing code? The answer: **abstractions** (interfaces, abstract classes, polymorphism).

---

## 🧠 Deep Dive: Why Does Modification Hurt?

Every time you modify existing code:

1. **You risk introducing bugs** in code that was previously working
2. **You have to re-test** everything that class touches
3. **You create merge conflicts** if others depend on the same file
4. **You break the Open/Closed contract** — clients trusted that class wouldn't change

### The If-Else Death Spiral

The classic OCP violation is the growing `if-else` or `switch` statement:

```csharp
// Monday: Supports 2 payment types. Clean!
if (type == "CreditCard") { ... }
else if (type == "PayPal") { ... }

// Wednesday: Boss says "add Bitcoin"
else if (type == "Bitcoin") { ... }

// Friday: "We need Apple Pay too"
else if (type == "ApplePay") { ... }

// Next month: 15 payment types, 200-line method, bugs everywhere
```

Every new payment type **modifies** the existing method. OCP says: **extend** instead.

---

## 📁 Files in This Directory

| File | Description |
|------|-------------|
| [bad-example.cs](./bad-example.cs) | ❌ Growing switch statement that violates OCP |
| [good-example.cs](./good-example.cs) | ✅ Polymorphism-based approach following OCP |
| [real-world-scenario.cs](./real-world-scenario.cs) | 🏗️ A notification system with pluggable channels |

---

## 🔑 How to Achieve OCP

### Strategy 1: Polymorphism (Most Common)
Define an interface → implement it for each variant → pass the interface around.

### Strategy 2: Strategy Pattern
Inject behavior through function delegates or strategy objects.

### Strategy 3: Decorator Pattern
Wrap existing behavior with additional behavior, without modifying the original.

### Strategy 4: Plugin Architecture
Load new behaviors at runtime from external modules.

---

## 🔍 How to Spot OCP Violations

### Red Flags 🚩

| Smell | Why It's Bad |
|-------|-------------|
| `switch` on a type/enum that grows | Every new type = modify this method |
| `if (obj is TypeA) ... else if (obj is TypeB)` | Type checking = missed polymorphism |
| "Add feature X" requires editing 5 existing files | The system isn't extensible |
| Comments like `// TODO: add case for new type` | You KNOW it'll need modification |

---

## 🤔 Common Misconceptions

### "OCP means I should never modify any code"
**No.** Bug fixes, refactoring, and changing business rules are valid reasons to modify code. OCP is about **new features/extensions** — those shouldn't require modifying *other people's* working code.

### "OCP means I need interfaces everywhere"
**No.** Premature abstraction is a code smell too. Apply OCP when you have **evidence** that something will need to be extended. If your app only ever has one payment method, you don't need an `IPaymentProcessor` interface.

### "OCP is only about classes"
**No.** OCP applies to functions, modules, packages, and even whole systems. A well-designed REST API is "closed" (existing endpoints don't change) but "open" (you can add new endpoints).

---

## 💡 Design Patterns That Implement OCP

| Pattern | How It Uses OCP |
|---------|----------------|
| **Strategy** | Swap algorithms without modifying the context |
| **Decorator** | Add behavior by wrapping, not modifying |
| **Factory Method** | Create new types without changing creation logic |
| **Observer** | Add new subscribers without modifying the publisher |
| **Template Method** | Override steps in a process without changing the process |

---

## 🎯 Key Takeaways

1. **Extend, don't modify** — add new classes/implementations instead of editing existing ones
2. **Use interfaces** (abstractions) as the extension point
3. **The if-else/switch smell** is often a sign you need polymorphism
4. **Don't over-apply** — only create abstractions when you have evidence of needed extensibility
5. **OCP + SRP together** create naturally extensible, focused code
