# Singleton Pattern

> ***"Ensure a class has only one instance and provide a global point of access to it."***
> — Gang of Four

---

## 📖 What Is the Singleton Pattern?

Singleton ensures **only one instance** of a class exists throughout the application's lifetime. Every request for the object returns the same instance.

### 🏛️ The President Analogy

A country has only **one president** at a time. Everyone refers to the same person when they say "the president." You don't create a new president every time someone needs one.

---

## ⚠️ Warning

Singleton is the **most controversial** pattern. It's useful for:  
✅ Configuration, Logging, Connection Pools, Caches

But dangerous when overused:  
❌ Makes testing hard, creates hidden dependencies, can become a God Object

---

## 📁 Files

| File | Description |
|------|-------------|
| [bad-example.cs](./bad-example.cs) | ❌ Naive singleton — not thread-safe |
| [good-example.cs](./good-example.cs) | ✅ Thread-safe singleton + DI-friendly approach |
