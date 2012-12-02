// ================================================================================================
// <summary>
//      LockObjectのテストクラスソース。</summary>
//
// <copyright file="LockObjectTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="LockObject"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class LockObjectTest
    {
        #region パラメータ単位のロック用メソッドテストケース

        /// <summary>
        /// <see cref="LockObject.GetObject"/>メソッドテストケース（正常系）。
        /// </summary>
        [TestMethod]
        public void TestGetObject()
        {
            // 同じ入力に同じオブジェクトが、違う入力には違うオブジェクトが返ること
            // ※ 厳密にはハッシュ単位なので一致することもありえるが
            LockObject lockObject = new LockObject();
            object obj = lockObject.GetObject("test");
            Assert.IsNotNull(obj);
            Assert.AreNotSame(obj, lockObject.GetObject("test2"));
            Assert.AreSame(obj, lockObject.GetObject("test"));
        }

        /// <summary>
        /// <see cref="LockObject.GetObject"/>メソッドテストケース（並列実行）。
        /// </summary>
        [TestMethod, Timeout(1500)]
        public void TestGetObjectParallel()
        {
            // 同じ入力に同じオブジェクトが返ること
            LockObject lockObject = new LockObject();
            Parallel.For(
                0,
                100000,
                (int i)
                    =>
                {
                    int key = i % 100;
                    object obj = lockObject.GetObject(key);
                    Assert.IsNotNull(obj);
                    Assert.AreSame(obj, lockObject.GetObject(key));
                });
        }

        /// <summary>
        /// <see cref="LockObject.GetObject"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetObjectNull()
        {
            new LockObject().GetObject(null);
        }
        
        #endregion
    }
}
