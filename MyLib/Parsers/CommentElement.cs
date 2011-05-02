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

    /// <summary>
    /// XML/HTMLのコメント要素をあらわすモデルクラスです。
    /// </summary>
    public class CommentElement : TextElement
    {
        #region 定数

        /// <summary>
        /// コメントの開始タグ。
        /// </summary>
        private static readonly string startSign = "<!--";

        /// <summary>
        /// コメントの閉じタグ。
        /// </summary>
        private static readonly string endSign = "-->";

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
        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                // プロパティ更新時は元文字列は破棄する
                base.Text = value;
                this.Original = null;
            }
        }

        /// <summary>
        /// Parse等によりインスタンスを生成した場合の元文字列。
        /// </summary>
        protected virtual string Original
        {
            get;
            set;
        }

        #endregion

        #region 静的メソッド

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
            if (String.IsNullOrEmpty(s) || !s.StartsWith(CommentElement.startSign))
            {
                return false;
            }

            // コメント終了まで取得
            int index = s.IndexOf(CommentElement.endSign, CommentElement.startSign.Length);
            if (index < 0)
            {
                // 閉じタグが存在しない場合、最後までコメントと判定
                result = new CommentElement(s.Substring(CommentElement.startSign.Length));

                // 不正な構文を保持するため、元の文字列を保持する
                result.Original = s;
                return true;
            }

            // 閉じタグがあった場合、閉じタグまでを返す
            result = new CommentElement(s.Substring(
                CommentElement.startSign.Length,
                index - CommentElement.startSign.Length));
            return true;
        }

        #endregion

        #region インタフェース実装メソッド

        /// <summary>
        /// このコメント要素の書式化して返す。
        /// </summary>
        /// <returns>このコメント要素のテキスト。</returns>
        public override string ToString()
        {
            // 元文字列があればそのまま返す
            if (this.Original != null)
            {
                return this.Original;
            }

            return CommentElement.startSign + base.ToString() + CommentElement.endSign;
        }

        #endregion
    }
}
