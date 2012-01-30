// ================================================================================================
// <summary>
//      Validateのテストクラスソース。</summary>
//
// <copyright file="ValidateTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Validateのテストクラスです。
    /// </summary>
    [TestFixture]
    public class ValidateTest
    {
        #region NotNullメソッドテストケース

        /// <summary>
        /// NotNullメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestNotNull()
        {
            // 引数一つ
            Assert.AreEqual(String.Empty, Validate.NotNull(String.Empty));
            Assert.AreEqual("not null", Validate.NotNull("not null"));

            // 引数二つ
            Assert.AreEqual(String.Empty, Validate.NotNull(String.Empty, null));
            Assert.AreEqual(String.Empty, Validate.NotNull(String.Empty, "test"));
            Assert.AreEqual("not null", Validate.NotNull("not null", "test"));
        }

        /// <summary>
        /// NotNullメソッドテストケース（異常系）。
        /// </summary>
        [Test]
        public void TestNotNullNg()
        {
            try
            {
                // 引数一つ
                Validate.NotNull<object>(null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            try
            {
                // 引数二つ
                Validate.NotNull<object>(null, "test");
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("test", ex.ParamName);
            }

            try
            {
                // 引数二つnull
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
        /// NotEmptyメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestNotEmpty()
        {
            // 引数一つ
            Assert.AreEqual("not empty", Validate.NotEmpty("not empty"));

            // 引数二つ
            Assert.AreEqual("not empty", Validate.NotEmpty("not empty", "test"));
        }

        /// <summary>
        /// NotEmptyメソッドテストケース（異常系）。
        /// </summary>
        [Test]
        public void TestNotEmptyNg()
        {
            try
            {
                // 引数一つ
                Validate.NotEmpty(null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            try
            {
                // 引数二つ
                Validate.NotEmpty(null, "test");
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("test", ex.ParamName);
            }

            try
            {
                // 引数二つnull
                Validate.NotEmpty(null, null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNull(ex.ParamName);
            }

            try
            {
                // 引数一つ
                Validate.NotEmpty(String.Empty);
                Assert.Fail("expected ArgumentException");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            try
            {
                // 引数二つ
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
        /// NotBlankメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestNotBlank()
        {
            // 引数一つ
            Assert.AreEqual("not blank", Validate.NotBlank("not blank"));

            // 引数二つ
            Assert.AreEqual("not blank", Validate.NotBlank("not blank", "test"));
        }

        /// <summary>
        /// NotBlankメソッドテストケース（異常系）。
        /// </summary>
        [Test]
        public void TestNotBlankNg()
        {
            try
            {
                // 引数一つ
                Validate.NotBlank(null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            try
            {
                // 引数二つ
                Validate.NotBlank(null, "test");
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("test", ex.ParamName);
            }

            try
            {
                // 引数二つnull
                Validate.NotBlank(null, null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                Assert.IsNull(ex.ParamName);
            }

            try
            {
                // 引数一つ
                Validate.NotBlank("  ");
                Assert.Fail("expected ArgumentException");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("value", ex.ParamName);
            }

            try
            {
                // 引数二つ
                Validate.NotBlank("   ", "test");
                Assert.Fail("expected ArgumentException");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("test", ex.ParamName);
            }
        }

        #endregion
    }
}
