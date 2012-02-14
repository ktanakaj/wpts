// ================================================================================================
// <summary>
//      大文字小文字を区別しないISet実装のラッパークラスソース</summary>
//
// <copyright file="IgnoreCaseSet.cs" company="honeplusのメモ帳">
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
    /// 大文字小文字を区別しない<see cref="ISet&lt;T&gt;"/>実装のラッパークラスです。
    /// </summary>
    /// <remarks>
    /// このクラスを使用して登録した値が、大文字小文字を区別しないことを保証する。
    /// （内部的には大文字小文字を区別した値も保持する。
    /// <c>Set</c>プロパティより参照可。）
    /// ただし、既に大文字小文字が異なるデータを含んでしまっている
    /// <c>ISet</c>をラップした場合、動作は保証しない。
    /// このクラスでは<c>null</c>要素を許容しない。
    /// </remarks>
    public class IgnoreCaseSet : ISet<string>
    {
        #region private変数

        /// <summary>
        /// ラップする<c>ISet</c>実装クラスのインスタンス。
        /// </summary>
        private ISet<string> set;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定された<see cref="ISet&lt;T&gt;"/>インスタンスをラップするインスタンスを生成。
        /// </summary>
        /// <param name="set">ラップされるインスタンス。</param>
        /// <exception cref="ArgumentNullException"><para>set</para>が<c>null</c>の場合。</exception>
        /// <remarks><para>set</para>に既に大文字小文字違いの値が格納されている場合、動作は保障しない。</remarks>
        public IgnoreCaseSet(ISet<string> set)
        {
            this.Set = set;
        }

        /// <summary>
        /// 空の<see cref="HashSet&lt;T&gt;"/>インスタンスをラップするインスタンスを生成。
        /// </summary>
        public IgnoreCaseSet()
            : this(new HashSet<string>())
        {
        }

        /// <summary>
        /// 指定されたコレクションからコピーされた要素を格納するインスタンスを生成。
        /// </summary>
        /// <param name="collection">新しいセットの要素のコピー元となるコレクション。</param>
        /// <exception cref="ArgumentNullException"><para>collection</para>が<c>null</c>の場合。</exception>
        public IgnoreCaseSet(IEnumerable<string> collection) : this()
        {
            foreach (string s in Validate.NotNull(collection))
            {
                this.Add(s);
            }
        }

        #endregion

        #region 独自実装公開プロパティ

        /// <summary>
        /// ラップする<c>ISet</c>実装クラスのインスタンス。
        /// </summary>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        /// <remarks>
        /// getしたインスタンスへの変更はこのクラスに反映されない。
        /// 必要ならsetで再度インスタンスを読み込ませること。
        /// </remarks>
        public ISet<string> Set
        {
            get
            {
                return this.set;
            }

            set
            {
                // 必須な情報が設定されていない場合、例外を返す
                this.set = Validate.NotNull(value);

                // 大文字小文字を無視した検索用キー情報を作成する
                this.KeyMap = new Dictionary<string, string>();
                foreach (string key in this.set)
                {
                    this.KeyMap[key.ToLower()] = key;
                }
            }
        }

        #endregion

        #region ラップするインスタンスを参照するプロパティ

        /// <summary>
        /// ラップする<c>ISet</c>インスタンスの<c>Count</c>プロパティを呼び出す。
        /// </summary>
        public int Count
        {
            get
            {
                return this.Set.Count;
            }
        }

        /// <summary>
        /// ラップする<c>ISet</c>インスタンスの<c>IsReadOnly</c>プロパティを呼び出す。
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return this.Set.IsReadOnly;
            }
        }

        #endregion

        #region 内部実装用プロパティ

        /// <summary>
        /// 大文字小文字を無視したキー情報を格納するための<c>IDictionary</c>。
        /// </summary>
        /// <remarks>
        /// 小文字変換後の値と変換前の値のマップ。
        /// <c>Set</c>プロパティと同期を取る必要がある。
        /// </remarks>
        protected IDictionary<string, string> KeyMap
        {
            get;
            set;
        }

        #endregion
        
        #region 独自実装メソッド

        /// <summary>
        /// 大文字小文字を区別せず、ラップする<c>ISet</c>インスタンスの<c>Add</c>メソッドを呼び出す。
        /// </summary>
        /// <param name="item">セットに追加する要素。</param>
        /// <returns>要素がセットに追加された場合は<c>true</c>。セットに既存の要素が存在していた場合は<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException"><para>key</para>が<c>null</c>の場合。</exception>
        public bool Add(string item)
        {
            // 小文字に変換し、マップを経てラップインスタンスにアクセス
            string k = Validate.NotNull(item).ToLower();
            string orgItem;
            bool contains = this.KeyMap.TryGetValue(k, out orgItem);
            if (contains)
            {
                // 大文字小文字が違う可能性があるので、
                // オリジナルデータは一旦削除して再登録する
                this.Set.Remove(orgItem);
            }

            this.KeyMap[k] = item;
            this.Set.Add(item);
            return !contains;
        }

        /// <summary>
        /// 大文字小文字を区別せず、ラップする<c>ISet</c>インスタンスの<c>Add</c>メソッドを呼び出す。
        /// </summary>
        /// <param name="item"><c>ICollection</c>に追加する要素。</param>
        /// <exception cref="ArgumentNullException"><para>item.key</para>が<c>null</c>の場合。</exception>
        void System.Collections.Generic.ICollection<string>.Add(string item)
        {
            this.Add(item);
        }

        /// <summary>
        /// 大文字小文字を区別せず、<c>ICollection</c>に特定の要素が格納されているかどうかを判断します。
        /// </summary>
        /// <param name="item"><c>ICollection</c>内で検索する文字列。</param>
        /// <returns><para>item</para>が<c>ICollection</c>に存在する場合は<c>true</c>。それ以外の場合は<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException"><para>key</para>が<c>null</c>の場合。</exception>
        public bool Contains(string item)
        {
            // 同期が取れていることを前提に、キーマップのみ確認する
            return this.KeyMap.ContainsKey(Validate.NotNull(item).ToLower());
        }

        /// <summary>
        /// ラップする<c>ISet</c>インスタンスの<c>Clear</c>メソッドを呼び出す。
        /// </summary>
        public void Clear()
        {
            this.KeyMap.Clear();
            this.Set.Clear();
        }

        /// <summary>
        /// <c>ICollection</c>の要素をArrayにコピーします。Arrayの特定のインデックスからコピーが開始されます。
        /// </summary>
        /// <param name="array"><c>ICollection</c>から要素がコピーされる1次元のArray。Arrayには、0から始まるインデックス番号が必要です。</param>
        /// <param name="arrayIndex">コピーの開始位置となる、arrayの0から始まるインデックス。</param>
        /// <exception cref="ArgumentNullException"><para>array</para>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>arrayIndex</para>が0未満の場合。</exception>
        /// <exception cref="ArgumentException">コピー元の<c>ICollection</c>の要素数が、コピー先の<para>array</para>の<para>arrayIndex</para>から最後までの領域を超えている場合。</exception>
        public void CopyTo(string[] array, int arrayIndex)
        {
            // 入力値チェック
            Validate.NotNull(array);
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
            foreach (string item in this)
            {
                array[i++] = item;
            }
        }

        /// <summary>
        /// 大文字小文字を区別せず、現在のセットから、指定されたコレクションに含まれる要素をすべて削除します。
        /// </summary>
        /// <param name="other">セットから削除する項目のコレクション。</param>
        /// <exception cref="ArgumentNullException"><para>other</para>が<c>null</c>の場合。</exception>
        public void ExceptWith(IEnumerable<string> other)
        {
            foreach (string s in Validate.NotNull(other))
            {
                this.Remove(s);
            }
        }

        /// <summary>
        /// 大文字小文字を区別せず、指定されたコレクションに存在する要素だけが含まれるように現在のセットを変更します。
        /// </summary>
        /// <param name="other">現在のセットと比較するコレクション。</param>
        /// <exception cref="ArgumentNullException"><para>other</para>が<c>null</c>の場合。</exception>
        public void IntersectWith(IEnumerable<string> other)
        {
            // 削除対象をリストアップしてまとめてExceptWithで消す
            // ※ ExceptWithを逆向きに使うことも考えられるが、それだとラップしているISet内の
            //    大文字小文字違いが保持されないので。
            IList<string> removeList = new List<string>();
            IgnoreCaseSet diff = new IgnoreCaseSet(other);
            foreach (string s in this)
            {
                if (!diff.Contains(s))
                {
                    removeList.Add(s);
                }
            }

            this.ExceptWith(removeList);
        }

        /// <summary>
        /// 大文字小文字を区別せず、現在のセットが、指定されたコレクションの真のサブセット（真部分集合）であるかどうかを判断します。
        /// </summary>
        /// <param name="other">現在のセットと比較するコレクション。</param>
        /// <returns>現在のセットが<para>other</para>パラメーターの真のサブセットの場合は<c>true</c>。それ以外の場合は<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException"><para>other</para>が<c>null</c>の場合。</exception>
        public bool IsProperSubsetOf(IEnumerable<string> other)
        {
            return new IgnoreCaseSet(other).IsProperSupersetOf(this);
        }

        /// <summary>
        /// 大文字小文字を区別せず、現在のセットが、指定されたコレクションの真のスーパーセット（真上位集合）であるかどうかを判断します。
        /// </summary>
        /// <param name="other">現在のセットと比較するコレクション。</param>
        /// <returns>現在のセットが<para>other</para>パラメーターの真のスーパーセットの場合は<c>true</c>。それ以外の場合は<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException"><para>other</para>が<c>null</c>の場合。</exception>
        public bool IsProperSupersetOf(IEnumerable<string> other)
        {
            int count = 0;
            foreach (string s in Validate.NotNull(other))
            {
                ++count;
                if (!this.Contains(s))
                {
                    return false;
                }
            }

            return this.Count > count;
        }

        /// <summary>
        /// 大文字小文字を区別せず、現在のセットが、指定されたコレクションのサブセットであるかどうかを判断します。
        /// </summary>
        /// <param name="other">現在のセットと比較するコレクション。</param>
        /// <returns>現在のセットが<para>other</para>パラメーターのサブセットの場合は<c>true</c>。それ以外の場合は<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException"><para>other</para>が<c>null</c>の場合。</exception>
        public bool IsSubsetOf(IEnumerable<string> other)
        {
            return new IgnoreCaseSet(other).IsSupersetOf(this);
        }

        /// <summary>
        /// 大文字小文字を区別せず、現在のセットが、指定されたコレクションのスーパーセットであるかどうかを判断します。
        /// </summary>
        /// <param name="other">現在のセットと比較するコレクション。</param>
        /// <returns>現在のセットが<para>other</para>パラメーターのスーパーセットの場合は<c>true</c>。それ以外の場合は<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException"><para>other</para>が<c>null</c>の場合。</exception>
        public bool IsSupersetOf(IEnumerable<string> other)
        {
            int count = 0;
            foreach (string s in Validate.NotNull(other))
            {
                ++count;
                if (!this.Contains(s))
                {
                    return false;
                }
            }

            return this.Count >= count;
        }

        /// <summary>
        /// 大文字小文字を区別せず、現在のセットと、指定されたコレクションとで重なり合う部分が存在するかどうかを判断します。
        /// </summary>
        /// <param name="other">現在のセットと比較するコレクション。</param>
        /// <returns>現在のセットと<para>other</para>との間に共通する要素が1つでも存在する場合<c>true</c>。それ以外の場合は<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException"><para>other</para>が<c>null</c>の場合。</exception>
        public bool Overlaps(IEnumerable<string> other)
        {
            foreach (string s in Validate.NotNull(other))
            {
                if (this.Contains(s))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 大文字小文字を区別せず、ラップする<c>ISet</c>インスタンスの<c>Remove</c>メソッドを呼び出す。
        /// </summary>
        /// <param name="item">削除する要素。</param>
        /// <returns>
        /// 要素が正常に削除された場合は<c>true</c>。それ以外の場合は<c>false</c>。
        /// このメソッドは、<para>item</para>が元の<c>ISet</c>に見つからなかった場合にも<c>false</c>を返します。
        /// </returns>
        /// <exception cref="ArgumentNullException"><para>item</para>が<c>null</c>の場合。</exception>
        public bool Remove(string item)
        {
            // 小文字に変換し、マップを経てラップインスタンスにアクセス
            string k = Validate.NotNull(item).ToLower();
            string orgItem;
            bool removed = false;
            if (this.KeyMap.TryGetValue(k, out orgItem))
            {
                removed = this.Set.Remove(orgItem);
            }

            // どちらかが削除できていれば削除成功と返す
            return this.KeyMap.Remove(k) || removed;
        }

        /// <summary>
        /// 大文字小文字を区別せず、現在のセットと指定されたコレクションに同じ要素が存在するかどうかを判断します。
        /// </summary>
        /// <param name="other">現在のセットと比較するコレクション。</param>
        /// <returns>現在のセットが<para>other</para>と等しい場合は<c>true</c>。それ以外の場合は<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException"><para>other</para>が<c>null</c>の場合。</exception>
        public bool SetEquals(IEnumerable<string> other)
        {
            // 重複をカウントしないため、otherをSetに詰めてから比較する
            IgnoreCaseSet diff = new IgnoreCaseSet(other);
            if (this.Count != diff.Count)
            {
                return false;
            }

            foreach (string s in diff)
            {
                if (!this.Contains(s))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 大文字小文字を区別せず、現在のセットを、そのセットと指定されたコレクションの（両方に存在するのではなく）どちらか一方に存在する要素だけが格納されるように変更します。
        /// </summary>
        /// <param name="other">現在のセットと比較するコレクション。</param>
        /// <exception cref="ArgumentNullException"><para>other</para>が<c>null</c>の場合。</exception>
        public void SymmetricExceptWith(IEnumerable<string> other)
        {
            // 現在のセットにはなくて指定されたコレクションにのみある要素を抽出
            IgnoreCaseSet set = new IgnoreCaseSet(other);
            set.ExceptWith(this);

            // 現在のセットから指定されたコレクションにある要素を除去し、抽出しておいたものとマージ
            this.ExceptWith(other);
            this.UnionWith(set);
        }

        /// <summary>
        /// 現在のセットを、そのセットと指定されたコレクションの両方に存在するすべての要素が格納されるように変更します。
        /// </summary>
        /// <param name="other">現在のセットと比較するコレクション。</param>
        /// <exception cref="ArgumentNullException"><para>other</para>が<c>null</c>の場合。</exception>
        public void UnionWith(IEnumerable<string> other)
        {
            // 重複は上書きされるだけなので単純に全部追加
            foreach (string s in Validate.NotNull(other))
            {
                this.Add(s);
            }
        }

        #endregion

        #region ラップするインスタンスを参照するメソッド

        /// <summary>
        /// ラップする<c>ISet</c>インスタンスの<c>GetEnumerator</c>メソッドを呼び出す。
        /// </summary>
        /// <returns>コレクションを反復処理するために使用できる<c>IEnumerator</c>オブジェクト。</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return this.Set.GetEnumerator();
        }

        /// <summary>
        /// ラップする<c>ISet</c>インスタンスの<c>GetEnumerator</c>メソッドを呼び出す。
        /// </summary>
        /// <returns>コレクションを反復処理するために使用できる<c>IEnumerator</c>オブジェクト。</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Set.GetEnumerator();
        }

        #endregion
    }
}
