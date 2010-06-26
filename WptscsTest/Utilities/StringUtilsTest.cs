// ================================================================================================
// <summary>
//      StringUtilsのテストクラスソース。</summary>
//
// <copyright file="StringUtilsTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;

    /// <summary>
    /// StringUtilsのテストクラスです。
    /// </summary>
    [TestFixture]
    public class StringUtilsTest
    {
        /// <summary>
        /// DefaultStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestDefaultString()
        {
            // 引数一つ
            Assert.AreEqual(StringUtils.DefaultString(null), String.Empty);
            Assert.AreEqual(StringUtils.DefaultString(String.Empty), String.Empty);
            Assert.AreEqual(StringUtils.DefaultString(" "), " ");
            Assert.AreEqual(StringUtils.DefaultString("null以外の文字列"), "null以外の文字列");

            // 引数二つ
            Assert.AreEqual(StringUtils.DefaultString(null, "初期値"), "初期値");
            Assert.AreEqual(StringUtils.DefaultString(String.Empty, "初期値"), String.Empty);
            Assert.AreEqual(StringUtils.DefaultString(" ", "初期値"), " ");
            Assert.AreEqual(StringUtils.DefaultString("null以外の文字列", "初期値"), "null以外の文字列");
        }
    }
}
