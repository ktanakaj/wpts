// ================================================================================================
// <summary>
//      TranslateMediaWikiのテストクラスソース。</summary>
//
// <copyright file="TranslateMediaWikiTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;

    /// <summary>
    /// TranslateMediaWikiのテストクラスです。
    /// </summary>
    [TestFixture]
    public class TranslateMediaWikiTest
    {
        #region 定数

        /// <summary>
        /// テストデータが格納されているフォルダパス。
        /// </summary>
        private static readonly string testUri = "file:///../Data/MediaWiki/";

        #endregion

        #region テストケース

        /// <summary>
        /// テストデータを用い、Runを通しで実行するテストケース。
        /// </summary>
        [Test]
        public void TestBasic()
        {
            MediaWiki from = new MediaWiki("en", testUri + "en");
            from.ExportPath = "/{0}.xml";
            from.NamespacePath = "/_api.xml";
            MediaWiki to = new MediaWiki("ja", testUri + "ja");
            to.ExportPath = "/{0}.xml";
            Translate translate = new TranslateMediaWiki(from, to);
            Assert.IsTrue(translate.Run("example"));

            // テストデータの変換結果を期待される結果と比較する
            string expectedText;
            using (StreamReader sr = new StreamReader("example_定型句なし.txt")) 
            {
                expectedText = sr.ReadToEnd();
            }

            // バージョン表記部分は毎回変化するため、期待される結果のうち該当部分を更新する
            expectedText.Replace("<!-- Wikipedia 翻訳支援ツール Ver0.xx", "<!-- " + FormUtils.ApplicationName());
            Assert.AreEqual(expectedText, translate.Text);

            // テストデータの変換ログを期待されるログと比較する
            string expectedLog;
            using (StreamReader sr = new StreamReader("example_定型句なし.txt"))
            {
                expectedLog = sr.ReadToEnd();
            }

            Assert.AreEqual(expectedLog, translate.Log);
        }

        #endregion
    }
}
