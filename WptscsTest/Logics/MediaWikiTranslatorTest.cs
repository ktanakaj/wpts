// ================================================================================================
// <summary>
//      MediaWikiTranslatorのテストクラスソース。</summary>
//
// <copyright file="MediaWikiTranslatorTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using NUnit.Framework;
    using Honememo.Tests;
    using Honememo.Parsers;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Parsers;
    using Honememo.Wptscs.Utilities;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// MediaWikiTranslatorのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiTranslatorTest
    {
        #region テスト用クラス

        /// <summary>
        /// MediaWikiTranslatorテスト用のクラスです。
        /// </summary>
        public class TestMediaWikiTranslator : MediaWikiTranslator
        {
            #region 非公開メソッドテスト用のオーラーライドメソッド

            /// <summary>
            /// 内部リンクを解析し、変換先言語の記事へのリンクに変換する。
            /// </summary>
            /// <param name="link">変換元リンク。</param>
            /// <param name="parent">サブページ用の親記事タイトル。</param>
            /// <returns>変換済みリンク。</returns>
            public new IElement ReplaceLink(MediaWikiLink link, string parent)
            {
                return base.ReplaceLink(link, parent);
            }

            /// <summary>
            /// テンプレートを解析し、変換先言語の記事へのテンプレートに変換する。
            /// </summary>
            /// <param name="template">変換元テンプレート。</param>
            /// <param name="parent">サブページ用の親記事タイトル。</param>
            /// <returns>変換済みテンプレート。</returns>
            public new IElement ReplaceTemplate(MediaWikiTemplate template, string parent)
            {
                return base.ReplaceTemplate(template, parent);
            }

            /// <summary>
            /// 指定された見出しに対して、対訳表による変換を行う。
            /// </summary>
            /// <param name="heading">見出し。</param>
            /// <param name="parent">サブページ用の親記事タイトル。</param>
            /// <returns>変換後の見出し。</returns>
            public new IElement ReplaceHeading(MediaWikiHeading heading, string parent)
            {
                return base.ReplaceHeading(heading, parent);
            }

            #endregion
        }

        #endregion

        #region 定数

        /// <summary>
        /// テスト結果が格納されているフォルダパス。
        /// </summary>
        private static readonly string resultDir = Path.Combine(MockFactory.TestMediaWikiDir, "result");

        #endregion

        #region 各処理のメソッドテストケース

        /// <summary>
        /// ReplaceLinkメソッドテストケース。
        /// </summary>
        [Test]
        public void TestReplaceLink()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("ja");
            translator.To = mock.GetMediaWiki("en");
            translator.To.LinkInterwikiFormat = null;

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["en"] = "External links";
            dic["ja"] = "外部リンク";
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "ja";
            translator.HeadingTable.To = "en";
            MediaWikiLink link;

            // 記事名だけの内部リンクで言語間リンクあり、変換先言語へのリンクとなる
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            Assert.AreEqual("[[Scaled Composites White Knight Two|ホワイトナイトツー]]", translator.ReplaceLink(link, "スペースシップツー").ToString());

            // 見出しあり
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            link.Section = "見出し";
            Assert.AreEqual("[[Scaled Composites White Knight Two#見出し|ホワイトナイトツー#見出し]]", translator.ReplaceLink(link, "スペースシップツー").ToString());

            // 変換パターンに該当する見出しの場合
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            link.Section = "外部リンク";
            Assert.AreEqual("[[Scaled Composites White Knight Two#External links|ホワイトナイトツー#外部リンク]]", translator.ReplaceLink(link, "スペースシップツー").ToString());

            // 表示名あり
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            link.Section = "外部リンク";
            link.PipeTexts.Add(new TextElement("母機"));
            Assert.AreEqual("[[Scaled Composites White Knight Two#External links|母機]]", translator.ReplaceLink(link, "スペースシップツー").ToString());

            // 記事名だけの内部リンクで言語間リンクなし、変換元言語へのリンクとなる
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");
            translator.To.LinkInterwikiFormat = null;
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            Assert.AreEqual("[[:en:Exemplum|Exemplum]]", translator.ReplaceLink(link, "example").ToString());

            // 見出しあり
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "Three examples of exempla";
            Assert.AreEqual("[[:en:Exemplum#Three examples of exempla|Exemplum#Three examples of exempla]]", translator.ReplaceLink(link, "example").ToString());

            // 変換パターンに該当する見出しの場合
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "External links";
            Assert.AreEqual("[[:en:Exemplum#外部リンク|Exemplum#External links]]", translator.ReplaceLink(link, "example").ToString());

            // 表示名あり
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "External links";
            link.PipeTexts.Add(new TextElement("Exemplum_1"));
            Assert.AreEqual("[[:en:Exemplum#外部リンク|Exemplum_1]]", translator.ReplaceLink(link, "example").ToString());

            // 記事名だけの内部リンクで赤リンク、処理されない
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            Assert.AreEqual("[[Nothing Page]]", translator.ReplaceLink(link, "example").ToString());

            // 見出しあり
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            link.Section = "Section A";
            Assert.AreEqual("[[Nothing Page#Section A]]", translator.ReplaceLink(link, "example").ToString());

            // 変換パターンに該当する見出しの場合
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            link.Section = "External links";
            Assert.AreEqual("[[Nothing Page#外部リンク]]", translator.ReplaceLink(link, "example").ToString());

            // 表示名あり
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            link.PipeTexts.Add(new TextElement("Dummy Link"));
            Assert.AreEqual("[[Nothing Page|Dummy Link]]", translator.ReplaceLink(link, "example").ToString());

            // [[Apollo&nbsp;17]] のように文字参照が入っていても処理できる
            link = new MediaWikiLink();
            link.Title = "Fuji&nbsp;(Spacecraft)";
            Assert.AreEqual("[[ふじ (宇宙船)|Fuji&nbsp;(Spacecraft)]]", translator.ReplaceLink(link, "example").ToString());
        }

        /// <summary>
        /// ReplaceLinkメソッドテストケース（カテゴリ）。
        /// </summary>
        [Test]
        public void TestReplaceLinkCategory()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("ja");
            translator.To = mock.GetMediaWiki("en");
            MediaWikiLink link;

            // 記事名だけの内部リンクで言語間リンクあり、変換先言語でのカテゴリとなる
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            Assert.AreEqual("[[Category:Manned spacecraft]]", translator.ReplaceLink(link, "スペースシップツー").ToString());

            // ソートキーあり
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            link.PipeTexts.Add(new TextElement("すへえすしつふつう"));
            Assert.AreEqual("[[Category:Manned spacecraft|すへえすしつふつう]]", translator.ReplaceLink(link, "スペースシップツー").ToString());

            // 記事名だけの内部リンクで言語間リンクなし、変換元言語へのリンクとなり元のカテゴリはコメントとなる
            translator.To = mock.GetMediaWiki("it");
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            Assert.AreEqual("[[:ja:Category:宇宙船]]<!-- [[Category:宇宙船]] -->", translator.ReplaceLink(link, "スペースシップツー").ToString());

            // ソートキーあり
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            link.PipeTexts.Add(new TextElement("すへえすしつふつう"));
            Assert.AreEqual("[[:ja:Category:宇宙船]]<!-- [[Category:宇宙船|すへえすしつふつう]] -->", translator.ReplaceLink(link, "スペースシップツー").ToString());

            // 記事名だけの内部リンクで赤リンク、処理されない
            link = new MediaWikiLink();
            link.Title = "Category:ｘｘ国の宇宙船";
            Assert.AreEqual("[[Category:ｘｘ国の宇宙船]]", translator.ReplaceLink(link, "スペースシップツー").ToString());

            // ソートキーあり
            link = new MediaWikiLink();
            link.Title = "Category:ｘｘ国の宇宙船";
            link.PipeTexts.Add(new TextElement("すへえすしつふつう"));
            Assert.AreEqual("[[Category:ｘｘ国の宇宙船|すへえすしつふつう]]", translator.ReplaceLink(link, "スペースシップツー").ToString());
        }

        /// <summary>
        /// ReplaceLinkメソッドテストケース（ファイル）。
        /// </summary>
        [Test]
        public void TestReplaceLinkFile()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");
            MediaWikiLink link;

            // 画像などのファイルについては、名前空間を他国語に置き換えるだけ
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            link = new MediaWikiLink();
            link.Title = "File:Kepler22b-artwork.jpg";
            Assert.AreEqual("[[ファイル:Kepler22b-artwork.jpg]]", translator.ReplaceLink(link, "Kepler-22b").ToString());

            // パラメータあり
            link = new MediaWikiLink();
            link.Title = "File:Kepler22b-artwork.jpg";
            link.PipeTexts.Add(new TextElement("thumb"));
            link.PipeTexts.Add(new TextElement("right"));
            link.PipeTexts.Add(new TextElement("Artist's conception of Kepler-22b."));
            Assert.AreEqual("[[ファイル:Kepler22b-artwork.jpg|thumb|right|Artist's conception of Kepler-22b.]]", translator.ReplaceLink(link, "Kepler-22b").ToString());

            // ja→enの場合もちゃんと置き換える（en→jaはFileのままでも使えるが逆は置き換えないと動かない）
            translator.From = mock.GetMediaWiki("ja");
            translator.To = mock.GetMediaWiki("en");

            link = new MediaWikiLink();
            link.Title = "ファイル:Kepler22b-artwork.jpg";
            Assert.AreEqual("[[File:Kepler22b-artwork.jpg]]", translator.ReplaceLink(link, "ケプラー22b").ToString());
        }

        /// <summary>
        /// ReplaceLinkメソッドテストケース（仮リンク）。
        /// </summary>
        [Test]
        public void TestReplaceLinkLinkInterwiki()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["en"] = "External links";
            dic["ja"] = "外部リンク";
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";
            MediaWikiLink link;

            // 記事名だけの内部リンクで言語間リンクなし、変換元言語へのリンクとなる
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            Assert.AreEqual("{{仮リンク|Exemplum|en|Exemplum|label=Exemplum}}", translator.ReplaceLink(link, "example").ToString());

            // 見出しあり
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "Three examples of exempla";
            Assert.AreEqual("{{仮リンク|Exemplum#Three examples of exempla|en|Exemplum#Three examples of exempla|label=Exemplum#Three examples of exempla}}", translator.ReplaceLink(link, "example").ToString());

            // 変換パターンに該当する見出しの場合
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "External links";
            Assert.AreEqual("{{仮リンク|Exemplum#外部リンク|en|Exemplum#External links|label=Exemplum#External links}}", translator.ReplaceLink(link, "example").ToString());

            // 表示名あり
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "External links";
            link.PipeTexts.Add(new TextElement("Exemplum_1"));
            Assert.AreEqual("{{仮リンク|Exemplum#外部リンク|en|Exemplum#External links|label=Exemplum_1}}", translator.ReplaceLink(link, "example").ToString());

            // 言語間リンクありの場合、仮リンクは使用されない
            link = new MediaWikiLink();
            link.Title = "Example.com";
            Assert.AreEqual("[[Example.com|Example.com]]", translator.ReplaceLink(link, "example").ToString());

            // 記事名だけの内部リンクで赤リンクも、使用されない
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            Assert.AreEqual("[[Nothing Page]]", translator.ReplaceLink(link, "example").ToString());
        }

        /// <summary>
        /// ReplaceTemplateメソッドテストケース。
        /// </summary>
        [Test]
        public void TestReplaceTemplate()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");
            MediaWikiTemplate template;

            // テンプレート名だけで言語間リンクあり、変換先言語でのテンプレートとなる
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            template = new MediaWikiTemplate("Citation needed");
            Assert.AreEqual("{{要出典}}", translator.ReplaceTemplate(template, "example").ToString());

            // パラメータあり
            template = new MediaWikiTemplate("Citation needed");
            template.PipeTexts.Add(new TextElement("date=January 2012"));
            Assert.AreEqual("{{要出典|date=January 2012}}", translator.ReplaceTemplate(template, "example").ToString());

            // テンプレート名だけで言語間リンクなし、変換元言語へのリンクとなり元のテンプレートはコメントとなる
            template = new MediaWikiTemplate("Wiktionary");
            Assert.AreEqual("[[:en:Template:Wiktionary]]<!-- {{Wiktionary}} -->", translator.ReplaceTemplate(template, "example").ToString());

            // パラメータあり
            template = new MediaWikiTemplate("Wiktionary");
            template.PipeTexts.Add(new TextElement("Sample"));
            Assert.AreEqual("[[:en:Template:Wiktionary]]<!-- {{Wiktionary|Sample}} -->", translator.ReplaceTemplate(template, "example").ToString());

            // テンプレート名だけで赤リンク、処理されない
            template = new MediaWikiTemplate("Invalid Template");
            Assert.AreEqual("{{Invalid Template}}", translator.ReplaceTemplate(template, "example").ToString());

            // パラメータあり
            template = new MediaWikiTemplate("Invalid Template");
            template.PipeTexts.Add(new TextElement("parameter=1"));
            Assert.AreEqual("{{Invalid Template|parameter=1}}", translator.ReplaceTemplate(template, "example").ToString());

            // システム定義変数、処理されない
            template = new MediaWikiTemplate("PAGENAME");
            Assert.AreEqual("{{PAGENAME}}", translator.ReplaceTemplate(template, "example").ToString());
        }

        /// <summary>
        /// ReplaceTemplateメソッドテストケース（入れ子）。
        /// </summary>
        [Test]
        public void TestReplaceTemplateNested()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");
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
            Assert.AreEqual("{{要出典|date=January 2012|note=See also [[ふじ (宇宙船)|Fuji (Spacecraft)]]}}", translator.ReplaceTemplate(template, "example").ToString());

            // テンプレート名だけで言語間リンクなし、入れ子は処理されない
            template = new MediaWikiTemplate("Wiktionary");
            template.PipeTexts.Add(new TextElement("Sample"));
            list = new ListElement();
            list.Add(new TextElement("note=See also "));
            link = new MediaWikiLink();
            link.Title = "Fuji (Spacecraft)";
            list.Add(link);
            template.PipeTexts.Add(list);
            Assert.AreEqual("[[:en:Template:Wiktionary]]<!-- {{Wiktionary|Sample|note=See also [[Fuji (Spacecraft)]]}} -->", translator.ReplaceTemplate(template, "example").ToString());

            // テンプレート名だけで赤リンク、入れ子は処理されない
            template = new MediaWikiTemplate("Invalid Template");
            template.PipeTexts.Add(new TextElement("parameter=1"));
            list = new ListElement();
            list.Add(new TextElement("note=See also "));
            link = new MediaWikiLink();
            link.Title = "Fuji (Spacecraft)";
            list.Add(link);
            template.PipeTexts.Add(list);
            Assert.AreEqual("{{Invalid Template|parameter=1|note=See also [[Fuji (Spacecraft)]]}}", translator.ReplaceTemplate(template, "example").ToString());
        }

        /// <summary>
        /// ReplaceHeadingメソッドテストケース。
        /// </summary>
        [Test]
        public void TestReplaceHeading()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("en");
            translator.To = mock.GetMediaWiki("ja");

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["en"] = "External links";
            dic["ja"] = "外部リンク";
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";

            MediaWikiHeading heading = new MediaWikiHeading();

            // 対訳表に登録されていない見出し
            // ※ 以下リストを毎回作り直しているのは、更新されてしまうケースがあるため
            heading.Level = 2;
            heading.Add(new TextElement(" invalid section "));
            Assert.AreEqual("== invalid section ==", translator.ReplaceHeading(heading, "example").ToString());

            // 対訳表に登録されている見出し
            heading.Clear();
            heading.Add(new TextElement(" External links "));
            Assert.AreEqual("==外部リンク==", translator.ReplaceHeading(heading, "example").ToString());

            // 一部が内部リンク等になっている場合、対訳表とは一致しない
            heading.Clear();
            heading.Add(new TextElement(" External "));
            heading.Add(new MediaWikiLink("link"));
            heading.Add(new TextElement("s "));
            Assert.AreEqual("== External [[link]]s ==", translator.ReplaceHeading(heading, "example").ToString());
        }

        /// <summary>
        /// ReplaceHeadingメソッドテストケース（入れ子）。
        /// </summary>
        [Test]
        public void TestReplaceHeadingNested()
        {
            TestMediaWikiTranslator translator = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translator.From = mock.GetMediaWiki("ja");
            translator.To = mock.GetMediaWiki("en");

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "ja";
            translator.HeadingTable.To = "en";

            MediaWikiHeading heading = new MediaWikiHeading();

            // 対訳表に登録されていない見出しの場合、入れ子も処理される
            // ※ 以下リストを毎回作り直しているのは、更新されてしまうケースがあるため
            heading.Level = 3;
            heading.Add(new TextElement(" "));
            heading.Add(new MediaWikiLink("宇宙旅行"));
            heading.Add(new MediaWikiTemplate("ref-en"));
            heading.Add(new TextElement(" "));
            Assert.AreEqual("=== [[Space tourism|宇宙旅行]]{{En icon}} ===", translator.ReplaceHeading(heading, "スペースシップツー").ToString());

            // 対訳表に登録されている見出しの場合、入れ子は処理されない
            dic["ja"] = "[[宇宙旅行]]{{ref-en}}";
            dic["en"] = "[[弾道飛行]]{{ref-en}}";
            heading.Clear();
            heading.Add(new TextElement(" "));
            heading.Add(new MediaWikiLink("宇宙旅行"));
            heading.Add(new MediaWikiTemplate("ref-en"));
            heading.Add(new TextElement(" "));
            Assert.AreEqual("===[[弾道飛行]]{{ref-en}}===", translator.ReplaceHeading(heading, "スペースシップツー").ToString());
        }

        #endregion

        #region 全体テストケース

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。基本動作。
        /// </summary>
        [Test]
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
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExampleIgnoreHeading Text > " + translate.Text);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example_定型句なし.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.xx", "<!-- " + FormUtils.ApplicationName()),
                translator.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExampleIgnoreHeading Log > " + translate.Log);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example_定型句なし.log")).Replace("file:///xxx/Data/MediaWiki/en/", from.Location),
                translator.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。基本動作見出しの変換含む。
        /// </summary>
        /// <remarks>C++/CLI版の0.73までと同等の動作。</remarks>
        [Test]
        public void TestExample()
        {
            MockFactory mock = new MockFactory();
            MediaWiki from = mock.GetMediaWiki("en");
            MediaWikiTranslator translator = new MediaWikiTranslator();
            translator.From = from;
            translator.To = mock.GetMediaWiki("ja");
            translator.To.LinkInterwikiFormat = null;

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["en"] = "See also";
            dic["ja"] = "関連項目";
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";
            translator.Run("example");

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Text > " + translate.Text);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.73", "<!-- " + FormUtils.ApplicationName()),
                translator.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Log > " + translate.Log);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example.log")).Replace("http://en.wikipedia.org", from.Location),
                translator.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。キャッシュ使用。
        /// </summary>
        [Test]
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
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["en"] = "See also";
            dic["ja"] = "関連項目";
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";

            // 以下のキャッシュパターンを指定して実行
            TranslationDictionary table = new TranslationDictionary("en", "ja");
            table.Add("Template:Wiktionary", new TranslationDictionary.Item { Word = "Template:Wiktionary" });
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
            Assert.IsEmpty(table["Exemplum"].Word);
            Assert.IsNull(table["Exemplum"].Alias);
            Assert.IsNotNull(table["Exemplum"].Timestamp);
            Assert.IsTrue(table.ContainsKey("example.net"));
            Assert.AreEqual("Example.com", table["example.net"].Word);
            Assert.AreEqual("Example.com", table["example.net"].Alias);
            Assert.IsNotNull(table["example.net"].Timestamp);

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExampleWithCache Text > " + translate.Text);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example_キャッシュ使用.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.xx", "<!-- " + FormUtils.ApplicationName()),
                translator.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExampleWithCache Log > " + translate.Log);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example_キャッシュ使用.log")).Replace("file:///xxx/Data/MediaWiki/en/", from.Location),
                translator.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。基本動作見出しの変換、{{仮リンク}}への置き換え含む。
        /// </summary>
        [Test]
        public void TestExampleWithLinkInterwiki()
        {
            MockFactory mock = new MockFactory();
            MediaWiki from = mock.GetMediaWiki("en");
            MediaWikiTranslator translator = new MediaWikiTranslator();
            translator.From = from;
            translator.To = mock.GetMediaWiki("ja");

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["en"] = "See also";
            dic["ja"] = "関連項目";
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "en";
            translator.HeadingTable.To = "ja";
            translator.Run("example");

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Text > " + translate.Text);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example_仮リンク有効.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.73", "<!-- " + FormUtils.ApplicationName()),
                translator.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Log > " + translate.Log);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example.log")).Replace("http://en.wikipedia.org", from.Location),
                translator.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース（日本語版→英語版）。
        /// </summary>
        /// <remarks>C++/CLI版の0.73までと同等の動作。</remarks>
        [Test]
        public void TestSpaceShipTwo()
        {
            MockFactory mock = new MockFactory();
            MediaWiki from = mock.GetMediaWiki("ja");
            MediaWikiTranslator translator = new MediaWikiTranslator();
            translator.From = from;
            translator.To = mock.GetMediaWiki("en");
            translator.To.LinkInterwikiFormat = null;
            translator.ItemTable = new TranslationDictionary("ja", "en");

            // 見出しの変換パターンを設定
            translator.HeadingTable = new TranslationTable();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["en"] = "See Also";
            dic["ja"] = "関連項目";
            translator.HeadingTable.Add(dic);
            dic = new Dictionary<string, string>();
            dic["en"] = "External links";
            dic["ja"] = "外部リンク";
            translator.HeadingTable.Add(dic);
            dic = new Dictionary<string, string>();
            dic["en"] = "Notes";
            dic["ja"] = "脚注";
            translator.HeadingTable.Add(dic);
            translator.HeadingTable.From = "ja";
            translator.HeadingTable.To = "en";
            translator.Run("スペースシップツー");

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Text > " + translate.Text);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "スペースシップツー.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.73", "<!-- " + FormUtils.ApplicationName()),
                translator.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Log > " + translate.Log);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "スペースシップツー.log")).Replace("http://ja.wikipedia.org", from.Location),
                translator.Log);
        }

        /// <summary>
        /// Runを通しで実行するテストケース（対象記事なし）。
        /// </summary>
        [Test]
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
                    + "→ 翻訳元として指定された記事は存在しません。記事名を確認してください。")
                    .Replace("http://en.wikipedia.org", from.Location),
                    translator.Log);
            }
        }

        #endregion
    }
}
