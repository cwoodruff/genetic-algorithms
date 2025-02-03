using engineering_design_GA.fitness_function;
using engineering_design_GA.genetic_algorithm;
using Microsoft.Extensions.DependencyInjection;

namespace portfolio_optimization_GA;

class Program
{
    static void Main(string[] args)
    {
        // Set up the dependency injection container.
        var services = new ServiceCollection();

        // -------------------------------
        // Define data for portfolio optimization.
        // In a real application, these could be loaded from a database or market data service.
        // -------------------------------
        // Example with 5 assets.
        double[] expectedReturns = new double[] { 0.10, 0.15, 0.12, 0.08, 0.20 };

        // A dummy covariance matrix (symmetric, positive values).
        double[,] covarianceMatrix = new double[5, 5]
        {
            { 0.005, 0.001, 0.001, 0.001, 0.001 },
            { 0.001, 0.040, 0.001, 0.001, 0.001 },
            { 0.001, 0.001, 0.023, 0.001, 0.001 },
            { 0.001, 0.001, 0.001, 0.018, 0.001 },
            { 0.001, 0.001, 0.001, 0.001, 0.030 }
        };

        // Risk aversion parameter: higher values penalize risk more strongly.
        double riskAversion = 0.5;

        // Register the PortfolioFitnessFunction as the implementation of IFitnessFunction<double[]>.
        services.AddSingleton<IFitnessFunction<double[]>>(new PortfolioFitnessFunction(expectedReturns, covarianceMatrix, riskAversion));

        // Register the GeneticAlgorithm with its parameters.
        // Chromosome length equals the number of assets.
        services.AddTransient<GeneticAlgorithm>(provider =>
        {
            var fitnessFunction = provider.GetRequiredService<IFitnessFunction<double[]>>();
            int populationSize = 100;
            int chromosomeLength = expectedReturns.Length;
            double mutationRate = 0.05;    // 5% chance to mutate each gene.
            double crossoverRate = 0.8;    // 80% chance to perform crossover.
            int tournamentSize = 5;
            int maxGenerations = 200;
            return new GeneticAlgorithm(
                populationSize,
                chromosomeLength,
                mutationRate,
                crossoverRate,
                tournamentSize,
                maxGenerations,
                fitnessFunction);
        });

        // Build the service provider.
        var serviceProvider = services.BuildServiceProvider();

        // Retrieve an instance of the GeneticAlgorithm.
        var ga = serviceProvider.GetService<GeneticAlgorithm>();

        // Run the GA.
        Console.WriteLine("Running Genetic Algorithm for Portfolio Optimization...\n");
        ga.Run();

        // Display the best portfolio found.
        Console.WriteLine("\nBest Portfolio Found (Raw Weights):");
        for (int i = 0; i < ga.BestIndividual.Chromosome.Length; i++)
        {
            Console.WriteLine($"Asset {i}: {ga.BestIndividual.Chromosome[i]:F4}");
        }

        // Normalize the weights for display.
        double sum = ga.BestIndividual.Chromosome.Sum();
        double[] normalizedWeights = ga.BestIndividual.Chromosome.Select(w => w / sum).ToArray();
        Console.WriteLine("\nNormalized Weights (summing to 1):");
        for (int i = 0; i < normalizedWeights.Length; i++)
        {
            Console.WriteLine($"Asset {i}: {normalizedWeights[i]:F4}");
        }

        Console.WriteLine($"\nFitness (Expected Return - RiskPenalty): {ga.BestIndividual.Fitness:F6}");
        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();
    }
}