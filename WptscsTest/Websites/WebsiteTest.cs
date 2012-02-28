// ================================================================================================
// <summary>
//      Websiteのテストクラスソース。</summary>
//
// <copyright file="WebsiteTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Utilities;
    using NUnit.Framework;

    /// <summary>
    /// <see cref="Website"/>のテストクラスです。
    /// </summary>
    [TestFixture]
    internal class WebsiteTest
    {
        #region プロパティテストケース

        /// <summary>
        /// <see cref="Website.Location"/>プロパティテストケース。
        /// </summary>
        [Test]
        public void TestLocation()
        {
            DummySite site = new DummySite();
            site.Location = "test";
            Assert.AreEqual("test", site.Location);
        }

        /// <summary>
        /// <see cref="Website.Location"/>プロパティテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLocationNull()
        {
            new DummySite().Location = null;
        }

        /// <summary>
        /// <see cref="Website.Location"/>プロパティテストケース（空）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLocationBlank()
        {
            new DummySite().Location = " ";
        }

        /// <summary>
        /// <see cref="Website.Language"/>プロパティテストケース。
        /// </summary>
        [Test]
        public void TestLanguage()
        {
            DummySite site = new DummySite();
            site.Language = new Language("ja");
            Assert.AreEqual("ja", site.Language.Code);
        }

        /// <summary>
        /// <see cref="Website.Language"/>プロパティテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLanguageNull()
        {
            new DummySite().Language = null;
        }

        /// <summary>
        /// <see cref="Website.WebProxy"/>プロパティテストケース。
        /// </summary>
        [Test]
        public void TestWebProxy()
        {
            DummySite site = new DummySite();

            // デフォルトでオブジェクトが格納されている
            Assert.IsNotNull(site.WebProxy);

            // 値を設定するとそのオブジェクトが返る
            IWebProxy proxy = new AppConfigWebProxy();
            site.WebProxy = proxy;
            Assert.AreSame(proxy, site.WebProxy);
        }

        /// <summary>
        /// <see cref="Website.WebProxy"/>プロパティテストケース（null）。
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWebProxyNull()
        {
            new DummySite().WebProxy = null;
        }

        #endregion

        #region モッククラス

        /// <summary>
        /// <see cref="Website"/>テスト用のモッククラスです。
        /// </summary>
        private class DummySite : Website
        {
            #region 非公開プロパティテスト用のオーラーライドプロパティ
            
            /// <summary>
            /// ウェブサイトの言語。
            /// </summary>
            public new Language Language
            {
                get
                {
                    return base.Language;
                }

                set
                {
                    base.Language = value;
                }
            }

            #endregion

            #region ダミーメソッド

            /// <summary>
            /// ページを取得。空実装。
            /// </summary>
            /// <param name="title">ページタイトル。</param>
            /// <returns><c>null</c>。</returns>
            public override Page GetPage(string title)
            {
                return null;
            }

            #endregion
        }

        #endregion
    }
}
