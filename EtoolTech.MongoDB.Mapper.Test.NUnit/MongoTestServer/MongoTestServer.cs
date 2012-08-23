namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public class MongoTestServer : IDisposable
    {
        #region Constants and Fields

        internal static Dictionary<int, MongoTestInstance> InstancesByPort = new Dictionary<int, MongoTestInstance>();

        private readonly int _port;

        private static string _mongodPath;

        #endregion

        #region Constructors and Destructors

        private MongoTestServer(int port)
        {
            this._port = port;
            List<KeyValuePair<int, MongoTestInstance>> instance =
                (from i in InstancesByPort where i.Key == port select i).ToList();
            if (instance.Any())
            {
                instance.First().Value.Clients++;
                return;
            }

            Directory.CreateDirectory(_mongodPath + port);

            var startInfo = new ProcessStartInfo(string.Format("{0}mongod.exe", _mongodPath))
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    Arguments = String.Format("--port {0} --dbpath {1}", port, _mongodPath + port)
                };

            Process process = Process.Start(startInfo);            
            var newInstance = new MongoTestInstance { Process = process, Clients = 1 };
            InstancesByPort.Add(port, newInstance);
            Thread.Sleep(10000);
        }

        #endregion

        #region Public Properties

        public string ConnectionString
        {
            get
            {
                return string.Format("mongodb://127.0.0.1:{0}", this._port);
            }
        }

        #endregion

        #region Public Methods

        public static void SetMongodPtah(string path)
        {
            _mongodPath = path;
        }

        public static MongoTestServer Start(int port)
        {
            return new MongoTestServer(port);
        }

        public void Dispose()
        {
            Thread.Sleep(3000);
            foreach (var instance in InstancesByPort)
            {
                if (instance.Key == this._port)
                {
                    instance.Value.Clients--;
                    if (instance.Value.Clients < 1)
                    {
                        if (!instance.Value.Process.HasExited)
                        {
                            instance.Value.Process.Kill();
                        }
                        //Damos tiempo a que se cierre el mongo
                        Thread.Sleep(5000);
                        Directory.Delete(_mongodPath + this._port, true);
                        InstancesByPort.Remove(this._port);
                    }
                    break;
                }
            }
        }

        #endregion
    }
}