#pragma once


class Client
{
public:
	Client();
	//Process Execution
	void startProcess(const std::string &);
	void runCmdCommand(const std::string &);

	//File Handling
	std::string readFile(const std::string &);
	bool writeFile(const std::string &, const std::string &);
	bool deleteFile(const std::string &);

	//Funny stuff
	void invertMouse();
	void rotateDisplay();
	void message();

	~Client();

private:

};

