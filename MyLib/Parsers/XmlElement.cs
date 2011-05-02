// ================================================================================================
// <summary>
//      ページのXML要素をあらわすモデルクラスソース</summary>
//
// <copyright file="XmlElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using Honememo.Utilities;

    /// <summary>
    /// ページのXML要素をあらわすモデルクラスです。
    /// </summary>
    public class XmlElement : ListElement
    {
        #region private変数

        /// <summary>
        /// タグ名。
        /// </summary>
        private string name;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたタグ名・属性・値からXML要素を生成する。
        /// </summary>
        /// <param name="name">タグ名。</param>
        /// <param name="attributes">属性。</param>
        /// <param name="innerElements">値。</param>
        /// <param name="parsedString">Parse解析時の元の文字列。</param>
        public XmlElement(string name, IDictionary<string, string> attributes,
            ICollection<IElement> innerElements, string parsedString = null)
        {
            this.Name = name;
            this.ParsedString = parsedString;
            if (attributes != null)
            {
                this.Attributes = new Dictionary<string, string>(attributes);
            }
            else
            {
                this.Attributes = new Dictionary<string, string>();
            }

            if (innerElements != null)
            {
                this.AddRange(innerElements);
            }
        }

        /// <summary>
        /// 指定されたタグ名・値からXML要素を生成する。
        /// </summary>
        /// <param name="name">タグ名。</param>
        /// <param name="value">値。未指定時は<c>null</c>。</param>
        public XmlElement(string name, string value = null)
        {
            this.Name = name;
            this.Attributes = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(value))
            {
                this.Add(new TextElement(value));
            }
        }

        #endregion
        
        #region プロパティ

        /// <summary>
        /// タグ名。
        /// </summary>
        /// <exception cref="ArgumentNullException">タグ名がnullの場合。</exception>
        /// <exception cref="ArgumentException">タグ名が空の場合。</exception>
        public virtual string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = Validate.NotBlank(value);
            }
        }

        /// <summary>
        /// タグに含まれる属性情報。
        /// </summary>
        public virtual IDictionary<string, string> Attributes
        {
            get;
            protected set;
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 渡されたテキストをXML/HTMLタグとして解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <returns>解析したタグ。</returns>
        /// <exception cref="FormatException">文字列が解析できないフォーマットの場合。</exception>
        /// <remarks>
        /// XML/HTMLタグと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// </remarks>
        public static XmlElement ParseLazy(string s)
        {
            XmlElement result;
            if (XmlElement.TryParseLazy(s, out result))
            {
                return result;
            }

            throw new FormatException("Invalid String : " + s);
        }

        /// <summary>
        /// 渡されたテキストをXML/HTMLタグとして解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="parser">解析に使用するパーサー。</param>
        /// <param name="result">解析したタグ。</param>
        /// <returns>タグの場合<c>true</c>。</returns>
        /// <remarks>
        /// XML/HTMLタグと判定するには、1文字目が開始タグである必要がある。
        /// </remarks>
        public static bool TryParse(string s, XmlParser parser, out XmlElement result)
        {
            // XML/HTML要素の解析は複雑なため、こちらで行わず専用クラスに委譲する
            return parser.TryParseXmlElement(s, out result);
        }

        /// <summary>
        /// 渡されたテキストをXML/HTMLタグとして解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="result">解析したタグ。</param>
        /// <returns>タグの場合<c>true</c>。</returns>
        /// <remarks>
        /// XML/HTMLタグと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// </remarks>
        public static bool TryParseLazy(string s, out XmlElement result)
        {
            // パーサーにXmlParserの標準設定（Lazyな設定）を指定して解析
            return XmlElement.TryParse(s, new XmlParser(), out result);
        }

        /// <summary>
        /// 渡された文字が<c>TryParse</c>等の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。</returns>
        /// <remarks>性能対策などで処理自体を呼ばせたく無い場合用。</remarks>
        public static bool IsElementPossible(char c)
        {
            return '<' == c;
        }

        #endregion

        #region 内部実装メソッド

        /// <summary>
        /// このXML要素を表す文字列を返す。
        /// </summary>
        /// <returns>このXML要素を表す文字列。</returns>
        protected override string ToStringImpl()
        {
            StringBuilder b = new StringBuilder();
            XmlWriterSettings s = new XmlWriterSettings();
            s.CheckCharacters = false;
            s.ConformanceLevel = ConformanceLevel.Fragment;
            using (XmlWriter w = XmlWriter.Create(b, s))
            {
                w.WriteStartElement(this.Name);
                foreach (KeyValuePair<string, string> attr in this.Attributes)
                {
                    w.WriteAttributeString(attr.Key, attr.Value);
                }
                foreach (IElement element in this)
                {
                    // エンコードする／しないは中身の責任として、ここではエンコードしない
                    // ※ エンコードするテキストを用意したい場合はXmlTextElementを作るなど・・・
                    w.WriteRaw(element.ToString());
                }
                w.WriteEndElement();
            }

            return b.ToString();
        }

        #endregion
    }
}
