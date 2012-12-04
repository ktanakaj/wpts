// ================================================================================================
// <summary>
//      MediaWikiTemplateのテストクラスソース。</summary>
//
// <copyright file="MediaWikiTemplateTest.cs" company="honeplusのメモ帳">
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
    /// <see cref="MediaWikiTemplate"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class MediaWikiTemplateTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            MediaWikiTemplate element = new MediaWikiTemplate("テンプレート名");
            Assert.AreEqual("テンプレート名", element.Title);
            Assert.AreEqual(0, element.PipeTexts.Count);
        }

        /// <summary>
        /// コンストラクタテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorNull()
        {
            MediaWikiTemplate element = new MediaWikiTemplate(null);
        }

        /// <summary>
        /// コンストラクタテストケース（空白）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorBlank()
        {
            MediaWikiTemplate element = new MediaWikiTemplate(" ");
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Titleプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestTitle()
        {
            MediaWikiTemplate element = new MediaWikiTemplate("テンプレート名");

            Assert.AreEqual("テンプレート名", element.Title);
            element.Title = "test";
            Assert.AreEqual("test", element.Title);
        }

        /// <summary>
        /// Titleプロパティテストケース（空白）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestTitleBlank()
        {
            MediaWikiTemplate element = new MediaWikiTemplate("テンプレート名");
            element.Title = " ";
        }

        /// <summary>
        /// IsMsgnwプロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestIsMsgnw()
        {
            MediaWikiTemplate element = new MediaWikiTemplate("テンプレート名");

            Assert.IsFalse(element.IsMsgnw);
            element.IsMsgnw = true;
            Assert.IsTrue(element.IsMsgnw);
            element.IsMsgnw = false;
            Assert.IsFalse(element.IsMsgnw);
        }

        /// <summary>
        /// <see cref="MediaWikiTemplate.Comment"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestComment()
        {
            MediaWikiTemplate element = new MediaWikiTemplate("テンプレート名");

            Assert.IsNull(element.Comment);
            element.Comment = "  <!-- コメント -->  \n";
            Assert.AreEqual("  <!-- コメント -->  \n", element.Comment);
            element.Comment = null;
            Assert.IsNull(element.Comment);
            element.Comment = string.Empty;
            Assert.AreEqual(string.Empty, element.Comment);
        }

        #endregion
        
        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            MediaWikiTemplate element = new MediaWikiTemplate("テンプレート名");

            // タイトルのみ
            Assert.AreEqual("{{テンプレート名}}", element.ToString());

            // タイトルとパイプ後の文字列
            element.PipeTexts.Add(new TextElement("パラメータ1"));
            element.PipeTexts.Add(new TextElement("パラメータ2"));
            Assert.AreEqual("{{テンプレート名|パラメータ1|パラメータ2}}", element.ToString());

            // msgnw: の付加
            element.IsMsgnw = true;
            Assert.AreEqual("{{msgnw:テンプレート名|パラメータ1|パラメータ2}}", element.ToString());

            // : の付加
            element.IsColon = true;
            Assert.AreEqual("{{:msgnw:テンプレート名|パラメータ1|パラメータ2}}", element.ToString());
            element.IsMsgnw = false;
            Assert.AreEqual("{{:テンプレート名|パラメータ1|パラメータ2}}", element.ToString());

            // 改行の付加
            // ※ テンプレート名直後以外のものは、普通に文字列中に格納される
            element.Comment = "\n";
            Assert.AreEqual("{{:テンプレート名\n|パラメータ1|パラメータ2}}", element.ToString());
            element.IsColon = false;
            Assert.AreEqual("{{テンプレート名\n|パラメータ1|パラメータ2}}", element.ToString());
        }

        #endregion
    }
}
