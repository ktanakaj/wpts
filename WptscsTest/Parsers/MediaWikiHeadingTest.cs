// ================================================================================================
// <summary>
//      MediaWikiHeadingのテストクラスソース。</summary>
//
// <copyright file="MediaWikiHeadingTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using Honememo.Parsers;
    using NUnit.Framework;

    /// <summary>
    /// MediaWikiHeadingのテストクラスです。
    /// </summary>
    [TestFixture]
    public class MediaWikiHeadingTest
    {
        #region プロパティテストケース

        /// <summary>
        /// Levelプロパティテストケース。
        /// </summary>
        [Test]
        public void TestLevel()
        {
            MediaWikiHeading element = new MediaWikiHeading();

            Assert.AreEqual(0, element.Level);
            element.Level = 2;
            Assert.AreEqual(2, element.Level);
            element.Level = -5;
            Assert.AreEqual(-5, element.Level);
        }

        #endregion
        
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            MediaWikiHeading element = new MediaWikiHeading();

            // 初期状態
            Assert.IsEmpty(element.ToString());

            // 見出し1階層
            element.Level = 1;
            Assert.AreEqual("==", element.ToString());

            // 見出し中身設定
            element.Add(new TextElement("見出し"));
            Assert.AreEqual("=見出し=", element.ToString());

            // 階層をいろいろ変更
            element.Level = 0;
            Assert.AreEqual("見出し", element.ToString());
            element.Level = 2;
            Assert.AreEqual("==見出し==", element.ToString());
            element.Level = -4;
            Assert.AreEqual("見出し", element.ToString());
            element.Level = 3;
            Assert.AreEqual("===見出し===", element.ToString());

            // 見出し中身追加
            element.Add(new XmlCommentElement("コメント"));
            Assert.AreEqual("===見出し<!--コメント-->===", element.ToString());
        }

        #endregion
    }
}
