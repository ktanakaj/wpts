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
        /// テスト結果が格納されているフォルダパス。
        /// </summary>
        private static readonly string resultDir = Path.Combine(MockFactory.TestMediaWikiDir, "result");

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
