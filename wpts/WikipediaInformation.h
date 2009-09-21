// Wikipediaの各種情報・設定などを管理するためのクラス
#ifndef WikipediaInformationH
#define WikipediaInformationH

#include "Language.h"

namespace wpts {

	using namespace System;
	using namespace System::Xml::Serialization;

	// 言語情報・サーバー情報に加え、Wikipediaのサーバーごとの設定を格納するクラス
	public ref class WikipediaInformation : public LanguageWithServerInformation {
	public:
		// 各サーバーでの名前空間の設定を格納するための構造体
		value struct Namespace : IComparable {
			int Key;				// 名前空間の番号
			String ^Name;			// 名前空間名

			// 配列のソート用メソッド
			virtual int CompareTo(Object^);
		};

		// コンストラクタ（シリアライズ用）
		WikipediaInformation(void)
			: LanguageWithServerInformation(){
//			System::Diagnostics::Debug::WriteLine("WikipediaInformation::WikipediaInformation > 推奨されないコンストラクタを使用しています");
			// 適当な値で通常のコンストラクタを実行
			this->WikipediaInformation::WikipediaInformation("unknown");
		}
		// コンストラクタ（通常）
		WikipediaInformation(String ^i_Code)
			: LanguageWithServerInformation(i_Code){
			// 初期値設定
			setDefault();
		}
		// デストラクタ
		virtual ~WikipediaInformation(){}

		// 指定した言語での名称を 記事名|略称 の形式で取得
		String ^GetFullName(String^);
		// 指定された番号の名前空間を取得
		String^ GetNamespace(int);
		// 指定された文字列がWikipediaのシステム変数に相当かを判定
		bool ChkSystemVariable(String^);

		// 記事のXMLデータが存在するパス
		property String ^ArticleXmlPath {
			String^ get()
			{
				return _ArticleXmlPath;
			}
			void set(String ^i_Path)
			{
				_ArticleXmlPath = ((i_Path != nullptr) ? i_Path->Trim() : "");
			}
		}

		// Wikipedia書式のシステム定義変数
		[XmlArrayItem("Variable")]
		array<String^> ^SystemVariables;

		// 括弧のフォーマット
		String ^Bracket;
		// リダイレクトの文字列
		String ^Redirect;

		// 名前空間の設定
		[XmlIgnoreAttribute()]
		array<Namespace> ^Namespaces;
		// テンプレート・カテゴリ・画像の名前空間を示す番号
		const static int TEMPLATENAMESPACENUMBER = 10;
		const static int CATEGORYNAMESPACENUMBER = 14;
		const static int IMAGENAMESPACENUMBER = 6;

		// 見出しの定型句
		[XmlArrayItem("Title")]
		array<String ^> ^TitleKeys;

	private:
		// メンバ変数の初期値設定
		void setDefault(void);

		// 記事のXMLデータが存在するパス（property）
		String ^_ArticleXmlPath;
	};

}
#endif
