namespace traveling_salesman_GA.fitness_function;

/// <summary>
/// Defines a generic interface for a fitness function.
/// The chromosome type (here, an int[] for TSP routes) is specified as a generic parameter.
/// </summary>
/// <typeparam name="T">The type of the chromosome.</typeparam>
public interface IFitnessFunction<T>
{
    /// <summary>
    /// Evaluates and returns the fitness score for the given chromosome.
    /// </summary>
    double Evaluate(T chromosome);
}