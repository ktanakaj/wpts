// ================================================================================================
// <summary>
//      AbstractParserのテストクラスソース。</summary>
//
// <copyright file="AbstractParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="AbstractParser"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class AbstractParserTest
    {
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="AbstractParser.Parse"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestParse()
        {
            // TryParseを呼んだ結果が返ること
            TestParser parser = new TestParser();
            parser.Success = true;
            Assert.AreEqual(string.Empty, parser.Parse(string.Empty).ToString());
            Assert.AreEqual("test", parser.Parse("test").ToString());
        }

        /// <summary>
        /// <see cref="AbstractParser.Parse"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestParseNull()
        {
            TestParser parser = new TestParser();
            parser.Success = true;
            parser.Parse(null);
        }

        /// <summary>
        /// <see cref="AbstractParser.Parse"/>メソッドテストケース（解析失敗）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestParseFail()
        {
            // TryParseがfalseを返した場合、例外を投げる
            TestParser parser = new TestParser();
            parser.Success = false;
            parser.Parse(string.Empty);
        }

        /// <summary>
        /// <see cref="AbstractParser.IsPossibleParse"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestIsPossibleParse()
        {
            // このクラスでは何を渡してもtrueが返る
            TestParser parser = new TestParser();
            Assert.IsTrue(parser.IsPossibleParse('a'));
            Assert.IsTrue(parser.IsPossibleParse('<'));
            Assert.IsTrue(parser.IsPossibleParse('>'));
            Assert.IsTrue(parser.IsPossibleParse('='));
            Assert.IsTrue(parser.IsPossibleParse('-'));
            Assert.IsTrue(parser.IsPossibleParse('['));
            Assert.IsTrue(parser.IsPossibleParse('{'));
        }

        #endregion

        #region 実装支援用メソッドテストケース

        /// <summary>
        /// <see cref="AbstractParser.TryParseAt"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestTryParseAt()
        {
            // 指定したパーサーで該当のインデックスの文字を解析する
            IElement element;
            TestParser parser = new TestParser();

            parser.Success = true;
            Assert.IsFalse(parser.TryParseAt("a", 0, out element));
            Assert.IsNull(element);
            Assert.IsTrue(parser.TryParseAt("a", 0, out element, parser));
            Assert.AreEqual("a", element.ToString());

            Assert.IsFalse(parser.TryParseAt("test[[test]]", 4, out element));
            Assert.IsNull(element);
            Assert.IsTrue(parser.TryParseAt("test[[test]]", 4, out element, parser));
            Assert.AreEqual("[[test]]", element.ToString());

            parser.Success = false;
            Assert.IsFalse(parser.TryParseAt("a", 0, out element, parser));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseAt("test[[test]]", 4, out element, parser));
            Assert.IsNull(element);
        }

        /// <summary>
        /// <see cref="AbstractParser.TryParseAt"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTryParseAtNull()
        {
            IElement element;
            TestParser parser = new TestParser();
            parser.Success = true;
            parser.TryParseAt(null, 0, out element);
        }

        /// <summary>
        /// <see cref="AbstractParser.TryParseAt"/>メソッドテストケース（範囲外）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestTryParseAtOutOfRange()
        {
            IElement element;
            TestParser parser = new TestParser();
            parser.Success = true;
            parser.TryParseAt(string.Empty, 1, out element);
        }

        #endregion

        #region テスト用AbstractParser実装

        /// <summary>
        /// テスト用<see cref="AbstractParser"/>実装クラスです。
        /// </summary>
        private class TestParser : AbstractParser
        {
            #region テスト用プロパティ

            /// <summary>
            /// <see cref="TryParse"/>の戻り値。
            /// </summary>
            public bool Success
            {
                get;
                set;
            }

            #endregion

            #region テスト用メソッド実装

            /// <summary>
            /// 渡された文字列の解析を行う。
            /// </summary>
            /// <param name="s">解析対象の文字列。</param>
            /// <param name="result">解析結果。渡された文字列をそのまま要素にして返す。</param>
            /// <returns><see cref="Success"/>の設定値。</returns>
            public override bool TryParse(string s, out IElement result)
            {
                result = new TextElement(s);
                return this.Success;
            }

            #endregion

            #region 非公開メソッドテスト用のオーラーライドメソッド

            /// <summary>
            /// 渡されたテキストの指定されたインデックス位置を各種解析処理で解析する。
            /// </summary>
            /// <param name="s">解析するテキスト。</param>
            /// <param name="index">処理インデックス。</param>
            /// <param name="result">解析した結果要素。</param>
            /// <param name="parsers">解析に用いるパーサー。指定された順に使用。</param>
            /// <returns>いずれかのパーサーで解析できた場合<c>true</c>。</returns>
            /// <exception cref="ArgumentNullException"><paramref name="s"/>または<paramref name="parsers"/>が<c>null</c>の場合。</exception>
            /// <exception cref="ArgumentOutOfRangeException">インデックスが文字列の範囲外の場合。</exception>
            public new bool TryParseAt(string s, int index, out IElement result, params IParser[] parsers)
            {
                return base.TryParseAt(s, index, out result, parsers);
            }

            #endregion
        }

        #endregion
    }
}
