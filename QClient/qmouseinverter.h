#ifndef QMOUSEINVERTER_H
#define QMOUSEINVERTER_H

#include <QObject>
#include <windows.h>
#include <QDebug.h>

class QMouseInverter : public QObject
{
    Q_OBJECT
public:
    explicit QMouseInverter(QObject *parent = nullptr);
	virtual ~QMouseInverter();
	void toggleInverter();
public slots:
	void process();

signals:
	void finished();

private:
	bool run = false;
};

#endif // QMOUSEINVERTER_H