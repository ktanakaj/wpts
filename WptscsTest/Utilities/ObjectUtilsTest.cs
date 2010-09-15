// ================================================================================================
// <summary>
//      ObjectUtilsのテストクラスソース。</summary>
//
// <copyright file="ObjectUtilsTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// ObjectUtilsのテストクラスです。
    /// </summary>
    [TestFixture]
    public class ObjectUtilsTest
    {
        // TODO: メソッド全てをカバーしていない

        #region null値許容メソッドテストケース

        /// <summary>
        /// Equalsメソッドテストケース。
        /// </summary>
        [Test]
        public void TestEquals()
        {
            Assert.IsTrue(ObjectUtils.Equals(null, null));
            Assert.IsFalse(ObjectUtils.Equals(null, String.Empty));
            Assert.IsFalse(ObjectUtils.Equals(String.Empty, null));
            Assert.IsTrue(ObjectUtils.Equals(String.Empty, String.Empty));
            Assert.IsFalse(ObjectUtils.Equals(true, null));
            Assert.IsFalse(ObjectUtils.Equals(true, "true"));
            Assert.IsTrue(ObjectUtils.Equals(true, true));
            Assert.IsFalse(ObjectUtils.Equals(true, false));
        }

        #endregion
    }
}
