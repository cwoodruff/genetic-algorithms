namespace genetic_algorithm_ts;

// This is a Chromosome in Genetic Algorithm terms
public class Route
{
    public List<City> t { get; private set; }
    public double distance { get; private set; }
    public double fitness { get; private set; }

    // ctor
    public Route(List<City> l)
    {
        this.t = l;
        this.distance = this.calcDist();
        this.fitness = this.calcFit();
    }

    // Functionality
    public static Route random(int n)
    {
        List<City> t = new List<City>();

        for (int i = 0; i < n; ++i)
            t.Add( City.random() );

        return new Route(t);
    }

    public Route shuffle()
    {
        List<City> tmp = new List<City>(this.t);
        int n = tmp.Count;

        while (n > 1)
        {
            n--;
            int k = Program.r!.Next(n + 1);
            (tmp[k], tmp[n]) = (tmp[n], tmp[k]);
        }

        return new Route(tmp);
    }

    public Route crossover(Route m)
    {
        int i = Program.r!.Next(0, m.t.Count);
        int j = Program.r.Next(i, m.t.Count);
        List<City> s = this.t.GetRange(i, j - i + 1);
        List<City> ms = m.t.Except(s).ToList();
        List<City> c = ms.Take(i)
            .Concat(s)
            .Concat( ms.Skip(i) )
            .ToList();
        return new Route(c);
    }

    public Route mutate()
    {
        List<City> tmp = new List<City>(this.t);

        if (Program.r != null && Program.r.NextDouble() < Env.MutRate)
        {
            int i = Program.r.Next(0, this.t.Count);
            int j = Program.r.Next(0, this.t.Count);
            (tmp[i], tmp[j]) = (tmp[j], tmp[i]);
        }

        return new Route(tmp);
    }

    private double calcDist()
    {
        double total = 0;
        for (int i = 0; i < this.t.Count; ++i)
            total += this.t[i].distanceTo( this.t[ (i + 1) % this.t.Count ] );

        return total;
    }

    private double calcFit()
    {
        return 1 / this.distance;
    }

}