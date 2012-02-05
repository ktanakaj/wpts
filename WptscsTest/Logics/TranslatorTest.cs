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
    using NUnit.Framework;
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
            /// <see cref="RunBody"/>で例外を投げるか？
            /// </summary>
            public bool exception = false;

            #endregion

            #region 非公開プロパティテスト用のオーラーライドプロパティ

            /// <summary>
            /// ログテキスト生成用ロガー。
            /// </summary>
            public new Logger Logger
            {
                get
                {
                    return base.Logger;
                }

                set
                {
                    base.Logger = value;
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
            protected override void RunBody(string name)
            {
                if (exception)
                {
                    throw new ApplicationException("Dummy");
                }
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
            protected override void RunBody(string name)
            {
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

            // 更新時にLogUpdateイベントが実行されること
            int count = 0;
            translator.LogUpdate += new EventHandler((object sender, EventArgs e) => { ++count; });
            Assert.AreEqual(0, count);
            translator.Logger.AddMessage("ログ");
            Assert.AreEqual(1, count);
            Assert.AreEqual("ログ" + Environment.NewLine, translator.Log);
            translator.Logger.AddMessage("add");
            Assert.AreEqual(2, count);
            Assert.AreEqual("ログ" + Environment.NewLine + "add" + Environment.NewLine, translator.Log);
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
            translator.Run("test");
            Assert.IsEmpty(translator.Log);
            Assert.IsEmpty(translator.Text);
            translator.Logger.AddMessage("testlog");
            translator.Text = "testtext";
            translator.Run("test");
            Assert.IsEmpty(translator.Log);
            Assert.IsEmpty(translator.Text);

            // 失敗はApplicationExceptionで表現、RunBodyから例外が投げられること
            translator.Logger.AddMessage("testlog");
            translator.Text = "testtext";
            translator.exception = true;
            try
            {
                translator.Run("test");
                Assert.Fail();
            }
            catch (ApplicationException e)
            {
                Assert.AreEqual("Dummy", e.Message);
            }

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
            translator.Run("test");
        }

        /// <summary>
        /// Runメソッドテストケース（ping失敗）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void TestRunPingFailed()
        {
            TranslatorMock translator = new TranslatorMock();
            translator.From = new WebsiteMock();
            translator.To = new WebsiteMock();

            // Fromにホストが指定されている場合、pingチェックが行われる
            translator.From.Location = "http://xxx.invalid";
            translator.Run("test");
        }

        #endregion
    }
}
