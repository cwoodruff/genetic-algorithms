namespace engineering_design_GA.genetic_algorithm;

/// <summary>
/// Represents an individual solution in the GA.
/// For engineering design optimization, the chromosome is an array of doubles.
/// </summary>
public class Individual
{
    public double[] Chromosome { get; set; }
    public double Fitness { get; set; }

    public Individual(int chromosomeLength)
    {
        Chromosome = new double[chromosomeLength];
    }

    /// <summary>
    /// Creates a deep copy of this individual.
    /// </summary>
    public Individual Clone()
    {
        var clone = new Individual(Chromosome.Length);
        Array.Copy(Chromosome, clone.Chromosome, Chromosome.Length);
        clone.Fitness = Fitness;
        return clone;
    }
}