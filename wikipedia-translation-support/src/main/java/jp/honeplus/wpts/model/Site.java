package jp.honeplus.wpts.model;

import java.util.List;

/**
 * Wikipediaのサイトを表すクラス。
 * @author Honeplus
 * @version $Id$
 */
public class Site {

	/** サイトの言語。 */
	private String lang;
	/** サイトのネームスペース。 */
	private List<String> namespaces;

	/**
	 * サイトの言語を取得する。
	 * @return サイトの言語
	 */
	public String getLang() {
		return lang;
	}
	/**
	 * サイトの言語を設定する。
	 * @param lang サイトの言語
	 */
	public void setLang(final String lang) {
		this.lang = lang;
	}

	/**
	 * サイトのネームスペースを取得する。
	 * @return サイトのネームスペース
	 */
	public List<String> getNamespaces() {
		return namespaces;
	}
	/**
	 * サイトのネームスペースを設定する。
	 * @param namespaces サイトのネームスペース
	 */
	public void setNamespaces(final List<String> namespaces) {
		this.namespaces = namespaces;
	}

	/**
	 * サイトより記事を取得する。
	 * @param title 記事名
	 * @return 取得した記事。存在しない場合は <code>null</code>
	 */
	public Article getArticle(final String title) {
		//TODO: 未実装
		return null;
	}
}
