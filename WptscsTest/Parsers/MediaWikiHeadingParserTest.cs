// ================================================================================================
// <summary>
//      MediaWikiHeadingParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiHeadingParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using System.Collections.Generic;
    using Honememo.Parsers;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Websites;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="MediaWikiHeadingParser"/>のテストクラスです。
    /// </summary>
    [TestClass]
    internal class MediaWikiHeadingParserTest
    {
        #region private変数

        /// <summary>
        /// 前処理・後処理で生成／解放される言語別の<see cref="MediaWikiParser"/>。
        /// </summary>
        private static IDictionary<string, MediaWikiParser> mediaWikiParsers = new Dictionary<string, MediaWikiParser>();

        #endregion

        #region 前処理・後処理

        /// <summary>
        /// テストの前処理。
        /// </summary>
        /// <param name="context">テスト用情報。</param>
        /// <remarks><see cref="MediaWikiParser.Dispose"/>が必要な<see cref="MediaWikiParser"/>の生成。</remarks>
        [ClassInitialize]
        public static void SetUpBeforeClass(TestContext context)
        {
            mediaWikiParsers["en"] = new MediaWikiParser(new MockFactory().GetMediaWiki("en"));
        }

        /// <summary>
        /// テストの後処理。
        /// </summary>
        /// <remarks><see cref="MediaWikiParser.Dispose"/>が必要な<see cref="MediaWikiParser"/>の解放。</remarks>
        [ClassCleanup]
        public static void TearDownAfterClass()
        {
            foreach (IDisposable parser in mediaWikiParsers.Values)
            {
                parser.Dispose();
            }

            mediaWikiParsers.Clear();
        }

        #endregion

        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            // TODO: ちゃんと設定されているかも確認する？
            MediaWikiHeadingParser parser = new MediaWikiHeadingParser(
                new MediaWikiParser(new MockFactory().GetMediaWiki("en")));
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiHeadingParser.TryParse"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestTryParse()
        {
            IElement element;
            MediaWikiHeading heading;
            MediaWikiHeadingParser parser = new MediaWikiHeadingParser(mediaWikiParsers["en"]);

            // 基本形
            Assert.IsTrue(parser.TryParse("==test==", out element));
            Assert.IsInstanceOfType(element, typeof(MediaWikiHeading));
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("==test==", heading.ToString());
            Assert.AreEqual(2, heading.Level);
            Assert.AreEqual(1, heading.Count);
            Assert.AreEqual("test", heading[0].ToString());

            // 後ろが改行
            Assert.IsTrue(parser.TryParse("== test == \r\ntest", out element));
            Assert.IsInstanceOfType(element, typeof(MediaWikiHeading));
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("== test == ", heading.ToString());
            Assert.AreEqual(2, heading.Level);
            Assert.AreEqual(1, heading.Count);
            Assert.AreEqual(" test ", heading[0].ToString());

            // 改行以外はNG
            Assert.IsFalse(parser.TryParse("== test == test", out element));
            Assert.IsNull(element);

            // 複数の要素を含む
            Assert.IsTrue(parser.TryParse("===[[TestMethod]] and sample===", out element));
            Assert.IsInstanceOfType(element, typeof(MediaWikiHeading));
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("===[[TestMethod]] and sample===", heading.ToString());
            Assert.AreEqual(3, heading.Level);
            Assert.AreEqual(2, heading.Count);
            Assert.AreEqual("[[TestMethod]]", heading[0].ToString());
            Assert.IsInstanceOfType(heading[0], typeof(MediaWikiLink));
            Assert.AreEqual(" and sample", heading[1].ToString());

            // 前後で数が違うのはOK、少ない側の階層と判定
            Assert.IsTrue(parser.TryParse("=test==", out element));
            Assert.IsInstanceOfType(element, typeof(MediaWikiHeading));
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("=test==", heading.ToString());
            Assert.AreEqual(1, heading.Level);

            // 前後で数が違うの逆パターン
            Assert.IsTrue(parser.TryParse("====test==", out element));
            Assert.IsInstanceOfType(element, typeof(MediaWikiHeading));
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("====test==", heading.ToString());
            Assert.AreEqual(2, heading.Level);

            // 先頭が = 以外はNG
            Assert.IsFalse(parser.TryParse(" ==test==", out element));
            Assert.IsNull(element);

            // 内部要素に改行を含むのはOK
            Assert.IsTrue(parser.TryParse("== {{lang\n|ja|見出し}} ==\n", out element));
            Assert.IsInstanceOfType(element, typeof(MediaWikiHeading));
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("== {{lang\n|ja|見出し}} ==", heading.ToString());
            Assert.AreEqual(2, heading.Level);
            Assert.AreEqual(3, heading.Count);
            Assert.AreEqual(" ", heading[0].ToString());
            Assert.AreEqual("{{lang\n|ja|見出し}}", heading[1].ToString());
            Assert.IsInstanceOfType(heading[1], typeof(MediaWikiTemplate));
            Assert.AreEqual(" ", heading[2].ToString());

            // 空・null
            Assert.IsFalse(parser.TryParse(string.Empty, out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParse(null, out element));
            Assert.IsNull(element);
        }

        /// <summary>
        /// <see cref="MediaWikiHeadingParser.TryParse"/>メソッドテストケース（コメント）。
        /// </summary>
        [TestMethod]
        public void TestTryParseComment()
        {
            IElement element;
            MediaWikiHeading heading;
            MediaWikiHeadingParser parser = new MediaWikiHeadingParser(mediaWikiParsers["en"]);

            // ↓1.01以前のバージョンで対応していたコメント、中のコメントが認識されなかった
            // // こんな無茶なコメントも一応対応
            // Assert.IsTrue(parser.TryParse("<!--test-->=<!--test-->=関連項目<!--test-->==<!--test-->\n", out element));
            // Assert.IsInstanceOfType(element, typeof(MediaWikiHeading));
            // heading = (MediaWikiHeading)element;
            // Assert.AreEqual("<!--test-->=<!--test-->=関連項目<!--test-->==<!--test-->", heading.ToString());
            // Assert.AreEqual(2, heading.Level);
            // // TODO: 本当は2でコメントも見つけるべきだが、コメントは中も除外しているので現状1
            // // Assert.AreEqual(2, heading.Count);
            // // Assert.AreEqual("関連項目", heading[0].ToString());
            // // Assert.AreEqual("<!--test-->", heading[1].ToString());
            // Assert.AreEqual(1, heading.Count);
            // Assert.AreEqual("関連項目", heading[0].ToString());
            // ↓1.10改修後での動作
            Assert.IsFalse(parser.TryParse("<!--test-->=<!--test-->=関連項目<!--test-->==<!--test-->\n", out element));
            Assert.IsTrue(parser.TryParse("=<!--test-->=関連項目<!--test-->==\n", out element));
            Assert.IsInstanceOfType(element, typeof(MediaWikiHeading));
            heading = (MediaWikiHeading)element;
            Assert.AreEqual(1, heading.Level);
            Assert.AreEqual(4, heading.Count);
            Assert.AreEqual("<!--test-->", heading[0].ToString());
            Assert.AreEqual("=関連項目", heading[1].ToString());
            Assert.AreEqual("<!--test-->", heading[2].ToString());
            Assert.AreEqual("=", heading[3].ToString());
            Assert.IsFalse(parser.TryParse("==関連項目<!--test-->==<!--test-->\n", out element));

            // ↓1.10改修後に対応しているコメント、変なところのコメントは駄目だが中のものを認識する
            Assert.IsTrue(parser.TryParse("==<!--test1-->関連項目<!--test2-->==\n", out element));
            Assert.IsInstanceOfType(element, typeof(MediaWikiHeading));
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("==<!--test1-->関連項目<!--test2-->==", heading.ToString());
            Assert.AreEqual(2, heading.Level);
            Assert.AreEqual(3, heading.Count);
            Assert.AreEqual("<!--test1-->", heading[0].ToString());
            Assert.IsInstanceOfType(heading[0], typeof(XmlCommentElement));
            Assert.AreEqual("関連項目", heading[1].ToString());
            Assert.AreEqual("<!--test2-->", heading[2].ToString());
        }

        #endregion
    }
}
