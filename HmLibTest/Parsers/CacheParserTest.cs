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
    using NUnit.Framework;

    /// <summary>
    /// CacheParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class CacheParserTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース（異常系）。
        /// </summary>
        /// <remarks>正常系は他のメソッドのテストの中で実施。</remarks>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorNull()
        {
            new CacheParser(null);
        }

        #endregion

        #region IParserインタフェース実装メソッドテストケース

        /// <summary>
        /// Parseメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestParse()
        {
            // ラップしているパーサーと同じ結果を返すこと
            // ※ キャッシュなのかどうかは検査未実施
            using (CacheParser parser = new CacheParser(new XmlCommentElementParser()))
            {
                Assert.AreEqual("<!-- comment -->", parser.Parse("<!-- comment -->test").ToString());
                Assert.AreEqual("<!-- [[comment]] -->", parser.Parse("<!-- [[comment]] -->test").ToString());
            }
        }

        /// <summary>
        /// Parseメソッドテストケース（異常系）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(FormatException))]
        public void TestParseIgnore()
        {
            // ラップしているパーサーと同じ結果を返すこと
            new CacheParser(new XmlCommentElementParser()).Parse(" <!-- comment -->test");
        }

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            // ラップしているパーサーと同じ結果を返すこと
            // ※ キャッシュなのかどうかは検査未実施
            IElement element;
            using (CacheParser parser = new CacheParser(new XmlCommentElementParser()))
            {
                Assert.IsTrue(parser.TryParse("<!-- comment -->test", out element));
                Assert.AreEqual("<!-- comment -->", element.ToString());

                Assert.IsTrue(parser.TryParse("<!-- [[comment]] -->test", out element));
                Assert.AreEqual("<!-- [[comment]] -->", element.ToString());

                Assert.IsFalse(parser.TryParse(" <!-- comment -->test", out element));
                Assert.IsNull(element);
            }
        }

        /// <summary>
        /// IsPossibleParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestIsPossibleParse()
        {
            // ラップしているパーサーと同じ結果を返すこと
            using (CacheParser parser = new CacheParser(new XmlCommentElementParser()))
            {
                Assert.IsTrue(parser.IsPossibleParse('<'));
                Assert.IsFalse(parser.IsPossibleParse('>'));
                Assert.IsFalse(parser.IsPossibleParse('['));
            }
        }

        #endregion
    }
}
