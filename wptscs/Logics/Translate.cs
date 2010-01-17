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
    using System.Net;
    using Honememo;
    using Honememo.Wptscs.Models;

    /// <summary>
    /// 翻訳支援処理を実装するための共通クラスです。
    /// </summary>
    public abstract class Translate
    {
        /// <summary>
        /// 改行コード。
        /// </summary>
        public static readonly string ENTER = "\r\n";

        /// <summary>
        /// 処理を途中で終了させるためのフラグ。
        /// </summary>
        public bool CancellationPending;

        /// <summary>
        /// 共通関数クラスのオブジェクト。
        /// </summary>
        protected Honememo.Cmn cmnAP;

        /// <summary>
        /// 翻訳元言語の言語コード。
        /// </summary>
        protected LanguageInformation source;

        /// <summary>
        /// 翻訳先言語の言語コード。
        /// </summary>
        protected LanguageInformation target;

        /// <summary>
        /// ログメッセージ（property）。
        /// </summary>
        private string log;

        /// <summary>
        /// 変換後テキスト（property）。
        /// </summary>
        private string text;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="source">翻訳元言語</param>
        /// <param name="target">翻訳先言語</param>
        public Translate(LanguageInformation source, LanguageInformation target)
        {
            // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            else if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            // メンバ変数の初期化
            this.cmnAP = new Honememo.Cmn();
            this.source = source;
            this.target = target;
            this.RunInitialize();
        }

        /// <summary>
        /// ログ更新伝達イベント。
        /// </summary>
        public event EventHandler LogUpdate;

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
        /// 翻訳支援処理実行。
        /// </summary>
        /// <param name="name">記事名</param>
        /// <returns><c>true</c> 処理成功</returns>
        public virtual bool Run(string name)
        {
            // 変数を初期化
            this.RunInitialize();

            // 翻訳支援処理実行部の本体を実行
            // ※以降の処理は、継承クラスにて定義
            return this.RunBody(name);
        }

        /// <summary>
        /// 翻訳支援処理実行部の本体。
        /// ※継承クラスでは、この関数に処理を実装すること
        /// </summary>
        /// <param name="name">記事名</param>
        /// <returns><c>true</c> 処理成功</returns>
        protected abstract bool RunBody(string name);

        /// <summary>
        /// 翻訳支援処理実行時の初期化処理。
        /// </summary>
        protected void RunInitialize()
        {
            // 変数を初期化
            this.log = String.Empty;
            this.Text = String.Empty;
            this.CancellationPending = false;
        }

        /// <summary>
        /// ログメッセージを1行追加出力。
        /// </summary>
        /// <param name="log">ログメッセージ</param>
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
    }
}
