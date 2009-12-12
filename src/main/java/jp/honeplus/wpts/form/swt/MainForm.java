package jp.honeplus.wpts.form.swt;

import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.widgets.Shell;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Group;
import org.eclipse.swt.SWT;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.widgets.Combo;
import org.eclipse.swt.widgets.Label;
import org.eclipse.swt.widgets.Link;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Text;

/**
 * Wikipedia翻訳支援ツールメイン画面SWT版。
 * @author Honeplus
 * @version $Id$
 */
public class MainForm {

	private Shell sShell = null;
	private Group groupTransfer = null;
	private Group groupSaveDirectory = null;
	private Group groupRun = null;
	private Combo comboSource = null;
	private Label labelSource = null;
	private Link linkSourceURL = null;
	private Label labelArrow = null;
	private Combo comboTarget = null;
	private Label labelTarget = null;
	private Button buttonConfig = null;
	private Button buttonSaveDirectory = null;
	private Text textSaveDirectory = null;
	private Label labelArticle = null;
	private Text textArticle = null;
	private Button buttonRun = null;
	private Button buttonStop = null;
	private Text textLog = null;

	/**
	 * This method initializes groupTransfer	
	 *
	 */
	private void createGroupTransfer() {
		GridData gridData5 = new GridData();
		gridData5.widthHint = 200;
		GridData gridData4 = new GridData();
		gridData4.widthHint = 100;
		GridData gridData3 = new GridData();
		gridData3.widthHint = 100;
		GridLayout gridLayout = new GridLayout();
		gridLayout.numColumns = 3;
		groupTransfer = new Group(sShell, SWT.NONE);
		createComboSource();
		groupTransfer.setLayout(gridLayout);
		groupTransfer.setText("翻訳元→先の言語を設定");
		labelSource = new Label(groupTransfer, SWT.NONE);
		labelSource.setText("");
		labelSource.setLayoutData(gridData3);
		linkSourceURL = new Link(groupTransfer, SWT.NONE);
		linkSourceURL.setText("<a>http://</a>");
		linkSourceURL.setLayoutData(gridData5);
		Label filler = new Label(groupTransfer, SWT.NONE);
		labelArrow = new Label(groupTransfer, SWT.NONE);
		labelArrow.setText("↓");
		Label filler1 = new Label(groupTransfer, SWT.NONE);
		createComboTarget();
		labelTarget = new Label(groupTransfer, SWT.NONE);
		labelTarget.setText("");
		labelTarget.setLayoutData(gridData4);
		buttonConfig = new Button(groupTransfer, SWT.NONE);
		buttonConfig.setText("設定");
	}

	/**
	 * This method initializes groupSaveDirectory	
	 *
	 */
	private void createGroupSaveDirectory() {
		GridData gridData6 = new GridData();
		gridData6.widthHint = 256;
		GridLayout gridLayout1 = new GridLayout();
		gridLayout1.numColumns = 2;
		groupSaveDirectory = new Group(sShell, SWT.NONE);
		groupSaveDirectory.setText("処理結果を出力するフォルダの選択");
		groupSaveDirectory.setLayout(gridLayout1);
		buttonSaveDirectory = new Button(groupSaveDirectory, SWT.NONE);
		buttonSaveDirectory.setText("参照");
		textSaveDirectory = new Text(groupSaveDirectory, SWT.BORDER);
		textSaveDirectory.setTextLimit(32768);
		textSaveDirectory.setLayoutData(gridData6);
	}

	/**
	 * This method initializes groupRun	
	 *
	 */
	private void createGroupRun() {
		GridData gridData8 = new GridData();
		gridData8.horizontalSpan = 4;
		gridData8.verticalAlignment = GridData.FILL;
		gridData8.grabExcessHorizontalSpace = true;
		gridData8.grabExcessVerticalSpace = true;
		gridData8.horizontalAlignment = GridData.FILL;
		GridData gridData7 = new GridData();
		gridData7.widthHint = 110;
		GridLayout gridLayout2 = new GridLayout();
		gridLayout2.numColumns = 4;
		GridData gridData = new GridData();
		gridData.horizontalAlignment = GridData.FILL;
		gridData.grabExcessHorizontalSpace = true;
		gridData.grabExcessVerticalSpace = true;
		gridData.verticalAlignment = GridData.FILL;
		groupRun = new Group(sShell, SWT.NONE);
		groupRun.setLayoutData(gridData);
		groupRun.setLayout(gridLayout2);
		groupRun.setText("翻訳する記事を指定して、実行");
		labelArticle = new Label(groupRun, SWT.NONE);
		labelArticle.setText("記事名");
		textArticle = new Text(groupRun, SWT.BORDER);
		textArticle.setTextLimit(32768);
		textArticle.setLayoutData(gridData7);
		buttonRun = new Button(groupRun, SWT.NONE);
		buttonRun.setText("実行");
		buttonRun.setEnabled(false);
		buttonStop = new Button(groupRun, SWT.NONE);
		buttonStop.setText("中止");
		buttonStop.setEnabled(false);
		textLog = new Text(groupRun, SWT.BORDER | SWT.READ_ONLY | SWT.MULTI);
		textLog.setBackground(Display.getCurrent().getSystemColor(SWT.COLOR_LIST_BACKGROUND));
		textLog.setLayoutData(gridData8);
	}

	/**
	 * This method initializes comboSource	
	 *
	 */
	private void createComboSource() {
		GridData gridData1 = new GridData();
		gridData1.widthHint = 50;
		comboSource = new Combo(groupTransfer, SWT.NONE);
		comboSource.setLayoutData(gridData1);
	}

	/**
	 * This method initializes comboTarget	
	 *
	 */
	private void createComboTarget() {
		GridData gridData2 = new GridData();
		gridData2.widthHint = 50;
		comboTarget = new Combo(groupTransfer, SWT.NONE);
		comboTarget.setLayoutData(gridData2);
	}

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO 自動生成されたメソッド・スタブ
		/* Before this is run, be sure to set up the launch configuration (Arguments->VM Arguments)
		 * for the correct SWT library path in order to run with the SWT dlls. 
		 * The dlls are located in the SWT plugin jar.  
		 * For example, on Windows the Eclipse SWT 3.1 plugin jar is:
		 *       installation_directory\plugins\org.eclipse.swt.win32_3.1.0.jar
		 */
		Display display = Display.getDefault();
		MainForm thisClass = new MainForm();
		thisClass.createSShell();
		thisClass.sShell.open();

		while (!thisClass.sShell.isDisposed()) {
			if (!display.readAndDispatch())
				display.sleep();
		}
		display.dispose();
	}

	/**
	 * This method initializes sShell
	 */
	private void createSShell() {
		sShell = new Shell();
		sShell.setText("Wikipedia 翻訳支援ツール");
		createGroupTransfer();
		createGroupSaveDirectory();
		createGroupRun();
		sShell.setSize(new Point(460, 460));
		sShell.setLayout(new GridLayout());
	}

}
