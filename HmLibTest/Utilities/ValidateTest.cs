// ================================================================================================
// <summary>
//      Validateのテストクラスソース。</summary>
//
// <copyright file="ValidateTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// <see cref="Validate"/>のテストクラスです。
    /// </summary>
    [TestFixture]
    internal class ValidateTest
    {
        #region NotNullメソッドテストケース

        /// <summary>
        /// <see cref="Validate.NotNull&lt;T&gt;(T, string)"/>メソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestNotNull()
        {
            // パラメータ名指定無し
            Assert.AreEqual(String.Empty, Validate.NotNull(String.Empty));
            Assert.AreEqual("not null", Validate.NotNull("not null"));

            // パラメータ名指定有り
            Assert.AreEqual(String.Empty, Validate.NotNull(String.Empty, null));
            Assert.AreEqual(String.Empty, Validate.NotNull(String.Empty, "test"));
            Assert.AreEqual("not null", Validate.NotNull("not null", "test"));
        }

        /// <summary>
        /// <see cref="Validate.NotNull&lt;T&gt;(T, string)"/>メソッドテストケース（異常系）。
        /// </summary>
        [Test]
        public void TestNotNullNg()
        {
            // obj = nullのチェック
            try
            {
                // パラメータ名指定無し
                Validate.NotNull<object>(null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            // 例外パラメータ名の確認
            try
            {
                // パラメータ名指定有り
                Validate.NotNull<object>(null, "test");
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("test", ex.ParamName);
            }

            try
            {
                // パラメータ名指定有りnull
                Validate.NotNull<object>(null, null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNull(ex.ParamName);
            }
        }

        #endregion

        #region NotEmptyメソッドテストケース

        /// <summary>
        /// <see cref="Validate.NotEmpty(string, string)"/>メソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestNotEmpty()
        {
            // パラメータ名指定無し
            Assert.AreEqual("not empty", Validate.NotEmpty("not empty"));

            // パラメータ名指定有り
            Assert.AreEqual("not empty", Validate.NotEmpty("not empty", "test"));
        }

        /// <summary>
        /// <see cref="Validate.NotEmpty(string, string)"/>メソッドテストケース（異常系）。
        /// </summary>
        [Test]
        public void TestNotEmptyNg()
        {
            // str = nullのチェック
            try
            {
                // パラメータ名指定無し
                Validate.NotEmpty(null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            // 例外パラメータ名の確認
            try
            {
                // パラメータ名指定有り
                Validate.NotEmpty(null, "test");
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("test", ex.ParamName);
            }

            try
            {
                // パラメータ名指定有りnull
                Validate.NotEmpty(null, null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNull(ex.ParamName);
            }

            // 空文字列のチェック
            try
            {
                // パラメータ名指定無し
                Validate.NotEmpty(String.Empty);
                Assert.Fail("expected ArgumentException");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            // 例外パラメータ名の確認
            try
            {
                // パラメータ名指定有り
                Validate.NotEmpty(String.Empty, "test");
                Assert.Fail("expected ArgumentException");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("test", ex.ParamName);
            }
        }

        #endregion

        #region NotBlankメソッドテストケース

        /// <summary>
        /// <see cref="Validate.NotBlank(string, string)"/>メソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestNotBlank()
        {
            // パラメータ名指定無し
            Assert.AreEqual("not blank", Validate.NotBlank("not blank"));

            // パラメータ名指定有り
            Assert.AreEqual("not blank", Validate.NotBlank("not blank", "test"));
        }

        /// <summary>
        /// <see cref="Validate.NotBlank(string, string)"/>メソッドテストケース（異常系）。
        /// </summary>
        [Test]
        public void TestNotBlankNg()
        {
            // str = nullのチェック
            try
            {
                // パラメータ名指定無し
                Validate.NotBlank(null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            // 例外パラメータ名の確認
            try
            {
                // パラメータ名指定有り
                Validate.NotBlank(null, "test");
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("test", ex.ParamName);
            }

            try
            {
                // パラメータ名指定有りnull
                Validate.NotBlank(null, null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNull(ex.ParamName);
            }

            // 空白のチェック
            try
            {
                // パラメータ名指定無し
                Validate.NotBlank("  ");
                Assert.Fail("expected ArgumentException");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            // 例外パラメータ名の確認
            try
            {
                // パラメータ名指定有り
                Validate.NotBlank("   ", "test");
                Assert.Fail("expected ArgumentException");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("test", ex.ParamName);
            }
        }

        #endregion

        #region InRangeメソッドテストケース

        /// <summary>
        /// <see cref="Validate.InRange(string, int, string, string)"/>
        /// メソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestInRangeStr()
        {
            // ※ 例外が起きなければOK
            // パラメータ名指定無し
            Validate.InRange("1", 0);
            Validate.InRange("range text", 9);

            // パラメータ名指定有り
            Validate.InRange("1", 0, "test", "testindex");
            Validate.InRange("range text", 9, "test", "testindex");
        }

        /// <summary>
        /// <see cref="Validate.InRange(string, int, string, string)"/>
        /// メソッドテストケース（異常系）。
        /// </summary>
        [Test]
        public void TestInRangeStrNg()
        {
            // str = nullのチェック
            string str = null;
            try
            {
                // パラメータ名指定無し
                Validate.InRange(str, 0);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            // 例外パラメータ名の確認
            try
            {
                // パラメータ名指定有り
                Validate.InRange(str, 0, "test", "testindex");
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("test", ex.ParamName);
            }

            try
            {
                // パラメータ名指定有りnull
                Validate.InRange(str, 0, null, null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNull(ex.ParamName);
            }

            // indexが範囲外のチェック
            try
            {
                // パラメータ名指定無し
                Validate.InRange(String.Empty, 0);
                Assert.Fail("expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("index", ex.ParamName);
            }

            try
            {
                // パラメータ名指定無し
                Validate.InRange("range text", 10);
                Assert.Fail("expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("index", ex.ParamName);
            }

            try
            {
                // パラメータ名指定無し
                Validate.InRange("range text", -1);
                Assert.Fail("expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("index", ex.ParamName);
            }

            // 例外パラメータ名の確認
            try
            {
                // パラメータ名指定有り
                Validate.InRange(String.Empty, 0, "test", "testindex");
                Assert.Fail("expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("testindex", ex.ParamName);
            }

            try
            {
                // パラメータ名指定有りnull
                Validate.InRange(String.Empty, 0, null, null);
                Assert.Fail("expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.IsNull(ex.ParamName);
            }
        }

        /// <summary>
        /// <see cref="Validate.InRange&lt;T&gt;(IList&lt;T&gt;, int, string, string)"/>
        /// メソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestInRangeIList()
        {
            // ※ 例外が起きなければOK
            // パラメータ名指定無し
            Validate.InRange(new object[1], 0);
            Validate.InRange(new object[10], 9);

            // パラメータ名指定有り
            Validate.InRange(new object[1], 0, "test", "testindex");
            Validate.InRange(new object[10], 9, "test", "testindex");
        }

        /// <summary>
        /// <see cref="Validate.InRange&lt;T&gt;(IList&lt;T&gt;, int, string, string)"/>
        /// メソッドテストケース（異常系）。
        /// </summary>
        [Test]
        public void TestInRangeIListNg()
        {
            // list = nullのチェック
            object[] list = null;
            try
            {
                // パラメータ名指定無し
                Validate.InRange(list, 0);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            // 例外パラメータ名の確認
            try
            {
                // パラメータ名指定有り
                Validate.InRange(list, 0, "test", "testindex");
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("test", ex.ParamName);
            }

            try
            {
                // パラメータ名指定有りnull
                Validate.InRange(list, 0, null, null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNull(ex.ParamName);
            }

            // index範囲外のチェック
            try
            {
                // パラメータ名指定無し
                Validate.InRange(new object[0], 0);
                Assert.Fail("expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("index", ex.ParamName);
            }

            try
            {
                // パラメータ名指定無し
                Validate.InRange(new object[10], 10);
                Assert.Fail("expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("index", ex.ParamName);
            }

            try
            {
                // パラメータ名指定無し
                Validate.InRange(new object[10], -1);
                Assert.Fail("expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("index", ex.ParamName);
            }

            // 例外パラメータ名の確認
            try
            {
                // パラメータ名指定有り
                Validate.InRange(new object[0], 0, "test", "testindex");
                Assert.Fail("expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual("testindex", ex.ParamName);
            }

            try
            {
                // パラメータ名指定有りnull
                Validate.InRange(new object[0], 0, null, null);
                Assert.Fail("expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.IsNull(ex.ParamName);
            }
        }

        #endregion
    }
}
