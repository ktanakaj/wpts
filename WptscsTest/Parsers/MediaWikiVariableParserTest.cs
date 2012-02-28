// ================================================================================================
// <summary>
//      MediaWikiVariableParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiVariableParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using System.Collections.Generic;
    using Honememo.Parsers;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Websites;
    using NUnit.Framework;

    /// <summary>
    /// <see cref="MediaWikiVariableParser"/>のテストクラスです。
    /// </summary>
    [TestFixture]
    internal class MediaWikiVariableParserTest
    {
        #region private変数

        /// <summary>
        /// 前処理・後処理で生成／解放される言語別の<see cref="MediaWikiParser"/>。
        /// </summary>
        private IDictionary<string, MediaWikiParser> mediaWikiParsers = new Dictionary<string, MediaWikiParser>();

        #endregion

        #region 前処理・後処理

        /// <summary>
        /// テストの前処理。
        /// </summary>
        /// <remarks><see cref="MediaWikiParser.Dispose"/>が必要な<see cref="MediaWikiParser"/>の生成。</remarks>
        [TestFixtureSetUp]
        public void SetUpBeforeClass()
        {
            this.mediaWikiParsers["en"] = new MediaWikiParser(new MockFactory().GetMediaWiki("en"));
            this.mediaWikiParsers["ja"] = new MediaWikiParser(new MockFactory().GetMediaWiki("ja"));
        }

        /// <summary>
        /// テストの後処理。
        /// </summary>
        /// <remarks><see cref="MediaWikiParser.Dispose"/>が必要な<see cref="MediaWikiParser"/>の解放。</remarks>
        [TestFixtureTearDown]
        public void TearDownAfterClass()
        {
            foreach (IDisposable parser in this.mediaWikiParsers.Values)
            {
                parser.Dispose();
            }

            this.mediaWikiParsers.Clear();
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiVariableParser.TryParse"/>メソッドテストケース（基本的な構文）。
        /// </summary>
        [Test]
        public void TestTryParseBasic()
        {
            IElement element;
            MediaWikiVariable variable;
            MediaWikiVariableParser parser = new MediaWikiVariableParser(this.mediaWikiParsers["en"]);

            // 変数のみ
            Assert.IsTrue(parser.TryParse("{{{変数名}}}", out element));
            variable = (MediaWikiVariable)element;
            Assert.AreEqual("変数名", variable.Variable);
            Assert.IsNull(variable.Value);

            // タイトルとパイプ後の文字列
            Assert.IsTrue(parser.TryParse("{{{変数名|デフォルト値}}}", out element));
            variable = (MediaWikiVariable)element;
            Assert.AreEqual("変数名", variable.Variable);
            Assert.AreEqual("デフォルト値", variable.Value.ToString());

            // よく見かけるパイプがあって後ろが無い奴
            Assert.IsTrue(parser.TryParse("{{{変数名|}}}", out element));
            variable = (MediaWikiVariable)element;
            Assert.AreEqual("変数名", variable.Variable);
            Assert.IsNotNull(variable.Value);
            Assert.IsEmpty(variable.Value.ToString());

            // コメントについてはあっても特に問題ない
            Assert.IsTrue(parser.TryParse("{{{変数名<!--必要に応じて変更1-->|デフォルト値<!--必要に応じて変更2-->}}}", out element));
            variable = (MediaWikiVariable)element;
            Assert.AreEqual("変数名<!--必要に応じて変更1-->", variable.Variable);
            Assert.AreEqual("デフォルト値<!--必要に応じて変更2-->", variable.Value.ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiVariableParser.TryParse"/>メソッドテストケース（NGパターン）。
        /// </summary>
        [Test]
        public void TestTryParseNg()
        {
            IElement element;
            MediaWikiVariableParser parser = new MediaWikiVariableParser(this.mediaWikiParsers["en"]);

            // 開始タグが無い
            Assert.IsFalse(parser.TryParse("変数名}}}", out element));

            // 閉じタグが無い
            Assert.IsFalse(parser.TryParse("{{{変数名", out element));

            // 先頭が開始タグではない
            Assert.IsFalse(parser.TryParse(" {{{変数名}}}", out element));

            // 外部リンクタグ
            Assert.IsFalse(parser.TryParse("[変数名]", out element));

            // 内部リンクタグ
            Assert.IsFalse(parser.TryParse("[[変数名]]", out element));

            // テンプレートリンクタグ
            Assert.IsFalse(parser.TryParse("{{変数名}}", out element));

            // 空・null
            Assert.IsFalse(parser.TryParse(String.Empty, out element));
            Assert.IsFalse(parser.TryParse(null, out element));
        }

        /// <summary>
        /// <see cref="MediaWikiVariableParser.TryParse"/>メソッドテストケース（入れ子）。
        /// </summary>
        [Test]
        public void TestTryParseNested()
        {
            IElement element;
            MediaWikiVariable variable;
            MediaWikiVariableParser parser = new MediaWikiVariableParser(this.mediaWikiParsers["ja"]);

            // 入れ子もあり
            Assert.IsTrue(parser.TryParse("{{{変数名|[[内部リンク]]{{ref-en}}}}}", out element));
            variable = (MediaWikiVariable)element;
            Assert.AreEqual("変数名", variable.Variable);
            Assert.AreEqual("[[内部リンク]]{{ref-en}}", variable.Value.ToString());
            Assert.IsInstanceOf(typeof(ListElement), variable.Value);
            ListElement list = (ListElement)variable.Value;
            Assert.AreEqual(2, list.Count);
            Assert.IsInstanceOf(typeof(MediaWikiLink), list[0]);
            Assert.AreEqual("[[内部リンク]]", list[0].ToString());
            Assert.IsInstanceOf(typeof(MediaWikiTemplate), list[1]);
            Assert.AreEqual("{{ref-en}}", list[1].ToString());
        }
        
        #endregion
    }
}
