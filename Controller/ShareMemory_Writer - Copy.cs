using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace Final_MMF_Writer.Controller
{

    public class Syncronized
    {
        public static Mutex nameDMutex = null;
        public static bool isMutexOwned = false;
        public static bool isMutexCreated = false;

        public static ManualResetEvent ManualEvent = new ManualResetEvent(false);
        public static bool localEventAvail = false;

        public static SemaphoreSlim _semaPhoreSlim = new SemaphoreSlim(0);
        public static bool isSemaphoreAvail = false;

        public static long releaseLocalMutex = 0;
        public static long afterReleaseMutex = 0;

        public static long _timeTicksToWrite = 0;

    }

    public class ShareMemory_Writer
    {
        private int pagePadding { get; set; }
        private int pageCount { get; set; }
        private int FileSize { get; set; }
        private string FileName { get; set; }

        private int writeCount { get; set; } = 0;

        private MemoryMappedFile shMemory { get; set; }
        private MemoryMappedViewAccessor shMemoryAccessor { get; set; }
        public ShareMemory_Writer(string name, int pagePadding = 40, int FileSize = 10000)
        {
            FileName = name;
            this.pagePadding = pagePadding;
            this.pageCount = 25;////FileSize / pagePadding;
            this.FileSize = FileSize;
            Create_MMF(FileName);

        }
        private void Create_MMF(string name)
        {
            shMemory = MemoryMappedFile.CreateNew(name, FileSize);
            Syncronized.nameDMutex = new Mutex(Syncronized.isMutexOwned, name + "mutex", out Syncronized.isMutexCreated);
            shMemoryAccessor = shMemory.CreateViewAccessor();
            Console.WriteLine($"Memory map file {name } has been created : Named mutex and accessor also created");
            Console.WriteLine($"Memory map file size {FileSize} , page count {pageCount}");
           
        }
        public void write_Bytes(byte[] writeableBytes , int i)
        {
            Syncronized.nameDMutex.WaitOne();
            //long writableLong = Syncronized.releaseLocalMutex;
            //var integer_value = /*(int)Syncronized.releaseLocalMutex*/ (Syncronized._timeTicksToWrite+=1);
            //double data = 1000000000.0 * (double)Syncronized.releaseLocalMutex / Stopwatch.Frequency;
            //writeableBytes = BitConverter.GetBytes(integer_value);

            writeCount = writeCount == FileSize ? 0 : writeCount;
            //var start = new Stopwatch(); start.Start();
             shMemoryAccessor.WriteArray<byte>(writeCount, writeableBytes, 0, writeableBytes.Length);
           // shMemoryAccessor.Write<long>(writeCount, ref writableLong);
            //Console.WriteLine($"Wrote {writeableBytes.Length}  {writeCount}");
            //start.Stop(); Console.WriteLine(start.Elapsed);
            writeCount = writeCount + pageCount;

            Syncronized.nameDMutex.ReleaseMutex();

            Syncronized.isMutexOwned = Syncronized.isMutexOwned == true ? false : true;

            Syncronized.nameDMutex.WaitOne();
            Syncronized.isMutexOwned = true;

            Syncronized.isSemaphoreAvail = false;
            Syncronized._semaPhoreSlim.Wait();

            //Syncronized.localEventAvail = false;
            //Syncronized.ManualEvent.WaitOne();
            Syncronized.afterReleaseMutex = Stopwatch.GetTimestamp();

            //Console.WriteLine("Release " + (1000000000.0 * (double)(Syncronized.afterReleaseMutex - Syncronized.releaseLocalMutex) / Stopwatch.Frequency) * 0.001);



        }




    }
}
