using System;

namespace genetic_algorithms.app
{
    class Program
    {
        //  optimal solution for this is (0.5,0.5)
        public static double theFitnessFunction(double[] values)
        {
            if (values.GetLength(0) != 2)
                throw new ArgumentOutOfRangeException($"should only have 2 args");

            double x = values[0];
            double y = values[1];
            double n = 9;

            double f1 = Math.Pow(15*x*y*(1-x)*(1-y)*Math.Sin(n*Math.PI*x)*Math.Sin(n*Math.PI*y),2);
            return f1;
        }
        
        public static void Main()
        {
            GA ga = new GA(0.8,0.05,100,2000,2);
		
            GA.FitnessFunction = theFitnessFunction;
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