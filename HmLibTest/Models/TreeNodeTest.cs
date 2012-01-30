// ================================================================================================
// <summary>
//      TreeNodeのテストクラスソース。</summary>
//
// <copyright file="TreeNodeTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Models
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// TreeNodeのテストクラスです。
    /// </summary>
    [TestFixture]
    public class TreeNodeTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース。
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            // 引数無しの場合null
            TreeNode<string> node = new TreeNode<string>();
            Assert.IsNull(node.Value);

            // 引数有りの場合その値
            node = new TreeNode<string>("test string");
            Assert.AreEqual("test string", node.Value);
        }

        #endregion

        #region プロパティテストケース

        /// <summary>
        /// Valueプロパティテストケース。
        /// </summary>
        [Test]
        public void TestValue()
        {
            // 初期状態ではnull
            TreeNode<string> node1 = new TreeNode<string>();
            Assert.IsNull(node1.Value);

            // 値を設定するとその値となる
            node1.Value = "test string";
            Assert.AreEqual("test string", node1.Value);

            // 別の型でも同様
            TreeNode<DateTime> node2 = new TreeNode<DateTime>();
            node2.Value = new DateTime(100L);
            Assert.AreEqual(new DateTime(100L), node2.Value);
        }

        /// <summary>
        /// Parentプロパティテストケース。
        /// </summary>
        /// <remarks>値についての確認はAdd()メソッド等の試験で実施。</remarks>
        [Test]
        public void TestParent()
        {
            // 初期状態ではnull
            TreeNode<string> node = new TreeNode<string>();
            Assert.IsNull(node.Parent);
        }

        /// <summary>
        /// Childrenプロパティテストケース。
        /// </summary>
        /// <remarks>値についての確認はAdd()メソッド等の試験で実施。</remarks>
        [Test]
        public void TestChildren()
        {
            // 初期状態では空のリスト
            TreeNode<string> node = new TreeNode<string>();
            Assert.IsNotNull(node.Children);
            Assert.AreEqual(0, node.Children.Count);
        }

        #endregion

        #region 公開メソッドテストケース

        /// <summary>
        /// Addメソッドテストケース正常系。
        /// </summary>
        [Test]
        public void TestAdd()
        {
            // 子ノードを1件登録
            TreeNode<string> parent = new TreeNode<string>("parent string");
            TreeNode<string> child1 = new TreeNode<string>("child1 string");
            parent.Add(child1);
            Assert.IsNull(parent.Parent);
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreSame(parent, child1.Parent);
            Assert.AreEqual(0, child1.Children.Count);
            Assert.AreSame(child1, parent.Children[0]);

            // 2件目を登録
            TreeNode<string> child2 = new TreeNode<string>("child2 string");
            parent.Add(child2);
            Assert.AreEqual(2, parent.Children.Count);
            Assert.AreSame(child1, parent.Children[0]);
            Assert.AreSame(parent, child2.Parent);
            Assert.AreEqual(0, child2.Children.Count);
            Assert.AreSame(child2, parent.Children[1]);
        }

        /// <summary>
        /// Addメソッドテストケース 自分を設定。
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddThis()
        {
            TreeNode<string> parent = new TreeNode<string>("parent string");
            TreeNode<string> child = new TreeNode<string>("child string");
            parent.Add(child);
            child.Add(parent);
        }

        /// <summary>
        /// Addメソッドテストケース 親を設定。
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddParent()
        {
            TreeNode<string> parent = new TreeNode<string>("parent string");
            TreeNode<string> child = new TreeNode<string>("child string");
            parent.Add(child);
            child.Add(parent);
        }

        /// <summary>
        /// Addメソッドテストケース 祖先を設定。
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddGrandparent()
        {
            TreeNode<string> grandparent = new TreeNode<string>("grandparent string");
            TreeNode<string> parent = new TreeNode<string>("parent string");
            TreeNode<string> child = new TreeNode<string>("child string");
            grandparent.Add(parent);
            parent.Add(child);
            child.Add(grandparent);
        }

        /// <summary>
        /// Removeメソッドテストケース。
        /// </summary>
        [Test]
        public void Remove()
        {
            // 子ノードを持つノードを作成
            TreeNode<string> parent = new TreeNode<string>("parent string");
            TreeNode<string> child1 = new TreeNode<string>("child1 string");
            TreeNode<string> child2 = new TreeNode<string>("child2 string");
            TreeNode<string> child3 = new TreeNode<string>("child3 string");
            parent.Add(child1);
            parent.Add(child2);
            parent.Add(child3);
            Assert.AreEqual(3, parent.Children.Count);
            Assert.AreSame(parent, child1.Parent);
            Assert.AreSame(child1, parent.Children[0]);

            // 子ノードを1件削除
            Assert.IsTrue(parent.Remove(child1));
            Assert.IsNull(child1.Parent);
            Assert.AreEqual(2, parent.Children.Count);
            Assert.AreSame(child2, parent.Children[0]);

            // 削除済みのノードを削除しようとするとfalse
            Assert.IsFalse(parent.Remove(child1));
            Assert.AreEqual(2, parent.Children.Count);

            // 子ノード2件目を削除
            Assert.IsTrue(parent.Remove(child3));
            Assert.IsNull(child3.Parent);
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreSame(child2, parent.Children[0]);
        }

        /// <summary>
        /// GetEnumeratorメソッドテストケース。
        /// </summary>
        [Test]
        public void TestGetEnumerator()
        {
            // 親ノードのみ
            TreeNode<string> parent = new TreeNode<string>("parent string");
            this.AreSameIEnumerator<TreeNode<string>>(parent.GetEnumerator(), parent);

            // 子ノードまで
            TreeNode<string> child1 = new TreeNode<string>("child1 string");
            TreeNode<string> child2 = new TreeNode<string>("child2 string");
            parent.Add(child1);
            parent.Add(child2);
            this.AreSameIEnumerator<TreeNode<string>>(parent.GetEnumerator(), parent, child1, child2);

            // 孫ノードまで
            TreeNode<string> grandchild1 = new TreeNode<string>("grandchild1 string");
            child1.Add(grandchild1);
            this.AreSameIEnumerator<TreeNode<string>>(parent.GetEnumerator(), parent, child1, grandchild1, child2);
        }

        #endregion

        #region テスト補助メソッド

        /// <summary>
        /// IEnumeratorが指定された値を返すかを検証する。
        /// </summary>
        /// <typeparam name="T">IEnumeratorが返す型。</typeparam>
        /// <param name="actual">検証するIEnumerator。</param>
        /// <param name="expected">期待値。</param>
        /// <remarks>値の数も含めて検証する。不一致は試験失敗。</remarks>
        private void AreSameIEnumerator<T>(IEnumerator<T> actual, params T[] expected)
        {
            foreach (T obj in expected)
            {
                if (actual.MoveNext())
                {
                    Assert.AreSame(obj, actual.Current);
                }
                else
                {
                    Assert.Fail("actual < expected");
                }
            }

            if (actual.MoveNext())
            {
                Assert.Fail("actual > expected");
            }
        }

        #endregion
    }
}
