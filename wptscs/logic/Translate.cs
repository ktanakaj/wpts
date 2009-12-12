using System;
using System.Net;
using MYAPP;
using wptscs.model;

namespace wptscs.logic
{
    // 翻訳支援処理を実装するための共通クラス
    public abstract class Translate
    {
		// 処理を途中で終了させるためのフラグ
		public bool CancellationPending;

		// ログ更新伝達イベント
		public event EventHandler LogUpdate;

		// ログメッセージ
		public String Log {
			get {
                return _Log;
            }
			protected set {
                _Log = ((value != null) ? value : "");
                LogUpdate(this, EventArgs.Empty);
			}
		}

		// 変換後テキスト
		public String Text {
			get {
                return _Text;
            }
			protected set {
                _Text = ((value != null) ? value : "");
            }
		}

		// 改行コード
        public static readonly String ENTER = "\r\n";

		// 共通関数クラスのオブジェクト
		protected MYAPP.Cmn cmnAP;

		// 翻訳元／先言語の言語コード
		protected LanguageInformation source;
		protected LanguageInformation target;

		// ログメッセージ（property）
		private String _Log;
		// 変換後テキスト（property）
		private String _Text;

        /* コンストラクタ */
        public Translate(LanguageInformation i_Source, LanguageInformation i_Target)
        {
	        // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
	        if(i_Source == null){
		        throw new ArgumentNullException("i_Source");
	        }
	        else if(i_Target == null){
		        throw new ArgumentNullException("i_Target");
	        }
	        // メンバ変数の初期化
	        cmnAP = new MYAPP.Cmn();
	        source = i_Source;
	        target = i_Target;
	        runInitialize();
        }

		// 翻訳支援処理実行部の本体
		// ※継承クラスでは、この関数に処理を実装すること
        protected abstract bool runBody(String i_Name);

        /* 翻訳支援処理実行 */
        public virtual bool Run(String i_Name)
        {
	        // 変数を初期化
	        runInitialize();
	        // 翻訳支援処理実行部の本体を実行
	        // ※以降の処理は、継承クラスにて定義
	        return runBody(i_Name);
        }

        /* 翻訳支援処理実行時の初期化処理 */
        protected void runInitialize()
        {
	        // 変数を初期化
	        _Log = "";
	        Text = "";
	        CancellationPending = false;
        }

        /* ログメッセージを1行追加出力 */
        protected void logLine(String i_Log)
        {
	        // 直前のログが改行されていない場合、改行して出力
	        if(Log != "" && Log.EndsWith(ENTER) == false){
		        Log += (ENTER + i_Log + ENTER);
	        }
	        else{
		        Log += (i_Log + ENTER);
	        }
        }
    }
}
