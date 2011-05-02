// ================================================================================================
// <summary>
//      XML/HTMLのコメント要素をあらわすモデルクラスソース</summary>
//
// <copyright file="CommentElement.cs" company="honeplusのメモ帳">
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
    public class CommentElement : TextElement
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
        public CommentElement(string comment = null)
            : base(comment)
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// このコメント要素のテキスト。
        /// </summary>
        /// <remarks>プロパティ更新時は<see cref="ParsedString"/>を破棄する。</remarks>
        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
                this.ParsedString = null;
            }
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 渡されたHTMLテキストがコメントかを解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <returns>解析したコメント。</returns>
        /// <exception cref="FormatException">文字列が解析できないフォーマットの場合。</exception>
        /// <remarks>
        /// コメントと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// </remarks>
        public static CommentElement ParseLazy(string s)
        {
            CommentElement result;
            if (CommentElement.TryParseLazy(s, out result))
            {
                return result;
            }

            throw new FormatException("Invalid String : " + s);
        }

        /// <summary>
        /// 渡されたHTMLテキストがコメントかを解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="result">解析したコメント。</param>
        /// <returns>コメントの場合<c>true</c>。</returns>
        /// <remarks>
        /// コメントと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// </remarks>
        public static bool TryParseLazy(string s, out CommentElement result)
        {
            // 入力値確認
            result = null;
            if (String.IsNullOrEmpty(s) || !s.StartsWith(CommentElement.delimiterStart))
            {
                return false;
            }

            // コメント終了まで取得
            int index = s.IndexOf(CommentElement.delimiterEnd, CommentElement.delimiterStart.Length);
            if (index < 0)
            {
                // 閉じタグが存在しない場合、最後までコメントと判定
                result = new CommentElement(s.Substring(CommentElement.delimiterStart.Length));

                // 不正な構文を保持するため、元の文字列を保持する
                result.ParsedString = s;
                return true;
            }

            // 閉じタグがあった場合、閉じタグまでを返す
            // ※ この場合元文字列は要らない
            result = new CommentElement(s.Substring(
                CommentElement.delimiterStart.Length,
                index - CommentElement.delimiterStart.Length));
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
            return CommentElement.delimiterStart[0] == c;
        }

        #endregion
        
        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// このコメント要素の書式化して返す。
        /// </summary>
        /// <returns>このコメント要素のテキスト。</returns>
        protected override string ToStringImpl()
        {
            return CommentElement.delimiterStart + StringUtils.DefaultString(this.Text) + CommentElement.delimiterEnd;
        }

        #endregion
    }
}
