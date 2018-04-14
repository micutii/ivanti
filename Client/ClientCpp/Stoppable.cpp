#include "stdafx.h"
#include "Stoppable.h"


Stoppable::Stoppable() :
	futureObj(exitSignal.get_future())
{
}

Stoppable::Stoppable(Stoppable && obj) : exitSignal(std::move(obj.exitSignal)), futureObj(std::move(obj.futureObj))
{
	
}

Stoppable & Stoppable::operator=(Stoppable && obj)
{
	exitSignal = std::move(obj.exitSignal);
	futureObj = std::move(obj.futureObj);
	return *this;
	
}

bool Stoppable::stopRequested()
{
	// checks if value in future object is available
	if (futureObj.wait_for(std::chrono::milliseconds(0)) == std::future_status::timeout)
		return false;
	return true;
}

void Stoppable::stop()
{
	exitSignal.set_value();
}

void Stoppable::resetPromise()
{
	exitSignal = std::promise<void>();
	futureObj = exitSignal.get_future();

}

Stoppable::~Stoppable()
{
}
