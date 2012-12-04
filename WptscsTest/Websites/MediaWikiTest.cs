﻿// ================================================================================================
// <summary>
//      MediaWikiのテストクラスソース。</summary>
//
// <copyright file="MediaWikiTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Honememo.Models;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="MediaWiki"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class MediaWikiTest
    {
        #region 定数

        /// <summary>
        /// XMLインポート／エクスポートで用いるテストデータ。
        /// </summary>
        private static readonly string TestXml = "<MediaWiki><Location>http://ja.wikipedia.org</Location>"
            + "<Language Code=\"ja\"><Names /><Bracket /></Language>"
            + "<MetaApi>_api.xml</MetaApi><ExportPath>/export/$1</ExportPath><Redirect>#飛ばす</Redirect>"
            + "<TemplateNamespace>100</TemplateNamespace><CategoryNamespace>101</CategoryNamespace><FileNamespace>200</FileNamespace>"
            + "<MagicWords><Variable>特別</Variable><Variable>マジックワード</Variable></MagicWords>"
            + "<InterwikiPrefixs><Prefix>外部ウィキ</Prefix><Prefix>ニュース</Prefix></InterwikiPrefixs>"
            + "<DocumentationTemplates><DocumentationTemplate DefaultPage=\"/サブページ\">Template:ドキュメント</DocumentationTemplate>"
            + "<DocumentationTemplate DefaultPage=\"/サブページ\">テンプレート:Doc</DocumentationTemplate></DocumentationTemplates>"
            + "<LinkInterwikiFormat>{{仮リンク|$1|$2|$3|label=$4}}</LinkInterwikiFormat>"
            + "<LangFormat>{{Lang|$1|$2}}</LangFormat>"
            + "<HasLanguagePage>True</HasLanguagePage></MediaWiki>";

        #endregion

        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタ（MediaWiki全般）テストケース。
        /// </summary>
        [TestMethod]
        public void TestConstructorLanguageLocation()
        {
            MediaWiki site = new MediaWiki(new Language("en"), "test");
            Assert.AreEqual("en", site.Language.Code);
            Assert.AreEqual("test", site.Location);
        }

        /// <summary>
        /// コンストラクタ（MediaWiki全般）テストケース（languageがnull）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorLanguageLocationByLanguageNull()
        {
            MediaWiki site = new MediaWiki(null, "test");
        }

        /// <summary>
        /// コンストラクタ（MediaWiki全般）テストケース（locationがnull）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorLanguageLocationByLocationNull()
        {
            MediaWiki site = new MediaWiki(new Language("en"), null);
        }

        /// <summary>
        /// コンストラクタ（Wikipedia用）テストケース。
        /// </summary>
        [TestMethod]
        public void TestConstructorLanguage()
        {
            MediaWiki site = new MediaWiki(new Language("en"));
            Assert.AreEqual("en", site.Language.Code);
            Assert.AreEqual("http://en.wikipedia.org", site.Location);
        }

        /// <summary>
        /// コンストラクタ（Wikipedia用）テストケース（languageがnull）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorLanguageByLanguageNull()
        {
            MediaWiki site = new MediaWiki(null);
        }

        #endregion

        #region 設定ファイルに初期値を持つプロパティテストケース

        /// <summary>
        /// <see cref="MediaWiki.MetaApi"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestMetaApi()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトでは設定ファイルの値が返される
            Assert.AreEqual("/w/api.php?format=xml&action=query&meta=siteinfo&siprop=namespaces|namespacealiases|interwikimap", site.MetaApi);

            // 値を設定するとその値が返る
            site.MetaApi = "test";
            Assert.AreEqual("test", site.MetaApi);

            // 空またはnullの場合、再び設定ファイルの値が入る
            site.MetaApi = null;
            Assert.AreEqual("/w/api.php?format=xml&action=query&meta=siteinfo&siprop=namespaces|namespacealiases|interwikimap", site.MetaApi);
            site.MetaApi = string.Empty;
            Assert.AreEqual("/w/api.php?format=xml&action=query&meta=siteinfo&siprop=namespaces|namespacealiases|interwikimap", site.MetaApi);
        }

        /// <summary>
        /// <see cref="MediaWiki.ExportPath"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestExportPath()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトでは設定ファイルの値が返される
            Assert.AreEqual("/wiki/Special:Export/$1", site.ExportPath);

            // 値を設定するとその値が返る
            site.ExportPath = "test";
            Assert.AreEqual("test", site.ExportPath);

            // 空またはnullの場合、再び設定ファイルの値が入る
            site.ExportPath = null;
            Assert.AreEqual("/wiki/Special:Export/$1", site.ExportPath);
            site.ExportPath = string.Empty;
            Assert.AreEqual("/wiki/Special:Export/$1", site.ExportPath);
        }

        /// <summary>
        /// <see cref="MediaWiki.Redirect"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestRedirect()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトでは設定ファイルの値が返される
            Assert.AreEqual("#REDIRECT", site.Redirect);

            // 値を設定するとその値が返る
            site.Redirect = "test";
            Assert.AreEqual("test", site.Redirect);

            // 空またはnullの場合、再び設定ファイルの値が入る
            site.Redirect = null;
            Assert.AreEqual("#REDIRECT", site.Redirect);
            site.Redirect = string.Empty;
            Assert.AreEqual("#REDIRECT", site.Redirect);
        }

        /// <summary>
        /// <see cref="MediaWiki.TemplateNamespace"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestTemplateNamespace()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトでは設定ファイルの値が返される
            Assert.AreEqual(10, site.TemplateNamespace);

            // 値を設定するとその値が返る
            site.TemplateNamespace = -1;
            Assert.AreEqual(-1, site.TemplateNamespace);
        }

        /// <summary>
        /// <see cref="MediaWiki.CategoryNamespace"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestCategoryNamespace()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトでは設定ファイルの値が返される
            Assert.AreEqual(14, site.CategoryNamespace);

            // 値を設定するとその値が返る
            site.CategoryNamespace = -1;
            Assert.AreEqual(-1, site.CategoryNamespace);
        }

        /// <summary>
        /// <see cref="MediaWiki.FileNamespace"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestFileNamespace()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトでは設定ファイルの値が返される
            Assert.AreEqual(6, site.FileNamespace);

            // 値を設定するとその値が返る
            site.FileNamespace = -1;
            Assert.AreEqual(-1, site.FileNamespace);
        }

        /// <summary>
        /// <see cref="MediaWiki.MagicWords"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestMagicWords()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトでは設定ファイルの値が返される
            Assert.IsTrue(site.MagicWords.Contains("SERVERNAME"));

            // 値を設定するとその値が返る
            site.MagicWords = new HashSet<string>();
            Assert.AreEqual(0, site.MagicWords.Count);

            // nullの場合、再び設定ファイルの値が入る
            site.MagicWords = null;
            Assert.IsTrue(site.MagicWords.Contains("SERVERNAME"));
        }

        #endregion

        #region サーバーから値を取得するプロパティテストケース

        /// <summary>
        /// <see cref="MediaWiki.Namespaces"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestNamespaces()
        {
            MediaWiki site = new MockFactory().GetMediaWiki("en");

            // サーバーからダウンロードした値が返される
            ISet<string> names = site.Namespaces[6];
            Assert.IsNotNull(site.Namespaces);
            Assert.IsTrue(site.Namespaces.Count > 0);
            Assert.IsTrue(names.Contains("File"));
            Assert.IsTrue(names.Contains("Image"));
        }

        /// <summary>
        /// <see cref="MediaWiki.InterwikiPrefixs"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestInterwikiPrefixs()
        {
            MediaWiki site = new MockFactory().GetMediaWiki("en");

            // デフォルトではサーバーからダウンロードした値+設定ファイルの値が返される
            Assert.IsNotNull(site.InterwikiPrefixs);
            Assert.IsTrue(site.InterwikiPrefixs.Count > 0);
            Assert.IsTrue(site.InterwikiPrefixs.Contains("ja"));
            Assert.IsTrue(site.InterwikiPrefixs.Contains("w"));
            Assert.IsTrue(site.InterwikiPrefixs.Contains("wikipedia"));
            Assert.IsTrue(site.InterwikiPrefixs.Contains("commons"));

            // 値を設定すると設定ファイルの値の代わりにその値が返る
            IgnoreCaseSet prefixs = new IgnoreCaseSet();
            prefixs.Add("testtesttest");
            site.InterwikiPrefixs = prefixs;
            Assert.IsFalse(site.InterwikiPrefixs.Contains("wikipedia"));
            Assert.IsTrue(site.InterwikiPrefixs.Contains("testtesttest"));

            // 空の場合、再び設定ファイルの値が入る
            site.InterwikiPrefixs = null;
            Assert.AreNotSame(prefixs, site.InterwikiPrefixs);
            Assert.IsTrue(site.InterwikiPrefixs.Contains("wikipedia"));
            Assert.IsFalse(site.InterwikiPrefixs.Contains("testtesttest"));
        }

        #endregion

        #region それ以外のプロパティテストケース

        /// <summary>
        /// <see cref="MediaWiki.DocumentationTemplates"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestDocumentationTemplates()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトでは空
            Assert.IsNotNull(site.DocumentationTemplates);
            Assert.AreEqual(0, site.DocumentationTemplates.Count);

            // 値を設定するとそのオブジェクトが返る
            string[] templates = new string[] { "Template:Documentation" };
            site.DocumentationTemplates = templates;
            Assert.AreEqual(1, site.DocumentationTemplates.Count);
            Assert.AreEqual("Template:Documentation", site.DocumentationTemplates[0]);
            Assert.AreSame(templates, site.DocumentationTemplates);

            // nullを設定すると初期化
            site.DocumentationTemplates = null;
            Assert.IsNotNull(site.DocumentationTemplates);
            Assert.AreEqual(0, site.DocumentationTemplates.Count);
        }

        /// <summary>
        /// <see cref="MediaWiki.DocumentationTemplateDefaultPage"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestDocumentationTemplateDefaultPage()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトでは空
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(site.DocumentationTemplateDefaultPage));

            // 値を設定するとその値が返る
            site.DocumentationTemplateDefaultPage = "/doc";
            Assert.AreEqual("/doc", site.DocumentationTemplateDefaultPage);
            site.DocumentationTemplateDefaultPage = null;
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(site.DocumentationTemplateDefaultPage));
        }

        /// <summary>
        /// <see cref="MediaWiki.LinkInterwikiFormat"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestLinkInterwikiFormat()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトでは空
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(site.LinkInterwikiFormat));

            // 値を設定するとその値が返る
            site.LinkInterwikiFormat = "{{仮リンク|$1|$2|$3|label=$4}}";
            Assert.AreEqual("{{仮リンク|$1|$2|$3|label=$4}}", site.LinkInterwikiFormat);
            site.LinkInterwikiFormat = null;
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(site.LinkInterwikiFormat));
        }

        /// <summary>
        /// <see cref="MediaWiki.LangFormat"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestLangFormat()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトでは空
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(site.LangFormat));

            // 値を設定するとその値が返る
            site.LangFormat = "{{Lang|$1|$2}}";
            Assert.AreEqual("{{Lang|$1|$2}}", site.LangFormat);
            site.LangFormat = null;
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(site.LangFormat));
        }

        /// <summary>
        /// <see cref="MediaWiki.HasLanguagePage"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestHasLanguagePage()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));

            // デフォルトではfalse
            Assert.IsFalse(site.HasLanguagePage);

            // 値を設定するとその値が返る
            site.HasLanguagePage = true;
            Assert.IsTrue(site.HasLanguagePage);
            site.HasLanguagePage = false;
            Assert.IsFalse(site.HasLanguagePage);
        }

        #endregion

        #region 公開メソッドテストケース

        /// <summary>
        /// <see cref="MediaWiki.GetPage"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestGetPage()
        {
            MediaWiki site = new MockFactory().GetMediaWiki("en");
            Page page = site.GetPage("example");
            Assert.IsInstanceOfType(page, typeof(MediaWikiPage));
            Assert.AreEqual("Example", page.Title);
            Assert.AreEqual(DateTime.Parse("2010/07/13T00:49:18Z"), page.Timestamp);
            Assert.IsTrue(page.Text.Length > 0);
            Assert.AreEqual(site, page.Website);
        }

        /// <summary>
        /// <see cref="MediaWiki.GetPage"/>メソッドテストケース（末尾ピリオド）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(EndPeriodException))]
        public void TestGetPageEndPeriodException()
        {
            // ピリオドで終わるページは2012年現在処理できないため、
            // 暫定対応として例外を投げる
            // ※ httpでページ名が末尾に来るパスになるよう設定
            //    処理の都合上、このテストはサーバーに接続しています
            MediaWiki site = new MediaWiki(new Language("en"));
            site.GetPage("Vulcan Inc.");
        }

        /// <summary>
        /// <see cref="MediaWiki.GetPage"/>メソッドテストケース（末尾クエッションマーク）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(EndPeriodException))]
        public void TestGetPageEndPeriodExceptionAboutQuestion()
        {
            // ?で終わるページも2012年現在処理できないため、
            // 暫定対応として例外を投げる
            // ※ httpでページ名が末尾に来るパスになるよう設定
            //    処理の都合上、このテストはサーバーに接続しています
            MediaWiki site = new MediaWiki(new Language("en"));
            site.GetPage("How does one patch KDE2 under FreeBSD?");
        }

        /// <summary>
        /// <see cref="MediaWiki.IsMagicWord"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestIsMagicWord()
        {
            MediaWiki site = new MediaWiki(new Language("en"), "http://example.com");
            site.MagicWords = new HashSet<string>();

            // 値が設定されていなければ一致しない
            Assert.IsFalse(site.IsMagicWord("CURRENTYEAR"));
            Assert.IsFalse(site.IsMagicWord("ns:1"));

            // 値が一致、大文字小文字は区別する
            site.MagicWords.Add("CURRENTYEAR");
            Assert.IsTrue(site.IsMagicWord("CURRENTYEAR"));
            Assert.IsFalse(site.IsMagicWord("currentyear"));
            Assert.IsFalse(site.IsMagicWord("ns:1"));

            // コロンが入るものは、その前の部分までで判定
            site.MagicWords.Add("ns");
            Assert.IsTrue(site.IsMagicWord("CURRENTYEAR"));
            Assert.IsTrue(site.IsMagicWord("ns:1"));
        }

        /// <summary>
        /// <see cref="MediaWiki.IsInterwiki"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestIsInterwiki()
        {
            MediaWiki site = new MockFactory().GetMediaWiki("en");
            site.InterwikiPrefixs = new IgnoreCaseSet();

            // 値が存在しなければ一致しない
            Assert.IsFalse(site.IsInterwiki("commons:test"));
            Assert.IsFalse(site.IsInterwiki("commons:"));
            Assert.IsFalse(site.IsInterwiki("common:"));
            Assert.IsFalse(site.IsInterwiki("zzz:zzz語版記事"));
            Assert.IsFalse(site.IsInterwiki("ZZZ:zzz語版記事"));

            // 値が設定されていれば、前方一致で一致する、大文字小文字は区別しない
            site.InterwikiPrefixs.Add("commons");
            site.InterwikiPrefixs.Add("zzz");
            Assert.IsTrue(site.IsInterwiki("commons:test"));
            Assert.IsTrue(site.IsInterwiki("commons:"));
            Assert.IsFalse(site.IsInterwiki("common:"));
            Assert.IsTrue(site.IsInterwiki("zzz:zzz語版記事"));
            Assert.IsTrue(site.IsInterwiki("ZZZ:zzz語版記事"));

            // 名前空間名と被るときはそちらが優先、ウィキ間リンクとは判定されない
            site.InterwikiPrefixs.Add("File");
            Assert.IsFalse(site.IsInterwiki("File:zzz語版記事"));
        }

        /// <summary>
        /// <see cref="MediaWiki.IsNamespace"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestIsNamespace()
        {
            MediaWiki site = new MockFactory().GetMediaWiki("en");

            // 値が設定されていれば、前方一致で一致する、大文字小文字は区別しない
            Assert.IsFalse(site.IsNamespace("page"));
            Assert.IsTrue(site.IsNamespace("File:image.png"));
            Assert.IsTrue(site.IsNamespace("file:image.png"));
            Assert.IsFalse(site.IsNamespace("画像:image.png"));
        }

        /// <summary>
        /// <see cref="MediaWiki.FormatLinkInterwiki"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestFormatLinkInterwiki()
        {
            MediaWiki site = new MediaWiki(new Language("en"), "http://example.com");

            // LinkInterwikiFormatが空の場合、nullが返る
            site.LinkInterwikiFormat = null;
            Assert.IsNull(site.FormatLinkInterwiki("記事名", "言語", "他言語版記事名", "表示名"));
            site.LinkInterwikiFormat = string.Empty;
            Assert.IsNull(site.FormatLinkInterwiki("記事名", "言語", "他言語版記事名", "表示名"));

            // 値が設定されている場合、パラメータを埋め込んで書式化される
            site.LinkInterwikiFormat = "{{仮リンク|$1|$2|$3|label=$4}}";
            Assert.AreEqual("{{仮リンク|記事名1|言語1|他言語版記事名1|label=表示名1}}", site.FormatLinkInterwiki("記事名1", "言語1", "他言語版記事名1", "表示名1"));
            site.LinkInterwikiFormat = "{{日本語版にない記事リンク|$1|$2|$3}}";
            Assert.AreEqual("{{日本語版にない記事リンク|記事名2|言語2|他言語版記事名2}}", site.FormatLinkInterwiki("記事名2", "言語2", "他言語版記事名2", "表示名2"));
            site.LinkInterwikiFormat = "[[:$2:$3|$4]]";
            Assert.AreEqual("[[:言語3:他言語版記事名3|表示名3]]", site.FormatLinkInterwiki("記事名3", "言語3", "他言語版記事名3", "表示名3"));
            site.LinkInterwikiFormat = "xxx";
            Assert.AreEqual("xxx", site.FormatLinkInterwiki("記事名", "言語", "他言語版記事名", "表示名"));

            // 値がnull等でも特に制限はない
            site.LinkInterwikiFormat = "{{仮リンク|$1|$2|$3|label=$4}}";
            Assert.AreEqual("{{仮リンク||||label=}}", site.FormatLinkInterwiki(null, null, null, null));
        }

        /// <summary>
        /// <see cref="MediaWiki.FormatLang"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestFormatLang()
        {
            MediaWiki site = new MediaWiki(new Language("en"), "http://example.com");

            // LangFormatが空の場合、nullが返る
            site.LangFormat = null;
            Assert.IsNull(site.FormatLang("ja", "日本語テキスト"));
            site.LangFormat = string.Empty;
            Assert.IsNull(site.FormatLang("ja", "日本語テキスト"));

            // 値が設定されている場合、パラメータを埋め込んで書式化される
            site.LangFormat = "{{Lang|$1|$2}}";
            Assert.AreEqual("{{Lang|ja|日本語テキスト}}", site.FormatLang("ja", "日本語テキスト"));
            site.LangFormat = "xxx";
            Assert.AreEqual("xxx", site.FormatLang("ja", "日本語テキスト"));

            // 値がnull等でも特に制限はない
            site.LangFormat = "{{Lang|$1|$2}}";
            Assert.AreEqual("{{Lang||}}", site.FormatLang(null, null));
        }

        #endregion

        #region XMLシリアライズ用メソッドテストケース

        /// <summary>
        /// XMLデシリアライズテストケース。
        /// </summary>
        [TestMethod]
        public void TestReadXml()
        {
            // ほぼ空の状態での読み込み
            MediaWiki site;
            using (XmlReader r = XmlReader.Create(
                new StringReader("<MediaWiki><Location>http://ja.wikipedia.org</Location>"
                    + "<Language Code=\"ja\"><Names /></Language></MediaWiki>")))
            {
                site = new XmlSerializer(typeof(MediaWiki)).Deserialize(r) as MediaWiki;
            }

            Assert.IsNotNull(site);
            Assert.AreEqual("http://ja.wikipedia.org", site.Location);
            Assert.AreEqual("ja", site.Language.Code);
            Assert.AreEqual(0, site.DocumentationTemplates.Count);
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(site.DocumentationTemplateDefaultPage));
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(site.LinkInterwikiFormat));
            Assert.AreEqual(string.Empty, StringUtils.DefaultString(site.LangFormat));
            Assert.IsFalse(site.HasLanguagePage);

            // 下記プロパティは、空の場合デフォルト値が返る
            // ※ Namespacesは空の場合サーバーからデフォルト値を取得するため、ここではテストしない
            // ※ InterwikiPrefixsのgetは常にサーバーからも値を取得するため、ここではテストしない
            Assert.AreEqual("/w/api.php?format=xml&action=query&meta=siteinfo&siprop=namespaces|namespacealiases|interwikimap", site.MetaApi);
            Assert.AreEqual("/wiki/Special:Export/$1", site.ExportPath);
            Assert.AreEqual("#REDIRECT", site.Redirect);
            Assert.AreEqual(10, site.TemplateNamespace);
            Assert.AreEqual(14, site.CategoryNamespace);
            Assert.AreEqual(6, site.FileNamespace);
            Assert.IsTrue(site.MagicWords.Count > 10);

            // プロパティに値が設定された状態での読み込み
            using (XmlReader r = XmlReader.Create(new StringReader(TestXml)))
            {
                site = new XmlSerializer(typeof(MediaWiki)).Deserialize(r) as MediaWiki;
            }

            // ※ InterwikiPrefixsのgetは常にサーバーからも値を取得するため、ここではテストしない
            Assert.IsNotNull(site);
            Assert.AreEqual("http://ja.wikipedia.org", site.Location);
            Assert.AreEqual("ja", site.Language.Code);
            Assert.AreEqual("_api.xml", site.MetaApi);
            Assert.AreEqual("/export/$1", site.ExportPath);
            Assert.AreEqual("#飛ばす", site.Redirect);
            Assert.AreEqual(100, site.TemplateNamespace);
            Assert.AreEqual(101, site.CategoryNamespace);
            Assert.AreEqual(200, site.FileNamespace);
            Assert.AreEqual(2, site.MagicWords.Count);
            Assert.IsTrue(site.MagicWords.Contains("特別"));
            Assert.IsTrue(site.MagicWords.Contains("マジックワード"));
            Assert.AreEqual(2, site.DocumentationTemplates.Count);
            Assert.IsTrue(site.DocumentationTemplates.Contains("Template:ドキュメント"));
            Assert.IsTrue(site.DocumentationTemplates.Contains("テンプレート:Doc"));
            Assert.AreEqual("/サブページ", site.DocumentationTemplateDefaultPage);
            Assert.AreEqual("{{仮リンク|$1|$2|$3|label=$4}}", site.LinkInterwikiFormat);
            Assert.AreEqual("{{Lang|$1|$2}}", site.LangFormat);
            Assert.IsTrue(site.HasLanguagePage);
        }

        /// <summary>
        /// XMLシリアライズテストケース。
        /// </summary>
        [TestMethod]
        public void TestWriteXml()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));
            StringBuilder b;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            // ほぼ空の状態での出力
            // 設定ファイルに初期値を持つプロパティはデフォルト値の場合出力しないという動作あり
            b = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b, settings))
            {
                new XmlSerializer(typeof(MediaWiki)).Serialize(w, site);
            }

            Assert.AreEqual(
                "<MediaWiki><Location>http://ja.wikipedia.org</Location><Language Code=\"ja\"><Names /><Bracket /></Language>"
                + "<MetaApi /><ExportPath /><Redirect /><TemplateNamespace /><CategoryNamespace /><FileNamespace />"
                + "<MagicWords /><InterwikiPrefixs /><DocumentationTemplates /><LinkInterwikiFormat /><LangFormat />"
                + "<HasLanguagePage>False</HasLanguagePage></MediaWiki>",
                b.ToString());

            // プロパティに値が設定された場合の出力
            site.MetaApi = "_api.xml";
            site.ExportPath = "/export/$1";
            site.Redirect = "#飛ばす";
            site.TemplateNamespace = 100;
            site.CategoryNamespace = 101;
            site.FileNamespace = 200;
            site.MagicWords = new HashSet<string>(new string[] { "特別", "マジックワード" });
            site.InterwikiPrefixs = new IgnoreCaseSet(new string[] { "外部ウィキ", "ニュース" });
            site.DocumentationTemplates = new string[] { "Template:ドキュメント", "テンプレート:Doc" };
            site.DocumentationTemplateDefaultPage = "/サブページ";
            site.LinkInterwikiFormat = "{{仮リンク|$1|$2|$3|label=$4}}";
            site.LangFormat = "{{Lang|$1|$2}}";
            site.HasLanguagePage = true;

            b = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b, settings))
            {
                new XmlSerializer(typeof(MediaWiki)).Serialize(w, site);
            }

            Assert.AreEqual(TestXml, b.ToString());
        }

        #endregion
    }
}
