// ClientCpp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "Client.h"

int main()
{
	Client c;

	//std::cout << c.readFile("aa.txt");
	//c.writeFile("aa.txt", "xxx");
	//std::cout << c.deleteFile("aaa.txt");
	//c.rotateDisplay();
	c.startProcess("C:\\Program Files (x86)\\Notepad++\\notepad++.exe");


	system("pause");

    return 0;
}

