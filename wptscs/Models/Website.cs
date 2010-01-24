// ================================================================================================
// <summary>
//      ウェブサイトをあらわすモデルクラスソース</summary>
//
// <copyright file="Website.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// ウェブサイトをあらわすモデルクラスです。
    /// </summary>
    /// <remarks>言語が異なる場合は、別のウェブサイトとして扱います。</remarks>
    public abstract class Website
    {
        #region private変数

        /// <summary>
        /// サーバー名（ドメイン or IPアドレス）。
        /// </summary>
        private string server;

        /// <summary>
        /// ウェブサイトの言語。
        /// </summary>
        private Language lang;

        #endregion

        #region コンストラクタ
        
        /// <summary>
        /// コンストラクタ（シリアライズ用）。
        /// </summary>
        public Website()
            : this(new Language("unknown"))
        {
            // ↑適当な値で通常のコンストラクタを実行
            System.Diagnostics.Debug.WriteLine("Website.Website : 推奨されないコンストラクタを使用しています");
        }

        /// <summary>
        /// コンストラクタ（通常）。
        /// </summary>
        /// <param name="lang">ウェブサイトの言語。</param>
        public Website(Language lang)
        {
            // メンバ変数の初期設定
            this.lang = lang;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// サーバー名（ドメイン or IPアドレス）。
        /// </summary>
        public string Server
        {
            get
            {
                return this.server;
            }

            protected set
            {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("server");
                }

                this.server = value;
            }
        }

        /// <summary>
        /// ウェブサイトの言語。
        /// </summary>
        public Language Lang
        {
            get
            {
                return this.lang;
            }

            protected set
            {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if (value == null)
                {
                    throw new ArgumentNullException("lang");
                }

                this.lang = value;
            }
        }

        /// <summary>
        /// ページを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <returns>取得したページ。ページが存在しない場合は <c>null</c> を返す。</returns>
        /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
        public abstract Page this[string title]
        {
            get;
        }

        #endregion
    }
}
