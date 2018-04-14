#include <QCoreApplication>

#include "qclient.h"

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);
	QClient client;
    return a.exec();
}
