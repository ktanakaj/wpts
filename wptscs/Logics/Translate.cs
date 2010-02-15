// ================================================================================================
// <summary>
//      翻訳支援処理を実装するための共通クラスソース</summary>
//
// <copyright file="Translate.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Net.NetworkInformation;
    using Honememo;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// 翻訳支援処理を実装するための共通クラスです。
    /// </summary>
    public abstract class Translate
    {
        #region private変数

        /// <summary>
        /// 改行コード。
        /// </summary>
        public static readonly string ENTER = "\r\n";

        /// <summary>
        /// ログメッセージ（property）。
        /// </summary>
        private string log;

        /// <summary>
        /// 変換後テキスト（property）。
        /// </summary>
        private string text;

        /// <summary>
        /// 処理を途中で終了させるためのフラグ。
        /// </summary>
        private bool cancellationPending;

        /// <summary>
        /// 翻訳元言語のサイト／言語情報。
        /// </summary>
        private Website from;

        /// <summary>
        /// 翻訳先言語のサイト／言語情報。
        /// </summary>
        private Website to;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="from">翻訳元サイト／言語。</param>
        /// <param name="to">翻訳先サイト／言語。</param>
        public Translate(Website from, Website to)
        {
            // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            else if (to == null)
            {
                throw new ArgumentNullException("to");
            }

            // メンバ変数の初期化
            this.from = from;
            this.to = to;
            this.Initialize();
        }

        #endregion

        #region イベント

        /// <summary>
        /// ログ更新伝達イベント。
        /// </summary>
        public event EventHandler LogUpdate;

        #endregion

        #region プロパティ

        /// <summary>
        /// ログメッセージ。
        /// </summary>
        public string Log
        {
            get
            {
                return this.log;
            }

            protected set
            {
                this.log = (value != null) ? value : String.Empty;
                this.LogUpdate(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 変換後テキスト。
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            protected set
            {
                this.text = (value != null) ? value : String.Empty;
            }
        }

        /// <summary>
        /// 処理を途中で終了させるためのフラグ。
        /// </summary>
        public bool CancellationPending
        {
            get
            {
                return this.cancellationPending;
            }

            set
            {
                this.cancellationPending = value;
            }
        }

        /// <summary>
        /// 翻訳元言語のサイト。
        /// </summary>
        protected Website From
        {
            get
            {
                return this.from;
            }
        }

        /// <summary>
        /// 翻訳先言語のサイト。
        /// </summary>
        protected Website To
        {
            get
            {
                return this.to;
            }
        }

        #endregion

        #region publicメソッド

        /// <summary>
        /// 翻訳支援処理実行。
        /// </summary>
        /// <param name="name">記事名。</param>
        /// <returns><c>true</c> 処理成功</returns>
        public virtual bool Run(string name)
        {
            // 変数を初期化
            this.Initialize();

            // サーバー接続チェック
            string host = new Uri(this.From.Location).Host;
            if (!String.IsNullOrEmpty(host))
            {
                if (!this.Ping(host))
                {
                    return false;
                }
            }

            // 翻訳支援処理実行部の本体を実行
            // ※以降の処理は、継承クラスにて定義
            return this.RunBody(name);
        }
        
        #endregion

        #region protectedメソッド

        /// <summary>
        /// 翻訳支援処理実行部の本体。
        /// </summary>
        /// <param name="name">記事名。</param>
        /// <returns><c>true</c> 処理成功</returns>
        /// <remarks>テンプレートメソッド的な構造になっています。</remarks>
        protected abstract bool RunBody(string name);

        /// <summary>
        /// ログメッセージを1行追加出力。
        /// </summary>
        /// <param name="log">ログメッセージ。</param>
        protected void LogLine(string log)
        {
            // 直前のログが改行されていない場合、改行して出力
            if (this.Log != String.Empty && this.Log.EndsWith(ENTER) == false)
            {
                this.Log += ENTER + log + ENTER;
            }
            else
            {
                this.Log += log + ENTER;
            }
        }

        /// <summary>
        /// ログメッセージを1行追加出力（入力された文字列を書式化して表示）。
        /// </summary>
        /// <param name="format">書式項目を含んだログメッセージ。</param>
        /// <param name="args">書式設定対象オブジェクト配列。</param>
        protected void LogLine(string format, params object[] args)
        {
            // オーバーロードメソッドをコール
            this.LogLine(String.Format(format, args));
        }

        #endregion

        #region privateメソッド

        /// <summary>
        /// 翻訳支援処理実行時の初期化処理。
        /// </summary>
        private void Initialize()
        {
            // 変数を初期化
            this.log = String.Empty;
            this.Text = String.Empty;
            this.CancellationPending = false;
        }

        /// <summary>
        /// サーバー接続チェック。
        /// </summary>
        /// <param name="server">サーバー名。</param>
        /// <returns><c>true</c> 接続成功。</returns>
        private bool Ping(string server)
        {
            // サーバー接続チェック
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(server);
                if (reply.Status != IPStatus.Success)
                {
                    this.LogLine(Resources.ErrorMessageConnectionFailed, reply.Status.ToString());
                    return false;
                }
            }
            catch (Exception e)
            {
                this.LogLine(Resources.ErrorMessageConnectionFailed, e.InnerException.Message);
                return false;
            }

            return true;
        }

        #endregion
    }
}
