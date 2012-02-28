// ================================================================================================
// <summary>
//      AppConfigWebProxyのテストクラスソース。</summary>
//
// <copyright file="AppConfigWebProxyTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Utilities
{
    using System;
    using System.IO;
    using Honememo.Wptscs.Properties;
    using Honememo.Wptscs.Utilities;
    using NUnit.Framework;

    /// <summary>
    /// <see cref="AppConfigWebProxy"/>のテストクラスです。
    /// </summary>
    [TestFixture]
    internal class AppConfigWebProxyTest
    {
        #region 定数

        /// <summary>
        /// テストデータが格納されているフォルダパス。
        /// </summary>
        private static readonly string testFile = "Data\\config.xml";

        #endregion

        #region インタフェース実装プロパティテストケース

        /// <summary>
        /// <see cref="AppConfigWebProxy.UserAgent"/>プロパティテストケース。
        /// </summary>
        /// <remarks>アプリ設定部分はアクセス権の関係上試験できず。</remarks>
        [Test]
        public void TestUserAgent()
        {
            IWebProxy proxy = new AppConfigWebProxy();
            ////Settings.Default.UserAgent = String.Empty;

            // 初期状態ではアプリ名を元に生成した値
            Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            ////Assert.AreEqual(
            ////    String.Format(Settings.Default.DefaultUserAgent, ver.Major, ver.Minor),
            ////    proxy.UserAgent);
            Assert.AreEqual(
                String.Format("Translation Support for Wikipedia/{0}.{1:D2}", ver.Major, ver.Minor),
                proxy.UserAgent);

            // プロパティ設定時はその値が返る
            proxy.UserAgent = "test property useragent";
            Assert.AreEqual("test property useragent", proxy.UserAgent);

            // 空でも有効
            proxy.UserAgent = String.Empty;
            Assert.IsEmpty(proxy.UserAgent);

            // nullなら無効
            proxy.UserAgent = null;
            Assert.IsNotEmpty(proxy.UserAgent);

            // アプリ設定時はアプリに格納された設定値が最優先
            ////Settings.Default.UserAgent = "test setting useragent";
            ////Assert.AreEqual("test setting useragent", proxy.UserAgent);
        }

        /// <summary>
        /// <see cref="AppConfigWebProxy.Referer"/>プロパティテストケース。
        /// </summary>
        /// <remarks>アプリ設定部分はアクセス権の関係上試験できず。</remarks>
        [Test]
        public void TestReferer()
        {
            IWebProxy proxy = new AppConfigWebProxy();
            ////Settings.Default.Referer = String.Empty;

            // 初期状態では空
            Assert.IsEmpty(proxy.Referer);

            // プロパティ設定時はその値が返る
            proxy.Referer = "test property referer";
            Assert.AreEqual("test property referer", proxy.Referer);

            // アプリ設定時はアプリに格納された設定値
            ////Settings.Default.Referer = "test setting referer";
            ////Assert.AreEqual("test setting referer", proxy.Referer);
        }

        #endregion

        #region インタフェース実装メソッドテストケース
        
        /// <summary>
        /// <see cref="AppConfigWebProxy.GetStream"/>メソッドテストケース。
        /// </summary>
        /// <remarks>内容的に難しいため、fileプロトコルのみ確認。</remarks>
        [Test]
        public void TestGetStream()
        {
            IWebProxy proxy = new AppConfigWebProxy();

            // テストファイルを読んで例外が発生しなければOKとする
            UriBuilder b = new UriBuilder("file", String.Empty);
            b.Path = Path.GetFullPath(testFile);
            using (proxy.GetStream(b.Uri))
            {
            }
        }

        #endregion
    }
}
