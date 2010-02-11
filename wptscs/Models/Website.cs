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
    using System.IO;
    using System.Net;
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
        private string language;

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

            protected set
            {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("location");
                }

                this.location = value;
            }
        }

        /// <summary>
        /// ウェブサイトの言語。
        /// </summary>
        public string Language
        {
            get
            {
                return this.language;
            }

            protected set
            {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if (value == null)
                {
                    throw new ArgumentNullException("language");
                }

                this.language = value;
            }
        }

        #endregion
        
        #region メソッド

        /// <summary>
        /// ページを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <returns>取得したページ。ページが存在しない場合は <c>null</c> を返す。</returns>
        /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
        public abstract Page GetPage(string title);

        /// <summary>
        /// 指定されたURIの情報をストリームで取得。
        /// </summary>
        /// <param name="uri">取得対象のURI。</param>
        /// <returns>取得したストリーム。使用後は必ずクローズすること。</returns>
        /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
        protected Stream GetStream(Uri uri)
        {
            // URIに応じたWebRequestを取得
            WebRequest req = WebRequest.Create(uri);

            // 設定が必要なRequestの場合は、必要な設定を行う
            // ※ FileWebRequestであれば特になし、それ以外は通すが未確認
            if (req as HttpWebRequest != null)
            {
                // HTTP/HTTPSの場合
                HttpWebRequest h = req as HttpWebRequest;

                // UserAgent設定
                string ua = Settings.Default.UserAgent;
                if (String.IsNullOrEmpty(ua))
                {
                    // 特に設定が無い場合はデフォルトの値を設定
                    Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    ua = String.Format(
                        Settings.Default.DefaultUserAgent,
                        ver.Major,
                        ver.Minor);
                }

                h.UserAgent = ua;

                // Referer設定
                string referer = Settings.Default.Referer;
                if (String.IsNullOrEmpty(referer))
                {
                    // 空の場合は、遷移元のURLを自動設定
                    // TODO: 実装したらサーバーにやさしいかなと思う。
                    referer = String.Empty;
                }

                h.Referer = referer;
            }

            // 応答データを受信するためのStreamを取得し、データを取得
            return req.GetResponse().GetResponseStream();
        }

        #endregion
    }
}
