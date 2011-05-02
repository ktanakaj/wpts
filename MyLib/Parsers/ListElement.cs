// ================================================================================================
// <summary>
//      ページ要素を複数格納する要素をあらわすモデルクラスソース</summary>
//
// <copyright file="ListElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// ページ要素を複数格納する要素をあらわすモデルクラスです。
    /// </summary>
    /// <remarks>テキストを扱うだけの単純なページ要素。</remarks>
    public class ListElement : List<IElement>, IElement
    {
        #region インタフェース実装プロパティ

        /// <summary>
        /// この要素の文字数。
        /// </summary>
        public virtual int Length
        {
            get
            {
                int length = 0;
                foreach (IElement element in this)
                {
                    length += element.Length;
                }

                return length;
            }
        }

        #endregion

        #region インタフェース実装メソッド

        /// <summary>
        /// この要素に格納されている要素のToStringを連結して返す。
        /// </summary>
        /// <returns>この要素に格納されている要素のテキスト。</returns>
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            foreach (IElement element in this)
            {
                b.Append(element.ToString());
            }

            return b.ToString();
        }

        #endregion
    }
}
