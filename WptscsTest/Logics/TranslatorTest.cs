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
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Websites;
    using NUnit.Framework;

    /// <summary>
    /// <see cref="Translator"/>のテストクラスです。
    /// </summary>
    [TestFixture]
    internal class TranslatorTest
    {
        #region プロパティテストケース

        /// <summary>
        /// <see cref="Translator.ItemTable"/>プロパティテストケース。
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
        /// <see cref="Translator.HeadingTable"/>プロパティテストケース。
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
        /// <see cref="Translator.Log"/>プロパティテストケース。
        /// </summary>
        [Test]
        public void TestLog()
        {
            // 初期状態は空
            TranslatorMock translator = new TranslatorMock();
            Assert.IsEmpty(translator.Log);

            // 更新時にLogUpdateイベントが実行されること
            int count = 0;
            translator.LogUpdated += new EventHandler((object sender, EventArgs e) => { ++count; });
            Assert.AreEqual(0, count);
            translator.Logger.AddMessage("ログ");
            Assert.AreEqual(1, count);
            Assert.AreEqual("ログ" + Environment.NewLine, translator.Log);
            translator.Logger.AddMessage("add");
            Assert.AreEqual(2, count);
            Assert.AreEqual("ログ" + Environment.NewLine + "add" + Environment.NewLine, translator.Log);
        }

        /// <summary>
        /// <see cref="Translator.Text"/>プロパティテストケース。
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
        /// <see cref="Translator.CancellationPending"/>プロパティテストケース。
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
        /// <see cref="Translator.From"/>プロパティテストケース。
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
        /// <see cref="Translator.To"/>プロパティテストケース。
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
        /// <see cref="Translator.Create"/>メソッドテストケース。
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
        /// <see cref="Translator.Create"/>メソッドテストケース（未対応のトランスレータクラス）。
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
        /// <see cref="Translator.Create"/>メソッドテストケース（トランスレータクラス以外の指定）。
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

        #region 公開メソッドテストケース

        /// <summary>
        /// <see cref="Translator.Run"/>メソッドテストケース。
        /// </summary>
        [Test]
        public void TestRun()
        {
            // ※ Runの処理ではpingも行っているが、そのテストについては2012年2月現在、
            //    App.configのデフォルト値がpingを行わない設定なっているため行えない
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
            translator.Exception = true;
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
        /// <see cref="Translator.Run"/>メソッドテストケース（必須パラメータ未設定）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRunLangEmpty()
        {
            // From, To が未設定の場合処理不能
            new TranslatorMock().Run("test");
        }

        #endregion

        #region モッククラス

        /// <summary>
        /// <see cref="Translator"/>テスト用のモッククラスです。
        /// </summary>
        private class TranslatorMock : Translator
        {
            #region テスト支援用プロパティ

            /// <summary>
            /// <see cref="RunBody"/>で例外を投げるか？
            /// </summary>
            public bool Exception
            {
                get;
                set;
            }

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

            #region ダミーメソッド

            /// <summary>
            /// 翻訳支援処理実行部の本体。
            /// </summary>
            /// <param name="name">記事名。</param>
            protected override void RunBody(string name)
            {
                if (this.Exception)
                {
                    throw new ApplicationException("Dummy");
                }
            }

            #endregion
        }

        /// <summary>
        /// <see cref="Translator"/>テスト用のモッククラスです。
        /// </summary>
        private class TranslatorIgnoreMock : Translator
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
            protected override void RunBody(string name)
            {
            }

            #endregion
        }

        /// <summary>
        /// <see cref="Translator"/>テスト用のモッククラスです。
        /// </summary>
        private class WebsiteMock : Website
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
    }
}
