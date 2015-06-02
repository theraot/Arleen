using System;
using System.Reflection;

namespace Arleen
{
    [Serializable]
    public struct CrossCaller
    {
        private readonly string _path;
        private readonly string _type;
        private readonly string _method;
        private readonly object _that;
        private readonly object[] _param;

        public CrossCaller(string type, string method)
        {
            _path = null;
            _type = type;
            _method = method;
            _that = null;
            _param = null;
        }

        public CrossCaller(string type, string method, object[] param)
        {
            _path = null;
            _type = type;
            _method = method;
            _that = null;
            _param = param;
        }

        public CrossCaller(string type, string method, object that, object[] param)
        {
            _path = null;
            _type = type;
            _method = method;
            _that = that;
            _param = param;
        }

        public CrossCaller(string type, string method, object that)
        {
            _path = null;
            _type = type;
            _method = method;
            _that = that;
            _param = null;
        }

        public CrossCaller(string path, string type, string method)
        {
            _path = path;
            _type = type;
            _method = method;
            _that = null;
            _param = null;
        }

        public CrossCaller(string path, string type, string method, object[] param)
        {
            _path = path;
            _type = type;
            _method = method;
            _that = null;
            _param = param;
        }

        public CrossCaller(string path, string type, string method, object that, object[] param)
        {
            _path = path;
            _type = type;
            _method = method;
            _that = that;
            _param = param;
        }

        public CrossCaller(string path, string type, string method, object that)
        {
            _path = path;
            _type = type;
            _method = method;
            _that = that;
            _param = null;
        }

        public void LoadAndCall()
        {
            // Let this method throw if unable to load the assembly
            var assembly = Assembly.LoadFrom(_path);
            // Let this method throw if the type is null
            var methodInfo = assembly.GetType (_type).GetMethod (_method);
            methodInfo.Invoke
            (
                _that,
                _param
            );
        }

        public void Call()
        {
            // Let this method throw if the type is null
            var methodInfo = Type.GetType (_type).GetMethod(_method);
            methodInfo.Invoke
            (
                _that,
                _param
            );
        }
    }
}