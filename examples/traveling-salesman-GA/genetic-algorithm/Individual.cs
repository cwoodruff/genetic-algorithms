namespace traveling_salesman_GA.genetic_algorithm;

/// <summary>
/// Represents an individual solution in the TSP genetic algorithm.
/// The chromosome is an array of integers representing a permutation of city indices.
/// </summary>
public class Individual
{
    public int[] Chromosome { get; set; }
    public double Fitness { get; set; }

    public Individual(int chromosomeLength)
    {
        Chromosome = new int[chromosomeLength];
    }

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