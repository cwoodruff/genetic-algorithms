
using Microsoft.Extensions.DependencyInjection;
using traveling_salesman_GA.fitness_function;
using traveling_salesman_GA.genetic_algorithm;

namespace traveling_salesman_GA;

class Program
    {
        static void Main(string[] args)
        {
            // Set up the dependency injection container.
            ServiceCollection services = new ServiceCollection();

            // Create a list of cities for the TSP.
            // (For a real application, you might load these from a file or database.)
            List<City> cities = new List<City>
            {
                new City { Id = 0, X = 60.0,  Y = 200.0 },
                new City { Id = 1, X = 180.0, Y = 200.0 },
                new City { Id = 2, X = 80.0,  Y = 180.0 },
                new City { Id = 3, X = 140.0, Y = 180.0 },
                new City { Id = 4, X = 20.0,  Y = 160.0 },
                new City { Id = 5, X = 100.0, Y = 160.0 },
                new City { Id = 6, X = 200.0, Y = 160.0 },
                new City { Id = 7, X = 140.0, Y = 140.0 },
                new City { Id = 8, X = 40.0,  Y = 120.0 },
                new City { Id = 9, X = 100.0, Y = 120.0 },
                new City { Id = 10, X = 180.0, Y = 100.0 },
                new City { Id = 11, X = 60.0,  Y = 80.0 },
                new City { Id = 12, X = 120.0, Y = 80.0 },
                new City { Id = 13, X = 180.0, Y = 60.0 },
                new City { Id = 14, X = 20.0,  Y = 40.0 },
                new City { Id = 15, X = 100.0, Y = 40.0 },
                new City { Id = 16, X = 200.0, Y = 40.0 },
                new City { Id = 17, X = 20.0,  Y = 20.0 },
                new City { Id = 18, X = 60.0,  Y = 20.0 },
                new City { Id = 19, X = 160.0, Y = 20.0 }
            };

            // Register the TSPFitnessFunction as the implementation of IFitnessFunction<int[]>.
            services.AddSingleton<IFitnessFunction<int[]>>(new TSPFitnessFunction(cities));

            // Register the GeneticAlgorithm with its parameters.
            // Note that chromosome length is set to the number of cities.
            services.AddTransient<GeneticAlgorithm>(provider =>
            {
                var fitnessFunction = provider.GetRequiredService<IFitnessFunction<int[]>>();
                int populationSize = 100;
                int chromosomeLength = cities.Count;
                double mutationRate = 0.02;
                double crossoverRate = 0.8;
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

            // Build the DI service provider.
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            // Retrieve an instance of the GeneticAlgorithm.
            var ga = serviceProvider.GetService<GeneticAlgorithm>();

            // Run the genetic algorithm.
            ga.Run();

            // Display the best route found.
            Console.WriteLine("\nBest Route Found (City Indices):");
            Console.WriteLine(string.Join(" -> ", ga.BestIndividual.Chromosome));
            Console.WriteLine($"Fitness (1/TotalDistance): {ga.BestIndividual.Fitness}");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }