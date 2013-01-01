using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Windows;

namespace Total_DMX_Control_WPF
{
    public class OpenDMXUSB2
    {
        //this should be true when you don't want to deal with actually connecting to the device
        //false otherwise
        private bool debugging = false;

        #region DLL Import
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_Open(short intDeviceNumber, ref int lngHandle);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_Close(int lngHandle);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_SetDivisor(int lngHandle, int div);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_Read(int lngHandle, string lpszBuffer, int lngBufferSize, ref int lngbytesReturned);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_Write(int lngHandle, string lpszBuffer, int lngBufferSize, ref int lngbytesWritten);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_SetBaudRate(int lngHandle, int lngBaudRate);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_SetDataCharacteristics(int lngHandle, byte byWordLength, byte byStopBits, byte byParity);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_SetFlowControl(int lngHandle, short intFlowControl, byte byXonChar, byte byXoffChar);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_ResetDevice(int lngHandle);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_SetDtr(int lngHandle);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_ClrDtr(int lngHandle);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_SetRts(int lngHandle);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_ClrRts(int lngHandle);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_GetModemStatus(int lngHandle, ref int lngModemStatus);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_Purge(int lngHandle, int lngMask);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_GetStatus(int lngHandle, ref int lngRxbytes, ref int lngTxbytes, ref int lngEventsDWord);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_GetQueueStatus(int lngHandle, ref int lngRxbytes);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_GetEventStatus(int lngHandle, ref int lngEventsDWord);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_SetChars(int lngHandle, byte byEventChar, byte byEventCharEnabled, byte byErrorChar, byte byErrorCharEnabled);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_SetTimeouts(int lngHandle, int lngReadTimeout, int lngWriteTimeout);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_SetBreakOn(int lngHandle);
        [DllImport("FTD2XX.DLL")]
        private static extern int FT_SetBreakOff(int lngHandle);
        #endregion

        private byte[] dmxArray = new byte[512];
        private System.Threading.Thread dmxThread = null;
        private short deviceIndex = 0;
        private int deviceHandle = 0;
        private bool connected = false;
        private int frameIntervalMs = 25;
        private byte startCode;
        private string lastError = string.Empty;
        private string writeBuffer = string.Empty;
        private int bytesWritten = 0;
        bool freezeThread = false;
        bool connectionOpen = false;

        const short FT_OK = 0;
        const short FT_INVALID_HANDLE = 1;
        const short FT_DEVICE_NOT_FOUND = 2;
        const short FT_DEVICE_NOT_OPENED = 3;
        const short FT_IO_ERROR = 4;
        const short FT_INSUFFICIENT_RESOURCES = 5;

        const short FT_BITS_8 = 8;
        // Word Lengths
        const short FT_STOP_BITS_2 = 2;
        // Stop Bits
        const short FT_PARITY_NONE = 0;
        // Parity
        const short FT_FLOW_NONE = 0x0;
        // Flow Control
        const short FT_PURGE_RX = 1;
        // Purge rx and tx buffers
        const short FT_PURGE_TX = 2;

        // This method needs to be called before the device can be used.
        // The controller's Start method calls this.
        public void Start()
        {
            connectionOpen = true;
            if (!debugging) InitializeDMXDevice();
            else MessageBox.Show("Note: Debugging Mode active, dmx device will not function.");

            dmxThread = new Thread(DmxThread);
            dmxThread.IsBackground = true;
            dmxThread.Start();
        }

        public void Stop()
        {
            connectionOpen = false;
        }

        private void InitializeDMXDevice()
        {

            connected = false;
            // not connected

            frameIntervalMs = 25;
            // default 30 ms between each frame
            startCode = 0;
            // default startcode of 0
            lastError = "";
            // no error has happened yet

            // ==== ATTEMPT TO OPEN DEVICE ====
            if (FT_Open(deviceIndex, ref deviceHandle) != FT_OK)
            {
                lastError = "Not Found";
                throw new Exception(lastError);
                return;
            }
            // ==== PREPARE DEVICE FOR DMX TRANSMISSION ====
            // reset the device
            if (FT_ResetDevice(deviceHandle) != FT_OK)
            {
                lastError = "Failed To Reset Device!";
                throw new Exception(lastError);
                return;
            }

            // get an ID from the widget from jumpers
            GetID();

            // set the baud rate
            if (FT_SetDivisor(deviceHandle, 12) != FT_OK)
            {
                lastError = "Failed To Set Baud Rate!";
                throw new Exception(lastError);
                return;
            }
            // shape the line
            if (FT_SetDataCharacteristics(deviceHandle, Convert.ToByte(FT_BITS_8), Convert.ToByte(FT_STOP_BITS_2), Convert.ToByte(FT_PARITY_NONE)) != FT_OK)
            {
                lastError = "Failed To Set Data Characteristics!";
                throw new Exception(lastError);
                return;
            }
            // no flow control
            if (FT_SetFlowControl(deviceHandle, FT_FLOW_NONE, 0, 0) != FT_OK)
            {
                lastError = "Failed to set flow control!";
                throw new Exception(lastError);
                return;
            }
            // set bus transiever to transmit enable
            if (FT_ClrRts(deviceHandle) != FT_OK)
            {
                lastError = "Failed to set RS485 to send!";
                throw new Exception(lastError);
                return;
            }
            // Clear TX & RX buffers
            if (FT_Purge(deviceHandle, FT_PURGE_TX) != FT_OK)
            {
                lastError = "Failed to purge TX buffer!";
                throw new Exception(lastError);
                return;
            }
            // empty buffers
            if (FT_Purge(deviceHandle, FT_PURGE_RX) != FT_OK)
            {
                lastError = "Failed to purge RX buffer!";
                throw new Exception(lastError);
                return;
            }
            writeBuffer = "";
            // clear the software buffer
            bytesWritten = 0;
            // clear byte counter

            // Success
            connected = true;
            // device connected

            // start/resume threads

            lastError = "Starting Thread";
            dmxThread = new Thread(DmxThread);
            dmxThread.IsBackground = true;
            dmxThread.Start();
            // start the thread
            // highest priority
            dmxThread.Priority = ThreadPriority.AboveNormal;
        }

        private void DmxThread()
        {
            while (connectionOpen)
            {
                if (!freezeThread)
                {
                    if (!debugging) SendDMX();
                }

                System.Threading.Thread.Sleep(frameIntervalMs);
            }
        }

        public void SetDMXChannel(int oneBasedChannel, int value)
        {
            dmxArray[oneBasedChannel - 1] = Convert.ToByte(value);
            SetDMXUniverse(dmxArray);
        }

        public void SetDMXUniverse(byte[] dmxBytes)
        {
            dmxArray = dmxBytes;
            //writeBuffer = Microsoft.VisualBasic.Strings.StrConv(System.Text.ASCIIEncoding.Unicode.GetString(dmxArray), 0, 0);
            writeBuffer = System.Text.Encoding.Default.GetString(dmxBytes);
        }

        // Zach added this method
        // Used when a dimmer wants to know its level
        public int GetDMXValue(int oneBasedChannel)
        {
            return Convert.ToInt32(dmxArray[oneBasedChannel - 1]);
        }

        private void SendDMX()
        {
            if (!connected) { return; }

            // break
            FT_SetBreakOn(deviceHandle);
            System.Threading.Thread.Sleep(1);
            FT_SetBreakOff(deviceHandle);
            
            /*
             * Zach messed with this part.
             * This is how we actually implement blackout.
             * Everytime the array of dmx channel levels is sent (about every few milliseconds) it checks to see if the board is in blackout mode.
             */ 
            string buffer;
            if (DMXController.Blackout)
            {
                buffer = Microsoft.VisualBasic.Strings.Chr(startCode).ToString() + System.Text.Encoding.Default.GetString(new byte[512]);
            }

            else
            {
                buffer = Microsoft.VisualBasic.Strings.Chr(startCode).ToString() + writeBuffer;
            }
            //End Zach messing with

            // write entire thing
            if (FT_Write(deviceHandle, buffer, buffer.Length, ref bytesWritten) != FT_OK)
            {
                lastError = "Failed to Write Startcode!";
                throw new Exception(lastError);
            }

            return;

            // write start code
            if (FT_Write(deviceHandle, Microsoft.VisualBasic.Strings.Chr(startCode).ToString(), 1, ref bytesWritten) != FT_OK)
            {
                lastError = "Failed to Write Startcode!";
                throw new Exception(lastError);
            }

            // n = JumperID(n) ' chose which universe from jumper settings

            // write dmx data
            if (FT_Write(deviceHandle, writeBuffer, writeBuffer.Length, ref bytesWritten) != FT_OK)
            {
                lastError = "Failed To Write DMX";
                throw new Exception(lastError);

            }
        }

        private int GetID()
        {
            int ModemStatus = 0;
            // CTS DSR ID
            // 0 0 = 0
            // 0 1 = 1
            // 1 0 = 2
            // 1 1 = 3

            int jumperId = -1;

            if (FT_GetModemStatus(deviceHandle, ref ModemStatus) != FT_OK)
            {
                lastError = "Failed To Get Modem Status";
                throw new Exception(lastError);
            }

            if ((ModemStatus == 0))
            {
                // ID 0
                jumperId = 0;
            }

            if ((ModemStatus == 32))
            {
                // ID 1
                jumperId = 1;
            }

            if ((ModemStatus == 16))
            {
                // id 2
                jumperId = 2;
            }

            if ((ModemStatus == 48))
            {
                // id 4
                jumperId = 4;
            }

            if (connected)
            {
                if (jumperId == 0)
                {
                    jumperId = deviceIndex;
                }
            }
            // return the id of the device

            return jumperId;
        }
    }
}
