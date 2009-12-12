using System;
using System.Xml.Serialization;

namespace wptscs.model
{
    // 言語に関する情報を格納するクラス
    public class LanguageInformation : IComparable
    {
        // ある言語の、各言語での名称・略称を格納するための構造体
        public struct LanguageName : IComparable
        {
			// 言語コード
			[XmlAttributeAttribute("Code")]
			public String Code {
				get {
                    return _Code;
				}
				set {
					// ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
					if(((value != null) ? value.Trim() : "") == ""){
						throw new ArgumentNullException("value");
					}
					_Code = value.Trim().ToLower();
				}
			}
			public String Name;			// その言語の名称（Wikipediaの場合、記事名）
			public String ShortName;		// その言語の略称

			// 配列のソート用メソッド
            public int CompareTo(Object obj)
            {
				// 言語コードでソート
                LanguageName name = (LanguageName)obj;
				return this.Code.CompareTo(name.Code);
			}
			// 言語コード（property）
			private String _Code;
		};

		// コンストラクタ（シリアライズ用）
        public LanguageInformation() : this("unknown")
        {
//			System.Diagnostics.Debug.WriteLine("LanguageInformation.LanguageInformation > 推奨されないコンストラクタを使用しています");
			// 適当な値で通常のコンストラクタを実行
		}
		// コンストラクタ（通常）
		public LanguageInformation(String i_Code){
			// メンバ変数の領域確保と初期設定
			Code = i_Code;
			Names = new LanguageName[1];
			Names[0].Code = i_Code;
			Names[0].Name = "";
			Names[0].ShortName = "";
		}

		// 配列のソート用メソッド
        public virtual int CompareTo(Object obj)
        {
			// 言語コードでソート
            LanguageInformation lang = obj as LanguageInformation;
			return this.Code.CompareTo(lang.Code);
		}

		// 指定した言語での名称を取得
		public String GetName(String i_Code){
			foreach(LanguageName name in Names){
				if(name.Code == i_Code){
					return name.Name;
				}
			}
			return "";
		}

		// 言語コード
		[XmlAttributeAttribute("Code")]
		public String Code {
			get {
				return _Code;
			}
			set {
				// ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
				if(((value != null) ? value.Trim() : "") == ""){
					throw new ArgumentNullException("value");
				}
				_Code = value.Trim().ToLower();
			}
		}
		// この言語の、各言語での名称
		public LanguageName[] Names;

		// 言語コード（property）
		private String _Code;
    }
}
