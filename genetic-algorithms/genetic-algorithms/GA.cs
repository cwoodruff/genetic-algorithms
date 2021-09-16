using System;
using System.Collections;
using System.IO;

namespace genetic_algorithms
{
    public delegate double GAFunction(double[] values);

    public class GA
    {
        public GA()
        {
            InitialValues();
            m_mutationRate = 0.05;
            m_crossoverRate = 0.80;
            m_populationSize = 100;
            m_generationSize = 2000;
            m_strFitness = "";
        }

        public GA(double crossoverRate, double mutationRate, int populationSize, int generationSize, int genomeSize)
        {
            InitialValues();
            m_mutationRate = mutationRate;
            m_crossoverRate = crossoverRate;
            m_populationSize = populationSize;
            m_generationSize = generationSize;
            m_genomeSize = genomeSize;
            m_strFitness = "";
        }

        public GA(int genomeSize)
        {
            InitialValues();
            m_genomeSize = genomeSize;
        }


        private void InitialValues()
        {
            m_elitism = false;
        }

        public void Go()
        {
            if (getFitness == null)
                throw new ArgumentNullException($"Need to supply fitness function");
            if (m_genomeSize == 0)
                throw new IndexOutOfRangeException("Genome size not set");

            //  Create the fitness table.
            m_fitnessTable = new ArrayList();
            m_thisGeneration = new ArrayList(m_generationSize);
            m_nextGeneration = new ArrayList(m_generationSize);
            Genome.MutationRate = m_mutationRate;


            CreateGenomes();
            RankPopulation();

            StreamWriter outputFitness = null;
            var write = false;
            if (m_strFitness != "")
            {
                write = true;
                outputFitness = new StreamWriter(m_strFitness);
            }

            for (var i = 0; i < m_generationSize; i++)
            {
                CreateNextGeneration();
                RankPopulation();
                if (write)
                {
                    double d = (m_thisGeneration[m_populationSize - 1] as Genome).Fitness;
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
            var last = m_populationSize - 1;
            var mid = last / 2;

            //  ArrayList's BinarySearch is for exact values only
            //  so do this by hand.
            while (idx == -1 && first <= last)
            {
                if (randomFitness < (double)m_fitnessTable[mid])
                {
                    last = mid;
                }
                else if (randomFitness > (double)m_fitnessTable[mid])
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
            for (int i = 0; i < m_populationSize; i++)
            {
                Genome g = ((Genome)m_thisGeneration[i]);
                g.Fitness = FitnessFunction(g.Genes());
                m_totalFitness += g.Fitness;
            }

            m_thisGeneration.Sort(new GenomeComparer());

            //  now sorted in order of fitness.
            double fitness = 0.0;
            m_fitnessTable.Clear();
            for (int i = 0; i < m_populationSize; i++)
            {
                fitness += ((Genome)m_thisGeneration[i]).Fitness;
                m_fitnessTable.Add(fitness);
            }
        }

        private void CreateGenomes()
        {
            for (int i = 0; i < m_populationSize; i++)
            {
                Genome g = new Genome(m_genomeSize);
                m_thisGeneration.Add(g);
            }
        }

        private void CreateNextGeneration()
        {
            m_nextGeneration.Clear();
            Genome g = null;
            if (m_elitism)
                g = (Genome)m_thisGeneration[m_populationSize - 1];

            for (int i = 0; i < m_populationSize; i += 2)
            {
                int pidx1 = RouletteSelection();
                int pidx2 = RouletteSelection();
                Genome parent1, parent2, child1, child2;
                parent1 = ((Genome)m_thisGeneration[pidx1]);
                parent2 = ((Genome)m_thisGeneration[pidx2]);

                if (m_random.NextDouble() < m_crossoverRate)
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

            if (m_elitism && g != null)
                m_nextGeneration[0] = g;

            m_thisGeneration.Clear();
            for (int i = 0; i < m_populationSize; i++)
                m_thisGeneration.Add(m_nextGeneration[i]);
        }


        private double m_mutationRate;
        private double m_crossoverRate;
        private int m_populationSize;
        private int m_generationSize;
        private int m_genomeSize;
        private double m_totalFitness;
        private string m_strFitness;
        private bool m_elitism;

        private ArrayList m_thisGeneration;
        private ArrayList m_nextGeneration;
        private ArrayList m_fitnessTable;

        static Random m_random = new Random();

        static private GAFunction getFitness;

        public GAFunction FitnessFunction
        {
            get => getFitness;
            set => getFitness = value;
        }

        //  Properties
        public int PopulationSize
        {
            get => m_populationSize;
            set => m_populationSize = value;
        }

        public int Generations
        {
            get => m_generationSize;
            set => m_generationSize = value;
        }

        public int GenomeSize
        {
            get => m_genomeSize;
            set => m_genomeSize = value;
        }

        public double CrossoverRate
        {
            get => m_crossoverRate;
            set => m_crossoverRate = value;
        }

        public double MutationRate
        {
            get => m_mutationRate;
            set => m_mutationRate = value;
        }

        public string FitnessFile
        {
            get => m_strFitness;
            set => m_strFitness = value;
        }

        public bool Elitism
        {
            get => m_elitism;
            set => m_elitism = value;
        }

        public void GetBest(out double[] values, out double fitness)
        {
            Genome g = ((Genome)m_thisGeneration[m_populationSize - 1]);
            values = new double[g.Length];
            g.GetValues(ref values);
            fitness = g.Fitness;
        }

        public void GetWorst(out double[] values, out double fitness)
        {
            GetNthGenome(0, out values, out fitness);
        }

        public void GetNthGenome(int n, out double[] values, out double fitness)
        {
            if (n < 0 || n > m_populationSize - 1)
                throw new ArgumentOutOfRangeException($"n too large, or too small");
            Genome g = ((Genome)m_thisGeneration[n]);
            values = new double[g.Length];
            g.GetValues(ref values);
            fitness = g.Fitness;
        }
    }
}