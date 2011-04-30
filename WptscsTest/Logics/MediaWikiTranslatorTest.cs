// ================================================================================================
// <summary>
//      MediaWikiTranslatorのテストクラスソース。</summary>
//
// <copyright file="MediaWikiTranslatorTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
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
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Utilities;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// MediaWikiTranslatorのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiTranslatorTest
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
            MediaWiki server = ObjectUtils.DefaultIfNull<MediaWiki>(
                TestingConfig.GetInstance("Data\\config.xml").GetWebsite(language) as MediaWiki,
                new MediaWiki(new Language(language)));
            UriBuilder b = new UriBuilder("file", "");
            b.Path = Path.GetFullPath(testDir) + "\\";
            server.Location = new Uri(b.Uri, language + "/").ToString();
            server.ExportPath = "{0}.xml";
            server.NamespacePath = "_api.xml";
            return server;
        }

        #endregion
        
        #region 全体テストケース

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。基本動作。
        /// </summary>
        [Test]
        public void TestExampleIgnoreHeading()
        {
            MediaWiki from = this.GetTestServer("en");
            Translator translate = new MediaWikiTranslator();
            translate.From = from;
            translate.To = this.GetTestServer("ja");
            translate.HeadingTable = new TranslationTable();
            translate.HeadingTable.From = "en";
            translate.HeadingTable.To = "ja";

            Assert.IsTrue(translate.Run("example"));

            // テストデータの変換結果を期待される結果と比較する
            string expectedText;
            using (StreamReader sr = new StreamReader(Path.Combine(testDir, "result\\example_定型句なし.txt"))) 
            {
                expectedText = sr.ReadToEnd();
            }

            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExampleIgnoreHeading Text > " + translate.Text);
            Assert.AreEqual(
                expectedText.Replace("<!-- Wikipedia 翻訳支援ツール Ver0.xx", "<!-- " + FormUtils.ApplicationName()),
                translate.Text);

            // テストデータの変換ログを期待されるログと比較する
            string expectedLog;
            using (StreamReader sr = new StreamReader(Path.Combine(testDir, "result\\example_定型句なし.log")))
            {
                expectedLog = sr.ReadToEnd();
            }

            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExampleIgnoreHeading Log > " + translate.Log);
            Assert.AreEqual(
                expectedLog.Replace("file:///xxx/Data/MediaWiki/en/", from.Location),
                translate.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。基本動作見出しの変換含む。
        /// </summary>
        /// <remarks>C++/CLI版の0.73までと同等の動作。</remarks>
        [Test]
        public void TestExample()
        {
            MediaWiki from = this.GetTestServer("en");
            Translator translate = new MediaWikiTranslator();
            translate.From = from;
            translate.To = this.GetTestServer("ja");

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
            string expectedText;
            using (StreamReader sr = new StreamReader(Path.Combine(testDir, "result\\example.txt")))
            {
                expectedText = sr.ReadToEnd();
            }

            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Text > " + translate.Text);
            Assert.AreEqual(
                expectedText.Replace("<!-- Wikipedia 翻訳支援ツール Ver0.73", "<!-- " + FormUtils.ApplicationName()),
                translate.Text);

            // テストデータの変換ログを期待されるログと比較する
            string expectedLog;
            using (StreamReader sr = new StreamReader(Path.Combine(testDir, "result\\example.log")))
            {
                expectedLog = sr.ReadToEnd();
            }

            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Log > " + translate.Log);
            Assert.AreEqual(
                expectedLog.Replace("http://en.wikipedia.org", from.Location),
                translate.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。キャッシュ使用。
        /// </summary>
        [Test]
        public void TestExampleWithCache()
        {
            MediaWiki from = this.GetTestServer("en");
            Translator translate = new MediaWikiTranslator();
            translate.From = from;
            translate.To = this.GetTestServer("ja");

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
            string expectedText;
            using (StreamReader sr = new StreamReader(Path.Combine(testDir, "result\\example_キャッシュ使用.txt")))
            {
                expectedText = sr.ReadToEnd();
            }

            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExampleWithCache Text > " + translate.Text);
            Assert.AreEqual(
                expectedText.Replace("<!-- Wikipedia 翻訳支援ツール Ver0.xx", "<!-- " + FormUtils.ApplicationName()),
                translate.Text);

            // テストデータの変換ログを期待されるログと比較する
            string expectedLog;
            using (StreamReader sr = new StreamReader(Path.Combine(testDir, "result\\example_キャッシュ使用.log")))
            {
                expectedLog = sr.ReadToEnd();
            }

            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExampleWithCache Log > " + translate.Log);
            Assert.AreEqual(
                expectedLog.Replace("file:///xxx/Data/MediaWiki/en/", from.Location),
                translate.Log);
        }

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース（日本語版→英語版）。
        /// </summary>
        /// <remarks>C++/CLI版の0.73までと同等の動作。</remarks>
        [Test]
        public void TestSpaceShipTwo()
        {
            MediaWiki from = this.GetTestServer("ja");
            Translator translate = new MediaWikiTranslator();
            translate.From = from;
            translate.To = this.GetTestServer("en");
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
            string expectedText;
            using (StreamReader sr = new StreamReader(Path.Combine(testDir, "result\\スペースシップツー.txt")))
            {
                expectedText = sr.ReadToEnd();
            }

            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Text > " + translate.Text);
            Assert.AreEqual(
                expectedText.Replace("<!-- Wikipedia 翻訳支援ツール Ver0.73", "<!-- " + FormUtils.ApplicationName()),
                translate.Text);

            // テストデータの変換ログを期待されるログと比較する
            string expectedLog;
            using (StreamReader sr = new StreamReader(Path.Combine(testDir, "result\\スペースシップツー.log")))
            {
                expectedLog = sr.ReadToEnd();
            }

            // 1行目のパスが一致しないので、期待される結果のうち該当部分を更新する
            //System.Diagnostics.Debug.WriteLine("TranslateMediaWikiTest.TestExample Log > " + translate.Log);
            Assert.AreEqual(
                expectedLog.Replace("http://ja.wikipedia.org", from.Location),
                translate.Log);
        }

        /// <summary>
        /// Runを通しで実行するテストケース（対象記事なし）。
        /// </summary>
        [Test]
        public void TestPageNothing()
        {
            MediaWiki from = this.GetTestServer("en");
            Translator translate = new MediaWikiTranslator();
            translate.From = from;
            translate.To = this.GetTestServer("ja");

            Assert.IsFalse(translate.Run("Nothing Page"));

            // 実行ログを期待されるログと比較する
            Assert.AreEqual(
                ("http://en.wikipedia.org より [[Nothing Page]] を取得。\r\n"
                + "→ 翻訳元として指定された記事は存在しません。記事名を確認してください。")
                .Replace("http://en.wikipedia.org", from.Location),
                translate.Log);
        }

        #endregion

        #region 整理予定の静的メソッドテストケース

        /// <summary>
        /// ChkCommentメソッドテストケース。
        /// </summary>
        [Test]
        public void TestChkComment()
        {
            // TryParseComment互換用の旧メソッド
            string comment;
            Assert.AreEqual(14, MediaWikiTranslator.ChkComment(out comment, "ab<!-- test -->cd", 2));
            Assert.AreEqual("<!-- test -->", comment);
            Assert.AreEqual(15, MediaWikiTranslator.ChkComment(out comment, "ab<!-- test --cd", 2));
            Assert.AreEqual("<!-- test --cd", comment);
            Assert.AreEqual(-1, MediaWikiTranslator.ChkComment(out comment, "ab<!-- test -->cd", 1));
            Assert.IsEmpty(comment);
            Assert.AreEqual(-1, MediaWikiTranslator.ChkComment(out comment, "ab<!-- test -->cd", 3));
            Assert.IsEmpty(comment);
        }
        
        /// <summary>
        /// ChkNowikiメソッドテストケース。
        /// </summary>
        [Test]
        public void TestChkNowiki()
        {
            // TryParseNowiki互換用の旧メソッド
            string nowiki;
            Assert.AreEqual(26, MediaWikiTranslator.ChkNowiki(out nowiki, "ab<nowiki>[[test]]</nowiki>cd", 2));
            Assert.AreEqual("<nowiki>[[test]]</nowiki>", nowiki);
            Assert.AreEqual(27, MediaWikiTranslator.ChkNowiki(out nowiki, "ab<nowiki>[[test]]</nowikicd", 2));
            Assert.AreEqual("<nowiki>[[test]]</nowikicd", nowiki);
            Assert.AreEqual(-1, MediaWikiTranslator.ChkNowiki(out nowiki, "ab<nowiki>[[test]]</nowiki>cd", 1));
            Assert.IsEmpty(nowiki);
            Assert.AreEqual(-1, MediaWikiTranslator.ChkNowiki(out nowiki, "ab<nowiki>[[test]]</nowiki>cd", 3));
            Assert.IsEmpty(nowiki);
        }

        #endregion
    }
}
