#include "stdafx.h"
#include "Client.h"

bool terminateThread = false;

Client::Client()
{
	
}

Client::~Client()
{
	if (invertMouseThread.joinable())
	{
		invertMouseThread.join();
	}
}

//Process Execution
void Client::startProcess(const std::string &proc)
{

}

void Client::runCmdCommand(const std::string &command)
{

}

//File Handling
std::string Client::readFile(const std::string &fileName)
{
	std::ifstream f;
	f.open(fileName);
	if (f.is_open())
	{
		std::string content((std::istreambuf_iterator<char>(f)),
			(std::istreambuf_iterator<char>()));
		f.close();
		return content;
	}
	f.close();
	return std::string("CANNOT OPEN FILE ") + fileName;
}

bool Client::writeFile(const std::string &fileName,const std::string &text)
{
	std::ofstream f;
	f.open(fileName);
	if (f.is_open())
	{
		f << text;
		f.close();
		return true;
	}
	f.close();
	return false;
}

bool Client::deleteFile(const std::string &fileName)
{
	return !std::remove(fileName.c_str());
}


//Funny stuff
void Client::run()
{
	POINT point, current;
	int x, y;
	while (!stopRequested())
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
	resetPromise();
}

void Client::toggleInvertMouse()
{
	if (!IsMouseInverted)
	{
		if (invertMouseThread.joinable())
		{
			invertMouseThread.join();
		}
		invertMouseThread = std::thread([&]() {
			run();
		});
		IsMouseInverted = true;
	}
	else
	{
		stop();
		IsMouseInverted = false;
	}
}



void Client::rotateDisplay()
{

}

void Client::message()
{

}