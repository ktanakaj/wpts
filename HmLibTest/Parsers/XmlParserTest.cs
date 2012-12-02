// ================================================================================================
// <summary>
//      XmlParserのテストクラスソース。</summary>
//
// <copyright file="XmlParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="XmlParser"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class XmlParserTest
    {
        #region プロパティテストケース

        /// <summary>
        /// <see cref="XmlParser.Parsers"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestParsers()
        {
            using (XmlParser parser = new XmlParser())
            {
                // 初期状態で値が格納されていること
                Assert.IsNotNull(parser.Parsers);
                Assert.AreEqual(2, parser.Parsers.Length);

                // 設定すればそのオブジェクトが入ること
                IParser[] parsers = new IParser[0];
                parser.Parsers = parsers;
                Assert.AreSame(parsers, parser.Parsers);
            }
        }

        /// <summary>
        /// <see cref="XmlParser.Parsers"/>プロパティテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestParsersNull()
        {
            using (XmlParser parser = new XmlParser())
            {
                parser.Parsers = null;
            }
        }

        /// <summary>
        /// <see cref="XmlParser.IgnoreCase"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestIgnoreCase()
        {
            using (XmlParser parser = new XmlParser())
            {
                // 初期値はtrue、値を設定すればその値に変わる
                Assert.IsTrue(parser.IgnoreCase);
                parser.IgnoreCase = false;
                Assert.IsFalse(parser.IgnoreCase);
                parser.IgnoreCase = true;
                Assert.IsTrue(parser.IgnoreCase);
            }
        }

        /// <summary>
        /// <see cref="XmlParser.IsHtml"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestIsHtml()
        {
            using (XmlParser parser = new XmlParser())
            {
                // 初期値はfalse、値を設定すればその値に変わる
                Assert.IsFalse(parser.IsHtml);
                parser.IsHtml = true;
                Assert.IsTrue(parser.IsHtml);
                parser.IsHtml = false;
                Assert.IsFalse(parser.IsHtml);
            }
        }

        #endregion

        #region IParserインタフェースメソッドテストケース

        // ※ 2012年2月現在、IParser, ITextParserの各メソッド実装は抽象クラス側で共通になっており、
        //    改造部分はどこかでやればテストされるのでそれで割愛

        /// <summary>
        /// <see cref="IParser.Parse"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestParse()
        {
            using (XmlParser parser = new XmlParser())
            {
                Assert.AreEqual("test", parser.Parse("test").ToString());

                IElement element = parser.Parse("testbefore<p>testinner</p><!--comment-->testafter");
                Assert.IsInstanceOfType(element, typeof(ICollection<IElement>));
                ICollection<IElement> collection = (ICollection<IElement>)element;
                Assert.AreEqual(4, collection.Count);
                Assert.AreEqual("testbefore", collection.ElementAt(0).ToString());
                Assert.IsInstanceOfType(collection.ElementAt(1), typeof(XmlElement));
                Assert.AreEqual("<p>testinner</p>", collection.ElementAt(1).ToString());
                Assert.IsInstanceOfType(collection.ElementAt(2), typeof(XmlCommentElement));
                Assert.AreEqual("<!--comment-->", collection.ElementAt(2).ToString());
                Assert.AreEqual("testafter", collection.ElementAt(3).ToString());
            }
        }

        /// <summary>
        /// <see cref="IParser.Parse"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestParseNull()
        {
            using (XmlParser parser = new XmlParser())
            {
                parser.Parse(null);
            }
        }

        /// <summary>
        /// <see cref="IParser.Parse"/>メソッドテストケース（Dispose）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestParseDispose()
        {
            XmlParser parser = new XmlParser();
            parser.Dispose();
            IElement result;
            parser.TryParse("test", out result);
        }

        #endregion

        #region IDisposableインタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="XmlParser.Dispose"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestDispose()
        {
            // 循環参照のあるParsersを解放する
            XmlParser parser = new XmlParser();
            Assert.IsNotNull(parser.Parsers);
            parser.Dispose();
            Assert.IsNull(parser.Parsers);
        }

        #endregion
    }
}
