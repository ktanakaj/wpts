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

        // TODO: 他のメソッドについてもテストを行う

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
            logger.AddSource(new TextElement("2nd string"));
            diff.AddSource(new TextElement("2nd string"));
            Assert.AreEqual(diff.ToString(), logger.ToString());

            // MediaWiki関連の一部要素は独自に整形して出力
            logger.Clear();
            logger.AddSource(new MediaWikiLink("記事名") { Section = "セクション", Code = "ja" });
            Assert.AreEqual("[[記事名]] → ", logger.ToString());
            logger.Clear();
            logger.AddSource(new MediaWikiTemplate("記事名") { Section = "セクション", Code = "ja" });
            Assert.AreEqual("{{記事名}} → ", logger.ToString());

            // 見出しの場合、右矢印が出ない＆直前に空行が入る
            logger.Clear();
            logger.AddSource(new MediaWikiHeading {ParsedString = "==見出し==" });
            Assert.AreEqual(Environment.NewLine + "==見出し==", logger.ToString());

            // いずれのケースでも、親クラスにある改行されていなければ改行は行われる
            logger.Clear();
            logger.Log = "test";
            logger.AddSource(new MediaWikiLink("記事名") { Section = "セクション", Code = "ja" });
            Assert.AreEqual("test" + Environment.NewLine + "[[記事名]] → ", logger.ToString());
            logger.AddSource(new MediaWikiHeading { ParsedString = "==見出し==" });
            Assert.AreEqual("test" + Environment.NewLine + "[[記事名]] → " + Environment.NewLine + Environment.NewLine + "==見出し==", logger.ToString());
        }

        #endregion
    }
}
