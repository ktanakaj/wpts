// ================================================================================================
// <summary>
//      MediaWikiTemplateParserのテストクラスソース。</summary>
//
// <copyright file="MediaWikiTemplateParserTest.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using System.Collections.Generic;
    using Honememo.Parsers;
    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Websites;
    using NUnit.Framework;

    /// <summary>
    /// <see cref="MediaWikiTemplateParser"/>のテストクラスです。
    /// </summary>
    [TestFixture]
    class MediaWikiTemplateParserTest
    {
        #region private変数

        /// <summary>
        /// 前処理・後処理で生成／解放される言語別の<see cref="MediaWikiParser"/>。
        /// </summary>
        private IDictionary<string, MediaWikiParser> mediaWikiParsers = new Dictionary<string, MediaWikiParser>();

        #endregion

        #region 前処理・後処理

        /// <summary>
        /// テストの前処理。
        /// </summary>
        /// <remarks><see cref="MediaWikiParser.Dispose"/>が必要な<see cref="MediaWikiParser"/>の生成。</remarks>
        [TestFixtureSetUp]
        public void SetUpBeforeClass()
        {
            this.mediaWikiParsers["en"] = new MediaWikiParser(new MockFactory().GetMediaWiki("en"));
            this.mediaWikiParsers["ja"] = new MediaWikiParser(new MockFactory().GetMediaWiki("ja"));
        }

        /// <summary>
        /// テストの後処理。
        /// </summary>
        /// <remarks><see cref="MediaWikiParser.Dispose"/>が必要な<see cref="MediaWikiParser"/>の解放。</remarks>
        [TestFixtureTearDown]
        public void TearDownAfterClass()
        {
            foreach (IDisposable parser in this.mediaWikiParsers.Values)
            {
                parser.Dispose();
            }

            this.mediaWikiParsers.Clear();
        }

        #endregion

        #region インタフェース実装メソッドテストケース

        /// <summary>
        /// <see cref="MediaWikiTemplateParser.TryParse"/>メソッドテストケース（基本的な構文）。
        /// </summary>
        [Test]
        public void TestTryParseBasic()
        {
            IElement element;
            MediaWikiTemplate template;
            MediaWikiTemplateParser parser = new MediaWikiTemplateParser(this.mediaWikiParsers["en"]);

            // タイトルのみ
            Assert.IsTrue(parser.TryParse("{{testtitle}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("testtitle", template.Title);
            Assert.AreEqual(0, template.PipeTexts.Count);
            Assert.IsNull(template.Interwiki);
            Assert.IsFalse(template.IsColon);
            Assert.IsFalse(template.IsMsgnw);
            Assert.IsFalse(template.NewLine);

            // テンプレートではセクションは無いはず
            Assert.IsTrue(parser.TryParse("{{testtitle#testsection}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("testtitle#testsection", template.Title);
            Assert.IsNull(template.Section);
            Assert.AreEqual(0, template.PipeTexts.Count);

            // タイトルとパイプ後の文字列
            Assert.IsTrue(parser.TryParse("{{testtitle|testpipe1|testpipe2}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("testtitle", template.Title);
            Assert.AreEqual(2, template.PipeTexts.Count);
            Assert.AreEqual("testpipe1", template.PipeTexts[0].ToString());
            Assert.AreEqual("testpipe2", template.PipeTexts[1].ToString());

            // タイトルに名前空間まで指定されている場合
            // ※ テンプレートでは言語間リンク等は無いはず
            Assert.IsTrue(parser.TryParse("{{Template:testtitle}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("Template:testtitle", template.Title);
            Assert.AreEqual(0, template.PipeTexts.Count);

            // 先頭のコロンは認識する
            Assert.IsTrue(parser.TryParse("{{:Template:testtitle}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("Template:testtitle", template.Title);
            Assert.AreEqual(0, template.PipeTexts.Count);
            Assert.IsTrue(template.IsColon);

            // msgnw（テンプレートのソースを出力する指定）は特別扱い
            Assert.IsTrue(parser.TryParse("{{msgnw:testtitle}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("testtitle", template.Title);
            Assert.AreEqual(0, template.PipeTexts.Count);
            Assert.IsTrue(template.IsMsgnw);

            // テンプレートでは、こんな風に改行が含まれるケースも多々存在
            Assert.IsTrue(parser.TryParse("{{testtitle\n|testpipe1\n|testpipe2\n}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("testtitle", template.Title);
            Assert.AreEqual(2, template.PipeTexts.Count);
            Assert.AreEqual("testpipe1\n", template.PipeTexts[0].ToString());
            Assert.AreEqual("testpipe2\n", template.PipeTexts[1].ToString());
            Assert.IsTrue(template.NewLine);

            // コメントはどこにあってもOK
            Assert.IsTrue(parser.TryParse("{{Cite web<!--newsの方がよいか？-->|url=http://www.example.com|title=test|publisher=[[company]]<!--|date=2011-08-05-->|accessdate=2011-11-10}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("Cite web<!--newsの方がよいか？-->", template.Title);
            Assert.AreEqual(4, template.PipeTexts.Count);
            Assert.AreEqual("url=http://www.example.com", template.PipeTexts[0].ToString());
            Assert.AreEqual("title=test", template.PipeTexts[1].ToString());
            Assert.AreEqual("publisher=[[company]]<!--|date=2011-08-05-->", template.PipeTexts[2].ToString());
            Assert.AreEqual("accessdate=2011-11-10", template.PipeTexts[3].ToString());
        }

        /// <summary>
        /// <see cref="MediaWikiTemplateParser.TryParse"/>メソッドテストケース（NGパターン）。
        /// </summary>
        [Test]
        public void TestTryParseNg()
        {
            IElement element;
            MediaWikiTemplateParser parser = new MediaWikiTemplateParser(this.mediaWikiParsers["en"]);

            // 開始タグが無い
            Assert.IsFalse(parser.TryParse("testtitle}}", out element));

            // 閉じタグが無い
            Assert.IsFalse(parser.TryParse("{{testtitle", out element));

            // 先頭が開始タグではない
            Assert.IsFalse(parser.TryParse(" {{testtitle}}", out element));

            // 外部リンクタグ
            Assert.IsFalse(parser.TryParse("[testtitle]", out element));

            // 内部リンクタグ
            Assert.IsFalse(parser.TryParse("[[testtitle]]", out element));

            // 無理な位置のコメント（2012年1月現在、内部リンクはこれでもMediaWikiに認識されたがテンプレートはNG）
            Assert.IsFalse(parser.TryParse("{<!--test-->{テンプレート}}", out element));

            // テンプレート名の部分に < > [ ] { } のいずれかの文字が存在する場合、NG
            // ※ コメントや変数であれば可能、それ以外で存在するのがNG
            Assert.IsFalse(parser.TryParse("{{test<title}}", out element));
            Assert.IsFalse(parser.TryParse("{{test>title}}", out element));
            Assert.IsFalse(parser.TryParse("{{test[title}}", out element));
            Assert.IsFalse(parser.TryParse("{{test]title}}", out element));
            Assert.IsFalse(parser.TryParse("{{test{title}}", out element));
            Assert.IsFalse(parser.TryParse("{{test}title}}", out element));

            // 空・null
            Assert.IsFalse(parser.TryParse(String.Empty, out element));
            Assert.IsFalse(parser.TryParse(null, out element));
        }

        /// <summary>
        /// <see cref="MediaWikiTemplateParser.TryParse"/>メソッドテストケース（入れ子）。
        /// </summary>
        [Test]
        public void TestTryParseNested()
        {
            IElement element;
            MediaWikiTemplate template;
            MediaWikiTemplateParser parser = new MediaWikiTemplateParser(this.mediaWikiParsers["ja"]);

            // 入れ子もあり
            Assert.IsTrue(parser.TryParse("{{outertemplate|test=[[innerlink]]{{innertemplate}}}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("outertemplate", template.Title);
            Assert.AreEqual(1, template.PipeTexts.Count);
            Assert.AreEqual("test=[[innerlink]]{{innertemplate}}", template.PipeTexts[0].ToString());
            Assert.IsInstanceOf(typeof(ListElement), template.PipeTexts[0]);
            ListElement list = (ListElement)template.PipeTexts[0];
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("test=", list[0].ToString());
            Assert.AreEqual("[[innerlink]]", list[1].ToString());
            Assert.AreEqual("{{innertemplate}}", list[2].ToString());

            // 変数の場合、テンプレート名の部分にも入れられる
            Assert.IsTrue(parser.TryParse("{{{{{title|デフォルトジャンル}}}メニュー}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("{{{title|デフォルトジャンル}}}メニュー", template.Title);
        }

        /// <summary>
        /// <see cref="MediaWikiTemplateParser.TryParse"/>メソッドテストケース（サブページ）。
        /// </summary>
        [Test]
        public void TestTryParseSubpage()
        {
            IElement element;
            MediaWikiTemplate template;
            MediaWikiTemplateParser parser = new MediaWikiTemplateParser(this.mediaWikiParsers["en"]);

            // 全て指定されているケースは通常の記事と同じ扱い
            Assert.IsTrue(parser.TryParse("{{testtitle/subpage}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("testtitle/subpage", template.Title);
            Assert.IsFalse(template.IsSubpage());

            // 記事名が省略されているケース
            Assert.IsTrue(parser.TryParse("{{/subpage}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("/subpage", template.Title);
            Assert.IsTrue(template.IsSubpage());

            // 記事名が省略されているケース2
            Assert.IsTrue(parser.TryParse("{{../../subpage}}", out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("../../subpage", template.Title);
            Assert.IsTrue(template.IsSubpage());
        }

        /// <summary>
        /// <see cref="MediaWikiTemplateParser.TryParse"/>メソッドテストケース（実データ複雑なinfobox）。
        /// </summary>
        /// <remarks>使用データは[[:en:Discovery Channel]]（2012年1月17日 14:07:11(UTC)）より抜粋。</remarks>
        [Test]
        public void TestTryParseInfoboxTvChannel()
        {
            IElement element;
            MediaWikiTemplate template;
            MediaWikiTemplateParser parser = new MediaWikiTemplateParser(this.mediaWikiParsers["en"]);

            // 全て指定されているケースは通常の記事と同じ扱い
            Assert.IsTrue(parser.TryParse(
                "{{Infobox TV channel\n"
                + "| name             = '''Discovery Channel'''\n"
                + "| logofile         = Discovery Channel International.svg\n"
                + "| logosize         = 220px\n"
                + "| slogan           = ''The world is just awesome.''\n"
                + "| launch           = June 17, 1985&lt;ref name=&quot;oscars.org&quot;&gt;{{cite web |url=http://www.oscars.org/awards/academyawards/legacy/ceremony/59th-winners.html |title=The 59th Academy Awards (1987) Nominees and Winners |accessdate=2011-07-23|work=oscars.org}}&lt;/ref&gt;\n"
                + "| owner            = [[Discovery Communications|Discovery Communications, Inc.]]\n"
                + "| CEO              = David Zaslav\n"
                + "| headquarters     = [[Silver Spring, Maryland]]\n"
                + "| country          = Worldwide\n"
                + "| language         = English\n"
                + "| sister names     = [[TLC (TV channel)|TLC]]&lt;br&gt;[[Animal Planet]]&lt;br&gt;[[OWN: Oprah Winfrey Network]]&lt;br&gt;[[Planet Green]]&lt;br&gt;[[Investigation Discovery]]&lt;br&gt;[[Discovery Fit &amp; Health]]&lt;br&gt;[[Military Channel]]&lt;br&gt;[[Science (TV channel)|Science]]&lt;br&gt;[[Velocity (TV channel)|Velocity]]\n"
                + "| web              = http://www.discovery.com\n"
                + "| picture format   = [[480i]] ([[SDTV]])&lt;br&gt;[[1080i]] ([[HDTV]])\n"
                + "| terr serv 1 = [[Selective TV Inc.]]&lt;br/&gt;'''([[Alexandria, Minnesota]])'''\n"
                + "| terr chan 1 = K47KZ (Channel 47)\n"
                + "| sat serv 1 = [[DirecTV]]\n"
                + "| sat chan 1 = Channel 278&lt;br&gt; Channel 1278 (VOD)\n"
                + "| sat serv 2 = [[Dish Network]]\n"
                + "| sat chan 2 = Channel 182 (SD/HD)&lt;br&gt; Channel 9487\n"
                + "| sat serv 3 = [[C-Band]]\n"
                + "| sat chan 3 = AMC 10-Channel 21\n"
                + "| sat serv 7 = [[SKY México]]\n"
                + "| sat chan 7 = Channel 251\n"
                + "| sat serv 8 = Dish Network Mexico\n"
                + "| sat chan 8 = Channel 402\n"
                + "| sat serv 9 = [[Sky (UK and Ireland)]]\n"
                + "| sat chan 9 = Channel 520\n"
                + "| cable serv 1 = CableVision (Argentina)\n"
                + "| cable chan 1 = Channel 52\n"
                + "| cable serv 2 = Available on most cable systems\n"
                + "| cable chan 2 = Check your local listings\n"
                + "| cable serv 3 = [[Verizon FiOS]] \n"
                + "| cable chan 3 = Channel 120 (SD)&lt;br&gt;Channel 620 (HD)\n"
                + "| adsl serv 1 =[[Sky Angel]]\n"
                + "| adsl chan 1 =Channel 313\n"
                + "| adsl serv 2 =[[AT&amp;T U-Verse]]\n"
                + "| adsl chan 2 =Channel 120 (SD)&lt;br&gt; 1120 (HD)\n"
                + "}}\n\n"
                + "'''Discovery Channel''' (formerly '''The Discovery Channel''') is an American [[satellite]] and [[cable]] [[specialty channel]] (also delivered via [[IPTV]], [[terrestrial television]] and [[internet television]] in other parts of the world), founded by [[John Hendricks]] and distributed by [[Discovery Communications]].",
                out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("Infobox TV channel", template.Title);
            Assert.AreEqual(37, template.PipeTexts.Count);
            Assert.AreEqual(" terr serv 1 = [[Selective TV Inc.]]&lt;br/&gt;'''([[Alexandria, Minnesota]])'''\n", template.PipeTexts[13].ToString());
            Assert.AreEqual(" adsl chan 2 =Channel 120 (SD)&lt;br&gt; 1120 (HD)\n", template.PipeTexts[36].ToString());
            Assert.IsNull(template.Interwiki);
            Assert.IsFalse(template.IsColon);
            Assert.IsFalse(template.IsMsgnw);
            Assert.IsTrue(template.NewLine);

            // TODO: パイプ後の部分を入れ子も含めてちゃんと動いているかもうちょい検証する
        }

        /// <summary>
        /// <see cref="MediaWikiTemplateParser.TryParse"/>メソッドテストケース（実データ複雑なテンプレートの一部）。
        /// </summary>
        /// <remarks>
        /// 使用データはWiktionaryの[[:en:Template:context]]（2011-08-29T20:15:35Z）より抜粋。
        /// Ver 1.11にて無限ループの不具合が発生していたデータ。
        /// 中身についてはほぼ処理できない類のものだが、無限ループにならないことだけ検証。
        /// </remarks>
        [Test, Timeout(10000)]
        public void TestTryParseTemplateContext()
        {
            IElement element;
            MediaWikiTemplate template;
            MediaWikiTemplateParser parser = new MediaWikiTemplateParser(this.mediaWikiParsers["en"]);

            // テンプレートではなくスクリプトだが、このパーサーで解析される対象ではあるため
            Assert.IsTrue(parser.TryParse(
                "{{#if:{{{poscat|}}}|{{#if:{{{skey|}}}|\n"
                + "[[Category:{{{{#if:{{{poscat|}}}|languagex|ns:0}}|{{#if:{{{lang|}}}|{{{lang}}}|en}}}} {{{poscat|}}}|{{{skey}}} "
                + "{{SUBPAGENAME}}]]{{#if:{{{script|}}}|[[Category:{{{{#if:{{{poscat|}}}|languagex|ns:0}}|{{#if:{{{lang|}}}|{{{lang}}}|en}}}} "
                + "{{{poscat|}}} in {{{script}}} script|{{{skey}}} {{SUBPAGENAME}}]]}}{{#if:{{{script2|}}}|[[Category:{{{{#if:{{{poscat|}}}|"
                + "languagex|ns:0}}|{{#if:{{{lang|}}}|{{{lang}}}|en}}}} {{{poscat|}}} in {{{script2}}} script|{{{skey2}}} {{SUBPAGENAME}}]]}}|\n"
                + "[[Category:{{{{#if:{{{poscat|}}}|languagex|ns:0}}|{{#if:{{{lang|}}}|{{{lang}}}|en}}}} {{{poscat|}}}|{{SUBPAGENAME}}]]"
                + "{{#if:{{{script|}}}|[[Category:{{{{#if:{{{poscat|}}}|languagex|ns:0}}|{{#if:{{{lang|}}}|{{{lang}}}|en}}}} {{{poscat|}}} in "
                + "{{{script}}} script|{{SUBPAGENAME}}]]}}{{#if:{{{script2|}}}|[[Category:{{{{#if:{{{poscat|}}}|languagex|ns:0}}|{{#if:{{{lang|}}}"
                + "|{{{lang}}}|en}}}} {{{poscat|}}} in {{{script2}}} script|{{SUBPAGENAME}}]]}}}}}}",
                out element));
            template = (MediaWikiTemplate)element;
            Assert.AreEqual("#if:{{{poscat|}}}", template.Title);

            // ※ 解析結果の詳細はちょっとややこしすぎてテストできない・・・
            Assert.IsTrue(template.PipeTexts.Count > 0);
            Assert.IsNull(template.Interwiki);
            Assert.IsFalse(template.IsColon);
            Assert.IsFalse(template.IsMsgnw);
            Assert.IsFalse(template.NewLine);
        }

        #endregion
    }
}
