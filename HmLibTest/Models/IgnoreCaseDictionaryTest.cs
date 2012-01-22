// ================================================================================================
// <summary>
//      IgnoreCaseDictionaryのテストクラスソース。</summary>
//
// <copyright file="IgnoreCaseDictionaryTest.cs" company="honeplusのメモ帳">
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
    /// IgnoreCaseDictionaryのテストクラスです。
    /// </summary>
    [TestFixture]
    public class IgnoreCaseDictionaryTest
    {
        #region コンストラクタテストケース

        /// <summary>
        /// コンストラクタテストケース（引数なし）。
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            Assert.AreEqual(0, d.Dictionary.Count);
        }

        /// <summary>
        /// コンストラクタテストケース（引数Dictionary）。
        /// </summary>
        [Test]
        public void TestConstructorDictionary()
        {
            IDictionary<string, string> inner = new Dictionary<string, string>();
            inner.Add("TestKey", "TestValue");
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>(inner);
            Assert.AreEqual(inner, d.Dictionary);
            Assert.IsTrue(d.ContainsKey("TESTkey"));
        }

        /// <summary>
        /// コンストラクタテストケース（引数Dictionary、null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorDictionaryNull()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>(null);
        }

        #endregion

        #region 独自実装公開プロパティテストケース
        
        /// <summary>
        /// Dictionaryプロパティテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestDictionary()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            IDictionary<string, string> inner = new Dictionary<string, string>();
            inner.Add("TestKey", "TestValue");
            d.Dictionary = inner;
            Assert.AreEqual(inner, d.Dictionary);
            Assert.IsTrue(d.ContainsKey("TESTkey"));
        }

        /// <summary>
        /// Dictionaryプロパティテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDictionaryNull()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            d.Dictionary = null;
        }
 
        #endregion

        #region ラップするインスタンスを参照するプロパティテストケース

        /// <summary>
        /// Keysプロパティテストケース。
        /// </summary>
        [Test]
        public void TestKeys()
        {
            // ラップするIDictionaryと同じ値であること
            IDictionary<string, string> inner = new Dictionary<string, string>();
            inner.Add("TestKey", "TestValue");
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>(inner);
            Assert.AreEqual(inner.Keys, d.Keys);
        }

        /// <summary>
        /// Valuesプロパティテストケース。
        /// </summary>
        [Test]
        public void TestValues()
        {
            // ラップするIDictionaryと同じ値であること
            IDictionary<string, string> inner = new Dictionary<string, string>();
            inner.Add("TestKey", "TestValue");
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>(inner);
            Assert.AreEqual(inner.Values, d.Values);
        }

        /// <summary>
        /// Countプロパティテストケース。
        /// </summary>
        [Test]
        public void TestCount()
        {
            // ラップするIDictionaryと同じ値であること
            IDictionary<string, string> inner = new Dictionary<string, string>();
            inner.Add("TestKey", "TestValue");
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>(inner);
            Assert.AreEqual(inner.Count, d.Count);
        }
        
        /// <summary>
        /// IsReadOnlyプロパティテストケース。
        /// </summary>
        [Test]
        public void TestIsReadOnly()
        {
            // ラップするIDictionaryと同じ値であること
            IDictionary<string, string> inner = new Dictionary<string, string>();
            inner.Add("TestKey", "TestValue");
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>(inner);
            Assert.AreEqual(inner.IsReadOnly, d.IsReadOnly);
        }

        #endregion
        
        #region 独自実装インデクサーテストケース

        /// <summary>
        /// thisインデクサーテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestThis()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            d["TestKey"] = "TestValue";
            Assert.AreEqual("TestValue", d["TESTkey"]);
            Assert.AreEqual("TestValue", d.Dictionary["TestKey"]);
            d["tESTkEY"] = "TestValue2";
            Assert.AreEqual("TestValue2", d["TESTkey"]);
            Assert.AreEqual("TestValue2", d.Dictionary["tESTkEY"]);
            Assert.IsFalse(d.Dictionary.ContainsKey("TestKey"));
        }

        /// <summary>
        /// thisインデクサーテストケース（get、null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThisGetNull()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            string dymmy = d.Dictionary[null];
        }

        /// <summary>
        /// thisインデクサーテストケース（set、null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThisSetNull()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            d.Dictionary[null] = "TestValue";
        }

        /// <summary>
        /// thisインデクサーテストケース（get、値なし）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestThisKeyNotFound()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            string dymmy = d.Dictionary["TestKey"];
        }

        #endregion

        #region 独自実装メソッドテストケース

        // TODO: いっぱい足りない

        #endregion

        #region ラップするインスタンスを参照するメソッドテストケース

        /// <summary>
        /// GetEnumeratorメソッドテストケース。
        /// </summary>
        [Test]
        public void TestGetEnumerator()
        {
            // ラップするIDictionaryと同じ値であること
            IDictionary<string, string> inner = new Dictionary<string, string>();
            inner.Add("TestKey", "TestValue");
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>(inner);
            Assert.AreEqual(inner.GetEnumerator(), d.GetEnumerator());
        }

        #endregion
    }
}
