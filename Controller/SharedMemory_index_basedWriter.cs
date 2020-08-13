using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Final_MMF_Writer.Controller
{



    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct Request
    {
        public byte RequestType;
        public Int64 ClOrderID;
        public Int64 OriginatorClOrderID;
        public Int64 OrderQuantity;
        public Int64 Price;
        public byte OrderType;
        public byte OrderSide;
        public byte TimeInForce;
        public byte SHFEComboOffsetFlag;
        public byte SecurityType;
        public fixed byte Symbol[10];
        public fixed byte SecurityID[25];
        public fixed byte ExchangeOrderID[40];

    };
    public class SharedMemory_index_basedWriter
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern unsafe void CopyMemory(void* dest, void* src, int count);
        private MemoryMappedViewAccessor shMemoryAccessor { get; set; }
        private long positionToWrite = 0;
        private EventWaitHandle handel;

        private Request[] requests = null;
        public unsafe byte[] Unsafe_Byte_Convertion()
        {
            var buffer = new byte[Marshal.SizeOf(requests[0])];
            fixed (void* destination = &buffer[0])
            {
                fixed (void* source = &requests[0])
                {
                    CopyMemory(destination, source, buffer.Length);
                }
            }


            return buffer;
        }

        public static byte[] Marshle_Byte_Convertion(Test req_structure)
        {
            try
            {

                int size = Marshal.SizeOf(req_structure);
                byte[] arr = new byte[size];

                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(req_structure, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
                Marshal.FreeHGlobal(ptr);

                return arr;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace + "\n" + ex.Message);
            }


            return null;
        }

        public void Symbol_SecurityID_Bytes(Request request, string Symbol, string SecurityId, string ExchangeOrderID)
        {

            unsafe
            {
                for (int i = 0; i < Symbol.Length; i++)
                {
                    request.Symbol[i] = (byte)Symbol[i];

                }
                for (int i = 0; i < SecurityId.Length; i++)
                {
                    request.SecurityID[i] = (byte)SecurityId[i];
                }

                for (int i = 0; i < ExchangeOrderID.Length; i++)
                {
                    request.ExchangeOrderID[i] = (byte)ExchangeOrderID[i];
                }

            }
            requests = new Request[1];
            requests[0] = request;
        }

        private Request InitializeStruct()
        {
            Request request = new Request
            {
                RequestType = (byte)'D',
                ClOrderID = 867868734,
                OrderQuantity = 23,
                OrderSide = 1,
                Price = 356416523456412,
                OrderType = 1,
                OriginatorClOrderID = 26537652,
                SecurityType = 1,
                SHFEComboOffsetFlag = 1,
                TimeInForce = 1
            };

            return request;
        }

        public void write_Bytes(byte[] writeableBytes, int i)
        {
            try
            {
                var startWrite = Stopwatch.GetTimestamp();
                startWrite = (long)(1000000000.0 * startWrite / Stopwatch.Frequency);

                var requestStruct = InitializeStruct();
                requestStruct.ClOrderID = startWrite;
                Symbol_SecurityID_Bytes(requestStruct, "fsdfs", "sfwfwe", "876283763");
                var bytes = Unsafe_Byte_Convertion();

                //Test test = new Test();
                //var data = Stopwatch.GetTimestamp();

                //test.id = (long)(1000000000.0 * data / Stopwatch.Frequency);
                //test.name = "vikas " + i;

                ////var bytes = Marshle_Byte_Convertion(InitializeStruct());
                //var bytes = Marshle_Byte_Convertion(test);
                shMemoryAccessor.WriteArray<byte>(0, bytes, 0, bytes.Length);


                var finishWrite = (long)(1000000000.0 * Stopwatch.GetTimestamp() / Stopwatch.Frequency);
                
                handel.Set();
                handel.Reset();
                var finishSet = (long)(1000000000.0 * Stopwatch.GetTimestamp() / Stopwatch.Frequency);
                // Console.WriteLine("Write on pos " + positionToWrite + " id " + test.id + " name " + test.name + " finish write " + (finishWrite - test.id) * 0.001 + " finish set " + (finishSet - finishWrite) * 0.001);
              Console.WriteLine("Write on pos " + positionToWrite + " id " + requestStruct.ClOrderID + " name " + requestStruct.OrderQuantity + " finish write " + (finishWrite -startWrite) * 0.001 + " finish set " + (finishSet - finishWrite) * 0.001);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }
        public SharedMemory_index_basedWriter(MemoryMappedViewAccessor accessor, long position, EventWaitHandle handle)
        {
            this.handel = handle;
            this.positionToWrite = position;
            this.shMemoryAccessor = accessor;
        }


    }
}
