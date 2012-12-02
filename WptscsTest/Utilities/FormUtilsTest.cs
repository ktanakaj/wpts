// ================================================================================================
// <summary>
//      FormUtilsのテストクラスソース。</summary>
//
// <copyright file="FormUtilsTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Utilities
{
    using System;
    using System.Windows.Forms;
    using Honememo.Wptscs.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="FormUtils"/>のテストクラスです。
    /// </summary>
    /// <remarks>
    /// その性質上、画面周りのメソッドならびに設定ファイル関連のメソッドについてはテストケースが作成できていない。
    /// これらのメソッドに手を入れる際は注意すること。
    /// </remarks>
    [TestClass]
    public class FormUtilsTest
    {
        #region private変数

        /// <summary>
        /// テスト実施中カルチャを変更し後で戻すため、そのバックアップ。
        /// </summary>
        private System.Globalization.CultureInfo backupCulture;

        #endregion

        #region 前処理・後処理

        /// <summary>
        /// テストの前処理。
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            // 一部処理結果はカルチャーにより変化するため、ja-JPを明示的に設定する
            this.backupCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("ja-JP");
        }

        /// <summary>
        /// テストの後処理。
        /// </summary>
        [TestCleanup]
        public void TearDown()
        {
            // カルチャーを元に戻す
            System.Threading.Thread.CurrentThread.CurrentUICulture = this.backupCulture;
        }

        #endregion

        #region リソース関連テストケース

        /// <summary>
        /// ReplaceInvalidFileNameCharsメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestApplicationName()
        {
            // ※ バージョンが変わるごとにバージョン表記の部分を書き換えるのは面倒なので置換
            Assert.AreEqual(
                "Wikipedia 翻訳支援ツール VerX.XX",
                new System.Text.RegularExpressions.Regex("Ver[0-9]+\\.[0-9]+").Replace(FormUtils.ApplicationName(), "VerX.XX"));
        }

        /// <summary>
        /// ReplaceInvalidFileNameCharsメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestReplaceInvalidFileNameChars()
        {
            Assert.AreEqual("C__test_test.doc", FormUtils.ReplaceInvalidFileNameChars("C:\\test\\test.doc"));
            Assert.AreEqual("_home_test_test.doc", FormUtils.ReplaceInvalidFileNameChars("/home/test/test.doc"));
            Assert.AreEqual("______", FormUtils.ReplaceInvalidFileNameChars("*?\"<>|"));

            // 一見普通のファイル名に見えるが、&nbsp;由来の半角スペース (u00a0) が含まれており問題を起こす
            // 通常の半角スペース (u0020) に変換する
            Assert.AreEqual("Fuji (Spacecraft).xml", FormUtils.ReplaceInvalidFileNameChars("Fuji (Spacecraft).xml"));
        }

        #endregion

        #region null値許容メソッドテストケース

        /// <summary>
        /// ToStringメソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            // 引数二つ
            Assert.IsNull(FormUtils.ToString(null, null));
            Assert.IsNull(FormUtils.ToString(new DummyCell(), null));
            Assert.AreEqual("null", FormUtils.ToString(new DummyCell(), "null"));
            Assert.AreEqual("not null", FormUtils.ToString(new DummyCell { Value = "not null" }, "null"));
            Assert.AreNotEqual("null", FormUtils.ToString(new DummyCell { Value = new object() }, "null"));

            // 引数一つ
            Assert.AreEqual(string.Empty, FormUtils.ToString(null));
            Assert.AreEqual(string.Empty, FormUtils.ToString(new DummyCell()));
            Assert.AreEqual("not null", FormUtils.ToString(new DummyCell { Value = "not null" }));
            Assert.IsTrue(FormUtils.ToString(new DummyCell { Value = new object() }).Length > 0);
        }

        #endregion

        #region モッククラス

        /// <summary>
        /// DataGridViewCellテスト用のモッククラスです。
        /// </summary>
        public class DummyCell : DataGridViewCell
        {
        }

        #endregion
    }
}
