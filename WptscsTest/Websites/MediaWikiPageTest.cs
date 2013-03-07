// ================================================================================================
// <summary>
//      MediaWikiPageのテストクラスソース。</summary>
//
// <copyright file="MediaWikiPageTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2013 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using Honememo.Parsers;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Parsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="MediaWikiPage"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class MediaWikiPageTest
    {
        #region 定数

        /// <summary>
        /// example.xmlのページ本文。
        /// </summary>
        private static readonly string ExampleText
            = "[[File:Example.png|thumb|Wikipedia's example image. (Example.png)]]\n{{wiktionary}}\n{{wikiquote}}\n"
                + "'''Example''' may refer to:\n\n*[[Example (rapper)]], a British rapper\n*[[example.com]], "
                + "[[example.net]], [[example.org]]  and [[.example]], domain names reserved for use in documentation "
                + "as examples \n\n==See also==\n*[[Exemplum]], medieval collections of short stories to be told in "
                + "sermons\n*[[Exemplar]], a prototype or model which others can use to understand a topic better\n\n"
                + "{{disambig}}\n\n[[fr:Example]]\n[[ksh:Example (Watt ėßß datt?)]]";
        
        /// <summary>
        /// example.xmlのページ本文。
        /// </summary>
        private static readonly DateTime ExampleTimestamp = DateTime.Parse("2010-07-13T00:49:18Z");

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// <see cref="MediaWikiPage.Text"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestText()
        {
            // 何も値が設定されていない場合、記事名からデータを読み込みその本文を返す
            // 同時にタイムスタンプ, URIも設定される
            // ※ 異常系については、MediaWiki側の実装なのでそちらでテストする
            MediaWiki site = new MockFactory().GetMediaWiki("en");
            Uri uri = new Uri(new Uri(site.Location), StringUtils.FormatDollarVariable(site.ContentApi, "example"));
            MediaWikiPageMock page = new MediaWikiPageMock(site, "example");
            Assert.IsNull(page.Uri);

            Assert.AreEqual(MediaWikiPageTest.ExampleText, page.Text);
            Assert.AreEqual(MediaWikiPageTest.ExampleTimestamp, page.Timestamp);
            Assert.AreEqual(uri, page.Uri);

            // 一度読み込むと、次回以降はその値が設定されている
            page.Title = "new name";
            Assert.AreEqual(MediaWikiPageTest.ExampleText, page.Text);
            Assert.AreEqual(MediaWikiPageTest.ExampleTimestamp, page.Timestamp);

            // 値が設定されている状態では、設定された値が返る
            page = new MediaWikiPageMock(site, "example");
            page.Text = "test body";
            Assert.AreEqual("test body", page.Text);
            Assert.IsNull(page.Uri);
        }

        /// <summary>
        /// <see cref="MediaWikiPage.Timestamp"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestTimestamp()
        {
            // 何も値が設定されていない場合、記事名からデータを読み込みそのタイムスタンプを返す
            // 同時にページ本文, URIも設定される
            // ※ 異常系については、MediaWiki側の実装なのでそちらでテストする
            MediaWiki site = new MockFactory().GetMediaWiki("en");
            Uri uri = new Uri(new Uri(site.Location), StringUtils.FormatDollarVariable(site.ContentApi, "example"));
            MediaWikiPageMock page = new MediaWikiPageMock(site, "example");
            Assert.IsNull(page.Uri);

            Assert.AreEqual(MediaWikiPageTest.ExampleTimestamp, page.Timestamp);
            Assert.AreEqual(MediaWikiPageTest.ExampleText, page.Text);
            Assert.AreEqual(uri, page.Uri);

            // 一度読み込むと、次回以降はその値が設定されている
            page.Title = "new name";
            Assert.AreEqual(MediaWikiPageTest.ExampleTimestamp, page.Timestamp);
            Assert.AreEqual(MediaWikiPageTest.ExampleText, page.Text);

            // 値が設定されている状態では、設定された値が返る
            page = new MediaWikiPageMock(site, "example");
            DateTime now = DateTime.Now;
            page.Timestamp = now;
            Assert.AreEqual(now, page.Timestamp);
            Assert.IsNull(page.Uri);
        }

        #endregion

        #region 静的メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiPage.GetFromQuery"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestGetFromQuery()
        {
            // XMLを変えつつ、クエリーから想定通りのパラメータが読み込まれていることを確認
            // ※ ページ本文とタイムスタンプはnullだが遅延読み込みされるので、ここではチェックしない
            // ※ XMLはいろんなパターンがありえるが、パターンが増えすぎるので使う項目しかテストしていない。
            //    ここ以外の項目は、基本的に影響していない・・・はず。

            // 必要最小限のパターン
            MediaWiki website = new MockFactory().GetMediaWiki("en");
            XElement pe = new XElement("page", new XAttribute("title", "Test page"));
            XElement query = new XElement("query", new XElement("pages", pe));
            MediaWikiPage page = MediaWikiPage.GetFromQuery(website, null, query);
            Assert.AreSame(website, page.Website);
            Assert.IsNull(page.Uri);
            Assert.AreEqual("Test page", page.Title);
            Assert.IsNull(page.GetInterlanguage("ja"));
            Assert.IsNull(page.GetInterlanguage("de"));
            Assert.IsNull(page.Redirect);

            // URI
            Uri uri = new Uri("http://example.com/");
            page = MediaWikiPage.GetFromQuery(website, uri, query);
            Assert.AreSame(website, page.Website);
            Assert.AreSame(uri, page.Uri);
            Assert.AreEqual("Test page", page.Title);
            Assert.IsNull(page.GetInterlanguage("ja"));
            Assert.IsNull(page.GetInterlanguage("de"));
            Assert.IsNull(page.Redirect);

            // 言語間リンク枠だけ、上と変わらず
            XElement les = new XElement("langlinks");
            pe.Add(les);
            page = MediaWikiPage.GetFromQuery(website, uri, query);
            Assert.AreSame(website, page.Website);
            Assert.AreSame(uri, page.Uri);
            Assert.AreEqual("Test page", page.Title);
            Assert.IsNull(page.GetInterlanguage("ja"));
            Assert.IsNull(page.GetInterlanguage("de"));
            Assert.IsNull(page.Redirect);

            // 言語間リンク
            les.Add(new XElement("ll", new XAttribute("lang", "ja"), "テストページ"));
            page = MediaWikiPage.GetFromQuery(website, uri, query);
            Assert.AreSame(website, page.Website);
            Assert.AreSame(uri, page.Uri);
            Assert.AreEqual("Test page", page.Title);
            Assert.AreEqual("テストページ", page.GetInterlanguage("ja"));
            Assert.IsNull(page.GetInterlanguage("de"));
            Assert.IsNull(page.Redirect);

            // 言語間リンク複数も可
            les.Add(new XElement("ll", new XAttribute("lang", "de"), "Test de page"));
            page = MediaWikiPage.GetFromQuery(website, uri, query);
            Assert.AreSame(website, page.Website);
            Assert.AreSame(uri, page.Uri);
            Assert.AreEqual("Test page", page.Title);
            Assert.AreEqual("テストページ", page.GetInterlanguage("ja"));
            Assert.AreEqual("Test de page", page.GetInterlanguage("de"));
            Assert.IsNull(page.Redirect);

            // リダイレクト
            query.Add(new XElement("redirects", new XElement("r", new XAttribute("from", "from Redirect"))));
            page = MediaWikiPage.GetFromQuery(website, uri, query);
            Assert.AreSame(website, page.Website);
            Assert.AreSame(uri, page.Uri);
            Assert.AreEqual("Test page", page.Title);
            Assert.AreEqual("テストページ", page.GetInterlanguage("ja"));
            Assert.AreEqual("Test de page", page.GetInterlanguage("de"));
            Assert.AreEqual("from Redirect", page.Redirect);
        }

        /// <summary>
        /// <see cref="MediaWikiPage.GetFromQuery"/>メソッドテストケース（サイトが<c>null</c>）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetFromQueryAboutWebsiteIsNull()
        {
            MediaWikiPage.GetFromQuery(
                null,
                null,
                new XElement(
                    "query",
                    new XElement(
                        "pages",
                        new XElement("page", new XAttribute("title", "Test page")))));
        }

        /// <summary>
        /// <see cref="MediaWikiPage.GetFromQuery"/>メソッドテストケース（XML不正）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetFromQueryAboutPageElementIsNotFound()
        {
            MediaWikiPage.GetFromQuery(
                new MockFactory().GetMediaWiki("en"),
                null,
                new XElement("query", new XElement("pages")));
        }

        /// <summary>
        /// <see cref="MediaWikiPage.GetFromQuery"/>メソッドテストケース（ページなし）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestGetFromQueryAboutMissingPage()
        {
            MediaWikiPage.GetFromQuery(
                new MockFactory().GetMediaWiki("en"),
                null,
                new XElement(
                    "query",
                    new XElement(
                        "pages",
                        new XElement(
                            "page",
                            new XAttribute("title", "Test page"),
                            new XAttribute("missing", string.Empty)))));
        }

        #endregion

        #region 公開メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiPage.GetInterlanguage"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestGetInterlanguage()
        {
            MediaWikiPageMock page = new MediaWikiPageMock(
                new MockFactory().GetMediaWiki("en"),
                "TestTitle");
            page.Interlanguages.Add("ja", "テストページ");
            page.Interlanguages.Add("fr", "Test_Fr");
            Assert.AreEqual("テストページ", page.GetInterlanguage("ja"));
            Assert.AreEqual("Test_Fr", page.GetInterlanguage("fr"));
            Assert.IsNull(page.GetInterlanguage("de"));
            Assert.IsNull(page.GetInterlanguage("ru"));
            Assert.IsNull(page.GetInterlanguage("zh"));
        }

        /// <summary>
        /// <see cref="MediaWikiPage.IsRedirect"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestIsRedirect()
        {
            MediaWiki site = new MediaWiki(new Language("en"));
            MediaWikiPage page = new MediaWikiPage(site, "TestTitle");
            Assert.IsFalse(page.IsRedirect());
            Assert.IsNull(page.Redirect);

            page.Redirect = "Test Redirect";
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual("Test Redirect", page.Redirect);

            page.Redirect = string.Empty;
            Assert.IsTrue(page.IsRedirect());
            Assert.AreEqual(string.Empty, page.Redirect);

            page.Redirect = null;
            Assert.IsFalse(page.IsRedirect());
            Assert.IsNull(page.Redirect);
        }

        /// <summary>
        /// <see cref="MediaWikiPage.Normalize"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestNormalize()
        {
            MediaWiki site = new MockFactory().GetMediaWiki("en");
            MediaWikiPage page = new MediaWikiPage(site, "A/b/c");

            // サブページの正規化
            Assert.AreEqual("Normal page", page.Normalize(new MediaWikiLink("Normal page")));
            Assert.AreEqual("A/b/c/s", page.Normalize(new MediaWikiLink("/s")));
            Assert.AreEqual("A/b/c/s", page.Normalize(new MediaWikiLink("/s/")));
            Assert.AreEqual("A/b", page.Normalize(new MediaWikiLink("../")));
            Assert.AreEqual("A", page.Normalize(new MediaWikiLink("../../")));
            Assert.AreEqual("A/b/s", page.Normalize(new MediaWikiLink("../s")));
            Assert.AreEqual("A/b/s", page.Normalize(new MediaWikiLink("../s/")));
            Assert.AreEqual("A/s", page.Normalize(new MediaWikiLink("../../s")));
            Assert.AreEqual("A/s", page.Normalize(new MediaWikiLink("../../s/")));

            // テンプレートの正規化
            Assert.AreEqual("Template:Template page", page.Normalize(new MediaWikiTemplate("Template page")));
            Assert.AreEqual("Normal page", page.Normalize(new MediaWikiTemplate("Normal page") { IsColon = true }));
            Assert.AreEqual("Wikipedia:Help page", page.Normalize(new MediaWikiTemplate("Wikipedia:Help page")));
            Assert.AreEqual("template:Template page", page.Normalize(new MediaWikiTemplate("template:Template page")));
            Assert.AreEqual("A/b/c/Doc", page.Normalize(new MediaWikiTemplate("/Doc")));
            Assert.AreEqual("CURRENTYEAR", page.Normalize(new MediaWikiTemplate("CURRENTYEAR")));
        }

        #endregion

        #region モッククラス

        /// <summary>
        /// <see cref="MediaWikiPage"/>テスト用のモッククラスです。
        /// </summary>
        private class MediaWikiPageMock : MediaWikiPage
        {
            #region コンストラクタ

            /// <summary>
            /// コンストラクタ。
            /// ページの本文, タイムスタンプには<c>null</c>を設定。
            /// </summary>
            /// <param name="website">ページが所属するウェブサイト。</param>
            /// <param name="title">ページタイトル。</param>
            public MediaWikiPageMock(MediaWiki website, string title)
                : base(website, title)
            {
            }

            #endregion

            #region 非公開プロパティテスト用のオーラーライドプロパティ

            /// <summary>
            /// ページタイトル。
            /// </summary>
            public new string Title
            {
                get
                {
                    return base.Title;
                }

                set
                {
                    base.Title = value;
                }
            }

            /// <summary>
            /// ページの本文。
            /// </summary>
            public new string Text
            {
                get
                {
                    return base.Text;
                }

                set
                {
                    base.Text = value;
                }
            }

            /// <summary>
            /// ページのタイムスタンプ。
            /// </summary>
            public new DateTime? Timestamp
            {
                get
                {
                    return base.Timestamp;
                }

                set
                {
                    base.Timestamp = value;
                }
            }

            /// <summary>
            /// 言語間リンクの対応表。
            /// </summary>
            public new IDictionary<string, string> Interlanguages
            {
                get
                {
                    return base.Interlanguages;
                }
            }

            #endregion
        }

        #endregion
    }
}
