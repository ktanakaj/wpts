using System;
using Honememo;

namespace Honememo.Wptscs.Models
{
    // Wikipedia�̋L���̏������������߂̃N���X
    public class WikipediaFormat
    {
		// Wikipedia�̃����N�̗v�f���i�[���邽�߂̍\����
		public struct Link {
			public String Text;				// �����N�̃e�L�X�g�i[[�`]]�j
			public String Article;			// �����N�̋L����
			public String Section;			// �����N�̃Z�N�V�������i#�j
			public String[] PipeTexts;	// �����N�̃p�C�v��̕�����i|�j
			public String Code;				// ����Ԃ܂��͑��v���W�F�N�g�ւ̃����N�̏ꍇ�A�R�[�h
			public bool TemplateFlag;			// �e���v���[�g�i{{�`}}�j���������t���O
			public bool SubPageFlag;			// �L�����̐擪�� / �Ŏn�܂邩�������t���O
			public bool StartColonFlag;		// �����N�̐擪�� : �Ŏn�܂邩�������t���O
			public bool MsgnwFlag;				// �e���v���[�g�̏ꍇ�Amsgnw: ���t������Ă��邩�������t���O
			public bool EnterFlag;				// �e���v���[�g�̏ꍇ�A�L�����̌�ŉ��s����邩�������t���O

            /* ������ */
            public void Initialize()
            {
	            // �R���X�g���N�^�̑���ɁA�K�v�Ȃ炱��ŏ�����
	            Text = null;
	            Article = null;
	            Section = null;
	            PipeTexts = new String[0];
	            Code = null;
	            TemplateFlag = false;
	            SubPageFlag = false;
	            StartColonFlag = false;
	            MsgnwFlag = false;
	            EnterFlag = false;
            }

            /* ���݂�Text�ȊO�̒l����AText�𐶐� */
            public String MakeText()
            {
	            // �߂�l������
	            String result = "";
	            // �g�̐ݒ�
	            String startSign = "[[";
	            String endSign = "]]";
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
		            result += MSGNW;
	            }
	            // ����R�[�h�E���v���W�F�N�g�R�[�h�̕t��
	            if(!String.IsNullOrEmpty(Code)){
		            result += Code;
	            }
	            // �����N�̕t��
	            if(!String.IsNullOrEmpty(Article)){
		            result += Article;
	            }
	            // �Z�N�V�������̕t��
	            if(!String.IsNullOrEmpty(Section)){
		            result += ("#" + Section);
	            }
	            // ���s�̕t��
	            if(EnterFlag){
		            result += '\n';
	            }
	            // �p�C�v��̕�����̕t��
	            if(PipeTexts != null){
		            foreach(String text in PipeTexts){
			            result += "|";
			            if(!String.IsNullOrEmpty(text)){
				            result += text;
			            }
		            }
	            }
	            // �I���̘g�̕t��
	            result += endSign;
	            return result;
            }
		};

        /* �R���X�g���N�^�i�T�[�o�[���w��j */
        public WikipediaFormat(WikipediaInformation i_Server)
        {
	        // ���K�{�ȏ�񂪐ݒ肳��Ă��Ȃ��ꍇ�AArgumentNullException��Ԃ�
	        if(i_Server == null){
		        throw new ArgumentNullException("i_Server");
	        }
	        // �����o�ϐ��̏�����
	        _Server = i_Server;
        }

        /* �n���ꂽ�L�������J�e�S���[�����`�F�b�N */
        public virtual bool IsCategory(String i_Name)
        {
	        // �w�肳�ꂽ�L�������J�e�S���[�iCategory:���Ŏn�܂�j�����`�F�b�N
            String category = Server.GetNamespace(WikipediaInformation.CATEGORYNAMESPACENUMBER);
	        if(category != ""){
		        if(i_Name.ToLower().StartsWith(category.ToLower() + ":") == true){
			        return true;
		        }
	        }
	        return false;
        }

        /* �n���ꂽ�L�������摜�����`�F�b�N */
        public virtual bool IsImage(String i_Name)
        {
	        // �w�肳�ꂽ�L�������摜�iImage:���Ŏn�܂�j�����`�F�b�N
	        // �����{��ł݂����ɁAimage: �ƌ���ŗL�� �摜: �݂����Ȃ̂�����Ǝv����̂ŁA
	        //   �|�󌳌���Ɖp��ł̐ݒ�Ń`�F�b�N
	        for(int i = 0 ; i < 2 ; i++){
                String image = Server.GetNamespace(WikipediaInformation.IMAGENAMESPACENUMBER);
		        if(i == 1){
			        if(Server.Code == "en"){
				        continue;
			        }
			        WikipediaInformation en = new WikipediaInformation("en");
                    image = en.GetNamespace(WikipediaInformation.IMAGENAMESPACENUMBER);
		        }
		        if(image != ""){
			        if(i_Name.ToLower().StartsWith(image.ToLower() + ":") == true){
				        return true;
			        }
		        }
	        }
	        return false;
        }

        /* �n���ꂽ�L�������W�����O��ԈȊO�����`�F�b�N */
        public virtual bool IsNotMainNamespace(String i_Name)
        {
	        // �w�肳�ꂽ�L�������W�����O��ԈȊO�̖��O��ԁiWikipedia:���Ŏn�܂�j�����`�F�b�N
	        foreach(WikipediaInformation.Namespace ns in Server.Namespaces){
		        if(i_Name.ToLower().StartsWith(ns.Name.ToLower() + ":") == true){
			        return true;
		        }
	        }
	        return false;
        }

        /* �n���ꂽWikipedia�̓��������N����� */
        public virtual Link ParseInnerLink(String i_Text)
        {
	        // �o�͒l������
	        Link result = new Link();
	        result.Initialize();
	        // ���͒l�m�F
	        if(i_Text.StartsWith("[[") == false){
		        return result;
	        }

	        // �\������͂��āA[[]]�����̕�������擾
	        // ���\����Wikipedia�̃v���r���[�ŐF�X�����Ċm�F�A����Ȃ�������Ԉ���Ă��肷�邩���E�E�E
	        String article = "";
	        String section = "";
	        String[] pipeTexts = new String[0];
	        int lastIndex = -1;
	        int pipeCounter = 0;
	        bool sharpFlag = false;
	        for(int i = 2 ; i < i_Text.Length ; i++){
		        char c = i_Text[i];
		        // ]]������������A��������I��
		        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, "]]") == true){
			        lastIndex = ++i;
			        break;
		        }
		        // | ���܂܂�Ă���ꍇ�A�ȍ~�̕�����͕\�����ȂǂƂ��Ĉ���
		        if(c == '|'){
			        ++pipeCounter;
			        Honememo.Cmn.AddArray(ref pipeTexts, "");
			        continue;
		        }
		        // �ϐ��i[[{{{1}}}]]�Ƃ��j�̍ċA�`�F�b�N
		        String dummy = "";
		        String variable = "";
		        int index = ChkVariable(ref variable, ref dummy, i_Text, i);
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
			        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, COMMENTSTART)){
				        break;
			        }
			        // nowiki�̃`�F�b�N
			        String nowiki = "";
			        index = ChkNowiki(ref nowiki, i_Text, i);
			        if(index != -1){
				        i = index;
				        pipeTexts[pipeCounter - 1] += nowiki;
				        continue;
			        }
			        // �����N [[ {{ �i[[image:xx|[[test]]�̉摜]]�Ƃ��j�̍ċA�`�F�b�N
			        Link link = new Link();
			        index = ChkLinkText(ref link, i_Text, i);
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
		        result.Text = i_Text.Substring(0, lastIndex + 1);
		        // �O��̃X�y�[�X�͍폜�i���o���͌��̂݁j
		        result.Article = article.Trim();
		        result.Section = section.TrimEnd();
		        // | �ȍ~�͂��̂܂ܐݒ�
		        result.PipeTexts = pipeTexts;
		        // �L����������𒊏o
		        // �T�u�y�[�W
		        if(result.Article.StartsWith("/") == true){
			        result.SubPageFlag = true;
		        }
		        // �擪�� :
		        else if(result.Article.StartsWith(":")){
			        result.StartColonFlag = true;
			        result.Article = result.Article.TrimStart(':').TrimStart();
		        }
		        // �W�����O��ԈȊO��[[xxx:yyy]]�̂悤�ɂȂ��Ă���ꍇ�A����R�[�h
		        if(result.Article.Contains(":") == true && !IsNotMainNamespace(result.Article)){
			        // ���{���́A����R�[�h���̈ꗗ�����A�����ƈ�v������̂��E�E�E�Ƃ��ׂ����낤���A
			        //   �����e������Ȃ��̂� : ���܂ޖ��O��ԈȊO��S�Č���R�[�h���Ɣ���
			        result.Code = result.Article.Substring(0, result.Article.IndexOf(':')).TrimEnd();
			        result.Article = result.Article.Substring(result.Article.IndexOf(':') + 1).TrimStart();
		        }
	        }
	        return result;
        }

        /* �n���ꂽWikipedia�̃e���v���[�g����� */
        public virtual Link ParseTemplate(String i_Text)
        {
	        // �o�͒l������
	        Link result = new Link();
	        result.Initialize();
	        result.TemplateFlag = true;
	        // ���͒l�m�F
	        if(i_Text.StartsWith("{{") == false){
		        return result;
	        }

	        // �\������͂��āA{{}}�����̕�������擾
	        // ���\����Wikipedia�̃v���r���[�ŐF�X�����Ċm�F�A����Ȃ�������Ԉ���Ă��肷�邩���E�E�E
	        String article = "";
	        String[] pipeTexts = new String[0];
	        int lastIndex = -1;
	        int pipeCounter = 0;
	        for(int i = 2 ; i < i_Text.Length ; i++){
		        char c = i_Text[i];
		        // }}������������A��������I��
		        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, "}}") == true){
			        lastIndex = ++i;
			        break;
		        }
		        // | ���܂܂�Ă���ꍇ�A�ȍ~�̕�����͈����ȂǂƂ��Ĉ���
		        if(c == '|'){
			        ++pipeCounter;
			        Honememo.Cmn.AddArray(ref pipeTexts, "");
			        continue;
		        }
		        // �ϐ��i[[{{{1}}}]]�Ƃ��j�̍ċA�`�F�b�N
		        String dummy = "";
		        String variable = "";
		        int index = ChkVariable(ref variable, ref dummy, i_Text, i);
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
			        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, COMMENTSTART)){
				        break;
			        }
			        // nowiki�̃`�F�b�N
			        String nowiki = "";
			        index = ChkNowiki(ref nowiki, i_Text, i);
			        if(index != -1){
				        i = index;
				        pipeTexts[pipeCounter - 1] += nowiki;
				        continue;
			        }
			        // �����N [[ {{ �i{{test|[[��]]}}�Ƃ��j�̍ċA�`�F�b�N
			        Link link = new Link();
			        index = ChkLinkText(ref link, i_Text, i);
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
		        result.Text = i_Text.Substring(0, lastIndex + 1);
		        // �O��̃X�y�[�X�E���s�͍폜�i���o���͌��̂݁j
		        result.Article = article.Trim();
		        // | �ȍ~�͂��̂܂ܐݒ�
		        result.PipeTexts = pipeTexts;
		        // �L����������𒊏o
		        // �T�u�y�[�W
		        if(result.Article.StartsWith("/") == true){
			        result.SubPageFlag = true;
		        }
		        // �擪�� :
		        else if(result.Article.StartsWith(":")){
			        result.StartColonFlag = true;
			        result.Article = result.Article.TrimStart(':').TrimStart();
		        }
		        // �擪�� msgnw:
		        result.MsgnwFlag = result.Article.ToLower().StartsWith(MSGNW.ToLower());
		        if(result.MsgnwFlag){
			        result.Article = result.Article.Substring(MSGNW.Length);
		        }
		        // �L��������̉��s�̗L��
		        if(article.TrimEnd(' ').EndsWith("\n")){
			        result.EnterFlag = true;
		        }
	        }
	        return result;
        }

        /* �n���ꂽ�e�L�X�g�̎w�肳�ꂽ�ʒu�ɑ��݂���Wikipedia�̓��������N�E�e���v���[�g���`�F�b�N */
        // �����펞�̖߂�l�ɂ́A]]�̌���]�̈ʒu�̃C���f�b�N�X��Ԃ��B�ُ펞��-1
        public int ChkLinkText(ref Link o_Link, String i_Text, int i_Index)
        {
	        // �o�͒l������
	        int lastIndex = -1;
	        o_Link.Initialize();
	        // ���͒l�ɉ����āA������U�蕪��
	        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, "[[") == true){
		        // ���������N
		        o_Link = ParseInnerLink(i_Text.Substring(i_Index));
	        }
	        else if(Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, "{{") == true){
		        // �e���v���[�g
		        o_Link = ParseTemplate(i_Text.Substring(i_Index));
	        }
	        // �������ʊm�F
	        if(!String.IsNullOrEmpty(o_Link.Text)){
		        lastIndex = i_Index + o_Link.Text.Length - 1;
	        }
	        return lastIndex;
        }

        /* �n���ꂽ�e�L�X�g�̎w�肳�ꂽ�ʒu�ɑ��݂���ϐ������ */
        public virtual int ChkVariable(ref String o_Variable, ref String o_Value, String i_Text, int i_Index)
        {
	        // �o�͒l������
	        int lastIndex = -1;
	        o_Variable = "";
	        o_Value = "";
	        // ���͒l�m�F
	        if(Honememo.Cmn.ChkTextInnerWith(i_Text.ToLower(), i_Index, "{{{") == false){
		        return lastIndex;
	        }
	        // �u���b�N�I���܂Ŏ擾
	        bool pipeFlag = false;
	        for(int i = i_Index + 3; i < i_Text.Length ; i++){
		        // �I�������̃`�F�b�N
		        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, "}}}") == true){
			        lastIndex = i + 2;
			        break;
		        }
		        // �R�����g�i<!--�j�̃`�F�b�N
		        String dummy = "";
		        int index = WikipediaArticle.ChkComment(ref dummy, i_Text, i);
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
			        String nowiki = "";
			        index = ChkNowiki(ref nowiki, i_Text, i);
			        if(index != -1){
				        i = index;
				        o_Value += nowiki;
				        continue;
			        }
			        // �ϐ��i{{{1|{{{2}}}}}}�Ƃ��j�̍ċA�`�F�b�N
			        String variable = "";
			        index = ChkVariable(ref variable, ref dummy, i_Text, i);
			        if(index != -1){
				        i = index;
				        o_Value += variable;
				        continue;
			        }
			        // �����N [[ {{ �i{{{1|[[test]]}}}�Ƃ��j�̍ċA�`�F�b�N
			        Link link = new Link();
			        index = ChkLinkText(ref link, i_Text, i);
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
		        o_Variable = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
	        }
	        // ����ȍ\���ł͂Ȃ������ꍇ�A�o�͒l���N���A
	        else{
		        o_Variable = "";
		        o_Value = "";
	        }
	        return lastIndex;
        }

        /* nowiki��Ԃ̃`�F�b�N */
        public static int ChkNowiki(ref String o_Text, String i_Text, int i_Index)
        {
	        // �o�͒l������
	        int lastIndex = -1;
	        o_Text = "";
	        // ���͒l�m�F
	        if(Honememo.Cmn.ChkTextInnerWith(i_Text.ToLower(), i_Index, NOWIKISTART.ToLower()) == false){
		        return lastIndex;
	        }
	        // �u���b�N�I���܂Ŏ擾
	        for(int i = i_Index + NOWIKISTART.Length; i < i_Text.Length ; i++){
		        // �I�������̃`�F�b�N
		        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, NOWIKIEND)){
			        lastIndex = i + NOWIKIEND.Length - 1;
			        break;
		        }
		        // �R�����g�i<!--�j�̃`�F�b�N
		        String dummy = "";
		        int index = WikipediaArticle.ChkComment(ref dummy, i_Text, i);
		        if(index != -1){
			        i = index;
			        continue;
		        }
	        }
	        // �I��肪������Ȃ��ꍇ�́A�S��nowiki�u���b�N�Ɣ��f
	        if(lastIndex == -1){
		        lastIndex = i_Text.Length - 1;
	        }
	        o_Text = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
	        return lastIndex;
        }

        /* �R�����g��Ԃ̃`�F�b�N */
        public static int ChkComment(ref String o_Text, String i_Text, int i_Index)
        {
	        // �o�͒l������
	        int lastIndex = -1;
	        o_Text = "";
	        // ���͒l�m�F
	        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i_Index, COMMENTSTART) == false){
		        return lastIndex;
	        }
	        // �R�����g�I���܂Ŏ擾
	        for(int i = i_Index + COMMENTSTART.Length; i < i_Text.Length ; i++){
		        if(Honememo.Cmn.ChkTextInnerWith(i_Text, i, COMMENTEND)){
			        lastIndex = i + COMMENTEND.Length - 1;
			        break;
		        }
	        }
	        // �I��肪������Ȃ��ꍇ�́A�S�ăR�����g�Ɣ��f
	        if(lastIndex == -1){
		        lastIndex = i_Text.Length - 1;
	        }
	        o_Text = i_Text.Substring(i_Index, lastIndex - i_Index + 1);
	        return lastIndex;
        }

		// Wikipedia�̌Œ�l�̏���
        public static readonly String COMMENTSTART = "<!--";
        public static readonly String COMMENTEND = "-->";
        public static readonly String NOWIKISTART = "<nowiki>";
        public static readonly String NOWIKIEND = "</nowiki>";
        public static readonly String MSGNW = "msgnw:";

		// �L������������T�[�o�[���
		public WikipediaInformation Server
        {
			get {
				return _Server;
			}
		}

		// �L������������T�[�o�[���iproperty�j
		protected WikipediaInformation _Server;
    }
}
