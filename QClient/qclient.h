#ifndef QCLIENT_H
#define QCLIENT_H

#include <QObject>
#include <QTcpSocket>

class QClient : public QObject
{
    Q_OBJECT
public:
    explicit QClient(QObject *parent = nullptr);
    virtual ~QClient();

signals:

public slots:
    void connected();
    void disconnected();
    void readyRead();


private:
    QTcpSocket *sock;
};

#endif // QCLIENT_H
