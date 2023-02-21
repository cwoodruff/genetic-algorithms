namespace genetic_algorithm_ts;

public class Population
{
    // Member variables
    private List<Route?> p { get; set; }
    public double maxFit { get; private set; }

    // ctor
    private Population(List<Route?> l)
    {
        this.p = l;
        this.maxFit = this.CalcMaxFit();
    }

    // Functionality
    public static Population Randomized(Route t, int n)
    {
        List<Route?> tmp = new List<Route?>();

        for (int i = 0; i < n; ++i)
            tmp.Add(t.Shuffle());

        return new Population(tmp);
    }

    private double CalcMaxFit()
    {
        return this.p.Max(t => t!.fitness);
    }

    private Route Select()
    {
        while (true)
        {
            int i = Program.rdm!.Next(0, Env.PopSize);

            if (Program.rdm.NextDouble() < this.p[i]!.fitness / this.maxFit)
                return new Route(this.p[i]!.t);
        }
    }

    private Population GenNewPop(int n)
    {
        List<Route?> tours = new List<Route?>();

        for (int i = 0; i < n; ++i)
        {
            Route t = this.Select().Crossover(this.Select());

            foreach (City unused in t.t)
                t = t.Mutate();

            tours.Add(t);
        }

        return new Population(tours);
    }

    private Population Elite(int n)
    {
        List<Route?> best = new List<Route?>();
        Population tmp = new Population(p);

        for (int i = 0; i < n; ++i)
        {
            best.Add(tmp.FindBest());
            tmp = new Population(tmp.p.Except(best).ToList());
        }

        return new Population(best);
    }

    public Route? FindBest()
    {
        foreach (Route? t in this.p)
        {
            double TOLERANCE = 0.000000001;
            if (Math.Abs(t!.fitness - this.maxFit) < TOLERANCE)
                return t;
        }

        return null;
    }

    public Population Evolve()
    {
        Population best = this.Elite(Env.Elitism);
        Population np = this.GenNewPop(Env.PopSize - Env.Elitism);
        return new Population(best.p.Concat(np.p).ToList());
    }
}