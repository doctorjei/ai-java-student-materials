/********************************************************************************
 * Form generated from reading UI file 'AppView.ui'
 *
 * Created by: QtJambi User Interface Compiler version 6.4.2
 *
 * WARNING! All changes made in this file will be lost when recompiling UI file!
 *******************************************************************************/

package edu.uwu.pathplanner;

import io.qt.core.*;
import io.qt.gui.*;
import io.qt.widgets.*;


public class AppView {

    public QAction actionOpen;
    public QAction actionAbout;
    public QAction actionExit;
    public QAction actionStop;
    public QAction actionRun;
    public QAction actionPause;
    public QAction actionFast_Run;
    public QWidget centralWidget;
    public QHBoxLayout horizontalLayout;
    public QScrollArea scrollArea;
    public QWidget tileGridView;
    public QVBoxLayout verticalLayout;
    public QTableView settingsTableView;
    public QTableView propertiesTableView;
    public QPlainTextEdit currentRunningPlainTextEdit;
    public QMenuBar menuBar;
    public QMenu menuPath_App;
    public QToolBar mainToolBar;

    public void setupUi(QMainWindow appView)
{
        if (appView.objectName().isEmpty())
            appView.setObjectName("appView");
        appView.resize(1400, 800);
        QIcon icon = new QIcon();
        icon.addFile(":/resources/small.ico", new QSize(), QIcon.Mode.Normal, QIcon.State.Off);
        appView.setWindowIcon(icon);
        this.actionOpen = new QAction(appView);
        this.actionOpen.setObjectName("actionOpen");
        QIcon icon1 = new QIcon();
        icon1.addFile(":/resources/open.ico", new QSize(), QIcon.Mode.Normal, QIcon.State.Off);
        this.actionOpen.setIcon(icon1);
        this.actionAbout = new QAction(appView);
        this.actionAbout.setObjectName("actionAbout");
        this.actionExit = new QAction(appView);
        this.actionExit.setObjectName("actionExit");
        this.actionStop = new QAction(appView);
        this.actionStop.setObjectName("actionStop");
        QIcon icon2 = new QIcon();
        icon2.addFile(":/resources/stop.ico", new QSize(), QIcon.Mode.Normal, QIcon.State.Off);
        this.actionStop.setIcon(icon2);

        this.actionRun = new QAction(appView);
        this.actionRun.setObjectName("actionRun");
        QIcon icon3 = new QIcon();
        icon3.addFile(":/resources/play.ico", new QSize(), QIcon.Mode.Normal, QIcon.State.Off);
        this.actionRun.setIcon(icon3);

        this.actionPause = new QAction(appView);
        this.actionPause.setObjectName("actionPause");
        QIcon icon4 = new QIcon();
        icon4.addFile(":/resources/play_pause.ico", new QSize(), QIcon.Mode.Normal, QIcon.State.Off);
        this.actionPause.setIcon(icon4);
        this.actionFast_Run = new QAction(appView);
        this.actionFast_Run.setObjectName("actionFast_Run");
        QIcon icon5 = new QIcon();
        icon5.addFile(":/resources/play_end.ico", new QSize(), QIcon.Mode.Normal, QIcon.State.Off);
        this.actionFast_Run.setIcon(icon5);
        this.centralWidget = new QWidget(appView);
        this.centralWidget.setObjectName("centralWidget");
        this.horizontalLayout = new QHBoxLayout(this.centralWidget);
        this.horizontalLayout.setSpacing(6);
        this.horizontalLayout.setContentsMargins(11, 11, 11, 11);
        this.horizontalLayout.setObjectName("horizontalLayout");
        this.scrollArea = new QScrollArea(this.centralWidget);
        this.scrollArea.setObjectName("scrollArea");
        this.scrollArea.setVerticalScrollBarPolicy(Qt.ScrollBarPolicy.ScrollBarAlwaysOn);
        this.scrollArea.setHorizontalScrollBarPolicy(Qt.ScrollBarPolicy.ScrollBarAlwaysOn);
        this.scrollArea.setWidgetResizable(true);
        this.tileGridView = new QWidget();
        this.tileGridView.setObjectName("tileGridView");
        this.tileGridView.setGeometry(new QRect(0, 0, 920, 698));
        QPalette palette = new QPalette();
        QBrush brush = new QBrush(new QColor(255, 255, 255, 255));
        brush.setStyle(Qt.BrushStyle.SolidPattern);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.WindowText, brush);
        QBrush brush1 = new QBrush(new QColor(0, 0, 0, 255));
        brush1.setStyle(Qt.BrushStyle.SolidPattern);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Button, brush1);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Light, brush1);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Midlight, brush1);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Dark, brush1);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Mid, brush1);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Text, brush);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.BrightText, brush);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.ButtonText, brush);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Base, brush1);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Window, brush1);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Shadow, brush1);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.AlternateBase, brush1);
        QBrush brush2 = new QBrush(new QColor(255, 255, 220, 255));
        brush2.setStyle(Qt.BrushStyle.SolidPattern);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.ToolTipBase, brush2);
        palette.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.ToolTipText, brush1);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.WindowText, brush);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Button, brush1);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Light, brush1);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Midlight, brush1);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Dark, brush1);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Mid, brush1);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Text, brush);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.BrightText, brush);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.ButtonText, brush);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Base, brush1);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Window, brush1);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Shadow, brush1);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.AlternateBase, brush1);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.ToolTipBase, brush2);
        palette.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.ToolTipText, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.WindowText, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Button, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Light, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Midlight, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Dark, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Mid, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Text, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.BrightText, brush);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.ButtonText, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Base, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Window, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Shadow, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.AlternateBase, brush1);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.ToolTipBase, brush2);
        palette.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.ToolTipText, brush1);
        this.tileGridView.setPalette(palette);
        this.scrollArea.setWidget(this.tileGridView);

        this.horizontalLayout.addWidget(this.scrollArea);
        this.verticalLayout = new QVBoxLayout();
        this.verticalLayout.setSpacing(6);
        this.verticalLayout.setObjectName("verticalLayout");
        this.settingsTableView = new QTableView(this.centralWidget);
        this.settingsTableView.setObjectName("settingsTableView");
        this.settingsTableView.setMinimumSize(new QSize(450, 0));
        this.settingsTableView.setMaximumSize(new QSize(350, 107));
        this.settingsTableView.setFrameShadow(QFrame.Shadow.Sunken);
        this.settingsTableView.setProperty("showDropIndicator", false);
        this.settingsTableView.setAlternatingRowColors(true);
        this.settingsTableView.setSelectionMode(QAbstractItemView.SelectionMode.NoSelection);

        this.verticalLayout.addWidget(this.settingsTableView, 0, Qt.AlignmentFlag.AlignRight);
        this.propertiesTableView = new QTableView(this.centralWidget);
        this.propertiesTableView.setObjectName("propertiesTableView");
        this.propertiesTableView.setMinimumSize(new QSize(450, 0));
        this.propertiesTableView.setMaximumSize(new QSize(350, 16777215));
        this.propertiesTableView.setFrameShadow(QFrame.Shadow.Sunken);
        this.propertiesTableView.setCornerButtonEnabled(false);

        this.verticalLayout.addWidget(this.propertiesTableView, 0, Qt.AlignmentFlag.AlignRight);
        this.currentRunningPlainTextEdit = new QPlainTextEdit(this.centralWidget);
        this.currentRunningPlainTextEdit.setObjectName("currentRunningPlainTextEdit");
        this.currentRunningPlainTextEdit.setMinimumSize(new QSize(450, 0));
        this.currentRunningPlainTextEdit.setMaximumSize(new QSize(350, 16777215));
        QPalette palette1 = new QPalette();
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.WindowText, brush);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Button, brush1);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Light, brush1);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Midlight, brush1);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Dark, brush1);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Mid, brush1);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Text, brush);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.BrightText, brush);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.ButtonText, brush);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Base, brush1);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Window, brush1);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.Shadow, brush1);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.AlternateBase, brush1);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.ToolTipBase, brush2);
        palette1.setBrush(QPalette.ColorGroup.Active, QPalette.ColorRole.ToolTipText, brush1);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.WindowText, brush);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Button, brush1);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Light, brush1);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Midlight, brush1);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Dark, brush1);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Mid, brush1);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Text, brush);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.BrightText, brush);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.ButtonText, brush);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Base, brush1);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Window, brush1);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.Shadow, brush1);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.AlternateBase, brush1);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.ToolTipBase, brush2);
        palette1.setBrush(QPalette.ColorGroup.Inactive, QPalette.ColorRole.ToolTipText, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.WindowText, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Button, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Light, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Midlight, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Dark, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Mid, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Text, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.BrightText, brush);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.ButtonText, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Base, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Window, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.Shadow, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.AlternateBase, brush1);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.ToolTipBase, brush2);
        palette1.setBrush(QPalette.ColorGroup.Disabled, QPalette.ColorRole.ToolTipText, brush1);
        this.currentRunningPlainTextEdit.setPalette(palette1);
        this.currentRunningPlainTextEdit.setFrameShadow(QFrame.Shadow.Sunken);

        this.verticalLayout.addWidget(this.currentRunningPlainTextEdit, 0, Qt.AlignmentFlag.AlignRight);

        this.horizontalLayout.addLayout(this.verticalLayout);
        appView.setCentralWidget(this.centralWidget);
        this.menuBar = new QMenuBar(appView);
        this.menuBar.setObjectName("menuBar");
        this.menuBar.setGeometry(new QRect(0, 0, 1400, 17));
        this.menuPath_App = new QMenu(this.menuBar);
        this.menuPath_App.setObjectName("menuPath_App");
        appView.setMenuBar(this.menuBar);
        this.mainToolBar = new QToolBar(appView);
        this.mainToolBar.setObjectName("mainToolBar");
        this.mainToolBar.setEnabled(true);
        this.mainToolBar.setCursor(new QCursor(Qt.CursorShape.PointingHandCursor));
        this.mainToolBar.setMovable(true);
        this.mainToolBar.setIconSize(new QSize(50, 50));
        this.mainToolBar.setFloatable(true);
        appView.addToolBar(Qt.ToolBarArea.TopToolBarArea, this.mainToolBar);

        this.menuBar.addAction(this.menuPath_App.menuAction());
        this.menuPath_App.addAction(this.actionOpen);
        this.menuPath_App.addAction(this.actionAbout);
        this.menuPath_App.addAction(this.actionExit);
        this.mainToolBar.addAction(this.actionOpen);
        this.mainToolBar.addAction(this.actionStop);
        this.mainToolBar.addAction(this.actionRun);
        this.mainToolBar.addAction(this.actionPause);
        this.mainToolBar.addAction(this.actionFast_Run);

        this.retranslateUi(appView);
        this.actionExit.triggered.connect(appView::close);

        QMetaObject.connectSlotsByName(appView);
    }

    public void retranslateUi(QMainWindow appView)
    {
        appView.setWindowTitle(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "Path Planner", null));
        this.actionOpen.setText(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "Open", null));
        this.actionAbout.setText(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "About", null));
        this.actionExit.setText(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "Exit", null));
        this.actionStop.setText(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "Stop", null));
        this.actionStop.setToolTip(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "Halt the search", null));
        this.actionRun.setText(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "Run", null));
        this.actionRun.setToolTip(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "Run the search", null));
        this.actionPause.setText(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "Pause", null));
        this.actionPause.setToolTip(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "Step one increment at a time", null));
        this.actionFast_Run.setText(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "Fast Run", null));
        this.actionFast_Run.setToolTip(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "Run to end of search without animation", null));
        this.menuPath_App.setTitle(io.qt.core.QCoreApplication.translate("edu.uwu.pathplanner.AppView", "App", null));
    }
}

