// ================================================================================================
// <summary>
//      XmlParserのテストクラスソース。</summary>
//
// <copyright file="XmlParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    /// <summary>
    /// XmlParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class XmlParserTest
    {
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// Parseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestParse()
        {
            // ※ 現状解析が失敗するパターンは無い
            XmlParser parser = new XmlParser();

            Assert.AreEqual("test", parser.Parse("test").ToString());

            IElement element = parser.Parse("testbefore<p>testinner</p><!--comment-->testafter");
            Assert.IsInstanceOf(typeof(ICollection<IElement>), element);
            ICollection<IElement> collection = (ICollection<IElement>)element;
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual("testbefore", collection.ElementAt(0).ToString());
            Assert.IsInstanceOf(typeof(XmlElement), collection.ElementAt(1));
            Assert.AreEqual("<p>testinner</p>", collection.ElementAt(1).ToString());
            Assert.IsInstanceOf(typeof(XmlCommentElement), collection.ElementAt(2));
            Assert.AreEqual("<!--comment-->", collection.ElementAt(2).ToString());
            Assert.AreEqual("testafter", collection.ElementAt(3).ToString());
        }

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            // ※ 現状解析が失敗するパターンは無い
            IElement element;
            XmlParser parser = new XmlParser();

            Assert.IsTrue(parser.TryParse("test", out element));
            Assert.IsInstanceOf(typeof(TextElement), element);
            Assert.AreEqual("test", element.ToString());

            Assert.IsTrue(parser.TryParse("testbefore<p>testinner</p><!--comment-->testafter", out element));
            Assert.IsInstanceOf(typeof(ICollection<IElement>), element);
            ICollection<IElement> collection = (ICollection<IElement>)element;
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual("testbefore", collection.ElementAt(0).ToString());
            Assert.IsInstanceOf(typeof(XmlElement), collection.ElementAt(1));
            Assert.AreEqual("<p>testinner</p>", collection.ElementAt(1).ToString());
            Assert.IsInstanceOf(typeof(XmlCommentElement), collection.ElementAt(2));
            Assert.AreEqual("<!--comment-->", collection.ElementAt(2).ToString());
            Assert.AreEqual("testafter", collection.ElementAt(3).ToString());
        }

        #endregion
    }
}
