using System;
using System.Xml.Serialization;

namespace wptscs.model
{
    // 言語情報・サーバー情報に加え、Wikipediaのサーバーごとの設定を格納するクラス
    public class WikipediaInformation : LanguageWithServerInformation
    {
        // 各サーバーでの名前空間の設定を格納するための構造体
		public struct Namespace : IComparable {
			public int Key;				// 名前空間の番号
			public String Name;			// 名前空間名

			// 配列のソート用メソッド
            public int CompareTo(Object obj)
            {
	            // 名前空間の番号でソート
                Namespace ns = (Namespace) obj;
	            return this.Key.CompareTo(ns.Key);
            }
		};

		// コンストラクタ（シリアライズ用）
        public WikipediaInformation() : this("unknown")
        {
//			System.Diagnostics.Debug.WriteLine("WikipediaInformation.WikipediaInformation > 推奨されないコンストラクタを使用しています");
			// 適当な値で通常のコンストラクタを実行
		}
		// コンストラクタ（通常）
		public WikipediaInformation(String i_Code) : base(i_Code){
			// 初期値設定
			setDefault();
		}
    
        /* メンバ変数の初期値設定 */
        public void setDefault()
        {
	        // メンバ変数の領域確保・初期設定
	        // ※各初期値は2006年9月時点のWikipedia英語版より
	        Server = String.Format("{0}.wikipedia.org", Code);
	        ArticleXmlPath = "wiki/Special:Export/";
	        SystemVariables = new String[]{
		        "CURRENTMONTH",
		        "CURRENTMONTHNAME",
		        "CURRENTDAY",
		        "CURRENTDAYNAME",
		        "CURRENTYEAR",
		        "CURRENTTIME",
		        "NUMBEROFARTICLES",
		        "SITENAME",
		        "SERVER",
		        "NAMESPACE",
		        "PAGENAME",
		        "ns:",
		        "localurl:",
		        "fullurl:",
		        "#if:"};
	        Bracket = " ({0}) ";
	        Redirect = "#REDIRECT";
	        Namespaces = new Namespace[0];
	        TitleKeys = new String[0];
        }

        /* 指定した言語での名称を 記事名|略称 の形式で取得 */
        public String GetFullName(String i_Code)
        {
	        foreach(LanguageName name in Names){
		        if(name.Code == i_Code){
			        if(name.ShortName != ""){
				        return (name.Name + "|" + name.ShortName);
			        }
			        else{
				        return name.Name;
			        }
		        }
	        }
	        return "";
        }

        /* 指定された番号の名前空間を取得 */
        public String GetNamespace(int i_Key)
        {
	        foreach(Namespace ns in Namespaces){
		        if(ns.Key == i_Key){
			        return ns.Name;
		        }
	        }
	        return "";
        }

        /* 指定された文字列がWikipediaのシステム変数に相当かを判定 */
        public bool ChkSystemVariable(String i_Text)
        {
	        String text = ((i_Text != null) ? i_Text : "");
	        // 基本は全文一致だが、定数が : で終わっている場合、textの:より前のみを比較
	        // ※ {{ns:1}}みたいな場合に備えて
	        foreach(String variable in SystemVariables){
		        if(variable.EndsWith(":") == true){
			        if(text.StartsWith(variable) == true){
				        return true;
			        }
		        }
		        else if(text == variable){
			        return true;
		        }
	        }
	        return false;
        }

		// 記事のXMLデータが存在するパス
		public String ArticleXmlPath {
			get {
				return _ArticleXmlPath;
			}
			set {
				_ArticleXmlPath = ((value != null) ? value.Trim() : "");
			}
		}

		// Wikipedia書式のシステム定義変数
		[XmlArrayItem("Variable")]
		public String[] SystemVariables;

		// 括弧のフォーマット
		public String Bracket;
		// リダイレクトの文字列
		public String Redirect;

		// 名前空間の設定
		[XmlIgnoreAttribute()]
		public Namespace[] Namespaces;
		// テンプレート・カテゴリ・画像の名前空間を示す番号
		public static readonly int TEMPLATENAMESPACENUMBER = 10;
        public static readonly int CATEGORYNAMESPACENUMBER = 14;
        public static readonly int IMAGENAMESPACENUMBER = 6;

		// 見出しの定型句
		[XmlArrayItem("Title")]
		public String[] TitleKeys;

		// 記事のXMLデータが存在するパス（property）
		private String _ArticleXmlPath;
    }
}
