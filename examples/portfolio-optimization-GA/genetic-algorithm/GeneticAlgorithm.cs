using engineering_design_GA.fitness_function;

namespace engineering_design_GA.genetic_algorithm;

/// <summary>
/// Implements a genetic algorithm for portfolio optimization.
/// Uses arithmetic crossover and mutation that adds random noise.
/// A custom fitness function is injected via dependency injection.
/// </summary>
public class GeneticAlgorithm
{
    // GA Parameters.
    public int PopulationSize { get; set; }
    public int ChromosomeLength { get; set; } // Equals the number of assets.
    public double MutationRate { get; set; }
    public double CrossoverRate { get; set; }
    public int TournamentSize { get; set; }
    public int MaxGenerations { get; set; }

    // The injected fitness function.
    private readonly IFitnessFunction<double[]> _fitnessFunction;

    // Population of individuals.
    public List<Individual> Population { get; set; }
    public Individual BestIndividual { get; set; }
    public Random Rand { get; set; }

    /// <summary>
    /// Constructor sets GA parameters and injects the fitness function.
    /// </summary>
    public GeneticAlgorithm(
        int populationSize,
        int chromosomeLength,
        double mutationRate,
        double crossoverRate,
        int tournamentSize,
        int maxGenerations,
        IFitnessFunction<double[]> fitnessFunction)
    {
        PopulationSize = populationSize;
        ChromosomeLength = chromosomeLength;
        MutationRate = mutationRate;
        CrossoverRate = crossoverRate;
        TournamentSize = tournamentSize;
        MaxGenerations = maxGenerations;
        _fitnessFunction = fitnessFunction;
        Population = new List<Individual>(populationSize);
        Rand = new Random();
    }

    /// <summary>
    /// Initializes the population with random asset weights (raw values).
    /// </summary>
    public void InitializePopulation()
    {
        Population.Clear();
        for (int i = 0; i < PopulationSize; i++)
        {
            Individual individual = new Individual(ChromosomeLength);
            // Assign a random value between 0 and 1 for each asset.
            for (int j = 0; j < ChromosomeLength; j++)
            {
                individual.Chromosome[j] = Rand.NextDouble();
            }

            // Evaluate fitness.
            individual.Fitness = _fitnessFunction.Evaluate(individual.Chromosome);
            Population.Add(individual);
        }

        BestIndividual = GetBestIndividual();
    }

    /// <summary>
    /// Evaluates the fitness of all individuals.
    /// </summary>
    public void EvaluatePopulation()
    {
        foreach (var individual in Population)
        {
            individual.Fitness = _fitnessFunction.Evaluate(individual.Chromosome);
        }
    }

    /// <summary>
    /// Returns the individual with the highest fitness.
    /// </summary>
    public Individual GetBestIndividual()
    {
        Individual best = Population[0];
        foreach (var individual in Population)
        {
            if (individual.Fitness > best.Fitness)
            {
                best = individual;
            }
        }

        return best;
    }

    /// <summary>
    /// Tournament selection: picks the best individual among a random subset.
    /// </summary>
    public Individual TournamentSelection()
    {
        Individual best = null;
        for (int i = 0; i < TournamentSize; i++)
        {
            int randomIndex = Rand.Next(PopulationSize);
            Individual competitor = Population[randomIndex];
            if (best == null || competitor.Fitness > best.Fitness)
            {
                best = competitor;
            }
        }

        return best;
    }

    /// <summary>
    /// Performs arithmetic crossover on two parents to produce two children.
    /// Each child is a weighted average of the parents.
    /// </summary>
    public (Individual, Individual) Crossover(Individual parent1, Individual parent2)
    {
        // Clone the parents to start with.
        Individual child1 = parent1.Clone();
        Individual child2 = parent2.Clone();

        if (Rand.NextDouble() < CrossoverRate)
        {
            // Use a random coefficient alpha.
            double alpha = Rand.NextDouble();
            child1 = new Individual(ChromosomeLength);
            child2 = new Individual(ChromosomeLength);
            for (int i = 0; i < ChromosomeLength; i++)
            {
                child1.Chromosome[i] = alpha * parent1.Chromosome[i] + (1 - alpha) * parent2.Chromosome[i];
                child2.Chromosome[i] = (1 - alpha) * parent1.Chromosome[i] + alpha * parent2.Chromosome[i];
            }
        }

        return (child1, child2);
    }

    /// <summary>
    /// Mutates an individual by adding a small random noise to each gene.
    /// Ensures that the gene remains non-negative.
    /// </summary>
    public void Mutate(Individual individual)
    {
        // Mutation range: maximum change per gene.
        double mutationRange = 0.1;
        for (int i = 0; i < ChromosomeLength; i++)
        {
            if (Rand.NextDouble() < MutationRate)
            {
                // Generate noise in [-mutationRange, mutationRange].
                double noise = (Rand.NextDouble() * 2 - 1) * mutationRange;
                individual.Chromosome[i] = Math.Max(0.0, individual.Chromosome[i] + noise);
            }
        }
    }

    /// <summary>
    /// Runs the GA for a specified number of generations.
    /// </summary>
    public void Run()
    {
        InitializePopulation();

        for (int generation = 0; generation < MaxGenerations; generation++)
        {
            List<Individual> newPopulation = new List<Individual>();

            // Create a new generation.
            while (newPopulation.Count < PopulationSize)
            {
                // Select two parents.
                Individual parent1 = TournamentSelection();
                Individual parent2 = TournamentSelection();

                // Apply crossover.
                (Individual child1, Individual child2) = Crossover(parent1, parent2);

                // Apply mutation.
                Mutate(child1);
                Mutate(child2);

                // Evaluate offspring.
                child1.Fitness = _fitnessFunction.Evaluate(child1.Chromosome);
                child2.Fitness = _fitnessFunction.Evaluate(child2.Chromosome);

                newPopulation.Add(child1);
                if (newPopulation.Count < PopulationSize)
                {
                    newPopulation.Add(child2);
                }
            }

            Population = newPopulation;

            // Update the overall best individual.
            Individual currentBest = GetBestIndividual();
            if (currentBest.Fitness > BestIndividual.Fitness)
            {
                BestIndividual = currentBest;
            }

            Console.WriteLine($"Generation {generation + 1}: Best Fitness = {BestIndividual.Fitness:F6}");
        }
    }
}