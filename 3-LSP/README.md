# L — Liskov Substitution Principle (LSP)

> ***"If S is a subtype of T, then objects of type T may be replaced with objects of type S without altering any of the desirable properties of the program."***
> — Barbara Liskov, 1987

---

## 📖 What Is LSP?

LSP is the most misunderstood SOLID principle. It's NOT just about "subclasses should work like their parent class." It's about **behavioral compatibility**.

When you create a subclass, you're making a **promise**: *"Anywhere the parent is used, I can be used too — and nothing will break."*

If your subclass violates that promise, your inheritance hierarchy is **lying**.

---

## 🧠 Deep Dive: The Contract

Every class has an implicit **contract** — a set of expectations:

1. **Preconditions**: What must be true BEFORE calling a method
2. **Postconditions**: What will be true AFTER calling a method
3. **Invariants**: What's ALWAYS true about the object

LSP says: **A subclass must not strengthen preconditions, must not weaken postconditions, and must preserve invariants.**

### What This Means in Plain English

| Rule | Parent Says | Subclass Can... | Subclass Cannot... |
|------|-------------|-----------------|-------------------|
| Preconditions | "Pass me any positive number" | Accept zero too (weaker) | Require only even numbers (stronger) |
| Postconditions | "I'll return a non-null string" | Also guarantee non-empty | Return null sometimes |
| Invariants | "Balance is always ≥ 0" | Keep balance ≥ 0 | Allow negative balance |

---

## 📁 Files in This Directory

| File | Description |
|------|-------------|
| [bad-example.cs](./bad-example.cs) | ❌ Classic Rectangle-Square problem + Bird example |
| [good-example.cs](./good-example.cs) | ✅ Interface-based approach following LSP |
| [real-world-scenario.cs](./real-world-scenario.cs) | 🏗️ Payment processing with proper substitutability |

---

## ❌ Classic Violations

### The Rectangle-Square Problem
A square IS a rectangle mathematically. But in OOP, making `Square` extend `Rectangle` violates LSP because setting width independently of height is part of Rectangle's contract.

### The Bird-Penguin Problem
A penguin IS a bird. But if `Bird` has a `Fly()` method, `Penguin` can't fulfill that contract.

### The ReadOnlyList Problem
If `ReadOnlyList` extends `List`, calling `Add()` throws an exception. The substitution breaks.

---

## 🔍 How to Spot LSP Violations

### Red Flags 🚩

| Smell | Example |
|-------|---------|
| `throw new NotImplementedException()` in a subclass | Subclass can't fulfill parent's contract |
| `throw new NotSupportedException()` in override | Same — breaking the contract |
| Type checks: `if (obj is SpecificType)` | Caller needs to know the concrete type |
| Overridden method does nothing (empty body) | Silent contract violation |
| Subclass changes fundamental behavior of parent | Square setting both dimensions |

### The Substitution Test

```
Can I replace the parent with this subclass in ALL scenarios 
and have EVERY test still pass?

YES → LSP is satisfied ✅
NO  → LSP is violated ❌
```

---

## 💡 Guidelines

1. **Favor composition over inheritance** when "is-a" doesn't truly hold
2. **Use interfaces** to define what objects CAN do, not what they ARE
3. **Think in behaviors**, not hierarchies
4. **The "Tell, Don't Ask" principle** helps — if you need type checks, your hierarchy is wrong

---

## 🎯 Key Takeaways

1. **LSP = behavioral substitutability**, not just signature compatibility
2. **Don't inherit** if the subclass can't honor the parent's full contract
3. **Prefer interfaces** over deep inheritance hierarchies
4. **Watch for:** `NotImplementedException`, empty overrides, type checks
5. **Test it:** Can you swap in the subclass and have all tests pass?
