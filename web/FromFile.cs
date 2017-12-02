using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

namespace Delver.FromFile
{
    /// <summary>
    /// An object that can be persisted to a file
    /// </summary>
    class FromFile<T> where T : class, ISerializable, new() 
    {
        /// <summary>
        /// In-memory version of the object. Might differ from value on-file. 
        /// Changes in a using(obj.Lock())-statement are automatically saved
        /// </summary>
        public T Value { get; set; }

        string _filename { get; set; }        
        string _serialized { get; set; }
        TimeSpan _timeout { get; set; }
        MyFileStream stream { get; set; }

        private T _default { get; set; }
        private bool _throwOnDeserializeException { get; set; }

        /// <summary>
        /// Create an object that can be persisted to a file
        /// </summary>
        /// <param name="filename">The file to read/write to</param>
        /// <param name="expire">Expiration time for old persisted files</param>
        /// <param name="newObject">Default value for an empty object</param>
        /// <param name="timeout">Timespan to wait for file to become availible</param>
        public FromFile(string filename, TimeSpan? expire = null, T newObject = null, TimeSpan? timeout = null, bool throwOnDeserializeException = true)
        {
            _default = newObject ?? new T();
            _filename = filename;
            _timeout = timeout ?? TimeSpan.FromSeconds(10);
            _throwOnDeserializeException = throwOnDeserializeException;
            if (File.Exists(_filename))
            {
                if (expire.HasValue && File.GetLastWriteTime(_filename).Add(expire.Value) < DateTime.Now)
                {
                    Value = _default;
                    Save();
                }
                else
                {
                    Read();
                }
            }
            else
            {
                Value = _default;
                Save();
            }
        }


        /// <summary>
        /// Lock the file containing the object from being access by others
        /// </summary>
        /// <returns></returns>
        public FileStream Lock(out T obj)
        {
            var stream = Lock();
            obj = Value;
            return stream;
        }

        /// <summary>
        /// Lock the file containing the object from being access by others
        /// </summary>
        /// <returns></returns>
        public FileStream Lock()
        {
            Retry<IOException>(() =>
            {
                stream = new MyFileStream(_filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None, this);
            });

            if (IsChanged())
                Save();
            else
                Read();
            return stream;
        }
        
        /// <summary>
        /// Read the object from file
        /// </summary>
        /// <returns></returns>
        public T Read()
        {
            Value = TryRead();
            return Value;
        }

        /// <summary>
        /// Save the object to file
        /// </summary>
        public void Save()
        {
            TryWrite();
        }
        
        public static implicit operator T(FromFile<T> obj)
        {
            return obj.Value;
        }

        private class MyFileStream : FileStream
        {
            FromFile<T> _parent;
            public MyFileStream(string path, FileMode mode, FileAccess access, FileShare share, FromFile<T> parent)
                : base(path,mode,access,share)
            {
                _parent = parent;
            }

            public override void Close()
            {
                _parent.Save();
                _parent.stream = null;
                Dispose(true);
            }
        }

        private bool IsChanged()
        {
            var serialized = JsonConvert.SerializeObject(Value);
            return serialized != _serialized;
        }

        private void TryWrite()
        {
            Retry<IOException>(() =>
            {
                var serialized = JsonConvert.SerializeObject(Value);
                if (stream != null && stream.CanWrite)
                {
                    if (serialized != _serialized)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        var sw = new StreamWriter(stream);
                        sw.Write(serialized);
                        sw.Flush();
                        stream.SetLength(stream.Position);
                    }
                }
                else
                {
                    File.WriteAllText(_filename, serialized);
                }
                _serialized = serialized;
            });
        }

        private T TryRead()
        {
            T obj = null;
            Retry<IOException>(() => {
                if (stream != null && stream.CanRead)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var sw = new StreamReader(stream);
                    _serialized = sw.ReadToEnd();
                }
                else
                {
                    _serialized = File.ReadAllText(_filename);
                }
                try
                {
                    obj = JsonConvert.DeserializeObject<T>(_serialized);
                }
                catch
                {
                    if (_throwOnDeserializeException)
                        throw;
                    obj = _default;
                }
            });
            return obj;
        }

        private void Retry<E>(Action action, TimeSpan? maxRunTime = null, TimeSpan? sleep = null) where E : Exception
        {
            var sleepDuration = (int)(sleep ?? TimeSpan.FromMilliseconds(50)).TotalMilliseconds;
            var endTime = DateTime.UtcNow + (maxRunTime ?? TimeSpan.FromSeconds(10));
            while (endTime > DateTime.UtcNow)
            {
                try
                {
                    action.Invoke();
                    return;
                }
                catch (E) { }
                Thread.Sleep(sleepDuration);
            }
            throw new TimeoutException();
        }
    }
}
