﻿// ================================================================================================
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
    using NUnit.Framework;

    /// <summary>
    /// StringUtilsのテストクラスです。
    /// </summary>
    [TestFixture]
    public class StringUtilsTest
    {
        #region 初期化メソッドテストケース

        /// <summary>
        /// DefaultStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestDefaultString()
        {
            // 引数一つ
            Assert.AreEqual(String.Empty, StringUtils.DefaultString(null));
            Assert.AreEqual(String.Empty, StringUtils.DefaultString(String.Empty));
            Assert.AreEqual(" ", StringUtils.DefaultString(" "));
            Assert.AreEqual("null以外の文字列", StringUtils.DefaultString("null以外の文字列"));

            // 引数二つ
            Assert.AreEqual("初期値", StringUtils.DefaultString(null, "初期値"));
            Assert.AreEqual(String.Empty, StringUtils.DefaultString(String.Empty, "初期値"));
            Assert.AreEqual(" ", StringUtils.DefaultString(" ", "初期値"));
            Assert.AreEqual("null以外の文字列", StringUtils.DefaultString("null以外の文字列", "初期値"));
        }

        #endregion

        #region 文字列チェックテストケース

        /// <summary>
        /// StartsWithメソッドテストケース。
        /// </summary>
        [Test]
        public void TestStartsWith()
        {
            // null
            Assert.IsTrue(StringUtils.StartsWith(null, null, 3));
            Assert.IsFalse(StringUtils.StartsWith(null, "", 2));
            Assert.IsFalse(StringUtils.StartsWith("", null, 5));

            // 空、文字数
            Assert.IsFalse(StringUtils.StartsWith("", "", 0));
            Assert.IsTrue(StringUtils.StartsWith("a", "", 0));
            Assert.IsTrue(StringUtils.StartsWith("abcedf0123あいうえお", "", 14));
            Assert.IsFalse(StringUtils.StartsWith("abcedf0123あいうえお", "", 15));
            Assert.IsFalse(StringUtils.StartsWith("abcedf0123あいうえお", "", -1));

            // 通常
            Assert.IsTrue(StringUtils.StartsWith("abcedf0123あいうえお", "bc", 1));
            Assert.IsFalse(StringUtils.StartsWith("abcedf0123あいうえお", "ab", 1));
            Assert.IsTrue(StringUtils.StartsWith("abcedf0123あいうえお", "あいうえお", 10));
            Assert.IsFalse(StringUtils.StartsWith("abcedf0123あいうえお", "あいうえおか", 10));
        }

        #endregion
    }
}
