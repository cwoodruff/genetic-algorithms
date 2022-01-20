using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace genetic_algorithm_ts
{
    public delegate double GAFunction(double[] values);

    public class GA
    {
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
            m_fitnessTable = new List<double>();
            m_thisGeneration = new List<Genome>(Generations);
            m_nextGeneration = new List<Genome>(Generations);
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
                    double d = m_thisGeneration[PopulationSize - 1].Fitness;
                    outputFitness.WriteLine("{0},{1}", i, d);
                }
            }

            if (outputFitness != null)
                outputFitness.Close();
        }

        private int RouletteSelection()
        {
            double randomFitness = m_random.NextDouble() * m_totalFitness;
            var idx = -1;
            var first = 0;
            var last = PopulationSize - 1;
            var mid = last / 2;

            //  ArrayList's BinarySearch is for exact values only
            //  so do this by hand.
            while (idx == -1 && first <= last)
            {
                if (randomFitness < m_fitnessTable[mid])
                {
                    last = mid;
                }
                else if (randomFitness > m_fitnessTable[mid])
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
            m_totalFitness = 0;
            foreach (var genome in m_thisGeneration)
            {
                genome.Fitness = FitnessFunction(genome.Genes());
                m_totalFitness += genome.Fitness;
            }

            m_thisGeneration.Sort(new GenomeComparer());

            //  now sorted in order of fitness.
            double fitness = 0.0;
            m_fitnessTable.Clear();
            
            foreach (var genome in m_thisGeneration)
            {
                fitness += genome.Fitness;
                m_fitnessTable.Add(fitness);
            }
        }

        private void CreateGenomes()
        {
            m_thisGeneration = Enumerable.Repeat(new Genome(GenomeSize), PopulationSize).ToList();
        }

        private void CreateNextGeneration()
        {
            m_nextGeneration.Clear();
            Genome g = null;
            if (Elitism)
                g = m_thisGeneration[PopulationSize - 1];

            for (var i = 0; i < PopulationSize; i += 2)
            {
                int pidx1 = RouletteSelection();
                int pidx2 = RouletteSelection();
                Genome parent1, parent2, child1, child2;
                parent1 = m_thisGeneration[pidx1];
                parent2 = m_thisGeneration[pidx2];

                if (m_random.NextDouble() < CrossoverRate)
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

                m_nextGeneration.Add(child1);
                m_nextGeneration.Add(child2);
            }

            if (Elitism && g != null)
                m_nextGeneration[0] = g;

            m_thisGeneration.Clear();
            foreach (var genome in m_nextGeneration) m_thisGeneration.Add(genome);
        }

        private double m_totalFitness;

        private List<Genome> m_thisGeneration;
        private List<Genome> m_nextGeneration;
        private List<double> m_fitnessTable;

        static Random m_random = new Random();

        public static GAFunction FitnessFunction { get; set; }

        //  Properties
        public int PopulationSize { get; set; }

        public int Generations { get; set; }

        public int GenomeSize { get; set; }

        public double CrossoverRate { get; set; }

        public double MutationRate { get; set; }

        public string FitnessFile { get; set; }

        public bool Elitism { get; set; }

        public void GetBest(out double[] values, out double fitness)
        {
            Genome g = m_thisGeneration[PopulationSize - 1];
            values = new double[g.Length];
            g.GetValues(ref values);
            fitness = g.Fitness;
        }

        public void GetWorst(out double[] values, out double fitness)
        {
            GetNthGenome(0, out values, out fitness);
        }

        public Genome GetNthGenome(int n, out double[] values, out double fitness)
        {
            if (n < 0 || n > PopulationSize - 1)
                throw new ArgumentOutOfRangeException($"n too large, or too small");
            Genome g = m_thisGeneration[n];
            values = new double[g.Length];
            g.GetValues(ref values);
            fitness = g.Fitness;
            return g;
        }
    }
}