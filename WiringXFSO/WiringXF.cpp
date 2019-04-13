#include <wiringPi.h>
#include <wiringPiI2C.h>

extern "C" {

#pragma region Setup 4
	int wiringPiSetup_(void) {
		wiringPiSetup();
	}
	int wiringPiSetupGpio_(void) {
		wiringPiSetupGpio();
	}
	int wiringPiSetupPhys_(void) {
		wiringPiSetupPhys();
	}
	int wiringPiSetupSys_(void) {
		wiringPiSetupSys();
	}
#pragma endregion



#pragma region Core Functions 7
	void pinMode_(int pin, int mode) {
		pinMode(pin, mode);
	}
	void pullUpDnControl_(int pin, int pud) {
		pullUpDnControl(pin, pud);
	}
	void digitalWrite_(int pin, int value) {
		digitalWrite(pin, value);
	}
	void pwmWrite_(int pin, int value) {
		pwmWrite(pin, value);
	}
	int digitalRead_(int pin) {
		return digitalRead(pin);
	}
	void analogRead_(int pin) {
		analogRead(pin);
	}
	void analogWrite_(int pin, int value) {
		analogWrite(pin, value);
	}

#pragma endregion



#pragma region Raspberry Pi Specifics 8
	void digitalWriteByte_(int value) {
		digitalWriteByte(value);
	}
	void pwmSetMode_(int mode) {
		pwmSetMode(mode);
	}
	void pwmSetRange_(unsigned int range) {
		pwmSetRange(range);
	}
	void pwmSetClock_(int divisor) {
		pwmSetClock(divisor);
	}
	void piBoardRev_(void) {
		piBoardRev();
	}
	void wpiPinToGpio_(int wPiPin) {
		wpiPinToGpio(wPiPin);
	}
	void physPinToGpio_(int physPin) {
		physPinToGpio(physPin);
	}
	void setPadDrive_(int group, int value) {
		setPadDrive(group, value);
	}


#pragma endregion



#pragma region Timing 4

	unsigned int millis_(void) {
		return millis();
	}
	unsigned int micros_(void) {
		return micros();
	}
	void delay_(unsigned int howLong) {
		delay(howLong);
	}
	void delayMicroseconds_(unsigned int howLong) {
		delayMicroseconds(howLong);
	}




#pragma endregion


#pragma region Priority, Interrupts and Threads
	int piHiPri_(int priority) {
		return piHiPri(priority);
	}
	int waitForInterrupt_(int pin, int timeOut) {
		return waitForInterrupt(pin, timeOut);
	}
	void piLock_(int keyNum) {
		piLock(keyNum);
	}
	void piUnlock_(int keyNum) {
		piUnlock(keyNum);
	}




#pragma endregion


#pragma region I2C Library

	int wiringPiI2CSetup_(int devId) {
		return wiringPiI2CSetup(devId);
	}
	int wiringPiI2CRead_(int fd) {
		return wiringPiI2CRead(fd);
	}
	int wiringPiI2CWrite_(int fd, int data) {
		return wiringPiI2CWrite(fd, data);
	}
	int wiringPiI2CWriteReg8_(int fd, int reg, int data) {
		return wiringPiI2CWriteReg8(fd, reg, data);
	}
	int wiringPiI2CWriteReg16_(int fd, int reg, int data) {
		return wiringPiI2CWriteReg16(fd, reg, data);
	}
	int wiringPiI2CReadReg8_(int fd, int reg) {
		return wiringPiI2CReadReg8(fd, reg);
	}
	int wiringPiI2CReadReg16_(int fd, int reg) {
		return wiringPiI2CReadReg16(fd, reg);
	}

#pragma endregion

	
	void testLed(int LED)
	{

		wiringPiSetupSys();
		pinMode(LED, OUTPUT);

		while (true)
		{
			digitalWrite(LED, HIGH);  
			delay(500); 
			digitalWrite(LED, LOW);	  
			delay(500);
		}
	}
}
