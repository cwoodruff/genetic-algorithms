namespace engineering_design_GA.fitness_function;

/// <summary>
/// Marker interface for custom configuration data needed by fitness functions.
/// </summary>
public interface IFitnessFunctionConfiguration { }

/// <summary>
/// An abstract base class for fitness functions that require custom configuration data.
/// </summary>
/// <typeparam name="T">The chromosome type.</typeparam>
/// <typeparam name="TConfig">The type of configuration data (must implement IFitnessFunctionConfiguration).</typeparam>
public abstract class BaseFitnessFunction<T, TConfig>(TConfig config) : IFitnessFunction<T>
    where TConfig : IFitnessFunctionConfiguration
{
    protected readonly TConfig Config = config;

    public abstract double Evaluate(T chromosome);
}