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
        /// ページタイトル。
        /// </summary>
        private string title;

        /// <summary>
        /// ページタイトル。
        /// </summary>
        private string text;

        /// <summary>
        /// ページのタイムスタンプ。
        /// </summary>
        private DateTime timestamp;

        /// <summary>
        /// ページのURL。
        /// </summary>
        private Uri url;

        #endregion

        #region プロパティ

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
        public DateTime Timestamp
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

        /// <summary>
        /// ページのURL。
        /// </summary>
        public Uri Url
        {
            get
            {
                return this.url;
            }

            protected set
            {
                this.url = value;
            }
        }
        
        #endregion
    }
}
