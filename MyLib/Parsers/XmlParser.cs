// ================================================================================================
// <summary>
//      XML/HTMLテキストを解析するためのクラスソース</summary>
//
// <copyright file="XmlParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Honememo.Utilities;

    /// <summary>
    /// XML/HTMLテキストを解析するためのクラスです。
    /// </summary>
    /// <remarks>HTMLについては、解析はできるもののほぼXml用のElementで結果が返されます。</remarks>
    public class XmlParser : AbstractParser
    {
        #region private変数

        /// <summary>
        /// 大文字小文字を無視するか？
        /// </summary>
        private bool ignoreCase = true;

        #endregion

        #region プロパティ

        /// <summary>
        /// 大文字小文字を無視するか？
        /// </summary>
        public bool IgnoreCase
        {
            get
            {
                return this.ignoreCase;
            }

            set
            {
                this.ignoreCase = value;
            }
        }

        /// <summary>
        /// タグはHTMLの書式か？
        /// </summary>
        public bool IsHtml
        {
            get;
            set;
        }

        #endregion
        
        #region XmlParser, XmlElementPaser共通メソッド

        /// <summary>
        /// 文字列をXML/HTML読み込み用にデコードする。
        /// </summary>
        /// <param name="str">デコードする文字列。</param>
        /// <returns>デコードされた文字列。<c>null</c>の場合、空文字列を返す。</returns>
        internal string Decode(string str)
        {
            if (str == null)
            {
                return String.Empty;
            }
            else if (this.IsHtml)
            {
                return System.Web.HttpUtility.HtmlDecode(str);
            }
            else
            {
                return XmlUtils.XmlDecode(str);
            }
        }

        #endregion

        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// 渡された位置の文字列が<see cref="TryParseElements"/>の候補となるかを判定する。
        /// </summary>
        /// <param name="s">全体文字列。</param>
        /// <param name="index">処理インデックス。</param>
        /// <returns>候補となる場合<c>true</c>。</returns>
        protected override bool IsElementPossible(string s, int index)
        {
            char c = s[index];
            return XmlCommentElement.IsPossibleParse(c)
                || new XmlElementParser(this).IsPossibleParse(c);
        }

        /// <summary>
        /// 渡されたテキストを各種解析処理で解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="result">解析した結果要素。</param>
        /// <returns>解析できた場合<c>true</c>。</returns>
        protected override bool TryParseElements(string s, out IElement result)
        {
            XmlCommentElement commentElement;
            IElement element;
            if (XmlCommentElement.TryParseLazy(s, out commentElement))
            {
                result = commentElement;
                return true;
            }
            else if (new XmlElementParser(this).TryParse(s, out element))
            {
                result = element;
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// 文字列が空でない場合、リストにTextエレメントを追加して、文字列をリセットする。
        /// </summary>
        /// <param name="list">追加されるリスト。</param>
        /// <param name="b">追加する文字列。</param>
        protected override void FlashText(ref ListElement list, ref StringBuilder b)
        {
            if (b.Length > 0)
            {
                string s = b.ToString();
                XmlTextElement e = new XmlTextElement(this.Decode(s));
                e.ParsedString = s;
                list.Add(e);
                b.Clear();
            }
        }

        #endregion
    }
}
