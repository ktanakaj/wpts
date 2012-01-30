// ================================================================================================
// <summary>
//      XmlCommentElementのテストクラスソース。</summary>
//
// <copyright file="XmlCommentElementTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// XmlCommentElementのテストクラスです。
    /// </summary>
    [TestFixture]
    public class XmlCommentElementTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            XmlCommentElement comment = new XmlCommentElement();
            Assert.IsNull(comment.Text);

            comment = new XmlCommentElement("test");
            Assert.AreEqual("test", comment.Text);
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            XmlCommentElement comment = new XmlCommentElement();
            Assert.AreEqual("<!---->", comment.ToString());

            comment.Text = "test";
            Assert.AreEqual("<!--test-->", comment.ToString());

            comment.ParsedString = "<!--test--";
            Assert.AreEqual("<!--test--", comment.ToString());
        }

        #endregion
    }
}
