// ================================================================================================
// <summary>
//      XmlTextElementのテストクラスソース。</summary>
//
// <copyright file="XmlTextElementTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="XmlTextElement"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class XmlTextElementTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            XmlTextElement element = new XmlTextElement();
            Assert.IsNull(element.Text);

            element = new XmlTextElement("test");
            Assert.AreEqual("test", element.Text);

            element = new XmlTextElement("<test>");
            Assert.AreEqual("<test>", element.Text);
            Assert.AreEqual("&lt;test&gt;", element.Raw);
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Textプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestText()
        {
            XmlTextElement element = new XmlTextElement();

            element.Text = "test";
            Assert.AreEqual("test", element.Text);

            element.Text = "<test>";
            Assert.AreEqual("<test>", element.Text);
            Assert.AreEqual("&lt;test&gt;", element.Raw);
        }

        /// <summary>
        /// Rawプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestRaw()
        {
            XmlTextElement element = new XmlTextElement();

            element.Raw = "test";
            Assert.AreEqual("test", element.Raw);

            element.Raw = "&lt;test&gt;";
            Assert.AreEqual("&lt;test&gt;", element.Raw);
            Assert.AreEqual("<test>", element.Text);
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            XmlTextElement element = new XmlTextElement();

            Assert.AreEqual(string.Empty, element.ToString());
            element.Text = "test";
            Assert.AreEqual("test", element.ToString());
            element.Text = "<test> & \"test'";
            Assert.AreEqual("&lt;test&gt; &amp; &quot;test&apos;", element.ToString());
        }

        #endregion
    }
}
