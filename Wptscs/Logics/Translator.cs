// ================================================================================================
// <summary>
//      翻訳支援処理を実装するための共通クラスソース</summary>
//
// <copyright file="Translator.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using Honememo.Utilities;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// 翻訳支援処理を実装するための共通クラスです。
    /// </summary>
    public abstract class Translator
    {
        #region private変数

        /// <summary>
        /// 改行コード。
        /// </summary>
        public static readonly string ENTER = "\r\n";

        /// <summary>
        /// ログメッセージ。
        /// </summary>
        private string log = String.Empty;

        /// <summary>
        /// 変換後テキスト。
        /// </summary>
        private string text = String.Empty;

        #endregion
        
        #region イベント

        /// <summary>
        /// ログ更新伝達イベント。
        /// </summary>
        public event EventHandler LogUpdate;

        #endregion

        #region プロパティ

        /// <summary>
        /// 言語間の項目の対訳表。
        /// </summary>
        public TranslationDictionary ItemTable
        {
            get;
            set;
        }

        /// <summary>
        /// 言語間の見出しの対訳表。
        /// </summary>
        public TranslationTable HeadingTable
        {
            get;
            set;
        }

        /// <summary>
        /// ログメッセージ。
        /// </summary>
        public string Log
        {
            // ※ 将来的には、ロジックでログメッセージを出すなんて形を止めて
            //    データとして保持させてメッセージはビューで・・・としたいが、
            //    手間を考えて当面はこの形のまま実装する。
            get
            {
                return this.log;
            }

            protected set
            {
                this.log = StringUtils.DefaultString(value);
                if (this.LogUpdate != null)
                {
                    this.LogUpdate(this, EventArgs.Empty);
                }
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
                this.text = StringUtils.DefaultString(value);
            }
        }

        /// <summary>
        /// 処理を途中で終了させるためのフラグ。
        /// </summary>
        public bool CancellationPending
        {
            get;
            set;
        }

        /// <summary>
        /// 翻訳元言語のサイト。
        /// </summary>
        public Website From
        {
            get;
            set;
        }

        /// <summary>
        /// 翻訳先言語のサイト。
        /// </summary>
        public Website To
        {
            get;
            set;
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 翻訳支援処理のインスタンスを作成。
        /// </summary>
        /// <param name="config">アプリケーション設定。</param>
        /// <param name="from">翻訳元言語。</param>
        /// <param name="to">翻訳先言語。</param>
        /// <returns>生成したインスタンス。</returns>
        /// <remarks>
        /// 設定は設定クラスより取得、無ければ一部自動生成する。
        /// インスタンス生成失敗時は例外を投げる。
        /// </remarks>
        public static Translator Create(Config config, string from, string to)
        {
            // 処理対象に応じてTranslatorを継承したオブジェクトを生成
            ConstructorInfo constructor = config.Translator.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                throw new NotImplementedException(config.Translator.FullName + " default constructor is not found");
            }

            // 設定に指定されたクラスを、引数無しのコンストラクタを用いて生成する
            Translator translator = (Translator)constructor.Invoke(null);

            // Webサイトの設定
            translator.From = config.GetWebsite(from);
            translator.To = config.GetWebsite(to);

            // 対訳表（項目）の設定
            translator.ItemTable = config.GetItemTableNeedCreate(from, to);

            // 対訳表（見出し）の設定、使用する言語は決まっているので組み合わせを設定
            translator.HeadingTable = config.HeadingTable;
            translator.HeadingTable.From = from;
            translator.HeadingTable.To = to;

            return translator;
        }

        #endregion

        #region publicメソッド

        /// <summary>
        /// 翻訳支援処理実行。
        /// </summary>
        /// <param name="name">記事名。</param>
        /// <exception cref="ApplicationException">処理が中断された場合。中断の理由は<see cref="Log"/>に出力される。</exception>
        /// <exception cref="InvalidOperationException"><see cref="From"/>, <see cref="To"/>が設定されていない場合。</exception>
        public virtual void Run(string name)
        {
            // ※必須な情報が設定されていない場合、InvalidOperationExceptionを返す
            if (this.From == null || this.To == null)
            {
                throw new InvalidOperationException("From or To is null");
            }

            // 変数を初期化
            this.Initialize();

            // サーバー接続チェック
            string host = new Uri(this.From.Location).Host;
            if (!String.IsNullOrEmpty(host) && !Settings.Default.IgnoreError)
            {
                if (!this.Ping(host))
                {
                    throw new ApplicationException("ping failed");
                }
            }

            // ここまでの間に終了条件が出ているかを確認
            this.ThrowExceptionIfCanceled();

            // 翻訳支援処理実行部の本体を実行
            // ※以降の処理は、継承クラスにて定義
            this.RunBody(name);
        }
        
        #endregion

        #region protectedメソッド

        /// <summary>
        /// 翻訳支援処理実行部の本体。
        /// </summary>
        /// <param name="name">記事名。</param>
        /// <exception cref="ApplicationException">処理を中断する場合。中断の理由は<see cref="Log"/>に出力する。</exception>
        /// <remarks>テンプレートメソッド的な構造になっています。</remarks>
        protected abstract void RunBody(string name);

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

        /// <summary>
        /// ログメッセージを出力しつつページを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <param name="notFoundMsg">取得できない場合に出力するメッセージ。</param>
        /// <returns>取得したページ。ページが存在しない場合は <c>null</c> を返す。</returns>
        /// <remarks>通信エラーなど例外が発生した場合は、別途エラーログを出力する。</remarks>
        protected Page GetPage(string title, string notFoundMsg)
        {
            try
            {
                // 取得できた場合はここで終了
                return this.From.GetPage(title);
            }
            catch (WebException e)
            {
                // 通信エラー
                if (e.Status == WebExceptionStatus.ProtocolError
                    && (e.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound)
                {
                    // 404
                    this.Log += notFoundMsg;
                }
                else
                {
                    // それ以外のエラー
                    this.LogLine(Resources.RightArrow + " " + e.Message);
                    if (e.Response != null)
                    {
                        this.LogLine(Resources.RightArrow + " " + String.Format(Resources.LogMessageErrorURL, e.Response.ResponseUri));
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // ファイル無し
                this.Log += notFoundMsg;
            }
            catch (Exception e)
            {
                // その他の想定外のエラー
                this.LogLine(Resources.RightArrow + " " + e.Message);
            }

            // 取得失敗時いずれの場合もnull
            return null;
        }

        /// <summary>
        /// 終了要求が出ている場合、例外を投げる。
        /// </summary>
        /// <exception cref="ApplicationException"><see cref="CancellationPending"/>が<c>true</c>の場合。</exception>
        protected void ThrowExceptionIfCanceled()
        {
            if (this.CancellationPending)
            {
                throw new ApplicationException("CancellationPending is true");
            }
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
