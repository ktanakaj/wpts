// 画面・機能によらない、共通的な関数クラス（2006年9月16日版）
#ifndef MYCmnH
#define MYCmnH

namespace MYAPP {

	using namespace System;
	using namespace System::Resources;
	using namespace System::Reflection;
	using namespace System::Windows::Forms;
	using namespace System::IO;
	using namespace System::Net;
	using namespace System::Xml::Serialization;

	public ref class Cmn {
	public:
		// コンストラクタ
		// ※exeと同名のリソースマネージャーを起動フォルダから読み込み
		//   指定されたリソースマネージャーを指定されたフォルダから設定
		//   渡されたリソースマネージャーを使用、の3種類
		Cmn(void);
		Cmn(String^, String^);
		Cmn(ResourceManager^);
		// デストラクタ
		virtual ~Cmn();

		// ↓要リソースマネージャー

		// 共通ダイアログ（通知／警告／エラー）
		// ※共通ダイアログは、入力された文字列をそのまま表示するものと、入力された文字列
		//   でリソースを取得、フォーマットして表示するものの各2種類
		virtual void InformationDialog(String^);
		virtual void InformationDialogResource(String^, ... array<Object^>^);
		virtual void WarningDialog(String^);
		virtual void WarningDialogResource(String^, ... array<Object^>^);
		virtual void ErrorDialog(String^);
		virtual void ErrorDialogResource(String^, ... array<Object^>^);

		// ↓リソースマネージャーがnullptrでも動作、その場合はダイアログが出ない

		// フォルダ／ファイルオープン
		virtual bool OpenFolder(String^, bool);
		bool OpenFolder(String^);
		virtual bool OpenFile(String^, bool);
		bool OpenFile(String^);

		// サーバー接続チェック
		virtual bool Ping(String^, bool);
		bool Ping(String^);

		// DataGridViewのCSVファイルへの出力
		virtual bool SaveDataGridViewCsv(DataGridView^, String^, bool);
		bool SaveDataGridViewCsv(DataGridView^, String^);

		// ↓以下は静的メンバ関数

		// NULL値チェック＆Trim
		static String^ NullCheckAndTrim(String^);
		static String^ NullCheckAndTrim(DataGridViewCell^);

		// 配列への要素追加
		generic<typename T> static int AddArray(array<T>^ %);
		generic<typename T> static int AddArray(array<T>^ %, T);

		// ソフト名+バージョン情報の文字列取得（アセンブリから取得）
		static String^ GetProductName(void);

		// 文字列中のファイル名に使用できない文字を置換
		static String^ ReplaceInvalidFileNameChars(String^);

		// オブジェクトのXMLへのシリアライズ、デシリアライズ
		// ※デシリアライズは、オブジェクト型で結果を受け取り、その後に呼び出し元でキャストすること
		static bool XmlSerialize(Object^, String^);
		static bool XmlDeserialize(Object^ %, Type^, String^);

		// 対象の文字列が、渡された文字列のインデックス番目に存在するかをチェック
		static bool ChkTextInnerWith(String^, int, String^);

		// コンボボックスを確認し、現在の値が一覧に無ければ登録
		static bool AddComboBoxNewItem(ComboBox^ %, String^);
		static bool AddComboBoxNewItem(ComboBox^ %);
		// コンボボックスを確認し、選択されている値を削除
		static bool RemoveComboBoxItem(ComboBox^ %);

		// リソースマネージャー
		ResourceManager ^Resource;
	};

}

#endif
