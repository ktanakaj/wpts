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
    /// <remarks>解析処理は複雑なため、<see cref="XmlParser"/>として別途実装。</remarks>
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
        public XmlElement(
            string name,
            IDictionary<string, string> attributes,
            ICollection<IElement> innerElements,
            string parsedString = null)
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
            if (!string.IsNullOrEmpty(value))
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
                    // ※ エンコードするテキストを用意したい場合はXmlTextElementを使うなど
                    w.WriteRaw(element.ToString());
                }

                w.WriteEndElement();
            }

            return b.ToString();
        }

        #endregion
    }
}
