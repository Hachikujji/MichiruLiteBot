using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichiruLite.Modules.Audio
{
    public class QueueService
    {
        public int Count => _queue.Count;
        public bool isEmpty => _queue.Count == 0;

        private List<QueueItem> _queue = new List<QueueItem>();

        public bool IsQueueLoop = false;

        public void Add(QueueItem queueItem)
        {
            _queue.Add(queueItem);
        }

        public void Add(string title, string url, float pitch, int offset)
        {
            var queueItem = new QueueItem()
            {
                Name = title,
                Offset = offset,
                PitchMul = pitch,
                Rate = 48000,
                Url = url
            };
            _queue.Add(queueItem);
        }

        public void RemoveAt(int index)
        {
            try
            {
                _queue.RemoveAt(index - 1);
            }
            catch (Exception e)
            {
            }
        }

        public void RemoveFirst()
        {
            var queueItem = _queue.FirstOrDefault();
            if (queueItem != null)
                _queue.Remove(queueItem);
        }

        public string Print()
        {
            var i = 1;
            var sb = new StringBuilder();
            sb.Append("__");
            foreach (var item in _queue)
            {
                sb.Append($"[{i++}] {item.Name}\n");
            }
            sb.Append("__");
            return sb.ToString();
        }

        public QueueItem GetFirstOrDefault()
        {
            return _queue.FirstOrDefault();
        }

        public void Put(int index, QueueItem queueItem)
        {
            _queue.Insert(index, queueItem);
        }
    }
}