// ================================================================================================
// <summary>
//      MediaWikiのウェブサイト（システム）をあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWiki.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// MediaWikiのウェブサイト（システム）をあらわすモデルクラスです。
    /// </summary>
    public class MediaWiki : Website
    {
        #region 定数定義

        /// <summary>
        /// WikipediaのXMLの固定値の書式。
        /// </summary>
        public static readonly string Xmlns = "http://www.mediawiki.org/xml/export-0.4/";

        /// <summary>
        /// テンプレートの名前空間を示す番号。
        /// </summary>
        public static readonly int TemplateNamespaceNumber = 10;

        /// <summary>
        /// カテゴリの名前空間を示す番号。
        /// </summary>
        public static readonly int CategoryNamespaceNumber = 14;

        /// <summary>
        /// 画像の名前空間を示す番号。
        /// </summary>
        public static readonly int ImageNamespaceNumber = 6;

        #endregion

        #region private変数

        // ※各変数の初期値は2006年9月時点のWikipedia英語版より

        /// <summary>
        /// Wikipedia書式のシステム定義変数。
        /// </summary>
        [XmlArrayItem("Variable")]
        public string[] SystemVariables = new string[]
        {
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
                "#if:"
        };

        /// <summary>
        /// 括弧のフォーマット。
        /// </summary>
        public string Bracket = " ({0}) ";

        /// <summary>
        /// リダイレクトの文字列。
        /// </summary>
        public string Redirect = "#REDIRECT";

        /// <summary>
        /// 名前空間の設定。
        /// </summary>
        [XmlIgnoreAttribute()]
        public IDictionary<int, string> Namespaces = new Dictionary<int, string>();

        /// <summary>
        /// 見出しの定型句。
        /// </summary>
        [XmlArrayItem("Title")]
        public string[] TitleKeys = new string[0];

        /// <summary>
        /// 記事のXMLデータが存在するパス。
        /// </summary>
        private string exportPath = "wiki/Special:Export/";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ（シリアライズ用）。
        /// </summary>
        public MediaWiki()
            : this(new Language("unknown"))
        {
            // 適当な値で通常のコンストラクタを実行
            System.Diagnostics.Debug.WriteLine("MediaWiki.MediaWiki > 推奨されないコンストラクタを使用しています");
        }

        /// <summary>
        /// コンストラクタ（通常）。
        /// </summary>
        /// <param name="lang">ウェブサイトの言語。</param>
        public MediaWiki(Language lang)
            : base(lang)
        {
            // メンバ変数の初期設定
            this.Server = String.Format("{0}.wikipedia.org", lang.Code);
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// ページを取得。
        /// </summary>
        /// <param name="title">ページタイトル。</param>
        /// <returns>取得したページ。ページが存在しない場合は <c>null</c> を返す。</returns>
        /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
        public override Page this[string title]
        {
            get
            {
                // 初期化と値チェック
                _Xml = null;
                _GetArticleStatus = HttpStatusCode.PaymentRequired;
                _GetArticleException = null;
                // 記事のXMLデータをWikipediaサーバーから取得
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
                    // UserAgent設定
                    // ※WikipediaはUserAgentが空の場合エラーとなるので、必ず設定する
                    if (!String.IsNullOrEmpty(i_UserAgent))
                    {
                        req.UserAgent = i_UserAgent;
                    }
                    else
                    {
                        Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                        req.UserAgent = "WikipediaTranslationSupportTool/" + ver.Major + "." + String.Format("{0:D2}", ver.Minor);
                    }
                    // Referer設定
                    if (!String.IsNullOrEmpty(i_Referer))
                    {
                        req.Referer = i_Referer;
                    }
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    _GetArticleStatus = res.StatusCode;

                    // 応答データを受信するためのStreamを取得し、データを取得
                    // ※取得したXMLが正常かは、ここでは確認しない
                    _Xml = new XmlDocument();
                    _Xml.Load(res.GetResponseStream());
                    res.Close();

                    // 取得したXMLを一時フォルダに保存
                    try
                    {
                        // 一時フォルダを確認
                        String tmpDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Application.ExecutablePath));
                        if (Directory.Exists(tmpDir) == false)
                        {
                            // 一時フォルダを作成
                            Directory.CreateDirectory(tmpDir);
                        }
                        // ファイルの保存
                        Xml.Save(Path.Combine(tmpDir, Honememo.Cmn.ReplaceInvalidFileNameChars(Title) + ".xml"));
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("WikipediaArticle.getServerArticle > 一時ファイルの保存に失敗しました : " + e.Message);
                    }
                }
                catch (WebException e)
                {
                    // ProtocolErrorエラーの場合、ステータスコードを保持
                    _Xml = null;
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        _GetArticleStatus = ((HttpWebResponse)e.Response).StatusCode;
                    }
                    _GetArticleException = e;
                    return false;
                }
                catch (Exception e)
                {
                    _Xml = null;
                    _GetArticleException = e;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 記事のXMLデータが存在するパス。
        /// </summary>
        public string ExportPath
        {
            get
            {
                return this.exportPath;
            }

            set
            {
                this.exportPath = value;
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 指定した言語での言語名称を ページ名|略称 の形式で取得。
        /// </summary>
        /// <param name="code">言語のコード。</param>
        /// <returns>ページ名|略称形式の言語名称。</returns>
        public string GetFullName(string code)
        {
            if (Lang.Names.ContainsKey(code))
            {
                Language.LanguageName name = Lang.Names[code];
                if (!String.IsNullOrEmpty(name.ShortName))
                {
                    return name.Name + "|" + name.ShortName;
                }
                else
                {
                    return name.Name;
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// 指定された文字列がWikipediaのシステム変数に相当かを判定。
        /// </summary>
        /// <param name="text">チェックする文字列。</param>
        /// <returns><c>true</c> システム変数に相当。</returns>
        public bool ChkSystemVariable(string text)
        {
            string s = text != null ? text : String.Empty;

            // 基本は全文一致だが、定数が : で終わっている場合、textの:より前のみを比較
            // ※ {{ns:1}}みたいな場合に備えて
            foreach (string variable in this.SystemVariables)
            {
                if (variable.EndsWith(":") == true)
                {
                    if (s.StartsWith(variable) == true)
                    {
                        return true;
                    }
                }
                else if (s == variable)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
