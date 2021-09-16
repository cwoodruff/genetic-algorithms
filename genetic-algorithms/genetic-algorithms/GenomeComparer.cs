#nullable enable

using System;
using System.Collections.Generic;

namespace genetic_algorithms
{
    public sealed class GenomeComparer : IComparer<Genome>
    {
        public int Compare(Genome? x, Genome? y)
        {
            if (x != null && y != null && x.Fitness > y.Fitness)
                return 1;
            if (x != null && y != null && Math.Abs(x.Fitness - y.Fitness) == 0)
                return 0;
            return -1;
        }
    }
}