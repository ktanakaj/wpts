// ================================================================================================
// <summary>
//      AbstractTextParserのテストクラスソース。</summary>
//
// <copyright file="AbstractTextParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Honememo.Utilities;
    using NUnit.Framework;

    /// <summary>
    /// <see cref="AbstractTextParser"/>のテストクラスです。
    /// </summary>
    [TestFixture]
    internal class AbstractTextParserTest
    {
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="AbstractTextParser.TryParseToEndCondition"/>メソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseToEndCondition()
        {
            IElement element;
            TestTextParser parser = new TestTextParser();

            // conditionの指定がないときは、文字列を最後までTryParseElementAtで解析
            parser.Success = true;
            Assert.IsFalse(parser.TryParseToEndCondition(null, null, out element));
            Assert.IsNull(element);

            Assert.IsTrue(parser.TryParseToEndCondition(String.Empty, null, out element));
            Assert.AreEqual(String.Empty, element.ToString());
            Assert.IsInstanceOf(typeof(TextElement), element);

            Assert.IsTrue(parser.TryParseToEndCondition("0123456789", null, out element));
            Assert.AreEqual("0123456789", element.ToString());
            Assert.IsInstanceOf(typeof(ListElement), element);
            ListElement list = (ListElement)element;
            Assert.AreEqual(10, list.Count);
            foreach (IElement e in list)
            {
                Assert.IsInstanceOf(typeof(TextElement), e);
            }

            // conditionが指定されている場合は、その条件を満たすまで
            Assert.IsTrue(parser.TryParseToEndCondition(
                "0123456789",
                (string s, int index) => s[index] == '5',
                out element));
            Assert.AreEqual("01234", element.ToString());
            Assert.IsInstanceOf(typeof(ListElement), element);
        }

        /// <summary>
        /// <see cref="AbstractTextParser.TryParseToDelimiter"/>メソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseToDelimiter()
        {
            IElement element;
            TestTextParser parser = new TestTextParser();

            // delimitersの指定がないときは、condition無しのTryParseToEndConditionと同じ
            parser.Success = true;
            Assert.IsFalse(parser.TryParseToDelimiter(null, out element));
            Assert.IsNull(element);

            Assert.IsTrue(parser.TryParseToDelimiter(String.Empty, out element));
            Assert.AreEqual(String.Empty, element.ToString());
            Assert.IsInstanceOf(typeof(TextElement), element);

            Assert.IsTrue(parser.TryParseToDelimiter("[[test]] is good", out element));
            Assert.AreEqual("[[test]] is good", element.ToString());
            Assert.IsInstanceOf(typeof(ListElement), element);

            // delimitersが指定されている場合は、その文字列まで
            // ※ 本当は "test]] is good" にした状態で用いる
            Assert.IsTrue(parser.TryParseToDelimiter("[[test]] is good", out element, "]]"));
            Assert.AreEqual("[[test", element.ToString());
            Assert.IsInstanceOf(typeof(ListElement), element);

            // delimitersは複数指定可能、先に見つけたもの優先
            Assert.IsTrue(parser.TryParseToDelimiter("[[test]] is good", out element, "]]", "s"));
            Assert.AreEqual("[[te", element.ToString());
            Assert.IsInstanceOf(typeof(ListElement), element);

            // delimitersの指定があっても見つからないときは最後まで処理する
            Assert.IsTrue(parser.TryParseToDelimiter("[[test]] is good", out element, "}}"));
            Assert.AreEqual("[[test]] is good", element.ToString());
            Assert.IsInstanceOf(typeof(ListElement), element);
        }

        /// <summary>
        /// <see cref="AbstractTextParser.TryParseToDelimiter"/>メソッドテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTryParseToDelimiterNull()
        {
            IElement element;
            string[] delimiters = null;
            new TestTextParser().TryParseToDelimiter(null, out element, delimiters);
        }

        /// <summary>
        /// <see cref="AbstractTextParser.TryParse"/>メソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            IElement element;
            TestTextParser parser = new TestTextParser();

            // condition無しのTryParseToEndConditionと同じ
            parser.Success = true;
            Assert.IsFalse(parser.TryParse(null, out element));
            Assert.IsNull(element);

            Assert.IsTrue(parser.TryParse(String.Empty, out element));
            Assert.AreEqual(String.Empty, element.ToString());
            Assert.IsInstanceOf(typeof(TextElement), element);

            Assert.IsTrue(parser.TryParse("0123456789", out element));
            Assert.AreEqual("0123456789", element.ToString());
            Assert.IsInstanceOf(typeof(ListElement), element);
            ListElement list = (ListElement)element;
        }

        #endregion

        #region 実装支援用メソッドテストケース

        /// <summary>
        /// <see cref="AbstractTextParser.FlashText"/>メソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestFlashText()
        {
            // ビルダーに値が詰まっている場合、その内容をリストに追加してクリアする
            ListElement list = new ListElement();
            StringBuilder b = new StringBuilder();
            TestTextParser parser = new TestTextParser();

            parser.FlashText(ref list, ref b);
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(String.Empty, b.ToString());

            b.Append("1st string");
            parser.FlashText(ref list, ref b);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("1st string", list[0].ToString());
            Assert.IsInstanceOf(typeof(TextElement), list[0]);
            Assert.AreEqual(String.Empty, b.ToString());

            b.Append("2nd string");
            parser.FlashText(ref list, ref b);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("1st string", list[0].ToString());
            Assert.AreEqual("2nd string", list[1].ToString());
            Assert.IsInstanceOf(typeof(TextElement), list[1]);
            Assert.AreEqual(String.Empty, b.ToString());
        }

        /// <summary>
        /// <see cref="AbstractTextParser.FlashText"/>メソッドテストケース（リストがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFlashTextListNull()
        {
            ListElement list = null;
            StringBuilder b = new StringBuilder();
            new TestTextParser().FlashText(ref list, ref b);
        }

        /// <summary>
        /// <see cref="AbstractTextParser.FlashText"/>メソッドテストケース（ビルダーがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFlashTextBNull()
        {
            ListElement list = new ListElement();
            StringBuilder b = null;
            new TestTextParser().FlashText(ref list, ref b);
        }

        #endregion

        #region テスト用AbstractTextParser実装

        /// <summary>
        /// テスト用<see cref="AbstractTextParser"/>実装クラスです。
        /// </summary>
        private class TestTextParser : AbstractTextParser
        {
            #region テスト用プロパティ

            /// <summary>
            /// <see cref="TryParseElementAt"/>の戻り値。
            /// </summary>
            public bool Success
            {
                get;
                set;
            }

            #endregion

            #region 非公開メソッドテスト用のオーラーライドメソッド

            /// <summary>
            /// 文字列が空でない場合、リストにText要素を追加して、文字列をリセットする。
            /// </summary>
            /// <param name="list">追加されるリスト。</param>
            /// <param name="b">追加する文字列。</param>
            /// <exception cref="ArgumentNullException"><paramref name="list"/>または<paramref name="b"/>が<c>null</c>の場合。</exception>
            public new void FlashText(ref ListElement list, ref StringBuilder b)
            {
                base.FlashText(ref list, ref b);
            }

            #endregion

            #region テスト用メソッド実装

            /// <summary>
            /// 渡されたテキストの指定されたインデックス位置を各種解析処理で解析する。
            /// </summary>
            /// <param name="s">解析するテキスト。</param>
            /// <param name="index">処理インデックス。</param>
            /// <param name="result">解析結果。渡された文字列のインデックス位置の値を要素にして返す。</param>
            /// <returns><see cref="Success"/>の設定値。</returns>
            protected override bool TryParseElementAt(string s, int index, out IElement result)
            {
                result = new TextElement(s[index].ToString());
                return this.Success;
            }

            #endregion
        }

        #endregion
    }
}
