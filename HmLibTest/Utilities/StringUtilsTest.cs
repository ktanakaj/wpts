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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="StringUtils"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class StringUtilsTest
    {
        #region 初期化メソッドテストケース

        /// <summary>
        /// DefaultStringメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestDefaultString()
        {
            // 引数一つ
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(null));
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(string.Empty));
            Assert.AreEqual(" ", StringUtils.DefaultString(" "));
            Assert.AreEqual("null以外の文字列", StringUtils.DefaultString("null以外の文字列"));

            // 引数二つ
            Assert.AreEqual("初期値", StringUtils.DefaultString(null, "初期値"));
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(string.Empty, "初期値"));
            Assert.AreEqual(" ", StringUtils.DefaultString(" ", "初期値"));
            Assert.AreEqual("null以外の文字列", StringUtils.DefaultString("null以外の文字列", "初期値"));
        }

        #endregion

        #region 切り出しメソッドテストケース

        /// <summary>
        /// Substringメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestSubstring()
        {
            // 引数一つ
            Assert.IsNull(StringUtils.Substring(null, 0));
            Assert.AreEqual("abc", StringUtils.Substring("abc", 0));
            Assert.AreEqual("c", StringUtils.Substring("abc", 2));
            Assert.AreEqual(string.Empty, StringUtils.Substring("abc", 4));
            Assert.AreEqual("abc", StringUtils.Substring("abc", -2));
            Assert.AreEqual("abc", StringUtils.Substring("abc", -4));
            Assert.AreEqual("3", StringUtils.Substring("0123", 3));
            Assert.AreEqual(string.Empty, StringUtils.Substring("0123", 4));

            // 引数二つ
            Assert.IsNull(StringUtils.Substring(null, 0, 0));
            Assert.AreEqual(string.Empty, StringUtils.Substring(string.Empty, 0, 1));
            Assert.AreEqual("ab", StringUtils.Substring("abc", 0, 2));
            Assert.AreEqual(string.Empty, StringUtils.Substring("abc", 2, 0));
            Assert.AreEqual("c", StringUtils.Substring("abc", 2, 2));
            Assert.AreEqual(string.Empty, StringUtils.Substring("abc", 4, 2));
            Assert.AreEqual(string.Empty, StringUtils.Substring("abc", -2, -1));
            Assert.AreEqual("ab", StringUtils.Substring("abc", -4, 2));
            Assert.AreEqual("3", StringUtils.Substring("0123", 3, 1));
            Assert.AreEqual("3", StringUtils.Substring("0123", 3, 2));
            Assert.AreEqual(string.Empty, StringUtils.Substring("0123", 4, 1));
        }

        #endregion

        #region 文字列チェックテストケース

        /// <summary>
        /// StartsWithメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestStartsWith()
        {
            // null
            Assert.IsTrue(StringUtils.StartsWith(null, null, 3));
            Assert.IsFalse(StringUtils.StartsWith(null, string.Empty, 2));
            Assert.IsFalse(StringUtils.StartsWith(string.Empty, null, 5));

            // 空、文字数
            Assert.IsFalse(StringUtils.StartsWith(string.Empty, string.Empty, 0));
            Assert.IsTrue(StringUtils.StartsWith("a", string.Empty, 0));
            Assert.IsTrue(StringUtils.StartsWith("abcedf0123あいうえお", string.Empty, 14));
            Assert.IsFalse(StringUtils.StartsWith("abcedf0123あいうえお", string.Empty, 15));
            Assert.IsFalse(StringUtils.StartsWith("abcedf0123あいうえお", string.Empty, -1));

            // 通常
            Assert.IsTrue(StringUtils.StartsWith("abcedf0123あいうえお", "bc", 1));
            Assert.IsFalse(StringUtils.StartsWith("abcedf0123あいうえお", "ab", 1));
            Assert.IsTrue(StringUtils.StartsWith("abcedf0123あいうえお", "あいうえお", 10));
            Assert.IsFalse(StringUtils.StartsWith("abcedf0123あいうえお", "あいうえおか", 10));
        }

        /// <summary>
        /// StartsWithメソッドテストケース（性能試験）。
        /// </summary>
        [TestMethod, Timeout(1500)]
        public void TestStartsWithResponse()
        {
            // テストデータとして適当な、ただしある文字が定期的に出現する長い文字列を生成
            StringBuilder b = new StringBuilder();
            int span = 0x7D - 0x20;
            for (int i = 0; i < 100000; i++)
            {
                b.Append(char.ConvertFromUtf32((i % span) + 0x20));
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
        [TestMethod]
        public void TestFormatDollarVariable()
        {
            // 空文字列
            Assert.AreEqual(string.Empty, StringUtils.FormatDollarVariable(string.Empty));
            Assert.AreEqual(string.Empty, StringUtils.FormatDollarVariable(string.Empty, string.Empty));

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
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFormatDollarVariableFormatNull()
        {
            StringUtils.FormatDollarVariable(null);
        }

        /// <summary>
        /// FormatDollarVariableメソッドテストケース（パラメータがnull）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFormatDollarVariableArgsNull()
        {
            StringUtils.FormatDollarVariable(string.Empty, null);
        }

        #endregion

        #region 比較メソッドテストケース

        /// <summary>
        /// CompareNullsLastメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestCompareNullsLast()
        {
            // 通常のString.Compareと同じ動作
            Assert.AreEqual(-1, StringUtils.CompareNullsLast("abc", "abd"));
            Assert.AreEqual(1, StringUtils.CompareNullsLast("abd", "abc"));
            Assert.AreEqual(0, StringUtils.CompareNullsLast("abc", "abc"));
            Assert.AreEqual(-1, StringUtils.CompareNullsLast("ab", "abc"));
            Assert.AreEqual(1, StringUtils.CompareNullsLast("abc", "ab"));
            Assert.AreEqual(0, StringUtils.CompareNullsLast(null, null));
            Assert.AreEqual(0, StringUtils.CompareNullsLast(string.Empty, string.Empty));

            // 独自の拡張部分、nullや空の値が大きいと判断される
            Assert.AreEqual(-1, StringUtils.CompareNullsLast("abc", null));
            Assert.AreEqual(1, StringUtils.CompareNullsLast(null, "abc"));
            Assert.AreEqual(-1, StringUtils.CompareNullsLast("abc", string.Empty));
            Assert.AreEqual(1, StringUtils.CompareNullsLast(string.Empty, "abc"));

            // nullと空の場合nullの方が大きいと判定
            Assert.AreEqual(1, StringUtils.CompareNullsLast(null, string.Empty));
            Assert.AreEqual(-1, StringUtils.CompareNullsLast(string.Empty, null));
        }

        #endregion
    }
}
