namespace engineering_design_GA.fitness_function;

/// <summary>
/// Portfolio optimization fitness function.
/// It expects the chromosome to be an array of raw asset weights.
/// The fitness is calculated as: 
///   expectedReturn – riskAversion * variance
/// where the weights are normalized to sum to 1.
/// </summary>
public class PortfolioFitnessFunction : IFitnessFunction<double[]>
{
    private readonly double[] _expectedReturns;
    private readonly double[,] _covarianceMatrix;
    private readonly double _riskAversion;

    /// <summary>
    /// Constructor.
    /// expectedReturns: an array of expected return for each asset.
    /// covarianceMatrix: the risk covariance matrix between assets.
    /// riskAversion: a penalty coefficient to control risk.
    /// </summary>
    public PortfolioFitnessFunction(double[] expectedReturns, double[,] covarianceMatrix, double riskAversion)
    {
        if (expectedReturns == null) throw new ArgumentNullException(nameof(expectedReturns));
        if (covarianceMatrix == null) throw new ArgumentNullException(nameof(covarianceMatrix));
        _expectedReturns = expectedReturns;
        _covarianceMatrix = covarianceMatrix;
        _riskAversion = riskAversion;
    }

    /// <summary>
    /// Evaluates the fitness of a portfolio.
    /// Normalizes the chromosome (raw weights) to sum to 1.
    /// Then computes:
    ///   Fitness = expectedReturn – riskAversion * variance
    /// </summary>
    public double Evaluate(double[] chromosome)
    {
        // Normalize the weights.
        double sum = chromosome.Sum();
        double epsilon = 1e-10; // To avoid division by zero.
        double[] weights = chromosome.Select(w => w / (sum + epsilon)).ToArray();

        // Calculate the portfolio's expected return.
        double expectedReturn = 0.0;
        for (int i = 0; i < weights.Length; i++)
        {
            expectedReturn += weights[i] * _expectedReturns[i];
        }

        // Calculate the portfolio variance: w^T * Cov * w.
        double variance = 0.0;
        int n = weights.Length;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                variance += weights[i] * weights[j] * _covarianceMatrix[i, j];
            }
        }

        // Our fitness function is higher for portfolios with high returns and low variance.
        return expectedReturn - _riskAversion * variance;
    }
}