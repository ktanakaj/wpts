// ================================================================================================
// <summary>
//      大文字小文字を区別しないIDictionary実装のラッパークラスソース</summary>
//
// <copyright file="IgnoreCaseDictionary.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Models
{
    using System;
    using System.Collections.Generic;
    using Honememo.Utilities;

    /// <summary>
    /// 大文字小文字を区別しない<c>IDictionary</c>実装のラッパークラスです。
    /// </summary>
    /// <typeparam name="TValue">ディクショナリ内の値の型。</typeparam>
    /// <remarks>
    /// このクラスを使用して登録した値が、大文字小文字を区別しないことを保証する。
    /// （内部的には大文字小文字を区別した値も保持する。
    /// <c>Dictionary</c>プロパティより参照可。）
    /// ただし、既にキー値に大文字小文字が異なるデータを含んでしまっている
    /// <c>IDictionary</c>をラップした場合、動作は保証しない。
    /// </remarks>
    public class IgnoreCaseDictionary<TValue> : IDictionary<string, TValue>
    {
        #region private変数

        /// <summary>
        /// ラップする<c>IDictionary</c>実装クラスのインスタンス。
        /// </summary>
        private IDictionary<string, TValue> dictionary;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定された<c>IDictionary</c>インスタンスをラップするインスタンスを生成。
        /// </summary>
        /// <param name="dictionary">ラップされるインスタンス。</param>
        /// <remarks>インスタンスに大文字小文字違いのキー値が格納されている場合、動作は保障しない。</remarks>
        public IgnoreCaseDictionary(IDictionary<string, TValue> dictionary)
        {
            this.Dictionary = dictionary;
        }

        /// <summary>
        /// 空の<c>Dictionary</c>インスタンスをラップするインスタンスを生成。
        /// </summary>
        public IgnoreCaseDictionary()
            : this(new Dictionary<string, TValue>())
        {
        }

        #endregion

        #region 独自実装公開プロパティ

        /// <summary>
        /// ラップする<c>IDictionary</c>実装クラスのインスタンス。
        /// </summary>
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
        /// ラップする<c>IDictionary</c>インスタンスの<c>Keys</c>プロパティを呼び出す。
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return this.Dictionary.Keys;
            }
        }

        /// <summary>
        /// ラップする<c>IDictionary</c>インスタンスの<c>Values</c>プロパティを呼び出す。
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                return this.Dictionary.Values;
            }
        }

        /// <summary>
        /// ラップする<c>IDictionary</c>インスタンスの<c>Count</c>プロパティを呼び出す。
        /// </summary>
        public int Count
        {
            get
            {
                return this.Dictionary.Count;
            }
        }

        /// <summary>
        /// ラップする<c>IDictionary</c>インスタンスの<c>IsReadOnly</c>プロパティを呼び出す。
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
        /// 大文字小文字を無視したキー情報を格納するための<c>IDictionary</c>。
        /// </summary>
        /// <remarks>
        /// 小文字変換後のキーと変換前のキーのマップ。
        /// <c>Dictionary</c>プロパティと同期を取る必要がある。
        /// </remarks>
        protected IDictionary<string, string> KeyMap
        {
            get;
            set;
        }

        #endregion

        #region 独自実装インデクサー

        /// <summary>
        /// 大文字小文字を区別せず、ラップする<c>IDictionary</c>インスタンスの<c>Item</c>プロパティを呼び出す。
        /// </summary>
        /// <param name="key">取得または設定する要素のキー。</param>
        /// <returns>指定したキーを持つ要素。</returns>
        public TValue this[string key]
        {
            get
            {
                // 小文字に変換し、マップを経てラップインスタンスにアクセス
                // ※ nullの場合は事前にArgumentNullExceptionを投げる
                //    またKeyがなければKeyNotFoundExceptionが飛ぶはず
                return this.Dictionary[this.KeyMap[Validate.NotNull(key).ToLower()]];
            }

            set
            {
                // 小文字に変換し、マップを経てラップインスタンスにアクセス
                // ※ nullの場合は事前にArgumentNullExceptionを投げる
                //    またKeyがなければKeyNotFoundExceptionが飛ぶはず
                string k = Validate.NotNull(key).ToLower();
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
        /// 大文字小文字を区別せず、ラップする<c>IDictionary</c>インスタンスの<c>Add</c>メソッドを呼び出す。
        /// </summary>
        /// <param name="key">追加する要素のキーとして使用するオブジェクト。</param>
        /// <param name="value">追加する要素の値として使用するオブジェクト。</param>
        public void Add(string key, TValue value)
        {
            // 小文字に変換し、マップを経てラップインスタンスにアクセス
            // ※ nullの場合は事前にArgumentNullExceptionを投げる
            string k = Validate.NotNull(key).ToLower();
            this.KeyMap.Add(k, key);
            this.Dictionary.Add(key, value);
        }

        /// <summary>
        /// 大文字小文字を区別せず、指定したキーの要素が<c>IgnoreCaseDictionary</c>に格納されているかどうかを確認します。
        /// </summary>
        /// <param name="key"><c>IgnoreCaseDictionary</c>内で検索されるキー。</param>
        /// <returns>指定したキーを持つ要素を<c>IgnoreCaseDictionary</c>が保持している場合は<c>true</c>。それ以外の場合は<c>false</c>。</returns>
        public bool ContainsKey(string key)
        {
            // 同期が取れていることを前提に、キーマップのみ確認する
            // （ラップインスタンスまで見ると、他の例外が起こりえて面倒なため）
            // ※ nullの場合は事前にArgumentNullExceptionを投げる
            return this.KeyMap.ContainsKey(Validate.NotNull(key).ToLower());
        }

        /// <summary>
        /// 大文字小文字を区別せず、ラップする<c>IDictionary</c>インスタンスの<c>Remove</c>メソッドを呼び出す。
        /// </summary>
        /// <param name="key">削除する要素のキー。</param>
        /// <returns>
        /// 要素が正常に削除された場合は<c>true</c>。それ以外の場合は<c>false</c>。
        /// このメソッドは、<c>key</c>が元の<c>IDictionary</c>に見つからなかった場合にも<c>false</c>を返します。
        /// </returns>
        public bool Remove(string key)
        {
            // 小文字に変換し、マップを経てラップインスタンスにアクセス
            // ※ nullの場合は事前にArgumentNullExceptionを投げる
            string k = Validate.NotNull(key).ToLower();
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
        /// 大文字小文字を区別せず、ラップする<c>IDictionary</c>インスタンスの<c>TryGetValue</c>メソッドを呼び出す。
        /// </summary>
        /// <param name="key">値を取得する対象のキー。</param>
        /// <param name="value">
        /// このメソッドが返されるときに、キーが見つかった場合は、指定したキーに関連付けられている値。
        /// それ以外の場合は<c>value</c>パラメーターの型に対する既定の値。このパラメーターは初期化せずに渡されます。</param>
        /// <returns>
        /// 指定したキーを持つ要素が<c>IDictionary</c>を実装するオブジェクトに格納されている場合は<c>true</c>。
        /// それ以外の場合は<c>false</c>。
        /// </returns>
        public bool TryGetValue(string key, out TValue value)
        {
            // 返り値をそのクラスのデフォルト値で初期化
            value = default(TValue);

            // 小文字に変換し、マップを経てラップインスタンスにアクセス
            // ※ nullの場合は事前にArgumentNullExceptionを投げる
            string k = Validate.NotNull(key).ToLower();
            string orgKey;
            if (this.KeyMap.TryGetValue(k, out orgKey))
            {
                return this.Dictionary.TryGetValue(orgKey, out value);
            }

            return false;
        }

        /// <summary>
        /// キーの大文字小文字を区別せず、ラップする<c>IDictionary</c>インスタンスの<c>Add</c>メソッドを呼び出す。
        /// </summary>
        /// <param name="item"><c>ICollection</c>に追加するオブジェクト。</param>
        public void Add(KeyValuePair<string, TValue> item)
        {
            // 小文字に変換し、マップを経てラップインスタンスにアクセス
            // ※ nullの場合は事前にArgumentNullExceptionを投げる
            string k = Validate.NotNull(item.Key).ToLower();
            this.KeyMap.Add(k, item.Key);
            this.Dictionary.Add(item);
        }

        /// <summary>
        /// ラップする<c>IDictionary</c>インスタンスの<c>Clear</c>メソッドを呼び出す。
        /// </summary>
        public void Clear()
        {
            this.KeyMap.Clear();
            this.Dictionary.Clear();
        }

        /// <summary>
        /// キーの大文字小文字を区別せず、<c>ICollection</c>に特定のキーと値が格納されているかどうかを判断します。
        /// </summary>
        /// <param name="item"><c>ICollection</c>内で検索される<c>KeyValuePair</c>構造体。</param>
        /// <returns><c>KeyValuePair</c>が<c>ICollection</c>に存在する場合は<c>true</c>。それ以外の場合は<c>false</c>。</returns>
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
        /// <c>ICollection</c>の要素をArrayにコピーします。Arrayの特定のインデックスからコピーが開始されます。
        /// </summary>
        /// <param name="array"><c>ICollection</c>から要素がコピーされる1次元のArray。Arrayには、0から始まるインデックス番号が必要です。</param>
        /// <param name="arrayIndex">コピーの開始位置となる、arrayの0から始まるインデックス。</param>
        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            // 入力値チェック
            Validate.NotNull(array);
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if (array.Length >= arrayIndex)
            {
                throw new ArgumentException("array.Length >= arrayIndex");
            }

            // 渡された情報をコピーする
            for (int i = arrayIndex; i < array.Length; i++)
            {
                this[array[i].Key] = array[i].Value;
            }
        }

        /// <summary>
        /// <c>ICollection</c>内で最初に見つかった特定のオブジェクトを削除します。
        /// </summary>
        /// <param name="item"><c>ICollection</c>から削除するオブジェクト。</param>
        /// <returns>
        /// itemが<c>ICollection</c>から正常に削除された場合は<c>true</c>。それ以外の場合は<c>false</c>。
        /// このメソッドは、<c>item</c>が元の<c>ICollection</c>に見つからなかった場合にも<c>false</c>を返します。
        /// </returns>
        public bool Remove(KeyValuePair<string, TValue> item)
        {
            // 多少処理が冗長にはなるものの、下記メソッドを呼ぶことで実装
            return this.Contains(item) && this.Remove(item.Key);
        }

        #endregion

        #region ラップするインスタンスを参照するメソッド
        
        /// <summary>
        /// ラップする<c>IDictionary</c>インスタンスの<c>GetEnumerator</c>メソッドを呼び出す。
        /// </summary>
        /// <returns>コレクションを反復処理するために使用できる<c>IEnumerator</c>オブジェクト。</returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return this.Dictionary.GetEnumerator();
        }

        /// <summary>
        /// ラップする<c>IDictionary</c>インスタンスの<c>GetEnumerator</c>メソッドを呼び出す。
        /// </summary>
        /// <returns>コレクションを反復処理するために使用できる<c>IEnumerator</c>オブジェクト。</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Dictionary.GetEnumerator();
        }

        #endregion
    }
}
