namespace engineering_design_GA.fitness_function;

/// <summary>
/// Configuration data for the engineering design optimization fitness function.
/// In this example, the design consists of a set of continuous variables.
/// BenefitCoefficients: each design variable’s linear benefit contribution.
/// PenaltyCoefficient: a quadratic penalty applied to all variables.
/// </summary>
public class EngineeringDesignFitnessConfiguration : IFitnessFunctionConfiguration
{
    public double[] BenefitCoefficients { get; set; }
    public double PenaltyCoefficient { get; set; }
}