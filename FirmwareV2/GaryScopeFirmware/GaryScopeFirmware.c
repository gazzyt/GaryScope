/*
 * GaryScopeFirmware.c
 *
 * Created: 24/08/2015 13:00:07
 *  Author: garyt
 */ 

#include <avr/interrupt.h>
#include <avr/io.h>
#include <avr/wdt.h>

#include "usbdrv.h"

const PROGMEM char usbHidReportDescriptor[USB_CFG_HID_REPORT_DESCRIPTOR_LENGTH] = { /* USB report descriptor */
	0x06, 0x56, 0xFF,                    // USAGE_PAGE (Generic Desktop)
	0x09, 0xA6,                    // USAGE (undefined)
	0xa1, 0x01,                    // COLLECTION (Application)
	0x09, 0xA7,					   //   USAGE (undefined)
	0x15, 0x00,                    //   LOGICAL_MINIMUM (0)
	0x26, 0xff, 0x00,              //   LOGICAL_MAXIMUM (255)
	0x75, 0x08,                    //   REPORT_SIZE (8)
	0x95, 20,                    //   REPORT_COUNT (2)
	0x82, 0x02, 0x01,              //   INPUT (Data,Var,Abs,Buf)
	0xc0                           // END_COLLECTION
};

void hadUsbReset(void)
{
	cli();
	calibrateOscillator();
	sei();
}

int __attribute__((noreturn)) main(void)
{
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

    while(1)
    {
        //TODO:: Please write your application code 
    }
}