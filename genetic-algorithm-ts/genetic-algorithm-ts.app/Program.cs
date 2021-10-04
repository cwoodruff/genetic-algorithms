using System;

namespace genetic_algorithm_ts.app
{
    class Program
    {
        public static double TravellingSalesmanFitnessFunction(double[] route)
        {
            var totalLength = 0.0;

            for (var i = 0; i < route.GetLength(0) - 1; i++)
                totalLength += Distance(route[i], route[i], route[i + 1], route[i + 1]);

            // Add the distance from the last to the first route location.
            totalLength += Distance(route[^1], route[^1], route[0], route[0]);

            return totalLength;
        }
        
        private static double Distance(double x1, double y1, double x2, double y2)
            =>  Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));

        public static void Main()
        {
            GA ga = new GA(0.8,0.05,1000,20000,2);
		
            GA.FitnessFunction = TravellingSalesmanFitnessFunction;
            ga.Elitism = true;
            ga.Go();

            ga.GetBest(out var values, out var fitness);
            Console.WriteLine("Best ({0}):", fitness);
            foreach (var t in values)
                Console.WriteLine("{0} ", t);

            ga.GetWorst(out values, out fitness);
            Console.WriteLine("\nWorst ({0}):", fitness);
            foreach (var t in values)
                Console.WriteLine("{0} ", t);
        }
    }
}