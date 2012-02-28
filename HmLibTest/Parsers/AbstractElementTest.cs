﻿// ================================================================================================
// <summary>
//      AbstractElementのテストクラスソース。</summary>
//
// <copyright file="AbstractElementTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// <see cref="AbstractElement"/>のテストクラスです。
    /// </summary>
    /// <remarks>テストには最小実装の<see cref="TextElement"/>を使用。</remarks>
    [TestFixture]
    internal class AbstractElementTest
    {
        #region インタフェース実装プロパティテストケース

        /// <summary>
        /// <see cref="AbstractElement.ParsedString"/>プロパティテストケース。
        /// </summary>
        [Test]
        public void TestParsedString()
        {
            // 値が普通に設定できること
            AbstractElement element = new TextElement();
            Assert.IsNull(element.ParsedString);
            element.ParsedString = "test";
            Assert.AreEqual("test", element.ParsedString);
            element.ParsedString = null;
            Assert.IsNull(element.ParsedString);
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="AbstractElement.ToString"/>メソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            // ParsedStringが設定されている場合その値が返ること
            AbstractElement element = new TextElement("Text element string");
            Assert.AreEqual("Text element string", element.ToString());
            element.ParsedString = "ParsedString string";
            Assert.AreEqual("ParsedString string", element.ToString());
        }

        #endregion
    }
}
