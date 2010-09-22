// ================================================================================================
// <summary>
//      ページ（Wikipediaの記事など）をあらわすモデルクラスソース</summary>
//
// <copyright file="Page.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Linq;
    using System.Text;
    using Honememo.Utilities;

    /// <summary>
    /// ページ（Wikipediaの記事など）をあらわすモデルクラスです。
    /// </summary>
    public class Page
    {
        #region 定数宣言

        /// <summary>
        /// コメントの開始。
        /// </summary>
        public static readonly string CommentStart = "<!--";

        /// <summary>
        /// コメントの終了。
        /// </summary>
        public static readonly string CommentEnd = "-->";

        #endregion

        #region private変数

        /// <summary>
        /// ページが所属するウェブサイト。
        /// </summary>
        private Website website;

        /// <summary>
        /// ページタイトル。
        /// </summary>
        private string title;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        /// <param name="text">ページの本文。</param>
        /// <param name="timestamp">ページのタイムスタンプ。</param>
        public Page(Website website, string title, string text, DateTime? timestamp)
        {
            // 初期値設定、基本的に以後外から変更されることを想定しない
            this.Website = website;
            this.Title = title;
            this.Text = text;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// コンストラクタ。
        /// ページのタイムスタンプには<c>null</c>を設定。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        /// <param name="text">ページの本文。</param>
        public Page(Website website, string title, string text)
            : this(website, title, text, null)
        {
        }

        /// <summary>
        /// コンストラクタ。
        /// ページの本文, タイムスタンプには<c>null</c>を設定。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        public Page(Website website, string title)
            : this(website, title, null, null)
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// ページが所属するウェブサイト。
        /// </summary>
        public virtual Website Website
        {
            get
            {
                return this.website;
            }

            protected set
            {
                // ウェブサイトは必須
                this.website = Validate.NotNull(value, "website");
            }
        }

        /// <summary>
        /// ページタイトル。
        /// </summary>
        public virtual string Title
        {
            get
            {
                return this.title;
            }

            protected set
            {
                // ページタイトルは必須
                this.title = Validate.NotBlank(value, "title");
            }
        }
        
        /// <summary>
        /// ページの本文。
        /// </summary>
        public virtual string Text
        {
            get;
            protected set;
        }

        /// <summary>
        /// ページのタイムスタンプ。
        /// </summary>
        public virtual DateTime? Timestamp
        {
            get;
            protected set;
        }
        
        #endregion

        #region 公開静的メソッド

        /// <summary>
        /// 渡されたHTMLテキストがコメントかを指定された位置から解析する。
        /// </summary>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="startIndex">解析開始インデックス。</param>
        /// <param name="comment">解析したコメント。</param>
        /// <returns>コメントの場合<c>true</c>。</returns>
        /// <remarks>
        /// コメントと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// </remarks>
        public static bool TryParseComment(string text, int startIndex, out string comment)
        {
            // 入力値確認
            comment = null;
            if (!StringUtils.StartsWith(text, Page.CommentStart, startIndex))
            {
                return false;
            }

            // コメント終了まで取得
            int endIndex = text.IndexOf(Page.CommentEnd, startIndex + Page.CommentStart.Length);
            if (endIndex < 0)
            {
                // 閉じタグが存在しない場合、最後までコメントと判定
                comment = text.Substring(startIndex);
                return true;
            }

            // 閉じタグがあった場合、閉じタグの終わりまでを返す
            comment = text.Substring(startIndex, endIndex - startIndex + Page.CommentEnd.Length);
            return true;
        }

        /// <summary>
        /// 渡されたHTMLテキストがコメントかを解析する。
        /// </summary>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="comment">解析したコメント。</param>
        /// <returns>コメントの場合<c>true</c>。</returns>
        /// <remarks>
        /// コメントと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// </remarks>
        public static bool TryParseComment(string text, out string comment)
        {
            // オーバーロードメソッドをコール
            return Page.TryParseComment(text, 0, out comment);
        }

        #endregion

        #region 公開インスタンスメソッド

        /// <summary>
        /// 本文の指定された位置からがコメントかを解析する。
        /// </summary>
        /// <param name="startIndex">解析開始インデックス。</param>
        /// <param name="comment">解析したコメント。</param>
        /// <returns>コメントの場合<c>true</c>。</returns>
        /// <remarks>
        /// コメントと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// </remarks>
        public bool TryParseComment(int startIndex, out string comment)
        {
            // オーバーロードメソッドをコール
            return Page.TryParseComment(this.Text, startIndex, out comment);
        }

        #endregion
        
        #region 実装支援用静的メソッド

        /// <summary>
        /// 渡されたテキストが指定されたタグブロックかを解析する。
        /// </summary>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="startIndex">解析開始インデックス。</param>
        /// <param name="tag">解析するタグ。</param>
        /// <param name="value">解析したタグブロック。</param>
        /// <returns>タグブロックの場合<c>true</c>。</returns>
        /// <exception cref="ArgumentNullException">タグが<c>null</c>。</exception>
        /// <exception cref="ArgumentException">タグが空か空白のみ。</exception>
        /// <remarks>
        /// タグブロックと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// また、本メソッドはあくまでMediaWikiの簡易的な構文用であり、入れ子は考慮しない。
        /// 大文字小文字も区別しない。厳格でない構文も受け入れる。
        /// </remarks>
        protected static bool TryParseTag(string text, int startIndex, string tag, out string value)
        {
            // TODO: 書いてたらどんどん膨らんできたので、もうタグ解析は別クラスにすべき

            // 初期化と入力値確認、タグだけは必須
            value = null;
            string lowerTag = Validate.NotBlank(tag).ToLower();
            if (String.IsNullOrEmpty(text) || text.ElementAtOrDefault<char>(startIndex) != '<')
            {
                // 空または1文字目が違う場合ここで終了
                // ※ 1文字目をチェックしてるのは余計な判定を避けるため
                return false;
            }

            // 開始タグを確認、比較用に小文字に変換する
            string subtext = text.Substring(startIndex).ToLower();
            if (!StringUtils.StartsWith(subtext, lowerTag, 1))
            {
                return false;
            }

            // 開始タグ文字列の後が普通の文字ではない（<p>のつもりで<pre>等）のチェック
            char c = subtext.ElementAtOrDefault<char>(lowerTag.Length + 1);
            if (!Char.IsWhiteSpace(c) && !Char.IsSymbol(c))
            {
                // 空白・記号以外は不可（厳密には違うけどこれで普通に存在するパターンはカバー）
                return false;
            }

            // 開始タグの終了位置
            int index = subtext.IndexOf('>');
            if (index < 0)
            {
                // 閉じてないのでタグではない
                return false;
            }

            // ブロック終了まで取得
            string endTag = new StringBuilder("</").Append(lowerTag).ToString();
            for (int i = index + 1; i < subtext.Length; i++)
            {
                // 終了条件のチェック
                if (StringUtils.StartsWith(subtext, endTag, i))
                {
                    // 終了タグが見つかった場合、その最終文字までをタグブロックと判断
                    // ※ 見つからない場合は、最終的に全部の条件に入る
                    int endIndex = subtext.IndexOf('>', i);
                    if (endIndex >= 0)
                    {
                        value = text.Substring(startIndex, endIndex + 1);
                    }

                    break;
                }

                // コメント（<!--）のチェック
                string comment;
                if (Page.TryParseComment(subtext, i, out comment))
                {
                    i += comment.Length - 1;
                }
            }

            // 終わりが見つからない場合は、開始タグ以降を全てタグブロックと判断
            if (value == null)
            {
                value = text.Substring(startIndex);
            }

            return true;
        }

        #endregion
    }
}
