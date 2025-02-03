using traveling_salesman_GA.fitness_function;

namespace traveling_salesman_GA.genetic_algorithm;

/// <summary>
    /// Implements a genetic algorithm tailored for the Traveling Salesman Problem.
    /// Custom operators for initialization, order crossover, and swap mutation are used.
    /// The fitness function is injected via dependency injection.
    /// </summary>
    public class GeneticAlgorithm
    {
        // GA parameters.
        public int PopulationSize { get; set; }
        public int ChromosomeLength { get; set; }  // Equals the number of cities.
        public double MutationRate { get; set; }
        public double CrossoverRate { get; set; }
        public int TournamentSize { get; set; }
        public int MaxGenerations { get; set; }

        // The injected fitness function.
        private readonly IFitnessFunction<int[]> _fitnessFunction;

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
            IFitnessFunction<int[]> fitnessFunction)
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
        /// Initializes the population with random permutations of city indices.
        /// </summary>
        public void InitializePopulation()
        {
            Population.Clear();
            for (int i = 0; i < PopulationSize; i++)
            {
                Individual individual = new Individual(ChromosomeLength);
                // Initialize the chromosome with an ordered list of city indices.
                for (int j = 0; j < ChromosomeLength; j++)
                {
                    individual.Chromosome[j] = j;
                }
                // Shuffle to create a random route.
                Shuffle(individual.Chromosome);
                // Evaluate its fitness.
                individual.Fitness = _fitnessFunction.Evaluate(individual.Chromosome);
                Population.Add(individual);
            }
            BestIndividual = GetBestIndividual();
        }

        /// <summary>
        /// Fisher–Yates shuffle algorithm to randomize an array.
        /// </summary>
        private void Shuffle(int[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Rand.Next(i + 1);
                int temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        /// <summary>
        /// Re-evaluates the fitness of every individual in the population.
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
        /// Uses tournament selection to choose an individual from the population.
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
        /// Applies Order Crossover (OX) to two parents to produce two children.
        /// OX preserves the permutation validity required for TSP.
        /// </summary>
        public (Individual, Individual) Crossover(Individual parent1, Individual parent2)
        {
            // Clone parents to start with.
            Individual child1 = parent1.Clone();
            Individual child2 = parent2.Clone();

            if (Rand.NextDouble() < CrossoverRate)
            {
                // Choose two crossover points.
                int point1 = Rand.Next(ChromosomeLength);
                int point2 = Rand.Next(ChromosomeLength);
                if (point1 > point2)
                {
                    int temp = point1;
                    point1 = point2;
                    point2 = temp;
                }
                // Generate children using Order Crossover.
                child1 = OrderCrossover(parent1, parent2, point1, point2);
                child2 = OrderCrossover(parent2, parent1, point1, point2);
            }
            return (child1, child2);
        }

        /// <summary>
        /// Performs Order Crossover (OX) for a pair of parents.
        /// </summary>
        /// <param name="parent1">The parent providing the fixed segment.</param>
        /// <param name="parent2">The parent providing the ordering for the remainder.</param>
        /// <param name="start">Start index of the crossover segment.</param>
        /// <param name="end">End index of the crossover segment.</param>
        private Individual OrderCrossover(Individual parent1, Individual parent2, int start, int end)
        {
            Individual child = new Individual(ChromosomeLength);
            // Initialize with a marker (-1) for unused positions.
            for (int i = 0; i < ChromosomeLength; i++)
                child.Chromosome[i] = -1;

            // Copy the segment from parent1.
            for (int i = start; i <= end; i++)
            {
                child.Chromosome[i] = parent1.Chromosome[i];
            }

            // Fill the rest of the child's chromosome with genes from parent2 in order.
            int currentIndex = (end + 1) % ChromosomeLength;
            for (int i = 0; i < ChromosomeLength; i++)
            {
                int candidate = parent2.Chromosome[(end + 1 + i) % ChromosomeLength];
                if (!child.Chromosome.Contains(candidate))
                {
                    child.Chromosome[currentIndex] = candidate;
                    currentIndex = (currentIndex + 1) % ChromosomeLength;
                }
            }
            return child;
        }

        /// <summary>
        /// Mutates an individual by performing swap mutations.
        /// Two random genes (cities) are swapped.
        /// </summary>
        public void Mutate(Individual individual)
        {
            for (int i = 0; i < ChromosomeLength; i++)
            {
                if (Rand.NextDouble() < MutationRate)
                {
                    int index1 = Rand.Next(ChromosomeLength);
                    int index2 = Rand.Next(ChromosomeLength);
                    // Swap the two cities.
                    int temp = individual.Chromosome[index1];
                    individual.Chromosome[index1] = individual.Chromosome[index2];
                    individual.Chromosome[index2] = temp;
                }
            }
        }

        /// <summary>
        /// Runs the genetic algorithm over a set number of generations.
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
                    // Select parents using tournament selection.
                    Individual parent1 = TournamentSelection();
                    Individual parent2 = TournamentSelection();

                    // Apply crossover.
                    (Individual child1, Individual child2) = Crossover(parent1, parent2);

                    // Apply mutation.
                    Mutate(child1);
                    Mutate(child2);

                    // Evaluate the offspring.
                    child1.Fitness = _fitnessFunction.Evaluate(child1.Chromosome);
                    child2.Fitness = _fitnessFunction.Evaluate(child2.Chromosome);

                    newPopulation.Add(child1);
                    if (newPopulation.Count < PopulationSize)
                        newPopulation.Add(child2);
                }

                // Replace the old population.
                Population = newPopulation;

                // Update the best individual.
                Individual currentBest = GetBestIndividual();
                if (currentBest.Fitness > BestIndividual.Fitness)
                    BestIndividual = currentBest;

                Console.WriteLine($"Generation {generation + 1}: Best Fitness (1/Distance) = {BestIndividual.Fitness}");
            }
        }
    }