// 言語関係の各種設定などを管理するためのクラス
#ifndef LanguageH
#define LanguageH

namespace wpts {

	using namespace System;
	using namespace System::Xml::Serialization;

	// 言語に関する情報を格納するクラス
	public ref class LanguageInformation : IComparable {
	public:
		// ある言語の、各言語での名称・略称を格納するための構造体
		value struct LanguageName : IComparable {
		public:
			// 言語コード
			[XmlAttributeAttribute("Code")]
			property String ^Code {
				String^ get()
				{
					return _Code;
				}
				void set(String ^i_Code)
				{
					// ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
					if(((i_Code != nullptr) ? i_Code->Trim() : "") == ""){
						throw gcnew ArgumentNullException("i_Code");
					}
					_Code = i_Code->Trim()->ToLower();
				}
			}
			String ^Name;			// その言語の名称（Wikipediaの場合、記事名）
			String ^ShortName;		// その言語の略称

			// 配列のソート用メソッド
			virtual int CompareTo(Object ^obj){
				// 言語コードでソート
				LanguageName ^name = dynamic_cast<LanguageName^>(obj);
				return this->Code->CompareTo(name->Code);
			}
		private:
			// 言語コード（property）
			String ^_Code;
		};

		// コンストラクタ（シリアライズ用）
		LanguageInformation(void){
//			System::Diagnostics::Debug::WriteLine("LanguageInformation::LanguageInformation > 推奨されないコンストラクタを使用しています");
			// 適当な値で通常のコンストラクタを実行
			this->LanguageInformation::LanguageInformation("unknown");
		}
		// コンストラクタ（通常）
		LanguageInformation(String ^i_Code){
			// メンバ変数の領域確保と初期設定
			Code = i_Code;
			Names = gcnew array<LanguageName>(1);
			Names[0].Code = i_Code;
			Names[0].Name = "";
			Names[0].ShortName = "";
		}
		// デストラクタ
		virtual ~LanguageInformation(){}

		// 配列のソート用メソッド
		virtual int CompareTo(Object ^obj){
			// 言語コードでソート
			LanguageInformation ^lang = dynamic_cast<LanguageInformation^>(obj);
			return this->Code->CompareTo(lang->Code);
		}

		// 指定した言語での名称を取得
		String ^GetName(String ^i_Code){
			for each(LanguageName ^name in Names){
				if(name->Code == i_Code){
					return name->Name;
				}
			}
			return "";
		}

		// 言語コード
		[XmlAttributeAttribute("Code")]
		property String ^Code {
			String^ get()
			{
				return _Code;
			}
			void set(String ^i_Code)
			{
				// ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
				if(((i_Code != nullptr) ? i_Code->Trim() : "") == ""){
					throw gcnew ArgumentNullException("i_Code");
				}
				_Code = i_Code->Trim()->ToLower();
			}
		}
		// この言語の、各言語での名称
		array<LanguageName> ^Names;

	private:
		// 言語コード（property）
		String ^_Code;
	};

	// 言語情報と、関連するサーバー情報を格納するクラス
	public ref class LanguageWithServerInformation : public LanguageInformation {
	public:
		// コンストラクタ（シリアライズ用）
		LanguageWithServerInformation(void)
			: LanguageInformation(){
//			System::Diagnostics::Debug::WriteLine("LanguageWithServerInformation::LanguageWithServerInformation > 推奨されないコンストラクタを使用しています");
			// 適当な値で通常のコンストラクタを実行
			this->LanguageWithServerInformation::LanguageWithServerInformation("unknown");
		}
		// コンストラクタ（通常）
		LanguageWithServerInformation(String ^i_Code)
			: LanguageInformation(i_Code){
			// 初期値設定
			// ※このクラスは定義のみ。実際の設定は、継承したクラスで行う
			Server = "unknown";
		}
		// デストラクタ
		virtual ~LanguageWithServerInformation(){}

		// サーバーの名称
		property String ^Server {
			String^ get()
			{
				return _Server;
			}
			void set(String ^i_Name)
			{
				// ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
				if(((i_Name != nullptr) ? i_Name->Trim() : "") == ""){
					throw gcnew ArgumentNullException("i_Name");
				}
				_Server = i_Name->Trim();
			}
		}

	private:
		// サーバーの名称（property）
		String ^_Server;
	};

}
#endif
