// ================================================================================================
// <summary>
//      アプリケーションデフォルトの値を用いてウェブアクセスするプロキシクラスソース</summary>
//
// <copyright file="AppDefaultWebProxy.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Utilities
{
    using System;
    using System.IO;
    using System.Net;
    using Honememo.Wptscs.Properties;
    
    /// <summary>
    /// アプリケーションデフォルトの値を用いてウェブアクセスするプロキシクラスです。
    /// </summary>
    public class AppDefaultWebProxy : IWebProxy
    {
        #region private変数

        /// <summary>
        /// このプロキシで使用するUserAgent。
        /// </summary>
        private string userAgent;

        /// <summary>
        /// このプロキシで使用するReferer。
        /// </summary>
        private string referer;

        #endregion

        #region インタフェース実装プロパティ

        /// <summary>
        /// このプロキシで使用するUserAgent。
        /// </summary>
        /// <returns>UserAgent。</returns>
        /// <remarks>プロパティ→アプリ設定値→アプリデフォルト値を生成 の順にあるものを返す。</remarks>
        public string UserAgent
        {
            get
            {
                // プロパティを確認
                if (this.userAgent != null)
                {
                    return this.userAgent;
                }

                // アプリ設定値を確認
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

                return ua;
            }

            set
            {
                this.userAgent = value;
            }
        }

        /// <summary>
        /// このプロキシで使用するReferer。
        /// </summary>
        /// <returns>Referer。</returns>
        /// <remarks>プロパティ→アプリ設定値 の順にあるものを返す。</remarks>
        public string Referer
        {
            get
            {
                // プロパティを確認
                if (this.referer != null)
                {
                    return this.referer;
                }

                // アプリ設定値を確認
                string r = Settings.Default.Referer;
                if (String.IsNullOrEmpty(r))
                {
                    r = String.Empty;
                }

                return r;
            }

            set
            {
                this.referer = value;
            }
        }

        #endregion

        #region インタフェース実装メソッド

        /// <summary>
        /// 指定されたURIの情報をストリームで取得。
        /// </summary>
        /// <param name="uri">取得対象のURI。</param>
        /// <returns>取得したストリーム。使用後は必ずクローズすること。</returns>
        /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
        public Stream GetStream(Uri uri)
        {
            // URIに応じたWebRequestを取得
            WebRequest req = WebRequest.Create(uri);

            // Requestに応じた設定を行う
            // ※ HttpWebRequest, FileWebRequestを想定（後者は特に処理無し）
            //    それ以外については通すが未確認
            if (req is HttpWebRequest)
            {
                // HTTP/HTTPSの場合
                this.InitializeHttpWebRequest((HttpWebRequest)req);
            }

            // 応答データを受信するためのStreamを取得し、データを取得
            return req.GetResponse().GetResponseStream();
        }

        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// HttpWebRequest用の設定を行う。
        /// </summary>
        /// <param name="req">設定対象のHttpWebRequest。</param>
        private void InitializeHttpWebRequest(HttpWebRequest req)
        {
            // UserAgent設定
            req.UserAgent = this.UserAgent;

            // Referer設定
            req.Referer = this.Referer;
        }
        
        #endregion
    }
}
