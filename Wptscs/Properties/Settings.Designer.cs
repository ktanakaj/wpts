﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Honememo.Wptscs.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Translation Support for Wikipedia/{0}.{1:D2}")]
        public string DefaultUserAgent {
            get {
                return ((string)(this["DefaultUserAgent"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://{0}.wikipedia.org")]
        public string WikipediaLocation {
            get {
                return ((string)(this["WikipediaLocation"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SaveDirectory {
            get {
                return ((string)(this["SaveDirectory"]));
            }
            set {
                this["SaveDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("en")]
        public string LastSelectedSource {
            get {
                return ((string)(this["LastSelectedSource"]));
            }
            set {
                this["LastSelectedSource"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LastSelectedTarget {
            get {
                return ((string)(this["LastSelectedTarget"]));
            }
            set {
                this["LastSelectedTarget"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string UserAgent {
            get {
                return ((string)(this["UserAgent"]));
            }
            set {
                this["UserAgent"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Referer {
            get {
                return ((string)(this["Referer"]));
            }
            set {
                this["Referer"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/w/api.php?action=query&prop=revisions&titles=$1&redirects&rvprop=timestamp|conte" +
            "nt&format=xml")]
        public string MediaWikiContentApi {
            get {
                return ((string)(this["MediaWikiContentApi"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(" ($1) ")]
        public string Bracket {
            get {
                return ((string)(this["Bracket"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int MediaWikiTemplateNamespace {
            get {
                return ((int)(this["MediaWikiTemplateNamespace"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("14")]
        public int MediaWikiCategoryNamespace {
            get {
                return ((int)(this["MediaWikiCategoryNamespace"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6")]
        public int MediaWikiFileNamespace {
            get {
                return ((int)(this["MediaWikiFileNamespace"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15.00:00:00")]
        public global::System.TimeSpan CacheExpire {
            get {
                return ((global::System.TimeSpan)(this["CacheExpire"]));
            }
            set {
                this["CacheExpire"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1.20.0.0")]
        public string ConfigurationCompatible {
            get {
                return ((string)(this["ConfigurationCompatible"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/w/api.php?format=xml&action=query&meta=siteinfo&siprop=namespaces|namespacealias" +
            "es|interwikimap")]
        public string MediaWikiMetaApi {
            get {
                return ((string)(this["MediaWikiMetaApi"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsUpgraded {
            get {
                return ((bool)(this["IsUpgraded"]));
            }
            set {
                this["IsUpgraded"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IgnoreError {
            get {
                return ((bool)(this["IgnoreError"]));
            }
            set {
                this["IgnoreError"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int MaxConnectRetries {
            get {
                return ((int)(this["MaxConnectRetries"]));
            }
            set {
                this["MaxConnectRetries"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2000")]
        public int ConnectRetryTime {
            get {
                return ((int)(this["ConnectRetryTime"]));
            }
            set {
                this["ConnectRetryTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LastSelectedLanguage {
            get {
                return ((string)(this["LastSelectedLanguage"]));
            }
            set {
                this["LastSelectedLanguage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Wikipedia")]
        public string LastSelectedConfiguration {
            get {
                return ((string)(this["LastSelectedConfiguration"]));
            }
            set {
                this["LastSelectedConfiguration"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".xml")]
        public string ConfigurationExtension {
            get {
                return ((string)(this["ConfigurationExtension"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/w/api.php?action=query&prop=langlinks&titles=$1&redirects=&lllimit=500&format=xm" +
            "l")]
        public string MediaWikiInterlanguageApi {
            get {
                return ((string)(this["MediaWikiInterlanguageApi"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>CURRENTYEAR</string>\r\n  <string>CURRENTMONTH</string>\r\n  <string>CURRENTMO" +
            "NTHNAME</string>\r\n  <string>CURRENTMONTHNAMEGEN</string>\r\n  <string>CURRENTMONTH" +
            "ABBREV</string>\r\n  <string>CURRENTDAY</string>\r\n  <string>CURRENTDAY2</string>\r\n" +
            "  <string>CURRENTDOW</string>\r\n  <string>CURRENTDAYNAME</string>\r\n  <string>CURR" +
            "ENTTIME</string>\r\n  <string>CURRENTHOUR</string>\r\n  <string>CURRENTWEEK</string>" +
            "\r\n  <string>CURRENTTIMESTAMP</string>\r\n  <string>LOCALYEAR</string>\r\n  <string>L" +
            "OCALMONTH</string>\r\n  <string>LOCALMONTHNAME</string>\r\n  <string>LOCALMONTHNAMEG" +
            "EN</string>\r\n  <string>LOCALMONTHABBREV</string>\r\n  <string>LOCALDAY</string>\r\n " +
            " <string>LOCALDAY2</string>\r\n  <string>LOCALDOW</string>\r\n  <string>LOCALDAYNAME" +
            "</string>\r\n  <string>LOCALTIME</string>\r\n  <string>LOCALHOUR</string>\r\n  <string" +
            ">LOCALWEEK</string>\r\n  <string>LOCALTIMESTAMP</string>\r\n  <string>SITENAME</stri" +
            "ng>\r\n  <string>SERVER</string>\r\n  <string>SERVERNAME</string>\r\n  <string>DIRMARK" +
            "</string>\r\n  <string>DIRECTIONMARK</string>\r\n  <string>SCRIPTPATH</string>\r\n  <s" +
            "tring>STYLEPATH</string>\r\n  <string>CURRENTVERSION</string>\r\n  <string>CONTENTLA" +
            "NGUAGE</string>\r\n  <string>CONTENTLANG</string>\r\n  <string>REVISIONID</string>\r\n" +
            "  <string>REVISIONDAY</string>\r\n  <string>REVISIONDAY2</string>\r\n  <string>REVIS" +
            "IONMONTH</string>\r\n  <string>REVISIONYEAR</string>\r\n  <string>REVISIONTIMESTAMP<" +
            "/string>\r\n  <string>REVISIONUSER</string>\r\n  <string>PAGESIZE</string>\r\n  <strin" +
            "g>PROTECTIONLEVEL</string>\r\n  <string>DISPLAYTITLE</string>\r\n  <string>DEFAULTSO" +
            "RT</string>\r\n  <string>DEFAULTSORTKEY</string>\r\n  <string>DEFAULTCATEGORYSORT</s" +
            "tring>\r\n  <string>NUMBEROFPAGES</string>\r\n  <string>NUMBEROFARTICLES</string>\r\n " +
            " <string>NUMBEROFFILES</string>\r\n  <string>NUMBEROFEDITS</string>\r\n  <string>NUM" +
            "BEROFVIEWS</string>\r\n  <string>NUMBEROFUSERS</string>\r\n  <string>NUMBEROFADMINS<" +
            "/string>\r\n  <string>NUMBEROFACTIVEUSERS</string>\r\n  <string>PAGESINCATEGORY</str" +
            "ing>\r\n  <string>PAGESINCAT</string>\r\n  <string>NUMBERINGROUP</string>\r\n  <string" +
            ">NUMINGROUP</string>\r\n  <string>PAGESINNS</string>\r\n  <string>PAGESINNAMESPACE</" +
            "string>\r\n  <string>FULLPAGENAME</string>\r\n  <string>PAGENAME</string>\r\n  <string" +
            ">BASEPAGENAME</string>\r\n  <string>SUBPAGENAME</string>\r\n  <string>SUBJECTPAGENAM" +
            "E</string>\r\n  <string>TALKPAGENAME</string>\r\n  <string>FULLPAGENAMEE</string>\r\n " +
            " <string>PAGENAMEE</string>\r\n  <string>BASEPAGENAMEE</string>\r\n  <string>SUBPAGE" +
            "NAMEE</string>\r\n  <string>SUBJECTPAGENAMEE</string>\r\n  <string>TALKPAGENAMEE</st" +
            "ring>\r\n  <string>NAMESPACE</string>\r\n  <string>SUBJECTSPACE</string>\r\n  <string>" +
            "ARTICLESPACE</string>\r\n  <string>TALKSPACE</string>\r\n  <string>NAMESPACEE</strin" +
            "g>\r\n  <string>SUBJECTSPACEE</string>\r\n  <string>TALKSPACEE</string>\r\n  <string>l" +
            "ocalurl</string>\r\n  <string>fullurl</string>\r\n  <string>filepath</string>\r\n  <st" +
            "ring>urlencode</string>\r\n  <string>anchorencode</string>\r\n  <string>ns</string>\r" +
            "\n  <string>lc</string>\r\n  <string>lcfirst</string>\r\n  <string>uc</string>\r\n  <st" +
            "ring>ucfirst</string>\r\n  <string>formatnum</string>\r\n  <string>#dateformat</stri" +
            "ng>\r\n  <string>#formatdate</string>\r\n  <string>padleft</string>\r\n  <string>padri" +
            "ght</string>\r\n  <string>plural</string>\r\n  <string>grammar</string>\r\n  <string>i" +
            "nt</string>\r\n  <string>#language</string>\r\n  <string>#special</string>\r\n  <strin" +
            "g>#tag</string>\r\n  <string>gender</string>\r\n  <string>groupconvert</string>\r\n  <" +
            "string>#expr</string>\r\n  <string>#if</string>\r\n  <string>#ifeq</string>\r\n  <stri" +
            "ng>#ifexist</string>\r\n  <string>#ifexpr</string>\r\n  <string>#switch</string>\r\n  " +
            "<string>#time</string>\r\n  <string>#rel2abs</string>\r\n  <string>#titleparts</stri" +
            "ng>\r\n  <string>#iferror</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection MediaWikiMagicWords {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["MediaWikiMagicWords"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>acronym</string>\r\n  <string>advogato</string>\r\n  <string>annotationwiki</s" +
            "tring>\r\n  <string>arxiv</string>\r\n  <string>c2find</string>\r\n  <string>cache</st" +
            "ring>\r\n  <string>commons</string>\r\n  <string>corpknowpedia</string>\r\n  <string>d" +
            "ictionary</string>\r\n  <string>disinfopedia</string>\r\n  <string>docbook</string>\r" +
            "\n  <string>doi</string>\r\n  <string>drumcorpswiki</string>\r\n  <string>dwjwiki</st" +
            "ring>\r\n  <string>emacswiki</string>\r\n  <string>elibre</string>\r\n  <string>foldoc" +
            "</string>\r\n  <string>foxwiki</string>\r\n  <string>freebsdman</string>\r\n  <string>" +
            "gej</string>\r\n  <string>gentoo-wiki</string>\r\n  <string>google</string>\r\n  <stri" +
            "ng>googlegroups</string>\r\n  <string>hammondwiki</string>\r\n  <string>hewikisource" +
            "</string>\r\n  <string>hrwiki</string>\r\n  <string>imdb</string>\r\n  <string>jargonf" +
            "ile</string>\r\n  <string>jspwiki</string>\r\n  <string>keiki</string>\r\n  <string>km" +
            "wiki</string>\r\n  <string>linuxwiki</string>\r\n  <string>lojban</string>\r\n  <strin" +
            "g>lqwiki</string>\r\n  <string>lugkr</string>\r\n  <string>mathsongswiki</string>\r\n " +
            " <string>meatball</string>\r\n  <string>mediazilla</string>\r\n  <string>mediawikiwi" +
            "ki</string>\r\n  <string>memoryalpha</string>\r\n  <string>metawiki</string>\r\n  <str" +
            "ing>metawikipedia</string>\r\n  <string>moinmoin</string>\r\n  <string>mozillawiki</" +
            "string>\r\n  <string>mw</string>\r\n  <string>oeis</string>\r\n  <string>openfacts</st" +
            "ring>\r\n  <string>openwiki</string>\r\n  <string>patwiki</string>\r\n  <string>pmeg</" +
            "string>\r\n  <string>ppr</string>\r\n  <string>pythoninfo</string>\r\n  <string>rfc</s" +
            "tring>\r\n  <string>s23wiki</string>\r\n  <string>seattlewiki</string>\r\n  <string>se" +
            "attlewireless</string>\r\n  <string>senseislibrary</string>\r\n  <string>slashdot</s" +
            "tring>\r\n  <string>sourceforge</string>\r\n  <string>squeak</string>\r\n  <string>sus" +
            "ning</string>\r\n  <string>svgwiki</string>\r\n  <string>tavi</string>\r\n  <string>te" +
            "jo</string>\r\n  <string>tmbw</string>\r\n  <string>tmnet</string>\r\n  <string>tmwiki" +
            "</string>\r\n  <string>theopedia</string>\r\n  <string>twiki</string>\r\n  <string>uea" +
            "</string>\r\n  <string>unreal</string>\r\n  <string>usemod</string>\r\n  <string>vinis" +
            "mo</string>\r\n  <string>webseitzwiki</string>\r\n  <string>why</string>\r\n  <string>" +
            "wiki</string>\r\n  <string>wikia</string>\r\n  <string>wikibooks</string>\r\n  <string" +
            ">wikicities</string>\r\n  <string>wikif1</string>\r\n  <string>wikihow</string>\r\n  <" +
            "string>wikinfo</string>\r\n  <string>wikimedia</string>\r\n  <string>wikinews</strin" +
            "g>\r\n  <string>wikiquote</string>\r\n  <string>wikipedia</string>\r\n  <string>wikiso" +
            "urce</string>\r\n  <string>wikispecies</string>\r\n  <string>wikitravel</string>\r\n  " +
            "<string>wikiversity</string>\r\n  <string>wikt</string>\r\n  <string>wiktionary</str" +
            "ing>\r\n  <string>wlug</string>\r\n  <string>zwiki</string>\r\n  <string>zzz wiki</str" +
            "ing>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection MediaWikiInterwikiPrefixs {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["MediaWikiInterwikiPrefixs"]));
            }
        }
    }
}
