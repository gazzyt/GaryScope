/*
 * GaryScope.h
 *
 * Created: 16/01/2013 10:58:24
 *  Author: garyt
 */ 


#ifndef GARYSCOPE_H_
#define GARYSCOPE_H_

#define REPORT_ID_SCOPE_SAMPLE	1
#define REPORT_ID_TIMER_CLOCK	2

typedef struct ScopeSampleReport
{
	//uchar	reportId;
	unsigned int	channel1Sample;
	unsigned int	channel2Sample;
} ScopeSampleReport_t;

typedef struct TimerClockReport
{
	//uchar reportId;
	uchar timerClockSelect;
} TimerClockReport_t;


#endif /* GARYSCOPE_H_ */