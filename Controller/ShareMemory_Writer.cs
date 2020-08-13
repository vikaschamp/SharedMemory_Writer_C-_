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
using System.Collections.Concurrent;

namespace Final_MMF_Writer.Controller
{

    public class Syncronized
    {

        public static ConcurrentDictionary<int, SharedMemory_index_basedWriter> sharedMemory = new ConcurrentDictionary<int, SharedMemory_index_basedWriter>();

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
        public static EventWaitHandle handle = null;

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
        public ShareMemory_Writer(string name, int pagePadding = 10, int FileSize = 1200)
        {
            FileName = name;
            this.pagePadding = pagePadding;
            this.pageCount = FileSize / pagePadding;
            this.FileSize = FileSize;
            Create_MMF(FileName);

        }
        private void Create_MMF(string name)
        {

            shMemory = MemoryMappedFile.CreateNew(name, FileSize);
            int accesorSize = 120;
            shMemoryAccessor = shMemory.CreateViewAccessor();

            for (int i = 0; i < FileSize; i++)
            {
                try
                {
                    Syncronized.sharedMemory.TryAdd(i, new SharedMemory_index_basedWriter(shMemory.CreateViewAccessor(writeCount, accesorSize), writeCount, new EventWaitHandle(false, EventResetMode.ManualReset, ($"{name}mutex{i}"))));
                    Console.WriteLine($"Accessor is ready :  Dict index {i} pos {writeCount} handel {name}mutex{i} accessor {accesorSize} ");
                    writeCount += pageCount;
                    accesorSize += pageCount;
                    if (writeCount == FileSize)
                        break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                }
            }






            //Syncronized.handle = new EventWaitHandle(false, EventResetMode.ManualReset, "testRemoteServer");

            //Console.WriteLine("Go baby GO");
            //Syncronized.nameDMutex = new Mutex(Syncronized.isMutexOwned, name + "mutex", out Syncronized.isMutexCreated);
            //Console.WriteLine($"Memory map file {name } has been created : Named mutex and accessor also created");
            //Console.WriteLine($"Memory map file size {FileSize} , page count {pageCount}");

            //if (!Syncronized.isMutexOwned) Syncronized.nameDMutex.WaitOne();
            //Thread.Sleep(10000);
            //Syncronized.nameDMutex.ReleaseMutex();
        }
        public void write_Bytes(byte[] writeableBytes, int i)
        {
            //  Stopwatch stopwatch = new Stopwatch(); stopwatch.Start();

            //var _enable_start = Stopwatch.GetTimestamp();
            //double data = 1000000000.0 * (double)_enable_start / Stopwatch.Frequency;
            //long data1 = (long)data;
            //writeableBytes = BitConverter.GetBytes(data1);
            //var route = Syncronized.nameDMutex.GetSafeWaitHandle();

            //if (!Syncronized.isMutexOwned) Syncronized.nameDMutex.WaitOne();
            Syncronized.handle.WaitOne();
            writeCount = writeCount == FileSize ? 0 : writeCount;
            shMemoryAccessor.WriteArray<byte>(writeCount, writeableBytes, 0, writeableBytes.Length);
            //shMemoryAccessor.Write<long>(writeCount, ref data1);
            writeCount = writeCount + pageCount;
            //Syncronized.handle.Set();
            //Syncronized.handle.Reset();
            //Syncronized.nameDMutex.ReleaseMutex();
            //Console.WriteLine("From " + data1);
            //stopwatch.Stop(); Console.WriteLine("mUTEX OWNED :" + stopwatch.Elapsed + " "+ data1 );
            Syncronized.isMutexOwned = Syncronized.isMutexOwned == true ? false : true;

            //  Syncronized.nameDMutex.WaitOne();
            Syncronized.isMutexOwned = true;
            Thread.Sleep(1000);
            //Syncronized.isSemaphoreAvail = false;
            //Syncronized._semaPhoreSlim.Wait();

            //Syncronized.localEventAvail = false;
            //Syncronized.ManualEvent.WaitOne();
            /////// Syncronized.afterReleaseMutex = Stopwatch.GetTimestamp();

            //Console.WriteLine("Release " + (1000000000.0 * (double)(Syncronized.afterReleaseMutex - Syncronized.releaseLocalMutex) / Stopwatch.Frequency) * 0.001);



        }




    }
}
