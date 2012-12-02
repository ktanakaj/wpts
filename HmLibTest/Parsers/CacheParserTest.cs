// ================================================================================================
// <summary>
//      CacheParserのテストクラスソース。</summary>
//
// <copyright file="CacheParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="CacheParser"/>のテストクラスです。
    /// </summary>
    [TestClass]
    internal class CacheParserTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース（異常系）。
        /// </summary>
        /// <remarks>正常系は他のメソッドのテストの中で実施。</remarks>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorNull()
        {
            new CacheParser(null);
        }

        #endregion

        #region IParserインタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="CacheParser.Parse"/>メソッドテストケース（正常系）。
        /// </summary>
        [TestMethod]
        public void TestParse()
        {
            // ラップしているパーサーと同じ結果を返すこと
            // （一度目は同じ値で、二度目は一度目と同じオブジェクトで）
            XmlCommentElementParser child = new XmlCommentElementParser();
            CacheParser parser = new CacheParser(child);
            IElement element;
            IElement diff;
            string text;

            text = "<!-- comment -->test";
            element = parser.Parse(text);
            diff = child.Parse(text);
            Assert.AreEqual("<!-- comment -->", element.ToString());
            Assert.AreEqual(diff.ToString(), element.ToString());
            Assert.AreNotSame(diff, element);
            Assert.AreSame(element, parser.Parse(text));

            text = "<!-- [[comment]] -->test";
            element = parser.Parse(text);
            diff = child.Parse(text);
            Assert.AreEqual("<!-- [[comment]] -->", element.ToString());
            Assert.AreEqual(diff.ToString(), element.ToString());
            Assert.AreNotSame(diff, element);
            Assert.AreSame(element, parser.Parse(text));
        }

        /// <summary>
        /// <see cref="CacheParser.Parse"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestParseNull()
        {
            // nullはどのParserをラップしている場合も自前で例外
            new CacheParser(new XmlCommentElementParser()).Parse(null);
        }

        /// <summary>
        /// <see cref="CacheParser.Parse"/>メソッドテストケース（解析失敗）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestParseIgnore()
        {
            // ラップしているパーサーと同じ例外を投げること
            new CacheParser(new XmlCommentElementParser()).Parse(" <!-- comment -->test");
        }

        /// <summary>
        /// <see cref="CacheParser.TryParse"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestTryParse()
        {
            // ラップしているパーサーと同じ結果を返すこと
            // （一度目は同じ値で、二度目は一度目と同じオブジェクトで）
            XmlCommentElementParser child = new XmlCommentElementParser();
            CacheParser parser = new CacheParser(child);
            IElement element;
            IElement diff;
            string text;

            text = "<!-- comment -->test";
            Assert.IsTrue(parser.TryParse(text, out element));
            Assert.IsTrue(child.TryParse(text, out diff));
            Assert.AreEqual("<!-- comment -->", element.ToString());
            Assert.AreEqual(diff.ToString(), element.ToString());
            Assert.AreNotSame(diff, element);
            Assert.AreEqual(child.TryParse(text, out diff), parser.TryParse(text, out element));
            Assert.IsTrue(parser.TryParse(text, out diff));
            Assert.AreSame(element, diff);

            text = "<!-- [[comment]] -->test";
            Assert.IsTrue(parser.TryParse(text, out element));
            Assert.IsTrue(child.TryParse(text, out diff));
            Assert.AreEqual("<!-- [[comment]] -->", element.ToString());
            Assert.AreEqual(diff.ToString(), element.ToString());
            Assert.AreNotSame(diff, element);
            Assert.AreEqual(child.TryParse(text, out diff), parser.TryParse(text, out element));
            Assert.IsTrue(parser.TryParse(text, out diff));
            Assert.AreSame(element, diff);

            text = " <!-- comment -->test";
            Assert.IsFalse(parser.TryParse(text, out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse(text, out element));
            Assert.IsNull(element);

            text = null;
            Assert.IsFalse(parser.TryParse(text, out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse(text, out element));
            Assert.IsNull(element);
        }

        /// <summary>
        /// <see cref="CacheParser.IsPossibleParse"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestIsPossibleParse()
        {
            // ラップしているパーサーと同じ結果を返すこと
            XmlCommentElementParser child = new XmlCommentElementParser();
            CacheParser parser = new CacheParser(child);
            char c;

            c = '<';
            Assert.IsTrue(parser.IsPossibleParse(c));
            Assert.AreEqual(child.IsPossibleParse(c), parser.IsPossibleParse(c));

            c = '>';
            Assert.IsFalse(parser.IsPossibleParse(c));
            Assert.AreEqual(child.IsPossibleParse(c), parser.IsPossibleParse(c));

            c = '[';
            Assert.IsFalse(parser.IsPossibleParse(c));
            Assert.AreEqual(child.IsPossibleParse(c), parser.IsPossibleParse(c));
        }

        #endregion
    }
}
