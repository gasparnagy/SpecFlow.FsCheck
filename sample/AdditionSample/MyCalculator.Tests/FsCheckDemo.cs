using System;
using FsCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyCalculator.Tests
{
    [TestClass]
    public class FsCheckDemo
    {
        [TestMethod]
        public void FsCheckDemo_Addition_Identity()
        {
            Func<int, bool> identity = 
                (a) => Addition.Add(a, 0) == a;

            Prop.ForAll(identity).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void FsCheckDemo_Addition_Commutativity()
        {
            Func<int, int, bool> commitative = 
                (a, b) => Addition.Add(a, b) == Addition.Add(b, a);

            Prop.ForAll(commitative).QuickCheckThrowOnFailure();
        }
    }
}
