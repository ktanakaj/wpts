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
            /// <returns>変換後の見出し。</returns>
            public new IElement ReplaceHeading(MediaWikiHeading heading)
            {
                return base.ReplaceHeading(heading);
            }

            #endregion
        }

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
            TestMediaWikiTranslator translate = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translate.From = mock.GetMediaWiki("ja");
            translate.To = mock.GetMediaWiki("en");

            // 見出しの変換パターンを設定
            translate.HeadingTable = new TranslationTable();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["en"] = "External links";
            dic["ja"] = "外部リンク";
            translate.HeadingTable.Add(dic);
            translate.HeadingTable.From = "ja";
            translate.HeadingTable.To = "en";
            MediaWikiLink link;

            // 記事名だけの内部リンクで言語間リンクあり、変換先言語へのリンクとなる
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            Assert.AreEqual("[[Scaled Composites White Knight Two|ホワイトナイトツー]]", translate.ReplaceLink(link, "スペースシップツー").ToString());

            // 見出しあり
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            link.Section = "見出し";
            Assert.AreEqual("[[Scaled Composites White Knight Two#見出し|ホワイトナイトツー]]", translate.ReplaceLink(link, "スペースシップツー").ToString());

            // 変換パターンに該当する見出しの場合
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            link.Section = "外部リンク";
            Assert.AreEqual("[[Scaled Composites White Knight Two#External links|ホワイトナイトツー]]", translate.ReplaceLink(link, "スペースシップツー").ToString());

            // 表示名あり
            link = new MediaWikiLink();
            link.Title = "ホワイトナイトツー";
            link.Section = "外部リンク";
            link.PipeTexts.Add(new TextElement("母機"));
            Assert.AreEqual("[[Scaled Composites White Knight Two#External links|母機]]", translate.ReplaceLink(link, "スペースシップツー").ToString());

            // 記事名だけの内部リンクで言語間リンクなし、変換元言語へのリンクとなる
            translate.From = mock.GetMediaWiki("en");
            translate.To = mock.GetMediaWiki("ja");
            translate.HeadingTable.From = "en";
            translate.HeadingTable.To = "ja";
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            Assert.AreEqual("[[:en:Exemplum|Exemplum]]", translate.ReplaceLink(link, "example").ToString());

            // 見出しあり
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "Three examples of exempla";
            Assert.AreEqual("[[:en:Exemplum#Three examples of exempla|Exemplum]]", translate.ReplaceLink(link, "example").ToString());

            // 変換パターンに該当する見出しの場合
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "External links";
            Assert.AreEqual("[[:en:Exemplum#外部リンク|Exemplum]]", translate.ReplaceLink(link, "example").ToString());

            // 表示名あり
            link = new MediaWikiLink();
            link.Title = "Exemplum";
            link.Section = "External links";
            link.PipeTexts.Add(new TextElement("Exemplum_1"));
            Assert.AreEqual("[[:en:Exemplum#外部リンク|Exemplum_1]]", translate.ReplaceLink(link, "example").ToString());

            // 記事名だけの内部リンクで赤リンク、処理されない
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            Assert.AreEqual("[[Nothing Page]]", translate.ReplaceLink(link, "example").ToString());

            // 見出しあり
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            link.Section = "Section A";
            Assert.AreEqual("[[Nothing Page#Section A]]", translate.ReplaceLink(link, "example").ToString());

            // 変換パターンに該当する見出しの場合
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            link.Section = "External links";
            Assert.AreEqual("[[Nothing Page#外部リンク]]", translate.ReplaceLink(link, "example").ToString());

            // 表示名あり
            link = new MediaWikiLink();
            link.Title = "Nothing Page";
            link.PipeTexts.Add(new TextElement("Dummy Link"));
            Assert.AreEqual("[[Nothing Page|Dummy Link]]", translate.ReplaceLink(link, "example").ToString());

            // [[Apollo&nbsp;17]] のように文字参照が入っていても処理できる
            link = new MediaWikiLink();
            link.Title = "Fuji&nbsp;(Spacecraft)";
            Assert.AreEqual("[[ふじ (宇宙船)]]", translate.ReplaceLink(link, "example").ToString());

        }

        /// <summary>
        /// ReplaceLinkメソッドテストケース（カテゴリ）。
        /// </summary>
        [Test]
        public void TestReplaceLinkCategory()
        {
            TestMediaWikiTranslator translate = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translate.From = mock.GetMediaWiki("ja");
            translate.To = mock.GetMediaWiki("en");
            MediaWikiLink link;

            // 記事名だけの内部リンクで言語間リンクあり、変換先言語でのカテゴリとなる
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            Assert.AreEqual("[[Category:Manned spacecraft]]", translate.ReplaceLink(link, "スペースシップツー").ToString());

            // ソートキーあり
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            link.PipeTexts.Add(new TextElement("すへえすしつふつう"));
            Assert.AreEqual("[[Category:Manned spacecraft|すへえすしつふつう]]", translate.ReplaceLink(link, "スペースシップツー").ToString());

            // 記事名だけの内部リンクで言語間リンクなし、変換元言語へのリンクとなり元のカテゴリはコメントとなる
            translate.To = mock.GetMediaWiki("it");
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            Assert.AreEqual("[[:ja:Category:宇宙船]]<!-- [[Category:宇宙船]] -->", translate.ReplaceLink(link, "スペースシップツー").ToString());

            // ソートキーあり
            link = new MediaWikiLink();
            link.Title = "Category:宇宙船";
            link.PipeTexts.Add(new TextElement("すへえすしつふつう"));
            Assert.AreEqual("[[:ja:Category:宇宙船]]<!-- [[Category:宇宙船|すへえすしつふつう]] -->", translate.ReplaceLink(link, "スペースシップツー").ToString());

            // 記事名だけの内部リンクで赤リンク、処理されない
            link = new MediaWikiLink();
            link.Title = "Category:ｘｘ国の宇宙船";
            Assert.AreEqual("[[Category:ｘｘ国の宇宙船]]", translate.ReplaceLink(link, "スペースシップツー").ToString());

            // ソートキーあり
            link = new MediaWikiLink();
            link.Title = "Category:ｘｘ国の宇宙船";
            link.PipeTexts.Add(new TextElement("すへえすしつふつう"));
            Assert.AreEqual("[[Category:ｘｘ国の宇宙船|すへえすしつふつう]]", translate.ReplaceLink(link, "スペースシップツー").ToString());
        }

        /// <summary>
        /// ReplaceLinkメソッドテストケース（ファイル）。
        /// </summary>
        [Test]
        public void TestReplaceLinkFile()
        {
            TestMediaWikiTranslator translate = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translate.From = mock.GetMediaWiki("en");
            translate.To = mock.GetMediaWiki("ja");
            MediaWikiLink link;

            // 画像などのファイルについては、名前空間を他国語に置き換えるだけ
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            link = new MediaWikiLink();
            link.Title = "File:Kepler22b-artwork.jpg";
            Assert.AreEqual("[[ファイル:Kepler22b-artwork.jpg]]", translate.ReplaceLink(link, "Kepler-22b").ToString());

            // パラメータあり
            link = new MediaWikiLink();
            link.Title = "File:Kepler22b-artwork.jpg";
            link.PipeTexts.Add(new TextElement("thumb"));
            link.PipeTexts.Add(new TextElement("right"));
            link.PipeTexts.Add(new TextElement("Artist's conception of Kepler-22b."));
            Assert.AreEqual("[[ファイル:Kepler22b-artwork.jpg|thumb|right|Artist's conception of Kepler-22b.]]", translate.ReplaceLink(link, "Kepler-22b").ToString());

            // ja→enの場合もちゃんと置き換える（en→jaはFileのままでも使えるが逆は置き換えないと動かない）
            translate.From = mock.GetMediaWiki("ja");
            translate.To = mock.GetMediaWiki("en");

            link = new MediaWikiLink();
            link.Title = "ファイル:Kepler22b-artwork.jpg";
            Assert.AreEqual("[[File:Kepler22b-artwork.jpg]]", translate.ReplaceLink(link, "ケプラー22b").ToString());
        }

        /// <summary>
        /// ReplaceTemplateメソッドテストケース。
        /// </summary>
        [Test]
        public void TestReplaceTemplate()
        {
            TestMediaWikiTranslator translate = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translate.From = mock.GetMediaWiki("en");
            translate.To = mock.GetMediaWiki("ja");
            MediaWikiTemplate template;

            // テンプレート名だけで言語間リンクあり、変換先言語でのテンプレートとなる
            // ※ 以下オブジェクトを毎回作り直しているのは、更新されてしまうケースがあるため
            template = new MediaWikiTemplate("Citation needed");
            Assert.AreEqual("{{要出典}}", translate.ReplaceTemplate(template, "example").ToString());

            // パラメータあり
            template = new MediaWikiTemplate("Citation needed");
            template.PipeTexts.Add(new TextElement("date=January 2012"));
            Assert.AreEqual("{{要出典|date=January 2012}}", translate.ReplaceTemplate(template, "example").ToString());

            // テンプレート名だけで言語間リンクなし、変換元言語へのリンクとなり元のテンプレートはコメントとなる
            template = new MediaWikiTemplate("Wiktionary");
            Assert.AreEqual("[[:en:Template:Wiktionary]]<!-- {{Wiktionary}} -->", translate.ReplaceTemplate(template, "example").ToString());

            // パラメータあり
            template = new MediaWikiTemplate("Wiktionary");
            template.PipeTexts.Add(new TextElement("Sample"));
            Assert.AreEqual("[[:en:Template:Wiktionary]]<!-- {{Wiktionary|Sample}} -->", translate.ReplaceTemplate(template, "example").ToString());

            // テンプレート名だけで赤リンク、処理されない
            template = new MediaWikiTemplate("Invalid Template");
            Assert.AreEqual("{{Invalid Template}}", translate.ReplaceTemplate(template, "example").ToString());

            // パラメータあり
            template = new MediaWikiTemplate("Invalid Template");
            template.PipeTexts.Add(new TextElement("parameter=1"));
            Assert.AreEqual("{{Invalid Template|parameter=1}}", translate.ReplaceTemplate(template, "example").ToString());

            // システム定義変数、処理されない
            template = new MediaWikiTemplate("PAGENAME");
            Assert.AreEqual("{{PAGENAME}}", translate.ReplaceTemplate(template, "example").ToString());
        }

        /// <summary>
        /// ReplaceTemplateメソッドテストケース（入れ子）。
        /// </summary>
        [Test]
        public void TestReplaceTemplateNested()
        {
            TestMediaWikiTranslator translate = new TestMediaWikiTranslator();
            MockFactory mock = new MockFactory();
            translate.From = mock.GetMediaWiki("en");
            translate.To = mock.GetMediaWiki("ja");
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
            Assert.AreEqual("{{要出典|date=January 2012|note=See also [[ふじ (宇宙船)|Fuji (Spacecraft)]]}}", translate.ReplaceTemplate(template, "example").ToString());

            // テンプレート名だけで言語間リンクなし、入れ子は処理されない
            template = new MediaWikiTemplate("Wiktionary");
            template.PipeTexts.Add(new TextElement("Sample"));
            list = new ListElement();
            list.Add(new TextElement("note=See also "));
            link = new MediaWikiLink();
            link.Title = "Fuji (Spacecraft)";
            list.Add(link);
            template.PipeTexts.Add(list);
            Assert.AreEqual("[[:en:Template:Wiktionary]]<!-- {{Wiktionary|Sample|note=See also [[Fuji (Spacecraft)]]}} -->", translate.ReplaceTemplate(template, "example").ToString());

            // テンプレート名だけで赤リンク、入れ子は処理されない
            template = new MediaWikiTemplate("Invalid Template");
            template.PipeTexts.Add(new TextElement("parameter=1"));
            list = new ListElement();
            list.Add(new TextElement("note=See also "));
            link = new MediaWikiLink();
            link.Title = "Fuji (Spacecraft)";
            list.Add(link);
            template.PipeTexts.Add(list);
            Assert.AreEqual("{{Invalid Template|parameter=1|note=See also [[Fuji (Spacecraft)]]}}", translate.ReplaceTemplate(template, "example").ToString());
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
            Translator translate = new MediaWikiTranslator();
            translate.From = from;
            translate.To = mock.GetMediaWiki("ja");
            translate.HeadingTable = new TranslationTable();
            translate.HeadingTable.From = "en";
            translate.HeadingTable.To = "ja";

            Assert.IsTrue(translate.Run("example"));

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExampleIgnoreHeading Text > " + translate.Text);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example_定型句なし.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.xx", "<!-- " + FormUtils.ApplicationName()),
                translate.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExampleIgnoreHeading Log > " + translate.Log);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example_定型句なし.log")).Replace("file:///xxx/Data/MediaWiki/en/", from.Location),
                translate.Log);
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
            Translator translate = new MediaWikiTranslator();
            translate.From = from;
            translate.To = mock.GetMediaWiki("ja");

            // 見出しの変換パターンを設定
            translate.HeadingTable = new TranslationTable();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["en"] = "See also";
            dic["ja"] = "関連項目";
            translate.HeadingTable.Add(dic);
            translate.HeadingTable.From = "en";
            translate.HeadingTable.To = "ja";

            Assert.IsTrue(translate.Run("example"));

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Text > " + translate.Text);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.73", "<!-- " + FormUtils.ApplicationName()),
                translate.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Log > " + translate.Log);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example.log")).Replace("http://en.wikipedia.org", from.Location),
                translate.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。キャッシュ使用。
        /// </summary>
        [Test]
        public void TestExampleWithCache()
        {
            MockFactory mock = new MockFactory();
            MediaWiki from = mock.GetMediaWiki("en");
            Translator translate = new MediaWikiTranslator();
            translate.From = from;
            translate.To = mock.GetMediaWiki("ja");

            // 見出しの変換パターンを設定
            translate.HeadingTable = new TranslationTable();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["en"] = "See also";
            dic["ja"] = "関連項目";
            translate.HeadingTable.Add(dic);
            translate.HeadingTable.From = "en";
            translate.HeadingTable.To = "ja";

            // 以下のキャッシュパターンを指定して実行
            TranslationDictionary table = new TranslationDictionary("en", "ja");
            table.Add("Template:Wiktionary", new TranslationDictionary.Item { Word = "Template:Wiktionary" });
            table.Add("example.org", new TranslationDictionary.Item());
            table.Add(".example", new TranslationDictionary.Item { Word = "。さんぷる", Alias = ".dummy" });
            table.Add("Template:Disambig", new TranslationDictionary.Item { Word = "Template:曖昧さ回避" });
            translate.ItemTable = table;

            Assert.IsTrue(translate.Run("example"));

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
                translate.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExampleWithCache Log > " + translate.Log);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "example_キャッシュ使用.log")).Replace("file:///xxx/Data/MediaWiki/en/", from.Location),
                translate.Log);
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
            Translator translate = new MediaWikiTranslator();
            translate.From = from;
            translate.To = mock.GetMediaWiki("en");
            translate.ItemTable = new TranslationDictionary("ja", "en");

            // 見出しの変換パターンを設定
            translate.HeadingTable = new TranslationTable();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["en"] = "See Also";
            dic["ja"] = "関連項目";
            translate.HeadingTable.Add(dic);
            dic = new Dictionary<string, string>();
            dic["en"] = "External links";
            dic["ja"] = "外部リンク";
            translate.HeadingTable.Add(dic);
            dic = new Dictionary<string, string>();
            dic["en"] = "Notes";
            dic["ja"] = "脚注";
            translate.HeadingTable.Add(dic);
            translate.HeadingTable.From = "ja";
            translate.HeadingTable.To = "en";

            Assert.IsTrue(translate.Run("スペースシップツー"));

            // テストデータの変換結果を期待される結果と比較する
            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Text > " + translate.Text);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "スペースシップツー.txt")).Replace("<!-- Wikipedia 翻訳支援ツール Ver0.73", "<!-- " + FormUtils.ApplicationName()),
                translate.Text);

            // テストデータの変換ログを期待されるログと比較する
            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Log > " + translate.Log);
            Assert.AreEqual(
                File.ReadAllText(Path.Combine(resultDir, "スペースシップツー.log")).Replace("http://ja.wikipedia.org", from.Location),
                translate.Log);
        }

        /// <summary>
        /// Runを通しで実行するテストケース（対象記事なし）。
        /// </summary>
        [Test]
        public void TestPageNothing()
        {
            MockFactory mock = new MockFactory();
            MediaWiki from = mock.GetMediaWiki("en");
            Translator translate = new MediaWikiTranslator();
            translate.From = from;
            translate.To = mock.GetMediaWiki("ja");

            Assert.IsFalse(translate.Run("Nothing Page"));

            // 実行ログを期待されるログと比較する
            Assert.AreEqual(
                ("http://en.wikipedia.org より [[Nothing Page]] を取得。\r\n"
                + "→ 翻訳元として指定された記事は存在しません。記事名を確認してください。")
                .Replace("http://en.wikipedia.org", from.Location),
                translate.Log);
        }

        #endregion
    }
}
