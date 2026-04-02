# Factory Method Pattern

> ***"Define an interface for creating an object, but let subclasses decide which class to instantiate."***
> — Gang of Four

---

## 📖 What Is the Factory Pattern?

The Factory Pattern **delegates object creation** to a method or class. Instead of using `new ConcreteClass()` everywhere, you ask a factory to create the right object based on some input.

### 🏭 The Restaurant Kitchen Analogy

You tell the waiter "I want a burger." The **kitchen** (factory) decides how to make it — which ingredients, which cooking method, which chef. You don't go into the kitchen and make it yourself.

---

## 📁 Files

| File | Description |
|------|-------------|
| [bad-example.cs](./bad-example.cs) | ❌ Direct `new` calls with switch statements |
| [good-example.cs](./good-example.cs) | ✅ Factory method creating objects through abstraction |

---

## 🔗 SOLID Connection

- **OCP**: New product types = new factory, no changes to client
- **DIP**: Client depends on abstract product, not concrete classes
