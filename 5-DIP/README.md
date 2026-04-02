# D — Dependency Inversion Principle (DIP)

> ***"High-level modules should not depend on low-level modules. Both should depend on abstractions. Abstractions should not depend on details. Details should depend on abstractions."***
> — Robert C. Martin

---

## 📖 What Is DIP?

DIP is the most architecturally significant SOLID principle. It flips the traditional dependency direction:

**Traditional:** Business Logic → Database, Email, File System (high depends on low)

**With DIP:** Business Logic → Interface ← Database, Email, File System (both depend on abstraction)

The high-level policy (your business rules) should NEVER know about the low-level details (which database you use, which email provider, which file system). Instead, both depend on an interface that sits between them.

---

## 🧠 Deep Dive: Why "Inversion"?

In traditional layered architecture, dependencies flow downward:

```
Controller → Service → Repository → Database
```

Every layer knows about the layer below it. But this creates a problem: **your business logic is chained to your infrastructure.**

DIP **inverts** this by saying: the Service layer **defines** the interface (what it needs), and the Repository **implements** it (how to provide it):

```
Controller → Service → IRepository ← SqlRepository
                                    ← MongoRepository
                                    ← MockRepository (for tests!)
```

The Service doesn't reach down to the database. The database reaches UP to the Service's interface.

---

## 📁 Files in This Directory

| File | Description |
|------|-------------|
| [bad-example.cs](./bad-example.cs) | ❌ Direct dependencies on concrete implementations |
| [good-example.cs](./good-example.cs) | ✅ Dependency injection through abstractions |
| [real-world-scenario.cs](./real-world-scenario.cs) | 🏗️ A full layered architecture with DIP applied |

---

## ⚠️ DIP vs Dependency Injection (DI)

These are **NOT the same thing!**

| | DIP | DI |
|---|-----|-----|
| **What** | A principle (design philosophy) | A technique (implementation pattern) |
| **Says** | "Depend on abstractions" | "Pass dependencies through constructors" |
| **Level** | Architectural | Code-level |
| **Can exist without** | Can be achieved without DI containers | Makes no sense without DIP thinking |

**DI is ONE WAY to achieve DIP.** You can also use:
- Service Locator pattern
- Factory pattern
- Plugin architecture

---

## 🔍 How to Spot DIP Violations

### Red Flags 🚩

| Smell | Why It's Bad |
|-------|-------------|
| `new ConcreteService()` inside business logic | Direct dependency on implementation |
| `using System.Data.SqlClient` in domain layer | Infrastructure leak into business logic |
| Can't unit test without a real database | Business logic is chained to infrastructure |
| Changing email provider requires editing 20 files | Tight coupling everywhere |
| Static method calls: `Database.Save(...)` | Untestable, inflexible |

### The Test: "Can I swap this out?"

Ask: *"If I wanted to switch from SQL Server to MongoDB, how many files would I need to change?"*

- **DIP Applied:** Change 1 file (the DI configuration) ✅
- **DIP Violated:** Change every file that uses the database ❌

---

## 🔌 The Three Layers of a Clean Architecture

```
┌─────────────────────────────────────┐
│         PRESENTATION LAYER          │  ← Controllers, UI
│         (depends on interfaces)     │
├─────────────────────────────────────┤
│         BUSINESS / DOMAIN LAYER     │  ← Core logic + Interface definitions
│         (defines interfaces)        │  ← OWNS the abstractions
├─────────────────────────────────────┤
│         INFRASTRUCTURE LAYER        │  ← Database, Email, APIs
│         (implements interfaces)     │  ← Details live here
└─────────────────────────────────────┘

Dependencies flow INWARD (toward the domain).
The domain layer depends on NOTHING external.
```

---

## 💡 Real-World Benefits

| Benefit | Example |
|---------|---------|
| **Testability** | Mock `IEmailService` in tests — no real emails sent |
| **Flexibility** | Swap `SqlRepository` for `MongoRepository` in config |
| **Parallel development** | Team A builds business logic while Team B builds database |
| **Plugin architecture** | Third parties can implement your interfaces |
| **Environment parity** | Use `InMemoryCache` in dev, `RedisCache` in production |

---

## 🎯 Key Takeaways

1. **High-level modules define interfaces**, low-level modules implement them
2. **Both layers depend on the abstraction**, not on each other
3. **Constructor injection** is the most common DIP technique
4. **The domain layer owns the interfaces** — infrastructure adapts to them
5. **Not the same as DI** — DIP is the principle, DI is a technique
6. **Makes testing trivial** — inject mocks instead of real infrastructure
