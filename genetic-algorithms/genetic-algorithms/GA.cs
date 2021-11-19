using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace genetic_algorithms
{
    public delegate double GAFunction(double[] values);

    public class GA
    {
        private double _TotalFitness;

        private List<Genome> _thisGeneration;
        private List<Genome> _nextGeneration;
        private List<double> _fitnessTable;

        private static readonly Random _random = new Random();

        public static GAFunction FitnessFunction { get; set; }
        
        private int PopulationSize { get; set; }

        private int Generations { get; set; }

        private int GenomeSize { get; set; }

        private double CrossoverRate { get; set; }

        private double MutationRate { get; set; }

        private string FitnessFile { get; set; }

        public bool Elitism { get; set; }
        public GA()
        {
            InitialValues();
            MutationRate = 0.05;
            CrossoverRate = 0.80;
            PopulationSize = 100;
            Generations = 2000;
            FitnessFile = "";
        }

        public GA(double crossoverRate, double mutationRate, int populationSize, int generationSize, int genomeSize)
        {
            InitialValues();
            MutationRate = mutationRate;
            CrossoverRate = crossoverRate;
            PopulationSize = populationSize;
            Generations = generationSize;
            GenomeSize = genomeSize;
            FitnessFile = "";
        }

        public GA(int genomeSize)
        {
            InitialValues();
            GenomeSize = genomeSize;
        }


        private void InitialValues()
        {
            Elitism = false;
        }

        public void Go()
        {
            if (FitnessFunction == null)
                throw new ArgumentNullException($"Need to supply fitness function");
            if (GenomeSize == 0)
                throw new IndexOutOfRangeException("Genome size not set");

            //  Create the fitness table.
            _fitnessTable = new List<double>();
            _thisGeneration = new List<Genome>(Generations);
            _nextGeneration = new List<Genome>(Generations);
            Genome.MutationRate = MutationRate;

            CreateGenomes();
            RankPopulation();

            StreamWriter outputFitness = null;
            var write = false;
            if (FitnessFile != "")
            {
                write = true;
                outputFitness = new StreamWriter(FitnessFile);
            }

            for (var i = 0; i < Generations; i++)
            {
                CreateNextGeneration();
                RankPopulation();
                if (write)
                {
                    double d = _thisGeneration[PopulationSize - 1].Fitness;
                    outputFitness.WriteLine("{0},{1}", i, d);
                }
            }

            if (outputFitness != null)
                outputFitness.Close();
        }

        private int RouletteSelection()
        {
            double randomFitness = _random.NextDouble() * _TotalFitness;
            var idx = -1;
            var first = 0;
            var last = PopulationSize - 1;
            var mid = last / 2;

            while (idx == -1 && first <= last)
            {
                if (randomFitness < _fitnessTable[mid])
                {
                    last = mid;
                }
                else if (randomFitness > _fitnessTable[mid])
                {
                    first = mid;
                }

                mid = (first + last) / 2;
                if ((last - first) == 1)
                    idx = last;
            }

            return idx;
        }

        private void RankPopulation()
        {
            _TotalFitness = 0;
            foreach (var genome in _thisGeneration)
            {
                genome.Fitness = FitnessFunction(genome.Genes());
                _TotalFitness += genome.Fitness;
            }

            _thisGeneration.Sort(new GenomeComparer());

            //  now sorted in order of fitness.
            double fitness = 0.0;
            _fitnessTable.Clear();

            foreach (var genome in _thisGeneration)
            {
                fitness += genome.Fitness;
                _fitnessTable.Add(fitness);
            }
        }

        private void CreateGenomes()
        {
            _thisGeneration = Enumerable.Repeat(new Genome(GenomeSize), PopulationSize).ToList();
        }

        private void CreateNextGeneration()
        {
            _nextGeneration.Clear();
            Genome g = null;
            if (Elitism)
                g = _thisGeneration[PopulationSize - 1];

            for (var i = 0; i < PopulationSize; i += 2)
            {
                var pidx1 = RouletteSelection();
                var pidx2 = RouletteSelection();
                Genome child1, child2;
                var parent1 = _thisGeneration[pidx1];
                var parent2 = _thisGeneration[pidx2];

                if (_random.NextDouble() < CrossoverRate)
                {
                    parent1.Crossover(ref parent2, out child1, out child2);
                }
                else
                {
                    child1 = parent1;
                    child2 = parent2;
                }

                child1.Mutate();
                child2.Mutate();

                _nextGeneration.Add(child1);
                _nextGeneration.Add(child2);
            }

            if (Elitism && g != null)
                _nextGeneration[0] = g;

            _thisGeneration.Clear();
            foreach (var genome in _nextGeneration) _thisGeneration.Add(genome);
        }

        public void GetBest(out double[] values, out double fitness)
        {
            Genome g = _thisGeneration[PopulationSize - 1];
            values = new double[g.Length];
            g.GetValues(ref values);
            fitness = g.Fitness;
        }

        public void GetWorst(out double[] values, out double fitness)
        {
            GetNthGenome(0, out values, out fitness);
        }

        private void GetNthGenome(int n, out double[] values, out double fitness)
        {
            if (n < 0 || n > PopulationSize - 1)
                throw new ArgumentOutOfRangeException($"n too large, or too small");
            Genome g = _thisGeneration[n];
            values = new double[g.Length];
            g.GetValues(ref values);
            fitness = g.Fitness;
        }
    }
}