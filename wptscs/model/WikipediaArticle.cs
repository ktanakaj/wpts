using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Windows.Forms;

namespace wptscs.model
{
    // Wikipedia�̋L�����Ǘ����邽�߂̃N���X
    public class WikipediaArticle : WikipediaFormat
    {
		// �R���X�g���N�^�i�T�[�o�[�ƋL�������w��j
		public WikipediaArticle(WikipediaInformation i_Server, String i_Name)
			: base(i_Server){
			// �����ݒ�
			Initialize(i_Name);
		}

		/* �����ݒ� */
        public void Initialize(String i_Title)
        {
	        // ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
	        if(MYAPP.Cmn.NullCheckAndTrim(i_Title).TrimStart(':') == ""){
		        throw new ArgumentNullException("i_Title");
	        }
	        // �����o�ϐ��̏�����
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

        /* �L���̏ڍ׏����擾 */
        public virtual bool GetArticle(String i_UserAgent, String i_Referer, TimeSpan i_CacheEnabledSpan)
        {
	        // �������ƒl�`�F�b�N
	        _Xml = null;
	        _Timestamp = DateTime.MinValue;
	        _Text = "";
	        _Redirect = "";
	        _GetArticleStatus = HttpStatusCode.PaymentRequired;
	        _GetArticleException = null;
	        // �L���̃f�[�^���L���b�V����Wikipedia�T�[�o�[����擾���AXML�Ɋi�[
	        if(getCacheArticle(i_CacheEnabledSpan) == false){
		        if(getServerArticle(i_UserAgent, i_Referer) == false){
			        return false;
		        }
	        }
	        // �擾���ꂽXML����͂��A�����o�ϐ��ɐݒ�
	        // ���O��ԏ��̏㏑��
	        _Server.Namespaces = GetNamespaces();
	        // �L�����̐ݒ�
	        XmlNamespaceManager nsMgr = new XmlNamespaceManager(Xml.NameTable);
	        nsMgr.AddNamespace("ns", XMLNS);
	        XmlElement pageElement = (XmlElement) Xml.SelectSingleNode("/ns:mediawiki/ns:page", nsMgr);
	        if(pageElement != null){
		        // �L�����̏㏑��
		        XmlElement titleElement = (XmlElement) pageElement.SelectSingleNode("ns:title", nsMgr);
		        _Title = (!String.IsNullOrEmpty(titleElement.InnerText) ? titleElement.InnerText : Title);
		        // �ŏI�X�V����
		        XmlElement timeElement = (XmlElement) pageElement.SelectSingleNode("ns:revision/ns:timestamp", nsMgr);
		        _Timestamp = DateTime.Parse(timeElement.InnerText);
		        // �L���{��
		        XmlElement textElement = (XmlElement) pageElement.SelectSingleNode("ns:revision/ns:text", nsMgr);
		        _Text = textElement.InnerText;
		        // ���_�C���N�g�̃`�F�b�N���s���Ă���
		        IsRedirect();
	        }
	        // �L�������݂��Ȃ��ꍇ�AXML�͎擾�ł��邪page�m�[�h�������̂ŁA404�G���[�Ɠ��l�Ɉ���
	        else{
		        _GetArticleStatus = HttpStatusCode.NotFound;
		        return false;
	        }
	        return true;
        }

        /* �L���̏ڍ׏����擾�i�L���b�V���L�����Ԃ̓f�t�H���g�j */
        public bool GetArticle(String i_UserAgent, String i_Referer)
        {
	        // �L���b�V���L������1�T�Ԃ�GetArticle�����s
	        // ���L���̗L���▼�́A���_�C���N�g�A����ԃ����N���͂���ȂɍX�V����Ȃ����낤
	        //   �E�E�E�Ƃ������ƂŁA���̊��ԂɁB
	        //   �K�v�ł���΁A�L���b�V�����g��Ȃ��ݒ�Ŗ{���\�b�h�𒼐ڌĂԂ���
            return GetArticle(i_UserAgent, i_Referer, new TimeSpan(7, 0, 0, 0));
        }

        /* �L���̏ڍ׏����擾�iUserAgent, Referer, �L���b�V���L�����Ԃ̓f�t�H���g�j */
        public bool GetArticle()
        {
	        // ����l��GetArticle�����s
	        return GetArticle("", "");
        }

        /* �L����XML���T�[�o�[���擾 */
        protected bool getServerArticle(String i_UserAgent, String i_Referer)
        {
	        // �������ƒl�`�F�b�N
	        _Xml = null;
	        _GetArticleStatus = HttpStatusCode.PaymentRequired;
	        _GetArticleException = null;
	        // �L����XML�f�[�^��Wikipedia�T�[�o�[����擾
	        try{
		        HttpWebRequest req = (HttpWebRequest) WebRequest.Create(Url);
		        // UserAgent�ݒ�
		        // ��Wikipedia��UserAgent����̏ꍇ�G���[�ƂȂ�̂ŁA�K���ݒ肷��
		        if(!String.IsNullOrEmpty(i_UserAgent)){
			        req.UserAgent = i_UserAgent;
		        }
		        else{
			        Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			        req.UserAgent = "WikipediaTranslationSupportTool/" + ver.Major + "." + String.Format("{0:D2}",ver.Minor);
		        }
		        // Referer�ݒ�
		        if(!String.IsNullOrEmpty(i_Referer)){
			        req.Referer = i_Referer;
		        }
		        HttpWebResponse res = (HttpWebResponse) req.GetResponse();
		        _GetArticleStatus = res.StatusCode;

		        // �����f�[�^����M���邽�߂�Stream���擾���A�f�[�^���擾
		        // ���擾����XML�����킩�́A�����ł͊m�F���Ȃ�
		        _Xml = new XmlDocument();
		        _Xml.Load(res.GetResponseStream());
		        res.Close();

		        // �擾����XML���ꎞ�t�H���_�ɕۑ�
		        try{
			        // �ꎞ�t�H���_���m�F
			        String tmpDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Application.ExecutablePath));
			        if(Directory.Exists(tmpDir) == false){
				        // �ꎞ�t�H���_���쐬
				        Directory.CreateDirectory(tmpDir);
			        }
			        // �t�@�C���̕ۑ�
			        Xml.Save(Path.Combine(tmpDir, MYAPP.Cmn.ReplaceInvalidFileNameChars(Title) + ".xml"));
		        }
		        catch(Exception e){
			        System.Diagnostics.Debug.WriteLine("WikipediaArticle.getServerArticle > �ꎞ�t�@�C���̕ۑ��Ɏ��s���܂��� : " + e.Message);
		        }
	        }
	        catch(WebException e){
		        // ProtocolError�G���[�̏ꍇ�A�X�e�[�^�X�R�[�h��ێ�
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

        /* �L����XML���L���b�V�����擾 */
        protected bool getCacheArticle(TimeSpan i_CacheEnabledSpan)
        {
	        // �������ƒl�`�F�b�N
	        _Xml = null;
	        _GetArticleStatus = HttpStatusCode.PaymentRequired;
	        _GetArticleException = null;
	        // �L���b�V�����g�p����ꍇ�̂�
	        if(i_CacheEnabledSpan > new TimeSpan(0)){
		        // �L����XML�f�[�^���L���b�V���t�@�C������擾
		        try{
			        // �ꎞ�t�@�C���ɃA�N�Z�X
			        String tmpFile = Path.Combine(Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Application.ExecutablePath)), MYAPP.Cmn.ReplaceInvalidFileNameChars(Title) + ".xml");
			        if(File.Exists(tmpFile) == true){
				        // �t�@�C�����L���������̂��̂����m�F
				        if((DateTime.UtcNow - File.GetLastWriteTimeUtc(tmpFile)) < i_CacheEnabledSpan){
					        // �t�@�C����Stream�ŊJ���A�f�[�^���擾
					        XmlDocument tmpXml = new XmlDocument();
					        FileStream fs = File.OpenRead(tmpFile);
					        try{
						        tmpXml.Load(fs);
					        }
					        finally{
						        fs.Close();
					        }
					        // �擾����XML�t�@�C�����A�ړI�Ƃ���L���̂��̂����m�F
					        XmlNamespaceManager nsMgr = new XmlNamespaceManager(tmpXml.NameTable);
					        nsMgr.AddNamespace("ns", XMLNS);
					        XmlElement rootElement = tmpXml.DocumentElement;
					        XmlElement pageElement = (XmlElement) tmpXml.SelectSingleNode("/ns:mediawiki/ns:page/ns:title", nsMgr);
					        if(pageElement != null){
						        // ����R�[�h�E�L�������`�F�b�N�B�啶���E���������قȂ�ꍇ�A�ʂ̋L���Ɣ��ʂ���
						        // ��Low Earth orbit�ւ̃��_�C���N�g��Low earth orbit�݂����Ȃ̂����邽��
						        //   �������擪��Wikipedia�̋Z�p�I�����ŏ�ɑ啶���Ȃ��߁A�啶���ŏ�������
						        String title = char.ToUpper(Title[0]).ToString();
						        if(Title.Length > 1){
							        title += Title.Substring(1);
						        }
						        if(rootElement.GetAttribute("xml:lang") == Server.Code &&
						           pageElement.InnerText == title){
							        // XML�������o�ϐ��ɐݒ肵�A����I��
							        System.Diagnostics.Debug.WriteLine("WikipediaArticle.getCacheArticle > �L���b�V���Ǎ��� : " + MYAPP.Cmn.ReplaceInvalidFileNameChars(Title) + ".xml");
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

        /* �w�肳�ꂽ����R�[�h�ւ̌���ԃ����N��Ԃ� */
        public virtual String GetInterWiki(String i_Code)
        {
	        // �������ƒl�`�F�b�N
	        String interWiki = "";
	        if(Text == ""){
		        // GetArticle���s���Ă��Ȃ��ꍇ�AInvalidOperationException��Ԃ�
		        throw new InvalidOperationException();
	        }
	        // �L���ɑ��݂���w�茾��ւ̌���ԃ����N���擾
	        for(int i = 0 ; i < Text.Length ; i++){
		        // �R�����g�i<!--�j�̃`�F�b�N
		        String comment = "";
		        int index = chkComment(ref comment, i);
		        if(index != -1){
			        i = index;
		        }
		        // �w�茾��ւ̌���ԃ����N�̏ꍇ�A���e���擾���A�����I��
		        else if(MYAPP.Cmn.ChkTextInnerWith(Text, i, "[[" + i_Code + ":") == true){
			        Link link = ParseInnerLink(Text.Substring(i));
			        if(!String.IsNullOrEmpty(link.Text)){
				        interWiki = link.Article;
				        break;
			        }
		        }
	        }
	        return interWiki;
        }

        /* �L����XML���疼�O��ԏ����擾 */
        public virtual WikipediaInformation.Namespace[] GetNamespaces()
        {
	        // XML���疼�O��ԏ����擾
	        WikipediaInformation.Namespace[] namespaces = new WikipediaInformation.Namespace[0];
	        if(Xml == null){
		        // GetArticle���s���Ă��Ȃ��ꍇ�AInvalidOperationException��Ԃ�
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
				        MYAPP.Cmn.AddArray(ref namespaces, ns);
			        }
                    catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine("WikipediaArticle.GetNamespaces > ��O���� : " + ex);
                    }
		        }
	        }
	        return namespaces;
        }

        /* �L�������_�C���N�g�����`�F�b�N */
        public virtual bool IsRedirect()
        {
	        // �l�`�F�b�N
	        if(Text == ""){
		        // GetArticle���s���Ă��Ȃ��ꍇ�AInvalidOperationException��Ԃ�
		        throw new InvalidOperationException();
	        }
	        // �w�肳�ꂽ�L�������_�C���N�g�L���i#REDIRECT���j�����`�F�b�N
	        // �����{��ł݂����ɁA#REDIRECT�ƌ���ŗL��#�]���݂����Ȃ̂�����Ǝv����̂ŁA
	        //   �|�󌳌���Ɖp��ł̐ݒ�Ń`�F�b�N
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

        /* �L�����J�e�S���[�����`�F�b�N */
        public virtual bool IsCategory()
        {
	        return IsCategory(Title);
        }

        /* �L�����摜�����`�F�b�N */
        public virtual bool IsImage()
        {
	        return IsImage(Title);
        }

        /* �L�����W�����O��ԈȊO�����`�F�b�N */
        public bool IsNotMainNamespace()
        {
	        return IsNotMainNamespace(Title);
        }

        /* �n���ꂽ���������N�E�e���v���[�g����� */
        protected virtual int chkLinkText(ref WikipediaFormat.Link o_Link, int i_Index)
        {
	        return ChkLinkText(ref o_Link, Text, i_Index);
        }

        /* �L���{���̎w�肳�ꂽ�ʒu�ɑ��݂�����������N�E�e���v���[�g����� */
        protected virtual int chkComment(ref String o_Text, int i_Index)
        {
	        return ChkComment(ref o_Text, Text, i_Index);
        }

		// �L����
		public String Title {
			get {
				return _Title;
			}
		}
		// �L����XML�f�[�^��URL�iproperty�j
		public Uri Url {
			get {
				return _Url;
			}
		}
		// �L����XML�f�[�^
		public XmlDocument Xml {
			get {
				return _Xml;
			}
		}
		// �L���̍ŏI�X�V�����iUTC�j
		public DateTime Timestamp {
			get {
				return _Timestamp;
			}
		}
		// �L���{��
		public String Text {
			get {
				return _Text;
			}
		}
		// ���_�C���N�g��L����
		public String Redirect {
			get {
				return _Redirect;
			}
		}
		// GetArticle���s����HttpStatus
		public HttpStatusCode GetArticleStatus {
			get {
				return _GetArticleStatus;
			}
		}
		// GetArticle��O�������̗�O���
		// ��GetArticle()��false�ŁAGetArticleStatus��NotFound�ȊO�̂Ƃ��A�ݒ肳���
		public Exception GetArticleException {
			get {
				return _GetArticleException;
			}
		}

		// Wikipedia��XML�̌Œ�l�̏���
		public static readonly String XMLNS = "http://www.mediawiki.org/xml/export-0.4/";

		// �L�����iproperty�j
		protected String _Title;
		// �L����XML�f�[�^��URL�iproperty�j
		protected Uri _Url;
		// �L����XML�f�[�^�iproperty�j
		protected XmlDocument _Xml;
		// �L���̍ŏI�X�V�����iUTC�j�iproperty�j
		protected DateTime _Timestamp;
		// �L���{���iproperty�j
		protected String _Text;
		// ���_�C���N�g��L�����iproperty�j
		protected String _Redirect;
		// GetArticle���s����HttpStatus�iproperty�j
		protected HttpStatusCode _GetArticleStatus;
		// GetArticle��O�������̗�O���iproperty�j
		protected Exception _GetArticleException;
    }
}
