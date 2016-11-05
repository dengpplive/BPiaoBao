/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.Common
* 文 件 名：QueueExt.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/12 13:32:44       
* 备注描述：           
************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace BPiaoBao.Common
{
    /// <summary>
    /// 队列扩展
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueExt<T>:IDisposable
    {
        /// <summary>
        /// 信号开关
        /// </summary>
        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        /// <summary>
        /// 线程安全队列对象
        /// </summary>
        private ConcurrentQueue<T> _queue;
        /// <summary>
        /// 对象锁
        /// </summary>
        private Object _obj = new object();
        /// <summary>
        /// 默认存入最大条数
        /// </summary>
        private int DefaultMaxCount = 5;
        /// <summary>
        /// 触发事件返回可操作的日志列表
        /// </summary>
        public QueueEventHandler<List<T>> OnQueueEventHandler;
        /// <summary>
        /// 以时间为基准检查队列的线程
        /// </summary>
        private Thread _timeThread;
        /// <summary>
        /// 以数量为基准检查队列的线程
        /// </summary>
        private Thread _maxThread;
        /// <summary>
        /// 是否开启线程监视队列变化
        /// </summary>
        private bool _isStartThreadListener;
        /// <summary>
        /// 数量监视队列线程是否在等待
        /// </summary>
        private bool _isMaxSleep = true; 
        /// <summary>
        /// 时间监视队列线程是否在等待
        /// </summary>
        private bool _isTimeSleep = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxCount">最大存入条数（达到最大条数时则触发事件回调输出队列中储存的对象列表）</param>
        /// <param name="isStartThreadListener">是否利用线程监控（如果此参数为空则最大数无用）</param>
        public QueueExt(int maxCount = 5, bool isStartThreadListener = true)
        {
            this.DefaultMaxCount = maxCount;
            this._queue = new ConcurrentQueue<T>();
            this._isStartThreadListener = isStartThreadListener;
            this.Start();

        }
        /// <summary>
        /// 以间隔时间为基准检查队列情况并全部返回队列数据
        /// </summary>
        private void CheckTimeDequeue()
        {
            while (true)
            {


                Thread.Sleep(2000 * 60); //2分钟循环一次 
                if (!_isMaxSleep) return;//如果数量监视队列线程是唤醒状态则返回
                _isTimeSleep = false;//唤醒
                var list = new List<T>();
                var max = _queue.Count;
                for (var i = 0; i < max; i++)
                {
                    T t;
                    _queue.TryDequeue(out t);
                    list.Add(t);
                }
                if (OnQueueEventHandler != null&&list.Count>0)
                {
                    OnQueueEventHandler(this, new QueueEventArgs<List<T>>() { Obj = list });
                }
                //count1 += list.Count;
                //Console.WriteLine("出队列--->时间---------------------------------->count1:" + count1);
                _isTimeSleep = true;//休眠
            }
        }

        //private int count = 0;
        //private int count1 = 0;
        /// <summary>
        ///  以数量为基准检查队列是否满足条件
        /// </summary>
        private void CheckMaxCountDequeue()
        {

            while (true)
            {

               
                _autoResetEvent.WaitOne();
                if (!_isTimeSleep) return;//如果时间监视队列线程是唤醒状态则返回
                _isMaxSleep = false;//唤醒
                var list = new List<T>();
                for (var i = 0; i < DefaultMaxCount; i++)
                {
                    T t;
                    _queue.TryDequeue(out t);
                    list.Add(t);
                }

                if (OnQueueEventHandler != null && list.Count > 0)
                {
                    OnQueueEventHandler(this, new QueueEventArgs<List<T>>() { Obj = list });
                }
                //count += list.Count;
                //Console.WriteLine("出队列--->数量---------------------------------->count:" + count);
                _isMaxSleep = true;//休眠
            }
        }

        /// <summary>
        /// 入队列
        /// </summary>
        /// <param name="t"></param>
        public void Enqueue(T t)
        {

            _queue.Enqueue(t);
           // Console.WriteLine("入队列");
            if (_queue.Count >= DefaultMaxCount)
            {
                _autoResetEvent.Set();

            }
        }
        /// <summary>
        /// 开始监听
        /// </summary>
        public void Start()
        {
            if (_isStartThreadListener)
            {
                _timeThread = new Thread(new ThreadStart(this.CheckTimeDequeue));
                _maxThread = new Thread(new ThreadStart(this.CheckMaxCountDequeue));
                _timeThread.Start();
                _maxThread.Start();
            }
        }
        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop()
        {
            if (_isStartThreadListener)
            {
                if (_timeThread != null)
                {
                    _timeThread.Abort();
                    _timeThread = null;
                }
                if (_maxThread != null)
                {
                    _maxThread.Abort();
                    _maxThread = null;
                }

            }

        }


        public void Dispose()
        {
            Stop();
        }
    }

    public class QueueEventArgs<T> : EventArgs
    {
        public T Obj { get; set; }
    }

    public delegate void QueueEventHandler<T>(object sender, QueueEventArgs<T> e);
}
