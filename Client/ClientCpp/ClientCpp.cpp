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
	c.toggleInvertMouse();
	Sleep(10000);
	c.toggleInvertMouse();
	Sleep(1000);
	c.toggleInvertMouse();
	Sleep(1000);
	c.toggleInvertMouse();
	getchar();

    return 0;
}

