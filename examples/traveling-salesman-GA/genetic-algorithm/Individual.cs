namespace traveling_salesman_GA.genetic_algorithm;

/// <summary>
/// Represents an individual solution in the TSP genetic algorithm.
/// The chromosome is an array of integers representing a permutation of city indices.
/// </summary>
public class Individual(int chromosomeLength)
{
    public int[] Chromosome { get; set; } = new int[chromosomeLength];
    public double Fitness { get; set; }

    /// <summary>
    /// Creates a deep copy of the individual.
    /// </summary>
    public Individual Clone()
    {
        Individual clone = new Individual(Chromosome.Length);
        Array.Copy(Chromosome, clone.Chromosome, Chromosome.Length);
        clone.Fitness = Fitness;
        return clone;
    }
}