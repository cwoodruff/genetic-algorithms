namespace engineering_design_GA.fitness_function;

/// <summary>
/// Generic interface for a fitness function.
/// </summary>
/// <typeparam name="T">Type of the chromosome.</typeparam>
public interface IFitnessFunction<T>
{
    /// <summary>
    /// Evaluates and returns the fitness of the provided chromosome.
    /// </summary>
    double Evaluate(T chromosome);
}