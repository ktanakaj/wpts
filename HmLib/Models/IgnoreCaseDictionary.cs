// ================================================================================================
// <summary>
//      大文字小文字を区別しないIDictionary実装のラッパークラスソース</summary>
//
// <copyright file="IgnoreCaseDictionary.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Models
{
    using System;
    using System.Collections.Generic;
    using Honememo.Utilities;

    /// <summary>
    /// 大文字小文字を区別しない<see cref="IDictionary&lt;TKey, TValue&gt;"/>実装のラッパークラスです。
    /// </summary>
    /// <typeparam name="TValue">ディクショナリ内の値の型。</typeparam>
    /// <remarks>
    /// このクラスを使用して登録した値が、大文字小文字を区別しないことを保証する
    /// （内部的には大文字小文字を区別した値も保持する。
    /// <see cref="Dictionary"/>プロパティより参照可）。
    /// ただし、既にキー値に大文字小文字が異なるデータを含んでしまっている
    /// <see cref="IDictionary&lt;TKey, TValue&gt;"/>をラップした場合、動作は保証しない。
    /// </remarks>
    public class IgnoreCaseDictionary<TValue> : IDictionary<string, TValue>
    {
        #region private変数

        /// <summary>
        /// ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>実装クラスのインスタンス。
        /// </summary>
        private IDictionary<string, TValue> dictionary;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定された<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスをラップするインスタンスを生成。
        /// </summary>
        /// <param name="dictionary">ラップされるインスタンス。</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/>が<c>null</c>の場合。</exception>
        /// <remarks><paramref name="dictionary"/>に既に大文字小文字違いのキー値が格納されている場合、動作は保障しない。</remarks>
        public IgnoreCaseDictionary(IDictionary<string, TValue> dictionary)
        {
            this.Dictionary = dictionary;
        }

        /// <summary>
        /// 空の<see cref="System.Collections.Generic.Dictionary&lt;TKey, TValue&gt;"/>インスタンスをラップするインスタンスを生成。
        /// </summary>
        public IgnoreCaseDictionary()
            : this(new Dictionary<string, TValue>())
        {
        }

        #endregion

        #region 独自実装公開プロパティ

        /// <summary>
        /// ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>実装クラスのインスタンス。
        /// </summary>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        /// <remarks>
        /// getしたインスタンスへの変更はこのクラスに反映されない。
        /// 必要ならsetで再度インスタンスを読み込ませること。
        /// </remarks>
        public IDictionary<string, TValue> Dictionary
        {
            get
            {
                return this.dictionary;
            }

            set
            {
                // 必須な情報が設定されていない場合、例外を返す
                this.dictionary = Validate.NotNull(value);

                // 大文字小文字を無視した検索用キー情報を作成する
                this.KeyMap = new Dictionary<string, string>();
                foreach (string key in this.dictionary.Keys)
                {
                    this.KeyMap[key.ToLower()] = key;
                }
            }
        }

        #endregion

        #region ラップするインスタンスを参照するプロパティ

        /// <summary>
        /// ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの<see cref="IDictionary&lt;TKey, TValue&gt;.Keys"/>プロパティを呼び出す。
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return this.Dictionary.Keys;
            }
        }

        /// <summary>
        /// ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの<see cref="IDictionary&lt;TKey, TValue&gt;.Values"/>プロパティを呼び出す。
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                return this.Dictionary.Values;
            }
        }

        /// <summary>
        /// ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの<see cref="ICollection&lt;T&gt;.Count"/>プロパティを呼び出す。
        /// </summary>
        public int Count
        {
            get
            {
                return this.Dictionary.Count;
            }
        }

        /// <summary>
        /// ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの<see cref="ICollection&lt;T&gt;.IsReadOnly"/>プロパティを呼び出す。
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return this.Dictionary.IsReadOnly;
            }
        }

        #endregion

        #region 内部実装用プロパティ

        /// <summary>
        /// 大文字小文字を無視したキー情報を格納するためのマップ。
        /// </summary>
        /// <remarks>
        /// 小文字変換後のキーと変換前のキーのマップ。
        /// <see cref="Dictionary"/>プロパティと同期を取る必要がある。
        /// </remarks>
        protected IDictionary<string, string> KeyMap
        {
            get;
            set;
        }

        #endregion

        #region 独自実装インデクサー

        /// <summary>
        /// 大文字小文字を区別せず、ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスのインデクサーを呼び出す。
        /// </summary>
        /// <param name="key">取得または設定する要素のキー。</param>
        /// <returns>指定したキーを持つ要素。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        /// <exception cref="KeyNotFoundException">プロパティが取得されたが、コレクション内に<paramref name="key"/>が存在しない場合。</exception>
        public TValue this[string key]
        {
            get
            {
                // 小文字に変換し、マップを経てラップインスタンスにアクセス
                return this.Dictionary[this.KeyMap[Validate.NotNull(key, "key").ToLower()]];
            }

            set
            {
                // 小文字に変換し、マップを経てラップインスタンスにアクセス
                string k = Validate.NotNull(key, "key").ToLower();
                string orgKey;
                if (this.KeyMap.TryGetValue(k, out orgKey))
                {
                    // 大文字小文字が違う可能性があるので、
                    // オリジナルデータは一旦削除して再登録する
                    this.Dictionary.Remove(orgKey);
                }

                this.KeyMap[k] = key;
                this.Dictionary[key] = value;
            }
        }

        #endregion

        #region 独自実装メソッド

        /// <summary>
        /// 大文字小文字を区別せず、ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Add"/>メソッドを呼び出す。
        /// </summary>
        /// <param name="key">追加する要素のキーとして使用するオブジェクト。</param>
        /// <param name="value">追加する要素の値として使用するオブジェクト。</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentException">同じキーを持つ要素が既に存在する場合。</exception>
        public void Add(string key, TValue value)
        {
            // 小文字に変換し、マップを経てラップインスタンスにアクセス
            string k = Validate.NotNull(key, "key").ToLower();
            this.KeyMap.Add(k, key);
            this.Dictionary.Add(key, value);
        }

        /// <summary>
        /// 大文字小文字を区別せず、指定したキーの要素がラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスに格納されているかどうかを確認する。
        /// </summary>
        /// <param name="key">ディクショナリ内で検索されるキー。</param>
        /// <returns>指定したキーを持つ要素をディクショナリが保持している場合は<c>true</c>。それ以外の場合は<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        public bool ContainsKey(string key)
        {
            // 同期が取れていることを前提に、キーマップのみ確認する
            // （ラップインスタンスまで見ると、他の例外が起こりえて面倒なため）
            return this.KeyMap.ContainsKey(Validate.NotNull(key, "key").ToLower());
        }

        /// <summary>
        /// 大文字小文字を区別せず、ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.Remove"/>メソッドを呼び出す。
        /// </summary>
        /// <param name="key">削除する要素のキー。</param>
        /// <returns>
        /// 要素が正常に削除された場合は<c>true</c>。それ以外の場合は<c>false</c>。
        /// このメソッドは、<paramref name="key"/>が元のディクショナリに見つからなかった場合にも<c>false</c>を返します。
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        public bool Remove(string key)
        {
            // 小文字に変換し、マップを経てラップインスタンスにアクセス
            string k = Validate.NotNull(key, "key").ToLower();
            string orgKey;
            bool removed = false;
            if (this.KeyMap.TryGetValue(k, out orgKey))
            {
                removed = this.Dictionary.Remove(orgKey);
            }

            // どちらかが削除できていれば削除成功と返す
            return this.KeyMap.Remove(k) || removed;
        }

        /// <summary>
        /// 大文字小文字を区別せず、ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの
        /// <see cref="IDictionary&lt;TKey, TValue&gt;.TryGetValue"/>メソッドを呼び出す。
        /// </summary>
        /// <param name="key">値を取得する対象のキー。</param>
        /// <param name="value">
        /// キーが見つかった場合は、指定したキーに関連付けられている値。
        /// それ以外の場合は<c>value</c>パラメーターの型に対する既定の値。</param>
        /// <returns>
        /// 指定したキーを持つ要素がディクショナリに格納されている場合は<c>true</c>。
        /// それ以外の場合は<c>false</c>。
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/>が<c>null</c>の場合。</exception>
        public bool TryGetValue(string key, out TValue value)
        {
            // 返り値をそのクラスのデフォルト値で初期化
            value = default(TValue);

            // 小文字に変換し、マップを経てラップインスタンスにアクセス
            string k = Validate.NotNull(key, "key").ToLower();
            string orgKey;
            if (this.KeyMap.TryGetValue(k, out orgKey))
            {
                return this.Dictionary.TryGetValue(orgKey, out value);
            }

            return false;
        }

        /// <summary>
        /// キーの大文字小文字を区別せず、ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの
        /// <see cref="ICollection&lt;T&gt;.Add"/>メソッドを呼び出す。
        /// </summary>
        /// <param name="item">ディクショナリに追加するオブジェクト。</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/>の<c>key</c>が<c>null</c>の場合。</exception>
        public void Add(KeyValuePair<string, TValue> item)
        {
            // 小文字に変換し、マップを経てラップインスタンスにアクセス
            string k = Validate.NotNull(item.Key, "item.key").ToLower();
            this.KeyMap.Add(k, item.Key);
            this.Dictionary.Add(item);
        }

        /// <summary>
        /// ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの<see cref="ICollection&lt;T&gt;.Clear"/>メソッドを呼び出す。
        /// </summary>
        public void Clear()
        {
            this.KeyMap.Clear();
            this.Dictionary.Clear();
        }

        /// <summary>
        /// キーの大文字小文字を区別せず、ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>
        /// インスタンスに特定のキーと値が格納されているかどうかを判断します。
        /// </summary>
        /// <param name="item">ディクショナリ内で検索される<see cref="KeyValuePair&lt;TKey, TValue&gt;"/>構造体。</param>
        /// <returns><paramref name="item"/>がディクショナリに存在する場合は<c>true</c>。それ以外の場合は<c>false</c>。</returns>
        public bool Contains(KeyValuePair<string, TValue> item)
        {
            // 大文字小文字を区別しないキーと、値で判定する
            if (item.Key == null)
            {
                // キー情報がnullの場合、このクラスでは検知できないのでNG
                return false;
            }

            TValue value;
            if (this.TryGetValue(item.Key, out value))
            {
                // キーから辿った値が一致していればOK
                return ObjectUtils.Equals(item.Value, value);
            }

            // どこにも該当しなければ不一致なのでNG
            return false;
        }

        /// <summary>
        /// ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの要素を配列にコピーします。配列の特定のインデックスからコピーが開始されます。
        /// </summary>
        /// <param name="array">ディクショナリから要素がコピーされる1次元の配列。配列には、0から始まるインデックス番号が必要です。</param>
        /// <param name="arrayIndex">コピーの開始位置となる、<paramref name="array"/>の0から始まるインデックス。</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/>が0未満の場合。</exception>
        /// <exception cref="ArgumentException">
        /// コピー元のディクショナリの要素数が、コピー先の<paramref name="array"/>の<paramref name="arrayIndex"/>から最後までの領域を超えている場合。
        /// </exception>
        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            // 入力値チェック
            Validate.NotNull(array, "array");
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if (array.Length - arrayIndex < this.Count)
            {
                throw new ArgumentException("array.Length - arrayIndex < this.Count");
            }

            // このコレクションが保持している情報をコピーする
            int i = arrayIndex;
            foreach (KeyValuePair<string, TValue> item in this)
            {
                array[i++] = item;
            }
        }

        /// <summary>
        /// ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンス内で最初に見つかった特定のオブジェクトを削除します。
        /// </summary>
        /// <param name="item">ディクショナリから削除するオブジェクト。</param>
        /// <returns>
        /// <paramref name="item"/>がディクショナリから正常に削除された場合は<c>true</c>。それ以外の場合は<c>false</c>。
        /// このメソッドは、<paramref name="item"/>が元のディクショナリに見つからなかった場合にも<c>false</c>を返します。
        /// </returns>
        public bool Remove(KeyValuePair<string, TValue> item)
        {
            // 多少処理が冗長にはなるものの、下記メソッドを呼ぶことで実装
            return this.Contains(item) && this.Remove(item.Key);
        }

        #endregion

        #region ラップするインスタンスを参照するメソッド
        
        /// <summary>
        /// ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの<see cref="IEnumerable&lt;T&gt;.GetEnumerator"/>メソッドを呼び出す。
        /// </summary>
        /// <returns>コレクションを反復処理するために使用できる<see cref="IEnumerator&lt;T&gt;"/>オブジェクト。</returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return this.Dictionary.GetEnumerator();
        }

        /// <summary>
        /// ラップする<see cref="IDictionary&lt;TKey, TValue&gt;"/>インスタンスの<see cref="System.Collections.IEnumerable.GetEnumerator"/>メソッドを呼び出す。
        /// </summary>
        /// <returns>コレクションを反復処理するために使用できる<see cref="System.Collections.IEnumerator"/>オブジェクト。</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Dictionary.GetEnumerator();
        }

        #endregion
    }
}
