#include <QCoreApplication>
#include "qclient.h"

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);
	QClient c("192.168.21.103", 13000);
    return a.exec();
}
