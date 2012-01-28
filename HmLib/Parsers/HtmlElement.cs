// ================================================================================================
// <summary>
//      ページのHTML要素をあらわすモデルクラスソース</summary>
//
// <copyright file="HtmlElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    /// <summary>
    /// ページのHTML要素をあらわすモデルクラスです。
    /// </summary>
    /// <remarks>解析処理は複雑なため、<see cref="XmlParser"/>として別途実装。</remarks>
    public class HtmlElement : XmlElement
    {
        #region コンストラクタ

        /// <summary>
        /// 指定されたタグ名・属性・値からHTML要素を生成する。
        /// </summary>
        /// <param name="name">タグ名。</param>
        /// <param name="attributes">属性。</param>
        /// <param name="innerElements">値。</param>
        /// <param name="parsedString">Parse解析時の元の文字列。</param>
        public HtmlElement(
            string name,
            IDictionary<string, string> attributes,
            ICollection<IElement> innerElements,
            string parsedString = null)
            : base(name, attributes, innerElements, parsedString)
        {
        }

        /// <summary>
        /// 指定されたタグ名・値からHTML要素を生成する。
        /// </summary>
        /// <param name="name">タグ名。</param>
        /// <param name="value">値。未指定時は<c>null</c>。</param>
        public HtmlElement(string name, string value = null)
            : base(name, value)
        {
        }

        #endregion

        #region 内部実装メソッド

        /// <summary>
        /// このXML要素を表す文字列を返す。
        /// </summary>
        /// <returns>このXML要素を表す文字列。</returns>
        protected override string ToStringImpl()
        {
            StringBuilder b = new StringBuilder();

            // 開始タグ
            b.Append('<');
            b.Append(WebUtility.HtmlEncode(this.Name));
            
            // 属性
            foreach (KeyValuePair<string, string> attr in this.Attributes)
            {
                b.Append(' ');
                b.Append(WebUtility.HtmlEncode(attr.Key));
                b.Append("=\"");
                b.Append(WebUtility.HtmlEncode(attr.Value));
                b.Append('"');
            }

            // 開始タグ閉じ文字
            b.Append('>');

            // コンテンツ
            foreach (IElement element in this)
            {
                // エンコードする／しないは中身の責任として、ここではエンコードしない
                // ※ エンコードするテキストを用意したい場合はHtmlTextElementを使うなど
                b.Append(element.ToString());
            }

            // 閉じタグは中身がある場合のみ
            if (this.Count > 0)
            {
                b.Append("</");
                b.Append(WebUtility.HtmlEncode(this.Name));
                b.Append('>');
            }

            return b.ToString();
        }

        #endregion
    }
}
