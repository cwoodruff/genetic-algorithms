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
        this.maxFit = this.calcMaxFit();
    }

    // Functionality
    public static Population randomized(Route t, int n)
    {
        List<Route?> tmp = new List<Route?>();

        for (int i = 0; i < n; ++i)
            tmp.Add( t.shuffle() );

        return new Population(tmp);
    }

    private double calcMaxFit()
    {
        return this.p.Max( t => t!.fitness );
    }

    private Route select()
    {
        while (true)
        {
            int i = Program.r!.Next(0, Env.popSize);

            if (Program.r.NextDouble() < this.p[i]!.fitness / this.maxFit)
                return new Route(this.p[i]!.t);
        }
    }

    private Population genNewPop(int n)
    {
        List<Route?> tours = new List<Route?>();

        for (int i = 0; i < n; ++i)
        {
            Route t = this.select().crossover( this.select() );

            foreach (City unused in t.t)
                t = t.mutate();

            tours.Add(t);
        }

        return new Population(tours);
    }

    private Population elite(int n)
    {
        List<Route?> best = new List<Route?>();
        Population tmp = new Population(p);

        for (int i = 0; i < n; ++i)
        {
            best.Add( tmp.findBest() );
            tmp = new Population( tmp.p.Except(best).ToList() );
        }

        return new Population(best);
    }

    public Route? findBest()
    {
        foreach (Route? t in this.p)
        {
            double TOLERANCE = 0.000000001;
            if (Math.Abs(t!.fitness - this.maxFit) < TOLERANCE)
                return t;
        }
        
        return null;
    }

    public Population evolve()
    {
        Population best = this.elite(Env.elitism);
        Population np = this.genNewPop(Env.popSize - Env.elitism);
        return new Population( best.p.Concat(np.p).ToList() );
    }
}