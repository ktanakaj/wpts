// ================================================================================================
// <summary>
//      MediaWikiLinkのテストクラスソース。</summary>
//
// <copyright file="MediaWikiLinkTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// MediaWikiLinkのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiLinkTest
    {
        #region プロパティテストケース

        /// <summary>
        /// Titleプロパティテストケース。
        /// </summary>
        [Test]
        public void TestTitle()
        {
            MediaWikiLink element = new MediaWikiLink();

            Assert.IsNull(element.Title);
            element.Title = "test";
            Assert.AreEqual("test", element.Title);
        }

        /// <summary>
        /// Sectionプロパティテストケース。
        /// </summary>
        [Test]
        public void TestSection()
        {
            MediaWikiLink element = new MediaWikiLink();

            Assert.IsNull(element.Section);
            element.Section = "test";
            Assert.AreEqual("test", element.Section);
        }

        /// <summary>
        /// PipeTextsプロパティテストケース。
        /// </summary>
        [Test]
        public void TestPipeTexts()
        {
            MediaWikiLink element = new MediaWikiLink();

            Assert.AreEqual(0, element.PipeTexts.Count);
            IList<string> list = new List<string>();
            list.Add("test");
            element.PipeTexts = list;
            Assert.AreEqual(1, element.PipeTexts.Count);
        }

        /// <summary>
        /// Codeプロパティテストケース。
        /// </summary>
        [Test]
        public void TestCode()
        {
            MediaWikiLink element = new MediaWikiLink();

            Assert.IsNull(element.Code);
            element.Code = "test";
            Assert.AreEqual("test", element.Code);
        }

        /// <summary>
        /// IsColonプロパティテストケース。
        /// </summary>
        [Test]
        public void TestIsColon()
        {
            MediaWikiLink element = new MediaWikiLink();

            Assert.IsFalse(element.IsColon);
            element.IsColon = true;
            Assert.IsTrue(element.IsColon);
            element.IsColon = false;
            Assert.IsFalse(element.IsColon);
        }

        #endregion
        
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            MediaWikiLink element = new MediaWikiLink();

            // タイトルのみ
            element.Title = "testtitle";
            Assert.AreEqual("[[testtitle]]", element.ToString());

            // タイトルとセクション
            element.Section = "testsection";
            Assert.AreEqual("[[testtitle#testsection]]", element.ToString());

            // タイトルとセクションとパイプ後の文字列
            element.PipeTexts.Add("testpipe1");
            element.PipeTexts.Add("testpipe2");
            Assert.AreEqual("[[testtitle#testsection|testpipe1|testpipe2]]", element.ToString());

            // タイトルとセクションとパイプ後の文字列とコード
            element.Code = "en";
            Assert.AreEqual("[[en:testtitle#testsection|testpipe1|testpipe2]]", element.ToString());

            // タイトルとセクションとパイプ後の文字列とコードとコロン
            element.IsColon = true;
            Assert.AreEqual("[[:en:testtitle#testsection|testpipe1|testpipe2]]", element.ToString());

            //TODO: もうちょっと組み合わせがあったほうがよい
        }

        #endregion

        #region 静的メソッドテストケース

        /// <summary>
        /// TryParseメソッドテストケース（基本的な構文）。
        /// </summary>
        [Test]
        public void TestTryParseBasic()
        {
            MediaWikiLink element;

            // タイトルのみ
            Assert.IsTrue(MediaWikiLink.TryParse("[[testtitle]]", out element));
            Assert.AreEqual("testtitle", element.Title);
            Assert.IsNull(element.Section);
            Assert.AreEqual(0, element.PipeTexts.Count);
            Assert.IsNull(element.Code);
            Assert.IsFalse(element.IsColon);

            // タイトルとセクション
            Assert.IsTrue(MediaWikiLink.TryParse("[[testtitle#testsection]]", out element));
            Assert.AreEqual("testtitle", element.Title);
            Assert.AreEqual("testsection", element.Section);
            Assert.AreEqual(0, element.PipeTexts.Count);
            Assert.IsNull(element.Code);
            Assert.IsFalse(element.IsColon);

            // タイトルとセクションとパイプ後の文字列
            Assert.IsTrue(MediaWikiLink.TryParse("[[testtitle#testsection|testpipe1|testpipe2]]", out element));
            Assert.AreEqual("testtitle", element.Title);
            Assert.AreEqual("testsection", element.Section);
            Assert.AreEqual(2, element.PipeTexts.Count);
            Assert.AreEqual("testpipe1", element.PipeTexts[0]);
            Assert.AreEqual("testpipe2", element.PipeTexts[1]);
            Assert.IsNull(element.Code);
            Assert.IsFalse(element.IsColon);

            // タイトルとセクションとパイプ後の文字列とコード
            Assert.IsTrue(MediaWikiLink.TryParse("[[en:testtitle#testsection|testpipe1|testpipe2]]", out element));
            Assert.AreEqual("testtitle", element.Title);
            Assert.AreEqual("testsection", element.Section);
            Assert.AreEqual(2, element.PipeTexts.Count);
            Assert.AreEqual("testpipe1", element.PipeTexts[0]);
            Assert.AreEqual("testpipe2", element.PipeTexts[1]);
            Assert.AreEqual("en", element.Code);
            Assert.IsFalse(element.IsColon);

            // タイトルとセクションとパイプ後の文字列とコードとコロン
            Assert.IsTrue(MediaWikiLink.TryParse("[[:en:testtitle#testsection|testpipe1|testpipe2]]", out element));
            Assert.AreEqual("testtitle", element.Title);
            Assert.AreEqual("testsection", element.Section);
            Assert.AreEqual(2, element.PipeTexts.Count);
            Assert.AreEqual("testpipe1", element.PipeTexts[0]);
            Assert.AreEqual("testpipe2", element.PipeTexts[1]);
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

            // 開始タグが無い
            Assert.IsFalse(MediaWikiLink.TryParse("testtitle]]", out element));

            // 閉じタグが無い
            Assert.IsFalse(MediaWikiLink.TryParse("[[testtitle", out element));

            // 先頭が開始タグではない
            Assert.IsFalse(MediaWikiLink.TryParse(" [[testtitle]]", out element));

            // 外部リンクタグ
            Assert.IsFalse(MediaWikiLink.TryParse("[testtitle]", out element));

            // テンプレートタグ
            Assert.IsFalse(MediaWikiLink.TryParse("{{testtitle}}", out element));

            // TODO: 使用不可の文字が含まれるパターン等
        }

        /// <summary>
        /// TryParseメソッドテストケース（名前空間）。
        /// </summary>
        [Test]
        public void TestTryParseNamespace()
        {
            MediaWikiLink element;

            // カテゴリ標準
            Assert.IsTrue(MediaWikiLink.TryParse("[[Category:test]]", out element));
            Assert.AreEqual("Category:test", element.Title);
            Assert.AreEqual(0, element.PipeTexts.Count);
            Assert.IsNull(element.Code);
            Assert.IsFalse(element.IsColon);

            // カテゴリソート名指定
            Assert.IsTrue(MediaWikiLink.TryParse("[[Category:test|てすと]]", out element));
            Assert.AreEqual("Category:test", element.Title);
            Assert.AreEqual(1, element.PipeTexts.Count);
            Assert.AreEqual("てすと", element.PipeTexts[0]);
            Assert.IsNull(element.Code);
            Assert.IsFalse(element.IsColon);

            // カテゴリにならないような指定
            Assert.IsTrue(MediaWikiLink.TryParse("[[:Category:test]]", out element));
            Assert.AreEqual("Category:test", element.Title);
            Assert.AreEqual(0, element.PipeTexts.Count);
            Assert.IsNull(element.Code);
            Assert.IsTrue(element.IsColon);

            // ファイル
            Assert.IsTrue(MediaWikiLink.TryParse("[[ファイル:test.png|thumb|100px|テスト[[画像]]]]", out element));
            Assert.AreEqual("ファイル:test.png", element.Title);
            Assert.AreEqual(3, element.PipeTexts.Count);
            Assert.AreEqual("thumb", element.PipeTexts[0]);
            Assert.AreEqual("100px", element.PipeTexts[1]);
            Assert.AreEqual("テスト[[画像]]", element.PipeTexts[2]);
            Assert.IsNull(element.Code);
        }

        #endregion
    }
}
