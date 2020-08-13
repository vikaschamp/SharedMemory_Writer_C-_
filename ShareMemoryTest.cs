using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Final_MMF_Writer.Controller;
namespace Final_MMF_Writer
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct Test
    {
        public long id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string name;
    }

    public class ShareMemoryTest
    {


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


        private ShareMemory_Writer _shreMemory = null;
        public ShareMemoryTest()
        {

            //Task.Run(() =>
            //{

            //    WriteSignalToWriter();
            //});

            _shreMemory = new ShareMemory_Writer("sharedMemoryTest");

            Console.WriteLine("Press <Enter> to start writing");
            Console.ReadLine();


            WriteThroughSHM();

            //Task.Run(() =>
            //{
            //    WriteData();
            //});

            //Task.Run(() =>
            //{

            //    WriteSignalToWriter();
            //});
            //Task.Run(() =>
            //{

            //    WriteData("Parrler thread");
            //});
            //  WriteData("Naib thread");

        }
        private void WriteData(string threadName)
        {

            for (int i = 1; i <= 100000; i++)
            {


                Test test = new Test();
                test.id = i;
                test.name = "vikas " + i;

                string data = "Hello Bhai";
                data += i;
                //Console.WriteLine("From " + Syncronized.releaseLocalMutex);

                var data2 = 1000000000.0 * Syncronized.releaseLocalMutex / Stopwatch.Frequency;
                var data1 = (long)data2;
                var bytes = Marshle_Byte_Convertion(test);/* BitConverter.GetBytes(data1)*/;

                Console.WriteLine("From " + test.id + " " + test.name);

                _shreMemory.write_Bytes(/*Encoding.ASCII.GetBytes(data)*/ bytes, i);
            }

        }

        private void WriteSignalToWriter()
        {

            for (int i = 0; i <= 100000000000000000; i++)
            {
                //if (!Syncronized.localEventAvail)
                {
                    Thread.Sleep(2000);
                    //Syncronized.handle.Set();
                    //Syncronized.handle.Reset();
                    //Syncronized._semaPhoreSlim.Release();
                    ////Syncronized.ManualEvent.Set();
                    ////Syncronized.ManualEvent.Reset();
                    //Syncronized.releaseLocalMutex = Stopwatch.GetTimestamp();



                }

            }



        }

        private void WriteThroughSHM()
        {
            while (true)
            {
                foreach (var value in Syncronized.sharedMemory)
                {
                    Test test = new Test();
                    var data= 1000000000.0 * Stopwatch.GetTimestamp() / Stopwatch.Frequency;
                    test.id = (long)data;
                    test.name = "vikas " + value.Key;

                    var bytes = Marshle_Byte_Convertion(test);/* BitConverter.GetBytes(data1)*/;

                    // Console.WriteLine("From " + test.id + " " + test.name);
                    value.Value.write_Bytes(bytes, value.Key);
                    Thread.Sleep(1000);
                }


            }

        }
    }
}
