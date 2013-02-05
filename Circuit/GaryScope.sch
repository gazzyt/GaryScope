EESchema Schematic File Version 2  date 05/02/2013 21:55:47
LIBS:power
LIBS:device
LIBS:transistors
LIBS:conn
LIBS:linear
LIBS:regul
LIBS:74xx
LIBS:cmos4000
LIBS:adc-dac
LIBS:memory
LIBS:xilinx
LIBS:special
LIBS:microcontrollers
LIBS:dsp
LIBS:microchip
LIBS:analog_switches
LIBS:motorola
LIBS:texas
LIBS:intel
LIBS:audio
LIBS:interface
LIBS:digital-audio
LIBS:philips
LIBS:display
LIBS:cypress
LIBS:siliconi
LIBS:opto
LIBS:atmel
LIBS:contrib
LIBS:valves
EELAYER 25  0
EELAYER END
$Descr User 7000 5000
encoding utf-8
Sheet 1 1
Title "ATTINY85 Oscilloscope"
Date "5 feb 2013"
Rev ""
Comp ""
Comment1 ""
Comment2 ""
Comment3 ""
Comment4 ""
$EndDescr
Wire Wire Line
	1900 2350 2800 2350
Wire Wire Line
	1900 1050 1900 2350
Wire Wire Line
	2800 2650 1900 2650
Wire Wire Line
	1800 1050 1800 2450
Wire Wire Line
	1800 2450 2800 2450
Wire Wire Line
	2200 2750 2200 3150
Wire Wire Line
	2200 2750 2300 2750
Connection ~ 3700 1250
Wire Wire Line
	3700 1250 3700 1350
Wire Wire Line
	3700 1350 2000 1350
Wire Wire Line
	2000 1350 2000 1050
Wire Wire Line
	5600 2700 5600 3150
Wire Wire Line
	5600 3150 4200 3150
Connection ~ 1600 3950
Wire Wire Line
	1600 3050 1600 3950
Wire Wire Line
	2800 2550 2200 2550
Connection ~ 3200 1750
Wire Wire Line
	1000 1750 1000 3950
Wire Wire Line
	1000 1750 5600 1750
Wire Wire Line
	5600 1750 5600 2300
Wire Wire Line
	4100 1700 4100 1750
Wire Wire Line
	5500 2750 5800 2750
Wire Wire Line
	5950 850  5950 950 
Wire Wire Line
	3500 1050 3500 1250
Connection ~ 5600 2750
Wire Wire Line
	3500 1250 5800 1250
Wire Wire Line
	5500 2250 5600 2250
Connection ~ 5600 2250
Wire Wire Line
	5700 850  5700 950 
Connection ~ 4100 1250
Wire Wire Line
	4100 1300 4100 1250
Wire Wire Line
	2800 2750 2800 2750
Wire Wire Line
	5800 1250 5800 2750
Wire Wire Line
	3200 1750 3200 1050
Connection ~ 4100 1750
Wire Wire Line
	2200 3350 2200 3950
Wire Wire Line
	2200 3950 1000 3950
Wire Wire Line
	1600 2550 1700 2550
Wire Wire Line
	1600 2550 1600 2850
Wire Wire Line
	2200 3150 3700 3150
Wire Wire Line
	1700 1750 1700 1050
Connection ~ 1700 1750
Wire Wire Line
	1900 3150 1900 3450
Wire Wire Line
	3400 1050 3400 2050
Wire Wire Line
	3400 2050 2300 2050
Wire Wire Line
	2300 2050 2300 2750
Wire Wire Line
	3300 1050 3300 1950
Wire Wire Line
	3300 1950 1700 1950
Wire Wire Line
	1700 1950 1700 2550
Wire Wire Line
	1900 3850 1900 3950
Connection ~ 1900 3950
NoConn ~ 2800 2250
$Comp
L R R4
U 1 1 510E4C8D
P 1900 2900
F 0 "R4" V 1980 2900 50  0000 C CNN
F 1 "220R" V 1900 2900 50  0000 C CNN
	1    1900 2900
	-1   0    0    1   
$EndComp
$Comp
L LED D3
U 1 1 510E4C71
P 1900 3650
F 0 "D3" H 1900 3750 50  0000 C CNN
F 1 "LED" H 1900 3550 50  0000 C CNN
	1    1900 3650
	0    -1   -1   0   
$EndComp
$Comp
L R R3
U 1 1 510E4B65
P 3950 3150
F 0 "R3" V 4030 3150 50  0000 C CNN
F 1 "2K2" V 3950 3150 50  0000 C CNN
	1    3950 3150
	0    -1   -1   0   
$EndComp
$Comp
L R R2
U 1 1 510E4AC3
P 1950 2550
F 0 "R2" V 2030 2550 50  0000 C CNN
F 1 "68R" V 1950 2550 50  0000 C CNN
	1    1950 2550
	0    -1   -1   0   
$EndComp
Text Label 3200 1050 2    60   ~ 0
GND
$Comp
L ZENERSMALL D2
U 1 1 50F4215F
P 1600 2950
F 0 "D2" H 1600 3050 40  0000 C CNN
F 1 "3V6" H 1600 2850 30  0000 C CNN
	1    1600 2950
	0    -1   -1   0   
$EndComp
$Comp
L ZENERSMALL D1
U 1 1 50F4215A
P 2200 3250
F 0 "D1" H 2200 3350 40  0000 C CNN
F 1 "3V6" H 2200 3150 30  0000 C CNN
	1    2200 3250
	0    -1   -1   0   
$EndComp
$Comp
L R R1
U 1 1 50F420AC
P 2550 2750
F 0 "R1" V 2630 2750 50  0000 C CNN
F 1 "68R" V 2550 2750 50  0000 C CNN
	1    2550 2750
	0    -1   -1   0   
$EndComp
$Comp
L CP1 C2
U 1 1 50F411C6
P 4100 1500
F 0 "C2" H 4150 1600 50  0000 L CNN
F 1 "47uF" H 4150 1400 50  0000 L CNN
	1    4100 1500
	1    0    0    -1  
$EndComp
Text Label 5700 950  0    60   ~ 0
VCC
Text Label 5950 950  0    60   ~ 0
GND
$Comp
L PWR_FLAG #FLG01
U 1 1 50F40F1F
P 5950 850
F 0 "#FLG01" H 5950 945 30  0001 C CNN
F 1 "PWR_FLAG" H 5950 1030 30  0000 C CNN
	1    5950 850 
	1    0    0    -1  
$EndComp
Text Label 3500 1150 0    60   ~ 0
VCC
$Comp
L PWR_FLAG #FLG02
U 1 1 50F40EB0
P 5700 850
F 0 "#FLG02" H 5700 945 30  0001 C CNN
F 1 "PWR_FLAG" H 5700 1030 30  0000 C CNN
	1    5700 850 
	1    0    0    -1  
$EndComp
$Comp
L C C1
U 1 1 50F40799
P 5600 2500
F 0 "C1" H 5650 2600 50  0000 L CNN
F 1 "100n" H 5650 2400 50  0000 L CNN
	1    5600 2500
	1    0    0    -1  
$EndComp
$Comp
L CONN_4 P1
U 1 1 50F40254
P 1850 700
F 0 "P1" V 1800 700 50  0000 C CNN
F 1 "CONN_4" V 1900 700 50  0000 C CNN
	1    1850 700 
	0    -1   -1   0   
$EndComp
$Comp
L USB_2 J1
U 1 1 50F4021D
P 3350 850
F 0 "J1" H 3275 1100 60  0000 C CNN
F 1 "USB_2" H 3400 550 60  0001 C CNN
F 4 "VCC" H 3675 1000 50  0001 C CNN "VCC"
F 5 "D+" H 3650 900 50  0001 C CNN "Data+"
F 6 "D-" H 3650 800 50  0001 C CNN "Data-"
F 7 "GND" H 3675 700 50  0001 C CNN "Ground"
	1    3350 850 
	0    1    1    0   
$EndComp
$Comp
L ATTINY85-P IC1
U 1 1 50F401D7
P 3000 2850
F 0 "IC1" H 3100 2900 60  0000 C CNN
F 1 "ATTINY85-P" H 5000 2100 60  0000 C CNN
F 2 "DIP8" H 3100 2100 60  0001 C CNN
	1    3000 2850
	1    0    0    1   
$EndComp
$EndSCHEMATC
