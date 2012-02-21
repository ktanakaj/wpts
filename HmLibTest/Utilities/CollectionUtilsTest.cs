// ================================================================================================
// <summary>
//      CollectionUtilsのテストクラスソース。</summary>
//
// <copyright file="CollectionUtilsTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// CollectionUtilsのテストクラスです。
    /// </summary>
    [TestFixture]
    public class CollectionUtilsTest
    {
        #region 比較メソッドテストケース

        /// <summary>
        /// ContainsIgnoreCaseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestContainsIgnoreCase()
        {
            string[] array = new string[0];
            Assert.IsFalse(CollectionUtils.ContainsIgnoreCase(array, null));
            Assert.IsFalse(CollectionUtils.ContainsIgnoreCase(array, String.Empty));
            Assert.IsFalse(CollectionUtils.ContainsIgnoreCase(array, "test"));

            array = new string[] { "test" };
            Assert.IsFalse(CollectionUtils.ContainsIgnoreCase(array, null));
            Assert.IsFalse(CollectionUtils.ContainsIgnoreCase(array, String.Empty));
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, "test"));
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, "teST"));
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, "TEST"));
            Assert.IsFalse(CollectionUtils.ContainsIgnoreCase(array, "tesd"));

            array = new string[] { "TEst" };
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, "test"));
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, "teST"));
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, "TEST"));
            Assert.IsFalse(CollectionUtils.ContainsIgnoreCase(array, "tesd"));

            array = new string[] { "Test", null, "日本語" };
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, null));
            Assert.IsFalse(CollectionUtils.ContainsIgnoreCase(array, String.Empty));
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, "test"));
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, "日本語"));

            array = new string[] { "Test", String.Empty, "日本語" };
            Assert.IsFalse(CollectionUtils.ContainsIgnoreCase(array, null));
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, String.Empty));
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, "test"));
            Assert.IsTrue(CollectionUtils.ContainsIgnoreCase(array, "日本語"));
        }

        /// <summary>
        /// ContainsIgnoreCaseメソッドテストケース（異常系）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestContainsIgnoreCaseNull()
        {
            CollectionUtils.ContainsIgnoreCase(null, "test");
        }

        #endregion

        #region 加工メソッドテストケース

        /// <summary>
        /// Trimメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTrim()
        {
            Assert.AreEqual(0, CollectionUtils.Trim(new string[0]).Length);
            Assert.AreEqual(1, CollectionUtils.Trim(new string[] { "test" }).Length);
            Assert.AreEqual(
                new string[] { "test" },
                CollectionUtils.Trim(new string[] { " test " }));
            Assert.AreEqual(
                new string[] { "Test", null, "日本語" },
                CollectionUtils.Trim(new string[] { " Test", null, "日本語 " }));
            Assert.AreEqual(
                new string[] { "Te st", String.Empty, "日 本 語" },
                CollectionUtils.Trim(new string[] { "Te st ", " ", " 日 本 語 " }));
        }

        /// <summary>
        /// Trimメソッドテストケース（異常系）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTrimNull()
        {
            CollectionUtils.Trim(null);
        }

        #endregion
    }
}
