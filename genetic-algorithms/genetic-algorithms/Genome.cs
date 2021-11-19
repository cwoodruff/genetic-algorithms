using System;

namespace genetic_algorithms
{
    public class Genome
    {
        private readonly double[] _genes;
        private static readonly Random _random = new Random();

        public double Fitness { get; set; }


        public static double MutationRate { get; set; }

        public int Length { get; }
        
        public Genome(int length)
        {
            Length = length;
            _genes = new double[length];
            CreateGenes();
        }

        private Genome(int length, bool createGenes)
        {
            Length = length;
            _genes = new double[length];
            if (createGenes)
                CreateGenes();
        }

        public Genome(ref double[] genes)
        {
            Length = genes.GetLength(0);
            _genes = new double[Length];
            for (var i = 0; i < Length; i++)
                _genes[i] = genes[i];
        }


        private void CreateGenes()
        {
            for (var i = 0; i < Length; i++)
                _genes[i] = _random.NextDouble();
        }

        public void Crossover(ref Genome genome2, out Genome child1, out Genome child2)
        {
            var pos = (int)(_random.NextDouble() * Length);
            child1 = new Genome(Length, false);
            child2 = new Genome(Length, false);
            for (var i = 0; i < Length; i++)
            {
                if (i < pos)
                {
                    child1._genes[i] = _genes[i];
                    child2._genes[i] = genome2._genes[i];
                }
                else
                {
                    child1._genes[i] = genome2._genes[i];
                    child2._genes[i] = _genes[i];
                }
            }
        }


        public void Mutate()
        {
            for (int pos = 0; pos < Length; pos++)
            {
                if (_random.NextDouble() < MutationRate)
                    _genes[pos] = (_genes[pos] + _random.NextDouble()) / 2.0;
            }
        }

        public double[] Genes()
        {
            return _genes;
        }

        public void Output()
        {
            for (var i = 0; i < Length; i++)
            {
                Console.WriteLine("{0:F4}", _genes[i]);
            }

            Console.Write("\n");
        }

        public void GetValues(ref double[] values)
        {
            for (var i = 0; i < Length; i++)
                values[i] = _genes[i];
        }
    }
}