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

        /// <summary>
        /// ページの本文。
        /// </summary>
        private string text;

        /// <summary>
        /// ページのタイムスタンプ。
        /// </summary>
        private DateTime? timestamp;

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
        public Website Website
        {
            get
            {
                return this.website;
            }

            protected set
            {
                // ウェブサイトは必須
                if (value == null)
                {
                    throw new ArgumentNullException("website");
                }

                this.website = value;
            }
        }

        /// <summary>
        /// ページタイトル。
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            protected set
            {
                // ページタイトルは必須
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("title");
                }

                this.title = value;
            }
        }
        
        /// <summary>
        /// ページの本文。
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            protected set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// ページのタイムスタンプ。
        /// </summary>
        public DateTime? Timestamp
        {
            get
            {
                return this.timestamp;
            }

            protected set
            {
                this.timestamp = value;
            }
        }
        
        #endregion
    }
}
