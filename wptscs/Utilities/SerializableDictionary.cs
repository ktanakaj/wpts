// ================================================================================================
// <summary>
//      シリアライズ可能なDictionaryクラスソース</summary>
//
// <copyright file="SerializableDictionary.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// <remarks>http://d.hatena.ne.jp/lord_hollow/20090206 を参考に作成させていただいたソース。</remarks>
// 
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// シリアライズ可能なDictionaryクラスです。
    /// </summary>
    /// <typeparam name="Tkey">ディクショナリ内のキーの型。</typeparam>
    /// <typeparam name="Tvalue">ディクショナリ内の値の型。</typeparam>
    public class SerializableDictionary<Tkey, Tvalue> : Dictionary<Tkey, Tvalue>, IXmlSerializable
    {
        #region メソッド

        /// <summary>
        /// シリアライズするXMLのスキーマ定義を返す。
        /// </summary>
        /// <returns>XML表現を記述するXmlSchema。</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// XMLからオブジェクトをデシリアライズする。
        /// </summary>
        /// <param name="reader">デシリアライズ元のXmlReader</param>
        public void ReadXml(XmlReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(KeyValue));
            reader.Read();
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                KeyValue kv = serializer.Deserialize(reader) as KeyValue;
                if (kv != null)
                {
                    Add(kv.Key, kv.Value);
                }
            }

            reader.Read();
        }

        /// <summary>
        /// オブジェクトをXMLにシリアライズする。
        /// </summary>
        /// <param name="writer">シリアライズ先のXmlWriter</param>
        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(KeyValue));
            foreach (Tkey key in Keys)
            {
                serializer.Serialize(writer, new KeyValue(key, this[key]));
            }
        }
        
        #endregion

        #region 内部クラス

        /// <summary>
        /// Dictionaryのキー・値を格納するためのクラスです。
        /// </summary>
        public class KeyValue
        {
            #region 内部クラスprivate変数

            /// <summary>
            /// Dictionaryのキー。
            /// </summary>
            private Tkey key;

            /// <summary>
            /// Dictionaryの値。
            /// </summary>
            private Tvalue value;

            #endregion

            #region 内部クラスコンストラクタ

            /// <summary>
            /// コンストラクタ。
            /// </summary>
            public KeyValue()
            {
            }

            /// <summary>
            /// コンストラクタ（キー・値を指定）。
            /// </summary>
            /// <param name="key">キー</param>
            /// <param name="value">値</param>
            public KeyValue(Tkey key, Tvalue value)
            {
                this.Key = key;
                this.Value = value;
            }

            #endregion

            #region 内部クラスプロパティ

            /// <summary>
            /// Dictionaryのキー。
            /// </summary>
            public Tkey Key
            {
                get
                {
                    return this.key;
                }

                set
                {
                    this.key = value;
                }
            }

            /// <summary>
            /// Dictionaryの値。
            /// </summary>
            public Tvalue Value
            {
                get
                {
                    return this.value;
                }

                set
                {
                    this.value = value;
                }
            }

            #endregion
        }

        #endregion
    }
}
