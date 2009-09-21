// XMLへの設定保存用クラス
#ifndef ConfigH
#define ConfigH

#include "stdafx.h"

namespace wpts {

	using namespace System;
	using namespace System::Xml::Serialization;

	public ref class Config {
	public:
		// プログラムの処理モードを示す列挙値
		enum class RunType{
			[XmlEnum(Name = "Wikipedia")]
			Wikipedia};

		// クライアントとしての機能関係の設定を格納するクラス
		ref class ClientConfig {
		public:
			// コンストラクタ
			ClientConfig(void){
				// メンバ変数の領域確保・初期設定
				RunMode = RunType::Wikipedia;
				SaveDirectory = "";
				LastSelectedSource = "en";
				LastSelectedTarget = "ja";
				UserAgent = "";
				Referer = "";
			}
			// デストラクタ
			virtual ~ClientConfig(){}

			// プログラムの処理対象
			RunType RunMode;

			// 実行結果を保存するフォルダ
			String ^SaveDirectory;

			// 最後に指定していた翻訳元言語
			String ^LastSelectedSource;
			// 最後に指定していた翻訳先言語
			String ^LastSelectedTarget;

			// 通信時に使用するUserAgent
			String ^UserAgent;
			// 通信時に使用するReferer
			String ^Referer;
		};

		// コンストラクタ（通常）
		Config(void){
			// メンバ変数の領域確保・初期設定
			Client = gcnew ClientConfig();
			Languages = gcnew array<LanguageInformation^>(0);;
		}
		// コンストラクタ（ファイル読み込みあり）
		Config(String ^i_Path){
			// ファイルから設定を読み込み
			path = MYAPP::Cmn::NullCheckAndTrim(i_Path);
			if(Load() == false){
				// 失敗した場合、通常のコンストラクタで初期化
				this->Config::Config();
			}
		}
		// デストラクタ
		~Config(){}

		// 設定をファイルに書き出し
		bool Save(void){
			// 設定をシリアライズ化
			if(path == ""){
				return false;
			}
			return MYAPP::Cmn::XmlSerialize(this, path);
		}
		// 設定をファイルから読み込み
		bool Load(void){
			// 設定をデシリアライズ化
			if(path == ""){
				return false;
			}
			Object ^obj = nullptr;
			if(MYAPP::Cmn::XmlDeserialize(obj, this->GetType(), path) == true){
				Config ^config = dynamic_cast<Config^>(obj);
				if(config != nullptr){
					this->Client = config->Client;
					this->Languages = config->Languages;
					return true;
				}
			}
			return false;
		}

		// 指定されたコードの言語情報（サーバー情報）を取得
		// ※存在しない場合、nullptr
		LanguageInformation^ GetLanguage(String ^i_Code, RunType i_Mode){
			Type ^type;
			if(i_Mode == RunType::Wikipedia){
				type = WikipediaInformation::typeid;
			}
			else{
				type = LanguageInformation::typeid;
			}
			for each(LanguageInformation ^lang in Languages){
				if(lang->GetType() == type){
					if(lang->Code == i_Code){
						return lang;
					}
				}
			}
			return nullptr;
		}
		// 指定されたコードの言語情報（サーバー情報）を取得（RunTypeの型）
		LanguageInformation^ GetLanguage(String ^i_Code){
			return GetLanguage(i_Code, Client->RunMode);
		}

		// クライアントとしての機能関係の設定を保存
		ClientConfig ^Client;

		// 言語ごとの情報（サーバーの設定なども）を保存
		[XmlArrayItem(LanguageInformation::typeid),
		XmlArrayItem(LanguageWithServerInformation::typeid),
		XmlArrayItem(WikipediaInformation::typeid)]
		array<LanguageInformation^> ^Languages;

	private:
		// インスタンスのファイル名
		String ^path;
	};
}
#endif