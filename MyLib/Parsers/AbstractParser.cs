// ================================================================================================
// <summary>
//      IParserを実装するための実装支援用抽象クラスソース</summary>
//
// <copyright file="AbstractParser.cs" company="honeplusのメモ帳">
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
    /// IParserを実装するための実装支援用抽象クラスです。
    /// </summary>
    public abstract class AbstractParser : IParser
    {        
        #region インタフェース実装メソッド

        /// <summary>
        /// 渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <returns>解析結果。</returns>
        /// <exception cref="FormatException">文字列が解析できないフォーマットの場合。</exception>
        /// <remarks><see cref="TryParse"/>を呼び出し。解析に失敗した場合は、各種例外を投げる。</remarks>
        public virtual IElement Parse(string s)
        {
            IElement result;
            if (this.TryParse(s, out result))
            {
                return result;
            }

            throw new FormatException("Invalid String : " + s);
        }

        /// <summary>
        /// 渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析結果。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        /// <remarks>
        /// 実装として <see cref="IsElementPossible"/>, <see cref="TryParseElements"/> を呼び出し。
        /// </remarks>
        public virtual bool TryParse(string s, out IElement result)
        {
            // 文字列を1文字ずつチェックし、その内容に応じた要素のリストを作成する
            ListElement list = new ListElement();
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                // 性能を考え、TryParseの前に処理対象になりそうかをチェック
                if (this.IsElementPossible(s, i))
                {
                    // 各要素のTryParse処理を呼び出し
                    IElement innerElement;
                    if (this.TryParseElements(s.Substring(i), out innerElement))
                    {
                        // それまでに解析済みのテキストを吐き出し、
                        // その後に解析した要素を追加
                        this.FlashText(ref list, ref b);
                        list.Add(innerElement);
                        i += innerElement.ToString().Length - 1;
                        continue;
                    }
                }

                // 通常の文字列はテキスト要素として積み上げる
                b.Append(s[i]);
            }

            // 残っていれば最後に解析済みのテキストを吐き出し
            this.FlashText(ref list, ref b);

            result = list;
            if (list.Count == 1)
            {
                // リストが1件であれば、その要素を直に返す
                result = list[0];
            }
            else if (list.Count == 0)
            {
                // 何もなければ、空文字列だったものとして空のテキスト要素を返す
                result = new TextElement();
            }

            return true;
        }

        #endregion

        #region 実装支援用抽象メソッド

        /// <summary>
        /// 渡された位置の文字列が<see cref="TryParseElements"/>の候補となるかを判定する。
        /// </summary>
        /// <param name="s">全体文字列。</param>
        /// <param name="index">処理インデックス。</param>
        /// <returns>候補となる場合<c>true</c>。このクラスでは常に<c>true</c>を返す。</returns>
        /// <remarks>
        /// 通常<see cref="TryParseElements"/>でも判定するため、特に定義せずとも問題ないが、
        /// 性能対策などで処理自体を呼ばせたく無い場合のために定義。
        /// </remarks>
        protected virtual bool IsElementPossible(string s, int index)
        {
            return true;
        }

        /// <summary>
        /// 渡されたテキストを各種解析処理で解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="result">解析した結果要素。</param>
        /// <returns>解析できた場合<c>true</c>。</returns>
        /// <remarks>
        /// ここでそのParserで必要な解析処理呼び出しを列挙する。
        /// </remarks>
        protected abstract bool TryParseElements(string s, out IElement result);

        /// <summary>
        /// 文字列が空でない場合、リストにTextエレメントを追加して、文字列をリセットする。
        /// </summary>
        /// <param name="list">追加されるリスト。</param>
        /// <param name="b">追加する文字列。</param>
        protected virtual void FlashText(ref ListElement list, ref StringBuilder b)
        {
            if (b.Length > 0)
            {
                list.Add(new TextElement(b.ToString()));
                b.Clear();
            }
        }

        #endregion
    }
}
