#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QFileDialog>

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);

    // testing
    for (int i = 0; i < 20; i++) {
        ui->listTrainers->addItem(QString::number(i) + " TEST");
    }
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::on_actionOpen_triggered()
{
    QFileDialog::getOpenFileName(this, tr("Open ROM"), "", "GBA ROMs (*.gba *.bin)");
}

void MainWindow::on_actionSave_triggered()
{
    ui->labelROM->setText("Saved!");
}
