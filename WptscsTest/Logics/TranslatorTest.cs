// ================================================================================================
// <summary>
//      Translatorのテストクラスソース。</summary>
//
// <copyright file="TranslatorTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using NUnit.Framework;
    using Honememo.Tests;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// Translatorのテストクラスです。
    /// </summary>
    [TestFixture]
    public class TranslatorTest
    {
        #region モッククラス

        /// <summary>
        /// Translatorテスト用のモッククラスです。
        /// </summary>
        public class TranslatorMock : Translator
        {
            #region テスト支援用パラメータ

            /// <summary>
            /// <see cref="RunBody"/>の戻り値。
            /// </summary>
            public bool result = false;

            #endregion

            #region 非公開プロパティテスト用のオーラーライドプロパティ

            /// <summary>
            /// ログメッセージ。
            /// </summary>
            public new string Log
            {
                get
                {
                    return base.Log;
                }

                set
                {
                    base.Log = value;
                }
            }

            /// <summary>
            /// 変換後テキスト。
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

            #endregion

            #region 非公開メソッドテスト用のオーラーライドメソッド

            /// <summary>
            /// ログメッセージを1行追加出力。
            /// </summary>
            /// <param name="log">ログメッセージ。</param>
            public new void LogLine(string log)
            {
                base.LogLine(log);
            }

            /// <summary>
            /// ログメッセージを1行追加出力（入力された文字列を書式化して表示）。
            /// </summary>
            /// <param name="format">書式項目を含んだログメッセージ。</param>
            /// <param name="args">書式設定対象オブジェクト配列。</param>
            public new void LogLine(string format, params object[] args)
            {
                base.LogLine(format, args);
            }

            /// <summary>
            /// ログメッセージを出力しつつページを取得。
            /// </summary>
            /// <param name="title">ページタイトル。</param>
            /// <param name="notFoundMsg">取得できない場合に出力するメッセージ。</param>
            /// <returns>取得したページ。ページが存在しない場合は <c>null</c> を返す。</returns>
            /// <remarks>通信エラーなど例外が発生した場合は、別途エラーログを出力する。</remarks>
            public new Page GetPage(string title, string notFoundMsg)
            {
                return base.GetPage(title, notFoundMsg);
            }

            #endregion

            #region ダミーメソッド

            /// <summary>
            /// 翻訳支援処理実行部の本体。
            /// </summary>
            /// <param name="name">記事名。</param>
            /// <returns><c>true</c> 処理成功</returns>
            protected override bool RunBody(string name)
            {
                return result;
            }

            #endregion
        }

        /// <summary>
        /// Translatorテスト用のモッククラスです。
        /// </summary>
        public class TranslatorIgnoreMock : Translator
        {
            #region コンストラクタ

            /// <summary>
            /// デフォルトコンストラクタを隠すためのダミーコンストラクタ。
            /// </summary>
            /// <param name="dummy">ダミー。</param>
            public TranslatorIgnoreMock(string dummy)
            {
            }

            #endregion

            #region ダミーメソッド

            /// <summary>
            /// 翻訳支援処理実行部の本体。
            /// </summary>
            /// <param name="name">記事名。</param>
            /// <returns><c>true</c> 処理成功</returns>
            protected override bool RunBody(string name)
            {
                return false;
            }

            #endregion
        }

        /// <summary>
        /// Websiteテスト用のモッククラスです。
        /// </summary>
        public class WebsiteMock : Website
        {
            #region ダミーメソッド

            /// <summary>
            /// ページを取得。
            /// </summary>
            /// <param name="title">ページタイトル。</param>
            /// <returns>取得したページ。</returns>
            /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
            public override Page GetPage(string title)
            {
                return null;
            }

            #endregion
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// ItemTableプロパティテストケース。
        /// </summary>
        [Test]
        public void TestItemTable()
        {
            // 初期状態がnull、設定すればそのオブジェクトが返されること
            TranslatorMock translator = new TranslatorMock();
            Assert.IsNull(translator.ItemTable);
            TranslationDictionary table = new TranslationDictionary("en", "ja");
            translator.ItemTable = table;
            Assert.AreSame(table, translator.ItemTable);
        }

        /// <summary>
        /// HeadingTableプロパティテストケース。
        /// </summary>
        [Test]
        public void TestHeadingTable()
        {
            // 初期状態がnull、設定すればそのオブジェクトが返されること
            TranslatorMock translator = new TranslatorMock();
            Assert.IsNull(translator.HeadingTable);
            TranslationTable table = new TranslationTable();
            translator.HeadingTable = table;
            Assert.AreSame(table, translator.HeadingTable);
        }

        /// <summary>
        /// Logプロパティテストケース。
        /// </summary>
        [Test]
        public void TestLog()
        {
            // 初期状態は空
            TranslatorMock translator = new TranslatorMock();
            Assert.IsEmpty(translator.Log);

            // null設定時は空白が設定されること、それ以外はそのまま
            translator.Log = null;
            Assert.IsEmpty(translator.Log);
            translator.Log = "test";
            Assert.AreEqual("test", translator.Log);

            // 更新時にLogUpdateイベントが実行されること
            int count = 0;
            translator.LogUpdate += new EventHandler((object sender, EventArgs e) => { ++count; });
            Assert.AreEqual(0, count);
            translator.Log = "ログ";
            Assert.AreEqual(1, count);
            Assert.AreEqual("ログ", translator.Log);
            translator.Log += "add";
            Assert.AreEqual(2, count);
            Assert.AreEqual("ログadd", translator.Log);
        }

        /// <summary>
        /// Textプロパティテストケース。
        /// </summary>
        [Test]
        public void TestText()
        {
            // 初期状態は空
            TranslatorMock translator = new TranslatorMock();
            Assert.IsEmpty(translator.Text);

            // null設定時は空白が設定されること、それ以外はそのまま
            translator.Text = null;
            Assert.IsEmpty(translator.Text);
            translator.Text = "test";
            Assert.AreEqual("test", translator.Text);
        }

        /// <summary>
        /// CancellationPendingプロパティテストケース。
        /// </summary>
        [Test]
        public void TestCancellationPending()
        {
            // 初期状態はfalse、設定すればそのオブジェクトが返されること
            TranslatorMock translator = new TranslatorMock();
            Assert.IsFalse(translator.CancellationPending);
            translator.CancellationPending = true;
            Assert.IsTrue(translator.CancellationPending);
            translator.CancellationPending = false;
            Assert.IsFalse(translator.CancellationPending);
        }

        /// <summary>
        /// Fromプロパティテストケース。
        /// </summary>
        [Test]
        public void TestFrom()
        {
            // 初期状態がnull、設定すればそのオブジェクトが返されること
            TranslatorMock translator = new TranslatorMock();
            Assert.IsNull(translator.From);
            WebsiteMock website = new WebsiteMock();
            translator.From = website;
            Assert.AreSame(website, translator.From);
        }

        /// <summary>
        /// Toプロパティテストケース。
        /// </summary>
        [Test]
        public void TestTo()
        {
            // 初期状態がnull、設定すればそのオブジェクトが返されること
            TranslatorMock translator = new TranslatorMock();
            Assert.IsNull(translator.To);
            WebsiteMock website = new WebsiteMock();
            translator.To = website;
            Assert.AreSame(website, translator.To);
        }

        #endregion

        #region 静的メソッドテストケース

        /// <summary>
        /// Createメソッドテストケース。
        /// </summary>
        [Test]
        public void TestCreate()
        {
            // コンフィグの情報から対応するトランスレータが生成されること
            Translator translator = Translator.Create(new MockFactory().GetConfig(), "en", "ja");
            Assert.IsNotNull(translator);
            Assert.IsInstanceOf(typeof(MediaWikiTranslator), translator);
            Assert.IsNotNull(translator.From);
            Assert.AreEqual("en", translator.From.Language.Code);
            Assert.IsNotNull(translator.To);
            Assert.AreEqual("ja", translator.To.Language.Code);
            Assert.IsNotNull(translator.ItemTable);
            Assert.AreEqual("en", translator.ItemTable.From);
            Assert.AreEqual("ja", translator.ItemTable.To);
            Assert.IsNotNull(translator.HeadingTable);
            Assert.AreEqual("en", translator.HeadingTable.From);
            Assert.AreEqual("ja", translator.HeadingTable.To);
        }

        /// <summary>
        /// Createメソッドテストケース（未対応のトランスレータクラス）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestCreateUnsupportedConstructor()
        {
            // コンフィグに引数無しのコンストラクタを持たないトランスレータクラス
            // が指定されていない場合、例外となること
            Config config = new MockFactory().GetConfig();
            config.Translator = typeof(TranslatorIgnoreMock);
            Translator.Create(config, "en", "ja");
        }

        /// <summary>
        /// Createメソッドテストケース（トランスレータクラス以外の指定）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestCreateIgnoreConstructor()
        {
            // コンフィグに引数無しのコンストラクタを持たないトランスレータクラス
            // が指定されていない場合、例外となること
            Config config = new MockFactory().GetConfig();
            config.Translator = this.GetType();
            Translator.Create(config, "en", "ja");
        }

        #endregion

        #region publicメソッドテストケース

        /// <summary>
        /// Runメソッドテストケース。
        /// </summary>
        [Test]
        public void TestRun()
        {
            TranslatorMock translator = new TranslatorMock();
            translator.From = new WebsiteMock();
            translator.From.Location = "file://";
            translator.To = new WebsiteMock();

            // 正常に実行が行えること
            // また、実行ごとに結果が初期化されること
            Assert.IsFalse(translator.Run("test"));
            Assert.IsEmpty(translator.Log);
            Assert.IsEmpty(translator.Text);
            translator.Log = "testlog";
            translator.Text = "testtext";
            translator.result = true;
            Assert.IsTrue(translator.Run("test"));
            Assert.IsEmpty(translator.Log);
            Assert.IsEmpty(translator.Text);
        }

        /// <summary>
        /// Runメソッドテストケース（必須パラメータ未設定）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRunLangEmpty()
        {
            // From, To が未設定の場合処理不能
            new TranslatorMock().Run("test");
        }

        /// <summary>
        /// Runメソッドテストケース（ping成功）。
        /// </summary>
        [Test]
        public void TestRunPing()
        {
            TranslatorMock translator = new TranslatorMock();
            translator.From = new WebsiteMock();
            translator.To = new WebsiteMock();

            // Fromにホストが指定されている場合、pingチェックが行われる
            translator.From.Location = "http://localhost";
            translator.result = true;
            Assert.IsTrue(translator.Run("test"));
        }

        /// <summary>
        /// Runメソッドテストケース（ping失敗）。
        /// </summary>
        [Test]
        public void TestRunPingFailed()
        {
            TranslatorMock translator = new TranslatorMock();
            translator.From = new WebsiteMock();
            translator.To = new WebsiteMock();

            // Fromにホストが指定されている場合、pingチェックが行われる
            translator.From.Location = "http://xxx.invalid";
            translator.result = true;
            Assert.IsFalse(translator.Run("test"));
        }

        #endregion

        #region protectedメソッドテストケース

        /// <summary>
        /// LogLineメソッドテストケース。
        /// </summary>
        [Test]
        public void TestLogLine()
        {
            TranslatorMock translator = new TranslatorMock();
            
            // 通常は一行が出力される
            Assert.IsEmpty(translator.Log);
            translator.LogLine("1st string");
            Assert.AreEqual("1st string\r\n", translator.Log);
            translator.LogLine("2nd string");
            Assert.AreEqual("1st string\r\n2nd string\r\n", translator.Log);

            // 直前のログが改行されていない場合、改行して出力される
            translator.Log += "3rd ";
            translator.LogLine("string");
            Assert.AreEqual("1st string\r\n2nd string\r\n3rd \r\nstring\r\n", translator.Log);

            // パラメータが二つの方は、String.Formatした値を出力する
            translator.LogLine("{0}th string", 4);
            Assert.AreEqual("1st string\r\n2nd string\r\n3rd \r\nstring\r\n4th string\r\n", translator.Log);
        }

        #endregion
    }
}
