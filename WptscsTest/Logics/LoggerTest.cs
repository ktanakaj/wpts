// ================================================================================================
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
    using NUnit.Framework;
    using Honememo.Parsers;

    /// <summary>
    /// Loggerのテストクラスです。
    /// </summary>
    [TestFixture]
    public class LoggerTest
    {
        #region モッククラス

        /// <summary>
        /// Loggerテスト用のモッククラスです。
        /// </summary>
        public class LoggerMock : Logger
        {
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

        #region プロパティテストケース

        /// <summary>
        /// Logプロパティテストケース。
        /// </summary>
        [Test]
        public void TestLog()
        {
            // 初期状態は空
            LoggerMock logger = new LoggerMock();
            Assert.IsEmpty(logger.Log);

            // null設定時は空白が設定されること、それ以外はそのまま
            logger.Log = null;
            Assert.IsEmpty(logger.Log);
            logger.Log = "test";
            Assert.AreEqual("test", logger.Log);

            // 更新時にLogUpdateイベントが実行されること
            int count = 0;
            logger.LogUpdate += new EventHandler((object sender, EventArgs e) => { ++count; });
            Assert.AreEqual(0, count);
            logger.Log = "ログ";
            Assert.AreEqual(1, count);
            Assert.AreEqual("ログ", logger.Log);
            logger.Log += "add";
            Assert.AreEqual(2, count);
            Assert.AreEqual("ログadd", logger.Log);
        }

        #endregion

        // TODO: 他のメソッドについてもテストを行う

        #region ログ登録メソッド（一般）テストケース

        /// <summary>
        /// AddMessageメソッドテストケース。
        /// </summary>
        [Test]
        public void TestAddMessage()
        {
            LoggerMock logger = new LoggerMock();
            
            // 通常は一行が出力される
            Assert.IsEmpty(logger.ToString());
            logger.AddMessage("1st string");
            Assert.AreEqual("1st string\r\n", logger.ToString());
            logger.AddMessage("2nd string");
            Assert.AreEqual("1st string\r\n2nd string\r\n", logger.ToString());

            // 直前のログが改行されていない場合、改行して出力される
            logger.Log += "3rd ";
            logger.AddMessage("string");
            Assert.AreEqual("1st string\r\n2nd string\r\n3rd \r\nstring\r\n", logger.ToString());

            // パラメータが二つの方は、String.Formatした値を出力する
            logger.AddMessage("{0}th string", 4);
            Assert.AreEqual("1st string\r\n2nd string\r\n3rd \r\nstring\r\n4th string\r\n", logger.ToString());
        }

        #endregion
    }
}
