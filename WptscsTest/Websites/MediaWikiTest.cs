// ================================================================================================
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
    using NUnit.Framework;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;

    /// <summary>
    /// MediaWikiのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiTest
    {
        #region 定数

        /// <summary>
        /// テストデータが格納されているフォルダパス。
        /// </summary>
        private static readonly string testDir = "Data\\MediaWiki";

        #endregion

        #region テスト支援メソッド

        /// <summary>
        /// テスト用の値を設定したMediaWikiオブジェクトを返す。
        /// </summary>
        public MediaWiki GetTestServer(string language)
        {
            // ※ 下記URL生成時は、きちんとパス区切り文字を入れてやら無いとフォルダが認識されない。
            //    また、httpで取得した場合とfileで取得した場合では先頭の大文字小文字が異なることが
            //    あるため、それについては随時期待値を調整して対処。
            UriBuilder b = new UriBuilder("file", "");
            b.Path = Path.GetFullPath(testDir) + "\\";
            MediaWiki server = new MediaWiki(new Language(language), new Uri(b.Uri, language + "/").ToString());
            server.ExportPath = "$1.xml";
            server.NamespacePath = "_api.xml";
            return server;
        }

        #endregion

        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタ（MediaWiki全般）テストケース。
        /// </summary>
        [Test]
        public void TestConstructorLanguageLocation()
        {
            MediaWiki site = new MediaWiki(new Language("en"), "test");
            Assert.AreEqual("en", site.Language.Code);
            Assert.AreEqual("test", site.Location);
        }

        /// <summary>
        /// コンストラクタ（MediaWiki全般）テストケース（languageがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorLanguageLocationByLanguageNull()
        {
            MediaWiki site = new MediaWiki(null, "test");
        }

        /// <summary>
        /// コンストラクタ（MediaWiki全般）テストケース（locationがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorLanguageLocationByLocationNull()
        {
            MediaWiki site = new MediaWiki(new Language("en"), null);
        }

        /// <summary>
        /// コンストラクタ（Wikipedia用）テストケース。
        /// </summary>
        [Test]
        public void TestConstructorLanguage()
        {
            MediaWiki site = new MediaWiki(new Language("en"));
            Assert.AreEqual("en", site.Language.Code);
            Assert.AreEqual("http://en.wikipedia.org", site.Location);
        }

        /// <summary>
        /// コンストラクタ（Wikipedia用）テストケース（languageがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorLanguageByLanguageNull()
        {
            MediaWiki site = new MediaWiki(null);
        }

        #endregion

        #region 設定ファイルに初期値を持つプロパティテストケース

        /// <summary>
        /// NamespacePathプロパティテストケース。
        /// </summary>
        [Test]
        public void TestNamespacePath()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));
            // デフォルトでは設定ファイルの値が返される
            Assert.AreEqual("/w/api.php?format=xml&action=query&meta=siteinfo&siprop=namespaces|namespacealiases", site.NamespacePath);
            // 値を設定するとその値が返る
            site.NamespacePath = "test";
            Assert.AreEqual("test", site.NamespacePath);
            // 空またはnullの場合、再び設定ファイルの値が入る
            site.NamespacePath = null;
            Assert.AreEqual("/w/api.php?format=xml&action=query&meta=siteinfo&siprop=namespaces|namespacealiases", site.NamespacePath);
            site.NamespacePath = String.Empty;
            Assert.AreEqual("/w/api.php?format=xml&action=query&meta=siteinfo&siprop=namespaces|namespacealiases", site.NamespacePath);
        }

        /// <summary>
        /// ExportPathプロパティテストケース。
        /// </summary>
        [Test]
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
            site.ExportPath = String.Empty;
            Assert.AreEqual("/wiki/Special:Export/$1", site.ExportPath);
        }

        /// <summary>
        /// Redirectプロパティテストケース。
        /// </summary>
        [Test]
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
            site.Redirect = String.Empty;
            Assert.AreEqual("#REDIRECT", site.Redirect);
        }

        /// <summary>
        /// TemplateNamespaceプロパティテストケース。
        /// </summary>
        [Test]
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
        /// CategoryNamespaceプロパティテストケース。
        /// </summary>
        [Test]
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
        /// FileNamespaceプロパティテストケース。
        /// </summary>
        [Test]
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
        /// MagicWordsプロパティテストケース。
        /// </summary>
        [Test]
        public void TestMagicWords()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));
            // デフォルトでは設定ファイルの値が返される
            Assert.IsTrue(site.MagicWords.Contains("SERVERNAME"));
            // 値を設定するとその値が返る
            site.MagicWords = new string[0];
            Assert.AreEqual(0, site.MagicWords.Count);
            // nullの場合、再び設定ファイルの値が入る
            site.MagicWords = null;
            Assert.IsTrue(site.MagicWords.Contains("SERVERNAME"));
        }

        #endregion

        #region それ以外のプロパティテストケース

        /// <summary>
        /// Namespacesプロパティテストケース。
        /// </summary>
        [Test]
        public void TestNamespaces()
        {
            MediaWiki site = this.GetTestServer("en");
            // デフォルトではサーバーからダウンロードした値が返される
            IList<string> names = site.Namespaces[6];
            Assert.AreEqual("File", names[0]);
            Assert.AreEqual("File", names[1]);
            Assert.AreEqual("Image", names[2]);
            // 値を設定するとその値が返る
            IDictionary<int, IList<string>> dic = new Dictionary<int, IList<string>>();
            dic.Add(1, new string[]{"test"});
            site.Namespaces = dic;
            Assert.AreEqual(1, site.Namespaces.Count);
            // 空の場合、再び設定ファイルの値が入る
            site.Namespaces = new Dictionary<int, IList<string>>();
            Assert.AreEqual("File", names[0]);
        }

        /// <summary>
        /// Namespacesプロパティテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNamespacesNull()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));
            site.Namespaces = null;
        }

        /// <summary>
        /// DocumentationTemplatesプロパティテストケース。
        /// </summary>
        [Test]
        public void TestDocumentationTemplates()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));
            // デフォルトでは空
            Assert.IsNotNull(site.DocumentationTemplates);
            Assert.AreEqual(0, site.DocumentationTemplates.Count);
            // 値を追加
            site.DocumentationTemplates.Add("Template:Documentation");
            Assert.AreEqual(1, site.DocumentationTemplates.Count);
            Assert.AreEqual("Template:Documentation", site.DocumentationTemplates[0]);
        }

        /// <summary>
        /// DocumentationTemplateDefaultPageプロパティテストケース。
        /// </summary>
        [Test]
        public void TestDocumentationTemplateDefaultPage()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));
            // デフォルトでは空
            Assert.IsNullOrEmpty(site.DocumentationTemplateDefaultPage);
            // 値を設定するとその値が返る
            site.DocumentationTemplateDefaultPage = "/doc";
            Assert.AreEqual("/doc", site.DocumentationTemplateDefaultPage);
            site.DocumentationTemplateDefaultPage = null;
            Assert.IsNullOrEmpty(site.DocumentationTemplateDefaultPage);
        }

        /// <summary>
        /// LinkInterwikiFormatプロパティテストケース。
        /// </summary>
        [Test]
        public void TestLinkInterwikiFormat()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));
            // デフォルトでは空
            Assert.IsNullOrEmpty(site.LinkInterwikiFormat);
            // 値を設定するとその値が返る
            site.LinkInterwikiFormat = ("{{仮リンク|$1|$2|$3|label=$4}}");
            Assert.AreEqual("{{仮リンク|$1|$2|$3|label=$4}}", site.LinkInterwikiFormat);
            site.LinkInterwikiFormat = null;
            Assert.IsNullOrEmpty(site.LinkInterwikiFormat);
        }

        /// <summary>
        /// LangFormatプロパティテストケース。
        /// </summary>
        [Test]
        public void TestLangFormat()
        {
            MediaWiki site = new MediaWiki(new Language("ja"));
            // デフォルトでは空
            Assert.IsNullOrEmpty(site.LangFormat);
            // 値を設定するとその値が返る
            site.LangFormat = ("{{Lang|$1|$2}}");
            Assert.AreEqual("{{Lang|$1|$2}}", site.LangFormat);
            site.LangFormat = null;
            Assert.IsNullOrEmpty(site.LangFormat);
        }

        #endregion

        #region 公開メソッドテストケース

        /// <summary>
        /// GetPageメソッドテストケース。
        /// </summary>
        [Test]
        public void TestGetPage()
        {
            MediaWiki site = this.GetTestServer("en");
            Page page = site.GetPage("example");
            Assert.IsInstanceOf(typeof(MediaWikiPage), page);
            Assert.AreEqual("Example", page.Title);
            Assert.AreEqual(DateTime.Parse("2010/07/13 09:49:18"), page.Timestamp);
            Assert.IsNotEmpty(page.Text);
            Assert.AreEqual(site, page.Website);
        }

        /// <summary>
        /// IsMagicWordメソッドテストケース。
        /// </summary>
        [Test]
        public void TestIsMagicWord()
        {
            MediaWiki site = new MediaWiki(new Language("en"), "http://example.com");
            site.MagicWords = new List<string>();

            // 値が設定されていなければ一致しない
            Assert.IsFalse(site.IsMagicWord("CURRENTYEAR"));
            Assert.IsFalse(site.IsMagicWord("ns:1"));

            // 値が一致
            site.MagicWords.Add("CURRENTYEAR");
            Assert.IsTrue(site.IsMagicWord("CURRENTYEAR"));
            Assert.IsFalse(site.IsMagicWord("ns:1"));

            // コロンが入るものは、その前の部分までで判定
            site.MagicWords.Add("ns");
            Assert.IsTrue(site.IsMagicWord("CURRENTYEAR"));
            Assert.IsTrue(site.IsMagicWord("ns:1"));
        }

        /// <summary>
        /// FormatLinkInterwikiメソッドテストケース。
        /// </summary>
        [Test]
        public void TestFormatLinkInterwiki()
        {
            MediaWiki site = new MediaWiki(new Language("en"), "http://example.com");

            // LinkInterwikiFormatが空の場合、nullが返る
            site.LinkInterwikiFormat = null;
            Assert.IsNull(site.FormatLinkInterwiki("記事名", "言語", "他言語版記事名", "表示名"));
            site.LinkInterwikiFormat = String.Empty;
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
        /// FormatLangメソッドテストケース。
        /// </summary>
        [Test]
        public void TestFormatLang()
        {
            MediaWiki site = new MediaWiki(new Language("en"), "http://example.com");

            // LangFormatが空の場合、nullが返る
            site.LangFormat = null;
            Assert.IsNull(site.FormatLang("ja", "日本語テキスト"));
            site.LangFormat = String.Empty;
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
        [Test]
        public void TestReadXml()
        {
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
            // TODO: プロパティに値が設定されたパターンを追加すべき
            // TODO: プロパティが空の場合、きちんとデフォルト値が参照されることも確認すべき
        }

        /// <summary>
        /// XMLシリアライズテストケース。
        /// </summary>
        [Test]
        public void TestWriteXml()
        {
            Language lang = new Language("ja");
            MediaWiki site = new MediaWiki(lang);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            StringBuilder b = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(b, settings))
            {
                new XmlSerializer(typeof(MediaWiki)).Serialize(w, site);
            }

            // プロパティはデフォルト値の場合出力しないという動作あり
            Assert.AreEqual("<MediaWiki><Location>http://ja.wikipedia.org</Location><Language Code=\"ja\"><Names /><Bracket /></Language><NamespacePath /><ExportPath /><Redirect /><TemplateNamespace /><CategoryNamespace /><FileNamespace /><MagicWords /><DocumentationTemplates /><LinkInterwikiFormat /><LangFormat /></MediaWiki>", b.ToString());
            // TODO: プロパティに値が設定されたパターンを追加すべき
        }

        #endregion
    }
}
