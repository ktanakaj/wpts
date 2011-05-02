// ================================================================================================
// <summary>
//      ページや文字列の各要素をあらわすモデルインタフェースソース</summary>
//
// <copyright file="IElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    /// <summary>
    /// ページや文字列の各要素をあらわすモデルのインタフェースです。
    /// </summary>
    public interface IElement
    {
        #region プロパティ

        /// <summary>
        /// 要素がParse等により生成された場合の解析元文字列。
        /// </summary>
        /// <remarks>
        /// <see cref="ToString"/>に生成した値を返して欲しい場合、この値を明示的に<c>null</c>にすべき。
        /// 元の文字列と完全に同じ文字列を生成できるクラスであれば、常に未設定でも問題ない。
        /// </remarks>
        string ParsedString
        {
            get;
            set;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 各要素を書式化したテキスト形式で返す。
        /// </summary>
        /// <returns>書式化したテキスト。<c>null</c>は返さないこと。</returns>
        /// <remarks>
        /// Parse系の処理を実装する上で元の文字列が必要なため、
        /// <see cref="ParsedString"/>が設定されている場合は、その値を返すべき。
        /// </remarks>
        string ToString();

        #endregion
    }
}
