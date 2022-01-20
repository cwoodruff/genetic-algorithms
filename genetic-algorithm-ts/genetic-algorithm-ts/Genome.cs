using System;
using System.Collections.Generic;

namespace genetic_algorithm_ts
{
    public class Genome
    {
        public Genome(int length)
        {
            Length = length;
            _mGenes = new List<double>();
            CreateGenes();
        }

        private Genome(int length, bool createGenes)
        {
            Length = length;
            _mGenes = new List<double>();
            if (createGenes)
                CreateGenes();
        }

        public Genome(ref List<double> genes)
        {
            Length = genes.Count;
            _mGenes = new List<double>();
            for (int i = 0; i < Length; i++)
                _mGenes.Add(genes[i]);
        }


        private void CreateGenes()
        {
            int iGeneSwap = 0;
            int geneTemp = 0;
            Random r = new Random();

            for (int iGene = 0; iGene < Length; iGene++)
            {
                iGeneSwap = r.Next(0, _mGenes.Count - 1);
        
                geneTemp = (int)_mGenes[iGene];
                _mGenes[iGene] = _mGenes[iGeneSwap];
                _mGenes[iGeneSwap] = geneTemp;
            }
        }

        public void Crossover(ref Genome genome2, out Genome child1, out Genome child2)
        {
            // This crossover method will copy the genes of two parent Genomes
            // without creating any duplicates and by preserving their order. 
            // The result will be a valid route for the Travelling Salesman Problem.
            //
            // A section of genes from genome2 will be copied to this genome and the
            // remaining genes will be copied to this dna in the order in which they
            // appear in dna2.

            // Get a random start and end index for a section of dna1.

            int iSecStart = 0;
            int iSecEnd = 0;
            Random r = new Random();

            while (iSecStart >= iSecEnd)
            {
                iSecStart = r.Next(0, genome2._mGenes.Count - 1);
                iSecEnd   = r.Next(0, this._mGenes.Count - 1);
            }

            // Copy the section of this to this dna.

            for (int iGene = iSecStart; iGene <= iSecEnd; iGene++)
                _mGenes[iGene] = this._mGenes[iGene];

            var sectionSize = iSecEnd - iSecStart;

            // Copy the genes of genome2 without the genes found in the section of this.
            // The copying begins after the end index of the section. When the end of
            // genome2 is reached, copy the genes from the beginning of dna2 to the start
            // index of the section.

            List<double> genomeDifference = genome2._mGenes.GetRange(index, count);
            genomeDifference.(genome2._mGenes.Count - sectionSize);

            if (iSecEnd + 1 <= genome2._mGenes.Count - 1)
                for (int iGene = iSecEnd + 1; iGene < genome2._mGenes.Count; iGene++)
                    if (!isGeneInSection(genome2._mGenes[iGene], iSecStart, iSecEnd))
                        genomeDifference.Add(genome2._mGenes[iGene]);

            for (int iGene = 0; iGene <= iSecEnd; iGene++)
                if (!isGeneInSection(_mGenes[iGene], iSecStart, iSecEnd))
                    genomeDifference.Add(_mGenes[iGene]);

            // The difference from dna1 and dna2 will be copied to this dna.
            // The insertion of genes into this dna begins after the end index of the
            // section. When the end of the dna is reached, insert the genes in the
            // beginning of the dna to the start index of the section.

            int i = 0;

            if (iSecEnd + 1 <= genome2._mGenes.Count - 1)
                i = iSecEnd + 1;

            for (int iGene = 0; iGene < genomeDifference.Count; iGene++)
            {
                _mGenes[i] = genomeDifference[iGene];
                i++;
                if (i > _mGenes.Count - 1)
                    i = 0;
            }
        }
        
        private bool isGeneInSection(
            double gene,
            int iSectionStart,
            int iSectionEnd)
        {
            for (int iGene = iSectionStart; iGene <= iSectionEnd; iGene++)
                if (gene == _mGenes[iGene])
                    return true;

            return false;
        }


        public void Mutate()
        {
            // This mutates the DNA in a minimal way by selecting two random genes
            // and swapping them with each other.

            List<double> iGene1 = getRandomIntegerInRange(0, _genes.size() - 1);
            List<double> iGene2 = getRandomIntegerInRange(0, _genes.size() - 1);

            const auto tempGene = _genes[iGene1];
            _genes[iGene1] = _genes[iGene2];
            _genes[iGene2] = tempGene;
        }
        
        private getRandomIntegerInRange(int minInclusive, int maxInclusive)
        {
            auto generator = getMersenneTwisterEngine();
            uniform_int_distribution<T> distribution(minInclusive, maxInclusive);
            return distribution(*generator);
        }

        public List<double> Genes()
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

        public void GetValues(ref List<double> values)
        {
            for (var i = 0; i < Length; i++)
                values[i] = _mGenes[i];
        }


        private readonly List<double> _mGenes;
        static readonly Random _mRandom = new Random();

        public double Fitness { get; set; }


        public static double MutationRate { get; set; }

        public int Length { get; }
    }
}