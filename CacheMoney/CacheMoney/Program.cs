using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheMoney
{
    public class Program
    {
        /// <summary>
        /// Receives a series of decimal addresses, and simulates cache entries and access as well
        /// as hit/miss info. Uses 16-bit addresses and 32-bit instructions.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int[] instruction_set = { 4, 8, 12, 16, 20, 32, 36, 40, 44, 20, 32, 36, 40, 44, 64, 68,
                4, 8, 12, 92, 96, 100, 104, 108, 112, 100, 112, 116, 120, 128, 140, 144 };

            //  Instantiate each cache type
            FullyAssoc fa = new FullyAssoc(4, 6);
            int FA_bit = FA_Bits(4);

            DirectMapped dm = new DirectMapped(4, 6);
            int DM_bit = DM_Bits(4);

            SetAssoc sa = new SetAssoc(2, 2, 6);
            int SA_bit = SA_Bits(2, 2);

            // Cycle trackers
            int FA_Cycles = 0;
            int DM_Cycles = 0;
            int SA_Cycles = 0;

            //  H/M Trackers
            int FA_tally = 0;
            int DM_tally = 0;
            int SA_tally = 0;

            //  H/M Arrays
            string[] FA_Hit = new string[32];
            string[] DM_Hit = new string[32];
            string[] SA_Hit = new string[32];

            // Test each instruction twice on each cache!
            for (int iteration = 1; iteration <=2; iteration++)
                for (int i = 0; i < 32; i++)
                {
                    // FA
                    if (fa.Access(instruction_set[i]) && iteration == 2)
                    {
                        FA_Hit[i] = "H";
                        FA_tally += 1;
                        FA_Cycles += 1;
                    }
                    else
                    {
                        FA_Hit[i] = "M";
                        FA_Cycles += 17;
                    }

                    //DM
                    if (dm.Access(instruction_set[i]) && iteration == 2)
                    {
                        DM_Cycles += 1;
                        DM_Hit[i] = "H";
                        DM_tally += 1;
                    }
                    else
                    {
                        DM_Cycles += 17;
                        DM_Hit[i] = "M";
                    }

                    //SA
                    if (sa.Access(instruction_set[i]) && iteration == 2)
                    {
                        SA_Cycles += 1;
                        SA_Hit[i] = "H";
                        SA_tally += 1;
                    }
                    else
                    {
                        SA_Cycles += 17;
                        SA_Hit[i] = "M";
                    }
                }


            //  Display FA
            Console.WriteLine("Fully-Associative Cache: 6 Byte Blocks, 4 Rows");
            Console.WriteLine("Cache size: " + FA_bit);
            Console.WriteLine("Cache miss time: 1 cycle + 10 cycle penalty + 6 Byte Blocks per cycle = 17 cycles");
            Console.WriteLine(FA_tally+" hits, "+(32-FA_tally)+" misses.");
            Console.WriteLine("Avg CPI = " + FA_Cycles);
            Console.WriteLine("Valid|Tag|LRU");
            fa.PrintData();
            Console.WriteLine();

            //  Display DM
            Console.WriteLine("Direct-Mapped Cache: 6 Byte Blocks, 4 Rows");
            Console.WriteLine("Cache size: " + DM_bit);
            Console.WriteLine("Cache miss time: 1 cycle + 10 cycle penalty + 6 Byte Blocks per cycle = 17 cycles");
            Console.WriteLine(DM_tally + " hits, " + (32 - DM_tally) + " misses.");
            Console.WriteLine("Avg CPI = " + DM_Cycles);
            Console.WriteLine("Valid|Tag");
            dm.PrintData();
            Console.WriteLine();

            //  Display SA
            Console.WriteLine("Fully-Associative Cache: 6 Byte Blocks, 4 Rows");
            Console.WriteLine("Cache size: " + SA_bit);
            Console.WriteLine("Cache miss time: 1 cycle + 10 cycle penalty + 6 Byte Blocks per cycle = 17 cycles");
            Console.WriteLine(SA_tally + " hits, " + (32 - SA_tally) + " misses.");
            Console.WriteLine("Avg CPI = " + SA_Cycles);
            Console.WriteLine("Valid|Tag|LRU");
            sa.PrintData();
            Console.WriteLine();

            Console.Read();
        }

        /// <summary>
        /// Calculates best block size for a row
        /// </summary>
        /// <param name="rows"></param>
        static int FA_Bits(int rows)
        {
            int max = 840;
            int lru = (int)Math.Log(rows, 2); // Get number of bits
            int equal, best = 0;
            int bS = 0;
            for (int i = 0; i < 100; i++)
            {
                int offsetBits = (int)Math.Log(i, 2);
                int tagBits = 16 - offsetBits;
                equal = rows * (1 + lru + tagBits + (8 * 4 * i));
                if (equal < max)
                {
                    best = equal;
                    bS = i;
                }
                else break;
            }
            return best;
            Console.WriteLine(rows + " rows can have up to " + bS + " bytes optimally.");
            Console.WriteLine((double)best * 100 / (double)max);
        }

        /// <summary>
        /// Calculates best block size for a row
        /// </summary>
        /// <param name="rows"></param>
        static int DM_Bits(int rows)
        {
            int max = 840;
            int equal, save = 0;
            int bS = 0;
            for (int i = 0; i < 100; i++)
            {
                int offsetBits = (int)Math.Log(i, 2);
                int tagBits = 16 - offsetBits;
                equal = rows * (1 + tagBits + (8 * 4 * i));
                if (equal < max)
                {
                    save = equal;
                    bS = i;
                }
                else break;
            }
            return save;
            Console.WriteLine(rows + " rows can have up to " + bS + " bytes optimally.");
            Console.WriteLine((double)save * 100 / (double)max);
        }

        /// <summary>
        /// Calculates best block size for a row
        /// </summary>
        /// <param name="rows"></param>
        static int SA_Bits(int rows, int n)
        {
            int max = 840;
            int lru = (int)Math.Log(rows, 2); // Get number of bits
            int equal, best = 0;
            int bS = 0;
            for (int i = 0; i < 100; i++)
            {
                int offsetBits = (int)Math.Log(i, 2);
                int tagBits = 16 - offsetBits;
                equal = rows * n * (1 + lru + tagBits + (8 * 4 * i));
                if (equal < max)
                {
                    best = equal;
                    bS = i;
                }
                else break;
            }
            return best;
            Console.WriteLine(rows + " rows can have up to " + bS + " bytes optimally.");
            Console.WriteLine((double)best * 100 / (double)max);
        }

        /// <summary>
        /// An abstract class to hold regular cache values and methods
        /// </summary>
        public abstract class Cache
        {
            protected int[,] table;
            protected int num_rows;
            protected int blockSize;
            protected int offsetBits;
            protected int tagBits;

            /// <summary>
            /// Instantiate class variables, set valid bit
            /// </summary>
            /// <param name="cols"></param>
            /// <param name="num_rows"></param>
            /// <param name="blockSize"></param>
            protected Cache(int cols, int num_rows, int bS)
            {
                // Instantiate state variables
                table = new int[cols, num_rows];
                this.num_rows = num_rows;
                this.blockSize = bS * 4; // 4 bytes per instruction
                offsetBits = (int)Math.Log(blockSize, 2);
                tagBits = 16 - offsetBits;

                // Set valid bits = 0
                for (int i = 0; i < num_rows; i++)
                    table[0, i] = 0;
            }

            /// <summary>
            /// Tries to access given tag, reporting if it was already in the cache (hit/miss)
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public abstract bool Access(int address);
            public abstract void PrintData();
        }

        /// <summary>
        /// Represents a Fully-Associative cache
        /// </summary>
        public class FullyAssoc : Cache 
        {
            private int lru_ciel;
            private bool allFull;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="num_rows"></param>
            public FullyAssoc(int num_rows, int blockSize) : base(4, num_rows, blockSize)   //  |Valid|Tag|LRU|Data|
            {
                // Instantiate state variables
                lru_ciel = (int)Math.Log(num_rows, 2); // Get number of bits
                lru_ciel = (int)Math.Pow(2, lru_ciel) - 1; // Get max number those bits can represent
                allFull = false;
            }

            public override bool Access(int address)
            {
                //  Get the tag, which is the bits on the left of the offset
                int tag = address / (int)Math.Pow(2, offsetBits);

                //  Find the tag in the cache
                for (int i = 0; i < num_rows; i++)
                    if (table[0, i] == 1 && table[1, i] == tag) // must be valid & tag match
                    {
                        ManageLRU();
                        table[2, i] = lru_ciel + 1;
                        return true;
                    }

                //  Add the entry since it's not in the cache //

                int row = 0;
                //  If we haven't used all the rows, just use an empty row
                if (!allFull)
                {
                    for (int i = 0; i < num_rows; i++)
                        if (table[0,i] == 0)
                            row = i;

                    // Check if we're all full
                    if (row == num_rows - 1)
                        allFull = true;
                }

                // Otherwise, take the first row with LRU of 0
                else
                {
                    for (int i = 0; i < num_rows; i++)
                        if (table[2,i] == 0)
                            row = i;
                }

                //  Set the row
                table[0,row] = 1;
                table[1, row] = tag;
                table[2,row] = lru_ciel + 1;
                table[3,row] = blockSize;

                ManageLRU();
                return false;
            }

            public override void PrintData()
            {
                for (int i = 0; i < num_rows; i++)
                    Console.WriteLine(table[0, i] + "|" + table[1, i] + "|" + table[2, i]);
            }

            private void ManageLRU()
            {
                //  Decrement all LRUs
                for (int i = 0; i < num_rows; i++)
                    if (table[2, i] > 0)
                        table[2, i] -= 1;
            }
        }

        /// <summary>
        /// Represents a Direct-Mapped cache
        /// </summary>
        public class DirectMapped : Cache
        {
            private int rowBits;

            public DirectMapped(int num_rows, int blockSize) : base(3, num_rows, blockSize) //  |Valid|Tag|Data|
            {
                rowBits = (int)Math.Log(num_rows, 2);
                tagBits -= rowBits;
            }

            public override bool Access(int address)
            {
                //  Get the row # & tag
                int row = (int)(address / Math.Pow(2, offsetBits));
                row = row % (int)Math.Pow(2, rowBits);
                int tag = address / ((int)(Math.Pow(2, rowBits + offsetBits)));

                //  Lookup
                if (table[0, row] == 1 && table[1, row] == tag)
                    return true;

                // Add the entry since it's not in the cache
                table[0, row] = 1;
                table[1, row] = tag;
                table[2, row] = blockSize;
                return false;
            }

            public override void PrintData()
            {
                for (int i = 0; i < num_rows; i++)
                    Console.WriteLine(table[0, i] + "|" + table[1, i]);
            }
        }

        /// <summary>
        /// Represents an n-Set Fully-Associative cache
        /// </summary>
        public class SetAssoc : Cache
        {
            private int n;
            private int rowBits;
            private Dictionary<int, Cache> rows; // One row points to a cache

            /// <summary>
            /// Since this cache relies on many n-set Fully-Associative caches, we'll do away with 
            /// the Cache table and add a dictionary of row numbers that point to a FA cache.
            /// </summary>
            /// <param name="rows"></param>
            /// <param name="blockSize"></param>
            public SetAssoc(int n, int num_rows, int blockSize) : base(0, 0, blockSize)// Row-> |Valid|Tag|LRU|Data|
            {
                rowBits = (int)Math.Log(num_rows, 2);
                tagBits -= rowBits;
                rows = new Dictionary<int, Cache>();
                for (int i = 0; i < num_rows; i++)
                    rows.Add(i, new FullyAssoc(n, blockSize));
            }

            public override bool Access(int address)
            {
                //  Get the row # & tag
                int row = (int)(address / Math.Pow(2, offsetBits));
                row = row % (int)Math.Pow(2, rowBits);
                int tag = address / ((int)(Math.Pow(2, rowBits + offsetBits)));

                //  Lookup
                rows.TryGetValue(row, out Cache temp);
                return temp.Access(address);
            }

            public override void PrintData()
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    Console.WriteLine("Row " + i + ":");
                    rows[i].PrintData();
                }
            }
        }
    }
}
