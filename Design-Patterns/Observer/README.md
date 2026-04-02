# Observer Pattern

> ***"Define a one-to-many dependency so that when one object changes state, all its dependents are notified."***
> — Gang of Four

---

## 📖 What Is the Observer Pattern?

The Observer Pattern creates a **subscription mechanism** — objects (observers) register to receive updates from another object (subject). When the subject's state changes, all observers are notified automatically.

### 📰 The Newsletter Analogy

You **subscribe** to a newsletter. When a new article is published, everyone who subscribed gets notified. You don't have to keep checking the website. And you can **unsubscribe** anytime.

---

## 📁 Files

| File | Description |
|------|-------------|
| [bad-example.cs](./bad-example.cs) | ❌ Direct coupling between event source and handlers |
| [good-example.cs](./good-example.cs) | ✅ Observer pattern with decoupled publishers/subscribers |

---

## 🔗 SOLID Connection

- **SRP**: Subject handles state, observers handle reactions — separate responsibilities
- **OCP**: Add new observers without modifying the subject
- **DIP**: Subject depends on observer interface, not concrete observers
