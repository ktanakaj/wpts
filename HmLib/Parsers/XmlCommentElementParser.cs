// ================================================================================================
// <summary>
//      XMLのコメント要素を解析するためのクラスソース</summary>
//
// <copyright file="XmlCommentElementParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;

    /// <summary>
    /// XMLのコメント要素を解析するためのクラスです。
    /// </summary>
    public class XmlCommentElementParser : AbstractParser
    {
        #region インタフェース実装メソッド

        /// <summary>
        /// 渡されたテキストをXMLコメントとして解析する。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析したタグ。</param>
        /// <returns>タグの場合<c>true</c>。</returns>
        /// <remarks>
        /// XML/HTMLタグと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// </remarks>
        public override bool TryParse(string s, out IElement result)
        {
            // 入力値確認
            result = null;
            if (String.IsNullOrEmpty(s) || !s.StartsWith(XmlCommentElement.DelimiterStart))
            {
                return false;
            }

            // コメント終了まで取得
            XmlCommentElement comment = new XmlCommentElement();
            int index = s.IndexOf(XmlCommentElement.DelimiterEnd, XmlCommentElement.DelimiterStart.Length);
            if (index < 0)
            {
                // 閉じタグが存在しない場合、最後までコメントと判定
                comment.Raw = s.Substring(XmlCommentElement.DelimiterStart.Length);
                comment.ParsedString = s;
            }
            else
            {
                // 閉じタグがあった場合、閉じタグまでを返す
                comment.Raw = s.Substring(
                    XmlCommentElement.DelimiterStart.Length,
                    index - XmlCommentElement.DelimiterStart.Length);
                comment.ParsedString = s.Substring(0, index + XmlCommentElement.DelimiterEnd.Length);
            }

            result = comment;
            return true;
        }

        /// <summary>
        /// 渡された文字が<see cref="Parse"/>, <see cref="TryParse"/>の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。このクラスでは常に<c>true</c>を返す。</returns>
        /// <remarks>性能対策などで<see cref="TryParse"/>を呼ぶ前に目処を付けたい場合用。</remarks>
        public override bool IsPossibleParse(char c)
        {
            return XmlCommentElement.DelimiterStart[0] == c;
        }

        #endregion
    }
}
