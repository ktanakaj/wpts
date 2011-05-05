﻿// ================================================================================================
// <summary>
//      MediaWikiHeadingParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiHeadingParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using NUnit.Framework;
    using Honememo.Parsers;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// MediaWikiHeadingParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiHeadingParserTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            MediaWikiHeadingParser parser = new MediaWikiHeadingParser(
                new MediaWikiParser(new MockFactory().GetMediaWiki("en")));
            // TODO: ちゃんと設定されているかも確認する？
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// TryParseメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParse()
        {
            IElement element;
            MediaWikiHeading heading;
            MediaWikiHeadingParser parser = new MediaWikiHeadingParser(
                new MediaWikiParser(new MockFactory().GetMediaWiki("en")));

            // 基本形
            Assert.IsTrue(parser.TryParse("==test==", out element));
            Assert.IsInstanceOf(typeof(MediaWikiHeading), element);
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("==test==", heading.ToString());
            Assert.AreEqual(2, heading.Level);
            Assert.AreEqual(1, heading.Count);
            Assert.AreEqual("test", heading[0].ToString());

            // 後ろが改行
            Assert.IsTrue(parser.TryParse("== test == \r\ntest", out element));
            Assert.IsInstanceOf(typeof(MediaWikiHeading), element);
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("== test == ", heading.ToString());
            Assert.AreEqual(2, heading.Level);
            Assert.AreEqual(1, heading.Count);
            Assert.AreEqual(" test ", heading[0].ToString());

            // 改行以外はNG
            Assert.IsFalse(parser.TryParse("== test == test", out element));
            Assert.IsNull(element);

            // 複数の要素を含む
            Assert.IsTrue(parser.TryParse("===[[test]] and sample===", out element));
            Assert.IsInstanceOf(typeof(MediaWikiHeading), element);
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("===[[test]] and sample===", heading.ToString());
            Assert.AreEqual(3, heading.Level);
            Assert.AreEqual(2, heading.Count);
            Assert.AreEqual("[[test]]", heading[0].ToString());
            Assert.AreEqual(" and sample", heading[1].ToString());

            // こんな無茶なコメントも一応対応
            Assert.IsTrue(parser.TryParse("<!--test-->=<!--test-->=関連項目<!--test-->==<!--test-->\n", out element));
            Assert.IsInstanceOf(typeof(MediaWikiHeading), element);
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("<!--test-->=<!--test-->=関連項目<!--test-->==<!--test-->", heading.ToString());
            Assert.AreEqual(2, heading.Level);
            // TODO: 本当は2でコメントも見つけるべきだが、コメントは中も除外しているので現状1
            // Assert.AreEqual(2, heading.Count);
            // Assert.AreEqual("関連項目", heading[0].ToString());
            // Assert.AreEqual("<!--test-->", heading[1].ToString());
            Assert.AreEqual(1, heading.Count);
            Assert.AreEqual("関連項目", heading[0].ToString());

            // 前後で数が違うのはOK、少ない側の階層と判定
            Assert.IsTrue(parser.TryParse("=test==", out element));
            Assert.IsInstanceOf(typeof(MediaWikiHeading), element);
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("=test==", heading.ToString());
            Assert.AreEqual(1, heading.Level);

            // 前後で数が違うの逆パターン
            Assert.IsTrue(parser.TryParse("====test==", out element));
            Assert.IsInstanceOf(typeof(MediaWikiHeading), element);
            heading = (MediaWikiHeading)element;
            Assert.AreEqual("====test==", heading.ToString());
            Assert.AreEqual(2, heading.Level);

            // 先頭が < = 以外はNG
            Assert.IsFalse(parser.TryParse(" ==test==", out element));
            Assert.IsNull(element);
        }

        #endregion
    }
}