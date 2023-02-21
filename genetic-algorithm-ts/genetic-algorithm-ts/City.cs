namespace genetic_algorithm_ts;

// This is a Gene in Genetic Algorithm terms
public class City
{

    // Member variables
    private double x { get; set; }
    private double y { get; set; }

    // ctor
    private City(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    // Functionality
    public double distanceTo(City c)
    {
        return Math.Sqrt(Math.Pow((c.x - this.x), 2) + Math.Pow((c.y - this.y), 2));
    }

    public static City random()
    {
        return new City( Program.rdm!.NextDouble(), Program.rdm.NextDouble() );
    }
}