// ================================================================================================
// <summary>
//      ウェブサイトをあらわすモデルクラスソース</summary>
//
// <copyright file="Website.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.IO;
    using System.Net;
    using Honememo.Models;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// ウェブサイトをあらわすモデルクラスです。
    /// </summary>
    /// <remarks>言語が異なる場合は、別のウェブサイトとして扱います。</remarks>
    public abstract class Website
    {
        #region private変数

        /// <summary>
        /// ウェブサイトの場所。
        /// </summary>
        /// <example>http://en.wikipedia.org</example>
        /// <remarks>動作確認はhttpとfileスキームのみ。</remarks>
        private string location;

        /// <summary>
        /// ウェブサイトの言語。
        /// </summary>
        private Language language;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <remarks>継承クラスでは忘れずに
        /// <see cref="Location"/>, <see cref="Language"/>
        /// の設定を行ってください。</remarks>
        public Website()
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// ウェブサイトの場所。
        /// </summary>
        /// <example>http://en.wikipedia.org</example>
        /// <remarks>動作確認はhttpとfileスキームのみ。</remarks>
        public string Location
        {
            get
            {
                return this.location;
            }

            set
            {
                // ※必須な情報が設定されていない場合、例外を返す
                this.location = Validate.NotBlank(value, "location");
            }
        }

        /// <summary>
        /// ウェブサイトの言語。
        /// </summary>
        public Language Language
        {
            get
            {
                return this.language;
            }

            protected set
            {
                // ※必須な情報が設定されていない場合、例外を返す
                this.language = Validate.NotNull(value, "language");
            }
        }

        #endregion
        
        #region メソッド

        /// <summary>
        /// ページを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <returns>取得したページ。</returns>
        /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
        public abstract Page GetPage(string title);

        #endregion
    }
}
