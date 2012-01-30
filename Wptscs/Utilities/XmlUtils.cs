// ================================================================================================
// <summary>
//      Xmlの処理に関するユーティリティクラスソース。</summary>
//
// <copyright file="XmlUtils.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Xml;

    /// <summary>
    /// Xmlの処理に関するユーティリティクラスです。
    /// </summary>
    public static class XmlUtils
    {
        #region null値許容メソッド

        /// <summary>
        /// ノードが<c>null</c>の場合に空の文字列を返す<c>InnerText</c>。
        /// </summary>
        /// <param name="node"><c>InnerText</c>するノード。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<c>InnerText</c>した結果。<c>null</c>の場合には空の文字列。</returns>
        public static string InnerText(XmlNode node)
        {
            return XmlUtils.InnerText(node, String.Empty);
        }

        /// <summary>
        /// ノードが<c>null</c>の場合に指定された文字列を返す<c>InnerText</c>。
        /// </summary>
        /// <param name="node"><c>InnerText</c>するノード。<c>null</c>も可。</param>
        /// <param name="nullStr">渡されたノードが<c>null</c>の場合に返される文字列。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<c>InnerText</c>した結果。<c>null</c>の場合には指定された文字列。</returns>
        public static string InnerText(XmlNode node, string nullStr)
        {
            if (node == null)
            {
                return nullStr;
            }

            return StringUtils.DefaultString(node.InnerText, nullStr);
        }

        /// <summary>
        /// ノードが<c>null</c>の場合に空の文字列を返す<c>InnerXml</c>。
        /// </summary>
        /// <param name="node"><c>InnerXml</c>するノード。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<c>InnerXml</c>した結果。<c>null</c>の場合には空の文字列。</returns>
        public static string InnerXml(XmlNode node)
        {
            return XmlUtils.InnerXml(node, String.Empty);
        }

        /// <summary>
        /// ノードが<c>null</c>の場合に指定された文字列を返す<c>InnerXml</c>。
        /// </summary>
        /// <param name="node"><c>InnerXml</c>するノード。<c>null</c>も可。</param>
        /// <param name="nullStr">渡されたノードが<c>null</c>の場合に返される文字列。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<c>InnerXml</c>した結果。<c>null</c>の場合には指定された文字列。</returns>
        public static string InnerXml(XmlNode node, string nullStr)
        {
            if (node == null)
            {
                return nullStr;
            }

            return StringUtils.DefaultString(node.InnerXml, nullStr);
        }

        /// <summary>
        /// ノードが<c>null</c>の場合に空の文字列を返す<c>OuterXml</c>。
        /// </summary>
        /// <param name="node"><c>OuterXml</c>するノード。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<c>OuterXml</c>した結果。<c>null</c>の場合には空の文字列。</returns>
        public static string OuterXml(XmlNode node)
        {
            return XmlUtils.OuterXml(node, String.Empty);
        }

        /// <summary>
        /// ノードが<c>null</c>の場合に指定された文字列を返す<c>OuterXml</c>。
        /// </summary>
        /// <param name="node"><c>OuterXml</c>するノード。<c>null</c>も可。</param>
        /// <param name="nullStr">渡されたノードが<c>null</c>の場合に返される文字列。<c>null</c>も可。</param>
        /// <returns>渡されたノードを<c>OuterXml</c>した結果。<c>null</c>の場合には指定された文字列。</returns>
        public static string OuterXml(XmlNode node, string nullStr)
        {
            if (node == null)
            {
                return nullStr;
            }

            return StringUtils.DefaultString(node.OuterXml, nullStr);
        }

        #endregion
    }
}
