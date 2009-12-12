using System;
using System.Net.NetworkInformation;

using wptscs.model;
using wptscs.Properties;

namespace wptscs.logic
{
    public abstract class TranslateNetworkObject : Translate
    {
		// 通信時に使用するUserAgent
        public String UserAgent;
		// 通信時に使用するReferer
        public String Referer;

		// コンストラクタ
		public TranslateNetworkObject(LanguageWithServerInformation i_Source, LanguageWithServerInformation i_Target)
			: base(i_Source, i_Target) {}
        
        /* 翻訳支援処理実行 */
        public override bool Run(String i_Name)
        {
	        // 変数を初期化
	        runInitialize();
	        // サーバー接続チェック
	        if(ping(((LanguageWithServerInformation) source).Server) == false){
		        return false;
	        }
	        // 翻訳支援処理実行部の本体を実行
	        // ※以降の処理は、継承クラスにて定義
	        return runBody(i_Name);
        }

        /* サーバー接続チェック */
        private bool ping(String i_Server)
        {
	        // サーバー接続チェック
	        Ping ping = new Ping();
	        try{
		        PingReply reply = ping.Send(i_Server);
		        if(reply.Status != IPStatus.Success){
			        logLine(String.Format(Resources.ErrorMessage_MissNetworkAccess, reply.Status.ToString()));
			        return false;
		        }
	        }
	        catch(Exception e){
		        logLine(String.Format(Resources.ErrorMessage_MissNetworkAccess, e.InnerException.Message));
		        return false;
	        }
	        return true;
        }
    }
}
