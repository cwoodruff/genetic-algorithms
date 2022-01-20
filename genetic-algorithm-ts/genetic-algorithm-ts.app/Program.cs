using System;
using System.Collections.Generic;

namespace genetic_algorithm_ts.app
{
    class Program
    {
        private static readonly double OptimalRouteLength = 7544.366;

        public static double TravellingSalesmanFitnessFunction(List<double> route)
        {
            var totalLength = 0.0;

            for (var i = 0; i < route.Count - 1; i++)
                totalLength += Distance(route[i], route[i], route[i + 1], route[i + 1]);

            // Add the distance from the last to the first route location.
            totalLength += Distance(route[^1], route[^1], route[0], route[0]);

            return totalLength;
        }

        private static double Distance(double x1, double y1, double x2, double y2)
            => Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));

        public static void Main()
        {
            // The 'Berlin52' data set consists of 52 route locations.
            
            List<double> routeLocationsX = new List<double>
            {
                565.0, 25.0, 345.0, 945.0, 845.0, 880.0, 25.0, 525.0, 580.0, 650.0,
                1605.0, 1220.0, 1465.0, 1530.0, 845.0, 725.0, 145.0, 415.0, 510.0, 560.0,
                300.0, 520.0, 480.0, 835.0, 975.0, 1215.0, 1320.0, 1250.0, 660.0, 410.0,
                420.0, 575.0, 1150.0, 700.0, 685.0, 685.0, 770.0, 795.0, 720.0, 760.0,
                475.0, 95.0, 875.0, 700.0, 555.0, 830.0, 1170.0, 830.0, 605.0, 595.0,
                1340.0, 1740.0
            };
            
            List<double> routeLocationsY = new List<double>
            {
                575.0, 185.0, 750.0, 685.0, 655.0, 660.0, 230.0, 1000.0, 1175.0, 1130.0,
                620.0, 580.0, 200.0, 5.0, 680.0, 370.0, 665.0, 635.0, 875.0, 365.0,
                465.0, 585.0, 415.0, 625.0, 580.0, 245.0, 315.0, 400.0, 180.0, 250.0,
                555.0, 665.0, 1160.0, 580.0, 595.0, 610.0, 610.0, 645.0, 635.0, 650.0,
                960.0, 260.0, 920.0, 500.0, 815.0, 485.0, 65.0, 610.0, 625.0, 360.0,
                725.0, 245.0
            };
            
            //  The correct answer for the 'Berlin52' data set is known, so this specific
            //  Travelling Salesman Problem can be used to evaluate the performance of
            //  the Genetic Algorithm. The shortest route (global optimal solution) is:
            //
            //   0, 48, 31, 44, 18, 40, 7, 8, 9, 42, 32, 50, 10, 51, 13, 12, 46, 25, 26,
            //   27, 11, 24, 3, 5, 14, 4, 23, 47, 37, 36, 39, 38, 35, 34, 33, 43, 45, 15,
            //   28, 49, 19, 22, 29, 1, 6, 41, 20, 16, 2, 17, 30, 21, (0)
            //
            //   Length: 7544.366
            //
            //  The route is a loop of location indices and can be read in
            //  forward or backward direction.
            
            GA ga = new GA(0.8, 0.05, 1000, 1000, 52, routeLocationsX, routeLocationsY, OptimalRouteLength);

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