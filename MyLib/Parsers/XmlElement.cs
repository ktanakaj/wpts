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
    /// <remarks>
    /// ※ この要素はその性質上Parse元と完全に同じ文字列を生成できない。
    ///    内容を厳密に保持する必要がある場合は代替手段を用いるなど注意を。
    /// </remarks>
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
        /// <param name="original">Parse解析時の元の文字列。</param>
        public XmlElement(string name, IDictionary<string, string> attributes, ICollection<IElement> innerElements, string original = null)
        {
            this.Name = name;
            this.Original = original;
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

        #region インタフェース実装プロパティ

        /// <summary>
        /// この要素の文字数。
        /// </summary>
        public override int Length
        {
            get
            {
                return this.Original != null ? this.Original.Length : this.ToString().Length;
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

        /// <summary>
        /// Parse等によりインスタンスを生成した場合の元文字列。
        /// </summary>
        protected virtual string Original
        {
            // ※ 本当はParseしてから値が変更されていない場合、
            //    ToStringでこの値を返すとしたいが、
            //    子要素があるのでこのクラスでは実現困難。
            //    継承クラスのために、Parseした結果を入れておく。
            get;
            set;
        }

        #endregion

        #region 静的メソッド

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

        #endregion

        #region インタフェース実装メソッド

        /// <summary>
        /// このXML要素を表す文字列を返す。
        /// </summary>
        /// <returns>このXML要素を表す文字列。</returns>
        public override string ToString()
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
