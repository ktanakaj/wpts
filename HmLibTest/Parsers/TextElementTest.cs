// ================================================================================================
// <summary>
//      TextElementのテストクラスソース。</summary>
//
// <copyright file="TextElementTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// TextElementのテストクラスです。
    /// </summary>
    [TestFixture]
    public class TextElementTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            TextElement element = new TextElement();
            Assert.IsNull(element.Text);

            element = new TextElement("test");
            Assert.AreEqual("test", element.Text);
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Textプロパティテストケース。
        /// </summary>
        [Test]
        public void TestText()
        {
            TextElement element = new TextElement();

            Assert.IsNull(element.Text);
            element.Text = "test";
            Assert.AreEqual("test", element.Text);
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            TextElement element = new TextElement();

            Assert.IsEmpty(element.ToString());
            element.Text = "test";
            Assert.AreEqual("test", element.ToString());
        }

        #endregion
    }
}
