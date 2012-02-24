// ================================================================================================
// <summary>
//      メモリ上でのキャッシュを扱うためのクラスソース</summary>
//
// <copyright file="MemoryCache.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Models
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Honememo.Utilities;

    /// <summary>
    /// メモリ上でのキャッシュを扱うためのクラスです。
    /// </summary>
    /// <typeparam name="TKey">キャッシュのキーの型。</typeparam>
    /// <typeparam name="TValue">キャッシュ内の値の型。</typeparam>
    /// <remarks>
    /// <para>
    /// キャッシュ数が指定された件数を超過した場合、空き容量が10%を回復するまで
    /// 最終アクセスが古い方からキャッシュを削除します
    /// （キャッシュサイズを条件としたかったが、参照型も含めて使用できる
    /// 汎用的なサイズ取得方法が見つからないため（値型ならsizeofで可能））。
    /// </para>
    /// <para>
    /// インタフェースは実装しないが、メソッド構成は
    /// <see cref="IDictionary&lt;TKey, TValue&gt;"/>にあわせています。
    /// </para>
    /// <para>
    /// このオブジェクトはスレッドセーフです。
    /// ただしキャッシュという目的上、各々のメソッドとしての原子性は実現しますが、
    /// 複数のメソッド間でのタイミング等までは重視しません
    /// （例、キャッシュの登録と読み取り、さらにもう一つ登録を3スレッドから同時に行った場合、
    /// タイミングによっては登録完了の直後でもキャッシュ無しが返ることはありえます。
    /// しかし、キャッシュの登録が失敗したり、壊れたりすることはありません）。
    /// </para>
    /// </remarks>
    public class MemoryCache<TKey, TValue>
    {
        #region 定数

        /// <summary>
        /// デフォルトのキャッシュ最大件数。
        /// </summary>
        private static readonly int DefaultCacheCapacity = 100;

        /// <summary>
        /// キャッシュ件数超過時に確保する空き容量割合 (%)。
        /// </summary>
        private static readonly int CacheCapacityPercentage = 10;

        #endregion

        #region private変数

        /// <summary>
        /// キャッシュ。
        /// </summary>
        private IDictionary<TKey, CacheItem<TValue>> caches = new ConcurrentDictionary<TKey, CacheItem<TValue>>();

        /// <summary>
        /// キャッシュ最大件数。
        /// </summary>
        private int capacity;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたキャッシュ最大件数のインスタンスを作成。
        /// </summary>
        /// <param name="capacity">キャッシュ最大件数。</param>
        /// <exception cref="ArgumentException"><paramref name="capacity"/>が0以下の値。</exception>
        public MemoryCache(int capacity)
        {
            this.Capacity = capacity;
        }

        /// <summary>
        /// デフォルトのキャッシュ最大件数（100件）でインスタンスを作成。
        /// </summary>
        public MemoryCache()
            : this(DefaultCacheCapacity)
        {
        }

        #endregion
        
        #region デリゲート

        /// <summary>
        /// <see cref="GetAndAddIfEmpty"/>
        /// にてキーに対応するキャッシュが登録されていない場合に、
        /// 値を取得する処理を表すデリゲート。
        /// </summary>
        /// <param name="key">キャッシュのキー。</param>
        /// <returns>キーに対応するキャッシュ値。<c>null</c>も可。</returns>
        /// <exception cref="Exception">
        /// 戻り値をキャッシュに登録せずに処理を終了する場合、例外を投げる。
        /// </exception>
        public delegate TValue ReturnCacheValue(TKey key);

        #endregion

        #region プロパティ

        /// <summary>
        /// キャッシュ最大件数。
        /// </summary>
        /// <exception cref="ArgumentException">0以下の値を指定。</exception>
        public virtual int Capacity
        {
            get
            {
                return this.capacity;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("value < 0");
                }

                this.capacity = value;
            }
        }

        /// <summary>
        /// キャッシュ。
        /// </summary>
        /// <remarks>キャッシュ更新時はこのオブジェクトをロックする。</remarks>
        protected virtual IDictionary<TKey, CacheItem<TValue>> Caches
        {
            get
            {
                return this.caches;
            }
        }

        #endregion

        #region インデクサー

        /// <summary>
        /// 指定したキーに対応するキャッシュを取得または設定する。
        /// </summary>
        /// <param name="key">取得または設定するキャッシュのキー。</param>
        /// <returns>指定したキーに対応するキャッシュ。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        /// <exception cref="KeyNotFoundException">キャッシュ内に<paramref name="key"/>が存在しない場合。</exception>
        public virtual TValue this[TKey key]
        {
            get
            {
                // TryGetValueにて処理を実施、ここでは例外処理のみ
                TValue value;
                if (!this.TryGetValue(key, out value))
                {
                    throw new KeyNotFoundException(key + " is not found");
                }

                return value;
            }

            set
            {
                lock (this.Caches)
                {
                    // キャッシュが割り当てられたサイズを超過する場合、
                    // 必要な空き容量が確保できるまで削除する
                    this.RemoveCachesIfOverCapacity(key, value);

                    // アクセス日時に現在時刻を指定してキャッシュを作成、登録
                    CacheItem<TValue> item = new CacheItem<TValue>();
                    item.LastAccessTime = DateTime.UtcNow;
                    item.Value = value;
                    this.Caches[key] = item;
                }
            }
        }

        #endregion

        #region IDictionaryインタフェースにあわせたメソッド

        /// <summary>
        /// 指定したキーと値の組み合わせをキャッシュに追加する。
        /// </summary>
        /// <param name="key">追加するキャッシュのキーとして使用するオブジェクト。</param>
        /// <param name="value">追加するキャッシュの値として使用するオブジェクト。</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentException">同じキーに対応するキャッシュが既に存在する場合。</exception>
        public virtual void Add(TKey key, TValue value)
        {
            lock (this.Caches)
            {
                // 値が既に存在するかをチェックした後、登録
                if (this.ContainsKey(key))
                {
                    throw new ArgumentException(key.ToString() + " is already");
                }

                this[key] = value;
            }
        }

        /// <summary>
        /// 指定したキーに対応するキャッシュが格納されているかどうかを確認する。
        /// </summary>
        /// <param name="key">キャッシュ内で検索されるキー。</param>
        /// <returns>指定したキーに対応するキャッシュを保持している場合は<c>true</c>。それ以外の場合は<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        public virtual bool ContainsKey(TKey key)
        {
            // ※ ConcurrentDictionaryを使用しており、キャッシュ全体に関わるアクセスは
            //    一箇所だけのため、特に読み取りロック等は行わない
            return this.Caches.ContainsKey(key);
        }

        /// <summary>
        /// 指定したキーに対応するキャッシュを削除する。
        /// </summary>
        /// <param name="key">削除するキャッシュのキー。</param>
        /// <returns>
        /// キャッシュが正常に削除された場合は<c>true</c>。それ以外の場合は<c>false</c>。
        /// このメソッドは、<c>key</c>がキャッシュに見つからなかった場合にも<c>false</c>を返します。
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        public virtual bool Remove(TKey key)
        {
            lock (this.Caches)
            {
                // キャッシュを削除
                return this.Caches.Remove(key);
            }
        }

        /// <summary>
        /// 指定したキーに対応するキャッシュを取得する。
        /// </summary>
        /// <param name="key">キャッシュを取得する対象のキー。</param>
        /// <param name="value">
        /// キーが見つかった場合は、指定した対応するキャッシュ。
        /// それ以外の場合は<c>value</c>パラメーターの型に対する既定の値。</param>
        /// <returns>
        /// 指定したキーに対応するキャッシュが格納されている場合は<c>true</c>。
        /// それ以外の場合は<c>false</c>。
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            // ※ ConcurrentDictionaryを使用しており、キャッシュ全体に関わるアクセスは
            //    一箇所だけのため、特に読み取りロック等は行わない
            
            // 返り値をそのクラスのデフォルト値で初期化
            value = default(TValue);

            // アクセス日時を現在時刻で更新しつつキャッシュを返す
            CacheItem<TValue> item;
            if (this.Caches.TryGetValue(key, out item))
            {
                // アクセス日時の更新が被らないようロック
                // ※ ここは個別のキャッシュに対する排他だけが必要なので、
                //    全体のロックとは別に独自にロックする
                lock (item)
                {
                    item.LastAccessTime = DateTime.UtcNow;
                }

                value = item.Value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// キャッシュを空にする。
        /// </summary>
        public virtual void Clear()
        {
            lock (this.Caches)
            {
                // キャッシュの消去
                this.Caches.Clear();
            }
        }

        #endregion

        #region 独自のメソッド

        /// <summary>
        /// 指定したキーに対応するキャッシュを取得する。
        /// </summary>
        /// <param name="key">取得するキャッシュのキー。</param>
        /// <returns>指定したキーに対応するキャッシュ。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        /// <exception cref="KeyNotFoundException">キャッシュ内に<paramref name="key"/>が存在しない場合。</exception>
        public virtual TValue Get(TKey key)
        {
            return this[key];
        }

        /// <summary>
        /// 指定したキーに対応するキャッシュを取得する。
        /// キャッシュが存在しない場合、<paramref name="function"/>
        /// に指定された処理を用いて値を取得し、その結果をキャッシュに登録して返す。
        /// </summary>
        /// <param name="key">取得または設定するキャッシュのキー。</param>
        /// <param name="function">キャッシュが存在しない場合に用いる値の取得処理。</param>
        /// <returns>
        /// 指定したキーに対応するキャッシュ。
        /// キャッシュが存在しない場合、<paramref name="function"/>にて取得した値。
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        /// <exception cref="Exception">
        /// <paramref name="function"/>が例外を投げる場合。
        /// 例外が投げられた場合、キャッシュは登録されない。
        /// </exception>
        public virtual TValue GetAndAddIfEmpty(TKey key, ReturnCacheValue function)
        {
            // キャッシュを取得、この時点では特にロック不要
            TValue value;
            if (this.TryGetValue(key, out value))
            {
                return value;
            }

            // 存在しない場合、まず更新準備の同期を取りつつキャッシュを再確認
            lock (this.Caches)
            {
                if (this.TryGetValue(key, out value))
                {
                    return value;
                }

                // それでも無ければ、書き込みの同期を取りつつ渡されたメソッドを実行。
                // 返された値をキャッシュに登録。
                value = function(key);
                this[key] = value;
            }

            return value;
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// キャッシュ件数が割り当てられた最大件数を超過する場合、
        /// ある程度の空き容量が出来るまで古いキャッシュから削除する。
        /// </summary>
        /// <param name="key">設定するキャッシュのキーとして使用するオブジェクト。</param>
        /// <param name="value">設定するキャッシュの値として使用するオブジェクト。</param>
        /// <remarks>キャッシュを更新するため、呼び出し元で必要なロックを行うこと。</remarks>
        protected virtual void RemoveCachesIfOverCapacity(TKey key, TValue value)
        {
            int count = this.Caches.Count;
            if (count >= this.Capacity)
            {
                // 指定された空きパーセントに件数が減るまで削除する
                int newMax = this.Capacity - (this.Capacity * CacheCapacityPercentage / 100);
                IList<TKey> removes = new List<TKey>();
                foreach (KeyValuePair<TKey, CacheItem<TValue>> pair in this.Caches.OrderBy(pair => pair.Value.LastAccessTime))
                {
                    // 削除予定リストに登録
                    removes.Add(pair.Key);
                    if (--count < newMax)
                    {
                        break;
                    }
                }

                // まとめて削除
                foreach (TKey k in removes)
                {
                    this.Remove(k);
                }
            }
        }

        #endregion

        #region 内部クラス

        /// <summary>
        /// キャッシュの要素を表すクラスです。
        /// </summary>
        /// <typeparam name="T">キャッシュ内の値の型。</typeparam>
        protected class CacheItem<T>
        {
            #region プロパティ

            /// <summary>
            /// キャッシュの値。
            /// </summary>
            public T Value
            {
                get;
                set;
            }

            /// <summary>
            /// キャッシュへの最終アクセス日時。
            /// </summary>
            public DateTime LastAccessTime
            {
                get;
                set;
            }

            #endregion
        }

        #endregion
    }
}
