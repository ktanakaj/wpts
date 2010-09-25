﻿// ================================================================================================
// <summary>
//      FormUtilsのテストクラスソース。</summary>
//
// <copyright file="FormUtilsTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Windows.Forms;
    using NUnit.Framework;

    /// <summary>
    /// FormUtilsのテストクラスです。
    /// </summary>
    /// <remarks>
    /// その性質上、画面周りのメソッドならびに設定ファイル関連のメソッドについてはテストケースが作成できていない。
    /// これらのメソッドに手を入れる際は注意すること。
    /// </remarks>
    [TestFixture]
    public class FormUtilsTest
    {
        /// <summary>
        /// Websiteテスト用のモッククラスです。
        /// </summary>
        public class DummyCell : DataGridViewCell 
        {
        }

        #region リソース関連テストケース
        
        /// <summary>
        /// ReplaceInvalidFileNameCharsメソッドテストケース。
        /// </summary>
        [Test]
        public void TestApplicationName()
        {
            // ※ バージョンが変わるごとにバージョン表記の部分を書き換えるのは面倒なので置換
            Assert.AreEqual(
                "Wikipedia 翻訳支援ツール VerX.XX",
                new System.Text.RegularExpressions.Regex("Ver[0-9]+\\.[0-9]+")
                .Replace(FormUtils.ApplicationName(), "VerX.XX"));
        }

        /// <summary>
        /// ReplaceInvalidFileNameCharsメソッドテストケース。
        /// </summary>
        [Test]
        public void TestReplaceInvalidFileNameChars()
        {
            Assert.AreEqual("C__test_test.doc", FormUtils.ReplaceInvalidFileNameChars("C:\\test\\test.doc"));
            Assert.AreEqual("_home_test_test.doc", FormUtils.ReplaceInvalidFileNameChars("/home/test/test.doc"));
            Assert.AreEqual("______", FormUtils.ReplaceInvalidFileNameChars("*?\"<>|"));
        }

        #endregion

        #region null値許容メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [Test]
        public void TestToString()
        {
            // 引数二つ
            Assert.IsNull(FormUtils.ToString(null, null));
            Assert.IsNull(FormUtils.ToString(new DummyCell(), null));
            Assert.AreEqual("null", FormUtils.ToString(new DummyCell(), "null"));
            Assert.AreEqual("not null", FormUtils.ToString(new DummyCell { Value = "not null" }, "null"));
            Assert.AreNotEqual("null", FormUtils.ToString(new DummyCell { Value = new object() }, "null"));

            // 引数一つ
            Assert.AreEqual(String.Empty, FormUtils.ToString(null));
            Assert.AreEqual(String.Empty, FormUtils.ToString(new DummyCell()));
            Assert.AreEqual("not null", FormUtils.ToString(new DummyCell { Value = "not null" }));
            Assert.IsNotEmpty(FormUtils.ToString(new DummyCell { Value = new object() }));
        }

        #endregion
    }
}