using System;
using System.Runtime.InteropServices;
namespace WiringXF
{
    /// <summary>
    /// Setup 函数，用于初始化引脚
    /// </summary>
    public class wiringPi_Setup{
        #region Setup 4

        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wiringPiSetup_();
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wiringPiSetupGpio_();
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wiringPiSetupPhys_();
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wiringPiSetupSys_();
        #endregion
    }


    /// <summary>
    /// 引脚核心函数
    /// </summary>
    public class wiringPi_Core
    {
        #region Core Functions 7

        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void pinMode_(int pin, int mode);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void pullUpDnControl_(int pin, int pud);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void digitalWrite_(int pin, int value);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void pwmWrite_(int pin, int value);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int digitalRead_(int pin);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void analogRead_(int pin);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void analogWrite_(int pin, int value);

        #endregion
    }

    /// <summary>
    /// 树莓派特有的函数
    /// </summary>
    public class wiringPi_Raspberry
    {

        #region Raspberry Pi Specifics 8
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void digitalWriteByte_(int value);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void pwmSetMode_(int mode);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void pwmSetRange_(uint range);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void pwmSetClock_(int divisor);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void piBoardRev_();
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wpiPinToGpio_(int wPiPin);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void physPinToGpio_(int physPin);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setPadDrive_(int group, int value);


        #endregion
    }




    /// <summary>
    /// 用于设置时间，例如 ：delay 休眠一段时间
    /// </summary>
    public class wiringPi_Timing
    {
        #region Timing 4

        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint millis_();
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint micros_();
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void delay_(uint howLong);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void delayMicroseconds_(uint howLong);




        #endregion
    }

    /// <summary>
    /// 用于处理线程、调度、优先级
    /// </summary>
    public class wiringPi_Threads
    {
        #region Priority, Interrupts and Threads


        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int piHiPri_(int priority);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int waitForInterrupt_(int pin, int timeOut);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void piLock_(int keyNum);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void piUnlock_(int keyNum);




        #endregion
    }

    /// <summary>
    /// I2C 函数
    /// </summary>
    public class wiringPi_I2C
    {
        #region I2C Library
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wiringPiI2CSetup_(int devId);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wiringPiI2CRead_(int fd);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wiringPiI2CWrite_(int fd, int data);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wiringPiI2CWriteReg8_(int fd, int reg, int data);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wiringPiI2CWriteReg16_(int fd, int reg, int data);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wiringPiI2CReadReg8_(int fd, int reg);
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int wiringPiI2CReadReg16_(int fd, int reg);

        #endregion
    }


    /// <summary>
    /// 一些示例
    /// </summary>
    public class wiringPi_Demo
    {
        #region Demo
        /// <summary>
        /// 点亮一个灯 ，默认 BCM 为 17
        /// </summary>
        /// <param name="LED"></param>
        [DllImport("./WiringXF.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void testLed(int LED = 17);
        #endregion
    }
}