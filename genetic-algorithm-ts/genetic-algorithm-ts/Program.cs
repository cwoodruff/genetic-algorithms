namespace genetic_algorithm_ts;

public abstract class Program
{
    public static Random? rdm { get; private set; }

    public static void Main()
    {
        rdm = new Random();
        
        Route dest = Route.random(Env.NumCities);
        Population p = Population.randomized(dest, Env.PopSize);

        int gen = 0;
        bool better = true;

        while (gen < Env.MaxGen)
        {
            if (better)
                display(p, gen);

            better = false;
            double oldFit = p.maxFit;

            p = p.evolve();
            if (p.maxFit > oldFit)
                better = true;

            gen++;
        }
    }

    private static void display(Population p, int gen)
    {
        Route? best = p.findBest();
        Console.WriteLine("Generation {0}\n" + "Best fitness:  {1}\n" + "Shortest route: {2}\n", gen, best!.fitness, best.distance);
    }
}

public abstract record Env
{
    public const double MutRate = 0.04;
    public const int Elitism = 5;
    public const int PopSize = 100;
    public const int NumCities = 500;
    public const int MaxGen = 10000;
}