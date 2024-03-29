﻿// ================================================================================================
// <summary>
//      ログテキストの生成を行うためのクラスソース</summary>
//
// <copyright file="Logger.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Net;
    using System.Text;
    using Honememo.Parsers;
    using Honememo.Utilities;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// ログテキストの生成を行うためのクラスです。
    /// </summary>
    public class Logger
    {
        #region private変数

        /// <summary>
        /// ログテキスト。
        /// </summary>
        private string log = string.Empty;

        #endregion

        #region イベント

        /// <summary>
        /// ログ更新伝達イベント。
        /// </summary>
        public event EventHandler LogUpdate;

        #endregion

        #region プロパティ

        /// <summary>
        /// ログテキスト。
        /// </summary>
        protected virtual string Log
        {
            get
            {
                return this.log;
            }

            set
            {
                this.log = StringUtils.DefaultString(value);
                if (this.LogUpdate != null)
                {
                    this.LogUpdate(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region ログ登録メソッド（一般）
        
        /// <summary>
        /// ログメッセージを登録する。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public virtual void AddMessage(string message)
        {
            // 直前のログが改行されていない場合、改行して出力
            this.AddNewLineIfNotEndWithNewLine();
            this.Log += message + Environment.NewLine;
        }

        /// <summary>
        /// ログメッセージを書式化して登録する。
        /// </summary>
        /// <param name="format">書式項目を含んだログメッセージ。</param>
        /// <param name="args">書式設定対象オブジェクト配列。</param>
        public virtual void AddMessage(string format, params object[] args)
        {
            // 書式化してオーバーロードメソッドをコール
            this.AddMessage(string.Format(format, args));
        }

        /// <summary>
        /// 応答メッセージ（「→ ～しました」のようなメッセージ）を登録する。
        /// </summary>
        /// <param name="response">ログメッセージ。</param>
        public virtual void AddResponse(string response)
        {
            // 右矢印の後に半角スペースを空けてメッセージ出力
            this.AddMessage(Resources.RightArrow + " " + response);
        }

        /// <summary>
        /// 応答メッセージ（「→ ～しました」のようなメッセージ）を登録する。
        /// </summary>
        /// <param name="format">書式項目を含んだログメッセージ。</param>
        /// <param name="args">書式設定対象オブジェクト配列。</param>
        public virtual void AddResponse(string format, params object[] args)
        {
            // 書式化してオーバーロードメソッドをコール
            this.AddResponse(string.Format(format, args));
        }

        /// <summary>
        /// 例外メッセージ（「→ 通信エラー」のようなメッセージ）を登録する。
        /// </summary>
        /// <param name="e">例外。</param>
        public virtual void AddError(Exception e)
        {
            // 応答形式で例外メッセージを出力
            this.AddResponse(e.Message);
            if (e is WebException && ((WebException)e).Response != null)
            {
                // 出せるならエラーとなったURLも出力
                this.AddResponse(Resources.LogMessageErrorURL, ((WebException)e).Response.ResponseUri);
            }
        }

        /// <summary>
        /// ログ上の区切りを登録する。
        /// </summary>
        public virtual void AddSeparator()
        {
            // この実装では、区切りは余分な空行で表す
            this.AddMessage(string.Empty);
        }

        #endregion

        #region ログ登録メソッド（翻訳支援処理）

        /// <summary>
        /// 変換元を表すログを追加する。
        /// </summary>
        /// <param name="source">変換元を表す要素。</param>
        /// <remarks>
        /// 変換結果をログ出力する際は
        /// <code>AddSource</code>→(<see cref="AddAlias"/>)→<see cref="AddDestination"/>
        /// の順で一連のメソッドをコールする。
        /// 上記以外のメソッドをコールした場合、該当行の変換結果の出力は終了したものとみなす。
        /// </remarks>
        public virtual void AddSource(IElement source)
        {
            // 直前のログが改行されていない場合、改行して出力
            this.AddNewLineIfNotEndWithNewLine();
            this.Log += source.ToString() + " " + Resources.RightArrow + " ";
        }

        /// <summary>
        /// 変換元の別名を表すログを追加する。
        /// </summary>
        /// <param name="alias">変換元の別名を表す要素。</param>
        /// <remarks>
        /// 変換結果をログ出力する際は
        /// <see cref="AddSource"/>→(<code>AddAlias</code>)→<see cref="AddDestination"/>
        /// の順で一連のメソッドをコールする。
        /// 上記以外のメソッドをコールした場合、該当行の変換結果の出力は終了したものとみなす。
        /// </remarks>
        public virtual void AddAlias(IElement alias)
        {
            this.Log += alias.ToString() + " " + Resources.RightArrow + " ";
        }

        /// <summary>
        /// 変換先を表すログを追加する。
        /// </summary>
        /// <param name="destination">変換先を表す要素。</param>
        /// <param name="cacheUsed">対訳表を使用している場合<code>true</code>。デフォルトは<code>false</code>。</param>
        /// <remarks>
        /// 変換結果をログ出力する際は
        /// <see cref="AddSource"/>→(<see cref="AddAlias"/>)→<code>AddDestination</code>
        /// の順で一連のメソッドをコールする。
        /// 上記以外のメソッドをコールした場合、該当行の変換結果の出力は終了したものとみなす。
        /// </remarks>
        public virtual void AddDestination(IElement destination, bool cacheUsed = false)
        {
            StringBuilder b = new StringBuilder(destination.ToString());
            if (cacheUsed)
            {
                b.Append(Resources.LogMessageNoteTranslation);
            }

            b.Append(Environment.NewLine);
            this.Log += b.ToString();
        }

        #endregion

        #region ログ出力メソッド

        /// <summary>
        /// このロガーが保持するログテキストを返す。
        /// </summary>
        /// <returns>ログテキスト。<c>null</c>は返さない。</returns>
        public override string ToString()
        {
            return this.Log;
        }

        #endregion

        #region 初期化メソッド

        /// <summary>
        /// このロガーが保持する内容を初期状態に戻す。
        /// </summary>
        public virtual void Clear()
        {
            this.Log = null;
        }

        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// 直前のログが改行されていない場合、改行する。
        /// </summary>
        protected void AddNewLineIfNotEndWithNewLine()
        {
            // ログが空以外で最後が改行ではない場合
            if (!string.IsNullOrEmpty(this.Log) && !this.Log.EndsWith(Environment.NewLine))
            {
                this.Log += Environment.NewLine;
            }
        }

        #endregion
    }
}
