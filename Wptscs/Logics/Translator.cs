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
    using System.Diagnostics;
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
        /// 処理状態メッセージ。
        /// </summary>
        private string status = String.Empty;

        /// <summary>
        /// 変換後テキスト。
        /// </summary>
        private string text = String.Empty;

        /// <summary>
        /// ログテキスト生成用ロガー。
        /// </summary>
        private Logger logger;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// インスタンスを生成する。
        /// </summary>
        public Translator()
        {
            this.Stopwatch = new Stopwatch();
            this.Logger = new Logger();
        }

        #endregion

        #region デリゲート

        /// <summary>
        /// <see cref="ChangeStatusInExecuting"/> で実行する処理のためのデリゲート。
        /// </summary>
        protected delegate void MethodWithChangeStatus();

        #endregion

        #region イベント

        /// <summary>
        /// ログ更新伝達イベント。
        /// </summary>
        public event EventHandler LogUpdate;

        /// <summary>
        /// 処理状態更新伝達イベント。
        /// </summary>
        public event EventHandler StatusUpdate;

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
            get
            {
                return this.Logger.ToString();
            }
        }

        /// <summary>
        /// 処理状態メッセージ。
        /// </summary>
        public string Status
        {
            get
            {
                return this.status;
            }

            protected set
            {
                this.status = StringUtils.DefaultString(value);
                if (this.StatusUpdate != null)
                {
                    this.StatusUpdate(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 処理時間ストップウォッチ。
        /// </summary>
        public Stopwatch Stopwatch
        {
            get;
            private set;
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

        /// <summary>
        /// ログテキスト生成用ロガー。
        /// </summary>
        protected Logger Logger
        {
            get
            {
                return this.logger;
            }

            set
            {
                // nullは不可。また、ロガー変更後はイベントを設定
                this.logger = Validate.NotNull(value);
                this.logger.LogUpdate += this.GetLogUpdate;
            }
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
        /// <exception cref="ApplicationException">処理が中断された場合。中断の理由は<see cref="Logger"/>に出力される。</exception>
        /// <exception cref="InvalidOperationException"><see cref="From"/>, <see cref="To"/>が設定されていない場合。</exception>
        public void Run(string name)
        {
            // ※必須な情報が設定されていない場合、InvalidOperationExceptionを返す
            if (this.From == null || this.To == null)
            {
                throw new InvalidOperationException("From or To is null");
            }

            // 変数を初期化、処理時間を測定開始
            this.Initialize();
            this.Stopwatch.Start();

            // サーバー接続チェック
            string host = new Uri(this.From.Location).Host;
            if (!String.IsNullOrEmpty(host) && !Settings.Default.IgnoreError)
            {
                if (!this.Ping(host))
                {
                    throw new ApplicationException("ping failed");
                }
            }

            // ここまでの間に終了要求が出ているかを確認
            this.ThrowExceptionIfCanceled();

            // 翻訳支援処理実行部の本体を実行
            // ※以降の処理は、継承クラスにて定義
            try
            {
                this.RunBody(name);
            }
            finally
            {
                // 終了後は処理状態をクリア、処理時間を測定終了
                this.Status = String.Empty;
                this.Stopwatch.Stop();
            }
        }
        
        #endregion

        #region protectedメソッド

        /// <summary>
        /// 翻訳支援処理実行部の本体。
        /// </summary>
        /// <param name="name">記事名。</param>
        /// <exception cref="ApplicationException">処理を中断する場合。中断の理由は<see cref="Logger"/>に出力する。</exception>
        /// <remarks>テンプレートメソッド的な構造になっています。</remarks>
        protected abstract void RunBody(string name);

        /// <summary>
        /// ログ出力によるエラー処理を含んだページ取得処理。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <param name="page">取得したページ。ページが存在しない場合は <c>null</c> を返す。</param>
        /// <returns>処理が成功した（404も含む）場合<c>true</c>、失敗した（通信エラーなど）の場合<c>false</c>。</returns>
        /// <exception cref="ApplicationException"><see cref="CancellationPending"/>が<c>true</c>の場合。</exception>
        /// <remarks>
        /// 本メソッドは、大きく3パターンの動作を行う。
        /// <list type="number">
        /// <item><description>正常にページが取得できた → <c>true</c>でページを設定、ログ出力無し</description></item>
        /// <item><description>404など想定内の例外でページが取得できなかった → <c>true</c>でページ無し、ログ出力無し</description></item>
        /// <item><description>想定外の例外でページが取得できなかった → <c>false</c>でページ無し、ログ出力有り
        ///                    or <c>ApplicationException</c>で処理中断（アプリケーション設定のIgnoreErrorによる）。</description></item>
        /// </list>
        /// また、実行中は処理状態をサーバー接続中に更新する。
        /// 実行前後には終了要求のチェックも行う。
        /// </remarks>
        protected bool TryGetPage(string title, out Page page)
        {
            // 通信開始の前に終了要求が出ているかを確認
            this.ThrowExceptionIfCanceled();

            // ページ取得処理、実行中は処理状態を変更
            bool success = false;
            Page result = null;
            this.ChangeStatusInExecuting(
                () => success = this.TryGetPageBody(title, out result),
                Resources.StatusDownloading);
            page = result;

            // 通信終了後にも再度終了要求を確認
            this.ThrowExceptionIfCanceled();
            return success;
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

        /// <summary>
        /// 指定された処理を実行する間、処理状態を渡された値に更新する。
        /// 処理終了後は以前の処理状態に戻す。
        /// </summary>
        /// <param name="method">実行する処理。</param>
        /// <param name="status">処理状態。</param>
        protected void ChangeStatusInExecuting(MethodWithChangeStatus method, string status)
        {
            // 現在の処理状態を保存、新しい処理状態をセットし、処理を実行する
            string oldStatus = this.Status;
            this.Status = status;
            try
            {
                method();
            }
            finally
            {
                // 処理状態を以前の状態に戻す
                this.Status = oldStatus;
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
            this.Logger.Clear();
            this.Status = String.Empty;
            this.Stopwatch.Reset();
            this.Text = String.Empty;
            this.CancellationPending = false;
        }

        /// <summary>
        /// サーバー接続チェック。
        /// </summary>
        /// <param name="server">サーバー名。</param>
        /// <returns><c>true</c> 接続成功。</returns>
        /// <remarks>実行中は処理状態をサーバー接続中に更新する。</remarks>
        private bool Ping(string server)
        {
            // サーバー接続チェック、実行中は処理状態を変更
            bool result = false;
            this.ChangeStatusInExecuting(
                () => result = this.PingBody(server),
                Resources.StatusPinging);
            return result;
        }

        /// <summary>
        /// サーバー接続チェック本体。
        /// </summary>
        /// <param name="server">サーバー名。</param>
        /// <returns><c>true</c> 接続成功。</returns>
        private bool PingBody(string server)
        {
            // サーバー接続チェック
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(server);
                if (reply.Status != IPStatus.Success)
                {
                    this.Logger.AddMessage(Resources.ErrorMessageConnectionFailed, reply.Status.ToString());
                    return false;
                }
            }
            catch (Exception e)
            {
                this.Logger.AddMessage(Resources.ErrorMessageConnectionFailed, e.InnerException.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// ログ出力によるエラー処理を含んだページ取得処理本体。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <param name="page">取得したページ。ページが存在しない場合は <c>null</c> を返す。</param>
        /// <returns>処理が成功した（404も含む）場合<c>true</c>、失敗した（通信エラーなど）の場合<c>false</c>。</returns>
        /// <remarks>
        /// 本メソッドは、大きく3パターンの動作を行う。
        /// <list type="number">
        /// <item><description>正常にページが取得できた → <c>true</c>でページを設定、ログ出力無し</description></item>
        /// <item><description>404など想定内の例外でページが取得できなかった → <c>true</c>でページ無し、ログ出力無し</description></item>
        /// <item><description>想定外の例外でページが取得できなかった → <c>false</c>でページ無し、ログ出力有り
        ///                    or <c>ApplicationException</c>で処理中断（アプリケーション設定のIgnoreError等による）。</description></item>
        /// </list>
        /// </remarks>
        private bool TryGetPageBody(string title, out Page page)
        {
            page = null;
            try
            {
                // 普通に取得できた場合はここで終了
                page = this.From.GetPage(title);
                return true;
            }
            catch (FileNotFoundException)
            {
                // ページ無しによる例外も正常終了
                return true;
            }
            catch (NotSupportedException)
            {
                // 末尾がピリオドで終わるページが処理できない既知の不具合への対応、警告メッセージを出す
                this.Logger.AddResponse(Resources.LogMessageErrorPageName, title);
                return false;
            }
            catch (Exception e)
            {
                // その他例外の場合、まずエラー情報を出力
                this.Logger.AddResponse(e.Message);
                if (e is WebException && ((WebException)e).Response != null)
                {
                    // 出せるならエラーとなったURLも出力
                    this.Logger.AddResponse(Resources.LogMessageErrorURL, ((WebException)e).Response.ResponseUri);
                }

                // エラーを無視しない場合、ここで翻訳支援処理を中断する
                if (!Settings.Default.IgnoreError)
                {
                    throw new ApplicationException(e.Message, e);
                }

                return false;
            }
        }

        /// <summary>
        /// ロガーのログ状態更新イベント用。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void GetLogUpdate(object sender, EventArgs e)
        {
            // もともとこのクラスにあったログ通知イベントをロガーに移動したため、入れ子で呼び出す
            if (this.LogUpdate != null)
            {
                this.LogUpdate(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
