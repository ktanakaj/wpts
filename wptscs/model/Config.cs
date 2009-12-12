using System;
using System.Xml.Serialization;

namespace wptscs.model
{
    // XMLへの設定保存用クラス
    public class Config
    {
        // プログラムの処理モードを示す列挙値
		public enum RunType{
			[XmlEnum(Name = "Wikipedia")]
			Wikipedia};

		// クライアントとしての機能関係の設定を格納するクラス
		public class ClientConfig {
			// コンストラクタ
			public ClientConfig(){
				// メンバ変数の領域確保・初期設定
				RunMode = RunType.Wikipedia;
				SaveDirectory = "";
				LastSelectedSource = "en";
				LastSelectedTarget = "ja";
				UserAgent = "";
				Referer = "";
			}

			// プログラムの処理対象
			public RunType RunMode;

			// 実行結果を保存するフォルダ
			public String SaveDirectory;

			// 最後に指定していた翻訳元言語
			public String LastSelectedSource;
			// 最後に指定していた翻訳先言語
			public String LastSelectedTarget;

			// 通信時に使用するUserAgent
			public String UserAgent;
			// 通信時に使用するReferer
			public String Referer;
		};

		// コンストラクタ（通常）
		public Config(){
			// メンバ変数の領域確保・初期設定
			Client = new ClientConfig();
			Languages = new LanguageInformation[0];
		}
		// コンストラクタ（ファイル読み込みあり）
		public Config(String i_Path){
			// ファイルから設定を読み込み
			path = MYAPP.Cmn.NullCheckAndTrim(i_Path);
			if(Load() == false){
                // 失敗した場合、通常のコンストラクタと同じ処理で初期化
                Client = new ClientConfig();
                Languages = new LanguageInformation[0];
			}
		}

		// 設定をファイルに書き出し
		public bool Save(){
			// 設定をシリアライズ化
			if(path == ""){
				return false;
			}
			return MYAPP.Cmn.XmlSerialize(this, path);
		}
		// 設定をファイルから読み込み
		public bool Load(){
			// 設定をデシリアライズ化
			if(path == ""){
				return false;
			}
			Object obj = null;
			if(MYAPP.Cmn.XmlDeserialize(ref obj, this.GetType(), path) == true){
                Config config = obj as Config;
				if(config != null){
					this.Client = config.Client;
					this.Languages = config.Languages;
					return true;
				}
			}
			return false;
		}

		// 指定されたコードの言語情報（サーバー情報）を取得
		// ※存在しない場合、null
		public LanguageInformation GetLanguage(String i_Code, RunType i_Mode){
			Type type;
			if(i_Mode == RunType.Wikipedia){
				type = typeof(WikipediaInformation);
			}
			else{
                type = typeof(LanguageInformation);
			}
			foreach(LanguageInformation lang in Languages){
				if(lang.GetType() == type){
					if(lang.Code == i_Code){
						return lang;
					}
				}
			}
			return null;
		}
		// 指定されたコードの言語情報（サーバー情報）を取得（RunTypeの型）
		public LanguageInformation GetLanguage(String i_Code){
			return GetLanguage(i_Code, Client.RunMode);
		}

		// クライアントとしての機能関係の設定を保存
		public ClientConfig Client;

		// 言語ごとの情報（サーバーの設定なども）を保存
		[XmlArrayItem(typeof(LanguageInformation)),
		XmlArrayItem(typeof(LanguageWithServerInformation)),
		XmlArrayItem(typeof(WikipediaInformation))]
		public LanguageInformation[] Languages;

		// インスタンスのファイル名
		private String path;
    }
}
