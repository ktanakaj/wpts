using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Windows.Forms;

namespace Honememo.Wptscs.Models
{
    // Wikipediaの記事を管理するためのクラス
    public class WikipediaArticle : WikipediaFormat
    {
        // コンストラクタ（サーバーと記事名を指定）
        public WikipediaArticle(WikipediaInformation i_Server, String i_Name)
            : base(i_Server){
            // 初期設定
            Initialize(i_Name);
        }

        /* 初期設定 */
        public void Initialize(String i_Title)
        {
            // ※必須な情報が設定されていない場合、ArgumentNullExceptionを返す
            if(Honememo.Cmn.NullCheckAndTrim(i_Title).TrimStart(':') == ""){
                throw new ArgumentNullException("i_Title");
            }
            // メンバ変数の初期化
            _Title = i_Title.Trim().TrimStart(':');
            UriBuilder uri = new UriBuilder("http", Server.Server);
            uri.Path = (Server.ArticleXmlPath + Title);
            _Url = uri.Uri;
            _Xml = null;
            _Timestamp = DateTime.MinValue;
            _Text = "";
            _Redirect = "";
            _GetArticleStatus = HttpStatusCode.PaymentRequired;
            _GetArticleException = null;
        }

        /* 記事の詳細情報を取得 */
        public virtual bool GetArticle(String i_UserAgent, String i_Referer, TimeSpan i_CacheEnabledSpan)
        {
            // 初期化と値チェック
            _Xml = null;
            _Timestamp = DateTime.MinValue;
            _Text = "";
            _Redirect = "";
            _GetArticleStatus = HttpStatusCode.PaymentRequired;
            _GetArticleException = null;
            // 記事のデータをキャッシュやWikipediaサーバーから取得し、XMLに格納
            if(getCacheArticle(i_CacheEnabledSpan) == false){
                if(getServerArticle(i_UserAgent, i_Referer) == false){
                    return false;
                }
            }
            // 取得されたXMLを解析し、メンバ変数に設定
            // 名前空間情報の上書き
            _Server.Namespaces = GetNamespaces();
            // 記事情報の設定
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(Xml.NameTable);
            nsMgr.AddNamespace("ns", XMLNS);
            XmlElement pageElement = (XmlElement) Xml.SelectSingleNode("/ns:mediawiki/ns:page", nsMgr);
            if(pageElement != null){
                // 記事名の上書き
                XmlElement titleElement = (XmlElement) pageElement.SelectSingleNode("ns:title", nsMgr);
                _Title = (!String.IsNullOrEmpty(titleElement.InnerText) ? titleElement.InnerText : Title);
                // 最終更新日時
                XmlElement timeElement = (XmlElement) pageElement.SelectSingleNode("ns:revision/ns:timestamp", nsMgr);
                _Timestamp = DateTime.Parse(timeElement.InnerText);
                // 記事本文
                XmlElement textElement = (XmlElement) pageElement.SelectSingleNode("ns:revision/ns:text", nsMgr);
                _Text = textElement.InnerText;
                // リダイレクトのチェックを行っておく
                IsRedirect();
            }
            // 記事が存在しない場合、XMLは取得できるがpageノードが無いので、404エラーと同様に扱う
            else{
                _GetArticleStatus = HttpStatusCode.NotFound;
                return false;
            }
            return true;
        }

        /* 記事の詳細情報を取得（キャッシュ有効期間はデフォルト） */
        public bool GetArticle(String i_UserAgent, String i_Referer)
        {
            // キャッシュ有効期間1週間でGetArticleを実行
            // ※記事の有無や名称、リダイレクト、言語間リンク等はそんなに更新されないだろう
            //   ・・・ということで、この期間に。
            //   必要であれば、キャッシュを使わない設定で本メソッドを直接呼ぶこと
            return GetArticle(i_UserAgent, i_Referer, new TimeSpan(7, 0, 0, 0));
        }

        /* 記事の詳細情報を取得（UserAgent, Referer, キャッシュ有効期間はデフォルト） */
        public bool GetArticle()
        {
            // 既定値でGetArticleを実行
            return GetArticle("", "");
        }

        /* 記事のXMLをサーバーより取得 */
        protected bool getServerArticle(String i_UserAgent, String i_Referer)
        {
            // 初期化と値チェック
            _Xml = null;
            _GetArticleStatus = HttpStatusCode.PaymentRequired;
            _GetArticleException = null;
            // 記事のXMLデータをWikipediaサーバーから取得
            try{
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(Url);
                // UserAgent設定
                // ※WikipediaはUserAgentが空の場合エラーとなるので、必ず設定する
                if(!String.IsNullOrEmpty(i_UserAgent)){
                    req.UserAgent = i_UserAgent;
                }
                else{
                    Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    req.UserAgent = "WikipediaTranslationSupportTool/" + ver.Major + "." + String.Format("{0:D2}",ver.Minor);
                }
                // Referer設定
                if(!String.IsNullOrEmpty(i_Referer)){
                    req.Referer = i_Referer;
                }
                HttpWebResponse res = (HttpWebResponse) req.GetResponse();
                _GetArticleStatus = res.StatusCode;

                // 応答データを受信するためのStreamを取得し、データを取得
                // ※取得したXMLが正常かは、ここでは確認しない
                _Xml = new XmlDocument();
                _Xml.Load(res.GetResponseStream());
                res.Close();

                // 取得したXMLを一時フォルダに保存
                try{
                    // 一時フォルダを確認
                    String tmpDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Application.ExecutablePath));
                    if(Directory.Exists(tmpDir) == false){
                        // 一時フォルダを作成
                        Directory.CreateDirectory(tmpDir);
                    }
                    // ファイルの保存
                    Xml.Save(Path.Combine(tmpDir, Honememo.Cmn.ReplaceInvalidFileNameChars(Title) + ".xml"));
                }
                catch(Exception e){
                    System.Diagnostics.Debug.WriteLine("WikipediaArticle.getServerArticle > 一時ファイルの保存に失敗しました : " + e.Message);
                }
            }
            catch(WebException e){
                // ProtocolErrorエラーの場合、ステータスコードを保持
                _Xml = null;
                if(e.Status == WebExceptionStatus.ProtocolError){
                    _GetArticleStatus = ((HttpWebResponse) e.Response).StatusCode;
                }
                _GetArticleException = e;
                return false;
            }
            catch(Exception e){
                _Xml = null;
                _GetArticleException = e;
                return false;
            }
            return true;
        }

        /* 記事のXMLをキャッシュより取得 */
        protected bool getCacheArticle(TimeSpan i_CacheEnabledSpan)
        {
            // 初期化と値チェック
            _Xml = null;
            _GetArticleStatus = HttpStatusCode.PaymentRequired;
            _GetArticleException = null;
            // キャッシュを使用する場合のみ
            if(i_CacheEnabledSpan > new TimeSpan(0)){
                // 記事のXMLデータをキャッシュファイルから取得
                try{
                    // 一時ファイルにアクセス
                    String tmpFile = Path.Combine(Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Application.ExecutablePath)), Honememo.Cmn.ReplaceInvalidFileNameChars(Title) + ".xml");
                    if(File.Exists(tmpFile) == true){
                        // ファイルが有効期限内のものかを確認
                        if((DateTime.UtcNow - File.GetLastWriteTimeUtc(tmpFile)) < i_CacheEnabledSpan){
                            // ファイルをStreamで開き、データを取得
                            XmlDocument tmpXml = new XmlDocument();
                            FileStream fs = File.OpenRead(tmpFile);
                            try{
                                tmpXml.Load(fs);
                            }
                            finally{
                                fs.Close();
                            }
                            // 取得したXMLファイルが、目的とする記事のものかを確認
                            XmlNamespaceManager nsMgr = new XmlNamespaceManager(tmpXml.NameTable);
                            nsMgr.AddNamespace("ns", XMLNS);
                            XmlElement rootElement = tmpXml.DocumentElement;
                            XmlElement pageElement = (XmlElement) tmpXml.SelectSingleNode("/ns:mediawiki/ns:page/ns:title", nsMgr);
                            if(pageElement != null){
                                // 言語コード・記事名をチェック。大文字・小文字が異なる場合、別の記事と判別する
                                // ※Low Earth orbitへのリダイレクトでLow earth orbitみたいなのがあるため
                                //   ただし先頭はWikipediaの技術的制限で常に大文字なため、大文字で処理する
                                String title = char.ToUpper(Title[0]).ToString();
                                if(Title.Length > 1){
                                    title += Title.Substring(1);
                                }
                                if(rootElement.GetAttribute("xml:lang") == Server.Code &&
                                   pageElement.InnerText == title){
                                    // XMLをメンバ変数に設定し、正常終了
                                    System.Diagnostics.Debug.WriteLine("WikipediaArticle.getCacheArticle > キャッシュ読込み : " + Honememo.Cmn.ReplaceInvalidFileNameChars(Title) + ".xml");
                                    _Xml = tmpXml;
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch(Exception e){
                    _Xml = null;
                    _GetArticleException = e;
                    return false;
                }
            }
            _GetArticleStatus = HttpStatusCode.NotFound;
            return false;
        }

        /* 指定された言語コードへの言語間リンクを返す */
        public virtual String GetInterWiki(String i_Code)
        {
            // 初期化と値チェック
            String interWiki = "";
            if(Text == ""){
                // GetArticleを行っていない場合、InvalidOperationExceptionを返す
                throw new InvalidOperationException();
            }
            // 記事に存在する指定言語への言語間リンクを取得
            for(int i = 0 ; i < Text.Length ; i++){
                // コメント（<!--）のチェック
                String comment = "";
                int index = chkComment(ref comment, i);
                if(index != -1){
                    i = index;
                }
                // 指定言語への言語間リンクの場合、内容を取得し、処理終了
                else if(Honememo.Cmn.ChkTextInnerWith(Text, i, "[[" + i_Code + ":") == true){
                    Link link = ParseInnerLink(Text.Substring(i));
                    if(!String.IsNullOrEmpty(link.Text)){
                        interWiki = link.Article;
                        break;
                    }
                }
            }
            return interWiki;
        }

        /* 記事のXMLから名前空間情報を取得 */
        public virtual WikipediaInformation.Namespace[] GetNamespaces()
        {
            // XMLから名前空間情報を取得
            WikipediaInformation.Namespace[] namespaces = new WikipediaInformation.Namespace[0];
            if(Xml == null){
                // GetArticleを行っていない場合、InvalidOperationExceptionを返す
                throw new InvalidOperationException();
            }
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(Xml.NameTable);
            nsMgr.AddNamespace("ns", XMLNS);
            XmlNodeList nodeList = Xml.SelectNodes("/ns:mediawiki/ns:siteinfo/ns:namespaces/ns:namespace", nsMgr);
            foreach(XmlNode node in nodeList){
                XmlElement e = (XmlElement) node;
                if(e != null){
                    try{
                        WikipediaInformation.Namespace ns = new WikipediaInformation.Namespace();
                        ns.Key = Decimal.ToInt16(Decimal.Parse(e.GetAttribute("key")));
                        ns.Name = e.InnerText;
                        Honememo.Cmn.AddArray(ref namespaces, ns);
                    }
                    catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine("WikipediaArticle.GetNamespaces > 例外発生 : " + ex);
                    }
                }
            }
            return namespaces;
        }

        /* 記事がリダイレクトかをチェック */
        public virtual bool IsRedirect()
        {
            // 値チェック
            if(Text == ""){
                // GetArticleを行っていない場合、InvalidOperationExceptionを返す
                throw new InvalidOperationException();
            }
            // 指定された記事がリダイレクト記事（#REDIRECT等）かをチェック
            // ※日本語版みたいに、#REDIRECTと言語固有の#転送みたいなのがあると思われるので、
            //   翻訳元言語と英語版の設定でチェック
            for(int i = 0 ; i < 2 ; i++){
                String redirect = (String) Server.Redirect.Clone();
                if(i == 1){
                    if(Server.Code == "en"){
                        continue;
                    }
                    WikipediaInformation en = new WikipediaInformation("en");
                    redirect = en.Redirect;
                }
                if(redirect != ""){
                    if(Text.ToLower().StartsWith(redirect.ToLower())){
                        Link link = ParseInnerLink(Text.Substring(redirect.Length).TrimStart());
                        if(!String.IsNullOrEmpty(link.Text)){
                            _Redirect = link.Article;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /* 記事がカテゴリーかをチェック */
        public virtual bool IsCategory()
        {
            return IsCategory(Title);
        }

        /* 記事が画像かをチェック */
        public virtual bool IsImage()
        {
            return IsImage(Title);
        }

        /* 記事が標準名前空間以外かをチェック */
        public bool IsNotMainNamespace()
        {
            return IsNotMainNamespace(Title);
        }

        /* 渡された内部リンク・テンプレートを解析 */
        protected virtual int chkLinkText(ref WikipediaFormat.Link o_Link, int i_Index)
        {
            return ChkLinkText(ref o_Link, Text, i_Index);
        }

        /* 記事本文の指定された位置に存在する内部リンク・テンプレートを解析 */
        protected virtual int chkComment(ref String o_Text, int i_Index)
        {
            return ChkComment(ref o_Text, Text, i_Index);
        }

        // 記事名
        public String Title {
            get {
                return _Title;
            }
        }
        // 記事のXMLデータのURL（property）
        public Uri Url {
            get {
                return _Url;
            }
        }
        // 記事のXMLデータ
        public XmlDocument Xml {
            get {
                return _Xml;
            }
        }
        // 記事の最終更新日時（UTC）
        public DateTime Timestamp {
            get {
                return _Timestamp;
            }
        }
        // 記事本文
        public String Text {
            get {
                return _Text;
            }
        }
        // リダイレクト先記事名
        public String Redirect {
            get {
                return _Redirect;
            }
        }
        // GetArticle実行時のHttpStatus
        public HttpStatusCode GetArticleStatus {
            get {
                return _GetArticleStatus;
            }
        }
        // GetArticle例外発生時の例外情報
        // ※GetArticle()がfalseで、GetArticleStatusがNotFound以外のとき、設定される
        public Exception GetArticleException {
            get {
                return _GetArticleException;
            }
        }

        // WikipediaのXMLの固定値の書式
        public static readonly String XMLNS = "http://www.mediawiki.org/xml/export-0.4/";

        // 記事名（property）
        protected String _Title;
        // 記事のXMLデータのURL（property）
        protected Uri _Url;
        // 記事のXMLデータ（property）
        protected XmlDocument _Xml;
        // 記事の最終更新日時（UTC）（property）
        protected DateTime _Timestamp;
        // 記事本文（property）
        protected String _Text;
        // リダイレクト先記事名（property）
        protected String _Redirect;
        // GetArticle実行時のHttpStatus（property）
        protected HttpStatusCode _GetArticleStatus;
        // GetArticle例外発生時の例外情報（property）
        protected Exception _GetArticleException;
    }
}
