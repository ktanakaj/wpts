// ================================================================================================
// <summary>
//      アプリケーション設定値を用いてウェブアクセスするプロキシクラスソース</summary>
//
// <copyright file="AppConfigWebProxy.cs" company="honeplusのメモ帳">
//      Copyright (C) 2026 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using Honememo.Utilities;
using Honememo.Wptscs.Properties;

namespace Honememo.Wptscs.Utilities;

/// <summary>
/// アプリケーション設定値を用いてウェブアクセスするプロキシクラスです。
/// </summary>
public class AppConfigWebProxy : IWebProxy
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
    /// <remarks>アプリ設定値→プロパティ→アプリデフォルト値を生成 の順に見つけたものを返す。</remarks>
    public string UserAgent
    {
        get
        {
            // アプリ設定値を確認
            string ua = Settings.Default.UserAgent;
            if (!string.IsNullOrEmpty(ua))
            {
                return ua;
            }

            // 存在しない場合、このプロパティの値を確認
            if (this.userAgent != null)
            {
                return this.userAgent;
            }

            // いずれも存在しない場合は、デフォルトの値を生成して返す
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format(Settings.Default.DefaultUserAgent, ver.Major, ver.Minor);
        }

        set
        {
            this.userAgent = value;
        }
    }

    /// <summary>
    /// このプロキシで使用するReferer。
    /// </summary>
    /// <remarks>アプリ設定値→プロパティ の順に見つけたものを返す。</remarks>
    public string Referer
    {
        get
        {
            // アプリ設定値を確認
            string r = Settings.Default.Referer;
            if (string.IsNullOrEmpty(r))
            {
                // 存在しない場合、このプロパティの値を返す
                r = this.referer;
            }

            return StringUtils.DefaultString(r);
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
        // 実際の処理はサブメソッドで行い、このメソッドではウェイトや通信エラー時のリトライを行う
        var requestInterval = Settings.Default.RequestInterval;
        if (requestInterval > 0)
        {
            Thread.Sleep(requestInterval);
        }

        var max = Settings.Default.MaxConnectRetries;
        var retry = max;
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

                // Retry-Afterヘッダーがある場合はそれを使用
                if (!this.TryParseRetryAfter(e.Response, out var waitTime))
                {
                    // 無い場合は、指数バックオフでリトライ
                    waitTime = TimeSpan.FromSeconds(Math.Pow(2, max - retry));
                }

                // ただし、待機時間が長すぎる場合は、即座にエラーにする
                if (waitTime > TimeSpan.FromMinutes(3))
                {
                    throw e;
                }

                System.Diagnostics.Debug.WriteLine($"AppConfigWebProxy.GetStream > retry {waitTime.TotalSeconds}s : {e.Message}");
                Thread.Sleep(waitTime);
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
    /// <see cref="HttpWebRequest"/>用の設定を行う。
    /// </summary>
    /// <param name="req">設定対象の<see cref="HttpWebRequest"/>。</param>
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
    /// <param name="e">発生した<see cref="WebException"/>。</param>
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
                || sc == HttpStatusCode.GatewayTimeout
                || sc == (HttpStatusCode)429;
        }

        // それ以外は、応答ステータスで判断する
        // ※ fileスキームのエラー等はUnknownErrorで来るので注意
        return e.Status != WebExceptionStatus.TrustFailure
            && e.Status != WebExceptionStatus.UnknownError
            && e.Status != WebExceptionStatus.RequestProhibitedByCachePolicy
            && e.Status != WebExceptionStatus.RequestProhibitedByProxy;
    }

    /// <summary>
    /// HTTPのRetry-Afterヘッダーをパースする。
    /// </summary>
    /// <param name="response">Webレスポンス。</param>
    /// <param name="retryAfter">パースしたRetry-Afterヘッダーの現在日時からの期間。</param>
    /// <returns>パースに成功した場合true。</returns>
    private bool TryParseRetryAfter(WebResponse response, out TimeSpan retryAfter)
    {
        retryAfter = TimeSpan.Zero;
        var retryAfterStr = response.Headers["Retry-After"];
        if (string.IsNullOrEmpty(retryAfterStr))
        {
            return false;
        }

        if (int.TryParse(retryAfterStr, out int seconds))
        {
            retryAfter = TimeSpan.FromSeconds(seconds);
            return true;
        }

        if (DateTime.TryParse(retryAfterStr, out DateTime retryAfterDate))
        {
            retryAfter = retryAfterDate - DateTime.UtcNow;
            return true;
        }

        return false;
    }

    #endregion
}
