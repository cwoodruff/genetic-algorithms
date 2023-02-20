namespace genetic_algorithm_ts;

public abstract class Program
{
    public static Random? r { get; private set; }

    public static void Main()
    {
        r = new Random();
        
        Tour dest = Tour.random(Env.numCities);
        Population p = Population.randomized(dest, Env.popSize);

        int gen = 0;
        bool better = true;

        while (gen < Env.maxGen)
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
        Tour? best = p.findBest();
        Console.WriteLine("Generation {0}\n" + "Best fitness:  {1}\n" + "Best distance: {2}\n", gen, best!.fitness, best.distance);
    }
}

public static class Env
{
    public const double mutRate = 0.03;
    public const int elitism = 6;
    public const int popSize = 90;
    public const int numCities = 120;
    public const int maxGen = 10000;
}