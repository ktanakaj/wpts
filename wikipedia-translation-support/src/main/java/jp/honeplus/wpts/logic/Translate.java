package jp.honeplus.wpts.logic;

/**
 * 翻訳支援処理ロジッククラス。
 * @author Honeplus
 * @version $Id$
 */
public class Translate {

	/** 翻訳元言語。 */
	private String fromLang;
	/** 翻訳先言語。 */
	private String toLang;
	/** 翻訳する記事名。 */
	private String title;
	/** 処理ログ。 */
	private StringBuffer log;

	/**
	 * 翻訳元言語を取得する。
	 * @return 翻訳元言語
	 */
	public String getFromLang() {
		return fromLang;
	}
	/**
	 * 翻訳元言語を設定する。
	 * @param fromLang 翻訳元言語
	 */
	public void setFromLang(final String fromLang) {
		this.fromLang = fromLang;
	}

	/**
	 * 翻訳先言語を取得する。
	 * @return 翻訳先言語
	 */
	public String getToLang() {
		return toLang;
	}
	/**
	 * 翻訳先言語を設定する。
	 * @param toLang 翻訳先言語
	 */
	public void setToLang(final String toLang) {
		this.toLang = toLang;
	}

	/**
	 * 翻訳する記事名を取得する。
	 * @return 翻訳する記事名
	 */
	public String getTitle() {
		return title;
	}
	/**
	 * 翻訳する記事名を設定する。
	 * @param title 翻訳する記事名
	 */
	public void setTitle(final String title) {
		this.title = title;
	}

	/**
	 * 処理ログを取得する。
	 * @return 処理ログ
	 * @throws IllegalStateException {@link #run} が実行されていない場合
	 */
	public String getLog() {
		if (log == null) {
			throw new IllegalStateException("Not runnig");
		}
		return log.toString();
	}

	/**
	 * 翻訳支援処理を実施する。
	 */
	public void run() {
		//TODO: 未実装
		// 初期化
		log = new StringBuffer();
	}
}
