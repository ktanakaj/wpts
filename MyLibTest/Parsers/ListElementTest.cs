// ================================================================================================
// <summary>
//      ListElementのテストクラスソース。</summary>
//
// <copyright file="ListElementTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// ListElementのテストクラスです。
    /// </summary>
    [TestFixture]
    public class ListElementTest
    {
        #region インタフェース実装プロパティテストケース

        /// <summary>
        /// ParsedStringプロパティテストケース。
        /// </summary>
        [Test]
        public void TestParsedString()
        {
            ListElement element = new ListElement();

            Assert.IsNull(element.ParsedString);
            element.ParsedString = "test";
            Assert.AreEqual("test", element.ParsedString);
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            ListElement element = new ListElement();

            Assert.IsEmpty(element.ToString());

            element.Add(new TextElement { Text = "test1" });
            Assert.AreEqual("test1", element.ToString());

            element.Add(new TextElement { Text = "test2" });
            Assert.AreEqual("test1test2", element.ToString());

            element.Add(new TextElement { Text = "test3" });
            element.RemoveAt(1);
            Assert.AreEqual("test1test3", element.ToString());

            element.ParsedString = "parsed test";
            Assert.AreEqual("parsed test", element.ToString());
        }

        #endregion
    }
}
