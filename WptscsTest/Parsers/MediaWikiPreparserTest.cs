// ================================================================================================
// <summary>
//      MediaWikiPreparserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiPreparserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using Honememo.Parsers;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Websites;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// <see cref="MediaWikiPreparser"/>のテストクラスです。
    /// </summary>
    [TestClass]
    public class MediaWikiPreparserTest
    {
        #region 定数

        /// <summary>
        /// 複数のテストケースで使用するテストテキスト（onlyinclude無し）。
        /// </summary>
        /// <remarks>タグの大文字小文字は区別されないはずのため、意図的に混入させている。</remarks>
        private static readonly string TestDataWithoutOnlyinclude = "This template is [[xxx]]<br />\r\n"
            + "<noWiki><nowiki>sample</nowiki></nowiki>\r\n"
            + "<inclUdeonly><p>include text<nowiki><includeonly>sample</includeonly></nowiki></p></includeonly>\r\n"
            + "<noInclude>[[ja:Template:sample/doc]]<!--noinclude only--></noinclude>\r\n"
            + "<!-- <includeonly>include only comment</includeonly> -->";

        /// <summary>
        /// 複数のテストケースで使用するテストテキスト。
        /// </summary>
        private static readonly string TestData = TestDataWithoutOnlyinclude
            + "<onlyinclude><noinclude>インクルード時は</noinclude>ここしか</onlyinclude>, <onlyinclude>有効にならない</onlyinclude>";

        #endregion

        #region IParserインタフェーステストケース

        // ※ 2012年2月現在、IParser, ITextParserの各メソッド実装は親クラス側で行われており、
        //    改造部分はどこかでやればテストされるのでそれで割愛

        /// <summary>
        /// <see cref="IParser.Parse"/>メソッドテストトケース。
        /// </summary>
        [TestMethod]
        public void TestParse()
        {
            IElement element;
            XmlElement xml;
            using (MediaWikiPreparser parser = new MediaWikiPreparser())
            {
                element = parser.Parse(TestDataWithoutOnlyinclude);
            }

            // 解析だけであればincludeonly等の処理は行われない、元の文字列が保持される
            Assert.AreEqual(TestDataWithoutOnlyinclude, element.ToString());

            // includeonly, noinclude, nowiki, コメントのみ特別な要素として認識する
            Assert.IsInstanceOfType(element, typeof(ListElement));
            ListElement list = (ListElement)element;
            Assert.AreEqual("This template is [[xxx]]<br />\r\n", list[0].ToString());
            Assert.AreEqual("<noWiki><nowiki>sample</nowiki>", list[1].ToString());
            Assert.AreEqual("</nowiki>\r\n", list[2].ToString());
            Assert.AreEqual("<inclUdeonly><p>include text<nowiki><includeonly>sample</includeonly></nowiki></p></includeonly>", list[3].ToString());
            Assert.AreEqual("\r\n", list[4].ToString());
            Assert.AreEqual("<noInclude>[[ja:Template:sample/doc]]<!--noinclude only--></noinclude>", list[5].ToString());
            Assert.AreEqual("\r\n", list[6].ToString());
            Assert.AreEqual("<!-- <includeonly>include only comment</includeonly> -->", list[7].ToString());
            Assert.AreEqual(8, list.Count);

            // 各要素の確認
            Assert.IsInstanceOfType(list[0], typeof(TextElement));

            // nowikiとコメントは再帰的に解析されない
            Assert.IsInstanceOfType(list[1], typeof(XmlElement));
            xml = (XmlElement)list[1];
            Assert.AreEqual("<nowiki>sample", xml[0].ToString());
            Assert.IsInstanceOfType(xml[0], typeof(XmlTextElement));
            Assert.IsInstanceOfType(list[7], typeof(XmlCommentElement));

            // includeonly, noincludeは再帰的に処理
            Assert.IsInstanceOfType(list[3], typeof(XmlElement));
            xml = (XmlElement)list[3];
            Assert.AreEqual("<p>include text", xml[0].ToString());
            Assert.AreEqual("<nowiki><includeonly>sample</includeonly></nowiki>", xml[1].ToString());
            Assert.IsInstanceOfType(xml[1], typeof(XmlElement));
            Assert.AreEqual("</p>", xml[2].ToString());
            Assert.AreEqual(3, xml.Count);

            Assert.IsInstanceOfType(list[5], typeof(XmlElement));
            xml = (XmlElement)list[5];
            Assert.AreEqual("[[ja:Template:sample/doc]]", xml[0].ToString());
            Assert.IsInstanceOfType(xml[0], typeof(TextElement));
            Assert.AreEqual("<!--noinclude only-->", xml[1].ToString());
            Assert.IsInstanceOfType(xml[1], typeof(XmlCommentElement));
            Assert.AreEqual(2, xml.Count);
        }

        /// <summary>
        /// <see cref="IParser.Parse"/>メソッドテストトケース（onlyinclude）。
        /// </summary>
        [TestMethod]
        public void TestParseOnlyinclude()
        {
            IElement element;
            XmlElement xml;
            using (MediaWikiPreparser parser = new MediaWikiPreparser())
            {
                element = parser.Parse(TestData);
            }

            // onlyincludeが存在するケース、解析時点では特に他のタグと同じ扱い
            // ※ 前半部分はTestParseと同じデータなので割愛
            Assert.AreEqual(TestData, element.ToString());
            Assert.IsInstanceOfType(element, typeof(ListElement));
            ListElement list = (ListElement)element;
            Assert.AreEqual("<onlyinclude><noinclude>インクルード時は</noinclude>ここしか</onlyinclude>", list[8].ToString());
            Assert.AreEqual(", ", list[9].ToString());
            Assert.AreEqual("<onlyinclude>有効にならない</onlyinclude>", list[10].ToString());
            Assert.AreEqual(11, list.Count);

            // onlyincludeも再帰的に処理
            Assert.IsInstanceOfType(list[8], typeof(XmlElement));
            xml = (XmlElement)list[8];
            Assert.AreEqual("<noinclude>インクルード時は</noinclude>", xml[0].ToString());
            Assert.IsInstanceOfType(xml[0], typeof(XmlElement));
            Assert.AreEqual("ここしか", xml[1].ToString());
            Assert.AreEqual(2, xml.Count);
        }

        #endregion

        #region 公開メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiPreparser.FilterByInclude"/>メソッドテストケース。
        /// </summary>
        /// <remarks>
        /// <see cref="TestParse"/>と同じデータを使うため、そちらのテストが通っていることを前提とする。
        /// </remarks>
        [TestMethod]
        public void TestFilterByInclude()
        {
            IElement element;
            ListElement list;
            using (MediaWikiPreparser parser = new MediaWikiPreparser())
            {
                element = parser.Parse(TestDataWithoutOnlyinclude);
                parser.FilterByInclude(ref element);
            }

            // includeonlyが展開され、noinclude, コメントが削除される
            Assert.IsInstanceOfType(element, typeof(ListElement));
            list = (ListElement)element;
            Assert.AreEqual("This template is [[xxx]]<br />\r\n", list[0].ToString());
            Assert.AreEqual("<noWiki><nowiki>sample</nowiki>", list[1].ToString());
            Assert.AreEqual("</nowiki>\r\n", list[2].ToString());
            Assert.AreEqual("<p>include text<nowiki><includeonly>sample</includeonly></nowiki></p>", list[3].ToString());
            Assert.AreEqual("\r\n", list[4].ToString());
            Assert.AreEqual("\r\n", list[5].ToString());
            Assert.AreEqual(6, list.Count);

            // 各要素の確認
            Assert.IsInstanceOfType(list[0], typeof(TextElement));
            Assert.IsInstanceOfType(list[1], typeof(XmlElement));

            // includeonlyはListElementに置き換わる
            Assert.IsInstanceOfType(list[3], typeof(ListElement));
            list = (ListElement)list[3];
            Assert.AreEqual("<p>include text", list[0].ToString());
            Assert.AreEqual("<nowiki><includeonly>sample</includeonly></nowiki>", list[1].ToString());
            Assert.IsInstanceOfType(list[1], typeof(XmlElement));
            Assert.AreEqual("</p>", list[2].ToString());
            Assert.AreEqual(3, list.Count);
        }

        /// <summary>
        /// <see cref="MediaWikiPreparser.FilterByInclude"/>メソッドテストケース（onlyinclude）。
        /// </summary>
        /// <remarks>
        /// <see cref="TestParseOnlyinclude"/>と同じデータを使うため、そちらのテストが通っていることを前提とする。
        /// </remarks>
        [TestMethod]
        public void TestFilterByIncludeOnlyinclude()
        {
            IElement element;
            ListElement list;
            using (MediaWikiPreparser parser = new MediaWikiPreparser())
            {
                element = parser.Parse(TestData);
                parser.FilterByInclude(ref element);
            }

            // onlyincludeが存在する場合、その外側は全て削除され、タグが展開される
            // ※ onlyincludeの内部にnoinclude等が存在する場合、それはそれで通常と同様処理される
            Assert.IsInstanceOfType(element, typeof(ListElement));
            list = (ListElement)element;
            Assert.AreEqual("ここしか", list[0].ToString());
            Assert.AreEqual("有効にならない", list[1].ToString());
            Assert.AreEqual(2, list.Count);

            // onlyincludeはListElementに置き換わる
            Assert.IsInstanceOfType(list[0], typeof(ListElement));
            Assert.IsInstanceOfType(list[1], typeof(ListElement));
        }

        /// <summary>
        /// <see cref="MediaWikiPreparser.FilterByInclude"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFilterByIncludeNull()
        {
            IElement element = null;
            using (MediaWikiPreparser parser = new MediaWikiPreparser())
            {
                parser.FilterByInclude(ref element);
            }
        }

        /// <summary>
        /// <see cref="MediaWikiPreparser.FilterByNoinclude"/>メソッドテストケース。
        /// </summary>
        /// <remarks>
        /// <see cref="TestParse"/>と同じデータを使うため、そちらのテストが通っていることを前提とする。
        /// </remarks>
        [TestMethod]
        public void TestFilterByNoinclude()
        {
            IElement element;
            ListElement innerList;
            using (MediaWikiPreparser parser = new MediaWikiPreparser())
            {
                element = parser.Parse(TestData);
                parser.FilterByNoinclude(ref element);
            }

            // noinclude, onlyincludeが展開され、includeonly, コメントが削除される
            Assert.IsInstanceOfType(element, typeof(ListElement));
            ListElement list = (ListElement)element;
            Assert.AreEqual("This template is [[xxx]]<br />\r\n", list[0].ToString());
            Assert.AreEqual("<noWiki><nowiki>sample</nowiki>", list[1].ToString());
            Assert.AreEqual("</nowiki>\r\n", list[2].ToString());
            Assert.AreEqual("\r\n", list[3].ToString());
            Assert.AreEqual("[[ja:Template:sample/doc]]", list[4].ToString());
            Assert.AreEqual("\r\n", list[5].ToString());
            Assert.AreEqual("インクルード時はここしか", list[6].ToString());
            Assert.AreEqual(", ", list[7].ToString());
            Assert.AreEqual("有効にならない", list[8].ToString());
            Assert.AreEqual(9, list.Count);

            // 各要素の確認
            Assert.IsInstanceOfType(list[0], typeof(TextElement));
            Assert.IsInstanceOfType(list[1], typeof(XmlElement));

            // noinclude, onlyincludeはListElementに置き換わる
            Assert.IsInstanceOfType(list[4], typeof(ListElement));
            innerList = (ListElement)list[4];
            Assert.AreEqual("[[ja:Template:sample/doc]]", innerList[0].ToString());
            Assert.IsInstanceOfType(innerList[0], typeof(TextElement));
            Assert.AreEqual(1, innerList.Count);

            Assert.IsInstanceOfType(list[6], typeof(ListElement));
            innerList = (ListElement)list[6];
            Assert.AreEqual("インクルード時は", innerList[0].ToString());
            Assert.IsInstanceOfType(innerList[0], typeof(ListElement));
            Assert.AreEqual("ここしか", innerList[1].ToString());
            Assert.IsInstanceOfType(innerList[1], typeof(TextElement));
            Assert.AreEqual(2, innerList.Count);
        }

        /// <summary>
        /// <see cref="MediaWikiPreparser.FilterByNoinclude"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestFilterByNoincludeNull()
        {
            IElement element = null;
            using (MediaWikiPreparser parser = new MediaWikiPreparser())
            {
                parser.FilterByNoinclude(ref element);
            }
        }

        #endregion

        #region 静的メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiPreparser.PreprocessByInclude"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestPreprocessByInclude()
        {
            // Parse→FilterByIncludeした結果をToStringしたものが返る
            Assert.AreEqual(
                "This template is [[xxx]]<br />\r\n<noWiki><nowiki>sample</nowiki></nowiki>\r\n"
                + "<p>include text<nowiki><includeonly>sample</includeonly></nowiki></p>\r\n\r\n",
                MediaWikiPreparser.PreprocessByInclude(TestDataWithoutOnlyinclude));
            Assert.AreEqual(
                "ここしか有効にならない",
                MediaWikiPreparser.PreprocessByInclude(TestData));
            Assert.AreEqual(string.Empty, MediaWikiPreparser.PreprocessByInclude(string.Empty));
        }

        /// <summary>
        /// <see cref="MediaWikiPreparser.PreprocessByInclude"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPreprocessByIncludeNull()
        {
            MediaWikiPreparser.PreprocessByInclude(null);
        }

        /// <summary>
        /// <see cref="MediaWikiPreparser.PreprocessByNoinclude"/>メソッドテストケース。
        /// </summary>
        [TestMethod]
        public void TestPreprocessByNoinclude()
        {
            // Parse→FilterByNoincludeした結果をToStringしたものが返る
            Assert.AreEqual(
                "This template is [[xxx]]<br />\r\n<noWiki><nowiki>sample</nowiki></nowiki>\r\n\r\n"
                + "[[ja:Template:sample/doc]]\r\nインクルード時はここしか, 有効にならない",
                MediaWikiPreparser.PreprocessByNoinclude(TestData));
            Assert.AreEqual(string.Empty, MediaWikiPreparser.PreprocessByNoinclude(string.Empty));
        }

        /// <summary>
        /// <see cref="MediaWikiPreparser.PreprocessByNoinclude"/>メソッドテストケース（null）。
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPreprocessByNoincludeNull()
        {
            MediaWikiPreparser.PreprocessByNoinclude(null);
        }

        #endregion
    }
}
