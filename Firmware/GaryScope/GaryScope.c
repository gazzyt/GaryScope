/*
 * GaryScope.c
 *
 * Created: 11/01/2013 22:14:31
 *  Author: garyt
 */ 


#include <avr/io.h>
#include <avr/wdt.h>
#include <avr/eeprom.h>
#include <avr/interrupt.h>
#include <avr/pgmspace.h>
#include <util/delay.h>

#include "usbdrv.h"
#include "oddebug.h"

/*
Pin assignment:
PB1 = key input (active low with pull-up)
PB3 = analog input (ADC3)
PB4 = LED output (active high)

PB0, PB2 = USB data lines
*/

#define BIT_LED 4
#define BIT_KEY 1


#define UTIL_BIN4(x)        (uchar)((0##x & 01000)/64 + (0##x & 0100)/16 + (0##x & 010)/4 + (0##x & 1))
#define UTIL_BIN8(hi, lo)   (uchar)(UTIL_BIN4(hi) * 16 + UTIL_BIN4(lo))

#ifndef NULL
#define NULL    ((void *)0)
#endif

/* ------------------------------------------------------------------------- */

static uchar    reportBuffer[4];    /* buffer for HID reports */
static uchar    idleRate;           /* in 4 ms units */

static uchar    adcPending;
static uchar    isRecording;

static unsigned int adcValue;
static uchar	adcValueToSend;
static uchar    *nextDigit;

/* ------------------------------------------------------------------------- */

const PROGMEM char usbHidReportDescriptor[USB_CFG_HID_REPORT_DESCRIPTOR_LENGTH] = { /* USB report descriptor */
    0x05, 0x01,                    // USAGE_PAGE (Generic Desktop)
    0x09, 0x00,                    // USAGE (undefined)
    0xa1, 0x01,                    // COLLECTION (Application)
    0x15, 0x00,                    //   LOGICAL_MINIMUM (0)
    0x26, 0xff, 0x00,              //   LOGICAL_MAXIMUM (255)
    0x75, 0x08,                    //   REPORT_SIZE (8)
    0x95, 0x04,                    //   REPORT_COUNT (2)
	0x09, 0x00,					   //   USAGE (undefined)
    0x82, 0x02, 0x01,              //   INPUT (Data,Var,Abs,Buf)
    0x75, 0x08,                    //   REPORT_SIZE (8)
    0x95, 0x02,                    //   REPORT_COUNT (2)
    0x09, 0x00,					   //   USAGE (undefined)
    0x92, 0x02, 0x01,              //   OUTPUT (Data,Var,Abs,Buf)
    0xc0                           // END_COLLECTION
};
/* We use a simplifed keyboard report descriptor which does not support the
 * boot protocol. We don't allow setting status LEDs and we only allow one
 * simultaneous key press (except modifiers). We can therefore use short
 * 2 byte input reports.
 * The report descriptor has been created with usb.org's "HID Descriptor Tool"
 * which can be downloaded from http://www.usb.org/developers/hidpage/.
 * Redundant entries (such as LOGICAL_MINIMUM and USAGE_PAGE) have been omitted
 * for the second INPUT item.
 */

/* ------------------------------------------------------------------------- */

static void evaluateADC(unsigned int value)
{
	*(unsigned int *)reportBuffer = value;
	//reportBuffer[0] = 12;
	//reportBuffer[1] = 34;
	adcValueToSend = 1;
}

/* ------------------------------------------------------------------------- */

static void setIsRecording(uchar newValue)
{
    isRecording = newValue;
    if(isRecording){
        PORTB |= 1 << BIT_LED;      /* LED on */
    }else{
        PORTB &= ~(1 << BIT_LED);   /* LED off */
    }
}


/*
	Sets the clock select bits of the Timer/Counter 1 (TCCR1) to the requested value.
	We set bits 3:0 of TCCR1 with the bits 3:0 of data[0]
*/
static void SetClock(uchar *data, uchar len)
{
	if (len == 1)
	{
		TCCR1 = (TCCR1 & 0xF0) | (data[0] & 0x0F);
	}
}

/* ------------------------------------------------------------------------- */

static void keyPoll(void)
{
static uchar    keyMirror;
uchar           key;

    key = PINB & (1 << BIT_KEY);
    if(keyMirror != key){   /* status changed */
        keyMirror = key;
        if(!key){           /* key was pressed */
            setIsRecording(!isRecording);
        }
    }
}

static void adcPoll(void)
{
    if(adcPending && !(ADCSRA & (1 << ADSC))){
        adcPending = 0;
        evaluateADC(ADC);
    }
}

static void timerPoll(void)
{
static uchar timerCnt;

    if(TIFR & (1 << TOV1)){
        TIFR = (1 << TOV1); /* clear overflow */
        keyPoll();
        if(++timerCnt >= 63){       /* ~ 1 second interval */
            timerCnt = 0;
            if(isRecording){
                adcPending = 1;
                ADCSRA |= (1 << ADSC);  /* start next conversion */
            }
        }
    }
}

/* ------------------------------------------------------------------------- */

static void timerInit(void)
{
    TCCR1 = 0x0b;           /* select clock: 16.5M/1k -> overflow rate = 16.5M/256k = 62.94 Hz */
	setIsRecording(1);
}

static void adcInit(void)
{
    ADMUX = UTIL_BIN8(1001, 0011);  /* Vref=2.56V, measure ADC0 */
    ADCSRA = UTIL_BIN8(1000, 0111); /* enable ADC, not free running, interrupt disable, rate = 1/128 */
}

/* ------------------------------------------------------------------------- */
/* ------------------------ interface to USB driver ------------------------ */
/* ------------------------------------------------------------------------- */

uchar   usbFunctionWrite(uchar *data, uchar len)
{
	//eeprom_write_byte(4, len);
	//eeprom_write_block(data, 5, len);
	if(len < 2)
		return 0xFF; // Stall
		
	if (data[0] == 1)
	{
		SetClock(&data[1], len-1);
		return 1;
	}	
	else if (data[0] == 2)
	{
		setIsRecording(data[1] != 0);
		return 1;
	}
	else
	{
		return 0xFF;
	}		
}


uchar	usbFunctionSetup(uchar data[8])
{
usbRequest_t    *rq = (void *)data;

    usbMsgPtr = reportBuffer;
    if((rq->bmRequestType & USBRQ_TYPE_MASK) == USBRQ_TYPE_CLASS){    /* class request type */
        if(rq->bRequest == USBRQ_HID_GET_REPORT){  /* wValue: ReportType (highbyte), ReportID (lowbyte) */
            /* we only have one report type, so don't look at wValue */
            return sizeof(reportBuffer);
		}else if(rq->bRequest == USBRQ_HID_SET_REPORT){
			return USB_NO_MSG;
        }else if(rq->bRequest == USBRQ_HID_GET_IDLE){
            usbMsgPtr = &idleRate;
            return 1;
        }else if(rq->bRequest == USBRQ_HID_SET_IDLE){
            idleRate = rq->wValue.bytes[1];
        }
    }else{
        /* no vendor specific requests implemented */
    }
	return 0;
}

/* ------------------------------------------------------------------------- */
/* ------------------------ Oscillator Calibration ------------------------- */
/* ------------------------------------------------------------------------- */

/* Calibrate the RC oscillator to 8.25 MHz. The core clock of 16.5 MHz is
 * derived from the 66 MHz peripheral clock by dividing. Our timing reference
 * is the Start Of Frame signal (a single SE0 bit) available immediately after
 * a USB RESET. We first do a binary search for the OSCCAL value and then
 * optimize this value with a neighboorhod search.
 * This algorithm may also be used to calibrate the RC oscillator directly to
 * 12 MHz (no PLL involved, can therefore be used on almost ALL AVRs), but this
 * is wide outside the spec for the OSCCAL value and the required precision for
 * the 12 MHz clock! Use the RC oscillator calibrated to 12 MHz for
 * experimental purposes only!
 */
static void calibrateOscillator(void)
{
uchar       step = 128;
uchar       trialValue = 0, optimumValue;
int         x, optimumDev, targetValue = (unsigned)(1499 * (double)F_CPU / 10.5e6 + 0.5);

    /* do a binary search: */
    do{
        OSCCAL = trialValue + step;
        x = usbMeasureFrameLength();    /* proportional to current real frequency */
        if(x < targetValue)             /* frequency still too low */
            trialValue += step;
        step >>= 1;
    }while(step > 0);
    /* We have a precision of +/- 1 for optimum OSCCAL here */
    /* now do a neighborhood search for optimum value */
    optimumValue = trialValue;
    optimumDev = x; /* this is certainly far away from optimum */
    for(OSCCAL = trialValue - 1; OSCCAL <= trialValue + 1; OSCCAL++){
        x = usbMeasureFrameLength() - targetValue;
        if(x < 0)
            x = -x;
        if(x < optimumDev){
            optimumDev = x;
            optimumValue = OSCCAL;
        }
    }
    OSCCAL = optimumValue;
}
/*
Note: This calibration algorithm may try OSCCAL values of up to 192 even if
the optimum value is far below 192. It may therefore exceed the allowed clock
frequency of the CPU in low voltage designs!
You may replace this search algorithm with any other algorithm you like if
you have additional constraints such as a maximum CPU clock.
For version 5.x RC oscillators (those with a split range of 2x128 steps, e.g.
ATTiny25, ATTiny45, ATTiny85), it may be useful to search for the optimum in
both regions.
*/

void    usbEventResetReady(void)
{
    /* Disable interrupts during oscillator calibration since
     * usbMeasureFrameLength() counts CPU cycles.
     */
    cli();
    calibrateOscillator();
    sei();
    eeprom_write_byte(0, OSCCAL);   /* store the calibrated value in EEPROM */
}

/* ------------------------------------------------------------------------- */
/* --------------------------------- main ---------------------------------- */
/* ------------------------------------------------------------------------- */

int main(void)
{
uchar   i;
uchar   calibrationValue;

    calibrationValue = eeprom_read_byte(0); /* calibration value from last time */
    if(calibrationValue != 0xff){
        OSCCAL = calibrationValue;
    }
    odDebugInit();
    usbDeviceDisconnect();
    for(i=0;i<20;i++){  /* 300 ms disconnect */
        _delay_ms(15);
    }
    usbDeviceConnect();
    DDRB |= 1 << BIT_LED;   /* output for LED */
    PORTB |= 1 << BIT_KEY;  /* pull-up on key input */
    wdt_enable(WDTO_1S);
    timerInit();
    adcInit();
    usbInit();
    sei();
    for(;;){    /* main event loop */
        wdt_reset();
        usbPoll();
        if(usbInterruptIsReady() && adcValueToSend != 0){ /* we can send another key */
            usbSetInterrupt(reportBuffer, sizeof(reportBuffer));
			adcValueToSend = 0;
        }
        timerPoll();
        adcPoll();
    }
    return 0;
}
