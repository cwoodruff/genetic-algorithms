namespace traveling_salesman_GA.fitness_function;

/// <summary>
/// A TSP-specific fitness function.
/// It calculates the total distance of the route (assumed to be a closed loop) and returns the inverse.
/// Because the genetic algorithm maximizes fitness, using the inverse of the route distance turns the minimization problem (minimize distance) into a maximization one.
/// </summary>
public class TSPFitnessFunction : IFitnessFunction<int[]>
{
    private readonly List<City> _cities;

    /// <summary>
    /// Constructor accepts a list of cities.
    /// </summary>
    public TSPFitnessFunction(List<City> cities)
    {
        _cities = cities;
    }

    /// <summary>
    /// Evaluates the fitness of a TSP route (chromosome) by calculating the total Euclidean distance.
    /// Fitness is computed as 1 / (totalDistance + epsilon).
    /// </summary>
    public double Evaluate(int[] chromosome)
    {
        double totalDistance = 0.0;
        int numCities = chromosome.Length;
        for (int i = 0; i < numCities; i++)
        {
            City current = _cities[chromosome[i]];
            // Ensure the route is a closed loop by wrapping around to the start.
            City next = _cities[chromosome[(i + 1) % numCities]];
            double dx = current.X - next.X;
            double dy = current.Y - next.Y;
            totalDistance += Math.Sqrt(dx * dx + dy * dy);
        }
        // Adding a small epsilon avoids division by zero.
        return 1.0 / (totalDistance + 1e-6);
    }
}