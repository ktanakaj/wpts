// Wikipedia�̋L�����Ǘ����邽�߂̃N���X
#include "stdafx.h"
#include "WikipediaArticle.h"

using namespace wpts;

// WikipediaArticle::Link

/* ������ */
void WikipediaFormat::Link::Initialize(void)
{
	// �R���X�g���N�^�̑���ɁA�K�v�Ȃ炱��ŏ�����
	Text = nullptr;
	Article = nullptr;
	Section = nullptr;
	PipeTexts = gcnew array<String^>(0);
	Code = nullptr;
	TemplateFlag = false;
	SubPageFlag = false;
	StartColonFlag = false;
	MsgnwFlag = false;
	EnterFlag = false;
}

/* ���݂�Text�ȊO�̒l����AText�𐶐� */
String^ WikipediaFormat::Link::MakeText(void)
{
	// �߂�l������
	String ^result = "";
	// �g�̐ݒ�
	String ^startSign = "[[";
	String ^endSign = "]]";
	if(TemplateFlag){
		startSign = "{{";
		endSign = "}}";
	}
	// �擪�̘g�̕t��
	result += startSign;
	// �擪�� : �̕t��
	if(StartColonFlag){
		result += ":";
	}
	// msgnw: �i�e���v���[�g��<nowiki>�^�O�ŋ��ށj�̕t��
	if(TemplateFlag && MsgnwFlag){
		result += const_cast<String^>(MSGNW);
	}
	// ����R�[�h�E���v���W�F�N�g�R�[�h�̕t��
	if(!String::IsNullOrEmpty(Code)){
		result += Code;
	}
	// �����N�̕t��
	if(!String::IsNullOrEmpty(Article)){
		result += Article;
	}
	// �Z�N�V�������̕t��
	if(!String::IsNullOrEmpty(Section)){
		result += ("#" + Section);
	}
	// ���s�̕t��
	if(EnterFlag){
		result += '\n';
	}
	// �p�C�v��̕�����̕t��
	if(PipeTexts != nullptr){
		for each(String ^text in PipeTexts){
			result += "|";
			if(!String::IsNullOrEmpty(text)){
				result += text;
			}
		}
	}
	// �I���̘g�̕t��
	result += endSign;
	return result;
}


// WikipediaFormat

/* �R���X�g���N�^�i�T�[�o�[���w��j */
WikipediaFormat::WikipediaFormat(WikipediaInformation ^i_Server)
{
	// ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
	if(i_Server == nullptr){
		throw gcnew ArgumentNullException("i_Server");
	}
	// �����o�ϐ��̏�����
	_Server = i_Server;
}

/* �n���ꂽ�L�������J�e�S���[�����`�F�b�N */
bool WikipediaFormat::IsCategory(String ^i_Name)
{
	// �w�肳�ꂽ�L�������J�e�S���[�iCategory:���Ŏn�܂�j�����`�F�b�N
	String ^category = Server->GetNamespace(Server->CATEGORYNAMESPACENUMBER);
	if(category != ""){
		if(i_Name->ToLower()->StartsWith(category->ToLower() + ":") == true){
			return true;
		}
	}
	return false;
}

/* �n���ꂽ�L�������摜�����`�F�b�N */
bool WikipediaFormat::IsImage(String ^i_Name)
{
	// �w�肳�ꂽ�L�������摜�iImage:���Ŏn�܂�j�����`�F�b�N
	// �����{��ł݂����ɁAimage: �ƌ���ŗL�� �摜: �݂����Ȃ̂�����Ǝv����̂ŁA
	//   �|�󌳌���Ɖp��ł̐ݒ�Ń`�F�b�N
	for(int i = 0 ; i < 2 ; i++){
		String ^image = Server->GetNamespace(Server->IMAGENAMESPACENUMBER);
		if(i == 1){
			if(Server->Code == "en"){
				continue;
			}
			WikipediaInformation en("en");
			image = en.GetNamespace(en.IMAGENAMESPACENUMBER);
		}
		if(image != ""){
			if(i_Name->ToLower()->StartsWith(image->ToLower() + ":") == true){
				return true;
			}
		}
	}
	return false;
}

/* �n���ꂽ�L�������W�����O��ԈȊO�����`�F�b�N */
bool WikipediaFormat::IsNotMainNamespace(String ^i_Name)
{
	// �w�肳�ꂽ�L�������W�����O��ԈȊO�̖��O��ԁiWikipedia:���Ŏn�܂�j�����`�F�b�N
	for each(WikipediaInformation::Namespace ns in Server->Namespaces){
		if(i_Name->ToLower()->StartsWith(ns.Name->ToLower() + ":") == true){
			return true;
		}
	}
	return false;
}

/* �n���ꂽWikipedia�̓��������N����� */
WikipediaFormat::Link WikipediaFormat::ParseInnerLink(String ^i_Text)
{
	// �o�͒l������
	Link result;
	result.Initialize();
	// ���͒l�m�F
	if(i_Text->StartsWith("[[") == false){
		return result;
	}

	// �\������͂��āA[[]]�����̕�������擾
	// ���\����Wikipedia�̃v���r���[�ŐF�X�����Ċm�F�A����Ȃ�������Ԉ���Ă��肷�邩���E�E�E
	String ^article = "";
	String ^section = "";
	array<String^> ^pipeTexts = gcnew array<String^>(0);
	int lastIndex = -1;
	int pipeCounter = 0;
	bool sharpFlag = false;
	for(int i = 2 ; i < i_Text->Length ; i++){
		wchar_t c = i_Text[i];
		// ]]������������A��������I��
		if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, "]]") == true){
			lastIndex = ++i;
			break;
		}
		// | ���܂܂�Ă���ꍇ�A�ȍ~�̕�����͕\�����ȂǂƂ��Ĉ���
		if(c == '|'){
			++pipeCounter;
			MYAPP::Cmn::AddArray(pipeTexts, "");
			continue;
		}
		// �ϐ��i[[{{{1}}}]]�Ƃ��j�̍ċA�`�F�b�N
		String ^dummy = "";
		String ^variable = "";
		int index = ChkVariable(variable, dummy, i_Text, i);
		if(index != -1){
			i = index;
			if(pipeCounter > 0){
				pipeTexts[pipeCounter - 1] += variable;
			}
			else if(sharpFlag){
				section += variable;
			}
			else{
				article += variable;
			}
			continue;
		}

		// | �̑O�̂Ƃ�
		if(pipeCounter <= 0){
			// �ϐ��ȊO�� { } �܂��� < > [ ] \n ���܂܂�Ă���ꍇ�A�����N�͖���
			if((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}') || (c == '\n')){
				break;
			}

			// # �̑O�̂Ƃ�
			if(!sharpFlag){
				// #���܂܂�Ă���ꍇ�A�ȍ~�̕�����͌��o���ւ̃����N�Ƃ��Ĉ����i1�߂�#�̂ݗL���j
				if(c == '#'){
					sharpFlag = true;
				}
				else{
					article += c;
				}
			}
			// # �̌�̂Ƃ�
			else{
				section += c;
			}
		}
		// | �̌�̂Ƃ�
		else{
			// �R�����g�i<!--�j���܂܂�Ă���ꍇ�A�����N�͖���
			if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, const_cast<String^>(COMMENTSTART)) == true){
				break;
			}
			// nowiki�̃`�F�b�N
			String ^nowiki = "";
			index = ChkNowiki(nowiki, i_Text, i);
			if(index != -1){
				i = index;
				pipeTexts[pipeCounter - 1] += nowiki;
				continue;
			}
			// �����N [[ {{ �i[[image:xx|[[test]]�̉摜]]�Ƃ��j�̍ċA�`�F�b�N
			Link link;
			index = ChkLinkText(link, i_Text, i);
			if(index != -1){
				i = index;
				pipeTexts[pipeCounter - 1] += link.Text;
				continue;
			}
			pipeTexts[pipeCounter - 1] += c;
		}
	}
	// ��͂ɐ��������ꍇ�A���ʂ�߂�l�ɐݒ�
	if(lastIndex != -1){
		// �ϐ��u���b�N�̕�����������N�̃e�L�X�g�ɐݒ�
		result.Text = i_Text->Substring(0, lastIndex + 1);
		// �O��̃X�y�[�X�͍폜�i���o���͌��̂݁j
		result.Article = article->Trim();
		result.Section = section->TrimEnd();
		// | �ȍ~�͂��̂܂ܐݒ�
		result.PipeTexts = pipeTexts;
		// �L����������𒊏o
		// �T�u�y�[�W
		if(result.Article->StartsWith("/") == true){
			result.SubPageFlag = true;
		}
		// �擪�� :
		else if(result.Article->StartsWith(":")){
			result.StartColonFlag = true;
			result.Article = result.Article->TrimStart(':')->TrimStart();
		}
		// �W�����O��ԈȊO��[[xxx:yyy]]�̂悤�ɂȂ��Ă���ꍇ�A����R�[�h
		if(result.Article->Contains(":") == true && !IsNotMainNamespace(result.Article)){
			// ���{���́A����R�[�h���̈ꗗ�����A�����ƈ�v������̂��E�E�E�Ƃ��ׂ����낤���A
			//   �����e������Ȃ��̂� : ���܂ޖ��O��ԈȊO��S�Č���R�[�h���Ɣ���
			result.Code = result.Article->Substring(0, result.Article->IndexOf(':'))->TrimEnd();
			result.Article = result.Article->Substring(result.Article->IndexOf(':') + 1)->TrimStart();
		}
	}
	return result;
}

/* �n���ꂽWikipedia�̃e���v���[�g����� */
WikipediaFormat::Link WikipediaFormat::ParseTemplate(String ^i_Text)
{
	// �o�͒l������
	Link result;
	result.Initialize();
	result.TemplateFlag = true;
	// ���͒l�m�F
	if(i_Text->StartsWith("{{") == false){
		return result;
	}

	// �\������͂��āA{{}}�����̕�������擾
	// ���\����Wikipedia�̃v���r���[�ŐF�X�����Ċm�F�A����Ȃ�������Ԉ���Ă��肷�邩���E�E�E
	String ^article = "";
	array<String^> ^pipeTexts = gcnew array<String^>(0);
	int lastIndex = -1;
	int pipeCounter = 0;
	for(int i = 2 ; i < i_Text->Length ; i++){
		wchar_t c = i_Text[i];
		// }}������������A��������I��
		if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, "}}") == true){
			lastIndex = ++i;
			break;
		}
		// | ���܂܂�Ă���ꍇ�A�ȍ~�̕�����͈����ȂǂƂ��Ĉ���
		if(c == '|'){
			++pipeCounter;
			MYAPP::Cmn::AddArray(pipeTexts, "");
			continue;
		}
		// �ϐ��i[[{{{1}}}]]�Ƃ��j�̍ċA�`�F�b�N
		String ^dummy = "";
		String ^variable = "";
		int index = ChkVariable(variable, dummy, i_Text, i);
		if(index != -1){
			i = index;
			if(pipeCounter > 0){
				pipeTexts[pipeCounter - 1] += variable;
			}
			else{
				article += variable;
			}
			continue;
		}

		// | �̑O�̂Ƃ�
		if(pipeCounter <= 0){
			// �ϐ��ȊO�� < > [ ] { } ���܂܂�Ă���ꍇ�A�����N�͖���
			if((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}')){
				break;
			}
			article += c;
		}
		// | �̌�̂Ƃ�
		else{
			// �R�����g�i<!--�j���܂܂�Ă���ꍇ�A�����N�͖���
			if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, const_cast<String^>(COMMENTSTART)) == true){
				break;
			}
			// nowiki�̃`�F�b�N
			String ^nowiki = "";
			index = ChkNowiki(nowiki, i_Text, i);
			if(index != -1){
				i = index;
				pipeTexts[pipeCounter - 1] += nowiki;
				continue;
			}
			// �����N [[ {{ �i{{test|[[��]]}}�Ƃ��j�̍ċA�`�F�b�N
			Link link;
			index = ChkLinkText(link, i_Text, i);
			if(index != -1){
				i = index;
				pipeTexts[pipeCounter - 1] += link.Text;
				continue;
			}
			pipeTexts[pipeCounter - 1] += c;
		}
	}
	// ��͂ɐ��������ꍇ�A���ʂ�߂�l�ɐݒ�
	if(lastIndex != -1){
		// �ϐ��u���b�N�̕�����������N�̃e�L�X�g�ɐݒ�
		result.Text = i_Text->Substring(0, lastIndex + 1);
		// �O��̃X�y�[�X�E���s�͍폜�i���o���͌��̂݁j
		result.Article = article->Trim();
		// | �ȍ~�͂��̂܂ܐݒ�
		result.PipeTexts = pipeTexts;
		// �L����������𒊏o
		// �T�u�y�[�W
		if(result.Article->StartsWith("/") == true){
			result.SubPageFlag = true;
		}
		// �擪�� :
		else if(result.Article->StartsWith(":")){
			result.StartColonFlag = true;
			result.Article = result.Article->TrimStart(':')->TrimStart();
		}
		// �擪�� msgnw:
		result.MsgnwFlag = result.Article->ToLower()->StartsWith(const_cast<String^>(MSGNW)->ToLower());
		if(result.MsgnwFlag){
			result.Article = result.Article->Substring(const_cast<String^>(MSGNW)->Length);
		}
		// �L��������̉��s�̗L��
		if(article->TrimEnd(' ')->EndsWith("\n")){
			result.EnterFlag = true;
		}
	}
	return result;
}

/* �n���ꂽ�e�L�X�g�̎w�肳�ꂽ�ʒu�ɑ��݂���Wikipedia�̓��������N�E�e���v���[�g���`�F�b�N */
// �����펞�̖߂�l�ɂ́A]]�̌���]�̈ʒu�̃C���f�b�N�X��Ԃ��B�ُ펞��-1
int WikipediaFormat::ChkLinkText(Link %o_Link, String ^i_Text, int i_Index)
{
	// �o�͒l������
	int lastIndex = -1;
	o_Link.Initialize();
	// ���͒l�ɉ����āA������U�蕪��
	if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i_Index, "[[") == true){
		// ���������N
		o_Link = ParseInnerLink(i_Text->Substring(i_Index));
	}
	else if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i_Index, "{{") == true){
		// �e���v���[�g
		o_Link = ParseTemplate(i_Text->Substring(i_Index));
	}
	// �������ʊm�F
	if(!String::IsNullOrEmpty(o_Link.Text)){
		lastIndex = i_Index + o_Link.Text->Length - 1;
	}
	return lastIndex;
}

/* �n���ꂽ�e�L�X�g�̎w�肳�ꂽ�ʒu�ɑ��݂���ϐ������ */
int WikipediaFormat::ChkVariable(String ^%o_Variable, String ^%o_Value, String ^i_Text, int i_Index)
{
	// �o�͒l������
	int lastIndex = -1;
	o_Variable = "";
	o_Value = "";
	// ���͒l�m�F
	if(MYAPP::Cmn::ChkTextInnerWith(i_Text->ToLower(), i_Index, "{{{") == false){
		return lastIndex;
	}
	// �u���b�N�I���܂Ŏ擾
	bool pipeFlag = false;
	for(int i = i_Index + 3; i < i_Text->Length ; i++){
		// �I�������̃`�F�b�N
		if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, "}}}") == true){
			lastIndex = i + 2;
			break;
		}
		// �R�����g�i<!--�j�̃`�F�b�N
		String ^dummy = "";
		int index = WikipediaArticle::ChkComment(dummy, i_Text, i);
		if(index != -1){
			i = index;
			continue;
		}
		// | ���܂܂�Ă���ꍇ�A�ȍ~�̕�����͑�����ꂽ�l�Ƃ��Ĉ���
		if(i_Text[i] == '|'){
			pipeFlag = true;
		}
		// | �̑O�̂Ƃ�
		else if(!pipeFlag){
			// ��Wikipedia�̎d�l��́A{{{1{|�\��}}} �̂悤�ɕϐ����̗��� { ��
			//   �܂߂邱�Ƃ��ł���悤�����A���ʂ�����Ȃ��̂ŁA�G���[�Ƃ���
			//   �i�ǂ����Ӑ}���Ă���Ȃ��Ƃ���l�͋��Ȃ����낤���E�E�E�j
			if(i_Text[i] == '{'){
				break;
			}
		}
		// | �̌�̂Ƃ�
		else{
			// nowiki�̃`�F�b�N
			String ^nowiki = "";
			index = ChkNowiki(nowiki, i_Text, i);
			if(index != -1){
				i = index;
				o_Value += nowiki;
				continue;
			}
			// �ϐ��i{{{1|{{{2}}}}}}�Ƃ��j�̍ċA�`�F�b�N
			String ^variable = "";
			index = ChkVariable(variable, dummy, i_Text, i);
			if(index != -1){
				i = index;
				o_Value += variable;
				continue;
			}
			// �����N [[ {{ �i{{{1|[[test]]}}}�Ƃ��j�̍ċA�`�F�b�N
			Link link;
			index = ChkLinkText(link, i_Text, i);
			if(index != -1){
				i = index;
				o_Value += link.Text;
				continue;
			}
			o_Value += i_Text[i];
		}
	}
	// �ϐ��u���b�N�̕�������o�͒l�ɐݒ�
	if(lastIndex != -1){
		o_Variable = i_Text->Substring(i_Index, lastIndex - i_Index + 1);
	}
	// ����ȍ\���ł͂Ȃ������ꍇ�A�o�͒l���N���A
	else{
		o_Variable = "";
		o_Value = "";
	}
	return lastIndex;
}

/* nowiki��Ԃ̃`�F�b�N */
int WikipediaFormat::ChkNowiki(String ^%o_Text, String ^i_Text, int i_Index)
{
	// �o�͒l������
	int lastIndex = -1;
	o_Text = "";
	// ���͒l�m�F
	if(MYAPP::Cmn::ChkTextInnerWith(i_Text->ToLower(), i_Index, const_cast<String^>(NOWIKISTART)->ToLower()) == false){
		return lastIndex;
	}
	// �u���b�N�I���܂Ŏ擾
	for(int i = i_Index + const_cast<String^>(NOWIKISTART)->Length; i < i_Text->Length ; i++){
		// �I�������̃`�F�b�N
		if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, const_cast<String^>(NOWIKIEND)) == true){
			lastIndex = i + const_cast<String^>(NOWIKIEND)->Length - 1;
			break;
		}
		// �R�����g�i<!--�j�̃`�F�b�N
		String ^dummy = "";
		int index = WikipediaArticle::ChkComment(dummy, i_Text, i);
		if(index != -1){
			i = index;
			continue;
		}
	}
	// �I��肪������Ȃ��ꍇ�́A�S��nowiki�u���b�N�Ɣ��f
	if(lastIndex == -1){
		lastIndex = i_Text->Length - 1;
	}
	o_Text = i_Text->Substring(i_Index, lastIndex - i_Index + 1);
	return lastIndex;
}

/* �R�����g��Ԃ̃`�F�b�N */
int WikipediaFormat::ChkComment(String ^%o_Text, String ^i_Text, int i_Index)
{
	// �o�͒l������
	int lastIndex = -1;
	o_Text = "";
	// ���͒l�m�F
	if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i_Index, const_cast<String^>(COMMENTSTART)) == false){
		return lastIndex;
	}
	// �R�����g�I���܂Ŏ擾
	for(int i = i_Index + const_cast<String^>(COMMENTSTART)->Length; i < i_Text->Length ; i++){
		if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i, const_cast<String^>(COMMENTEND)) == true){
			lastIndex = i + const_cast<String^>(COMMENTEND)->Length - 1;
			break;
		}
	}
	// �I��肪������Ȃ��ꍇ�́A�S�ăR�����g�Ɣ��f
	if(lastIndex == -1){
		lastIndex = i_Text->Length - 1;
	}
	o_Text = i_Text->Substring(i_Index, lastIndex - i_Index + 1);
	return lastIndex;
}


// WikipediaArticle

/* �����ݒ� */
void WikipediaArticle::Initialize(String ^i_Title)
{
	// ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
	if(MYAPP::Cmn::NullCheckAndTrim(i_Title)->TrimStart(':') == ""){
		throw gcnew ArgumentNullException("i_Title");
	}
	// �����o�ϐ��̏�����
	_Title = i_Title->Trim()->TrimStart(':');
	UriBuilder ^uri = gcnew UriBuilder("http", Server->Server);
	uri->Path = (Server->ArticleXmlPath + Title);
	_Url = uri->Uri;
	_Xml = nullptr;
	_Timestamp = DateTime::MinValue;
	_Text = "";
	_Redirect = "";
	_GetArticleStatus = HttpStatusCode::PaymentRequired;
	_GetArticleException = nullptr;
}

/* �L���̏ڍ׏����擾 */
bool WikipediaArticle::GetArticle(String ^i_UserAgent, String ^i_Referer, TimeSpan i_CacheEnabledSpan)
{
	// �������ƒl�`�F�b�N
	_Xml = nullptr;
	_Timestamp = DateTime::MinValue;
	_Text = "";
	_Redirect = "";
	_GetArticleStatus = HttpStatusCode::PaymentRequired;
	_GetArticleException = nullptr;
	// �L���̃f�[�^���L���b�V����Wikipedia�T�[�o�[����擾���AXML�Ɋi�[
	if(getCacheArticle(i_CacheEnabledSpan) == false){
		if(getServerArticle(i_UserAgent, i_Referer) == false){
			return false;
		}
	}
	// �擾���ꂽXML����͂��A�����o�ϐ��ɐݒ�
	// ���O��ԏ��̏㏑��
	_Server->Namespaces = GetNamespaces();
	// �L�����̐ݒ�
	XmlNamespaceManager ^nsMgr = gcnew XmlNamespaceManager(Xml->NameTable);
	nsMgr->AddNamespace("ns", const_cast<String^>(XMLNS));
	XmlElement ^pageElement = safe_cast<XmlElement^>(Xml->SelectSingleNode("/ns:mediawiki/ns:page", nsMgr));
	if(pageElement != nullptr){
		// �L�����̏㏑��
		XmlElement ^titleElement = safe_cast<XmlElement^>(pageElement->SelectSingleNode("ns:title", nsMgr));
		_Title = (!String::IsNullOrEmpty(titleElement->InnerText) ? titleElement->InnerText : Title);
		// �ŏI�X�V����
		XmlElement ^timeElement = safe_cast<XmlElement^>(pageElement->SelectSingleNode("ns:revision/ns:timestamp", nsMgr));
		_Timestamp = DateTime::Parse(timeElement->InnerText);
		// �L���{��
		XmlElement ^textElement = safe_cast<XmlElement^>(pageElement->SelectSingleNode("ns:revision/ns:text", nsMgr));
		_Text = textElement->InnerText;
		// ���_�C���N�g�̃`�F�b�N���s���Ă���
		IsRedirect();
	}
	// �L�������݂��Ȃ��ꍇ�AXML�͎擾�ł��邪page�m�[�h�������̂ŁA404�G���[�Ɠ��l�Ɉ���
	else{
		_GetArticleStatus = HttpStatusCode::NotFound;
		return false;
	}
	return true;
}

/* �L���̏ڍ׏����擾�i�L���b�V���L�����Ԃ̓f�t�H���g�j */
bool WikipediaArticle::GetArticle(String ^i_UserAgent, String ^i_Referer)
{
	// �L���b�V���L������1�T�Ԃ�GetArticle�����s
	// ���L���̗L���▼�́A���_�C���N�g�A����ԃ����N���͂���ȂɍX�V����Ȃ����낤
	//   �E�E�E�Ƃ������ƂŁA���̊��ԂɁB
	//   �K�v�ł���΁A�L���b�V�����g��Ȃ��ݒ�Ŗ{���\�b�h�𒼐ڌĂԂ���
	return GetArticle(i_UserAgent, i_Referer, TimeSpan(7, 0, 0, 0));
}

/* �L���̏ڍ׏����擾�iUserAgent, Referer, �L���b�V���L�����Ԃ̓f�t�H���g�j */
bool WikipediaArticle::GetArticle(void)
{
	// ����l��GetArticle�����s
	return GetArticle("", "");
}

/* �L����XML���T�[�o�[���擾 */
bool WikipediaArticle::getServerArticle(String ^i_UserAgent, String ^i_Referer)
{
	// �������ƒl�`�F�b�N
	_Xml = nullptr;
	_GetArticleStatus = HttpStatusCode::PaymentRequired;
	_GetArticleException = nullptr;
	// �L����XML�f�[�^��Wikipedia�T�[�o�[����擾
	try{
		HttpWebRequest ^req = safe_cast<HttpWebRequest^>(WebRequest::Create(Url));
		// UserAgent�ݒ�
		// ��Wikipedia��UserAgent����̏ꍇ�G���[�ƂȂ�̂ŁA�K���ݒ肷��
		if(!String::IsNullOrEmpty(i_UserAgent)){
			req->UserAgent = i_UserAgent;
		}
		else{
			Version ^ver = System::Reflection::Assembly::GetExecutingAssembly()->GetName()->Version;
			req->UserAgent = "WikipediaTranslationSupportTool/" + ver->Major + "." + String::Format("{0:D2}",ver->Minor);
		}
		// Referer�ݒ�
		if(!String::IsNullOrEmpty(i_Referer)){
			req->Referer = i_Referer;
		}
		HttpWebResponse ^res = safe_cast<HttpWebResponse^>(req->GetResponse());
		_GetArticleStatus = res->StatusCode;

		// �����f�[�^����M���邽�߂�Stream���擾���A�f�[�^���擾
		// ���擾����XML�����킩�́A�����ł͊m�F���Ȃ�
		_Xml = gcnew XmlDocument();
		_Xml->Load(res->GetResponseStream());
		res->Close();

		// �擾����XML���ꎞ�t�H���_�ɕۑ�
		try{
			// �ꎞ�t�H���_���m�F
			String ^tmpDir = Path::Combine(Path::GetTempPath(), Path::GetFileNameWithoutExtension(Application::ExecutablePath));
			if(Directory::Exists(tmpDir) == false){
				// �ꎞ�t�H���_���쐬
				Directory::CreateDirectory(tmpDir);
			}
			// �t�@�C���̕ۑ�
			Xml->Save(Path::Combine(tmpDir, MYAPP::Cmn::ReplaceInvalidFileNameChars(Title) + ".xml"));
		}
		catch(Exception ^e){
			System::Diagnostics::Debug::WriteLine("WikipediaArticle::getServerArticle > �ꎞ�t�@�C���̕ۑ��Ɏ��s���܂��� : " + e->Message);
		}
	}
	catch(WebException ^e){
		// ProtocolError�G���[�̏ꍇ�A�X�e�[�^�X�R�[�h��ێ�
		_Xml = nullptr;
		if(e->Status == WebExceptionStatus::ProtocolError){
			_GetArticleStatus = static_cast<HttpWebResponse^>(e->Response)->StatusCode;
		}
		_GetArticleException = e;
		return false;
	}
	catch(Exception ^e){
		_Xml = nullptr;
		_GetArticleException = e;
		return false;
	}
	return true;
}

/* �L����XML���L���b�V�����擾 */
bool WikipediaArticle::getCacheArticle(TimeSpan i_CacheEnabledSpan)
{
	// �������ƒl�`�F�b�N
	_Xml = nullptr;
	_GetArticleStatus = HttpStatusCode::PaymentRequired;
	_GetArticleException = nullptr;
	// �L���b�V�����g�p����ꍇ�̂�
	if(i_CacheEnabledSpan > TimeSpan(0)){
		// �L����XML�f�[�^���L���b�V���t�@�C������擾
		try{
			// �ꎞ�t�@�C���ɃA�N�Z�X
			String ^tmpFile = Path::Combine(Path::Combine(Path::GetTempPath(), Path::GetFileNameWithoutExtension(Application::ExecutablePath)), MYAPP::Cmn::ReplaceInvalidFileNameChars(Title) + ".xml");
			if(File::Exists(tmpFile) == true){
				// �t�@�C�����L���������̂��̂����m�F
				if((DateTime::UtcNow - File::GetLastWriteTimeUtc(tmpFile)) < i_CacheEnabledSpan){
					// �t�@�C����Stream�ŊJ���A�f�[�^���擾
					XmlDocument ^tmpXml = gcnew XmlDocument();
					FileStream ^fs = File::OpenRead(tmpFile);
					try{
						tmpXml->Load(fs);
					}
					finally{
						fs->Close();
					}
					// �擾����XML�t�@�C�����A�ړI�Ƃ���L���̂��̂����m�F
					XmlNamespaceManager ^nsMgr = gcnew XmlNamespaceManager(tmpXml->NameTable);
					nsMgr->AddNamespace("ns", const_cast<String^>(XMLNS));
					XmlElement ^rootElement = tmpXml->DocumentElement;
					XmlElement ^pageElement = safe_cast<XmlElement^>(tmpXml->SelectSingleNode("/ns:mediawiki/ns:page/ns:title", nsMgr));
					if(pageElement != nullptr){
						// ����R�[�h�E�L�������`�F�b�N�B�啶���E���������قȂ�ꍇ�A�ʂ̋L���Ɣ��ʂ���
						// ��Low Earth orbit�ւ̃��_�C���N�g��Low earth orbit�݂����Ȃ̂����邽��
						//   �������擪��Wikipedia�̋Z�p�I�����ŏ�ɑ啶���Ȃ��߁A�啶���ŏ�������
						String ^title = wchar_t::ToUpper(Title[0]).ToString();
						if(Title->Length > 1){
							title += Title->Substring(1);
						}
						if(rootElement->GetAttribute("xml:lang") == Server->Code &&
						   pageElement->InnerText == title){
							// XML�������o�ϐ��ɐݒ肵�A����I��
							System::Diagnostics::Debug::WriteLine("WikipediaArticle::getCacheArticle > �L���b�V���Ǎ��� : " + MYAPP::Cmn::ReplaceInvalidFileNameChars(Title) + ".xml");
							_Xml = tmpXml;
							return true;
						}
					}
				}
			}
		}
		catch(Exception ^e){
			_Xml = nullptr;
			_GetArticleException = e;
			return false;
		}
	}
	_GetArticleStatus = HttpStatusCode::NotFound;
	return false;
}

/* �w�肳�ꂽ����R�[�h�ւ̌���ԃ����N��Ԃ� */
String^ WikipediaArticle::GetInterWiki(String ^i_Code)
{
	// �������ƒl�`�F�b�N
	String ^interWiki = "";
	if(Text == ""){
		// GetArticle���s���Ă��Ȃ��ꍇ�AInvalidOperationException��Ԃ�
		throw gcnew InvalidOperationException();
	}
	// �L���ɑ��݂���w�茾��ւ̌���ԃ����N���擾
	for(int i = 0 ; i < Text->Length ; i++){
		// �R�����g�i<!--�j�̃`�F�b�N
		String ^comment = "";
		int index = chkComment(comment, i);
		if(index != -1){
			i = index;
		}
		// �w�茾��ւ̌���ԃ����N�̏ꍇ�A���e���擾���A�����I��
		else if(MYAPP::Cmn::ChkTextInnerWith(Text, i, "[[" + i_Code + ":") == true){
			Link link = ParseInnerLink(Text->Substring(i));
			if(!String::IsNullOrEmpty(link.Text)){
				interWiki = link.Article;
				break;
			}
		}
	}
	return interWiki;
}

/* �L����XML���疼�O��ԏ����擾 */
array<WikipediaInformation::Namespace>^ WikipediaArticle::GetNamespaces(void)
{
	// XML���疼�O��ԏ����擾
	array<WikipediaInformation::Namespace> ^namespaces = gcnew array<WikipediaInformation::Namespace>(0);
	if(Xml == nullptr){
		// GetArticle���s���Ă��Ȃ��ꍇ�AInvalidOperationException��Ԃ�
		throw gcnew InvalidOperationException();
	}
	XmlNamespaceManager ^nsMgr = gcnew XmlNamespaceManager(Xml->NameTable);
	nsMgr->AddNamespace("ns", const_cast<String^>(XMLNS));
	XmlNodeList ^nodeList = Xml->SelectNodes("/ns:mediawiki/ns:siteinfo/ns:namespaces/ns:namespace", nsMgr);
	for each(XmlNode ^node in nodeList){
		XmlElement ^e = safe_cast<XmlElement^>(node);
		if(e != nullptr){
			try{
				WikipediaInformation::Namespace ns = {Decimal::ToInt16(Decimal::Parse(e->GetAttribute("key"))), e->InnerText};
				MYAPP::Cmn::AddArray(namespaces, ns);
			}
			catch(...){}
		}
	}
	return namespaces;
}

/* �L�������_�C���N�g�����`�F�b�N */
bool WikipediaArticle::IsRedirect(void)
{
	// �l�`�F�b�N
	if(Text == ""){
		// GetArticle���s���Ă��Ȃ��ꍇ�AInvalidOperationException��Ԃ�
		throw gcnew InvalidOperationException();
	}
	// �w�肳�ꂽ�L�������_�C���N�g�L���i#REDIRECT���j�����`�F�b�N
	// �����{��ł݂����ɁA#REDIRECT�ƌ���ŗL��#�]���݂����Ȃ̂�����Ǝv����̂ŁA
	//   �|�󌳌���Ɖp��ł̐ݒ�Ń`�F�b�N
	for(int i = 0 ; i < 2 ; i++){
		String ^redirect = static_cast<String^>(Server->Redirect->Clone());
		if(i == 1){
			if(Server->Code == "en"){
				continue;
			}
			WikipediaInformation en("en");
			redirect = en.Redirect;
		}
		if(redirect != ""){
			if(Text->ToLower()->StartsWith(redirect->ToLower()) == true){
				Link link = ParseInnerLink(Text->Substring(redirect->Length)->TrimStart());
				if(!String::IsNullOrEmpty(link.Text)){
					_Redirect = link.Article;
					return true;
				}
			}
		}
	}
	return false;
}

/* �L�����J�e�S���[�����`�F�b�N */
bool WikipediaArticle::IsCategory(void)
{
	return IsCategory(Title);
}

/* �L�����摜�����`�F�b�N */
bool WikipediaArticle::IsImage(void)
{
	return IsImage(Title);
}

/* �L�����W�����O��ԈȊO�����`�F�b�N */
bool WikipediaArticle::IsNotMainNamespace(void)
{
	return IsNotMainNamespace(Title);
}

/* �n���ꂽ���������N�E�e���v���[�g����� */
int WikipediaArticle::chkLinkText(WikipediaFormat::Link %o_Link, int i_Index)
{
	return ChkLinkText(o_Link, Text, i_Index);
}

/* �L���{���̎w�肳�ꂽ�ʒu�ɑ��݂�����������N�E�e���v���[�g����� */
int WikipediaArticle::chkComment(String ^%o_Text, int i_Index)
{
	return ChkComment(o_Text, Text, i_Index);
}
