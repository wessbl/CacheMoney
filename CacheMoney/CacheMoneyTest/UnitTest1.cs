using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CacheMoney.Program;

namespace CacheMoneyTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FA_Simple()
        {
            //  2-entry cache, that has 2 blocks of memory
            FullyAssoc fa = new FullyAssoc(2, 2);
            int sum = 0;
            if (fa.Access(24)) sum += 1;    //M add A
            if (fa.Access(28)) sum += 1;    //H
            if (fa.Access(32)) sum += 1;    //M add B
            if (fa.Access(24)) sum += 1;    //H
            if (fa.Access(20)) sum += 1;    //M add B
            if (fa.Access(16)) sum += 1;    //H
            if (fa.Access(28)) sum += 1;    //H
            if (fa.Access(20)) sum += 1;    //H
            Assert.AreEqual(5, sum);

            // All Misses: Skip every other block (1 block = 8)
            fa = new FullyAssoc(2, 2);
            sum = 0;
             for (int i = 0; i < 1025; i += 8)
                if (fa.Access(i)) sum += 1;     //M
            Assert.AreEqual(0, sum);

            //  Get every block, hit every other time
            fa = new FullyAssoc(2, 2);
            sum = 0;
            for (int i = 0; i < 1025; i += 4)
            if (fa.Access(i)) sum += 1;     //M
            Assert.AreEqual(128, sum);
        }

        [TestMethod]
        public void DM_Simple()
        {
            //  2-entry cache, that has 2 blocks of memory
            DirectMapped dm = new DirectMapped(2, 2);
            int sum = 0;
            if (dm.Access(24)) sum += 1;    //M add A
            if (dm.Access(28)) sum += 1;    //H
            if (dm.Access(92)) sum += 1;    //M add A
            if (dm.Access(88)) sum += 1;    //H
            if (dm.Access(24)) sum += 1;    //M add A
            if (dm.Access(16)) sum += 1;    //M add B
            if (dm.Access(28)) sum += 1;    //H
            if (dm.Access(20)) sum += 1;    //H
            Assert.AreEqual(4, sum);

            // All Misses: Skip every other block (1 block = 8)
            dm = new DirectMapped(2, 2);
            sum = 0;
            for (int i = 0; i < 1025; i += 8)
                if (dm.Access(i)) sum += 1;     //M
            Assert.AreEqual(0, sum);

            //  Get every block, hit every other time
            dm = new DirectMapped(2, 2);
            sum = 0;
            for (int i = 0; i < 1025; i += 4)
                if (dm.Access(i)) sum += 1;     //M
            Assert.AreEqual(128, sum);
        }

        [TestMethod]
        public void SA_Simple()
        {
            //  2-set, 2-entry cache, that has 2 blocks of memory
            SetAssoc sa = new SetAssoc(2, 2, 2);
            int sum = 0;
            if (sa.Access(24)) sum += 1;    //M B.1
            Assert.AreEqual(0, sum);
            if (sa.Access(28)) sum += 1;    //H
            Assert.AreEqual(1, sum);
            if (sa.Access(92)) sum += 1;    //M B.2
            Assert.AreEqual(1, sum);
            if (sa.Access(88)) sum += 1;    //H
            if (sa.Access(24)) sum += 1;    //H 
            Assert.AreEqual(3, sum);
            if (sa.Access(16)) sum += 1;    //M A.1
            Assert.AreEqual(3, sum);
            if (sa.Access(28)) sum += 1;    //H 
            if (sa.Access(20)) sum += 1;    //H
            Assert.AreEqual(5, sum);

            // All Misses: Skip every other block (1 block = 8)
            sa = new SetAssoc(2, 2, 2);
            sum = 0;
            for (int i = 0; i < 1025; i += 8)
                if (sa.Access(i)) sum += 1;     //M
            Assert.AreEqual(0, sum);

            //  Get every block, hit every other time
            sa = new SetAssoc(2, 2, 2);
            sum = 0;
            for (int i = 0; i < 1025; i += 4)
                if (sa.Access(i)) sum += 1;     //M
            Assert.AreEqual(128, sum);
        }
    }
}
