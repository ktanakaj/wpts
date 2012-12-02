// ================================================================================================
// <summary>
//      MediaWikiVariableのテストクラスソース。</summary>
//
// <copyright file="MediaWikiVariableTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using Honememo.Parsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="MediaWikiVariable"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class MediaWikiVariableTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            MediaWikiVariable element;
            IElement value;

            element = new MediaWikiVariable(null);
            Assert.IsNull(element.Variable);
            Assert.IsNull(element.Value);

            element = new MediaWikiVariable("変数名1");
            Assert.AreEqual("変数名1", element.Variable);
            Assert.IsNull(element.Value);

            value = new TextElement("値");
            element = new MediaWikiVariable("変数名2", value);
            Assert.AreEqual("変数名2", element.Variable);
            Assert.AreSame(value, element.Value);
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Variableプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestVariable()
        {
            MediaWikiVariable element = new MediaWikiVariable("変数名");

            Assert.AreEqual("変数名", element.Variable);
            element.Variable = "test";
            Assert.AreEqual("test", element.Variable);
            element.Variable = null;
            Assert.IsNull(element.Variable);
        }

        /// <summary>
        /// Valueプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestValue()
        {
            MediaWikiVariable element = new MediaWikiVariable("変数名");
            IElement value = new TextElement("値");

            Assert.IsNull(element.Value);
            element.Value = value;
            Assert.AreSame(value, element.Value);
            element.Value = null;
            Assert.IsNull(element.Value);
        }

        #endregion
        
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            MediaWikiVariable element = new MediaWikiVariable("変数名");

            // 変数名のみ
            Assert.AreEqual("{{{変数名}}}", element.ToString());

            // 単純な値
            element.Value = new TextElement("値");
            Assert.AreEqual("{{{変数名|値}}}", element.ToString());

            // 複雑な値
            ListElement list = new ListElement();
            list.Add(new MediaWikiLink("記事名"));
            list.Add(new MediaWikiTemplate("テンプレート名"));
            element.Value = list;
            Assert.AreEqual("{{{変数名|[[記事名]]{{テンプレート名}}}}}", element.ToString());
        }

        #endregion
    }
}
