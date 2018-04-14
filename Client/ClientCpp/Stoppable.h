#pragma once
class Stoppable
{
private:
	std::promise<void> exitSignal;
	std::future<void> futureObj;
public:
	Stoppable();
	Stoppable(Stoppable && obj);

	Stoppable & operator=(Stoppable && obj);

	// Task need to provide defination  for this function
	// It will be called by thread function
	virtual void run() = 0;

	//Checks if thread is requested to stop
	bool stopRequested();

	// Request the thread to stop by setting value in promise object
	void stop();
	void resetPromise();
	virtual ~Stoppable();
};

