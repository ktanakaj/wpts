﻿// ================================================================================================
// <summary>
//      Loggerのテストクラスソース。</summary>
//
// <copyright file="LoggerTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Collections.Generic;
    using Honememo.Parsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="Logger"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class LoggerTest
    {
        #region private変数

        /// <summary>
        /// テスト実施中カルチャを変更し後で戻すため、そのバックアップ。
        /// </summary>
        private System.Globalization.CultureInfo backupCulture;

        #endregion

        #region 前処理・後処理

        /// <summary>
        /// テストの前処理。
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            // ロガーの処理結果はカルチャーにより変化するため、ja-JPを明示的に設定する
            this.backupCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("ja-JP");
        }

        /// <summary>
        /// テストの後処理。
        /// </summary>
        [TestCleanup]
        public void TearDown()
        {
            // カルチャーを元に戻す
            System.Threading.Thread.CurrentThread.CurrentUICulture = this.backupCulture;
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Logプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestLog()
        {
            // 初期状態は空
            LoggerMock logger = new LoggerMock();
            Assert.AreEqual(string.Empty, logger.Log);

            // null設定時は空白が設定されること、それ以外はそのまま
            logger.Log = null;
            Assert.AreEqual(string.Empty, logger.Log);
            logger.Log = "test";
            Assert.AreEqual("test", logger.Log);

            // 更新時にLogUpdateイベントが実行されること
            logger.Count = 0;
            Assert.AreEqual(0, logger.Count);
            logger.Log = "ログ";
            Assert.AreEqual(1, logger.Count);
            Assert.AreEqual("ログ", logger.Log);
            logger.Log += "add";
            Assert.AreEqual(2, logger.Count);
            Assert.AreEqual("ログadd", logger.Log);
        }

        #endregion

        #region ログ登録メソッド（一般）テストケース

        /// <summary>
        /// AddMessageメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestAddMessage()
        {
            LoggerMock logger = new LoggerMock();
            
            // 通常は一行が出力される
            Assert.AreEqual(string.Empty, logger.ToString());
            logger.AddMessage("1st string");
            Assert.AreEqual("1st string" + Environment.NewLine, logger.ToString());
            Assert.AreEqual(1, logger.Count);
            logger.AddMessage("2nd string");
            Assert.AreEqual("1st string" + Environment.NewLine + "2nd string" + Environment.NewLine, logger.ToString());
            Assert.AreEqual(2, logger.Count);

            // 直前のログが改行されていない場合、改行して出力される
            logger.Log += "3rd ";
            logger.AddMessage("string");
            Assert.AreEqual("1st string" + Environment.NewLine + "2nd string" + Environment.NewLine + "3rd " + Environment.NewLine + "string" + Environment.NewLine, logger.ToString());

            // パラメータが二つの方は、String.Formatした値を出力する
            logger.AddMessage("{0}th string", 4);
            Assert.AreEqual("1st string" + Environment.NewLine + "2nd string" + Environment.NewLine + "3rd " + Environment.NewLine + "string" + Environment.NewLine + "4th string" + Environment.NewLine, logger.ToString());
        }

        /// <summary>
        /// AddResponseメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestAddResponse()
        {
            LoggerMock logger = new LoggerMock();

            // 通常は一行が出力される
            Assert.AreEqual(string.Empty, logger.ToString());
            logger.AddResponse("1st string");
            Assert.AreEqual("→ 1st string" + Environment.NewLine, logger.ToString());
            Assert.AreEqual(1, logger.Count);
            logger.AddResponse("2nd string");
            Assert.AreEqual("→ 1st string" + Environment.NewLine + "→ 2nd string" + Environment.NewLine, logger.ToString());
            Assert.AreEqual(2, logger.Count);

            // 直前のログが改行されていない場合、改行して出力される
            logger.Log += "3rd ";
            logger.AddResponse("string");
            Assert.AreEqual("→ 1st string" + Environment.NewLine + "→ 2nd string" + Environment.NewLine + "3rd " + Environment.NewLine + "→ string" + Environment.NewLine, logger.ToString());

            // パラメータが二つの方は、String.Formatした値を出力する
            logger.AddResponse("{0}th string", 4);
            Assert.AreEqual("→ 1st string" + Environment.NewLine + "→ 2nd string" + Environment.NewLine + "3rd " + Environment.NewLine + "→ string" + Environment.NewLine + "→ 4th string" + Environment.NewLine, logger.ToString());
        }

        /// <summary>
        /// AddErrorメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestAddError()
        {
            LoggerMock logger = new LoggerMock();

            // 通常は例外内のメッセージだけが出力される
            Assert.AreEqual(string.Empty, logger.ToString());
            logger.AddError(new ArgumentException("this is exception"));
            Assert.AreEqual("→ this is exception" + Environment.NewLine, logger.ToString());
            Assert.AreEqual(1, logger.Count);

            // WebExceptionの場合、レスポンスがあればURLまで出力する
            // ※ WebResponseが簡単にnewできないので現在テスト未実装
            logger.AddError(new System.Net.WebException("通信エラー"));
            Assert.AreEqual("→ this is exception" + Environment.NewLine + "→ 通信エラー" + Environment.NewLine, logger.ToString());
            Assert.AreEqual(2, logger.Count);

            // 直前のログが改行されていない場合、改行して出力される
            logger.Log += "3rd ";
            logger.AddError(new ArgumentException("this is exception2"));
            Assert.AreEqual("→ this is exception" + Environment.NewLine + "→ 通信エラー" + Environment.NewLine + "3rd " + Environment.NewLine + "→ this is exception2" + Environment.NewLine, logger.ToString());
        }

        /// <summary>
        /// AddSeparatorメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestAddSeparator()
        {
            LoggerMock logger = new LoggerMock();

            // 空行が出力される
            Assert.AreEqual(string.Empty, logger.ToString());
            logger.AddSeparator();
            Assert.AreEqual(Environment.NewLine, logger.ToString());
            Assert.AreEqual(1, logger.Count);
            logger.AddSeparator();
            Assert.AreEqual(Environment.NewLine + Environment.NewLine, logger.ToString());
            Assert.AreEqual(2, logger.Count);

            // 直前のログが改行されていない場合、改行して出力される
            logger.Log += "text";
            logger.AddSeparator();
            Assert.AreEqual(Environment.NewLine + Environment.NewLine + "text" + Environment.NewLine + Environment.NewLine, logger.ToString());
        }

        #endregion

        #region ログ登録メソッド（翻訳支援処理）テストケース

        /// <summary>
        /// AddSourceメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestAddSource()
        {
            LoggerMock logger = new LoggerMock();

            // 直前のログが改行されていない場合、改行して出力
            Assert.AreEqual(string.Empty, logger.ToString());
            logger.AddSource(new TextElement("1st string"));
            Assert.AreEqual("1st string → ", logger.ToString());
            Assert.AreEqual(1, logger.Count);
            logger.AddSource(new TextElement("2nd string"));
            Assert.AreEqual("1st string → " + Environment.NewLine + "2nd string → ", logger.ToString());
            Assert.AreEqual(3, logger.Count);
        }

        /// <summary>
        /// AddAliasメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestAddAlias()
        {
            LoggerMock logger = new LoggerMock();

            // 矢印付きの出力のみ
            Assert.AreEqual(string.Empty, logger.ToString());
            logger.AddAlias(new TextElement("1st string"));
            Assert.AreEqual("1st string → ", logger.ToString());
            Assert.AreEqual(1, logger.Count);
            logger.AddAlias(new TextElement("2nd string"));
            Assert.AreEqual("1st string → 2nd string → ", logger.ToString());
            Assert.AreEqual(2, logger.Count);
        }

        /// <summary>
        /// AddDestinationメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestAddDestination()
        {
            LoggerMock logger = new LoggerMock();

            // 変換先の出力と改行、キャッシュの指定があればそのコメント
            Assert.AreEqual(string.Empty, logger.ToString());
            logger.AddDestination(new TextElement("1st string"));
            Assert.AreEqual("1st string" + Environment.NewLine, logger.ToString());
            Assert.AreEqual(1, logger.Count);
            logger.AddDestination(new TextElement("2nd string"), false);
            Assert.AreEqual("1st string" + Environment.NewLine + "2nd string" + Environment.NewLine, logger.ToString());
            Assert.AreEqual(2, logger.Count);
            logger.AddDestination(new TextElement("3rd string"), true);
            Assert.AreEqual("1st string" + Environment.NewLine + "2nd string" + Environment.NewLine + "3rd string ※キャッシュ" + Environment.NewLine, logger.ToString());
            Assert.AreEqual(3, logger.Count);
        }

        #endregion

        #region ログ出力メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            LoggerMock logger = new LoggerMock();

            // ログテキストとして格納している内容がそのまま出力される
            Assert.AreEqual(string.Empty, logger.ToString());
            logger.Log = "テストログ";
            Assert.AreEqual("テストログ", logger.ToString());
        }

        #endregion

        #region 初期化メソッドテストケース

        /// <summary>
        /// Clearメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestClear()
        {
            LoggerMock logger = new LoggerMock();

            // ログテキストを空にする
            logger.Log = "テストログ";
            Assert.AreEqual("テストログ", logger.ToString());
            logger.Clear();
            Assert.AreEqual(string.Empty, logger.ToString());
        }

        #endregion

        #region モッククラス

        /// <summary>
        /// Loggerテスト用のモッククラスです。
        /// </summary>
        public class LoggerMock : Logger
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
    }
}
