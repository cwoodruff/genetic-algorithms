namespace engineering_design_GA.genetic_algorithm;

/// <summary>
/// Represents an individual solution in the portfolio optimization GA.
/// The chromosome is an array of doubles representing raw (unnormalized) asset weights.
/// </summary>
public class Individual(int chromosomeLength)
{
    public double[] Chromosome { get; set; } = new double[chromosomeLength];
    public double Fitness { get; set; }

    /// <summary>
    /// Creates a deep copy of this individual.
    /// </summary>
    public Individual Clone()
    {
        Individual clone = new Individual(Chromosome.Length);
        Array.Copy(Chromosome, clone.Chromosome, Chromosome.Length);
        clone.Fitness = Fitness;
        return clone;
    }
}