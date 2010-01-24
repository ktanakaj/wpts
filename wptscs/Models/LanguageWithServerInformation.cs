using System;

namespace Honememo.Wptscs.Models
{
    // 言語情報と、関連するサーバー情報を格納するクラス
    public class LanguageWithServerInformation : LanguageInformation
    {
        // コンストラクタ（シリアライズ用）
        public LanguageWithServerInformation() : this("unknown")
        {
            //System.Diagnostics.Debug.WriteLine("LanguageWithServerInformation.LanguageWithServerInformation > 推奨されないコンストラクタを使用しています");
            // 適当な値で通常のコンストラクタを実行
        }
        // コンストラクタ（通常）
        public LanguageWithServerInformation(String i_Code) : base(i_Code){
            // 初期値設定
            // ※このクラスは定義のみ。実際の設定は、継承したクラスで行う
            Server = "unknown";
        }

        // サーバーの名称
        public String Server {
            get {
                return _Server;
            }
            set {
                // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
                if(((value != null) ? value.Trim() : "") == ""){
                    throw new ArgumentNullException("i_Name");
                }
                _Server = value.Trim();
            }
        }

        // サーバーの名称（property）
        private String _Server;
    }
}
