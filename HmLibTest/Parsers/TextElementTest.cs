// ================================================================================================
// <summary>
//      TextElementのテストクラスソース。</summary>
//
// <copyright file="TextElementTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2026 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Honememo.Parsers;

/// <summary>
/// <see cref="TextElement"/>のテストクラスです。
/// </summary>
[TestClass]
public class TextElementTest
{
    #region コンストラクタテストケース

    /// <summary>
    /// コンストラクタテストケース。
    /// </summary>
    [TestMethod]
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
    [TestMethod]
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
    [TestMethod]
    public void TestToString()
    {
        TextElement element = new TextElement();

        Assert.AreEqual(string.Empty, element.ToString());
        element.Text = "test";
        Assert.AreEqual("test", element.ToString());
    }

    #endregion
}
