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
        private static readonly string delimiterStart = "<!--";

        /// <summary>
        /// コメントの閉じタグ。
        /// </summary>
        private static readonly string delimiterEnd = "-->";

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

        #region 静的メソッド

        /// <summary>
        /// 渡されたXMLテキストがコメントかを解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <returns>解析したコメント。</returns>
        /// <exception cref="FormatException">文字列が解析できないフォーマットの場合。</exception>
        /// <remarks>
        /// コメントと判定するには、1文字目が開始タグである必要がある。
        /// </remarks>
        public static XmlCommentElement Parse(string s)
        {
            XmlCommentElement result;
            if (XmlCommentElement.TryParse(s, out result))
            {
                return result;
            }

            throw new FormatException("Invalid String : " + s);
        }

        /// <summary>
        /// 渡されたXMLテキストがコメントかを解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="result">解析したコメント。</param>
        /// <returns>コメントの場合<c>true</c>。</returns>
        /// <remarks>
        /// コメントと判定するには、1文字目が開始タグである必要がある。
        /// </remarks>
        public static bool TryParse(string s, out XmlCommentElement result)
        {
            // 処理の大半はLazyと同様
            if (!XmlCommentElement.TryParseLazy(s, out result))
            {
                return false;
            }

            // 閉じタグで終わっていない場合NGとする
            if (result.ParsedString.Length < XmlCommentElement.delimiterStart.Length + XmlCommentElement.delimiterEnd.Length
                || !result.ParsedString.EndsWith(XmlCommentElement.delimiterEnd))
            {
                result = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 渡されたXMLテキストがコメントかを解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="result">解析したコメント。</param>
        /// <returns>コメントの場合<c>true</c>。</returns>
        /// <remarks>
        /// コメントと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// </remarks>
        public static bool TryParseLazy(string s, out XmlCommentElement result)
        {
            // 入力値確認
            result = null;
            if (String.IsNullOrEmpty(s) || !s.StartsWith(XmlCommentElement.delimiterStart))
            {
                return false;
            }

            // コメント終了まで取得
            result = new XmlCommentElement();
            int index = s.IndexOf(XmlCommentElement.delimiterEnd, XmlCommentElement.delimiterStart.Length);
            if (index < 0)
            {
                // 閉じタグが存在しない場合、最後までコメントと判定
                result.Raw = s.Substring(XmlCommentElement.delimiterStart.Length);
                result.ParsedString = s;
            }
            else
            {
                // 閉じタグがあった場合、閉じタグまでを返す
                result.Raw = s.Substring(
                    XmlCommentElement.delimiterStart.Length,
                    index - XmlCommentElement.delimiterStart.Length);
                result.ParsedString = s.Substring(0, index + XmlCommentElement.delimiterEnd.Length);
            }

            return true;
        }

        /// <summary>
        /// 渡された文字が<c>TryParse</c>等の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。</returns>
        /// <remarks>性能対策などで処理自体を呼ばせたく無い場合用。</remarks>
        public static bool IsElementPossible(char c)
        {
            return XmlCommentElement.delimiterStart[0] == c;
        }

        #endregion
        
        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// このコメント要素の書式化して返す。
        /// </summary>
        /// <returns>このコメント要素のテキスト。</returns>
        protected override string ToStringImpl()
        {
            return XmlCommentElement.delimiterStart + base.ToStringImpl() + XmlCommentElement.delimiterEnd;
        }

        #endregion
    }
}
