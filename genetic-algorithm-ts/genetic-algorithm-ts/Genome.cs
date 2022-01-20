using System;

namespace genetic_algorithm_ts
{
    public class Genome
    {
        public Genome(int length)
        {
            Length = length;
            _mGenes = new double[length];
            CreateGenes();
        }

        private Genome(int length, bool createGenes)
        {
            Length = length;
            _mGenes = new double[length];
            if (createGenes)
                CreateGenes();
        }

        public Genome(ref double[] genes)
        {
            Length = genes.GetLength(0);
            _mGenes = new double[Length];
            for (int i = 0; i < Length; i++)
                _mGenes[i] = genes[i];
        }


        private void CreateGenes()
        {
            // DateTime d = DateTime.UtcNow;
            for (int i = 0; i < Length; i++)
                _mGenes[i] = _mRandom.NextDouble();
        }

        public void Crossover(ref Genome genome2, out Genome child1, out Genome child2)
        {
            int pos = (int)(_mRandom.NextDouble() * Length);
            child1 = new Genome(Length, false);
            child2 = new Genome(Length, false);
            for (int i = 0; i < Length; i++)
            {
                if (i < pos)
                {
                    child1._mGenes[i] = _mGenes[i];
                    child2._mGenes[i] = genome2._mGenes[i];
                }
                else
                {
                    child1._mGenes[i] = genome2._mGenes[i];
                    child2._mGenes[i] = _mGenes[i];
                }
            }
        }


        public void Mutate()
        {
            for (int pos = 0; pos < Length; pos++)
            {
                if (_mRandom.NextDouble() < MutationRate)
                    _mGenes[pos] = (_mGenes[pos] + _mRandom.NextDouble()) / 2.0;
            }
        }

        public double[] Genes()
        {
            return _mGenes;
        }

        public void Output()
        {
            for (var i = 0; i < Length; i++)
            {
                Console.WriteLine("{0:F4}", _mGenes[i]);
            }

            Console.Write("\n");
        }

        public void GetValues(ref double[] values)
        {
            for (var i = 0; i < Length; i++)
                values[i] = _mGenes[i];
        }


        private readonly double[] _mGenes;
        static readonly Random _mRandom = new Random();

        public double Fitness { get; set; }


        public static double MutationRate { get; set; }

        public int Length { get; }
    }
}