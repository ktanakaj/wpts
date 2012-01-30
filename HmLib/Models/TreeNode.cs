// ================================================================================================
// <summary>
//      ツリー構造のデータを扱うためのクラスソース</summary>
//
// <copyright file="TreeNode.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// ツリー構造のデータを扱うためのクラスです。
    /// </summary>
    /// <typeparam name="T">ノード内の値の型。</typeparam>
    /// <remarks>
    /// 特に循環などはチェックしていないため、そうしたデータは作成しないよう注意。
    /// </remarks>
    public class TreeNode<T> : IEnumerable<TreeNode<T>>
    {
        #region private変数

        /// <summary>
        /// このノードにぶら下がる子ノード。
        /// </summary>
        private IList<TreeNode<T>> children = new List<TreeNode<T>>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ノードを作成する。
        /// </summary>
        /// <param name="value">このノードが保持するオブジェクト。未指定の場合その型のデフォルト値。</param>
        public TreeNode(T value = default(T))
        {
            this.Value = value;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// このノードが保持するオブジェクト。
        /// </summary>
        public T Value
        {
            get;
            set;
        }

        /// <summary>
        /// このノードがぶら下がる親ノード。
        /// </summary>
        public TreeNode<T> Parent
        {
            get;
            protected set;
        }

        /// <summary>
        /// このノードにぶら下がる子ノード。
        /// </summary>
        /// <remarks>この属性への変更はツリー構造には反映されない。</remarks>
        public IList<TreeNode<T>> Children
        {
            get
            {
                return new List<TreeNode<T>>(this.children);
            }
        }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// このノードに子ノードを追加する。
        /// </summary>
        /// <param name="node">追加するノード。</param>
        public void Add(TreeNode<T> node)
        {
            // 循環参照となる場合例外を投げる
            if (node == this || this.IsParent(node))
            {
                throw new InvalidOperationException("circular reference.");
            }

            // 子ノードを登録、親ノードを更新
            this.children.Add(node);
            node.Parent = this;
        }

        /// <summary>
        /// このノードから子ノードを取り除く。
        /// </summary>
        /// <param name="node">取り除くノード。</param>
        /// <returns>ノードが取り除かれた場合<c>true</c>。それ以外の場合<c>false</c>。</returns>
        public bool Remove(TreeNode<T> node)
        {
            // 子ノードを除去、親ノードを更新
            if (this.children.Remove(node))
            {
                node.Parent = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// このツリーの全ノードを取得する<c>IEnumerator</c>を返す。
        /// </summary>
        /// <returns>コレクションを反復処理するために使用できる<c>IEnumerator</c>オブジェクト。</returns>
        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            // 再帰的にツリーの全ノードを返す
            yield return this;
            foreach (TreeNode<T> child in this.children)
            {
                foreach (TreeNode<T> node in child)
                {
                    yield return node;
                }
            }
        }

        /// <summary>
        /// このツリーの全ノードを取得する<c>IEnumerator</c>を返す。
        /// </summary>
        /// <returns>コレクションを反復処理するために使用できる<c>IEnumerator</c>オブジェクト。</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// 指定されたノードが自分の祖先に存在するかを判定。
        /// </summary>
        /// <param name="node">確認するノード。</param>
        /// <returns>祖先の場合<c>true</c>。</returns>
        private bool IsParent(TreeNode<T> node)
        {
            if (this.Parent == null)
            {
                return false;
            }

            if (this.Parent == node)
            {
                return true;
            }

            // 再帰的に親ノードを探索
            return this.Parent.IsParent(node);
        }

        #endregion
    }
}
