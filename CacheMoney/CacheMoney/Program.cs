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
    }
}
