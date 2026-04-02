# Decorator Pattern

> ***"Attach additional responsibilities to an object dynamically. Decorators provide a flexible alternative to subclassing."***
> — Gang of Four

---

## 📖 What Is the Decorator Pattern?

The Decorator Pattern lets you **wrap objects** to add new behavior without modifying the original class. Each decorator adds one layer of functionality, and they can be **stacked**.

### ☕ The Coffee Shop Analogy

Start with a **basic coffee** ($3). Add **milk** (+$0.50). Add **whipped cream** (+$0.70). Add **caramel** (+$0.60). Each addition *wraps* the previous one. You can combine them in any way.

---

## 📁 Files

| File | Description |
|------|-------------|
| [bad-example.cs](./bad-example.cs) | ❌ Class explosion with inheritance |
| [good-example.cs](./good-example.cs) | ✅ Decorator pattern with stackable wrappers |

---

## 🔗 SOLID Connection

- **OCP**: Add behavior by wrapping, not modifying original
- **SRP**: Each decorator adds exactly one responsibility
