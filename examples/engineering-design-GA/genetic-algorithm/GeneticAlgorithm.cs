using engineering_design_GA.fitness_function;

namespace engineering_design_GA.genetic_algorithm;

/// <summary>
    /// A genetic algorithm implementation that is agnostic of the problem domain.
    /// It works with any fitness function (provided via DI) that evaluates a chromosome of type double[].
    /// </summary>
    public class GeneticAlgorithm(
        int populationSize,
        int chromosomeLength,
        double mutationRate,
        double crossoverRate,
        int tournamentSize,
        int maxGenerations,
        IFitnessFunction<double[]> fitnessFunction)
    {
        // GA parameters.
        public int PopulationSize { get; set; } = populationSize;
        public int ChromosomeLength { get; set; } = chromosomeLength;
        public double MutationRate { get; set; } = mutationRate;
        public double CrossoverRate { get; set; } = crossoverRate;
        public int TournamentSize { get; set; } = tournamentSize;
        public int MaxGenerations { get; set; } = maxGenerations;

        // The injected fitness function.

        public List<Individual> Population { get; set; } = new(populationSize);
        public Individual BestIndividual { get; set; }
        public Random Rand { get; set; } = new();

        /// <summary>
        /// Initializes the population with random design parameters.
        /// For demonstration, each gene is randomly generated between 0 and 10.
        /// </summary>
        public void InitializePopulation()
        {
            Population.Clear();
            for (int i = 0; i < PopulationSize; i++)
            {
                var individual = new Individual(ChromosomeLength);
                for (int j = 0; j < ChromosomeLength; j++)
                {
                    individual.Chromosome[j] = Rand.NextDouble() * 10;
                }
                individual.Fitness = fitnessFunction.Evaluate(individual.Chromosome);
                Population.Add(individual);
            }
            BestIndividual = GetBestIndividual();
        }

        /// <summary>
        /// Returns the individual with the highest fitness in the population.
        /// </summary>
        public Individual GetBestIndividual()
        {
            var best = Population[0];
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
        /// Selects an individual using tournament selection.
        /// </summary>
        public Individual TournamentSelection()
        {
            Individual best = null;
            for (int i = 0; i < TournamentSize; i++)
            {
                int randomIndex = Rand.Next(PopulationSize);
                var competitor = Population[randomIndex];
                if (best == null || competitor.Fitness > best.Fitness)
                {
                    best = competitor;
                }
            }
            return best;
        }

        /// <summary>
        /// Performs arithmetic crossover on two parents to generate two children.
        /// </summary>
        public (Individual, Individual) Crossover(Individual parent1, Individual parent2)
        {
            var child1 = parent1.Clone();
            var child2 = parent2.Clone();

            if (Rand.NextDouble() < CrossoverRate)
            {
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
        /// Mutates an individual by adding a small random noise to each design variable.
        /// </summary>
        public void Mutate(Individual individual)
        {
            double mutationRange = 0.5; // Maximum change per gene.
            for (int i = 0; i < ChromosomeLength; i++)
            {
                if (Rand.NextDouble() < MutationRate)
                {
                    double noise = (Rand.NextDouble() * 2 - 1) * mutationRange;
                    individual.Chromosome[i] = Math.Max(0.0, individual.Chromosome[i] + noise);
                }
            }
        }

        /// <summary>
        /// Runs the genetic algorithm for the specified number of generations.
        /// </summary>
        public void Run()
        {
            InitializePopulation();

            for (int generation = 0; generation < MaxGenerations; generation++)
            {
                var newPopulation = new List<Individual>();

                while (newPopulation.Count < PopulationSize)
                {
                    var parent1 = TournamentSelection();
                    var parent2 = TournamentSelection();

                    (Individual child1, Individual child2) = Crossover(parent1, parent2);
                    Mutate(child1);
                    Mutate(child2);

                    child1.Fitness = fitnessFunction.Evaluate(child1.Chromosome);
                    child2.Fitness = fitnessFunction.Evaluate(child2.Chromosome);

                    newPopulation.Add(child1);
                    if (newPopulation.Count < PopulationSize)
                    {
                        newPopulation.Add(child2);
                    }
                }

                Population = newPopulation;
                var currentBest = GetBestIndividual();
                if (currentBest.Fitness > BestIndividual.Fitness)
                {
                    BestIndividual = currentBest;
                }

                Console.WriteLine($"Generation {generation + 1}: Best Fitness = {BestIndividual.Fitness:F6}");
            }
        }
    }