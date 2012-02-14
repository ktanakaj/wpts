// ================================================================================================
// <summary>
//      IgnoreCaseSetのテストクラスソース。</summary>
//
// <copyright file="IgnoreCaseSetTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Models
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// IgnoreCaseSetのテストクラスです。
    /// </summary>
    [TestFixture]
    public class IgnoreCaseSetTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース（引数なし）。
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            Assert.AreEqual(0, set.Set.Count);
        }

        /// <summary>
        /// コンストラクタテストケース（引数Set）。
        /// </summary>
        [Test]
        public void TestConstructorSet()
        {
            ISet<string> inner = new HashSet<string>();
            inner.Add("TestValue");
            IgnoreCaseSet set = new IgnoreCaseSet(inner);
            Assert.AreEqual(inner, set.Set);
            Assert.IsTrue(set.Contains("TESTValue"));
        }

        /// <summary>
        /// コンストラクタテストケース（引数Enumerable）。
        /// </summary>
        [Test]
        public void TestConstructorEnumerable()
        {
            IList<string> other = new List<string>();
            other.Add("TestValue");
            IgnoreCaseSet set = new IgnoreCaseSet(other);
            Assert.IsTrue(set.Contains("TESTValue"));
        }

        /// <summary>
        /// コンストラクタテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorSetNull()
        {
            new IgnoreCaseSet(null);
        }

        #endregion

        #region 独自実装公開プロパティテストケース
        
        /// <summary>
        /// Setプロパティテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestSet()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            ISet<string> inner = new HashSet<string>();
            inner.Add("TestValue");
            set.Set = inner;
            Assert.AreSame(inner, set.Set);
            Assert.IsTrue(set.Contains("TESTvalue"));
        }

        /// <summary>
        /// Setプロパティテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetNull()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Set = null;
        }
 
        #endregion

        #region ラップするインスタンスを参照するプロパティテストケース

        /// <summary>
        /// Countプロパティテストケース。
        /// </summary>
        [Test]
        public void TestCount()
        {
            // ラップするISetと同じ値であること
            ISet<string> inner = new HashSet<string>();
            inner.Add("TestValue");
            IgnoreCaseSet set = new IgnoreCaseSet(inner);
            Assert.AreEqual(inner.Count, set.Count);
        }
        
        /// <summary>
        /// IsReadOnlyプロパティテストケース。
        /// </summary>
        [Test]
        public void TestIsReadOnly()
        {
            // ラップするISetと同じ値であること
            ISet<string> inner = new HashSet<string>();
            inner.Add("TestValue");
            IgnoreCaseSet set = new IgnoreCaseSet(inner);
            Assert.AreEqual(inner.IsReadOnly, set.IsReadOnly);
        }

        #endregion
        
        #region 独自実装メソッドテストケース

        /// <summary>
        /// Addメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestAdd()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            Assert.AreEqual(0, set.Count);
            set.Add("TestValue");
            Assert.AreEqual(1, set.Count);
            Assert.IsTrue(set.Contains("TESTvalue"));
            Assert.IsTrue(set.Set.Contains("TestValue"));
            Assert.IsFalse(set.Set.Contains("TESTvalue"));
            set.Add("tESTvALUE2");
            Assert.AreEqual(2, set.Count);
            Assert.IsTrue(set.Contains("TESTvalue2"));
            Assert.IsTrue(set.Set.Contains("tESTvALUE2"));
        }

        /// <summary>
        /// Addメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddNull()
        {
            new IgnoreCaseSet().Add(null);
        }

        /// <summary>
        /// Containsメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestContains()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            Assert.IsFalse(set.Contains("TestValue"));
            set.Add("TestValue");
            Assert.IsTrue(set.Contains("TestValue"));
            Assert.IsTrue(set.Contains("tESTvALue"));
            Assert.IsFalse(set.Contains("TestValue2"));
        }

        /// <summary>
        /// Containsメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestContainsNull()
        {
            new IgnoreCaseSet().Contains(null);
        }

        /// <summary>
        /// Clearメソッドテストケース。
        /// </summary>
        [Test]
        public void TestClear()
        {
            // 全データが削除されること、またラップしているオブジェクトは維持されること
            ISet<string> inner = new HashSet<string>();
            IgnoreCaseSet set = new IgnoreCaseSet(inner);
            set.Add("TestValue");
            set.Add("tESTVAlue2");
            Assert.AreEqual(2, set.Count);
            Assert.AreSame(inner, set.Set);

            set.Clear();
            Assert.AreEqual(0, set.Count);
            Assert.AreSame(inner, set.Set);
        }

        /// <summary>
        /// CopyToメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestCopyTo()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            string[] array = new string[5];
            set.CopyTo(array, 3);
            Assert.IsNull(array[0]);
            Assert.IsNull(array[1]);
            Assert.IsNull(array[2]);
            Assert.AreEqual("TestValue", array[3]);
            Assert.AreEqual("tESTvAlue2", array[4]);
        }

        /// <summary>
        /// CopyToメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyToNull()
        {
            new IgnoreCaseSet().CopyTo(null, 0);
        }

        /// <summary>
        /// CopyToメソッドテストケース（インデックスがマイナス値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCopyToOutOfRange()
        {
            new IgnoreCaseSet().CopyTo(new string[5], -1);
        }

        /// <summary>
        /// CopyToメソッドテストケース（領域不足）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCopyToOverflow()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            set.CopyTo(new string[5], 4);
        }

        /// <summary>
        /// ExceptWithメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestExceptWith()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            set.Add("testvalue3");
            set.ExceptWith(new string[] { "TestValue", "testvalUE2", "TestValue4" });
            Assert.AreEqual(1, set.Count);
            Assert.IsTrue(set.Set.Contains("testvalue3"));
        }

        /// <summary>
        /// ExceptWithメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestExceptWithNull()
        {
            new IgnoreCaseSet().ExceptWith(null);
        }

        /// <summary>
        /// IntersectWithメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestIntersectWith()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            set.Add("testvalue3");
            set.IntersectWith(new string[] { "TestValue", "testvalUE2", "TestValue4" });
            Assert.AreEqual(2, set.Count);
            Assert.IsTrue(set.Set.Contains("TestValue"));
            Assert.IsTrue(set.Set.Contains("tESTvAlue2"));
        }

        /// <summary>
        /// IntersectWithメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestIntersectWithNull()
        {
            new IgnoreCaseSet().IntersectWith(null);
        }

        /// <summary>
        /// IsProperSubsetOfメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestIsProperSubsetOf()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            set.Add("testvalue3");
            Assert.IsFalse(set.IsProperSubsetOf(new string[0]));
            Assert.IsFalse(set.IsProperSubsetOf(new string[] { "TestValue", "testvalUE2" }));
            Assert.IsFalse(set.IsProperSubsetOf(new string[] { "TestValue", "testvalUE2", "TestValue4" }));
            Assert.IsFalse(set.IsProperSubsetOf(new string[] { "TestValue", "testvalUE2", "TestValue3" }));
            Assert.IsTrue(set.IsProperSubsetOf(new string[] { "TestValue", "testvalUE2", "TestValue3", "TestValue4" }));
            Assert.IsFalse(set.IsProperSubsetOf(new string[] { "TestValue4" }));
        }

        /// <summary>
        /// IsProperSubsetOfメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestIsProperSubsetOfNull()
        {
            new IgnoreCaseSet().IsProperSubsetOf(null);
        }

        /// <summary>
        /// IsProperSupersetOfメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestIsProperSupersetOf()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            set.Add("testvalue3");
            Assert.IsTrue(set.IsProperSupersetOf(new string[0]));
            Assert.IsTrue(set.IsProperSupersetOf(new string[] { "TestValue", "testvalUE2" }));
            Assert.IsFalse(set.IsProperSupersetOf(new string[] { "TestValue", "testvalUE2", "TestValue4" }));
            Assert.IsFalse(set.IsProperSupersetOf(new string[] { "TestValue", "testvalUE2", "TestValue3" }));
            Assert.IsFalse(set.IsProperSupersetOf(new string[] { "TestValue", "testvalUE2", "TestValue3", "TestValue4" }));
            Assert.IsFalse(set.IsProperSupersetOf(new string[] { "TestValue4" }));
        }

        /// <summary>
        /// IsProperSupersetOfメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestIsProperSupersetOfNull()
        {
            new IgnoreCaseSet().IsProperSupersetOf(null);
        }

        /// <summary>
        /// IsSubsetOfメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestIsSubsetOf()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            set.Add("testvalue3");
            Assert.IsFalse(set.IsSubsetOf(new string[0]));
            Assert.IsFalse(set.IsSubsetOf(new string[] { "TestValue", "testvalUE2" }));
            Assert.IsFalse(set.IsSubsetOf(new string[] { "TestValue", "testvalUE2", "TestValue4" }));
            Assert.IsTrue(set.IsSubsetOf(new string[] { "TestValue", "testvalUE2", "TestValue3" }));
            Assert.IsTrue(set.IsSubsetOf(new string[] { "TestValue", "testvalUE2", "TestValue3", "TestValue4" }));
            Assert.IsFalse(set.IsSubsetOf(new string[] { "TestValue4" }));
        }

        /// <summary>
        /// IsSubsetOfメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestIsSubsetOfNull()
        {
            new IgnoreCaseSet().IsSubsetOf(null);
        }

        /// <summary>
        /// IsSupersetOfメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestIsSupersetOf()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            set.Add("testvalue3");
            Assert.IsTrue(set.IsSupersetOf(new string[0]));
            Assert.IsTrue(set.IsSupersetOf(new string[] { "TestValue", "testvalUE2" }));
            Assert.IsFalse(set.IsSupersetOf(new string[] { "TestValue", "testvalUE2", "TestValue4" }));
            Assert.IsTrue(set.IsSupersetOf(new string[] { "TestValue", "testvalUE2", "TestValue3" }));
            Assert.IsFalse(set.IsSupersetOf(new string[] { "TestValue", "testvalUE2", "TestValue3", "TestValue4" }));
            Assert.IsFalse(set.IsSupersetOf(new string[] { "TestValue4" }));
        }

        /// <summary>
        /// IsSupersetOfメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestIsSupersetOfNull()
        {
            new IgnoreCaseSet().IsSupersetOf(null);
        }

        /// <summary>
        /// Overlapsメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestOverlaps()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            set.Add("testvalue3");
            Assert.IsFalse(set.Overlaps(new string[0]));
            Assert.IsTrue(set.Overlaps(new string[] { "TestValue", "testvalUE2" }));
            Assert.IsTrue(set.Overlaps(new string[] { "TestValue", "testvalUE2", "TestValue4" }));
            Assert.IsTrue(set.Overlaps(new string[] { "TestValue", "testvalUE2", "TestValue3" }));
            Assert.IsTrue(set.Overlaps(new string[] { "TestValue", "testvalUE2", "TestValue3", "TestValue4" }));
            Assert.IsFalse(set.Overlaps(new string[] { "TestValue4" }));
        }

        /// <summary>
        /// Overlapsメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestOverlapsNull()
        {
            new IgnoreCaseSet().Overlaps(null);
        }

        /// <summary>
        /// Removeメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestRemove()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlUe2");
            Assert.AreEqual(2, set.Count);
            Assert.IsTrue(set.Contains("TestValue"));
            Assert.IsTrue(set.Contains("tESTvAlUe2"));

            Assert.IsTrue(set.Remove("tEstValue"));
            Assert.AreEqual(1, set.Count);
            Assert.IsFalse(set.Contains("TestValue"));
            Assert.IsTrue(set.Contains("tESTvAlUe2"));

            Assert.IsFalse(set.Remove("tESTvAlUe3"));
            Assert.AreEqual(1, set.Count);
            Assert.IsTrue(set.Contains("tESTvAlUe2"));

            Assert.IsTrue(set.Remove("tESTvAlUe2"));
            Assert.AreEqual(0, set.Count);
            Assert.IsFalse(set.Contains("tESTvAlUe2"));
        }

        /// <summary>
        /// Removeメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveNull()
        {
            new IgnoreCaseSet().Remove(null);
        }

        /// <summary>
        /// SetEqualsメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestSetEquals()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            set.Add("testvalue3");
            Assert.IsFalse(set.SetEquals(new string[0]));
            Assert.IsFalse(set.SetEquals(new string[] { "TestValue", "testvalUE2" }));
            Assert.IsFalse(set.SetEquals(new string[] { "TestValue", "testvalUE2", "TestValue4" }));
            Assert.IsTrue(set.SetEquals(new string[] { "TestValue", "testvalUE2", "TestValue3" }));
            Assert.IsFalse(set.SetEquals(new string[] { "TestValue", "testvalUE2", "TestValue3", "TestValue4" }));
            Assert.IsFalse(set.SetEquals(new string[] { "TestValue4" }));
        }

        /// <summary>
        /// SetEqualsメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetEqualsNull()
        {
            new IgnoreCaseSet().SetEquals(null);
        }

        /// <summary>
        /// SymmetricExceptWithメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestSymmetricExceptWith()
        {
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            set.Add("testvalue3");
            set.SymmetricExceptWith(new string[] { "TestValue", "testvalUE2", "TestValue4" });
            Assert.AreEqual(2, set.Count);
            Assert.IsTrue(set.Set.Contains("testvalue3"));
            Assert.IsTrue(set.Set.Contains("TestValue4"));
        }

        /// <summary>
        /// SymmetricExceptWithメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSymmetricExceptWithNull()
        {
            new IgnoreCaseSet().SymmetricExceptWith(null);
        }

        /// <summary>
        /// UnionWithメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestUnionWith()
        {
            // 両方の値が格納される、重複するものは後の値で上書き
            IgnoreCaseSet set = new IgnoreCaseSet();
            set.Add("TestValue");
            set.Add("tESTvAlue2");
            set.Add("testvalue3");
            set.UnionWith(new string[] { "TestValue", "testvalUE2", "TestValue4" });
            Assert.AreEqual(4, set.Count);
            Assert.IsTrue(set.Set.Contains("TestValue"));
            Assert.IsTrue(set.Set.Contains("testvalUE2"));
            Assert.IsTrue(set.Set.Contains("testvalue3"));
            Assert.IsTrue(set.Set.Contains("TestValue4"));
        }

        /// <summary>
        /// UnionWithメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUnionWithNull()
        {
            new IgnoreCaseSet().UnionWith(null);
        }

        #endregion

        #region ラップするインスタンスを参照するメソッドテストケース

        /// <summary>
        /// GetEnumeratorメソッドテストケース。
        /// </summary>
        [Test]
        public void TestGetEnumerator()
        {
            // ラップするISetと同じ値であること
            ISet<string> inner = new HashSet<string>();
            inner.Add("TestValue");
            IgnoreCaseSet set = new IgnoreCaseSet(inner);
            Assert.AreEqual(inner.GetEnumerator(), set.GetEnumerator());
        }

        #endregion
    }
}
