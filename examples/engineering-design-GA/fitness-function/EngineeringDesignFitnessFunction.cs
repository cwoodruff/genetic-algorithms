namespace engineering_design_GA.fitness_function;

/// <summary>
/// A fitness function for engineering design optimization.
/// The chromosome is an array of doubles representing design variables.
/// The function computes:
///   Fitness = sum(BenefitCoefficients[i] * x[i]) - PenaltyCoefficient * sum(x[i]^2)
/// Higher fitness values represent better designs.
/// </summary>
public class EngineeringDesignFitnessFunction : BaseFitnessFunction<double[], EngineeringDesignFitnessConfiguration>
{
    public EngineeringDesignFitnessFunction(EngineeringDesignFitnessConfiguration config) : base(config) { }

    public override double Evaluate(double[] chromosome)
    {
        double benefit = 0.0;
        double penalty = 0.0;
        for (int i = 0; i < chromosome.Length; i++)
        {
            // Ensure design parameters are non-negative.
            double x = Math.Max(0.0, chromosome[i]);
            benefit += Config.BenefitCoefficients[i] * x;
            penalty += x * x;
        }
        return benefit - Config.PenaltyCoefficient * penalty;
    }
}