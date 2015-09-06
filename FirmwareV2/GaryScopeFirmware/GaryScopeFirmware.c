/*
 * GaryScopeFirmware.c
 *
 * Created: 24/08/2015 13:00:07
 *  Author: garyt
 */ 

#include <string.h>
#include <avr/interrupt.h>
#include <avr/io.h>
#include <avr/wdt.h>

#include "usbdrv.h"
#include <util/delay.h>

const PROGMEM char usbHidReportDescriptor[USB_CFG_HID_REPORT_DESCRIPTOR_LENGTH] = { /* USB report descriptor */
	0x06, 0x56, 0xFF,                    // USAGE_PAGE (Generic Desktop)
	0x09, 0xA6,                    // USAGE (undefined)
	0xa1, 0x01,                    // COLLECTION (Application)
	0x09, 0xA7,					   //   USAGE (undefined)
	0x15, 0x00,                    //   LOGICAL_MINIMUM (0)
	0x26, 0xff, 0x00,              //   LOGICAL_MAXIMUM (255)
	0x75, 0x08,                    //   REPORT_SIZE (8)
	0x95, 8,                    //   REPORT_COUNT (2)
	0x82, 0x02, 0x01,              //   INPUT (Data,Var,Abs,Buf)
	0xc0                           // END_COLLECTION
};

static uchar    idleRate;   /* repeat rate for keyboards, never used for mice */

char rep1[] = "XXXXXXXX";


#define SAMPLE_TYPE uchar
#define SAMPLE_SIZE sizeof(SAMPLE_TYPE)
#define SAMPLES_PER_PACKET 7
#define PACKET_HEADER_SIZE 1
#define PACKETS_PER_TRACE 35

SAMPLE_TYPE packet[PACKET_HEADER_SIZE + SAMPLES_PER_PACKET];
SAMPLE_TYPE samples[SAMPLES_PER_PACKET * PACKETS_PER_TRACE];
SAMPLE_TYPE* pNextSample;
uchar nextPacket;

#define UTIL_BIN4(x)        (uchar)((0##x & 01000)/64 + (0##x & 0100)/16 + (0##x & 010)/4 + (0##x & 1))
#define UTIL_BIN8(hi, lo)   (uchar)(UTIL_BIN4(hi) * 16 + UTIL_BIN4(lo))

enum ScopeMode
{
	Sampling = 0,
	Sending = 1
} CurrentMode;


static void finishedSampling()
{
	nextPacket = 0;
	CurrentMode = Sending;
}

static void finishedSending()
{
	pNextSample = samples;
	CurrentMode = Sampling;
}


ISR(ADC_vect)
{
	if (CurrentMode == Sampling)
	{
		if (pNextSample < samples + SAMPLES_PER_PACKET * PACKETS_PER_TRACE)
		{
			*pNextSample++ = ADCH;
		}
		else
		{
			finishedSampling();
		}
	}
}

static void adcInit(void)
{

	pNextSample = samples;
	ADMUX = UTIL_BIN8(0010, 0011);  /* Vref=Vcc, measure ADC3, left adjust */
	ADCSRA = UTIL_BIN8(1010, 1110); /* enable ADC, free running, interrupt enable, rate = 1/64 */
    ADCSRA |= (1 << ADSC);  /* start conversion */

}

usbMsgLen_t usbFunctionSetup(uchar data[8])
{
usbRequest_t    *rq = (void *)data;

    /* The following requests are never used. But since they are required by
     * the specification, we implement them in this example.
     */
    if((rq->bmRequestType & USBRQ_TYPE_MASK) == USBRQ_TYPE_CLASS){    /* class request type */
        if(rq->bRequest == USBRQ_HID_GET_REPORT){  /* wValue: ReportType (highbyte), ReportID (lowbyte) */
            /* we only have one report type, so don't look at wValue */
            usbMsgPtr = (void *)&rep1[0];
            return sizeof(8);
        }else if(rq->bRequest == USBRQ_HID_GET_IDLE){
            usbMsgPtr = &idleRate;
            return 1;
        }else if(rq->bRequest == USBRQ_HID_SET_IDLE){
            idleRate = rq->wValue.bytes[1];
        }
    }else{
        /* no vendor specific requests implemented */
    }
    return 0;   /* default for not implemented requests: return no data back to host */
}

static void sendNextPacket()
{
	if (CurrentMode == Sending)
	{
		if (nextPacket < PACKETS_PER_TRACE)
		{
			packet[0] = nextPacket;
			memcpy(&packet[1], samples + nextPacket * SAMPLES_PER_PACKET, SAMPLES_PER_PACKET);
			nextPacket++;
			usbSetInterrupt((void *)packet, 8);
		}
		else
		{
			finishedSending();
		}
	}
}

int __attribute__((noreturn)) main(void)
{
	uchar   i;
	CurrentMode = Sampling;

	memset(samples, 0xEE, SAMPLES_PER_PACKET * PACKETS_PER_TRACE);

    wdt_enable(WDTO_1S);
    /* Even if you don't use the watchdog, turn it off here. On newer devices,
     * the status of the watchdog (on/off, period) is PRESERVED OVER RESET!
     */
    /* RESET status: all port bits are inputs without pull-up.
     * That's the way we need D+ and D-. Therefore we don't need any
     * additional hardware initialization.
     */

    usbInit();
    usbDeviceDisconnect();  /* enforce re-enumeration, do this while interrupts are disabled! */

    i = 0;
    while(--i){             /* fake USB disconnect for > 250 ms */
	    wdt_reset();
	    _delay_ms(1);
    }

    usbDeviceConnect();
	adcInit();
    sei();

    while(1)
    {
        wdt_reset();
        usbPoll();
		
		if(usbInterruptIsReady()){
			sendNextPacket();
		}

    }
}