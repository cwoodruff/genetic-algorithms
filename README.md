# Genetic Algorithms: Another AI Path

**Building Evolutionary Optimization Solutions in C# and .NET**

This repository contains the slides, sample code, and supporting material for my conference talk *"Genetic Algorithms: Another AI Path"* — a practical introduction to evolutionary computing for .NET developers, software architects, and anyone curious about the branches of AI that don't involve a large language model.

> **Evolve better solutions.** We only define what a good answer looks like — evolution discovers how to build one.

## What's in This Repo

| Path | Contents |
|---|---|
| `Another AI Path Building Genetic Algorithms in CSharp and .NET.pptx` | The full slide deck, including speaker notes |
| `genetic-algorithms/` | Core genetic algorithm library code (C# / .NET) |
| `examples/` | Sample implementations demonstrating GA concepts |

## About the Talk

Optimization is the process of finding the best solution among many possible solutions — and the search space is usually enormous. Routing, scheduling, resource allocation, manufacturing, network design, and machine-learning tuning are all optimization problems, and simple hill-climbing search gets trapped on local optima long before it finds the global one.

Genetic algorithms take a different path: they borrow Darwin's ideas of natural selection and adaptation (formalized as an algorithm by John Holland in 1975) and use them as a search strategy. Instead of improving one candidate solution, a GA evolves a whole *population* of them.

### The Genetic Algorithm Lifecycle

```
Create Population → Evaluate Fitness → Selection → Crossover → Mutation → New Generation
        ↑                                                                      │
        └────────────────────── repeat until converged ───────────────────────┘
```

- **Population** — a group of candidate solutions competing to survive
- **Fitness** — every candidate is scored; the score *is* the goal
- **Selection** — the fittest earn the right to become parents (e.g., roulette-wheel, rank, or tournament selection)
- **Crossover** — two parents combine genes so good building blocks meet in one offspring
- **Mutation** — a low rate of random change keeps the population exploring and escaping local optima

The talk builds the intuition with biology (DNA → gene → chromosome), a "natural selection in three beats" beetle example, and a 60-second warm-up GA that maximizes *f(x) = 120x − x²* with a 7-bit encoding — small enough to verify by hand (the population converges on `0111100`, which is 60).

### Why Reach for a GA

- **No gradient required** — the fitness function can be a black box, a simulation, or a legacy cost engine
- **Good answers early** — usable solutions appear within a few generations and keep improving
- **A set, not a single answer** — the final population holds several near-optimal options to choose between
- **Embarrassingly parallel** — fitness evaluation fans out across cores or machines
- **Escapes local optima** — mutation plus population diversity beats hill-climbing's biggest weakness
- **Cheap to retrofit** — if you already compute a cost, you already have most of a GA

### And the Honest Tradeoffs

- **Fitness function design** is the hard intellectual work — a flawed fitness function evolves the wrong thing *beautifully*
- **Computational cost** — thousands of evaluations per run means fitness must be fast or parallel
- **Convergence challenges** — populations can converge prematurely on mediocre solutions

The talk closes the theory section with six design questions to answer before writing any code (encoding, fitness, selection method, operators, stopping criteria, and baseline) and the tuning knobs that make results reproducible: population size, generation cap, selection method, crossover rate (~0.6–0.9), mutation rate (kept deliberately low), and elitism count.

### Proof It Works in the Wild

Genetic algorithms have produced published, peer-reviewed results where the search space defeats hand-tuning — most famously the NASA ST5 evolved antenna, a bent-wire design no human would sketch that beat the hand-built alternative and flew to space in 2006. Other examples covered in the talk include recovering galaxy orbital parameters, constraining the dark-energy equation of state from supernova data, fitting stellar dust spectra, and tuning binary-star light-curve models — plus the everyday workhorses: fleet routing, crew rostering, and timetabling.

## Case Study: AirFreightRouter ✈️

The talk's running case study is **[AirFreightRouter](https://github.com/cwoodruff/AirFreightRouter)** (SkyRoute Express) — a C# / .NET desktop application that computes the shortest round-trip air-freight delivery route from its Albany, NY home base through a set of user-selected delivery cities: a classic traveling-salesman problem wearing an air-freight uniform.

The concept mapping is direct — this is where theory becomes software:

| Genetic Algorithm | Routing Equivalent |
|---|---|
| Gene | Airport |
| Chromosome | Route |
| Population | Candidate routes |
| Fitness function | Route score (cost, time, constraints) |
| Selection | Best routes survive |
| Mutation | Route variation |

The AirFreightRouter repo also supplies the crucial ingredient the talk insists on: **a baseline**. It implements a guaranteed brute-force permutation search (Heap's algorithm, with background execution, cooperative cancellation, and an interactive animated route map in WPF), which finds the provably optimal tour — but at *n!* cost, which is why it warns you beyond 12 cities (6.2 billion permutations). That exact-search wall is exactly the motivation for evolutionary search: a GA delivers near-optimal routes across far larger city sets in a tiny fraction of the evaluations, and you can measure it honestly against the brute-force optimum on small instances.

**Explore the project:** https://github.com/cwoodruff/AirFreightRouter — .NET 8, C#, WPF (MVVM via CommunityToolkit.Mvvm), xUnit test suite, CSV city import (`City,State,Latitude,Longitude`). Clone it, load a city file, and experiment.

## Getting Started

```bash
git clone https://github.com/cwoodruff/genetic-algorithms.git
cd genetic-algorithms
dotnet build
```

Requires the [.NET SDK](https://dotnet.microsoft.com/download). Start with the projects in `examples/` to see the lifecycle in action, then read through the core library in `genetic-algorithms/`.

## Going Deeper

**Books**

- *Genetic Algorithms in Search, Optimization & Machine Learning* — David E. Goldberg (the theory)
- *An Introduction to Genetic Algorithms* — Melanie Mitchell (the intuition)
- *Practical Genetic Algorithms* — Randy & Sue Ellen Haupt (the practice)

**Projects**

- [GeneticSharp](https://github.com/giacomelli/GeneticSharp) — mature open-source GA library for .NET
- [AirFreightRouter](https://github.com/cwoodruff/AirFreightRouter) — this talk's case study; clone it and experiment

## About the Speaker

**Chris Woodruff** — developer, architect, speaker, and AI & .NET enthusiast.

- 🌐 [woodruff.dev](https://woodruff.dev)
- 🐙 [github.com/cwoodruff](https://github.com/cwoodruff)
- ✉️ chris@woodruff.dev

## License

This project is licensed under the [MIT License](LICENSE).
