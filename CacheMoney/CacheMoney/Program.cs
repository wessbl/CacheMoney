using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheMoney
{
    class Program
    {
        /// <summary>
        /// Receives a series of decimal addresses, and simulates cache entries and access as well
        /// as hit/miss info. Uses 16-bit addresses and 32-bit instructions.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            DisplayCache();
        }

        static void DisplayCache()
        {
            //  Display Demo
            Console.WriteLine("Direct-Mapped Cache: 8 Byte Blocks, 8 Rows");
            Console.WriteLine("Cache size: X Rows * 1 Valid bit * X Tag bits * X X-byte blocks = 683 bits");
            Console.WriteLine("16-bit address -> | 22-bit tag | 8 Rows | 2 Offset |");
            Console.WriteLine("Cache miss time: 1 cycle + 10 cycle penalty + 8 Byte Blocks per cycle = 19 cycles");
            Console.WriteLine("Address: |0|4|8|...");
            Console.WriteLine("Hit/Miss: |M|H|M|...");
            Console.WriteLine("X hits, X misses. X% hit rate.");
            Console.WriteLine("(Avg CPI = X hits + X misses * X-cycle miss penalty) / X address lookups = X Avg CPI");
            Console.WriteLine("Row|Valid|Tag|Data");
            for (int i = 0; i < 3; i++)
                Console.WriteLine(i + "|1|01100|X Bytes");
            Console.Read();
        }

        /// <summary>
        /// Represents a Fully-Associative cache
        /// </summary>
        public class FullyAssoc
        {
            // Create a table with rows for bits: |Valid|Tag|LRU|Data|
            private int[,] table;
            private int rows;
            private int blockSize;
            private int lru_ciel;
            private bool allFull;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="rows"></param>
            public FullyAssoc(int rows, int blockSize)
            {
                // Instantiate state variables
                table = new int[4,rows]; // 4 Columns X # of Rows
                this.rows = rows;
                this.blockSize = blockSize;
                lru_ciel = (int)Math.Log(rows, 2);
                allFull = false;

                // Set valid bits = 0
                for (int i = 0; i < rows; i++)
                    table[0,i] = 0;
            }

            public void Access(string s)
            {

            }

            private void AddEntry(int tag)
            {
                int row = 0;
                //  If we haven't used all the rows, just use an empty row
                if (!allFull)
                {
                    for (int i = 0; i < rows; i++)
                        if (table[0,i] == 0)
                            row = i;
                }

                // Otherwise, take the first row with LRU of 0
                else
                {
                    for (int i = 0; i < rows; i++)
                        if (table[2,i] == 0)
                            row = i;
                }

                //  Set the row
                table[0,row] = 1;
                table[1,row] = tag;
                table[2,row] = lru_ciel + 1;
                table[3,row] = blockSize;
            }
        }

        /// <summary>
        /// Represents a Direct-Mapped cache
        /// </summary>
        public class DirectMapped
        {

        }

        /// <summary>
        /// Represents an n-Set Fully-Associative cache
        /// </summary>
        public class SetAssoc
        {

        }
    }
}
