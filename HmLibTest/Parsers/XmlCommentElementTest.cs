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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="XmlCommentElement"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class XmlCommentElementTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [TestMethod]
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
        [TestMethod]
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
