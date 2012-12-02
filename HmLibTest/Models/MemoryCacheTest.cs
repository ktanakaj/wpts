// ================================================================================================
// <summary>
//      MemoryCacheのテストクラスソース。</summary>
//
// <copyright file="MemoryCacheTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Models
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="MemoryCache&lt;TKey, TValue&gt;"/>のテストクラスです。
    /// </summary>
    [TestClass]
    internal class MemoryCacheTest
    {
        #region プロパティテストケース

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.Capacity"/>プロパティテストケース（正常系）。
        /// </summary>
        [TestMethod]
        public void TestCapacity()
        {
            MemoryCache<string, string> cache = new MemoryCache<string, string>();
            cache.Capacity = 1;
            Assert.AreEqual(1, cache.Capacity);
            cache.Capacity = 100;
            Assert.AreEqual(100, cache.Capacity);
            cache.Capacity = int.MaxValue;
            Assert.AreEqual(int.MaxValue, cache.Capacity);
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.Capacity"/>プロパティテストケース（0以下の値）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCapacityZero()
        {
            new MemoryCache<string, string>().Capacity = 0;
        }

        #endregion

        #region インデクサーテストケース

        /// <summary>
        /// インデクサーテストケース（正常系）。
        /// </summary>
        [TestMethod]
        public void TestThis()
        {
            // ※ 以下古いほうから削除の確認のために、登録時に毎回ウェイトを入れる
            MemoryCache<string, string> cache = new MemoryCache<string, string>();

            // 普通にget/set
            cache["testkey"] = "testvalue";
            Assert.AreEqual("testvalue", cache["testkey"]);
            Assert.IsFalse(cache.ContainsKey("testkey2"));
            Assert.IsFalse(cache.ContainsKey("testkey3"));
            Thread.Sleep(1);

            cache["testkey2"] = "testvalue2";
            Assert.AreEqual("testvalue", cache["testkey"]);
            Assert.AreEqual("testvalue2", cache["testkey2"]);
            Assert.IsFalse(cache.ContainsKey("testkey3"));
            Thread.Sleep(1);

            // インデクサーは上書き可
            cache["testkey2"] = "testvalue2-2";
            Assert.AreEqual("testvalue2-2", cache["testkey2"]);
            Thread.Sleep(1);

            // nullも格納可能
            cache["testkey3"] = null;
            Assert.IsTrue(cache.ContainsKey("testkey3"));
            Assert.IsNull(cache["testkey3"]);
            Thread.Sleep(1);

            // キャッシュが100件を超えると最後にgetされたのが古い方から10件削除される
            for (int i = 4; i <= 100; i++)
            {
                cache[i.ToString()] = i.ToString();
                Thread.Sleep(1);
            }

            // 100件丁度ではまだ存在する
            Assert.IsTrue(cache.ContainsKey("testkey"));
            Assert.IsTrue(cache.ContainsKey("testkey2"));
            Assert.IsTrue(cache.ContainsKey("testkey3"));
            Assert.IsTrue(cache.ContainsKey("4"));
            Assert.IsTrue(cache.ContainsKey("11"));
            Assert.IsTrue(cache.ContainsKey("12"));
            Assert.IsTrue(cache.ContainsKey("100"));
            Assert.IsFalse(cache.ContainsKey("testkey101"));

            // 片方だけgetを呼んだ上で一件追加
            Assert.AreEqual("testvalue2-2", cache["testkey2"]);
            cache["testkey101"] = "testvalue101";

            // アクセスが古い方から10件が削除される
            Assert.IsFalse(cache.ContainsKey("testkey"));
            Assert.IsTrue(cache.ContainsKey("testkey2"));
            Assert.IsFalse(cache.ContainsKey("testkey3"));
            Assert.IsFalse(cache.ContainsKey("4"));
            Assert.IsFalse(cache.ContainsKey("11"));
            Assert.IsTrue(cache.ContainsKey("12"));
            Assert.IsTrue(cache.ContainsKey("100"));
            Assert.IsTrue(cache.ContainsKey("testkey101"));
            Assert.AreEqual("testvalue2-2", cache["testkey2"]);
            Assert.AreEqual("12", cache["12"]);
            Assert.AreEqual("testvalue101", cache["testkey101"]);
        }

        /// <summary>
        /// インデクサーテストケース（getでnull）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThisGetNull()
        {
            string dummy = new MemoryCache<string, string>()[null];
        }

        /// <summary>
        /// インデクサーテストケース（getで値無し）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestThisGetEmpty()
        {
            string dummy = new MemoryCache<string, string>()["ignore key"];
        }

        /// <summary>
        /// インデクサーテストケース（setでnull）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestThisSetNull()
        {
            new MemoryCache<string, string>()[null] = "test";
        }

        #endregion

        #region IDictionaryインタフェースにあわせたメソッド

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.Add"/>メソッドテストケース（正常系）。
        /// </summary>
        [TestMethod]
        public void TestAdd()
        {
            // ※ 2012年2月現在、重複チェック以外はインデクサーと同じなので割愛
            MemoryCache<string, string> cache = new MemoryCache<string, string>();

            cache.Add("testkey", "testvalue");
            Assert.AreEqual("testvalue", cache["testkey"]);
            Assert.IsFalse(cache.ContainsKey("testkey2"));

            cache.Add("testkey2", "testvalue2");
            Assert.AreEqual("testvalue", cache["testkey"]);
            Assert.AreEqual("testvalue2", cache["testkey2"]);
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.Add"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddNull()
        {
            new MemoryCache<string, string>().Add(null, "dummy");
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.Add"/>メソッドテストケース（重複）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddAlready()
        {
            // 同じ値を重複登録
            MemoryCache<string, string> cache = new MemoryCache<string, string>();
            cache["testkey"] = "testvalue";
            cache.Add("testkey", "testvalue");
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.ContainsKey"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestContainsKey()
        {
            MemoryCache<string, string> cache = new MemoryCache<string, string>();

            Assert.IsFalse(cache.ContainsKey("testkey"));
            Assert.IsFalse(cache.ContainsKey("testkey2"));
            
            cache["testkey"] = "testvalue";
            Assert.IsTrue(cache.ContainsKey("testkey"));
            Assert.IsFalse(cache.ContainsKey("testkey2"));

            cache["testkey2"] = "testvalue2";
            Assert.IsTrue(cache.ContainsKey("testkey"));
            Assert.IsTrue(cache.ContainsKey("testkey2"));
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.ContainsKey"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestContainsKeyNull()
        {
            new MemoryCache<string, string>().ContainsKey(null);
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.Remove"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestRemove()
        {
            // 指定されたキャッシュが削除される
            MemoryCache<string, string> cache = new MemoryCache<string, string>();
            cache["testkey"] = "testvalue";
            cache["testkey2"] = "testvalue2";
            Assert.IsTrue(cache.ContainsKey("testkey"));
            Assert.IsTrue(cache.ContainsKey("testkey2"));
            Assert.AreEqual("testvalue", cache.Get("testkey"));
            Assert.AreEqual("testvalue2", cache.Get("testkey2"));

            Assert.IsTrue(cache.Remove("testkey"));
            Assert.IsFalse(cache.ContainsKey("testkey"));
            Assert.IsTrue(cache.ContainsKey("testkey2"));

            Assert.IsTrue(cache.Remove("testkey2"));
            Assert.IsFalse(cache.ContainsKey("testkey"));
            Assert.IsFalse(cache.ContainsKey("testkey2"));

            // Removeは値が無くても呼べる
            Assert.IsFalse(cache.Remove("testkey"));
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.Remove"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveNull()
        {
            new MemoryCache<string, string>().Remove(null);
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.TryGetValue"/>メソッドテストケース（正常系）。
        /// </summary>
        [TestMethod]
        public void TestTryGetValue()
        {
            // ※ 2012年2月現在、例外処理以外についてはインデクサーやGetと同じなので割愛
            MemoryCache<string, string> cache = new MemoryCache<string, string>();
            string result;

            Assert.IsFalse(cache.TryGetValue("testkey", out result));

            cache["testkey"] = "testvalue";
            Assert.IsTrue(cache.TryGetValue("testkey", out result));
            Assert.AreEqual("testvalue", cache.Get("testkey"));
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.TryGetValue"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTryGetValueNull()
        {
            string dummy;
            new MemoryCache<string, string>().TryGetValue(null, out dummy);
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.Clear"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestClear()
        {
            // 全てのキャッシュが削除される
            MemoryCache<string, string> cache = new MemoryCache<string, string>();
            cache["testkey"] = "testvalue";
            cache["testkey2"] = "testvalue2";
            Assert.IsTrue(cache.ContainsKey("testkey"));
            Assert.IsTrue(cache.ContainsKey("testkey2"));
            Assert.AreEqual("testvalue", cache.Get("testkey"));
            Assert.AreEqual("testvalue2", cache.Get("testkey2"));

            cache.Clear();
            Assert.IsFalse(cache.ContainsKey("testkey"));
            Assert.IsFalse(cache.ContainsKey("testkey2"));
        }

        #endregion

        #region 独自のメソッドテストケース
        
        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.Get"/>メソッドテストケース（正常系）。
        /// </summary>
        [TestMethod]
        public void TestGet()
        {
            // ※ 2012年2月現在インデクサーを呼んでいるだけなのでテスト割愛
            MemoryCache<string, string> cache = new MemoryCache<string, string>();
            cache["testkey"] = "testvalue";
            Assert.AreEqual("testvalue", cache.Get("testkey"));
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.Get"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetNull()
        {
            new MemoryCache<string, string>().Get(null);
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.Get"/>メソッドテストケース（値無し）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestGetEmpty()
        {
            new MemoryCache<string, string>().Get("ignore key");
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.GetAndAddIfEmpty"/>メソッドテストケース（正常系）。
        /// </summary>
        [TestMethod]
        public void TestGetAndAddIfEmpty()
        {
            MemoryCache<string, string> cache = new MemoryCache<string, string>();

            // 値があればその値を、無ければファンクションが返す値を返す
            cache["testkey"] = "testvalue";
            Assert.AreEqual("testvalue", cache.GetAndAddIfEmpty("testkey", (string str) => "testvalue2"));
            Assert.AreEqual("testvalue2", cache.GetAndAddIfEmpty("testkey2", (string str) => "testvalue2"));
            Assert.AreEqual("testvalue2", cache.GetAndAddIfEmpty("testkey3", (string str) => "testvalue2"));

            // ファンクションが返した値はキャッシュに登録される
            Assert.AreEqual("testvalue2", cache["testkey2"]);
            Assert.AreEqual("testvalue2", cache["testkey3"]);

            // nullもキャッシュされる
            Assert.IsNull(cache.GetAndAddIfEmpty("testkey4", (string str) => null));
            Assert.IsTrue(cache.ContainsKey("testkey4"));
        }

        /// <summary>
        /// <see cref="MemoryCache&lt;TKey, TValue&gt;.GetAndAddIfEmpty"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetAndAddIfEmptyNull()
        {
            new MemoryCache<string, string>().GetAndAddIfEmpty(null, (string str) => "testvalue2");
        }

        #endregion

        #region 全体のテストケース

        /// <summary>
        /// 全体のテストケース（並列実行）。
        /// </summary>
        [TestMethod, Timeout(1500)]
        public void TestMemoryCacheParallel()
        {
            // 並列実行でいくつかのメソッドを呼んで問題ないかを確認する
            // ※ 結果などはあまり確認できないので、とりあえず変なエラーにならないことだけ
            MemoryCache<int, string> cache = new MemoryCache<int, string>();
            Parallel.For(
                0,
                100000,
                (int i)
                    =>
                {
                    int key = i % 200;
                    cache[key] = i.ToString();
                    string result;
                    if (cache.TryGetValue(key, out result))
                    {
                        // ※ 登録した直後でも、別スレッドからの登録でキャッシュがあふれて消えることもあるので
                        Assert.AreEqual(key, int.Parse(result) % 200);
                    }
                });
            cache.Clear();
        }

        #endregion
    }
}
