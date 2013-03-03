// ================================================================================================
// <summary>
//      XmlUtilsのテストクラスソース。</summary>
//
// <copyright file="XmlUtilsTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2013 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="XmlUtils"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class XmlUtilsTest
    {
        #region null値許容メソッドテストケース

        /// <summary>
        /// <see cref="XmlUtils.InnerText(XmlNode)"/>,
        /// <see cref="XmlUtils.InnerText(XmlNode, string)"/>
        /// メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestInnerText()
        {
            // 引数一つ
            Assert.AreEqual(string.Empty, XmlUtils.InnerText(null));
            Assert.AreEqual("test", XmlUtils.InnerText(new XmlDocument { InnerXml = "<dummy>test</dummy>" }));

            // 引数二つ
            Assert.IsNull(XmlUtils.InnerText(null, null));
            Assert.AreEqual("null", XmlUtils.InnerText(null, "null"));
            Assert.AreEqual("test", XmlUtils.InnerText(new XmlDocument { InnerXml = "<dummy>test</dummy>" }, "null"));
        }

        /// <summary>
        /// <see cref="XmlUtils.InnerXml(XmlNode)"/>,
        /// <see cref="XmlUtils.InnerXml(XmlNode, string)"/>
        /// メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestInnerXml()
        {
            // 引数一つ
            Assert.AreEqual(string.Empty, XmlUtils.InnerXml(null));
            Assert.AreEqual("<test />", XmlUtils.InnerXml(new XmlDocument { InnerXml = "<test />" }));

            // 引数二つ
            Assert.IsNull(XmlUtils.InnerXml(null, null));
            Assert.AreEqual("<null />", XmlUtils.InnerXml(null, "<null />"));
            Assert.AreEqual("<test />", XmlUtils.InnerXml(new XmlDocument { InnerXml = "<test />" }, "<null />"));
        }

        /// <summary>
        /// <see cref="XmlUtils.OuterXml(XmlNode)"/>,
        /// <see cref="XmlUtils.OuterXml(XmlNode, string)"/>
        /// メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestOuterXml()
        {
            // 引数一つ
            Assert.AreEqual(string.Empty, XmlUtils.OuterXml(null));
            Assert.AreEqual("<test />", XmlUtils.OuterXml(new XmlDocument { InnerXml = "<test />" }));

            // 引数二つ
            Assert.IsNull(XmlUtils.OuterXml(null, null));
            Assert.AreEqual("<null />", XmlUtils.OuterXml(null, "<null />"));
            Assert.AreEqual("<test />", XmlUtils.OuterXml(new XmlDocument { InnerXml = "<test />" }, "<null />"));
        }

        /// <summary>
        /// <see cref="XmlUtils.Value(XAttribute)"/>,
        /// <see cref="XmlUtils.Value(XAttribute, string)"/>
        /// メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestValue()
        {
            // 引数一つ
            Assert.AreEqual(string.Empty, XmlUtils.Value(null));
            Assert.AreEqual("100", XmlUtils.Value(new XAttribute("id", "100")));

            // 引数二つ
            Assert.IsNull(XmlUtils.Value(null, null));
            Assert.AreEqual("abc", XmlUtils.Value(null, "abc"));
            Assert.AreEqual("100", XmlUtils.Value(new XAttribute("id", "100"), "abc"));
        }

        #endregion

        #region エンコード／デコードテストケース

        /// <summary>
        /// <see cref="XmlUtils.XmlEncode"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
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
        /// <see cref="XmlUtils.XmlEncode"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXmlEncodeNull()
        {
            XmlUtils.XmlEncode(null);
        }

        /// <summary>
        /// <see cref="XmlUtils.XmlDecode"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
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
        /// <see cref="XmlUtils.XmlDecode"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestXmlDecodeNull()
        {
            XmlUtils.XmlDecode(null);
        }

        #endregion
    }
}
