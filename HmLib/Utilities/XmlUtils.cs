// ================================================================================================
// <summary>
//      XMLの処理に関するユーティリティクラスソース。</summary>
//
// <copyright file="XmlUtils.cs" company="honeplusのメモ帳">
//      Copyright (C) 2013 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// XMLの処理に関するユーティリティクラスです。
    /// </summary>
    public static class XmlUtils
    {
        #region null値許容メソッド

        /// <summary>
        /// ノードが<c>null</c>の場合に空の文字列を返す<see cref="XmlNode.InnerText"/>。
        /// </summary>
        /// <param name="node"><see cref="XmlNode.InnerText"/>するノード。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<see cref="XmlNode.InnerText"/>した結果。<c>null</c>の場合には空の文字列。</returns>
        public static string InnerText(XmlNode node)
        {
            return XmlUtils.InnerText(node, string.Empty);
        }

        /// <summary>
        /// ノードが<c>null</c>の場合に指定された文字列を返す<see cref="XmlNode.InnerText"/>。
        /// </summary>
        /// <param name="node"><see cref="XmlNode.InnerText"/>するノード。<c>null</c>も可。</param>
        /// <param name="nullStr">渡されたノードが<c>null</c>の場合に返される文字列。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<see cref="XmlNode.InnerText"/>した結果。<c>null</c>の場合には指定された文字列。</returns>
        public static string InnerText(XmlNode node, string nullStr)
        {
            if (node == null)
            {
                return nullStr;
            }

            return StringUtils.DefaultString(node.InnerText, nullStr);
        }

        /// <summary>
        /// ノードが<c>null</c>の場合に空の文字列を返す<see cref="XmlNode.InnerXml"/>。
        /// </summary>
        /// <param name="node"><see cref="XmlNode.InnerXml"/>するノード。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<see cref="XmlNode.InnerXml"/>した結果。<c>null</c>の場合には空の文字列。</returns>
        public static string InnerXml(XmlNode node)
        {
            return XmlUtils.InnerXml(node, string.Empty);
        }

        /// <summary>
        /// ノードが<c>null</c>の場合に指定された文字列を返す<see cref="XmlNode.InnerXml"/>。
        /// </summary>
        /// <param name="node"><see cref="XmlNode.InnerXml"/>するノード。<c>null</c>も可。</param>
        /// <param name="nullStr">渡されたノードが<c>null</c>の場合に返される文字列。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<see cref="XmlNode.InnerXml"/>した結果。<c>null</c>の場合には指定された文字列。</returns>
        public static string InnerXml(XmlNode node, string nullStr)
        {
            if (node == null)
            {
                return nullStr;
            }

            return StringUtils.DefaultString(node.InnerXml, nullStr);
        }

        /// <summary>
        /// ノードが<c>null</c>の場合に空の文字列を返す<see cref="XmlNode.OuterXml"/>。
        /// </summary>
        /// <param name="node"><see cref="XmlNode.OuterXml"/>するノード。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<see cref="XmlNode.OuterXml"/>した結果。<c>null</c>の場合には空の文字列。</returns>
        public static string OuterXml(XmlNode node)
        {
            return XmlUtils.OuterXml(node, string.Empty);
        }

        /// <summary>
        /// ノードが<c>null</c>の場合に指定された文字列を返す<see cref="XmlNode.OuterXml"/>。
        /// </summary>
        /// <param name="node"><see cref="XmlNode.OuterXml"/>するノード。<c>null</c>も可。</param>
        /// <param name="nullStr">渡されたノードが<c>null</c>の場合に返される文字列。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<see cref="XmlNode.OuterXml"/>した結果。<c>null</c>の場合には指定された文字列。</returns>
        public static string OuterXml(XmlNode node, string nullStr)
        {
            if (node == null)
            {
                return nullStr;
            }

            return StringUtils.DefaultString(node.OuterXml, nullStr);
        }

        /// <summary>
        /// 属性が<c>null</c>の場合に空の文字列を返す<see cref="XAttribute.Value"/>。
        /// </summary>
        /// <param name="attribute"><see cref="XAttribute.Value"/>する属性。<c>null</c>も可。</param>
        /// <returns>渡された属性を<see cref="XAttribute.Value"/>した結果。<c>null</c>の場合には空の文字列。</returns>
        public static string Value(XAttribute attribute)
        {
            return XmlUtils.Value(attribute, string.Empty);
        }

        /// <summary>
        /// 属性が<c>null</c>の場合に指定された文字列を返す<see cref="XAttribute.Value"/>。
        /// </summary>
        /// <param name="attribute"><see cref="XAttribute.Value"/>する属性。<c>null</c>も可。</param>
        /// <param name="nullStr">渡された属性が<c>null</c>の場合に返される文字列。<c>null</c>も可。</param>
        /// <returns>渡された属性を<see cref="XAttribute.Value"/>した結果。<c>null</c>の場合には指定された文字列。</returns>
        public static string Value(XAttribute attribute, string nullStr)
        {
            if (attribute == null)
            {
                return nullStr;
            }

            return StringUtils.DefaultString(attribute.Value, nullStr);
        }

        #endregion

        #region エンコード／デコード

        /// <summary>
        /// 指定された文字列をXMLエンコードする。
        /// </summary>
        /// <param name="s">エンコードする文字列。</param>
        /// <returns>エンコードした文字列。</returns>
        /// <exception cref="ArgumentNullException">文字列が<c>null</c>。</exception>
        /// <remarks>
        /// 使う場所によってはエンコードが必要ない文字もあるが、汎用のため常時
        /// &lt;, &gt;, &quot;, &apos;, &amp; の5文字を変換する。
        /// </remarks>
        public static string XmlEncode(string s)
        {
            return Validate.NotNull(s, "s").Replace("&", "&amp;").Replace("<", "&lt;")
                .Replace(">", "&gt;").Replace("\"", "&quot;").Replace("\'", "&apos;");
        }

        /// <summary>
        /// 指定された文字列をXMLデコードする。
        /// </summary>
        /// <param name="s">エンコードされた文字列。</param>
        /// <returns>エンコードを解除した文字列。</returns>
        /// <exception cref="ArgumentNullException">文字列が<c>null</c>。</exception>
        /// <remarks>
        /// &lt;, &gt;, &quot;, &apos;, &amp; の5文字を変換する。
        /// </remarks>
        public static string XmlDecode(string s)
        {
            return Validate.NotNull(s, "s").Replace("&lt;", "<").Replace("&gt;", ">")
                .Replace("&quot;", "\"").Replace("&apos;", "\'").Replace("&amp;", "&");
        }

        #endregion
    }
}
