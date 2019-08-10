using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace TtcMonitor
{
    public class Schedule
    {
        private static Random rnd = new Random();
        private static readonly object syncRoot = new object();

        public int Iteration { get; private set; }
        public ScheduleSetup Configuration { get; private set; }
        public int ItemId { get { return Configuration.ItemId; } }
        public int NumberOfPages { get { return Configuration.NumberOfPages; } }
        public int AverageInterval { get { return Configuration.Interval; } }

        public int CurrentInterval { get; private set; }
        public DateTime LastEvent { get; private set; }
        public DateTime NextEvent { get { return LastEvent.AddSeconds(CurrentInterval); } }
        public List<TradeItem> Items { get; set; }

        public Schedule(ScheduleSetup configuraton)
        {
            Configuration = configuraton;
            Iteration = 0;
            Items = new List<TradeItem>();
        }

        public async Task Tick()
        {
            var now = DateTime.Now;
            if ((now - LastEvent).TotalSeconds >= CurrentInterval)
            {
                LastEvent = now;
                RandomizeInterval();
                Iteration++;

                await LoadData();
            }
        }

        public async Task LoadData()
        {
            var tasks = new Task[NumberOfPages];
            for (int i = 0; i < tasks.Length; i++)
            {
                object x = i + 1;
                tasks[i] = Task.Run(() => LoadPage(x));
            }
            await Task.WhenAll(tasks);
        }

        private async Task LoadPage(object pageNumber)
        {
            int n = (int)pageNumber;
            Debug.WriteLine("started " + DateTime.Now + " " + Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(500 + rnd.Next(5000));            
            var list = await PageDownloader.GetDataForItem(ItemId, n);            
            lock (syncRoot)
            {
                foreach (var newItem in list)
                {
                    if (!Items.Any(s => s.Id == newItem.Id))
                    {
                        Items.Add(newItem);
                    }
                }
            }
            Debug.WriteLine("ended " + DateTime.Now + " " + Thread.CurrentThread.ManagedThreadId);
        }

        private void RandomizeInterval()
        {
            var perc8 = AverageInterval * 0.08;
            int lb = (int)Math.Round(AverageInterval - perc8);
            int ub = (int)Math.Round(AverageInterval + perc8);
            CurrentInterval = lb + rnd.Next(ub - lb);

        }


    }
}
