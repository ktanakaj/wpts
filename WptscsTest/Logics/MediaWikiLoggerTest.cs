// ================================================================================================
// <summary>
//      MediaWikiLoggerのテストクラスソース。</summary>
//
// <copyright file="MediaWikiLoggerTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Honememo.Parsers;
    using Honememo.Wptscs.Parsers;

    /// <summary>
    /// MediaWikiLoggerのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiLoggerTest
    {
        #region モッククラス

        /// <summary>
        /// Loggerテスト用のモッククラスです。
        /// </summary>
        public class LoggerMock : MediaWikiLogger
        {
            #region コンストラクタ

            /// <summary>
            /// テスト用コンストラクタ。
            /// </summary>
            public LoggerMock()
            {
                this.LogUpdate += new EventHandler((object sender, EventArgs e) => { ++Count; });
            }

            #endregion

            #region テスト用プロパティ

            /// <summary>
            /// LogUpdateイベントが呼ばれた回数のカウンタ。
            /// </summary>
            public int Count
            {
                get;
                set;
            }

            #endregion

            #region 非公開プロパティテスト用のオーラーライドプロパティ

            /// <summary>
            /// ログテキスト。
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

            #endregion
        }

        #endregion

        #region private変数

        /// <summary>
        /// テスト実施中カルチャを変更し後で戻すため、そのバックアップ。
        /// </summary>
        System.Globalization.CultureInfo backupCulture;

        #endregion

        #region 前処理・後処理

        /// <summary>
        /// テストの前処理。
        /// </summary>
        [TestFixtureSetUp]
        public void SetUp()
        {
            // ロガーの処理結果はカルチャーにより変化するため、ja-JPを明示的に設定する
            this.backupCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("ja-JP");
        }

        /// <summary>
        /// テストの後処理。
        /// </summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
            // カルチャーを元に戻す
            System.Threading.Thread.CurrentThread.CurrentUICulture = this.backupCulture;
        }

        #endregion

        #region ログ登録メソッド（翻訳支援処理）テストケース

        /// <summary>
        /// AddSourceメソッドテストケース。
        /// </summary>
        [Test]
        public void TestAddSource()
        {
            LoggerMock logger = new LoggerMock();
            Logger diff = new Logger();
            
            // 通常の要素の場合、普通のロガーと同様に処理される
            Assert.IsEmpty(logger.ToString());
            logger.AddSource(new TextElement("1st string"));
            diff.AddSource(new TextElement("1st string"));
            Assert.AreEqual(diff.ToString(), logger.ToString());
            Assert.AreEqual(1, logger.Count);
            logger.AddSource(new TextElement("2nd string"));
            diff.AddSource(new TextElement("2nd string"));
            Assert.AreEqual(diff.ToString(), logger.ToString());
            Assert.AreEqual(3, logger.Count);

            // MediaWiki関連の一部要素は独自に整形して出力
            logger.Clear();
            logger.AddSource(new MediaWikiLink("記事名") { Section = "セクション", Interwiki = "ja" });
            Assert.AreEqual("[[記事名]] → ", logger.ToString());
            logger.Clear();
            logger.AddSource(new MediaWikiTemplate("記事名") { Section = "セクション", Interwiki = "ja" });
            Assert.AreEqual("{{記事名}} → ", logger.ToString());

            // 見出しの場合、右矢印が出ない＆直前に空行が入る
            logger.Clear();
            logger.AddSource(new MediaWikiHeading {ParsedString = "==見出し==" });
            Assert.AreEqual(Environment.NewLine + "==見出し==", logger.ToString());

            // いずれのケースでも、親クラスにある改行されていなければ改行は行われる
            logger.Clear();
            logger.Log = "test";
            logger.AddSource(new MediaWikiLink("記事名") { Section = "セクション", Interwiki = "ja" });
            Assert.AreEqual("test" + Environment.NewLine + "[[記事名]] → ", logger.ToString());
            logger.AddSource(new MediaWikiHeading { ParsedString = "==見出し==" });
            Assert.AreEqual("test" + Environment.NewLine + "[[記事名]] → " + Environment.NewLine + Environment.NewLine + "==見出し==", logger.ToString());
        }

        /// <summary>
        /// AddAliasメソッドテストケース。
        /// </summary>
        [Test]
        public void TestAddAlias()
        {
            LoggerMock logger = new LoggerMock();

            // リダイレクトとして出力
            Assert.IsEmpty(logger.ToString());
            logger.AddAlias(new TextElement("1st string"));
            Assert.AreEqual("リダイレクト 1st string → ", logger.ToString());
            Assert.AreEqual(1, logger.Count);
            logger.AddAlias(new TextElement("2nd string"));
            Assert.AreEqual("リダイレクト 1st string → リダイレクト 2nd string → ", logger.ToString());
            Assert.AreEqual(2, logger.Count);

            // MediaWiki関連の一部要素は独自に整形して出力
            logger.Clear();
            logger.AddAlias(new MediaWikiLink("記事名") { Section = "セクション", Interwiki = "ja" });
            Assert.AreEqual("リダイレクト [[記事名]] → ", logger.ToString());
            logger.Clear();
            logger.AddAlias(new MediaWikiTemplate("記事名") { Section = "セクション", Interwiki = "ja" });
            Assert.AreEqual("リダイレクト {{記事名}} → ", logger.ToString());

            // 直前のログが見出しの場合矢印を出力、その後リダイレクトを出力
            // ※ 見出しでリダイレクトというのはありえないが、一応AddDestinationとあわせて処理は入れている
            logger.Clear();
            logger.AddSource(new MediaWikiHeading { ParsedString = "==見出し1==" });
            logger.AddAlias(new MediaWikiHeading { ParsedString = "==見出し2==" });
            Assert.AreEqual(Environment.NewLine + "==見出し1== → リダイレクト ==見出し2== → ", logger.ToString());
        }

        /// <summary>
        /// AddDestinationメソッドテストケース。
        /// </summary>
        [Test]
        public void TestAddDestination()
        {
            LoggerMock logger = new LoggerMock();
            Logger diff = new Logger();

            // 通常の要素の場合、普通のロガーと同様に処理される
            Assert.IsEmpty(logger.ToString());
            logger.AddDestination(new TextElement("1st string"));
            diff.AddDestination(new TextElement("1st string"));
            Assert.AreEqual(diff.ToString(), logger.ToString());
            Assert.AreEqual(1, logger.Count);
            logger.AddDestination(new TextElement("2nd string"), false);
            diff.AddDestination(new TextElement("2nd string"), false);
            Assert.AreEqual(diff.ToString(), logger.ToString());
            Assert.AreEqual(2, logger.Count);
            logger.AddDestination(new TextElement("3rd string"), true);
            diff.AddDestination(new TextElement("3rd string"), true);
            Assert.AreEqual(diff.ToString(), logger.ToString());
            Assert.AreEqual(3, logger.Count);

            // MediaWiki関連の一部要素は独自に整形して出力
            logger.Clear();
            logger.AddDestination(new MediaWikiLink("記事名") { Section = "セクション", Interwiki = "ja" });
            Assert.AreEqual("[[記事名]]" + Environment.NewLine, logger.ToString());
            logger.Clear();
            logger.AddDestination(new MediaWikiTemplate("記事名") { Section = "セクション", Interwiki = "ja" }, true);
            Assert.AreEqual("{{記事名}} ※キャッシュ" + Environment.NewLine, logger.ToString());

            // 直前のログが見出しの場合矢印を出力、その後独自に整形した変換先を出力
            logger.Clear();
            logger.AddSource(new MediaWikiHeading { ParsedString = "==見出し1==" });
            logger.AddDestination(new MediaWikiHeading { ParsedString = "==見出し2==" });
            Assert.AreEqual(Environment.NewLine + "==見出し1== → ==見出し2==" + Environment.NewLine, logger.ToString());
        }

        #endregion
    }
}
