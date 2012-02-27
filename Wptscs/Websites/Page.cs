// ================================================================================================
// <summary>
//      ページ（Wikipediaの記事など）をあらわすモデルクラスソース</summary>
//
// <copyright file="Page.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using Honememo.Utilities;

    /// <summary>
    /// ページ（Wikipediaの記事など）をあらわすモデルクラスです。
    /// </summary>
    public class Page
    {
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
        /// 指定されたウェブサイトの渡されたタイトル, 本文, タイムスタンプのページを作成。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        /// <param name="text">ページの本文。</param>
        /// <param name="timestamp">ページのタイムスタンプ。</param>
        /// <param name="uri">ページのURI。</param>
        /// <exception cref="ArgumentNullException"><paramref name="website"/>または<paramref name="title"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentException"><paramref name="title"/>が空の文字列の場合。</exception>
        public Page(Website website, string title, string text, DateTime? timestamp, Uri uri)
        {
            // 初期値設定、基本的に以降外から変更されることを想定しない
            this.Website = website;
            this.Title = title;
            this.Text = text;
            this.Timestamp = timestamp;
            this.Uri = uri;
        }

        /// <summary>
        /// 指定されたウェブサイトの渡されたタイトル, 本文のページを作成。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        /// <param name="text">ページの本文。</param>
        /// <remarks>ページのタイムスタンプ, URIには<c>null</c>を設定。</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="website"/>または<paramref name="title"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentException"><paramref name="title"/>が空の文字列の場合。</exception>
        public Page(Website website, string title, string text)
            : this(website, title, text, null, null)
        {
        }

        /// <summary>
        /// 指定されたウェブサイトの渡されたタイトルのページを作成。
        /// </summary>
        /// <param name="website">ページが所属するウェブサイト。</param>
        /// <param name="title">ページタイトル。</param>
        /// <remarks>ページの本文, タイムスタンプ, URIには<c>null</c>を設定。</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="website"/>または<paramref name="title"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentException"><paramref name="title"/>が空の文字列の場合。</exception>
        public Page(Website website, string title)
            : this(website, title, null, null, null)
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// ページが所属するウェブサイト。
        /// </summary>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        public virtual Website Website
        {
            get
            {
                return this.website;
            }

            protected set
            {
                this.website = Validate.NotNull(value);
            }
        }

        /// <summary>
        /// ページタイトル。
        /// </summary>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        /// <exception cref="ArgumentException">空文字列が指定された場合。</exception>
        public virtual string Title
        {
            get
            {
                return this.title;
            }

            protected set
            {
                this.title = Validate.NotBlank(value);
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

        /// <summary>
        /// ページのURI。
        /// </summary>
        public virtual Uri Uri
        {
            get;
            protected set;
        }
        
        #endregion
    }
}
