#include "qclient.h"

QClient::QClient(QObject *parent) : QObject(parent)
{
    sock = new QTcpSocket(this);
	connect(sock, SIGNAL(connected()), this, SLOT(connected()));
	connect(sock, SIGNAL(disconnected()), this, SLOT(disconnected()));
}

void QClient::connected()
{

}

void QClient::disconnected()
{

}

QClient::~QClient()
{
    delete sock;
}
