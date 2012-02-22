// ================================================================================================
// <summary>
//      HashLockのテストクラスソース。</summary>
//
// <copyright file="HashLockTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// HashLockのテストクラスです。
    /// </summary>
    [TestFixture]
    public class HashLockTest
    {
        #region パラメータに対応するインスタンスを参照するメソッドテストケース

        // TestHashLockAllに記述のように、正常系は通しの試験のみ

        /// <summary>
        /// EnterReadLockメソッドテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEnterReadLockNull()
        {
            using (HashLock lockObject = new HashLock())
            {
                lockObject.EnterReadLock(null);
            }
        }

        /// <summary>
        /// EnterUpgradeableReadLockメソッドテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEnterUpgradeableReadLockNull()
        {
            using (HashLock lockObject = new HashLock())
            {
                lockObject.EnterUpgradeableReadLock(null);
            }
        }

        /// <summary>
        /// EnterWriteLockメソッドテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEnterWriteLockNull()
        {
            using (HashLock lockObject = new HashLock())
            {
                lockObject.EnterWriteLock(null);
            }
        }

        /// <summary>
        /// ExitReadLockメソッドテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestExitReadLockNull()
        {
            using (HashLock lockObject = new HashLock())
            {
                lockObject.ExitReadLock(null);
            }
        }

        /// <summary>
        /// ExitUpgradeableReadLockメソッドテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestExitUpgradeableReadLockNull()
        {
            using (HashLock lockObject = new HashLock())
            {
                lockObject.ExitUpgradeableReadLock(null);
            }
        }

        /// <summary>
        /// ExitWriteLockメソッドテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestExitWriteLockNull()
        {
            using (HashLock lockObject = new HashLock())
            {
                lockObject.ExitWriteLock(null);
            }
        }

        #endregion

        #region その他のメソッドテストケース

        // TestHashLockAllに記述のように、正常系は通しの試験のみ

        /// <summary>
        /// GetReaderWriterLockメソッドテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetReaderWriterLockNull()
        {
            using (HashLock lockObject = new HashLock())
            {
                lockObject.GetReaderWriterLock(null);
            }
        }
        
        #endregion

        #region 全体テストケース

        /// <summary>
        /// HashLock全体テストケース。
        /// </summary>
        /// <remarks>
        /// クラスの内容的に個別のテストが行いにくい内容のため、正常系は通しでの確認のみ。
        /// また、ロックされているかの確認は行わない。
        /// </remarks>
        [Test]
        public void TestHashLockAll()
        {
            string param = "test";
            using(HashLock lockObject = new HashLock())
            {
                // 読み取りロック、他のロックとは排他
                // ※ これもアップグレード可能ロックのように本当はtry-finallyで使う
                lockObject.EnterReadLock(param);
                lockObject.ExitReadLock(param);

                // アップグレード可能ロック
                lockObject.EnterUpgradeableReadLock(param);
                try
                {
                    // 書き込みロック
                    // ※ これもアップグレード可能ロックのように本当はtry-finallyで使う
                    lockObject.EnterWriteLock(param);
                    lockObject.ExitWriteLock(param);
                }
                finally
                {
                    lockObject.ExitUpgradeableReadLock(param);
                }

                // パラメータごとに異なるロックオブジェクトが返る
                // ※ 厳密にはハッシュなので同じになることもある
                Assert.AreNotSame(
                    lockObject.GetReaderWriterLock(param),
                    lockObject.GetReaderWriterLock("test2"));
            }
        }

        #endregion
    }
}
