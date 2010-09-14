// ================================================================================================
// <summary>
//      MediaWikiTranslatorのテストクラスソース。</summary>
//
// <copyright file="MediaWikiTranslatorTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
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
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;

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
        
        #region テストケース

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。基本動作。
        /// </summary>
        [Test]
        public void TestExampleIgnoreHeading()
        {
            MediaWiki from = this.GetTestServer("en");
            Translator translate = new MediaWikiTranslator(from, this.GetTestServer("ja"));
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
            Translator translate = new MediaWikiTranslator(from, this.GetTestServer("ja"));

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
            Translator translate = new MediaWikiTranslator(from, this.GetTestServer("ja"));

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
            table.Add(".example", new TranslationDictionary.Item { Word = "。さんぷる", Redirect = ".dummy" });
            table.Add("Template:Disambig", new TranslationDictionary.Item { Word = "Template:曖昧さ回避" });
            translate.ItemTable = table;

            Assert.IsTrue(translate.Run("example"));

            // キャッシュに今回の処理で取得した内容が更新されているかを確認
            Assert.IsTrue(table.ContainsKey("example.com"));
            Assert.AreEqual("Example.com", table["example.com"].Word);
            Assert.IsNull(table["example.com"].Redirect);
            Assert.IsNotNull(table["example.com"].Timestamp);
            Assert.IsTrue(table.ContainsKey("Exemplum"));
            Assert.IsEmpty(table["Exemplum"].Word);
            Assert.IsNull(table["Exemplum"].Redirect);
            Assert.IsNotNull(table["Exemplum"].Timestamp);
            Assert.IsTrue(table.ContainsKey("example.net"));
            Assert.AreEqual("Example.com", table["example.net"].Word);
            Assert.AreEqual("Example.com", table["example.net"].Redirect);
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

        #endregion
    }
}
