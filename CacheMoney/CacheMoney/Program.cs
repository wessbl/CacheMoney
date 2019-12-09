﻿using System;
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
            for (int i = 1; i < 100; i++)
            {
                //  Get the row #
                int row = (int)(i / Math.Pow(2, 3));
                row = row % (int)Math.Pow(2, 3);
                Console.WriteLine(i + ": " + row);
            }

            int[] instruction_set = { 4, 8, 12, 16, 20, 32, 36, 40, 44, 20, 32, 36, 40, 44, 64, 68,
                4, 8, 12, 92, 96, 100, 104, 108, 112, 100, 112, 116, 120, 128, 140, 144 };

            //DisplayCache();
            Console.Read();
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
        }

        public abstract class Cache
        {
            protected int[,] table;
            protected int num_rows;
            protected int blockSize;
            protected int offsetBits;
            protected int tagBits;

            protected Cache(int cols, int num_rows, int blockSize)
            {
                // Instantiate state variables
                table = new int[cols, num_rows];
                this.num_rows = num_rows;
                this.blockSize = blockSize;
                offsetBits = (int)Math.Log(blockSize, 2);
                tagBits = 16 - offsetBits;
                Console.Read();

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

            /// <summary>
            /// Adds an entry that doesn't already exist in the cache
            /// </summary>
            /// <param name="tag"></param>
            protected abstract void AddEntry(int tag, int row);
        }

        /// <summary>
        /// Represents a Fully-Associative cache
        /// </summary>
        public class FullyAssoc : Cache 
        {
            // Create a table with rows for bits: 
            private int lru_ciel;
            private bool allFull;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="num_rows"></param>
            public FullyAssoc(int num_rows, int blockSize) : base(4, num_rows, blockSize)   //  |Valid|Tag|LRU|Data|
            {
                // Instantiate state variables
                lru_ciel = (int)Math.Log(num_rows, 2);
                allFull = false;
            }

            public override bool Access(int address)
            {
                //  Get the tag, which is the bits on the left of the offset
                int tag = address / (int)Math.Pow(2, offsetBits);

                //  Find the tag in the cache
                for (int i = 0; i < num_rows; i++)
                    if (table[0, i] == 1 && table[1, i] == tag) // must be valid & tag match
                        return true;

                AddEntry(tag, 0);
                return false;
            }

            /// <summary>
            /// Adds a new entry in the cache
            /// </summary>
            /// <param name="tag"></param>
            protected override void AddEntry(int tag, int not_needed)
            {
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

            public DirectMapped(int rows, int blockSize) : base(3, rows, blockSize) //  |Valid|Tag|Data|
            {
                rowBits = (int)Math.Log(rows, 2);
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

                AddEntry(tag, row);
                return false;
            }

            protected override void AddEntry(int tag, int row)
            {
                table[0, row] = 1;
                table[1, row] = tag;
                table[2, row] = blockSize;
            }
        }

        /// <summary>
        /// Represents an n-Set Fully-Associative cache
        /// </summary>
        public class SetAssoc : Cache
        {
            private Dictionary<int, Cache> rows; // One row points to a cache

            /// <summary>
            /// Since this cache relies on many n-set Fully-Associative caches, we'll do away with 
            /// the Cache table and add a dictionary of row numbers that point to a FA cache.
            /// </summary>
            /// <param name="rows"></param>
            /// <param name="blockSize"></param>
            public SetAssoc(int num_rows, int blockSize) : base(0, 0, blockSize)// Row-> |Valid|Tag|LRU|Data|
            {
                rows = new Dictionary<int, Cache>();
                for (int i = 0; i < num_rows; i++)
                    rows.Add(i, new FullyAssoc(num_rows, blockSize));
            }

            public override bool Access(int tag)
            {
                throw new NotImplementedException();
            }

            protected override void AddEntry(int tag, int row)
            {
                throw new NotImplementedException();
            }
        }
    }
}
