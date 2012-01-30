// ================================================================================================
// <summary>
//      StringUtilsのテストクラスソース。</summary>
//
// <copyright file="StringUtilsTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Text;
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
            Assert.IsEmpty(StringUtils.DefaultString(null));
            Assert.IsEmpty(StringUtils.DefaultString(String.Empty));
            Assert.AreEqual(" ", StringUtils.DefaultString(" "));
            Assert.AreEqual("null以外の文字列", StringUtils.DefaultString("null以外の文字列"));

            // 引数二つ
            Assert.AreEqual("初期値", StringUtils.DefaultString(null, "初期値"));
            Assert.IsEmpty(StringUtils.DefaultString(String.Empty, "初期値"));
            Assert.AreEqual(" ", StringUtils.DefaultString(" ", "初期値"));
            Assert.AreEqual("null以外の文字列", StringUtils.DefaultString("null以外の文字列", "初期値"));
        }

        #endregion

        #region 切り出しメソッドテストケース

        /// <summary>
        /// Substringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestSubstring()
        {
            // 引数一つ
            Assert.IsNull(StringUtils.Substring(null, 0));
            Assert.AreEqual("abc", StringUtils.Substring("abc", 0));
            Assert.AreEqual("c", StringUtils.Substring("abc", 2));
            Assert.IsEmpty(StringUtils.Substring("abc", 4));
            Assert.AreEqual("abc", StringUtils.Substring("abc", -2));
            Assert.AreEqual("abc", StringUtils.Substring("abc", -4));
            Assert.AreEqual("3", StringUtils.Substring("0123", 3));
            Assert.IsEmpty(StringUtils.Substring("0123", 4));

            // 引数二つ
            Assert.IsNull(StringUtils.Substring(null, 0, 0));
            Assert.IsEmpty(StringUtils.Substring(String.Empty, 0, 1));
            Assert.AreEqual("ab", StringUtils.Substring("abc", 0, 2));
            Assert.IsEmpty(StringUtils.Substring("abc", 2, 0));
            Assert.AreEqual("c", StringUtils.Substring("abc", 2, 2));
            Assert.IsEmpty(StringUtils.Substring("abc", 4, 2));
            Assert.IsEmpty(StringUtils.Substring("abc", -2, -1));
            Assert.AreEqual("ab", StringUtils.Substring("abc", -4, 2));
            Assert.AreEqual("3", StringUtils.Substring("0123", 3, 1));
            Assert.AreEqual("3", StringUtils.Substring("0123", 3, 2));
            Assert.IsEmpty(StringUtils.Substring("0123", 4, 1));
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

        /// <summary>
        /// StartsWithメソッドテストケース（性能試験）。
        /// </summary>
        [Test, Timeout(1500)]
        public void TestStartsWithResponse()
        {
            // テストデータとして適当な、ただしある文字が定期的に出現する長い文字列を生成
            StringBuilder b = new StringBuilder();
            int span = 0x7D - 0x20;
            for (int i = 0; i < 100000; i++)
            {
                b.Append(Char.ConvertFromUtf32(i % span + 0x20));
            }

            // 先頭から最後までひたすら実行して時間がかかりすぎないかをチェック
            string s = b.ToString();
            for (int i = 0; i < s.Length; i++)
            {
                StringUtils.StartsWith(s, "a", i);
            }
        }

        #endregion

        #region 書式化メソッドテストケース

        /// <summary>
        /// FormatDollarVariableメソッドテストケース。
        /// </summary>
        [Test]
        public void TestFormatDollarVariable()
        {
            // 空文字列
            Assert.IsEmpty(StringUtils.FormatDollarVariable(String.Empty));
            Assert.IsEmpty(StringUtils.FormatDollarVariable(String.Empty, String.Empty));

            // 通常
            Assert.AreEqual("test", StringUtils.FormatDollarVariable("test"));
            Assert.AreEqual("testtest", StringUtils.FormatDollarVariable("test$1test"));
            Assert.AreEqual("test15test", StringUtils.FormatDollarVariable("test$1test", 15));
            Assert.AreEqual("testtest", StringUtils.FormatDollarVariable("test$2test", 15));
            Assert.AreEqual(
                "int[] value = {30, {0}, 10000};\nstring[] = {\"文字列$1\", \"12.345\"};\n",
                StringUtils.FormatDollarVariable("int[] value = {$1, $2, $3};\nstring[] = {\"$4\", \"$5\"};\n", 30, "{0}", 10000, "文字列$1", 12.345));
        }

        /// <summary>
        /// FormatDollarVariableメソッドテストケース（書式がnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFormatDollarVariableFormatNull()
        {
            StringUtils.FormatDollarVariable(null);
        }

        /// <summary>
        /// FormatDollarVariableメソッドテストケース（パラメータがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFormatDollarVariableArgsNull()
        {
            StringUtils.FormatDollarVariable(String.Empty, null);
        }

        #endregion
    }
}
