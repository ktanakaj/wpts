// �|��x�������̖{�̃N���X�AWikipedia�p
#include "stdafx.h"
#include "TranslateWikipedia.h"

using namespace wpts;

/* �|��x���������s���̖{�� */
bool TranslateWikipedia::runBody(String ^i_Name)
{
	System::Diagnostics::Debug::WriteLine("\nTranslateWikipedia::runBody > " + i_Name);
	// �ΏۋL�����擾
	WikipediaArticle ^article = chkTargetArticle(i_Name);
	if(article->Text == ""){
		return false;
	}
	// �ΏۋL���Ɍ���ԃ����N�����݂���ꍇ�A�������p�����邩�m�F
	String ^interWiki = article->GetInterWiki(target->Code);
	if(interWiki != ""){
		if(MessageBox::Show(
				String::Format(cmnAP->Resource->GetString("QuestionMessage_ArticleExist"), interWiki),
				cmnAP->Resource->GetString("QuestionTitle"),
				MessageBoxButtons::YesNo,
				MessageBoxIcon::Question)
		   == System::Windows::Forms::DialogResult::No){
			logLine(ENTER + String::Format(cmnAP->Resource->GetString("QuestionMessage_ArticleExist"), interWiki));
			return false;
		}
		else{
			logLine("�� " + String::Format(cmnAP->Resource->GetString("LogMessage_ArticleExistInterWiki"), interWiki));
		}
	}

	// �`�������쐬
	Text += "'''xxx'''";
	String ^bracket = static_cast<WikipediaInformation^>(target)->Bracket;
	if(bracket->Contains("{0}") == true){
		String ^originalName = "";
		String ^langTitle = static_cast<WikipediaInformation^>(source)->GetFullName(target->Code);
		if(langTitle != ""){
			originalName = "[[" + langTitle + "]]: ";
		}
		Text += String::Format(bracket, originalName + "'''" + i_Name + "'''");
	}
	Text += "\n\n";
	// ����ԃ����N�E��^��̕ϊ�
	logLine(ENTER + "�� " + String::Format(cmnAP->Resource->GetString("LogMessage_CheckAndReplaceStart"), interWiki));
	Text += replaceText(article->Text, article->Title);
	// ���[�U�[����̒��~�v�����`�F�b�N
	if(CancellationPending){
		return false;
	}
	// �V��������ԃ����N�ƁA�R�����g��ǋL
	Text += ("\n\n[[" + source->Code + ":" + i_Name + "]]\n");
	Text += (String::Format(cmnAP->Resource->GetString("ArticleFooter"), MYAPP::Cmn::GetProductName(),
		source->Code, i_Name, article->Timestamp.ToString("U")) + "\n");
	// �_�E�����[�h�����e�L�X�g��LF�Ȃ̂ŁA�����őS��CRLF�ɕϊ�
	// ���_�E�����[�h����CRLF�ɂ���悤�Ȏd�g�݂�������΁A��������g��
	//   ���̏ꍇ�A��̂悤��\n���ׂ��ɓf���Ă��镔�����C������
	Text = Text->Replace("\n", const_cast<String^>(ENTER));

	System::Diagnostics::Debug::WriteLine("TranslateWikipedia::runBody > Success!");
	return true;
}

/* �|��x���Ώۂ̋L�����擾 */
WikipediaArticle^ TranslateWikipedia::chkTargetArticle(String ^i_Name)
{
	// �w�肳�ꂽ�L���̐��f�[�^��Wikipedia����擾
	logLine(String::Format(cmnAP->Resource->GetString("LogMessage_GetArticle"), "http://" + static_cast<WikipediaInformation^>(source)->Server, i_Name));
	WikipediaArticle ^article = gcnew WikipediaArticle(static_cast<WikipediaInformation^>(source), i_Name);
	if(article->GetArticle(UserAgent, Referer, TimeSpan(0)) == false){
		if(article->GetArticleStatus == HttpStatusCode::NotFound){
			logLine("�� " + cmnAP->Resource->GetString("LogMessage_ArticleNothing"));
		}
		else{
			logLine("�� " + article->GetArticleException->Message);
			logLine("�� " + String::Format(cmnAP->Resource->GetString("LogMessage_ErrorURL"), article->Url));
		}
	}
	else{
		// Wikipedia�ւ̏���A�N�Z�X���ɁA���O��ԏ����擾����
		static_cast<WikipediaInformation^>(source)->Namespaces = article->GetNamespaces();
		// ���_�C���N�g�����`�F�b�N���A���_�C���N�g�ł���΁A���̐�̋L�����擾
		if(article->IsRedirect()){
			logLine("�� " + cmnAP->Resource->GetString("LogMessage_Redirect") + " [[" + article->Redirect + "]]");
			article = gcnew WikipediaArticle(static_cast<WikipediaInformation^>(source), article->Redirect);
			if(article->GetArticle(UserAgent, Referer, TimeSpan(0)) == false){
				if(article->GetArticleStatus == HttpStatusCode::NotFound){
					logLine("�� " + cmnAP->Resource->GetString("LogMessage_ArticleNothing"));
				}
				else{
					logLine("�� " + article->GetArticleException->Message);
					logLine("�� " + String::Format(cmnAP->Resource->GetString("LogMessage_ErrorURL"), article->Url));
				}
			}
		}
	}
	return article;
}

/* �w�肳�ꂽ�L�����擾���A����ԃ����N���m�F�A�Ԃ� */
String^ TranslateWikipedia::getInterWiki(String ^i_Name, bool i_TemplateFlag)
{
	// �w�肳�ꂽ�L���̐��f�[�^��Wikipedia����擾
	// ���L�����̂����݂��Ȃ��ꍇ�ANULL��Ԃ�
	String ^interWiki = nullptr;
	String ^name = i_Name;
	if(!i_TemplateFlag){
		Log += "[[" + name + "]] �� ";
	}
	else{
		Log += "{{" + name + "}} �� ";
	}
	WikipediaArticle ^article = gcnew WikipediaArticle(static_cast<WikipediaInformation^>(source), i_Name);
	if(article->GetArticle(UserAgent, Referer) == false){
		if(article->GetArticleStatus == HttpStatusCode::NotFound){
			Log += cmnAP->Resource->GetString("LogMessage_LinkArticleNothing");
		}
		else{
			logLine("�� " + article->GetArticleException->Message);
			logLine("�� " + String::Format(cmnAP->Resource->GetString("LogMessage_ErrorURL"), article->Url));
		}
	}
	else{
		// ���_�C���N�g�����`�F�b�N���A���_�C���N�g�ł���΁A���̐�̋L�����擾
		if(article->IsRedirect()){
			Log += (cmnAP->Resource->GetString("LogMessage_Redirect") + " [[" + article->Redirect + "]] �� ");
			article = gcnew WikipediaArticle(static_cast<WikipediaInformation^>(source), article->Redirect);
			if(article->GetArticle(UserAgent, Referer) == false){
				if(article->GetArticleStatus == HttpStatusCode::NotFound){
					logLine("�� " + cmnAP->Resource->GetString("LogMessage_ArticleNothing"));
				}
				else{
					logLine("�� " + article->GetArticleException->Message);
					logLine("�� " + String::Format(cmnAP->Resource->GetString("LogMessage_ErrorURL"), article->Url));
				}
			}
		}
		if(article->Text != ""){
			// �|��挾��ւ̌���ԃ����N��{��
			interWiki = article->GetInterWiki(target->Code);
			if(interWiki != ""){
				Log += ("[[" + interWiki + "]]");
			}
			else{
				Log += cmnAP->Resource->GetString("LogMessage_InterWikiNothing");
			}
		}
	}
	// ���s���o�͂���Ă��Ȃ��ꍇ�i���펞�j�A���s
	if(Log->EndsWith(const_cast<String^>(ENTER)) == false){
		Log += ENTER;
	}
	return interWiki;
}

/* �w�肳�ꂽ�L�����擾���A����ԃ����N���m�F�A�Ԃ��i�e���v���[�g�ȊO�j */
String^ TranslateWikipedia::getInterWiki(String ^i_Name)
{
	return getInterWiki(i_Name, false);
}

// �n���ꂽ�e�L�X�g����͂��A����ԃ����N�E���o�����̕ϊ����s��
String^ TranslateWikipedia::replaceText(String ^i_Text, String ^i_Parent, bool i_TitleFlag)
{
	// �w�肳�ꂽ�L���̌���ԃ����N�E���o����T�����A�|��挾��ł̖��̂ɕϊ����A����ɒu�������������Ԃ�
	String ^result = "";
	bool enterFlag = true;
	WikipediaFormat wikiAP(static_cast<WikipediaInformation^>(source));
	for(int i = 0 ; i < i_Text->Length ; i++){
		// ���[�U�[����̒��~�v�����`�F�b�N
		if(CancellationPending == true){
			break;
		}
		wchar_t c = i_Text[i];
		// ���o���������Ώۂ̏ꍇ
		if(i_TitleFlag){
			// ���s�̏ꍇ�A���̃��[�v�Ō��o���s�`�F�b�N���s��
			if(c == '\n'){
				enterFlag = true;
				result += c;
				continue;
			}
			// �s�̎n�߂ł́A���̍s�����o���̍s���̃`�F�b�N���s��
			if(enterFlag){
				String ^newTitleLine = "";
				int index = chkTitleLine(newTitleLine, i_Text, i);
				if(index != -1){
					// �s�̏I���܂ŃC���f�b�N�X���ړ�
					i = index;
					// �u��������ꂽ���o���s���o��
					result += newTitleLine;
					continue;
				}
				else{
					enterFlag = false;
				}
			}
		}
		// �R�����g�i<!--�j�̃`�F�b�N
		String ^comment = "";
		int index = WikipediaFormat::ChkComment(comment, i_Text, i);
		if(index != -1){
			i = index;
			result += comment;
			if(comment->Contains("\n") == true){
				enterFlag = true;
			}
			continue;
		}
		// nowiki�̃`�F�b�N
		String ^nowiki = "";
		index = WikipediaFormat::ChkNowiki(nowiki, i_Text, i);
		if(index != -1){
			i = index;
			result += nowiki;
			continue;
		}
		// �ϐ��i{{{1}}}�Ƃ��j�̃`�F�b�N
		String ^variable = "";
		String ^value = "";
		index = wikiAP.ChkVariable(variable, value, i_Text, i);
		if(index != -1){
			i = index;
			// �ϐ��� | �ȍ~�ɒl���L�q����Ă���ꍇ�A����ɑ΂��čċA�I�ɏ������s��
			int valueIndex = variable->IndexOf('|');
			if(valueIndex != -1 && !String::IsNullOrEmpty(value)){
				variable = (variable->Substring(0, valueIndex + 1) + replaceText(value, i_Parent) + "}}}");
			}
			result += variable;
			continue;
		}
		// ���������N�E�e���v���[�g�̃`�F�b�N���ϊ��A����ԃ����N���擾���o�͂���
		String ^text = "";
		index = replaceLink(text, i_Text, i, i_Parent);
		if(index != -1){
			i = index;
			result += text;
			continue;
		}
		// �ʏ�͂��̂܂܃R�s�[
		result += i_Text[i];
	}
	return result;
}

// �n���ꂽ�e�L�X�g����͂��A����ԃ����N�E���o�����̕ϊ����s��
String^ TranslateWikipedia::replaceText(String ^i_Text, String ^i_Parent)
{
	return replaceText(i_Text, i_Parent, true);
}

// �����N�̉�́E�u�����s��
int TranslateWikipedia::replaceLink(String ^%o_Link, String ^i_Text, int i_Index, String ^i_Parent)
{
	// �o�͒l������
	int lastIndex = -1;
	o_Link = "";
	WikipediaFormat::Link link;
	// ���������N�E�e���v���[�g�̊m�F�Ɖ��
	WikipediaFormat wikiAP(static_cast<WikipediaInformation^>(source));
	lastIndex = wikiAP.ChkLinkText(link, i_Text, i_Index);
	if(lastIndex != -1){
		// �L�����ɕϐ����g���Ă���ꍇ������̂ŁA���̃`�F�b�N�ƓW�J
		int index = link.Article->IndexOf("{{{");
		if(index != -1){
			String ^variable = "";
			String ^value = "";
			int lastIndex = wikiAP.ChkVariable(variable, value, link.Article, index);
			if(lastIndex != -1 && !String::IsNullOrEmpty(value)){
				// �ϐ��� | �ȍ~�ɒl���L�q����Ă���ꍇ�A����ɒu��������
				String ^newArticle = (link.Article->Substring(0, index) + value);
				if(lastIndex + 1 < link.Article->Length){
					newArticle += link.Article->Substring(lastIndex + 1);
				}
				link.Article = newArticle;
			}
			// �l���ݒ肳��Ă��Ȃ��ꍇ�A�������Ă����傤���Ȃ��̂ŁA���O
			else{
				System::Diagnostics::Debug::WriteLine("TranslateWikipedia::replaceLink > �ΏۊO : " + link.Text);
				return -1;
			}
		}

		String ^newText = nullptr;
		// ���������N�̏ꍇ
		if(i_Text[i_Index] == '['){
			// ���������N�̕ϊ��㕶������擾
			newText = replaceInnerLink(link, i_Parent);
		}
		// �e���v���[�g�̏ꍇ
		else if(i_Text[i_Index] == '{'){
			// �e���v���[�g�̕ϊ��㕶������擾
			newText = replaceTemplate(link, i_Parent);
		}
		// ��L�ȊO�̏ꍇ�́A�ΏۊO
		else{
			System::Diagnostics::Debug::WriteLine("TranslateWikipedia::replaceLink > �v���O�����~�X : " + link.Text);
		}
		// �ϊ��㕶����NULL�ȊO
		if(newText != nullptr){
			o_Link = newText;
		}
		else{
			lastIndex = -1;
		}
	}
	return lastIndex;
}

// ���������N�̕������ϊ�����
String^ TranslateWikipedia::replaceInnerLink(WikipediaFormat::Link i_Link, String ^i_Parent)
{
	// �ϐ������ݒ�
	String ^result = "[[";
	String ^comment = "";
	WikipediaFormat::Link link = i_Link;
	// �L�������w���Ă���ꍇ�i[[#�֘A����]]�����Ƃ��j�ȊO
	if(!String::IsNullOrEmpty(link.Article) &&
	   !(link.Article == i_Parent && String::IsNullOrEmpty(link.Code) && !String::IsNullOrEmpty(link.Section))){
		// �ϊ��̑ΏۊO�Ƃ��郊���N�����`�F�b�N
		WikipediaArticle article(static_cast<WikipediaInformation^>(source), link.Article);
		// �T�u�y�[�W�̏ꍇ�A�L�������U
		if(link.SubPageFlag){
			link.Article = i_Parent + link.Article;
		}
		// ����ԃ����N�E�o���v���W�F�N�g�ւ̃����N�E�摜�͑ΏۊO
		else if(!String::IsNullOrEmpty(link.Code) || article.IsImage()){
			result = "";
			// �擪�� : �łȂ��A�|��挾��ւ̌���ԃ����N�̏ꍇ
			if(!link.StartColonFlag && link.Code == target->Code){
				// �폜����B����I���ŁA�u���㕶����Ȃ���Ԃ�
				System::Diagnostics::Debug::WriteLine("TranslateWikipedia::replaceInnerLink > " + link.Text + " ���폜");
				return "";
			}
			// ����ȊO�͑ΏۊO
			System::Diagnostics::Debug::WriteLine("TranslateWikipedia::replaceInnerLink > �ΏۊO : " + link.Text);
			return nullptr;
		}
		// �����N��H��A�ΏۋL���̌���ԃ����N���擾
		String ^interWiki = getInterWiki(link.Article);
		// �L�����̂����݂��Ȃ��i�ԃ����N�j�ꍇ�A�����N�͂��̂܂�
		if(interWiki == nullptr){
			result += link.Article;
		}
		// ����ԃ����N�����݂��Ȃ��ꍇ�A[[:en:xxx]]�݂����Ȍ`���ɒu��
		else if(interWiki == ""){
			result += (":" + source->Code + ":" + link.Article);
		}
		// ����ԃ����N�����݂���ꍇ�A��������w���悤�ɒu��
		else{
			// �O�̕�����𕜌�
			if(link.SubPageFlag){
				int index = interWiki->IndexOf('/');
				if(index == -1){
					index = 0;
				}
				result += interWiki->Substring(index);
			}
			else if(link.StartColonFlag){
				result += (":" + interWiki);
			}
			else{
				result += interWiki;
			}
		}
		// �J�e�S���[�̏ꍇ�́A�R�����g�Ō��̕������ǉ�����
		if(article.IsCategory() && !link.StartColonFlag){
			comment = (WikipediaFormat::COMMENTSTART + " " + link.Text + " " + WikipediaFormat::COMMENTEND);
			// �J�e�S���[��[[:en:xxx]]�݂����Ȍ`���ɂ����ꍇ�A| �ȍ~�͕s�v�Ȃ̂ō폜
			if(interWiki == ""){
				link.PipeTexts = gcnew array<String^>(0);
			}
		}
		// �\���������݂��Ȃ��ꍇ�A���̖��O��\�����ɐݒ�
		else if(link.PipeTexts->Length == 0 && interWiki != nullptr){
			MYAPP::Cmn::AddArray(link.PipeTexts, article.Title);
		}
	}
	// ���o���i[[#�֘A����]]�Ƃ��j���o��
	if(!String::IsNullOrEmpty(link.Section)){
		// ���o���́A��^��ϊ���ʂ�
		result += ("#" + getKeyWord(link.Section));
	}
	// �\�������o��
	for each(String ^text in link.PipeTexts){
		result += "|";
		if(!String::IsNullOrEmpty(text)){
			// �摜�̏ꍇ�A| �̌�ɓ��������N��e���v���[�g��������Ă���ꍇ�����邪�A
			// �摜�͏����ΏۊO�ł��肻�̒��̃����N�͌ʂɍēx��������邽�߁A�����ł͓��ɉ������Ȃ�
			result += text;
		}
	}
	// �����N�����
	result += "]]";
	// �R�����g��t��
	if(comment != ""){
		result += comment;
	}
	System::Diagnostics::Debug::WriteLine("TranslateWikipedia::replaceInnerLink > " + link.Text);
	return result;
}

// �e���v���[�g�̕������ϊ�����
String^ TranslateWikipedia::replaceTemplate(WikipediaFormat::Link i_Link, String ^i_Parent)
{
	// �ϐ������ݒ�
	String ^result = "";
	String ^comment = "";
	WikipediaFormat::Link link = i_Link;
	// �e���v���[�g�͋L�������K�{
	if(String::IsNullOrEmpty(link.Article)){
		System::Diagnostics::Debug::WriteLine("TranslateWikipedia::replaceTemplate > �ΏۊO : " + link.Text);
		return nullptr;
	}
	// �V�X�e���ϐ��̏ꍇ�͑ΏۊO
	if(static_cast<WikipediaInformation^>(source)->ChkSystemVariable(link.Article) == true){
		System::Diagnostics::Debug::WriteLine("TranslateWikipedia::replaceTemplate > �V�X�e���ϐ� : " + link.Text);
		return nullptr;
	}
	// �e���v���[�g���O��Ԃ��A���ʂ̋L�����𔻒�
	if(!link.StartColonFlag && !link.SubPageFlag){
		String ^templateStr = static_cast<WikipediaInformation^>(source)->GetNamespace(static_cast<WikipediaInformation^>(source)->TEMPLATENAMESPACENUMBER);
		if(templateStr != "" && !link.Article->StartsWith(templateStr + ":")){
			WikipediaArticle article(static_cast<WikipediaInformation^>(source), templateStr + ":" + link.Article);
			// �L�������݂���ꍇ�A�e���v���[�g���������O�Ŏ擾
			if(article.GetArticle(UserAgent, Referer) == true){
				link.Article = article.Title;
			}
			// �L�����擾�ł��Ȃ��ꍇ���A404�łȂ��ꍇ�͑��݂���Ƃ��ď���
			else if(article.GetArticleStatus != HttpStatusCode::NotFound){
				logLine(String::Format(cmnAP->Resource->GetString("LogMessage_TemplateUnknown"), link.Article, templateStr, article.GetArticleException->Message));
				link.Article = article.Title;
			}
		}
	}
	// �T�u�y�[�W�̏ꍇ�A�L�������U
	else if(link.SubPageFlag){
		link.Article = i_Parent + link.Article;
	}
	// �����N��H��A�ΏۋL���̌���ԃ����N���擾
	String ^interWiki = getInterWiki(link.Article, true);
	// �L�����̂����݂��Ȃ��i�ԃ����N�j�ꍇ�A�����N�͂��̂܂�
	if(interWiki == nullptr){
		result += link.Text;
	}
	// ����ԃ����N�����݂��Ȃ��ꍇ�A[[:en:Template:xxx]]�݂����ȕ��ʂ̃����N�ɒu��
	else if(interWiki == ""){
		// ���܂��ŁA���̃e���v���[�g�̏�Ԃ��R�����g�ł���
		result += ("[[:" + source->Code + ":" + link.Article + "]]" + WikipediaFormat::COMMENTSTART + " " + link.Text + " " + WikipediaFormat::COMMENTEND);
	}
	// ����ԃ����N�����݂���ꍇ�A��������w���悤�ɒu��
	else{
		result += "{{";
		// �O�̕�����𕜌�
		if(link.StartColonFlag){
			result += ":";
		}
		if(link.MsgnwFlag){
			result += WikipediaFormat::MSGNW;
		}
		// : ���O�̕������폜���ďo�́i: �������Ƃ���-1+1��0����j
		result += interWiki->Substring(interWiki->IndexOf(':') + 1);
		// ���s�𕜌�
		if(link.EnterFlag){
			result += "\n";
		}
		// | �̌��t��
		for each(String ^text in link.PipeTexts){
			result += "|";
			if(!String::IsNullOrEmpty(text)){
				// | �̌�ɓ��������N��e���v���[�g��������Ă���ꍇ������̂ŁA�ċA�I�ɏ�������
				result += replaceText(text, i_Parent);
			}
		}
		// �����N�����
		result += "}}";
	}
	System::Diagnostics::Debug::WriteLine("TranslateWikipedia::replaceTemplate > " + link.Text);
	return result;
}

/* �w�肳�ꂽ�C���f�b�N�X�̈ʒu�ɑ��݂��錩�o��(==�֘A����==�݂����Ȃ�)����͂��A�\�ł���Εϊ����ĕԂ� */
int TranslateWikipedia::chkTitleLine(String ^%o_Title, String ^i_Text, int i_Index)
{
	// ������
	// �����o���ł͂Ȃ��A�\�������������Ȃǂ̏ꍇ�A-1��Ԃ�
	int lastIndex = -1;
	o_Title = "";
/*	// ���͒l�m�F�A�t�@�C���̐擪�A�܂��͉��s���==�Ŏn�܂��Ă��邩���`�F�b�N
	// ���R�����g���Ƃ��l����ƃ��Y���̂ŁA==�����`�F�b�N���āA��͌Ăяo�����ōs��
	if(i_Index != 0){
		if((MYAPP::Cmn::ChkTextInnerWith(i_Text, i_Index - 1, "\n==") == false){
			return lastIndex;
		}
	}
	else if(MYAPP::Cmn::ChkTextInnerWith(i_Text, i_Index, "==") == false){
		return lastIndex;
	}
*/	// �\������͂��āA1�s�̕�����ƁA=�̌����擾
	// ���\����Wikipedia�̃v���r���[�ŐF�X�����Ċm�F�A����Ȃ�������Ԉ���Ă��肷�邩���E�E�E
	// ��Wikipedia�ł� <!--test-->=<!--test-->=�֘A����<!--test-->==<!--test--> �݂����Ȃ̂ł�
	//   ����ɔF������̂ŁA�ł��邾���Ή�����
	// ���ϊ�������ɍs��ꂽ�ꍇ�A�R�����g�͍폜�����
	bool startFlag = true;
	int startSignCounter = 0;
	String ^nonCommentLine = "";
	for(lastIndex = i_Index ; lastIndex < i_Text->Length ; lastIndex++){
		wchar_t c = i_Text[lastIndex];
		// ���s�܂�
		if(c == '\n'){
			break;
		}
		// �R�����g�͖�������
		String ^comment = "";
		int index = WikipediaArticle::ChkComment(comment, i_Text, lastIndex);
		if(index != -1){
			o_Title += comment;
			lastIndex = index;
			continue;
		}
		// �擪���̏ꍇ�A=�̐��𐔂���
		else if(startFlag){
			if(c == '='){
				++startSignCounter;
			}
			else{
				startFlag = false;
			}
		}
		nonCommentLine += c;
		o_Title += c;
	}
	// ���s�����A�܂��͕��͂̍Ō�+1�ɂȂ��Ă���͂��Ȃ̂ŁA1�����߂�
	--lastIndex;
	// = �Ŏn�܂�s�ł͂Ȃ��ꍇ�A�����ΏۊO
	if(startSignCounter < 1){
		o_Title = "";
		return -1;
	}
	// �I���� = �̐����m�F
	// �����̏������ƒ��g�̖����s�i====�Ƃ��j�͒e����Ă��܂����A�ǂ��������ł��Ȃ��̂ŋ��e����
	int endSignCounter = 0;
	for(int i = nonCommentLine->Length - 1 ; i >= startSignCounter ; i--){
		if(nonCommentLine[i] == '='){
			++endSignCounter;
		}
		else{
			break;
		}
	}
	// = �ŏI���s�ł͂Ȃ��ꍇ�A�����ΏۊO
	if(endSignCounter < 1){
		o_Title = "";
		return -1;
	}
	// �n�܂�ƏI���A=�̏��Ȃ��ق��ɂ��킹��i==test===�Ƃ��p�̏����j
	int signCounter = startSignCounter;
	if(startSignCounter > endSignCounter){
		signCounter = endSignCounter;
	}
	// ��^��ϊ�
	String ^oldText = nonCommentLine->Substring(signCounter, nonCommentLine->Length - (signCounter * 2))->Trim();
	String ^newText = getKeyWord(oldText);
	if(oldText != newText){
		String ^sign = "=";
		for(int i = 1 ; i < signCounter ; i++){
			sign += "=";
		}
		String ^newTitle = (sign + newText + sign);
		logLine(ENTER + o_Title + " �� " + newTitle);
		o_Title = newTitle;
	}
	else{
		logLine(ENTER + o_Title);
	}
	return lastIndex;
}

/* �w�肳�ꂽ�R�[�h�ł̒�^��ɑ�������A�ʂ̌���ł̒�^����擾 */
String^ TranslateWikipedia::getKeyWord(String ^i_Key)
{
	// ���ݒ肪���݂��Ȃ��ꍇ�A���͒�^������̂܂܂�Ԃ�
	String ^key = ((i_Key != nullptr) ? i_Key : "");
	WikipediaInformation^ src = static_cast<WikipediaInformation^>(source);
	WikipediaInformation^ tar = static_cast<WikipediaInformation^>(target);
	if(key->Trim() == ""){
		return key;
	}
	for(int i = 0 ; i < src->TitleKeys->Length ; i++){
		if(src->TitleKeys[i]->ToLower() == key->Trim()->ToLower()){
			if(tar->TitleKeys->Length > i){
				if(tar->TitleKeys[i] != ""){
					key = tar->TitleKeys[i];
				}
				break;
			}
		}
	}
	return key;
}
