// ================================================================================================
// <summary>
//      MediaWikiLinkのテストクラスソース。</summary>
//
// <copyright file="MediaWikiLinkTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using System.Collections.Generic;
    using Honememo.Parsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="MediaWikiLink"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class MediaWikiLinkTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            MediaWikiLink element;

            element = new MediaWikiLink();
            Assert.IsNull(element.Title);
            Assert.IsNotNull(element.PipeTexts);
            Assert.AreEqual(0, element.PipeTexts.Count);

            element = new MediaWikiLink("記事名");
            Assert.AreEqual("記事名", element.Title);
            Assert.IsNotNull(element.PipeTexts);
            Assert.AreEqual(0, element.PipeTexts.Count);
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Titleプロパティテストケース。
        /// </summary>
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
        public void TestPipeTexts()
        {
            MediaWikiLink element = new MediaWikiLink();

            Assert.AreEqual(0, element.PipeTexts.Count);
            IList<IElement> list = new List<IElement>();
            list.Add(new TextElement("test"));
            element.PipeTexts = list;
            Assert.AreEqual(1, element.PipeTexts.Count);
        }

        /// <summary>
        /// Interwikiプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestInterwiki()
        {
            MediaWikiLink element = new MediaWikiLink();

            Assert.IsNull(element.Interwiki);
            element.Interwiki = "test";
            Assert.AreEqual("test", element.Interwiki);
        }

        /// <summary>
        /// IsColonプロパティテストケース。
        /// </summary>
        [TestMethod]
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

        #region 公開メソッドテストケース

        /// <summary>
        /// IsSubpageメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestIsSubpage()
        {
            MediaWikiLink element = new MediaWikiLink();

            // 通常の記事へのリンク
            element.Title = "testtitle";
            Assert.IsFalse(element.IsSubpage());

            // 先頭が / で始まるサブページへのリンク
            element.Title = "/testtitle";
            Assert.IsTrue(element.IsSubpage());
            element.Title = "/testtitle/";
            Assert.IsTrue(element.IsSubpage());

            // 先頭が ../ で始まるサブページへのリンク
            element.Title = "../";
            Assert.IsTrue(element.IsSubpage());
            element.Title = "../../";
            Assert.IsTrue(element.IsSubpage());
            element.Title = "../testtitle";
            Assert.IsTrue(element.IsSubpage());
            element.Title = "../../testtitle";
            Assert.IsTrue(element.IsSubpage());
        }

        /// <summary>
        /// GetLinkStringメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestGetLinkString()
        {
            MediaWikiLink element = new MediaWikiLink();

            // タイトルのみ
            element.Title = "testtitle";
            Assert.AreEqual("testtitle", element.GetLinkString());

            // タイトルとセクション
            element.Section = string.Empty;
            Assert.AreEqual("testtitle#", element.GetLinkString());
            element.Section = "testsection";
            Assert.AreEqual("testtitle#testsection", element.GetLinkString());

            // タイトルとセクションとパイプ後の文字列
            element.PipeTexts.Add(new TextElement("testpipe1"));
            element.PipeTexts.Add(new TextElement("testpipe2"));
            Assert.AreEqual("testtitle#testsection", element.GetLinkString());

            // タイトルとセクションとパイプ後の文字列とコード
            element.Interwiki = "en";
            Assert.AreEqual("en:testtitle#testsection", element.GetLinkString());

            // タイトルとセクションとパイプ後の文字列とコードとコロン
            element.IsColon = true;
            Assert.AreEqual(":en:testtitle#testsection", element.GetLinkString());

            // 実例）ファイルタグ
            element.Title = "ファイル:Kepler22b-artwork.jpg";
            element.Section = null;
            element.PipeTexts.Clear();
            element.PipeTexts.Add(new TextElement("thumb"));
            element.PipeTexts.Add(new TextElement("right"));
            element.PipeTexts.Add(new TextElement("[[ケプラー22b]]（想像図）"));
            element.Interwiki = null;
            element.IsColon = false;
            Assert.AreEqual("ファイル:Kepler22b-artwork.jpg", element.GetLinkString());
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            MediaWikiLink element = new MediaWikiLink();

            // タイトルのみ
            element.Title = "testtitle";
            Assert.AreEqual("[[testtitle]]", element.ToString());

            // タイトルとセクション
            element.Section = string.Empty;
            Assert.AreEqual("[[testtitle#]]", element.ToString());
            element.Section = "testsection";
            Assert.AreEqual("[[testtitle#testsection]]", element.ToString());

            // タイトルとセクションとパイプ後の文字列
            element.PipeTexts.Add(new TextElement("testpipe1"));
            element.PipeTexts.Add(new TextElement("testpipe2"));
            Assert.AreEqual("[[testtitle#testsection|testpipe1|testpipe2]]", element.ToString());

            // タイトルとセクションとパイプ後の文字列とコード
            element.Interwiki = "en";
            Assert.AreEqual("[[en:testtitle#testsection|testpipe1|testpipe2]]", element.ToString());

            // タイトルとセクションとパイプ後の文字列とコードとコロン
            element.IsColon = true;
            Assert.AreEqual("[[:en:testtitle#testsection|testpipe1|testpipe2]]", element.ToString());

            // 実例）ファイルタグ
            element.Title = "ファイル:Kepler22b-artwork.jpg";
            element.Section = null;
            element.PipeTexts.Clear();
            element.PipeTexts.Add(new TextElement("thumb"));
            element.PipeTexts.Add(new TextElement("right"));
            element.PipeTexts.Add(new TextElement("[[ケプラー22b]]（想像図）"));
            element.Interwiki = null;
            element.IsColon = false;
            Assert.AreEqual("[[ファイル:Kepler22b-artwork.jpg|thumb|right|[[ケプラー22b]]（想像図）]]", element.ToString());
        }

        #endregion
    }
}
