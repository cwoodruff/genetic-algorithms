﻿namespace genetic_algorithm_ts;

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
        this.distance = this.CalcDist();
        this.fitness = this.CalcFit();
    }

    // Functionality
    public static Route random(int n)
    {
        List<City> t = new List<City>();

        for (int i = 0; i < n; ++i)
            t.Add(City.Random());

        return new Route(t);
    }

    public Route Shuffle()
    {
        List<City> tmp = new List<City>(this.t);
        int n = tmp.Count;

        while (n > 1)
        {
            n--;
            int k = Program.rdm!.Next(n + 1);
            (tmp[k], tmp[n]) = (tmp[n], tmp[k]);
        }

        return new Route(tmp);
    }

    public Route Crossover(Route m)
    {
        int i = Program.rdm!.Next(0, m.t.Count);
        int j = Program.rdm.Next(i, m.t.Count);
        List<City> s = this.t.GetRange(i, j - i + 1);
        List<City> ms = m.t.Except(s).ToList();
        List<City> c = ms.Take(i)
            .Concat(s)
            .Concat(ms.Skip(i))
            .ToList();
        return new Route(c);
    }

    public Route Mutate()
    {
        List<City> tmp = new List<City>(this.t);

        if (Program.rdm != null && Program.rdm.NextDouble() < Env.MutRate)
        {
            int i = Program.rdm.Next(0, this.t.Count);
            int j = Program.rdm.Next(0, this.t.Count);
            (tmp[i], tmp[j]) = (tmp[j], tmp[i]);
        }

        return new Route(tmp);
    }

    private double CalcDist()
    {
        double total = 0;
        for (int i = 0; i < this.t.Count; ++i)
            total += this.t[i].DistanceTo(this.t[(i + 1) % this.t.Count]);

        return total;
    }

    private double CalcFit()
    {
        return 1 / this.distance;
    }
}