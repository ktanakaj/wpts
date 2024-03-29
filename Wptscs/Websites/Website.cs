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
    using Honememo.Wptscs.Utilities;

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

        /// <summary>
        /// このウェブサイトへのアクセスで使用するProxyインスタンス。
        /// </summary>
        private Honememo.Wptscs.Utilities.IWebProxy webProxy = new AppConfigWebProxy();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定された言語, サーバーのウェブサイトを表すインスタンスを作成。
        /// </summary>
        /// <param name="language">ウェブサイトの言語。</param>
        /// <param name="location">ウェブサイトの場所。</param>
        /// <exception cref="ArgumentNullException"><paramref name="language"/>または<paramref name="location"/>が<c>null</c>の場合。</exception>
        /// <exception cref="ArgumentException"><paramref name="location"/>が空の文字列の場合。</exception>
        public Website(Language language, string location)
        {
            this.Language = language;
            this.Location = location;
        }

        /// <summary>
        /// 空のインスタンスを作成（シリアライズ or 拡張用）。
        /// </summary>
        /// <remarks>
        /// <see cref="Language"/>, <see cref="Location"/>
        /// はウェブサイトでは必須扱いのため、通常はチェックが行われる
        /// <see cref="Website(Language, string)"/>
        /// のコンストラクタを使用してください。
        /// </remarks>
        protected Website()
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// ウェブサイトの場所。
        /// </summary>
        /// <example>http://en.wikipedia.org</example>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        /// <exception cref="ArgumentException">空の文字列が指定された場合。</exception>
        public string Location
        {
            get
            {
                return this.location;
            }

            set
            {
                this.location = Validate.NotBlank(value);
            }
        }

        /// <summary>
        /// ウェブサイトの言語。
        /// </summary>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        public Language Language
        {
            get
            {
                return this.language;
            }

            protected set
            {
                this.language = Validate.NotNull(value);
            }
        }

        /// <summary>
        /// このウェブサイトへのアクセスで使用するProxyインスタンス。
        /// </summary>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        public Honememo.Wptscs.Utilities.IWebProxy WebProxy
        {
            get
            {
                return this.webProxy;
            }

            set
            {
                this.webProxy = Validate.NotNull(value);
            }
        }

        #endregion
        
        #region 抽象メソッド

        /// <summary>
        /// ページを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <returns>取得したページ。</returns>
        /// <exception cref="FileNotFoundException">ページが存在しない場合。</exception>
        /// <remarks>ページの取得に失敗した場合（通信エラーなど）は、その状況に応じた例外を投げる。</remarks>
        public abstract Page GetPage(string title);

        #endregion

        #region 実装支援用メソッド

        /// <summary>
        /// 指定された<see cref="WebException"/>は対象データ無しで発生したものか？
        /// </summary>
        /// <param name="e">判定する例外。</param>
        /// <returns>対象データ無しで発生したものの場合<c>true</c>。</returns>
        /// <remarks>HTTPスキームの404と、fileスキームのファイル無しをデータ無しと判定。</remarks>
        protected bool IsNotFound(WebException e)
        {
            if (e.Status == WebExceptionStatus.ProtocolError
                && ((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotFound)
            {
                // HTTPのエラーでステータスコードが404
                return true;
            }

            if (e.Status == WebExceptionStatus.ConnectFailure
                || e.Status == WebExceptionStatus.UnknownError)
            {
                // fileスキームでは、FileNotFoundExceptionが入れ子（2段とかもある）
                // のUnknownError, ConnectFailureとかで返ってくるので再帰的にチェック
                if (e.InnerException is FileNotFoundException)
                {
                    return true;
                }
                else if (e.InnerException is WebException)
                {
                    return this.IsNotFound((WebException)e.InnerException);
                }
            }

            return false;
        }

        #endregion
    }
}
