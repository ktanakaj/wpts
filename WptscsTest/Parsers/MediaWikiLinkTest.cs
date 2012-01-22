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
    using NUnit.Framework;
    using Honememo.Parsers;

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
            IList<IElement> list = new List<IElement>();
            list.Add(new TextElement("test"));
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
            element.Section = String.Empty;
            Assert.AreEqual("[[testtitle#]]", element.ToString());
            element.Section = "testsection";
            Assert.AreEqual("[[testtitle#testsection]]", element.ToString());

            // タイトルとセクションとパイプ後の文字列
            element.PipeTexts.Add(new TextElement("testpipe1"));
            element.PipeTexts.Add(new TextElement("testpipe2"));
            Assert.AreEqual("[[testtitle#testsection|testpipe1|testpipe2]]", element.ToString());

            // タイトルとセクションとパイプ後の文字列とコード
            element.Code = "en";
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
            element.Code = null;
            element.IsColon = false;
            Assert.AreEqual("[[ファイル:Kepler22b-artwork.jpg|thumb|right|[[ケプラー22b]]（想像図）]]", element.ToString());
        }

        #endregion
    }
}
