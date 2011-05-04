// ================================================================================================
// <summary>
//      MediaWikiLinkParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiLinkParserTest.cs" company="honeplusのメモ帳">
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
    /// MediaWikiLinkParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiLinkParserTest
    {
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// TryParseメソッドテストケース（基本的な構文）。
        /// </summary>
        [Test]
        public void TestTryParseBasic()
        {
            IElement element;
            MediaWikiLink link;
            MediaWikiLinkParser parser = new MediaWikiLinkParser(new MediaWikiParser(new MockFactory().GetMediaWiki("en")));

            // タイトルのみ
            Assert.IsTrue(parser.TryParse("[[testtitle]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle", link.Title);
            Assert.IsNull(link.Section);
            Assert.AreEqual(0, link.PipeTexts.Count);
            Assert.IsNull(link.Code);
            Assert.IsFalse(link.IsColon);

            // タイトルとセクション
            Assert.IsTrue(parser.TryParse("[[testtitle#testsection]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle", link.Title);
            Assert.AreEqual("testsection", link.Section);
            Assert.AreEqual(0, link.PipeTexts.Count);
            Assert.IsNull(link.Code);
            Assert.IsFalse(link.IsColon);

            // タイトルとセクションとパイプ後の文字列
            Assert.IsTrue(parser.TryParse("[[testtitle#testsection|testpipe1|testpipe2]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle", link.Title);
            Assert.AreEqual("testsection", link.Section);
            Assert.AreEqual(2, link.PipeTexts.Count);
            Assert.AreEqual("testpipe1", link.PipeTexts[0].ToString());
            Assert.AreEqual("testpipe2", link.PipeTexts[1].ToString());
            Assert.IsNull(link.Code);
            Assert.IsFalse(link.IsColon);

            // タイトルとセクションとパイプ後の文字列とコード
            Assert.IsTrue(parser.TryParse("[[en:testtitle#testsection|testpipe1|testpipe2]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle", link.Title);
            Assert.AreEqual("testsection", link.Section);
            Assert.AreEqual(2, link.PipeTexts.Count);
            Assert.AreEqual("testpipe1", link.PipeTexts[0].ToString());
            Assert.AreEqual("testpipe2", link.PipeTexts[1].ToString());
            Assert.AreEqual("en", link.Code);
            Assert.IsFalse(link.IsColon);

            // タイトルとセクションとパイプ後の文字列とコードとコロン
            Assert.IsTrue(parser.TryParse("[[:en:testtitle#testsection|testpipe1|testpipe2]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle", link.Title);
            Assert.AreEqual("testsection", link.Section);
            Assert.AreEqual(2, link.PipeTexts.Count);
            Assert.AreEqual("testpipe1", link.PipeTexts[0].ToString());
            Assert.AreEqual("testpipe2", link.PipeTexts[1].ToString());
            Assert.AreEqual("en", link.Code);
            Assert.IsTrue(link.IsColon);
        }

        /// <summary>
        /// TryParseメソッドテストケース（NGパターン）。
        /// </summary>
        [Test]
        public void TestTryParseNg()
        {
            IElement element;
            MediaWikiLinkParser parser = new MediaWikiLinkParser(new MediaWikiParser(new MockFactory().GetMediaWiki("en")));

            // 開始タグが無い
            Assert.IsFalse(parser.TryParse("testtitle]]", out element));

            // 閉じタグが無い
            Assert.IsFalse(parser.TryParse("[[testtitle", out element));

            // 先頭が開始タグではない
            Assert.IsFalse(parser.TryParse(" [[testtitle]]", out element));

            // 外部リンクタグ
            Assert.IsFalse(parser.TryParse("[testtitle]", out element));

            // テンプレートタグ
            Assert.IsFalse(parser.TryParse("{{testtitle}}", out element));

            // TODO: 使用不可の文字が含まれるパターン等
        }

        /// <summary>
        /// TryParseメソッドテストケース（名前空間）。
        /// </summary>
        [Test]
        public void TestTryParseNamespace()
        {
            IElement element;
            MediaWikiLink link;
            MediaWikiLinkParser parser = new MediaWikiLinkParser(new MediaWikiParser(new MockFactory().GetMediaWiki("ja")));

            // カテゴリ標準
            Assert.IsTrue(parser.TryParse("[[Category:test]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("Category:test", link.Title);
            Assert.AreEqual(0, link.PipeTexts.Count);
            Assert.IsNull(link.Code);
            Assert.IsFalse(link.IsColon);

            // カテゴリソート名指定
            Assert.IsTrue(parser.TryParse("[[Category:test|てすと]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("Category:test", link.Title);
            Assert.AreEqual(1, link.PipeTexts.Count);
            Assert.AreEqual("てすと", link.PipeTexts[0].ToString());
            Assert.IsNull(link.Code);
            Assert.IsFalse(link.IsColon);

            // カテゴリにならないような指定
            Assert.IsTrue(parser.TryParse("[[:Category:test]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("Category:test", link.Title);
            Assert.AreEqual(0, link.PipeTexts.Count);
            Assert.IsNull(link.Code);
            Assert.IsTrue(link.IsColon);

            // ファイル、入れ子もあり
            Assert.IsTrue(parser.TryParse("[[ファイル:test.png|thumb|100px|テスト[[画像]]]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("ファイル:test.png", link.Title);
            Assert.AreEqual(3, link.PipeTexts.Count);
            Assert.AreEqual("thumb", link.PipeTexts[0].ToString());
            Assert.AreEqual("100px", link.PipeTexts[1].ToString());
            Assert.AreEqual("テスト[[画像]]", link.PipeTexts[2].ToString());
            Assert.IsInstanceOf(typeof(ListElement), link.PipeTexts[2]);
            ListElement list = ((ListElement)link.PipeTexts[2]);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("テスト", list[0].ToString());
            Assert.AreEqual("[[画像]]", list[1].ToString());
            Assert.IsNull(link.Code);
        }

        #endregion
    }
}
