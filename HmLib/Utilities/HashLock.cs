// ================================================================================================
// <summary>
//      パラメータのハッシュ単位でのロックを提供するクラスソース</summary>
//
// <copyright file="HashLock.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// パラメータのハッシュ単位でのロックを提供するクラスです。
    /// </summary>
    /// <remarks>
    /// <para>
    /// パラメータのハッシュ取得には<see cref="Object.GetHashCode"/>
    /// を、ロック処理には<see cref="ReaderWriterLockSlim"/>を使用します。
    /// </para>
    /// <para>
    /// このオブジェクトはスレッドセーフです。
    /// </para>
    /// </remarks>
    public class HashLock : IDisposable
    {
        #region private変数

        /// <summary>
        /// ハッシュに対応するロックオブジェクト。
        /// </summary>
        private IDictionary<int, ReaderWriterLockSlim> locks = new Dictionary<int, ReaderWriterLockSlim>();

        /// <summary>
        /// <see cref="locks"/>アクセスロック用のロックオブジェクト。
        /// </summary>
        private ReaderWriterLockSlim hashLock = new ReaderWriterLockSlim();

        /// <summary>
        /// ロック再帰ポリシー。
        /// </summary>
        private LockRecursionPolicy recursionPolicy;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ロック再帰ポリシーを指定して、新しいインスタンスを作成する。
        /// </summary>
        /// <param name="recursionPolicy">ロック再帰ポリシー。</param>
        public HashLock(LockRecursionPolicy recursionPolicy)
        {
            this.recursionPolicy = recursionPolicy;
        }

        /// <summary>
        /// デフォルトのロック再帰ポリシーを使用する、新しいインスタンスを作成する。
        /// </summary>
        /// <remarks>ロック再帰ポリシーには<see cref="LockRecursionPolicy.NoRecursion"/>を使用。</remarks>
        public HashLock()
            : this(LockRecursionPolicy.NoRecursion)
        {
        }

        #endregion

        #region デストラクタ

        /// <summary>
        /// オブジェクトのリソースを破棄する。
        /// </summary>
        /// <remarks><see cref="Dispose"/>の呼び出しのみ。</remarks>
        ~HashLock()
        {
            this.Dispose();
        }

        #endregion

        #region パラメータに対応するインスタンスを参照するメソッド

        /// <summary>
        /// パラメータのハッシュ単位で<see cref="ReaderWriterLockSlim.EnterReadLock"/>を行う。
        /// </summary>
        /// <param name="param">ロックの単位となるパラメータ。</param>
        /// <exception cref="ArgumentNullException"><paramref name="param"/>が<c>null</c>。</exception>
        /// <exception cref="LockRecursionException">
        /// ロック再帰ポリシーが<see cref="LockRecursionPolicy.NoRecursion"/>
        /// で再帰的にロックが行われた場合、または再帰が深すぎる場合。
        /// </exception>
        public void EnterReadLock(object param)
        {
            this.GetReaderWriterLock(param).EnterReadLock();
        }

        /// <summary>
        /// パラメータのハッシュ単位で<see cref="ReaderWriterLockSlim.EnterUpgradeableReadLock"/>を行う。
        /// </summary>
        /// <param name="param">ロックの単位となるパラメータ。</param>
        /// <exception cref="ArgumentNullException"><paramref name="param"/>が<c>null</c>。</exception>
        /// <exception cref="LockRecursionException">
        /// ロック再帰ポリシーが<see cref="LockRecursionPolicy.NoRecursion"/>
        /// で再帰的にロックが行われた場合、
        /// または現在のスレッドが読み取りモードでロックに入っている場合、
        /// または再帰が深すぎる場合。
        /// </exception>
        public void EnterUpgradeableReadLock(object param)
        {
            this.GetReaderWriterLock(param).EnterUpgradeableReadLock();
        }

        /// <summary>
        /// パラメータのハッシュ単位で<see cref="ReaderWriterLockSlim.EnterWriteLock"/>を行う。
        /// </summary>
        /// <param name="param">ロックの単位となるパラメータ。</param>
        /// <exception cref="ArgumentNullException"><paramref name="param"/>が<c>null</c>。</exception>
        /// <exception cref="LockRecursionException">
        /// ロック再帰ポリシーが<see cref="LockRecursionPolicy.NoRecursion"/>
        /// で再帰的にロックが行われた場合、
        /// または現在のスレッドが読み取りモードでロックに入っている場合、
        /// または再帰が深すぎる場合。
        /// </exception>
        public void EnterWriteLock(object param)
        {
            this.GetReaderWriterLock(param).EnterWriteLock();
        }

        /// <summary>
        /// パラメータのハッシュ単位で<see cref="ReaderWriterLockSlim.ExitReadLock"/>を行う。
        /// </summary>
        /// <param name="param">ロックの単位となるパラメータ。</param>
        /// <exception cref="ArgumentNullException"><paramref name="param"/>が<c>null</c>。</exception>
        /// <exception cref="SynchronizationLockException">
        /// 現在のスレッドが読み取りモードでロックに入っていない場合。
        /// </exception>
        public void ExitReadLock(object param)
        {
            this.GetReaderWriterLock(param).ExitReadLock();
        }

        /// <summary>
        /// パラメータのハッシュ単位で<see cref="ReaderWriterLockSlim.ExitUpgradeableReadLock"/>を行う。
        /// </summary>
        /// <param name="param">ロックの単位となるパラメータ。</param>
        /// <exception cref="ArgumentNullException"><paramref name="param"/>が<c>null</c>。</exception>
        /// <exception cref="SynchronizationLockException">
        /// 現在のスレッドがアップグレード可能モードでロックに入っていない場合。
        /// </exception>
        public void ExitUpgradeableReadLock(object param)
        {
            this.GetReaderWriterLock(param).ExitUpgradeableReadLock();
        }

        /// <summary>
        /// パラメータのハッシュ単位で<see cref="ReaderWriterLockSlim.ExitWriteLock"/>を行う。
        /// </summary>
        /// <param name="param">ロックの単位となるパラメータ。</param>
        /// <exception cref="ArgumentNullException"><paramref name="param"/>が<c>null</c>。</exception>
        /// <exception cref="SynchronizationLockException">
        /// 現在のスレッドが書き込みモードでロックに入っていない場合。
        /// </exception>
        public void ExitWriteLock(object param)
        {
            this.GetReaderWriterLock(param).ExitWriteLock();
        }

        /// <summary>
        /// パラメータのハッシュに対応する<see cref="ReaderWriterLockSlim"/>を返す。
        /// </summary>
        /// <param name="param">ロックの単位となるパラメータ。</param>
        /// <returns>対応するロックオブジェクト。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="param"/>が<c>null</c>。</exception>
        /// <exception cref="ObjectDisposedException"><see cref="Dispose"/>が実行済みの場合。</exception>
        public ReaderWriterLockSlim GetReaderWriterLock(object param)
        {
            // ロックオブジェクトが解放済みかのチェック（同時にnullになるので代表でhashLock）
            if (this.hashLock == null)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            // 読み取りの同期を取りつつ、ハッシュに対応するロックオブジェクトを取得
            int hashcode = Validate.NotNull(param).GetHashCode();
            ReaderWriterLockSlim lockObject;
            this.hashLock.EnterReadLock();
            try
            {
                if (this.locks.TryGetValue(hashcode, out lockObject))
                {
                    return lockObject;
                }
            }
            finally
            {
                this.hashLock.ExitReadLock();
            }

            // 存在しない場合、まず更新準備の同期を取りつつオブジェクトを再確認
            this.hashLock.EnterUpgradeableReadLock();
            try
            {
                if (this.locks.TryGetValue(hashcode, out lockObject))
                {
                    return lockObject;
                }

                // それでも無ければ、更新の同期を取りつつハッシュに対応するロックオブジェクトを登録
                this.hashLock.EnterWriteLock();
                try
                {
                    lockObject = new ReaderWriterLockSlim(this.recursionPolicy);
                    this.locks[hashcode] = lockObject;
                }
                finally
                {
                    this.hashLock.ExitWriteLock();
                }
            }
            finally
            {
                this.hashLock.ExitUpgradeableReadLock();
            }

            return lockObject;
        }

        #endregion

        #region IDisposableインタフェース実装メソッド

        /// <summary>
        /// 全てのロックオブジェクトを解放する。
        /// </summary>
        public virtual void Dispose()
        {
            // 全てのロックオブジェクトのDisposeを呼び出し
            // ※ Disposeは通常、何度呼ばれてもよいはずだが、.net 4.0現在
            //    ReaderWriterLockSlimのDisposeが二度呼んだときに例外を投げるため、nullも代入
            if (this.hashLock != null)
            {
                this.hashLock.Dispose();
                this.hashLock = null;
            }

            if (this.locks != null)
            {
                foreach (ReaderWriterLockSlim lockObject in this.locks.Values)
                {
                    if (lockObject != null)
                    {
                        lockObject.Dispose();
                    }
                }

                this.locks = null;
            }

            // ファイナライザ（このクラスではDisposeを呼ぶだけ）が不要であることを通知
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
