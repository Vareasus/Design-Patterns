# Strategy Pattern

> ***"Define a family of algorithms, encapsulate each one, and make them interchangeable."***
> — Gang of Four

---

## 📖 What Is the Strategy Pattern?

The Strategy Pattern lets you **swap algorithms at runtime** without changing the code that uses them. Instead of hardcoding behavior in if-else chains, you define a family of algorithms, put each in its own class, and let the client choose.

### 🎮 The Game Controller Analogy

A game console works with **any controller** — wired, wireless, fight stick, racing wheel. The console doesn't care which one you're using. It knows the **interface**: buttons, joystick, triggers. Plug in any controller and play.

---

## 📁 Files

| File | Description |
|------|-------------|
| [bad-example.cs](./bad-example.cs) | ❌ Hardcoded if-else for different sorting algorithms |
| [good-example.cs](./good-example.cs) | ✅ Strategy pattern with swappable algorithms |

---

## 🔗 SOLID Connection

- **OCP**: New strategies = new classes, zero changes to context
- **DIP**: Context depends on interface, not concrete strategies
- **SRP**: Each strategy has one job — one algorithm
