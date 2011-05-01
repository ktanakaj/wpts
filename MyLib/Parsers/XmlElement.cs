// ================================================================================================
// <summary>
//      ページのXML/HTML要素をあらわすモデルクラスソース</summary>
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
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Honememo.Utilities;

    /// <summary>
    /// ページのXML/HTML要素をあらわすモデルクラスです。
    /// </summary>
    /// <remarks>
    /// ※ この要素はその性質上Parse元と完全に同じ文字列を生成できない。
    ///    内容を厳密に保持する必要がある場合は代替手段を用いるなど注意を。
    /// </remarks>
    public class XmlElement : ListElement
    {
        #region コンストラクタ

        public XmlElement(string name, IDictionary<string, string> attributes, ICollection<IElement> innerElements)
        {
            this.Name = name;
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
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// タグに含まれる属性情報。
        /// </summary>
        public IDictionary<string, string> Attributes
        {
            get;
            protected set;
        }

        #endregion

        #region 静的メソッド

        public static bool TryParseLazy(string s, IParser parser, out XmlElement result)
        {
            // XML/HTML要素の解析は複雑なため、こちらで行わず専用クラスに委譲する
            LazyXmlParser p = parser as LazyXmlParser;
            if (p == null)
            {
                p = new LazyXmlParser();
            }

            return p.TryParseTag(s, out result);
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

            // 開始タグ
            b.Append('<');
            b.Append(this.Name);


            return "";
        }

        #endregion
    }
}
