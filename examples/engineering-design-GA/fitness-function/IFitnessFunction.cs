namespace engineering_design_GA.fitness_function;

/// <summary>
/// A generic interface for any fitness function.
/// </summary>
/// <typeparam name="T">The chromosome (solution) type.</typeparam>
public interface IFitnessFunction<T>
{
    double Evaluate(T chromosome);
}