package jp.honeplus.wpts.model;

import java.util.Date;

/**
 * Wikipediaの記事を表すクラス。
 * @author HonePlus
 * @version $Id$
 */
public class Article {

	/** 記事のタイトル。 */
	private String title;
	/** 記事の更新日時。 */
	private Date timestamp;
	/** 記事の本文。 */
	private String text;
	/** 記事が所属するサイト。 */
	private Site site;

	/**
	 * 記事のタイトルを取得。
	 * @return 記事のタイトル
	 */
	public String getTitle() {
		return title;
	}
	/**
	 * 記事のタイトルを設定。
	 * @param title 記事のタイトル
	 */
	public void setTitle(final String title) {
		this.title = title;
	}

	/**
	 * 記事の更新日時を取得。
	 * @return 記事の更新日時
	 */
	public Date getTimestamp() {
		return timestamp != null ? new Date(timestamp.getTime()) : null;
	}
	/**
	 * 記事の更新日時を設定。
	 * @param timestamp 記事の更新日時
	 */
	public void setTimestamp(final Date timestamp) {
		this.timestamp = timestamp != null ? new Date(timestamp.getTime()) : null;
	}

	/**
	 * 記事の本文を取得。
	 * @return 記事の本文
	 */
	public String getText() {
		return text;
	}
	/**
	 * 記事の本文を設定。
	 * @param text 記事の本文
	 */
	public void setText(final String text) {
		this.text = text;
	}

	/**
	 * 記事が属するサイトを取得。
	 * @return 記事が属するサイト
	 */
	public Site getSite() {
		return site;
	}
	/**
	 * 記事が属するサイトを設定。
	 * @param site 記事が属するサイト
	 */
	public void setSite(final Site site) {
		this.site = site;
	}

	/**
	 * 記事がリダイレクトかを判定する。
	 * @return <code>true</code> リダイレクト
	 */
	public boolean isRedirect() {
		//TODO: 未実装
		return false;
	}
	/**
	 * 記事がカテゴリーかを判定する。
	 * @return <code>true</code> カテゴリー
	 */
	public boolean isCategory() {
		//TODO: 未実装
		return false;
	}
	/**
	 * 記事が画像などのファイルかを判定する。
	 * @return <code>true</code> 画像などのファイル
	 */
	public boolean isImage() {
		//TODO: 未実装
		return false;
	}
}
