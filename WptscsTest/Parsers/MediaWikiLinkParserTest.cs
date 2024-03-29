﻿// ================================================================================================
// <summary>
//      MediaWikiLinkParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiLinkParserTest.cs" company="honeplusのメモ帳">
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
    /// <see cref="MediaWikiLinkParser"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class MediaWikiLinkParserTest
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
            mediaWikiParsers["ja"] = new MediaWikiParser(new MockFactory().GetMediaWiki("ja"));
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

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiLinkParser.TryParse"/>メソッドテストケース（基本的な構文）。
        /// </summary>
        [TestMethod]
        public void TestTryParseBasic()
        {
            IElement element;
            MediaWikiLink link;
            MediaWikiLinkParser parser = new MediaWikiLinkParser(mediaWikiParsers["en"]);

            // タイトルのみ
            Assert.IsTrue(parser.TryParse("[[testtitle]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle", link.Title);
            Assert.IsNull(link.Section);
            Assert.AreEqual(0, link.PipeTexts.Count);
            Assert.IsNull(link.Interwiki);
            Assert.IsFalse(link.IsColon);

            // タイトルとセクション
            Assert.IsTrue(parser.TryParse("[[testtitle#testsection]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle", link.Title);
            Assert.AreEqual("testsection", link.Section);
            Assert.AreEqual(0, link.PipeTexts.Count);
            Assert.IsNull(link.Interwiki);
            Assert.IsFalse(link.IsColon);

            // タイトルとセクションとパイプ後の文字列
            Assert.IsTrue(parser.TryParse("[[testtitle#testsection|testpipe1|testpipe2]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle", link.Title);
            Assert.AreEqual("testsection", link.Section);
            Assert.AreEqual(2, link.PipeTexts.Count);
            Assert.AreEqual("testpipe1", link.PipeTexts[0].ToString());
            Assert.AreEqual("testpipe2", link.PipeTexts[1].ToString());
            Assert.IsNull(link.Interwiki);
            Assert.IsFalse(link.IsColon);

            // タイトルとセクションとパイプ後の文字列とコード
            Assert.IsTrue(parser.TryParse("[[en:testtitle#testsection|testpipe1|testpipe2]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle", link.Title);
            Assert.AreEqual("testsection", link.Section);
            Assert.AreEqual(2, link.PipeTexts.Count);
            Assert.AreEqual("testpipe1", link.PipeTexts[0].ToString());
            Assert.AreEqual("testpipe2", link.PipeTexts[1].ToString());
            Assert.AreEqual("en", link.Interwiki);
            Assert.IsFalse(link.IsColon);

            // コメントはどこにあってもOK
            // TODO: [<!--test-->[タイトル]] みたいなのもMediaWiki上では認識されるが、2012年1月現在未対応
            Assert.IsTrue(parser.TryParse("[[testtitle<!--仮-->|testpipe1<!--コメントアウト-->]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle<!--仮-->", link.Title);
            Assert.AreEqual(1, link.PipeTexts.Count);
            Assert.AreEqual("testpipe1<!--コメントアウト-->", link.PipeTexts[0].ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiLinkParser.TryParse"/>メソッドテストケース（NGパターン）。
        /// </summary>
        [TestMethod]
        public void TestTryParseNg()
        {
            IElement element;
            MediaWikiLinkParser parser = new MediaWikiLinkParser(mediaWikiParsers["en"]);

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

            // 記事名の部分に < > [ ] { } \n のいずれかの文字が存在する場合、NG
            // ※ コメントや変数であれば可能、それ以外で存在するのがNG
            Assert.IsFalse(parser.TryParse("[[test<title]]", out element));
            Assert.IsFalse(parser.TryParse("[[test>title]]", out element));
            Assert.IsFalse(parser.TryParse("[[test[title]]", out element));
            Assert.IsFalse(parser.TryParse("[[test]title]]", out element));
            Assert.IsFalse(parser.TryParse("[[test{title]]", out element));
            Assert.IsFalse(parser.TryParse("[[test}title]]", out element));
            Assert.IsFalse(parser.TryParse("[[testtitle\n]]", out element));

            // 空・null
            Assert.IsFalse(parser.TryParse(string.Empty, out element));
            Assert.IsFalse(parser.TryParse(null, out element));
        }

        /// <summary>
        /// <see cref="MediaWikiLinkParser.TryParse"/>メソッドテストケース（入れ子）。
        /// </summary>
        [TestMethod]
        public void TestTryParseNested()
        {
            IElement element;
            MediaWikiLink link;
            MediaWikiLinkParser parser = new MediaWikiLinkParser(mediaWikiParsers["en"]);

            // テンプレートはパイプ以後にある分には全てOK
            Assert.IsTrue(parser.TryParse("[[ロシア語|{{lang|ru|русский язык}}]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("ロシア語", link.Title);
            Assert.AreEqual(1, link.PipeTexts.Count);
            Assert.AreEqual("{{lang|ru|русский язык}}", link.PipeTexts[0].ToString());

            // 変数の場合、記事名の部分にも入れられる
            Assert.IsTrue(parser.TryParse("[[{{{title|デフォルトジャンル}}}-stub]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("{{{title|デフォルトジャンル}}}-stub", link.Title);
        }

        /// <summary>
        /// <see cref="MediaWikiLinkParser.TryParse"/>メソッドテストケース（名前空間）。
        /// </summary>
        [TestMethod]
        public void TestTryParseNamespace()
        {
            IElement element;
            MediaWikiLink link;
            MediaWikiLinkParser parser = new MediaWikiLinkParser(mediaWikiParsers["ja"]);

            // カテゴリ標準
            Assert.IsTrue(parser.TryParse("[[Category:test]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("Category:test", link.Title);
            Assert.AreEqual(0, link.PipeTexts.Count);
            Assert.IsNull(link.Interwiki);
            Assert.IsFalse(link.IsColon);

            // カテゴリソート名指定
            Assert.IsTrue(parser.TryParse("[[Category:test|てすと]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("Category:test", link.Title);
            Assert.AreEqual(1, link.PipeTexts.Count);
            Assert.AreEqual("てすと", link.PipeTexts[0].ToString());
            Assert.IsNull(link.Interwiki);
            Assert.IsFalse(link.IsColon);

            // カテゴリにならないような指定
            Assert.IsTrue(parser.TryParse("[[:Category:test]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("Category:test", link.Title);
            Assert.AreEqual(0, link.PipeTexts.Count);
            Assert.IsNull(link.Interwiki);
            Assert.IsTrue(link.IsColon);

            // ファイル、入れ子もあり
            Assert.IsTrue(parser.TryParse("[[ファイル:test.png|thumb|100px|テスト[[画像]]]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("ファイル:test.png", link.Title);
            Assert.AreEqual(3, link.PipeTexts.Count);
            Assert.AreEqual("thumb", link.PipeTexts[0].ToString());
            Assert.AreEqual("100px", link.PipeTexts[1].ToString());
            Assert.AreEqual("テスト[[画像]]", link.PipeTexts[2].ToString());
            Assert.IsInstanceOfType(link.PipeTexts[2], typeof(ListElement));
            ListElement list = (ListElement)link.PipeTexts[2];
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("テスト", list[0].ToString());
            Assert.AreEqual("[[画像]]", list[1].ToString());
            Assert.IsNull(link.Interwiki);
        }

        /// <summary>
        /// <see cref="MediaWikiLinkParser.TryParse"/>メソッドテストケース（サブページ）。
        /// </summary>
        [TestMethod]
        public void TestTryParseSubpage()
        {
            IElement element;
            MediaWikiLink link;
            MediaWikiLinkParser parser = new MediaWikiLinkParser(mediaWikiParsers["en"]);

            // 全て指定されているケースは通常の記事と同じ扱い
            Assert.IsTrue(parser.TryParse("[[testtitle/subpage]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle/subpage", link.Title);
            Assert.IsFalse(link.IsSubpage());

            // 記事名が省略されているケース
            Assert.IsTrue(parser.TryParse("[[/subpage]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("/subpage", link.Title);
            Assert.IsTrue(link.IsSubpage());

            // 記事名が省略されているケース2
            Assert.IsTrue(parser.TryParse("[[../../subpage]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("../../subpage", link.Title);
            Assert.IsTrue(link.IsSubpage());
        }

        /// <summary>
        /// <see cref="MediaWikiLinkParser.TryParse"/>メソッドテストケース（ウィキ間リンク）。
        /// </summary>
        [TestMethod]
        public void TestTryParseInterwiki()
        {
            IElement element;
            MediaWikiLink link;
            MediaWikiLinkParser parser = new MediaWikiLinkParser(mediaWikiParsers["en"]);

            // タイトルとセクションとパイプ後の文字列とウィキ間リンクとコロンの全部入り
            Assert.IsTrue(parser.TryParse("[[:en:testtitle#testsection|testpipe1|testpipe2]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("testtitle", link.Title);
            Assert.AreEqual("testsection", link.Section);
            Assert.AreEqual(2, link.PipeTexts.Count);
            Assert.AreEqual("testpipe1", link.PipeTexts[0].ToString());
            Assert.AreEqual("testpipe2", link.PipeTexts[1].ToString());
            Assert.AreEqual("en", link.Interwiki);
            Assert.IsTrue(link.IsColon);

            // 普通の言語間リンク
            Assert.IsTrue(parser.TryParse("[[ja:日本語版記事名]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("日本語版記事名", link.Title);
            Assert.AreEqual("ja", link.Interwiki);

            // ウィキ間リンク
            Assert.IsTrue(parser.TryParse("[[commons:コモンズ記事名]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("コモンズ記事名", link.Title);
            Assert.AreEqual("commons", link.Interwiki);

            // ウィキ間リンクの構文だが名前空間と被るためそうならないもの
            Assert.IsTrue(parser.TryParse("[[wikipedia:ウィキペディア記事名]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("wikipedia:ウィキペディア記事名", link.Title);
            Assert.IsNull(link.Interwiki);

            // :が含まれるだけの普通の記事（1.10以前のバージョンでは誤判定されていた）
            Assert.IsTrue(parser.TryParse("[[Marathon 2: Durandal]]", out element));
            link = (MediaWikiLink)element;
            Assert.AreEqual("Marathon 2: Durandal", link.Title);
            Assert.IsNull(link.Interwiki);
        }

        #endregion
    }
}
