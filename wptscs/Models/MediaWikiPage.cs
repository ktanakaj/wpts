// ================================================================================================
// <summary>
//      MediaWikiのページをあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiPage.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// MediaWikiのページをあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiPage : Page
    {
        #region private変数

        /// <summary>
        /// ページのXMLデータ。
        /// </summary>
        private XmlDocument xml;

        /// <summary>
        /// リダイレクト先のページ名。
        /// </summary>
        private string redirect;

        #endregion

        #region プロパティ

        /// <summary>
        /// ページのXMLデータ。
        /// </summary>
        public XmlDocument Xml
        {
            get
            {
                return this.xml;
            }

            protected set
            {
                this.xml = value;
            }
        }

        /// <summary>
        /// リダイレクト先のページ名。
        /// </summary>
        public string Redirect
        {
            get
            {
                return this.redirect;
            }

            protected set
            {
                this.redirect = value;
            }
        }

        #endregion
    }
}
