using engineering_design_GA.fitness_function;
using engineering_design_GA.genetic_algorithm;
using Microsoft.Extensions.DependencyInjection;

namespace engineering_design_GA;

class Program
    {
        static void Main(string[] args)
        {
            // Set up the dependency injection container.
            var services = new ServiceCollection();

            // --- Engineering Design Optimization Configuration ---
            // For this example, assume we have three design variables.
            // The optimal value for each variable (by solving derivative = 0) would be:
            //   x[i] = BenefitCoefficients[i] / (2 * PenaltyCoefficient)
            var designConfig = new EngineeringDesignFitnessConfiguration
            {
                BenefitCoefficients = new double[] { 5.0, 3.0, 8.0 },
                PenaltyCoefficient = 0.5
            };

            // Register the configuration instance.
            services.AddSingleton<EngineeringDesignFitnessConfiguration>(designConfig);

            // Register the EngineeringDesignFitnessFunction as the implementation of IFitnessFunction<double[]>.
            services.AddSingleton<IFitnessFunction<double[]>, EngineeringDesignFitnessFunction>();

            // Register the GeneticAlgorithm.
            // The chromosome length is equal to the number of design variables.
            services.AddTransient<GeneticAlgorithm>(provider =>
            {
                var fitnessFunction = provider.GetRequiredService<IFitnessFunction<double[]>>();
                int populationSize = 100;
                int chromosomeLength = designConfig.BenefitCoefficients.Length; // 3 design variables
                double mutationRate = 0.1;
                double crossoverRate = 0.8;
                int tournamentSize = 5;
                int maxGenerations = 100;
                return new GeneticAlgorithm(
                    populationSize,
                    chromosomeLength,
                    mutationRate,
                    crossoverRate,
                    tournamentSize,
                    maxGenerations,
                    fitnessFunction);
            });

            // Build the DI provider.
            var serviceProvider = services.BuildServiceProvider();

            // Retrieve and run the genetic algorithm.
            var ga = serviceProvider.GetService<GeneticAlgorithm>();
            Console.WriteLine("Running Genetic Algorithm for Engineering Design Optimization...\n");
            ga.Run();

            // Display the best design found.
            Console.WriteLine("\nBest Design Found (Raw Parameters):");
            for (int i = 0; i < ga.BestIndividual.Chromosome.Length; i++)
            {
                Console.WriteLine($"Design Variable {i}: {ga.BestIndividual.Chromosome[i]:F4}");
            }
            Console.WriteLine($"\nFitness (Objective Value): {ga.BestIndividual.Fitness:F6}");

            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }
    }