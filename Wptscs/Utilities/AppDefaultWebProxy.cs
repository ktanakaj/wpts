﻿// ================================================================================================
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
    using System.Threading;
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
        /// <remarks>
        /// 通信エラー等の場合、アプリケーション設定に指定されている回数リトライする。
        /// それでも取得できない場合は例外を投げる。
        /// </remarks>
        public Stream GetStream(Uri uri)
        {
            // 実際の処理はサブメソッドで行い、このメソッドでは通信エラー時のリトライを行う
            int retry = Settings.Default.MaxConnectRetries;
            int wait = Settings.Default.ConnectRetryTime;
            while (true)
            {
                try
                {
                    return this.GetStreamBody(uri);
                }
                catch (WebException e)
                {
                    // 通信エラーの場合、指定回数までリトライを試みる
                    if (--retry < 0 || !this.IsRetryable(e))
                    {
                        // リトライ回数を消化、またはリトライしても意味が無い場合、そのまま例外を投げる
                        throw e;
                    }

                    // 時間を置いてからリトライする（0はスレッド停止のため除外）
                    System.Diagnostics.Debug.WriteLine("AppDefaultWebProxy.GetStream > retry : " + e.Message);
                    if (wait > 0)
                    {
                        Thread.Sleep(wait);
                    }
                }
            }
        }

        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// 指定されたURIの情報をストリームで取得。
        /// </summary>
        /// <param name="uri">取得対象のURI。</param>
        /// <returns>取得したストリーム。使用後は必ずクローズすること。</returns>
        /// <remarks>
        /// 通信エラー等の場合、アプリケーション設定に指定されている回数リトライする。
        /// それでも取得できない場合は例外を投げる。
        /// </remarks>
        private Stream GetStreamBody(Uri uri)
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

            // 可能であれば自動的に圧縮を行う用設定
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }

        /// <summary>
        /// 渡された<see cref="WebException"/>がリトライ可能なものか？
        /// </summary>
        /// <param name="e">発生した<c>WebException</c>。</param>
        /// <returns>リトライ可能な場合<c>true</c>。</returns>
        private bool IsRetryable(WebException e)
        {
            // HTTPプロトコルエラーの場合、ステータスコードで判断する
            if (e.Status == WebExceptionStatus.ProtocolError)
            {
                HttpStatusCode sc = ((HttpWebResponse)e.Response).StatusCode;
                return sc == HttpStatusCode.InternalServerError
                    || sc == HttpStatusCode.BadGateway
                    || sc == HttpStatusCode.ServiceUnavailable
                    || sc == HttpStatusCode.GatewayTimeout;
            }

            // それ以外は、応答ステータスで判断する
            // ※ fileスキームのエラー等はUnknownErrorで来るので注意
            return e.Status != WebExceptionStatus.TrustFailure
                && e.Status != WebExceptionStatus.UnknownError
                && e.Status != WebExceptionStatus.RequestProhibitedByCachePolicy
                && e.Status != WebExceptionStatus.RequestProhibitedByProxy;
        }

        #endregion
    }
}
