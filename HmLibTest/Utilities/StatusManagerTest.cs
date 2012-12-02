// ================================================================================================
// <summary>
//      StatusManagerのテストクラスソース。</summary>
//
// <copyright file="StatusManagerTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="StatusManager&lt;T&gt;"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class StatusManagerTest
    {
        #region プロパティテストケース

        /// <summary>
        /// <see cref="StatusManager&lt;T&gt;.Status"/>プロパティテストケース。
        /// </summary>
        [TestMethod]
        public void TestStatus()
        {
            var sm = new StatusManager<string>();

            // 初期状態ではnull
            Assert.IsNull(sm.Status);

            // 値を設定すればその値が入る
            sm.Status = "test";
            Assert.AreEqual("test", sm.Status);

            // 更新時はChangedイベントが呼ばれる
            bool called = false;
            sm.Changed += new EventHandler(delegate { called = true; });
            sm.Status = "test2";
            Assert.IsTrue(called);
            Assert.AreEqual("test2", sm.Status);

            // Switchで値が設定されていた場合、Statusを更新すると戻らなくなる
            sm.Switch("switchstatus");
            Assert.AreEqual("switchstatus", sm.Status);
            sm.Status = "newstatus";
            Assert.AreEqual("newstatus", sm.Status);
            sm.Dispose();
            Assert.AreEqual("newstatus", sm.Status);
        }

        #endregion

        #region 公開メソッドテストケース

        /// <summary>
        /// <see cref="StatusManager&lt;T&gt;.Switch"/>,
        /// <see cref="StatusManager&lt;T&gt;.Dispose"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestSwitch()
        {
            var sm = new StatusManager<string>();

            // 初期状態ではnull
            Assert.IsNull(sm.Status);

            // Switchでステータスが更新、Disposeで元の値に戻る
            using (var sm1 = sm.Switch("switch1"))
            {
                Assert.AreEqual("switch1", sm.Status);

                // 入れ子も可能
                using (var sm2 = sm.Switch("switch2"))
                {
                    Assert.AreEqual("switch2", sm.Status);

                    using (var sm3 = sm.Switch("switch3"))
                    {
                        Assert.AreEqual("switch3", sm.Status);
                    }

                    Assert.AreEqual("switch2", sm.Status);
                }

                Assert.AreEqual("switch1", sm.Status);

                // 設定時と戻り時はChangedイベントが呼ばれる
                int count = 0;
                sm.Changed += new EventHandler(delegate { ++count; });
                Assert.AreEqual(0, count);
                using (var sm2 = sm.Switch("switch4"))
                {
                    Assert.AreEqual(1, count);
                    Assert.AreEqual("switch4", sm.Status);
                }

                Assert.AreEqual(2, count);
                Assert.AreEqual("switch1", sm.Status);
            }

            Assert.IsNull(sm.Status);
        }

        /// <summary>
        /// <see cref="StatusManager&lt;T&gt;.Clear"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestClear()
        {
            var sm = new StatusManager<int>();

            // ステータスを初期状態に戻す
            Assert.AreEqual(0, sm.Status);
            sm.Status = 5;
            Assert.AreEqual(5, sm.Status);
            sm.Clear();
            Assert.AreEqual(0, sm.Status);

            // Switchで値が設定されていた場合、Clearすると戻らなくなる
            sm.Switch(10);
            Assert.AreEqual(10, sm.Status);
            sm.Switch(20);
            Assert.AreEqual(20, sm.Status);
            sm.Clear();
            Assert.AreEqual(0, sm.Status);
            sm.Dispose();
            Assert.AreEqual(0, sm.Status);
        }

        #endregion
    }
}
