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
    using NUnit.Framework;

    /// <summary>
    /// ObjectUtilsのテストクラスです。
    /// </summary>
    [TestFixture]
    public class ObjectUtilsTest
    {
        #region 初期化メソッドテストケース

        /// <summary>
        /// Equalsメソッドテストケース。
        /// </summary>
        [Test]
        public void TestDefaultIfNull()
        {
            Assert.IsNull(ObjectUtils.DefaultIfNull<object>(null, null));
            Assert.AreEqual(String.Empty, ObjectUtils.DefaultIfNull(String.Empty, "null"));
            Assert.AreEqual("not null", ObjectUtils.DefaultIfNull("not null", "null"));
            Assert.AreEqual("null", ObjectUtils.DefaultIfNull(null, "null"));
        }

        #endregion

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

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            // 引数二つ
            Assert.IsNull(ObjectUtils.ToString(null, null));
            Assert.AreEqual(String.Empty, ObjectUtils.ToString(String.Empty, "null"));
            Assert.AreEqual("not null", ObjectUtils.ToString("not null", "null"));
            Assert.AreEqual("null", ObjectUtils.ToString(null, "null"));
            Assert.IsNotEmpty(ObjectUtils.ToString(new object(), null));

            // 引数一つ
            Assert.AreEqual(String.Empty, ObjectUtils.ToString(null));
            Assert.AreEqual("not null", ObjectUtils.ToString("not null"));
            Assert.IsNotEmpty(ObjectUtils.ToString(new object()));
        }

        #endregion
    }
}
