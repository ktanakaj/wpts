// ================================================================================================
// <summary>
//      XMLのコメント要素をあらわすモデルクラスソース</summary>
//
// <copyright file="XmlCommentElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using Honememo.Utilities;

    /// <summary>
    /// XML/HTMLのコメント要素をあらわすモデルクラスです。
    /// </summary>
    public class XmlCommentElement : XmlTextElement
    {
        #region 定数

        /// <summary>
        /// コメントの開始タグ。
        /// </summary>
        public static readonly string DelimiterStart = "<!--";

        /// <summary>
        /// コメントの閉じタグ。
        /// </summary>
        public static readonly string DelimiterEnd = "-->";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたコメントを用いて要素を作成する。
        /// </summary>
        /// <param name="comment">コメント文字列、未指定時は<c>null</c>。</param>
        public XmlCommentElement(string comment = null)
            : base(comment)
        {
        }

        #endregion

        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// このコメント要素の書式化して返す。
        /// </summary>
        /// <returns>このコメント要素のテキスト。</returns>
        protected override string ToStringImpl()
        {
            return XmlCommentElement.DelimiterStart + base.ToStringImpl() + XmlCommentElement.DelimiterEnd;
        }

        #endregion
    }
}
