// ================================================================================================
// <summary>
//      IgnoreCaseDictionaryのテストクラスソース。</summary>
//
// <copyright file="IgnoreCaseDictionaryTest.cs" company="honeplusのメモ帳">
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
            new IgnoreCaseDictionary<string>(null);
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
            Assert.AreSame(inner, d.Dictionary);
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
            string dymmy = d[null];
        }

        /// <summary>
        /// thisインデクサーテストケース（set、null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThisSetNull()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            d[null] = "TestValue";
        }

        /// <summary>
        /// thisインデクサーテストケース（get、値なし）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestThisKeyNotFound()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            string dymmy = d["TestKey"];
        }

        #endregion

        #region 独自実装メソッドテストケース

        /// <summary>
        /// Addメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestAdd()
        {
            // IDictionaryのAddメソッド
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            Assert.AreEqual(0, d.Count);
            d.Add("TestKey", "TestValue");
            Assert.AreEqual(1, d.Count);
            Assert.AreEqual("TestValue", d["TESTkey"]);
            Assert.AreEqual("TestValue", d.Dictionary["TestKey"]);
            Assert.IsFalse(d.Dictionary.ContainsKey("TESTkey"));
            d.Add("tESTkEY2", "TestValue2");
            Assert.AreEqual(2, d.Count);
            Assert.AreEqual("TestValue2", d["TESTkey2"]);
            Assert.AreEqual("TestValue2", d.Dictionary["tESTkEY2"]);

            // ICollectionのAddメソッド
            d.Add(new KeyValuePair<string, string>("tESTkEY3", "TestValue3"));
            Assert.AreEqual(3, d.Count);
            Assert.AreEqual("TestValue3", d["TESTkey3"]);
            Assert.AreEqual("TestValue3", d.Dictionary["tESTkEY3"]);
        }

        /// <summary>
        /// Addメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddNull()
        {
            new IgnoreCaseDictionary<string>().Add(null, "TestValue");
        }

        /// <summary>
        /// Addメソッドテストケース（KeyValuePairのKeyがnull）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddKeyNull()
        {
            new IgnoreCaseDictionary<string>().Add(new KeyValuePair<string, string>(null, "TestValue"));
        }

        /// <summary>
        /// Addメソッドテストケース（重複）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddDuplicate()
        {
            // 大文字小文字が異なるキーでも重複と判断する
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            d.Add("TestKey", "TestValue");
            d.Add("tESTkEY", "TestValue2");
        }

        /// <summary>
        /// Addメソッドテストケース（KeyValuePairのKeyが重複）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddKeyDuplicate()
        {
            // 大文字小文字が異なるキーでも重複と判断する
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            d.Add(new KeyValuePair<string, string>("TestKey", "TestValue"));
            d.Add(new KeyValuePair<string, string>("tESTkEY", "TestValue2"));
        }

        /// <summary>
        /// ContainsKeyメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestContainsKey()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            Assert.IsFalse(d.ContainsKey("TestKey"));
            d.Add("TestKey", "TestValue");
            Assert.IsTrue(d.ContainsKey("TestKey"));
            Assert.IsTrue(d.ContainsKey("tESTkEy"));
            Assert.IsFalse(d.ContainsKey("TestKey2"));
        }

        /// <summary>
        /// ContainsKeyメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestContainsKeyNull()
        {
            new IgnoreCaseDictionary<string>().ContainsKey(null);
        }

        /// <summary>
        /// Removeメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestRemove()
        {
            // IDictionaryのRemoveメソッド
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            d.Add("TestKey", "TestValue");
            d.Add("tESTkEY2", "TestValue2");
            Assert.AreEqual(2, d.Count);
            Assert.IsTrue(d.ContainsKey("TestKey"));
            Assert.IsTrue(d.ContainsKey("tESTkEY2"));

            Assert.IsTrue(d.Remove("tEstKey"));
            Assert.AreEqual(1, d.Count);
            Assert.IsFalse(d.ContainsKey("TestKey"));
            Assert.IsTrue(d.ContainsKey("tESTkEY2"));

            Assert.IsFalse(d.Remove("tESTkEY3"));
            Assert.AreEqual(1, d.Count);
            Assert.IsTrue(d.ContainsKey("tESTkEY2"));

            // ICollectionのRemoveメソッド
            Assert.IsFalse(d.Remove(new KeyValuePair<string, string>("tESTkEY3", "TestValue2")));
            Assert.AreEqual(1, d.Count);
            Assert.IsTrue(d.ContainsKey("tESTkEY2"));

            Assert.IsTrue(d.Remove(new KeyValuePair<string, string>("TestKey2", "TestValue2")));
            Assert.AreEqual(0, d.Count);
            Assert.IsFalse(d.ContainsKey("tESTkEY2"));
        }

        /// <summary>
        /// Removeメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveNull()
        {
            new IgnoreCaseDictionary<string>().Remove(null);
        }

        /// <summary>
        /// TryGetValueメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestTryGetValue()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            String value;

            Assert.IsFalse(d.TryGetValue("TestKey", out value));
            Assert.IsNull(value);

            d.Add("TestKey", "TestValue");
            Assert.IsTrue(d.TryGetValue("TestKey", out value));
            Assert.AreEqual("TestValue", value);

            Assert.IsFalse(d.TryGetValue("TestKey2", out value));
            Assert.IsNull(value);

            Assert.IsTrue(d.TryGetValue("tESTkEy", out value));
            Assert.AreEqual("TestValue", value);
        }

        /// <summary>
        /// TryGetValueメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTryGetValueNull()
        {
            string dummy;
            new IgnoreCaseDictionary<string>().TryGetValue(null, out dummy);
        }

        /// <summary>
        /// Clearメソッドテストケース。
        /// </summary>
        [Test]
        public void TestClear()
        {
            // 全データが削除されること、またラップしているオブジェクトは維持されること
            IDictionary<string, string> inner = new Dictionary<string, string>();
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>(inner);
            d.Add("TestKey", "TestValue");
            d.Add("tESTkEY2", "TestValue2");
            Assert.AreEqual(2, d.Count);
            Assert.AreSame(inner, d.Dictionary);

            d.Clear();
            Assert.AreEqual(0, d.Count);
            Assert.AreSame(inner, d.Dictionary);
        }

        /// <summary>
        /// Containsメソッドテストケース。
        /// </summary>
        [Test]
        public void TestContains()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            Assert.IsFalse(d.Contains(new KeyValuePair<string, string>("TestKey", "TestValue")));
            d.Add("TestKey", "TestValue");
            Assert.IsTrue(d.Contains(new KeyValuePair<string, string>("TestKey", "TestValue")));
            Assert.IsTrue(d.Contains(new KeyValuePair<string, string>("tESTkEy", "TestValue")));
            Assert.IsFalse(d.Contains(new KeyValuePair<string, string>("TestKey2", "TestValue")));
            Assert.IsFalse(d.Contains(new KeyValuePair<string, string>(null, "TestValue")));
        }

        /// <summary>
        /// CopyToメソッドテストケース（正常系）。
        /// </summary>
        [Test]
        public void TestCopyTo()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            d.Add("TestKey", "TestValue");
            d.Add("tESTkEY2", "TestValue2");
            KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[5];
            d.CopyTo(array, 3);
            Assert.IsNull(array[0].Key);
            Assert.IsNull(array[0].Value);
            Assert.IsNull(array[1].Key);
            Assert.IsNull(array[1].Value);
            Assert.IsNull(array[2].Key);
            Assert.IsNull(array[2].Value);
            Assert.AreEqual("TestKey", array[3].Key);
            Assert.AreEqual("TestValue", array[3].Value);
            Assert.AreEqual("tESTkEY2", array[4].Key);
            Assert.AreEqual("TestValue2", array[4].Value);
        }

        /// <summary>
        /// CopyToメソッドテストケース（null値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyToNull()
        {
            new IgnoreCaseDictionary<string>().CopyTo(null, 0);
        }

        /// <summary>
        /// CopyToメソッドテストケース（インデックスがマイナス値）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCopyToOutOfRange()
        {
            new IgnoreCaseDictionary<string>().CopyTo(new KeyValuePair<string, string>[5], -1);
        }

        /// <summary>
        /// CopyToメソッドテストケース（領域不足）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCopyToOverflow()
        {
            IgnoreCaseDictionary<string> d = new IgnoreCaseDictionary<string>();
            d.Add("TestKey", "TestValue");
            d.Add("tESTkEY2", "TestValue2");
            d.CopyTo(new KeyValuePair<string, string>[5], 4);
        }

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
