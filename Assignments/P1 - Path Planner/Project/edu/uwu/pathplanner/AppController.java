package edu.uwu.pathplanner;

import java.util.ArrayList;
import io.qt.core.*;
import io.qt.gui.*;
import io.qt.widgets.*;

import edu.uwu.tiles.Tile;
import edu.uwu.tiles.TileMap;


// Main Application Class
public class AppController extends QMainWindow
{
    public enum Parameter { START_ROW, START_COL, GOAL_ROW, GOAL_COL, COUNT }
    public enum RunState { DEFAULT, READY, PAUSED, RUNNING, COMPLETED }

    public static final int BTN_NONE = 0x00, BTN_OPEN = 0x01, BTN_STOP = 0x02, BTN_START = 0x04,
                            BTN_PLAY = 0x10, BTN_STEP = 0x20, BTN_QUICK = 0x40, BTN_ALL = 0x7F;

    // Inner class TileGridView; Inherits from QWidget, allowing its own paint event handler.
    public static class TileGridView extends QWidget
    {
        private AppController parent;

        public TileGridView(AppController _parent)
        {
            parent = _parent;
            parent.ui.scrollArea.verticalScrollBar().sliderMoved.connect(this::onScroll);
            parent.ui.scrollArea.horizontalScrollBar().sliderMoved.connect(this::onScroll);
        }

        @Override
        protected void paintEvent(QPaintEvent event)
        {
            QPainter painter = new QPainter(this);
            parent.paintTheMap(painter);
            painter.end();
        }

        private void onScroll(int value) { repaint(); }
    }

    // Coordinates, offsets, and dimensions
    private int gridWidth;
    private int gridHeight;
    private QPoint gridOffset = new QPoint(0, 0); // Unneeded?

    private AppView ui;
    private TileGridView tileGridView;
    private QStandardItemModel parameters;
    private QStandardItemModel settings;

    private int buttons = BTN_NONE;
    private RunState currentState = RunState.DEFAULT;
    private int shortTimer = 0;
    private int longTimer = 5000;

    private QTimer timer;
    private AppModel appModel;
    private TileMap tileMap;
    private QIcon runIcon, pauseIcon;

    // Constructors
    public AppController() { this(null); }

    public AppController(QWidget parent)
    {
        super(parent);
        ui = new AppView();
        ui.setupUi(this);

        tileGridView = new TileGridView(this);
        ui.scrollArea.setWidget(tileGridView);
        buttons = BTN_NONE;

        parameters = new QStandardItemModel(4, 1, this);
        parameters.setHorizontalHeaderItem(0, new QStandardItem("Properties"));
        parameters.setVerticalHeaderItem(Parameter.START_ROW.ordinal(), new QStandardItem("Start Row"));
        parameters.setVerticalHeaderItem(Parameter.START_COL.ordinal(), new QStandardItem("Start Column"));
        parameters.setVerticalHeaderItem(Parameter.GOAL_ROW.ordinal(), new QStandardItem("Goal Row"));
        parameters.setVerticalHeaderItem(Parameter.GOAL_COL.ordinal(), new QStandardItem("Goal Column"));
        ui.propertiesTableView.setModel(parameters);
        ui.propertiesTableView.resizeColumnsToContents();

//    settings = new QStandardItemModel(4, 2, this);
//    settings->setHorizontalHeaderItem(0,new QStandardItem(QString("Setting Value")));
//    settings->setHorizontalHeaderItem(1,new QStandardItem(QString("Setting Name")));
//    settings->setItem(0, 0, new QStandardItem(QString::number(MIN_TILE_RADIUS)));
//    settings->setItem(0, 1, new QStandardItem(QString("Minimum Tile Radius")));
//    settings->setItem(1, 0, new QStandardItem(QString::number(0)));
//    settings->setItem(1, 1, new QStandardItem(QString("Regular Run Step (ms)")));
//    settings->setItem(2, 0, new QStandardItem(QString::number(5000)));
//    settings->setItem(2, 1, new QStandardItem(QString("Quick Run Step (ms)")));
//    settings->setItem(3, 0, new QStandardItem(QString::number(1)));
//    settings->setItem(3, 1, new QStandardItem(QString("Number of Repetitions")));
//    ui->settingsTableView->setModel(settings);
//    ui->settingsTableView->verticalHeader()->setVisible(false);
//    ui->settingsTableView->resizeColumnsToContents();
        appModel = new AppModel();
        timer = new QTimer(this);
        timer.setTimerType(Qt.TimerType.PreciseTimer);
        timer.timeout.connect(this::runSearch);

        parameters.itemChanged.connect(this::onPropertyChange);
        ui.actionOpen.triggered.connect(this::onActionOpenTriggered);
        ui.actionRun.triggered.connect(this::onActionRunTriggered);
        ui.actionStop.triggered.connect(this::onActionStopTriggered);
        ui.actionPause.triggered.connect(this::onActionPauseTriggered);
        ui.actionFast_Run.triggered.connect(this::onActionFastRunTriggered);

        runIcon = new QIcon(":/resources/play.ico");
        pauseIcon = new QIcon(":/resources/pause.ico");

        if (!load())
        { // Start loading data.
            System.out.println("ERROR: Couldn't load map!");
            System.out.println("Working Directory = " + System.getProperty("user.dir"));
        }
    }

    // Paint methods
    public void paintTheMap(QPainter canvasPainter)
    {
        paintTheTiles(canvasPainter);
        paintEndPoints(canvasPainter);
        paintTheProgress(canvasPainter);
    }

    private void paintTheTiles(QPainter painter)
    {
        int xOffset = gridOffset.x();
        int yOffset = gridOffset.y();

        int rowCount = appModel.getTileMap().getRowCount();
        int columnCount = appModel.getTileMap().getColumnCount();
        double radius = appModel.getTileMap().getTileRadius();

        // Set up the paint brush and drawing area (black background)
        painter.setBrush(new QBrush(new QColor(Qt.GlobalColor.black), Qt.BrushStyle.SolidPattern));
        painter.drawRect(0, 0, this.geometry().right(), this.geometry().bottom());

        // Set up hexagon to be painted; apply general x / y offset (in painted area)
        int hRadius = (int)(radius / Math.sqrt(3.0) - 0.5);

        ArrayList<QPoint> hexagon = new ArrayList<>();
        hexagon.add(new QPoint(xOffset, yOffset - hRadius * 2));
        hexagon.add(new QPoint(xOffset + (int)(radius - 0.5), yOffset - hRadius));
        hexagon.add(new QPoint(xOffset + (int)(radius - 0.5), yOffset + hRadius));
        hexagon.add(new QPoint(xOffset, yOffset + hRadius * 2));
        hexagon.add(new QPoint(xOffset - ((int)(radius - 0.5)), yOffset + hRadius));
        hexagon.add(new QPoint(xOffset - ((int)(radius - 0.5)), yOffset - hRadius));

        QPolygon hexRegion = new QPolygon(hexagon);
        QBrush[] brushes = new QBrush[16];
        QBrush blackBrush = new QBrush(new QColor(Qt.GlobalColor.black), Qt.BrushStyle.SolidPattern);

        for (int i = 0; i < 16; i++)
        {
            int tileWeight = 255 - (i << 4);
            brushes[i] = new QBrush(new QColor(tileWeight, tileWeight, tileWeight), Qt.BrushStyle.SolidPattern);
        }

        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                Tile tile = appModel.getTileMap().getTile(row, column);
                int xOffset2 = (int)tile.getX();
                int yOffset2 = (int)tile.getY();

                QPolygon target = new QPolygon(hexRegion);
                target.translate(xOffset2, yOffset2);
                int tileWeight = tile.getWeight();

                if (tileWeight != 0)
                {
                    if (tileWeight > 15)
                        tileWeight = 15;

                    painter.setBrush(brushes[tileWeight]);
                }
                else
                    painter.setBrush(blackBrush);

                painter.drawPolygon(target, Qt.FillRule.WindingFill);
            }
        }
    }

    private void paintEndPoints(QPainter painter)
    {
        QPen pen = new QPen(new QColor(Qt.GlobalColor.black));
        pen.setWidth(2);
        painter.setPen(pen);

        Tile startTile = appModel.getStartTile();
        Tile goalTile = appModel.getGoalTile();

        // Add offset to draw from  the center.
        int length = (int)(appModel.getTileMap().getTileRadius() * 0.75);
        int startCenterX = (int)startTile.getX() + gridOffset.x() - length / 2 + 1;
        int startCenterY = (int)startTile.getY() + gridOffset.y() - length + 1;
        int goalCenterX = (int)goalTile.getX() + gridOffset.x() - length / 2 + 1;
        int goalCenterY = (int)goalTile.getY() + gridOffset.y() - length + 1;

        QRect startRect = new QRect(startCenterX, startCenterY, length - 1, length);
        QRect goalRect = new QRect(goalCenterX, goalCenterY, length - 1, length);

        QBrush brush = new QBrush(new QColor(Qt.GlobalColor.red), Qt.BrushStyle.SolidPattern);
        painter.setBrush(brush);
        painter.drawRect(startRect);

        brush.setColor(new QColor(Qt.GlobalColor.green));
        painter.setBrush(brush);
        painter.drawRect(goalRect);
    }

    private void paintTheProgress(QPainter painter)
    {
        int radius = (int) appModel.getTileMap().getTileRadius();

        QPen tilePen = new QPen();
        QBrush tileBrush = new QBrush(Qt.BrushStyle.SolidPattern);
        tilePen.setWidth(2);

        for (int i = 0; i < appModel.getTileMap().getRowCount(); i++)
        {
            for (int j = 0; j < appModel.getTileMap().getColumnCount(); j++)
            {
                Tile tile = appModel.getTileMap().getTile(i, j);
                int fillColor = tile.getFill();
                int markerColor = tile.getMarker();
                int x = (int)tile.getX() + gridOffset.x();
                int y = (int)tile.getY() + gridOffset.y();
                ArrayList<Tile.Line> lines = tile.getLines();

                if (fillColor != 0)
                {
                    tileBrush.setColor(new QColor(fillColor));
                    tilePen.setColor(new QColor(tile.getOutline()));
                    painter.setPen(tilePen);
                    painter.setBrush(tileBrush);
                    painter.drawEllipse(x - radius / 2, y - radius / 2, radius, radius);
                }

                if (markerColor != 0)
                {
                    tileBrush.setColor(new QColor(markerColor));
                    tilePen.setColor(new QColor(tile.getOutline()));
                    painter.setPen(tilePen);
                    painter.setBrush(tileBrush);
                    painter.drawEllipse(x - radius / 2, y - radius / 2, radius, radius);
                }

                for (Tile.Line line : lines)
                {
                    Tile target = line.endpoint;
//                    int lineColor = line.color;
                    tilePen.setColor(new QColor(line.color));
                    painter.setPen(tilePen);
                    painter.drawLine(x, y, (int)target.getX() + gridOffset.x(), (int)target.getY() + gridOffset.y());
                }
            }
        }

        if (appModel.isGoalFound())
        {
            ArrayList<Tile> path = appModel.getSolution();
            int smallRadius = (int)(radius * 0.25);
            tileBrush.setColor(new QColor(Qt.GlobalColor.green));
            painter.setBrush(tileBrush);

            for (Tile tile : path)
            {
                QRect rect = new QRect((int)tile.getX() - smallRadius + gridOffset.x(),
                                       (int)tile.getY() - smallRadius + gridOffset.y(),
                                       smallRadius * 2, smallRadius * 2);
                painter.drawRect(rect);
            }
        }
    }

    // Action handlers
    private void onActionOpenTriggered()
    {
        var response = QFileDialog.getOpenFileName(this, "Open Data File",
                QDir.currentPath(), "Data Files (*.txt)");

        if (response != null && response.result != null && !response.result.isEmpty())
            load(response.result);
    }

    public boolean load() { return load(""); } // Handles case of empty parameters.

    public boolean load(String filename)
    {
        // Put GUI in default (unloaded) state until a map is loaded; clear memory.
        updateState(RunState.DEFAULT);
        appModel.shutdown();
        appModel.unload();

        if (!appModel.load(filename))
        {
            // TODO: General failure message box here: "Couldn't load tile map."
            gridWidth = gridHeight = 0;
            return false;
        }

        // Load up the GUI for a ready state and then adjust properties, etc.
        Tile startTile = appModel.getStartTile();
        Tile goalTile = appModel.getGoalTile();

        parameters.itemChanged.disconnect(this::onPropertyChange);
        parameters.setItem(0, 0, new QStandardItem(String.valueOf(startTile.getRow())));
        parameters.setItem(1, 0, new QStandardItem(String.valueOf(startTile.getColumn())));
        parameters.setItem(2, 0, new QStandardItem(String.valueOf(goalTile.getRow())));
        parameters.setItem(3, 0, new QStandardItem(String.valueOf(goalTile.getColumn())));
        parameters.itemChanged.connect(this::onPropertyChange);
        updateState(RunState.READY);

        double tileRadius = appModel.getTileMap().getTileRadius();
        gridWidth = (int)(tileRadius * (2 * appModel.getTileMap().getColumnCount() + 1));
        gridHeight = (int)(tileRadius * (3 * appModel.getTileMap().getRowCount() + 4) / Math.sqrt(3.0));
        tileGridView.setMinimumSize(gridWidth, gridHeight);
        tileGridView.setMaximumSize(gridWidth, gridHeight);
        tileGridView.resize(gridWidth, gridHeight);
        tileGridView.update();
        tileGridView.repaint();
        return true;
    }

    private void checkSolution()
    {
        String resultString = appModel.checkSolution();
        if (resultString != null)
        {
            QMessageBox msgBox = new QMessageBox();
            msgBox.setWindowTitle("Solution Checker");
            msgBox.setText(resultString);
            msgBox.exec();
        }
    }

    private void onActionRunTriggered() {
        switch (currentState)
        {
            case READY:
                for (int index = 0; index < Parameter.COUNT.ordinal(); index++)
                    parameters.item(index, 0).setEditable(false);
                updateState(RunState.RUNNING);
                timer.start();
                break;
            case RUNNING:
                updateState(RunState.PAUSED);
                break;
            case PAUSED:
                updateState(RunState.RUNNING);
                break;
            case COMPLETED:
                updateState(RunState.COMPLETED);
                timer.stop();
                break;
            case DEFAULT:
            default:
                // We should never get here; maybe add an error message?
                updateState(RunState.DEFAULT);
                break;
        }
    }

    private void onActionStopTriggered()
    {
        appModel.shutdown();
        appModel.getTileMap().resetTileDrawing();
        appModel.initialize();
        tileGridView.update();
        updateState(RunState.READY);
    }

// change the start flag by clicking on a tile
// void AppController::on_actionSet_Start_triggered()

// change the goal flag by clicking on a tile
// void AppController::on_actionSet_Goal_triggered()

    // TODO: Rename "actionStep".
    private void onActionPauseTriggered()
    {
        if (appModel.isGoalFound())
            return;

        // Run an update cycle, then repaint.
        updateState(RunState.PAUSED);
        appModel.update(0);
        tileGridView.update();

        // Handle case of reaching the goal.
        if (appModel.isGoalFound())
        {
            updateState(RunState.COMPLETED);
            checkSolution();
        }
    }

    private void onActionFastRunTriggered()
    {
        if (appModel.isGoalFound())
            return;

        // Run until complete
        appModel.update(Integer.MAX_VALUE); // TODO: Sanity check? 4 min maybe?...

        // Handle case of reaching the goal.
        if (appModel.isGoalFound())
            checkSolution();

        tileGridView.update();
        updateState(RunState.COMPLETED);
    }

    // Run an update cycle, then repaint.
    private void runSearch()
    {
        if (currentState != RunState.RUNNING)
            return;

        appModel.update(0);
        tileGridView.update();

        // Handle case of reaching the goal.
        if (appModel.isGoalFound())
        {
            updateState(RunState.COMPLETED);
            checkSolution();
            timer.stop();
        }
    }

    private void onPropertyChange(QStandardItem item)
    {
        // If the column isn't zero, something is wrong; abort.
        if (item.index().column() != 0)
            return;

        Tile startTile = appModel.getStartTile();
        Tile goalTile = appModel.getGoalTile();
        int startRow = startTile.getRow();
        int startColumn = startTile.getColumn();
        int goalRow = goalTile.getRow();
        int goalColumn = goalTile.getColumn();

        int value = Integer.parseInt(item.text());

        switch (item.index().row())
        {
            case 0: // START_ROW
                if (value == startRow) return;
                startRow = value;
                break;
            case 1: // START_COL
                if (value == startColumn) return;
                startColumn = value;
                break;
            case 2: // GOAL_ROW
                if (value == goalRow) return;
                goalRow = value;
                break;
            case 3: // GOAL_COL
                if (value == goalColumn) return;
                goalColumn = value;
                break;
            default:
                return;
        }

        appModel.initialize(startRow, startColumn, goalRow, goalColumn);
        tileGridView.update();
        updateState(RunState.READY);
    }

    private void updateState(RunState newState)
    {
        switch (newState)
        {
            case READY:
                for (int index = 0; index < Parameter.COUNT.ordinal(); index++)
                    parameters.item(index, 0).setEditable(true);

                buttons = BTN_OPEN | BTN_PLAY | BTN_STEP | BTN_QUICK; // BTN_START | BTN_GOAL
                ui.actionRun.setIcon(runIcon);
                currentState = RunState.READY;
                break;
            case RUNNING:
                buttons = BTN_OPEN | BTN_STOP | BTN_PLAY | BTN_STEP | BTN_QUICK;
                ui.actionRun.setIcon(pauseIcon);
                currentState = RunState.RUNNING;
                break;
            case PAUSED:
                buttons = BTN_OPEN | BTN_STOP | BTN_PLAY | BTN_STEP | BTN_QUICK;
                ui.actionRun.setIcon(runIcon);
                currentState = RunState.PAUSED;
                break;
            case COMPLETED:
                buttons = BTN_OPEN | BTN_STOP;
                ui.actionRun.setIcon(runIcon);
                currentState = RunState.COMPLETED;
                break;
            case DEFAULT:
            default:
                buttons = BTN_OPEN;
                ui.actionRun.setIcon(runIcon);
                break;
        }

        // Update the buttons based on the state change.
        ui.actionOpen.setEnabled((buttons & BTN_OPEN) == BTN_OPEN);
        ui.actionStop.setEnabled((buttons & BTN_STOP) == BTN_STOP);
        ui.actionRun.setEnabled((buttons & BTN_PLAY) == BTN_PLAY);
        ui.actionPause.setEnabled((buttons & BTN_STEP) == BTN_STEP);
        ui.actionFast_Run.setEnabled((buttons & BTN_QUICK) == BTN_QUICK);
//        ui->actionSet_Start->setEnabled((buttons & START) == START);
//        ui->actionSet_Goal->setEnabled((buttons & GOAL) == GOAL);
    }

    // Main method (App kick-off)
    public static void main(String[] args)
    {
        QApplication.initialize(args);
        AppController window = new AppController();
        window.show();
        QApplication.exec();
    }
}
