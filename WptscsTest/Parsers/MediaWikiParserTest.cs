// ================================================================================================
// <summary>
//      MediaWikiParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiParserTest.cs" company="honeplusのメモ帳">
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
    /// MediaWikiParserのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiParserTest
    {
        #region 公開静的メソッドテストケース

        /// <summary>
        /// TryParseMediaWikiLinkメソッドテストケース（基本的な構文）。
        /// </summary>
        [Test]
        public void TestTryParseMediaWikiLinkBasic()
        {
            MediaWikiLink element;
            MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("en"));

            // タイトルのみ
            Assert.IsTrue(parser.TryParseMediaWikiLink("[[testtitle]]", out element));
            Assert.AreEqual("testtitle", element.Title);
            Assert.IsNull(element.Section);
            Assert.AreEqual(0, element.PipeTexts.Count);
            Assert.IsNull(element.Code);
            Assert.IsFalse(element.IsColon);

            // タイトルとセクション
            Assert.IsTrue(parser.TryParseMediaWikiLink("[[testtitle#testsection]]", out element));
            Assert.AreEqual("testtitle", element.Title);
            Assert.AreEqual("testsection", element.Section);
            Assert.AreEqual(0, element.PipeTexts.Count);
            Assert.IsNull(element.Code);
            Assert.IsFalse(element.IsColon);

            // タイトルとセクションとパイプ後の文字列
            Assert.IsTrue(parser.TryParseMediaWikiLink("[[testtitle#testsection|testpipe1|testpipe2]]", out element));
            Assert.AreEqual("testtitle", element.Title);
            Assert.AreEqual("testsection", element.Section);
            Assert.AreEqual(2, element.PipeTexts.Count);
            Assert.AreEqual("testpipe1", element.PipeTexts[0].ToString());
            Assert.AreEqual("testpipe2", element.PipeTexts[1].ToString());
            Assert.IsNull(element.Code);
            Assert.IsFalse(element.IsColon);

            // タイトルとセクションとパイプ後の文字列とコード
            Assert.IsTrue(parser.TryParseMediaWikiLink("[[en:testtitle#testsection|testpipe1|testpipe2]]", out element));
            Assert.AreEqual("testtitle", element.Title);
            Assert.AreEqual("testsection", element.Section);
            Assert.AreEqual(2, element.PipeTexts.Count);
            Assert.AreEqual("testpipe1", element.PipeTexts[0].ToString());
            Assert.AreEqual("testpipe2", element.PipeTexts[1].ToString());
            Assert.AreEqual("en", element.Code);
            Assert.IsFalse(element.IsColon);

            // タイトルとセクションとパイプ後の文字列とコードとコロン
            Assert.IsTrue(parser.TryParseMediaWikiLink("[[:en:testtitle#testsection|testpipe1|testpipe2]]", out element));
            Assert.AreEqual("testtitle", element.Title);
            Assert.AreEqual("testsection", element.Section);
            Assert.AreEqual(2, element.PipeTexts.Count);
            Assert.AreEqual("testpipe1", element.PipeTexts[0].ToString());
            Assert.AreEqual("testpipe2", element.PipeTexts[1].ToString());
            Assert.AreEqual("en", element.Code);
            Assert.IsTrue(element.IsColon);
        }

        /// <summary>
        /// TryParseメソッドテストケース（NGパターン）。
        /// </summary>
        [Test]
        public void TestTryParseNg()
        {
            MediaWikiLink element;
            MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("en"));

            // 開始タグが無い
            Assert.IsFalse(parser.TryParseMediaWikiLink("testtitle]]", out element));

            // 閉じタグが無い
            Assert.IsFalse(parser.TryParseMediaWikiLink("[[testtitle", out element));

            // 先頭が開始タグではない
            Assert.IsFalse(parser.TryParseMediaWikiLink(" [[testtitle]]", out element));

            // 外部リンクタグ
            Assert.IsFalse(parser.TryParseMediaWikiLink("[testtitle]", out element));

            // テンプレートタグ
            Assert.IsFalse(parser.TryParseMediaWikiLink("{{testtitle}}", out element));

            // TODO: 使用不可の文字が含まれるパターン等
        }

        /// <summary>
        /// TryParseメソッドテストケース（名前空間）。
        /// </summary>
        [Test]
        public void TestTryParseNamespace()
        {
            MediaWikiLink element;
            MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("ja"));

            // カテゴリ標準
            Assert.IsTrue(parser.TryParseMediaWikiLink("[[Category:test]]", out element));
            Assert.AreEqual("Category:test", element.Title);
            Assert.AreEqual(0, element.PipeTexts.Count);
            Assert.IsNull(element.Code);
            Assert.IsFalse(element.IsColon);

            // カテゴリソート名指定
            Assert.IsTrue(parser.TryParseMediaWikiLink("[[Category:test|てすと]]", out element));
            Assert.AreEqual("Category:test", element.Title);
            Assert.AreEqual(1, element.PipeTexts.Count);
            Assert.AreEqual("てすと", element.PipeTexts[0].ToString());
            Assert.IsNull(element.Code);
            Assert.IsFalse(element.IsColon);

            // カテゴリにならないような指定
            Assert.IsTrue(parser.TryParseMediaWikiLink("[[:Category:test]]", out element));
            Assert.AreEqual("Category:test", element.Title);
            Assert.AreEqual(0, element.PipeTexts.Count);
            Assert.IsNull(element.Code);
            Assert.IsTrue(element.IsColon);

            // ファイル
            // TODO: 将来的には入れ子を解析するようにする
            Assert.IsTrue(parser.TryParseMediaWikiLink("[[ファイル:test.png|thumb|100px|テスト[[画像]]]]", out element));
            Assert.AreEqual("ファイル:test.png", element.Title);
            Assert.AreEqual(3, element.PipeTexts.Count);
            Assert.AreEqual("thumb", element.PipeTexts[0].ToString());
            Assert.AreEqual("100px", element.PipeTexts[1].ToString());
            Assert.AreEqual("テスト[[画像]]", element.PipeTexts[2].ToString());
            Assert.IsNull(element.Code);
        }

        /// <summary>
        /// TryParseNowikiメソッドテストケース。
        /// </summary>
        [Test]
        public void TestTryParseNowiki()
        {
            XmlElement element;
            MediaWikiParser parser = new MediaWikiParser(new MockFactory().GetMediaWiki("en"));

            Assert.IsTrue(parser.TryParseNowiki("<nowiki>[[test]]</nowiki>", out element));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<NOWIKI>[[test]]</NOWIKI>", out element));
            Assert.AreEqual("<NOWIKI>[[test]]</NOWIKI>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<Nowiki>[[test]]</noWiki>", out element));
            Assert.AreEqual("<Nowiki>[[test]]</noWiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki>[[test]]</nowiki></nowiki>", out element));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki>[[test]]nowiki", out element));
            Assert.AreEqual("<nowiki>[[test]]nowiki", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki>\n\n[[test]]\r\n</nowiki>", out element));
            Assert.AreEqual("<nowiki>\n\n[[test]]\r\n</nowiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki><!--[[test]]--></nowiki>", out element));
            Assert.AreEqual("<nowiki><!--[[test]]--></nowiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki><!--<nowiki>[[test]]</nowiki>--></nowiki>", out element));
            Assert.AreEqual("<nowiki><!--<nowiki>[[test]]</nowiki>--></nowiki>", element.ToString());
            Assert.IsTrue(parser.TryParseNowiki("<nowiki><!--[[test]]", out element));
            Assert.AreEqual("<nowiki><!--[[test]]", element.ToString());
            Assert.IsFalse(parser.TryParseNowiki("<nowik>[[test]]</nowik>", out element));
            Assert.IsNull(element);
            Assert.IsFalse(parser.TryParseNowiki("<nowiki[[test]]</nowiki>", out element));
            Assert.IsNull(element);
        }

        #endregion
    }
}
