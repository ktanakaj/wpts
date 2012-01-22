// ================================================================================================
// <summary>
//      XmlUtilsのテストクラスソース。</summary>
//
// <copyright file="XmlUtilsTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Xml;
    using NUnit.Framework;

    /// <summary>
    /// XmlUtilsのテストクラスです。
    /// </summary>
    [TestFixture]
    public class XmlUtilsTest
    {
        #region null値許容メソッドテストケース

        /// <summary>
        /// InnerTextメソッドテストケース。
        /// </summary>
        [Test]
        public void TestInnerText()
        {
            // 引数一つ
            Assert.AreEqual(String.Empty, XmlUtils.InnerText(null));
            Assert.AreEqual("test", XmlUtils.InnerText(new XmlDocument { InnerXml = "<dummy>test</dummy>" }));

            // 引数二つ
            Assert.IsNull(XmlUtils.InnerText(null, null));
            Assert.AreEqual("null", XmlUtils.InnerText(null, "null"));
            Assert.AreEqual("test", XmlUtils.InnerText(new XmlDocument { InnerXml = "<dummy>test</dummy>" }, "null"));
        }

        /// <summary>
        /// InnerXmlメソッドテストケース。
        /// </summary>
        [Test]
        public void TestInnerXml()
        {
            // 引数一つ
            Assert.AreEqual(String.Empty, XmlUtils.InnerXml(null));
            Assert.AreEqual("<test />", XmlUtils.InnerXml(new XmlDocument { InnerXml = "<test />" }));

            // 引数二つ
            Assert.IsNull(XmlUtils.InnerText(null, null));
            Assert.AreEqual("<null />", XmlUtils.InnerXml(null, "<null />"));
            Assert.AreEqual("<test />", XmlUtils.InnerXml(new XmlDocument { InnerXml = "<test />" }, "<null />"));
        }

        /// <summary>
        /// OuterXmlメソッドテストケース。
        /// </summary>
        [Test]
        public void TestOuterXml()
        {
            // 引数一つ
            Assert.AreEqual(String.Empty, XmlUtils.OuterXml(null));
            Assert.AreEqual("<test />", XmlUtils.OuterXml(new XmlDocument { InnerXml = "<test />" }));

            // 引数二つ
            Assert.IsNull(XmlUtils.InnerText(null, null));
            Assert.AreEqual("<null />", XmlUtils.OuterXml(null, "<null />"));
            Assert.AreEqual("<test />", XmlUtils.OuterXml(new XmlDocument { InnerXml = "<test />" }, "<null />"));
        }

        #endregion

        #region エンコード／デコードテストケース

        /// <summary>
        /// XmlEncodeメソッドテストケース。
        /// </summary>
        [Test]
        public void TestXmlEncode()
        {
            Assert.AreEqual("test", XmlUtils.XmlEncode("test"));
            Assert.AreEqual("&lt;", XmlUtils.XmlEncode("<"));
            Assert.AreEqual("&gt;", XmlUtils.XmlEncode(">"));
            Assert.AreEqual("&amp;", XmlUtils.XmlEncode("&"));
            Assert.AreEqual("&quot;", XmlUtils.XmlEncode("\""));
            Assert.AreEqual("&apos;", XmlUtils.XmlEncode("'"));
        }

        /// <summary>
        /// XmlEncodeメソッドテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXmlEncodeNull()
        {
            XmlUtils.XmlEncode(null);
        }

        /// <summary>
        /// XmlDecodeメソッドテストケース。
        /// </summary>
        [Test]
        public void TestXmlDecode()
        {
            Assert.AreEqual("test", XmlUtils.XmlDecode("test"));
            Assert.AreEqual("<", XmlUtils.XmlDecode("&lt;"));
            Assert.AreEqual(">", XmlUtils.XmlDecode("&gt;"));
            Assert.AreEqual("&", XmlUtils.XmlDecode("&amp;"));
            Assert.AreEqual("\"", XmlUtils.XmlDecode("&quot;"));
            Assert.AreEqual("'", XmlUtils.XmlDecode("&apos;"));
        }

        /// <summary>
        /// XmlDecodeメソッドテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXmlDecodeNull()
        {
            XmlUtils.XmlDecode(null);
        }

        #endregion
    }
}
