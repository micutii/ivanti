#include "stdafx.h"
#include "Client.h"

Client::Client()
{
}

Client::~Client()
{
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
void Client::invertMouse()
{

}

void Client::rotateDisplay()
{

}

void Client::message()
{

}