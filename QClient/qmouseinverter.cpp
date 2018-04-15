#include "qmouseinverter.h"

QMouseInverter::QMouseInverter(QObject *parent) : QObject(parent)
{

}

QMouseInverter::~QMouseInverter()
{

}

void QMouseInverter::toggleInverter()
{
	run = !run;
}

void QMouseInverter::process()
{
	POINT point, current;
	int x, y;
	while (1)
	{
		if (run)
		{
			GetCursorPos(&current);
			Sleep(10);
			if (GetCursorPos(&point) != current.y || current.x)
			{
				x = (point.x - current.x) * 2;
				y = (point.y - current.y) * 2;
				SetCursorPos(point.x - x, point.y - y);
			}
		}
		else {
			Sleep(30);
		}
	}
	emit finished();
}