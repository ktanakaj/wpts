// ================================================================================================
// <summary>
//      ロック用のオブジェクトを提供するクラスソース</summary>
//
// <copyright file="LockObject.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// ロック用のオブジェクトを提供するクラスです。
    /// </summary>
    /// <remarks>このクラスはスレッドセーフです。</remarks>
    public class LockObject
    {
        #region private変数

        /// <summary>
        /// ハッシュに対応するロックオブジェクト。
        /// </summary>
        private IDictionary<int, object> locks = new ConcurrentDictionary<int, object>();

        #endregion

        #region パラメータ単位のロック用メソッド
        
        /// <summary>
        /// パラメータのハッシュに対応するロックオブジェクトを返す。
        /// </summary>
        /// <param name="param">ロックの単位となるパラメータ。</param>
        /// <returns>対応するロックオブジェクト。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="param"/>が<c>null</c>。</exception>
        /// <remarks>
        /// パラメータのハッシュ取得には<see cref="Object.GetHashCode"/>を使用する。
        /// </remarks>
        public object GetObject(object param)
        {
            // ロックオブジェクトを取得、この時点では特にロック不要
            // ※ ConcurrentDictionaryを使用しており、一回の処理で更新しているため
            int hashcode = Validate.NotNull(param, "param").GetHashCode();
            object lockObject;
            if (this.locks.TryGetValue(hashcode, out lockObject))
            {
                return lockObject;
            }

            // 存在しない場合、ロックを行い念のためキャッシュを再確認
            lock (this.locks)
            {
                if (this.locks.TryGetValue(hashcode, out lockObject))
                {
                    return lockObject;
                }

                // それでも無ければ、ハッシュに対応するロックオブジェクトを作成して返す
                lockObject = new object();
                this.locks[hashcode] = lockObject;
            }

            return lockObject;
        }

        #endregion
    }
}
