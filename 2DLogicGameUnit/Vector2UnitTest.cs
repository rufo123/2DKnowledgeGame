using System;
using _2DLogicGame;
using _2DLogicGame.ServerSide;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _2DLogicGameUnit
{
    [TestClass]
    public class Vector2UnitTest
    {
        [TestMethod]
        public void TestSumVector()
        {
            Vector2 tmpVector1 = new Vector2(800, 800);
            Vector2 tmpVector2 = new Vector2(125.56F, 555.57F);

            Vector2 tmpCalculatedNewVector = new Vector2(925.56F, 1355.57F);

            Assert.IsTrue(tmpCalculatedNewVector == tmpVector1 + tmpVector2);

            try
            {
                Assert.IsNotNull(tmpCalculatedNewVector == tmpVector1 + tmpVector2);
            }
            catch (Exception nullException)
            {
                Console.WriteLine(nullException);
                throw;
            }
        }

        [TestMethod]
        public void TestSumVector_ShouldThrowNullException()
        {
            Vector2 tmpVector1 = new Vector2(800, 800);
            Vector2 tmpVector2 = null;

            Vector2 tmpCalculatedNewVector = new Vector2(925.56F, 1355.57F);

            Assert.ThrowsException<System.NullReferenceException>(() => tmpVector1 + tmpVector2 );
        }


    }
}
