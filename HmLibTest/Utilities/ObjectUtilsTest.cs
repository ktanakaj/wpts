// ================================================================================================
// <summary>
//      ObjectUtilsのテストクラスソース。</summary>
//
// <copyright file="ObjectUtilsTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="ObjectUtils"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class ObjectUtilsTest
    {
        #region 初期化メソッドテストケース

        /// <summary>
        /// Equalsメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestDefaultIfNull()
        {
            Assert.IsNull(ObjectUtils.DefaultIfNull<object>(null, null));
            Assert.AreEqual(string.Empty, ObjectUtils.DefaultIfNull(string.Empty, "null"));
            Assert.AreEqual("not null", ObjectUtils.DefaultIfNull("not null", "null"));
            Assert.AreEqual("null", ObjectUtils.DefaultIfNull(null, "null"));
        }

        #endregion

        #region null値許容メソッドテストケース

        /// <summary>
        /// Equalsメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestEquals()
        {
            Assert.IsTrue(ObjectUtils.Equals(null, null));
            Assert.IsFalse(ObjectUtils.Equals(null, string.Empty));
            Assert.IsFalse(ObjectUtils.Equals(string.Empty, null));
            Assert.IsTrue(ObjectUtils.Equals(string.Empty, string.Empty));
            Assert.IsFalse(ObjectUtils.Equals(true, null));
            Assert.IsFalse(ObjectUtils.Equals(true, "true"));
            Assert.IsTrue(ObjectUtils.Equals(true, true));
            Assert.IsFalse(ObjectUtils.Equals(true, false));
        }

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            // 引数二つ
            Assert.IsNull(ObjectUtils.ToString(null, null));
            Assert.AreEqual(string.Empty, ObjectUtils.ToString(string.Empty, "null"));
            Assert.AreEqual("not null", ObjectUtils.ToString("not null", "null"));
            Assert.AreEqual("null", ObjectUtils.ToString(null, "null"));
            Assert.IsTrue(ObjectUtils.ToString(new object(), null).Length > 0);

            // 引数一つ
            Assert.AreEqual(string.Empty, ObjectUtils.ToString(null));
            Assert.AreEqual("not null", ObjectUtils.ToString("not null"));
            Assert.IsTrue(ObjectUtils.ToString(new object()).Length > 0);
        }

        #endregion
    }
}
