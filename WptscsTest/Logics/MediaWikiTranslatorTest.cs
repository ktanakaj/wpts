// ================================================================================================
// <summary>
//      MediaWikiTranslatorのテストクラスソース。</summary>
//
// <copyright file="MediaWikiTranslatorTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2013 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Honememo.Parsers;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Parsers;
    using Honememo.Wptscs.Utilities;
    using Honememo.Wptscs.Websites;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="MediaWikiTranslator"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class MediaWikiTranslatorTest
    {
        #region 定数

        /// <summary>
        /// テスト結果が格納されているフォルダパス。
        /// </summary>
        private static readonly string ResultDir = Path.Combine(MockFactory.TestMediaWikiDir, "result");

        #endregion

        #region private変数

        /// <summary>
        /// テスト実施中カルチャを変更し後で戻すため、そのバックアップ。
        /// </summary>
        private System.Globalization.CultureInfo backupCulture;

        #endregion

        #region 前処理・後処理

        /// <summary>
        /// テストの前処理。
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            // ロガーの処理結果はカルチャーにより変化するため、ja-JPを明示的に設定する
            this.backupCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("ja-JP");
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
        }

        /// <summary>
        /// テストの後処理。
        /// </summary>
        [TestCleanup]
        public void TearDown()
        {
            // カルチャーを元に戻す
            System.Threading.Thread.CurrentThread.CurrentUICulture = this.backupCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = this.backupCulture;
        }

        #endregion

        #region 各処理のメソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiTranslator.ReplaceLink"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestReplaceLink()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("ja");
            translator.To = mock.GetMediaWiki("en");
            translator.To.LinkInterwikiFormat = null;

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string[]> dic = new Dictionary<string, string[]>();
            dic["en"] = new string[] { "External links" };
            dic["ja"] = new string[] { "外部リンク" };
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "ja";
            translator.HeadingTable.To = "en";
            MediaWikiPage parent;
            MediaWikiLink link;

            // 記事名だけの内部リンクで言語間リンクあり、変換先言語へのリンクとなる
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            parent = new MediaWikiPage(translator.From, "スペースシップツー");
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            Assert.AreEqual("[[Scaled Composites White Knight Two|ホワイトナイトツー]]", translator.ReplaceLink(link, parent).ToString());

            // 見出しあり
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            link.Section = "見出し";
            Assert.AreEqual("[[Scaled Composites White Knight Two#見出し|ホワイトナイトツー#見出し]]", translator.ReplaceLink(link, parent).ToString());

            // 変換パターンに該当する見出しの場合
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            link.Section = "外部リンク";
            Assert.AreEqual("[[Scaled Composites White Knight Two#External links|ホワイトナイトツー#外部リンク]]", translator.ReplaceLink(link, parent).ToString());

            // 表示名あり
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            link.Section = "外部リンク";
            link.PipeTexts.Add(new TextElement("母機"));
            Assert.AreEqual("[[Scaled Composites White Knight Two#External links|母機]]", translator.ReplaceLink(link, parent).ToString());

            // 記事名だけの内部リンクで言語間リンクなし、変換元言語へのリンクとなる
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");
            translator.To.LinkInterwikiFormat = null;
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";
            parent = new MediaWikiPage(translator.From, "example");
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            Assert.AreEqual("[[:en:Exemplum|Exemplum]]", translator.ReplaceLink(link, parent).ToString());

            // 見出しあり
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "Three examples of exempla";
            Assert.AreEqual("[[:en:Exemplum#Three examples of exempla|Exemplum#Three examples of exempla]]", translator.ReplaceLink(link, parent).ToString());

            // 変換パターンに該当する見出しの場合
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "External links";
            Assert.AreEqual("[[:en:Exemplum#外部リンク|Exemplum#External links]]", translator.ReplaceLink(link, parent).ToString());

            // 表示名あり
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "External links";
            link.PipeTexts.Add(new TextElement("Exemplum_1"));
            Assert.AreEqual("[[:en:Exemplum#外部リンク|Exemplum_1]]", translator.ReplaceLink(link, parent).ToString());

            // 記事名だけの内部リンクで赤リンク、処理されない
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            Assert.AreEqual("[[Nothing Page]]", translator.ReplaceLink(link, parent).ToString());

            // 見出しあり
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            link.Section = "Section A";
            Assert.AreEqual("[[Nothing Page#Section A]]", translator.ReplaceLink(link, parent).ToString());

            // 変換パターンに該当する見出しの場合
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            link.Section = "External links";
            Assert.AreEqual("[[Nothing Page#外部リンク]]", translator.ReplaceLink(link, parent).ToString());

            // 表示名あり
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            link.PipeTexts.Add(new TextElement("Dummy Link"));
            Assert.AreEqual("[[Nothing Page|Dummy Link]]", translator.ReplaceLink(link, parent).ToString());

            // [[Apollo&nbsp;17]] のように文字参照が入っていても処理できる
            link = new MediaWikiLink();
            link.Title = "Fuji&nbsp;(Spacecraft)";
            Assert.AreEqual("[[ふじ (宇宙船)|Fuji&nbsp;(Spacecraft)]]", translator.ReplaceLink(link, parent).ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiTranslator.ReplaceLink"/>メソッドテストケース（サブページ）。
        /// </summary>
        [TestMethod]
        public void TestReplaceLinkSubpage()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");
            MediaWikiPage parent;
            MediaWikiLink link;

            // 全て指定したサブページ
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            parent = new MediaWikiPage(translator.From, "Template:Table cell templates");
            link = new MediaWikiLink();
            link.Title = "Template:Table cell templates/doc";
            Assert.AreEqual("[[Template:Table cell templates|Template:Table cell templates/doc]]", translator.ReplaceLink(link, parent).ToString());

            // サブページ（子）
            link = new MediaWikiLink();
            link.Title = "/Doc";
            Assert.AreEqual("[[Template:Table cell templates|/Doc]]", translator.ReplaceLink(link, parent).ToString());

            // サブページ（親）、処理対象外
            link = new MediaWikiLink();
            link.Title = "../";
            Assert.AreEqual("[[../]]", translator.ReplaceLink(link, parent).ToString());

            // サブページ（兄弟）
            parent = new MediaWikiPage(translator.From, "Template:Table cell templates/xxx");
            link = new MediaWikiLink();
            link.Title = "../Doc";
            Assert.AreEqual("[[Template:Table cell templates|../Doc]]", translator.ReplaceLink(link, parent).ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiTranslator.ReplaceLink"/>メソッドテストケース（カテゴリ）。
        /// </summary>
        [TestMethod]
        public void TestReplaceLinkCategory()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("ja");
            translator.To = mock.GetMediaWiki("en");
            MediaWikiPage parent = new MediaWikiPage(translator.From, "スペースシップツー");
            MediaWikiLink link;

            // 記事名だけの内部リンクで言語間リンクあり、変換先言語でのカテゴリとなる
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            Assert.AreEqual("[[Category:Spacecraft]]", translator.ReplaceLink(link, parent).ToString());

            // ソートキーあり
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            link.PipeTexts.Add(new TextElement("すへえすしつふつう"));
            Assert.AreEqual("[[Category:Spacecraft|すへえすしつふつう]]", translator.ReplaceLink(link, parent).ToString());

            // 記事名だけの内部リンクで言語間リンクなし、変換元言語へのリンクとなり元のカテゴリはコメントとなる
            translator.To = mock.GetMediaWiki("zh-tw");
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            Assert.AreEqual("[[:ja:Category:宇宙船]]<!-- [[Category:宇宙船]] -->", translator.ReplaceLink(link, parent).ToString());

            // ソートキーあり
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            link.PipeTexts.Add(new TextElement("すへえすしつふつう"));
            Assert.AreEqual("[[:ja:Category:宇宙船]]<!-- [[Category:宇宙船|すへえすしつふつう]] -->", translator.ReplaceLink(link, parent).ToString());

            // 記事名だけの内部リンクで赤リンク、処理されない
            link = new MediaWikiLink();
            link.Title = "Category:ｘｘ国の宇宙船";
            Assert.AreEqual("[[Category:ｘｘ国の宇宙船]]", translator.ReplaceLink(link, parent).ToString());

            // ソートキーあり
            link = new MediaWikiLink();
            link.Title = "Category:ｘｘ国の宇宙船";
            link.PipeTexts.Add(new TextElement("すへえすしつふつう"));
            Assert.AreEqual("[[Category:ｘｘ国の宇宙船|すへえすしつふつう]]", translator.ReplaceLink(link, parent).ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiTranslator.ReplaceLink"/>メソッドテストケース（ファイル）。
        /// </summary>
        [TestMethod]
        public void TestReplaceLinkFile()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");
            MediaWikiPage parent;
            MediaWikiLink link;

            // 画像などのファイルについては、名前空間を他国語に置き換えるだけ
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            parent = new MediaWikiPage(translator.From, "Kepler-22b");
            link = new MediaWikiLink();
            link.Title = "File:Kepler22b-artwork.jpg";
            Assert.AreEqual("[[ファイル:Kepler22b-artwork.jpg]]", translator.ReplaceLink(link, parent).ToString());

            // パラメータあり
            link = new MediaWikiLink();
            link.Title = "File:Kepler22b-artwork.jpg";
            link.PipeTexts.Add(new TextElement("thumb"));
            link.PipeTexts.Add(new TextElement("right"));
            link.PipeTexts.Add(new TextElement("Artist's conception of Kepler-22b."));
            Assert.AreEqual("[[ファイル:Kepler22b-artwork.jpg|thumb|right|Artist's conception of Kepler-22b.]]", translator.ReplaceLink(link, parent).ToString());

            // ja→enの場合もちゃんと置き換える（en→jaはFileのままでも使えるが逆は置き換えないと動かない）
            translator.From = mock.GetMediaWiki("ja");
            translator.To = mock.GetMediaWiki("en");

            parent = new MediaWikiPage(translator.From, "ケプラー22b");
            link = new MediaWikiLink();
            link.Title = "ファイル:Kepler22b-artwork.jpg";
            Assert.AreEqual("[[File:Kepler22b-artwork.jpg]]", translator.ReplaceLink(link, parent).ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiTranslator.ReplaceLink"/>メソッドテストケース（仮リンク）。
        /// </summary>
        [TestMethod]
        public void TestReplaceLinkLinkInterwiki()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string[]> dic = new Dictionary<string, string[]>();
            dic["en"] = new string[] { "External links" };
            dic["ja"] = new string[] { "外部リンク" };
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";
            MediaWikiPage parent = new MediaWikiPage(translator.From, "example");
            MediaWikiLink link;

            // 記事名だけの内部リンクで言語間リンクなし、変換元言語へのリンクとなる
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            Assert.AreEqual("{{仮リンク|Exemplum|en|Exemplum|label=Exemplum}}", translator.ReplaceLink(link, parent).ToString());

            // 見出しあり
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "Three examples of exempla";
            Assert.AreEqual("{{仮リンク|Exemplum#Three examples of exempla|en|Exemplum#Three examples of exempla|label=Exemplum#Three examples of exempla}}", translator.ReplaceLink(link, parent).ToString());

            // 変換パターンに該当する見出しの場合
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "External links";
            Assert.AreEqual("{{仮リンク|Exemplum#外部リンク|en|Exemplum#External links|label=Exemplum#External links}}", translator.ReplaceLink(link, parent).ToString());

            // 表示名あり
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "External links";
            link.PipeTexts.Add(new TextElement("Exemplum_1"));
            Assert.AreEqual("{{仮リンク|Exemplum#外部リンク|en|Exemplum#External links|label=Exemplum_1}}", translator.ReplaceLink(link, parent).ToString());

            // 言語間リンクありの場合、仮リンクは使用されない
            link = new MediaWikiLink();
            link.Title = "Example.com";
            Assert.AreEqual("[[Example.com|Example.com]]", translator.ReplaceLink(link, parent).ToString());

            // 記事名だけの内部リンクで赤リンクも、使用されない
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            Assert.AreEqual("[[Nothing Page]]", translator.ReplaceLink(link, parent).ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiTranslator.ReplaceTemplate"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestReplaceTemplate()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");
            MediaWikiPage parent = new MediaWikiPage(translator.From, "example");
            MediaWikiTemplate template;

            // テンプレート名だけで言語間リンクあり、変換先言語でのテンプレートとなる
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            template = new MediaWikiTemplate("Citation needed");
            Assert.AreEqual("{{要出典}}", translator.ReplaceTemplate(template, parent).ToString());

            // パラメータあり
            template = new MediaWikiTemplate("Citation needed");
            template.PipeTexts.Add(new TextElement("date=January 2012"));
            Assert.AreEqual("{{要出典|date=January 2012}}", translator.ReplaceTemplate(template, parent).ToString());

            // テンプレート名だけで言語間リンクなし、変換元言語へのリンクとなり元のテンプレートはコメントとなる
            template = new MediaWikiTemplate("context");
            Assert.AreEqual("[[:en:Template:context]]<!-- {{context}} -->", translator.ReplaceTemplate(template, parent).ToString());

            // パラメータあり
            template = new MediaWikiTemplate("context");
            template.PipeTexts.Add(new TextElement("Sample"));
            Assert.AreEqual("[[:en:Template:context]]<!-- {{context|Sample}} -->", translator.ReplaceTemplate(template, parent).ToString());

            // テンプレート名だけで赤リンク、処理されない
            template = new MediaWikiTemplate("Invalid Template");
            Assert.AreEqual("{{Invalid Template}}", translator.ReplaceTemplate(template, parent).ToString());

            // パラメータあり
            template = new MediaWikiTemplate("Invalid Template");
            template.PipeTexts.Add(new TextElement("parameter=1"));
            Assert.AreEqual("{{Invalid Template|parameter=1}}", translator.ReplaceTemplate(template, parent).ToString());

            // システム定義変数、処理されない
            template = new MediaWikiTemplate("PAGENAME");
            Assert.AreEqual("{{PAGENAME}}", translator.ReplaceTemplate(template, parent).ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiTranslator.ReplaceTemplate"/>メソッドテストケース（入れ子）。
        /// </summary>
        [TestMethod]
        public void TestReplaceTemplateNested()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");
            MediaWikiPage parent = new MediaWikiPage(translator.From, "example");
            MediaWikiTemplate template;
            MediaWikiLink link;
            ListElement list;

            // テンプレート名だけで言語間リンクあり、入れ子も処理される
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            template = new MediaWikiTemplate("Citation needed");
            template.PipeTexts.Add(new TextElement("date=January 2012"));
            list = new ListElement();
            list.Add(new TextElement("note=See also "));
            link = new MediaWikiLink();
            link.Title = "Fuji (Spacecraft)";
            list.Add(link);
            template.PipeTexts.Add(list);
            Assert.AreEqual("{{要出典|date=January 2012|note=See also [[ふじ (宇宙船)|Fuji (Spacecraft)]]}}", translator.ReplaceTemplate(template, parent).ToString());

            // テンプレート名だけで言語間リンクなし、入れ子は処理されない
            template = new MediaWikiTemplate("context");
            template.PipeTexts.Add(new TextElement("Sample"));
            list = new ListElement();
            list.Add(new TextElement("note=See also "));
            link = new MediaWikiLink();
            link.Title = "Fuji (Spacecraft)";
            list.Add(link);
            template.PipeTexts.Add(list);
            Assert.AreEqual("[[:en:Template:context]]<!-- {{context|Sample|note=See also [[Fuji (Spacecraft)]]}} -->", translator.ReplaceTemplate(template, parent).ToString());

            // テンプレート名だけで赤リンク、入れ子は処理されない
            template = new MediaWikiTemplate("Invalid Template");
            template.PipeTexts.Add(new TextElement("parameter=1"));
            list = new ListElement();
            list.Add(new TextElement("note=See also "));
            link = new MediaWikiLink();
            link.Title = "Fuji (Spacecraft)";
            list.Add(link);
            template.PipeTexts.Add(list);
            Assert.AreEqual("{{Invalid Template|parameter=1|note=See also [[Fuji (Spacecraft)]]}}", translator.ReplaceTemplate(template, parent).ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiTranslator.ReplaceHeading"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestReplaceHeading()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string[]> dic = new Dictionary<string, string[]>();
            dic["en"] = new string[] { "External links" };
            dic["ja"] = new string[] { "外部リンク" };
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";

            MediaWikiPage parent = new MediaWikiPage(translator.From, "example");
            MediaWikiHeading heading = new MediaWikiHeading();

            // 対訳表に登録されていない見出し
            // ※ 以下リストを毎回作り直しているのは、更新されてしまうケースがあるため
            heading.Level = 2;
            heading.Add(new TextElement(" invalid section "));
            Assert.AreEqual("== invalid section ==", translator.ReplaceHeading(heading, parent).ToString());

            // 対訳表に登録されている見出し
            heading.Clear();
            heading.Add(new TextElement(" External links "));
            Assert.AreEqual("==外部リンク==", translator.ReplaceHeading(heading, parent).ToString());

            // 一部が内部リンク等になっている場合、対訳表とは一致しない
            heading.Clear();
            heading.Add(new TextElement(" External "));
            heading.Add(new MediaWikiLink("link"));
            heading.Add(new TextElement("s "));
            Assert.AreEqual("== External [[link]]s ==", translator.ReplaceHeading(heading, parent).ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiTranslator.ReplaceHeading"/>メソッドテストケース（入れ子）。
        /// </summary>
        [TestMethod]
        public void TestReplaceHeadingNested()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("ja");
            translator.To = mock.GetMediaWiki("en");

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string[]> dic = new Dictionary<string, string[]>();
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "ja";
            translator.HeadingTable.To = "en";

            MediaWikiPage parent = new MediaWikiPage(translator.From, "スペースシップツー");
            MediaWikiHeading heading = new MediaWikiHeading();

            // 対訳表に登録されていない見出しの場合、入れ子も処理される
            // ※ 以下リストを毎回作り直しているのは、更新されてしまうケースがあるため
            heading.Level = 3;
            heading.Add(new TextElement(" "));
            heading.Add(new MediaWikiLink("宇宙旅行"));
            heading.Add(new MediaWikiTemplate("ref-en"));
            heading.Add(new TextElement(" "));
            Assert.AreEqual("=== [[Space tourism|宇宙旅行]]{{En icon}} ===", translator.ReplaceHeading(heading, parent).ToString());

            // 対訳表に登録されている見出しの場合、入れ子は処理されない
            dic["ja"] = new string[] { "[[宇宙旅行]]{{ref-en}}" };
            dic["en"] = new string[] { "[[弾道飛行]]{{ref-en}}" };
            heading.Clear();
            heading.Add(new TextElement(" "));
            heading.Add(new MediaWikiLink("宇宙旅行"));
            heading.Add(new MediaWikiTemplate("ref-en"));
            heading.Add(new TextElement(" "));
            Assert.AreEqual("===[[弾道飛行]]{{ref-en}}===", translator.ReplaceHeading(heading, parent).ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiTranslator.CreateOpening"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestCreateOpening()
        {
            // From, Toの設定に応じて変換後記事の冒頭部を作り出す
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");

            // 完全な形
            Assert.AreEqual(
                "'''xxx'''（[[英語|英]]: '''{{Lang|en|English's title}}'''）\n\n",
                translator.CreateOpening("English's title"));

            // リンクなし・略称あり
            translator.To.HasLanguagePage = false;
            Assert.AreEqual(
                "'''xxx'''（英: '''{{Lang|en|English's title}}'''）\n\n",
                translator.CreateOpening("English's title"));

            // リンクなし・略称なし
            Language.LanguageName name = translator.From.Language.Names["ja"];
            name.ShortName = null;
            translator.From.Language.Names["ja"] = name;
            Assert.AreEqual(
                "'''xxx'''（英語: '''{{Lang|en|English's title}}'''）\n\n",
                translator.CreateOpening("English's title"));

            // リンクあり・略称なし
            translator.To.HasLanguagePage = true;
            Assert.AreEqual(
                "'''xxx'''（[[英語]]: '''{{Lang|en|English's title}}'''）\n\n",
                translator.CreateOpening("English's title"));

            // 言語名なし・リンク有り
            translator.From.Language.Names.Remove("ja");
            Assert.AreEqual(
                "'''xxx'''（'''{{Lang|en|English's title}}'''）\n\n",
                translator.CreateOpening("English's title"));

            // 言語名なし・リンクなし
            translator.To.HasLanguagePage = false;
            Assert.AreEqual(
                "'''xxx'''（'''{{Lang|en|English's title}}'''）\n\n",
                translator.CreateOpening("English's title"));

            // 仮リンクフォーマットなし
            translator.To.LangFormat = null;
            Assert.AreEqual(
                "'''xxx'''（'''English's title'''）\n\n",
                translator.CreateOpening("English's title"));

            // 括弧のフォーマット変更
            // ※ パラメータ上なしには出来ない
            translator.To.Language.Bracket = "「$1」";
            Assert.AreEqual(
                "'''xxx'''「'''English's title'''」\n\n",
                translator.CreateOpening("English's title"));

            // 括弧のフォーマットの$1なし
            translator.To.Language.Bracket = "後で消す";
            Assert.AreEqual(
                "'''xxx'''後で消す\n\n",
                translator.CreateOpening("English's title"));
        }

        #endregion

        #region 全体テストケース

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。基本動作。
        /// </summary>
        [TestMethod]
        public void TestExampleIgnoreHeading()
        {
            MockFactory mock = new MockFactory();
            MediaWiki from = mock.GetMediaWiki("en");
            MediaWikiTranslator translator = new MediaWikiTranslator();
            translator.From = from;
            translator.To = mock.GetMediaWiki("ja");
            translator.To.LinkInterwikiFormat = null;
            translator.HeadingTable = new TranslationTable();
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";
            translator.Run("example");

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(ResultDir, "example_定型句なし.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.xx", "<!-- " + FormUtils.ApplicationName()),
                translator.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(ResultDir, "example_定型句なし.log")).Replace("file:///xxx/Data/MediaWiki/en/", from.Location),
                translator.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。基本動作見出しの変換含む。
        /// </summary>
        /// <remarks>C++/CLI版の0.73までと同等の動作。</remarks>
        [TestMethod]
        public void TestExample()
        {
            MockFactory mock = new MockFactory();
            MediaWiki from = mock.GetMediaWiki("en");
            MediaWikiTranslator translator = new MediaWikiTranslator();
            translator.From = from;
            translator.To = mock.GetMediaWiki("ja");
            translator.To.LinkInterwikiFormat = null;
            translator.To.LangFormat = null;

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string[]> dic = new Dictionary<string, string[]>();
            dic["en"] = new string[] { "See also" };
            dic["ja"] = new string[] { "関連項目" };
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";
            translator.Run("example");

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(ResultDir, "example.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.73", "<!-- " + FormUtils.ApplicationName()),
                translator.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(ResultDir, "example.log")).Replace("http://en.wikipedia.org", from.Location),
                translator.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。キャッシュ使用。
        /// </summary>
        [TestMethod]
        public void TestExampleWithCache()
        {
            MockFactory mock = new MockFactory();
            MediaWiki from = mock.GetMediaWiki("en");
            MediaWikiTranslator translator = new MediaWikiTranslator();
            translator.From = from;
            translator.To = mock.GetMediaWiki("ja");
            translator.To.LinkInterwikiFormat = null;

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string[]> dic = new Dictionary<string, string[]>();
            dic["en"] = new string[] { "See also" };
            dic["ja"] = new string[] { "関連項目" };
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";

            // 以下のキャッシュパターンを指定して実行
            TranslationDictionary table = new TranslationDictionary("en", "ja");
            table.Add("Template:Wikiquote", new TranslationDictionary.Item());
            table.Add("example.org", new TranslationDictionary.Item());
            table.Add(".example", new TranslationDictionary.Item { Word = "。さんぷる", Alias = ".dummy" });
            table.Add("Template:Disambig", new TranslationDictionary.Item { Word = "Template:曖昧さ回避" });
            translator.ItemTable = table;
            translator.Run("example");

            // キャッシュに今回の処理で取得した内容が更新されているかを確認
            Assert.IsTrue(table.ContainsKey("example.com"));
            Assert.AreEqual("Example.com", table["example.com"].Word);
            Assert.IsNull(table["example.com"].Alias);
            Assert.IsNotNull(table["example.com"].Timestamp);
            Assert.IsTrue(table.ContainsKey("Exemplum"));
            Assert.AreEqual(string.Empty, table["Exemplum"].Word);
            Assert.IsNull(table["Exemplum"].Alias);
            Assert.IsNotNull(table["Exemplum"].Timestamp);
            Assert.IsTrue(table.ContainsKey("example.net"));
            Assert.AreEqual("Example.com", table["example.net"].Word);
            Assert.AreEqual("Example.com", table["example.net"].Alias);
            Assert.IsNotNull(table["example.net"].Timestamp);

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(ResultDir, "example_キャッシュ使用.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.xx", "<!-- " + FormUtils.ApplicationName()),
                translator.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(ResultDir, "example_キャッシュ使用.log")).Replace("file:///xxx/Data/MediaWiki/en/", from.Location),
                translator.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。基本動作見出しの変換、{{仮リンク}}への置き換え含む。
        /// </summary>
        [TestMethod]
        public void TestExampleWithLinkInterwiki()
        {
            MockFactory mock = new MockFactory();
            MediaWiki from = mock.GetMediaWiki("en");
            MediaWikiTranslator translator = new MediaWikiTranslator();
            translator.From = from;
            translator.To = mock.GetMediaWiki("ja");

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string[]> dic = new Dictionary<string, string[]>();
            dic["en"] = new string[] { "See also" };
            dic["ja"] = new string[] { "関連項目" };
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";
            translator.Run("example");

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(ResultDir, "example_仮リンク有効.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.73", "<!-- " + FormUtils.ApplicationName()),
                translator.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(ResultDir, "example.log")).Replace("http://en.wikipedia.org", from.Location),
                translator.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース（日本語版→英語版）。
        /// </summary>
        /// <remarks>C++/CLI版の0.73までと同等の動作。</remarks>
        [TestMethod]
        public void TestSpaceShipTwo()
        {
            MockFactory mock = new MockFactory();
            MediaWiki from = mock.GetMediaWiki("ja");
            MediaWikiTranslator translator = new MediaWikiTranslator();
            translator.From = from;
            translator.To = mock.GetMediaWiki("en");
            translator.To.LinkInterwikiFormat = null;
            translator.ItemTable = new TranslationDictionary("ja", "en");
            translator.IsContinueAtInterwikiExisted = (string interwiki) => true;

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string[]> dic = new Dictionary<string, string[]>();
            dic["en"] = new string[] { "See Also" };
            dic["ja"] = new string[] { "関連項目" };
            translator.HeadingTable.Add(dic);
            dic = new Dictionary<string, string[]>();
            dic["en"] = new string[] { "External links" };
            dic["ja"] = new string[] { "外部リンク" };
            translator.HeadingTable.Add(dic);
            dic = new Dictionary<string, string[]>();
            dic["en"] = new string[] { "Notes" };
            dic["ja"] = new string[] { "脚注" };
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "ja";
            translator.HeadingTable.To = "en";
            translator.Run("スペースシップツー");

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(ResultDir, "スペースシップツー.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.73", "<!-- " + FormUtils.ApplicationName()),
                translator.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(ResultDir, "スペースシップツー.log")).Replace("http://ja.wikipedia.org", from.Location),
                translator.Log);
        }

        /// <summary>
        /// Runを通しで実行するテストケース（対象記事なし）。
        /// </summary>
        [TestMethod]
        public void TestPageNothing()
        {
            MockFactory mock = new MockFactory();
            MediaWiki from = mock.GetMediaWiki("en");
            Translator translator = new MediaWikiTranslator();
            translator.From = from;
            translator.To = mock.GetMediaWiki("ja");

            try
            {
                translator.Run("Nothing Page");
                Assert.Fail();
            }
            catch (ApplicationException)
            {
                // 実行ログを期待されるログと比較する
                Assert.AreEqual(
                    ("http://en.wikipedia.org より [[Nothing Page]] を取得。\r\n"
                    + "→ 翻訳元として指定された記事は存在しません。記事名を確認してください。\r\n")
                    .Replace("http://en.wikipedia.org", from.Location),
                    translator.Log);
            }
        }

        #endregion

        #region テスト用クラス

        /// <summary>
        /// <see cref="MediaWikiTranslator"/>テスト用のクラスです。
        /// </summary>
        private class TestMediaWikiTranslator : MediaWikiTranslator
        {
            #region 非公開メソッドテスト用のオーラーライドメソッド

            /// <summary>
            /// 内部リンクを解析し、変換先言語の記事へのリンクに変換する。
            /// </summary>
            /// <param name="link">変換元リンク。</param>
            /// <param name="parent">ページ要素を取得した変換元記事。</param>
            /// <returns>変換済みリンク。</returns>
            public new IElement ReplaceLink(MediaWikiLink link, MediaWikiPage parent)
            {
                return base.ReplaceLink(link, parent);
            }

            /// <summary>
            /// テンプレートを解析し、変換先言語の記事へのテンプレートに変換する。
            /// </summary>
            /// <param name="template">変換元テンプレート。</param>
            /// <param name="parent">ページ要素を取得した変換元記事。</param>
            /// <returns>変換済みテンプレート。</returns>
            public new IElement ReplaceTemplate(MediaWikiTemplate template, MediaWikiPage parent)
            {
                return base.ReplaceTemplate(template, parent);
            }

            /// <summary>
            /// 指定された見出しに対して、対訳表による変換を行う。
            /// </summary>
            /// <param name="heading">見出し。</param>
            /// <param name="parent">ページ要素を取得した変換元記事。</param>
            /// <returns>変換後の見出し。</returns>
            public new IElement ReplaceHeading(MediaWikiHeading heading, MediaWikiPage parent)
            {
                return base.ReplaceHeading(heading, parent);
            }

            /// <summary>
            /// 変換後記事冒頭用の「'''日本語記事名'''（[[英語|英]]: '''{{Lang|en|英語記事名}}'''）」みたいなのを作成する。
            /// </summary>
            /// <param name="title">翻訳支援対象の記事名。</param>
            /// <returns>冒頭部のテキスト。</returns>
            public new string CreateOpening(string title)
            {
                return base.CreateOpening(title);
            }

            #endregion
        }

        #endregion
    }
}
