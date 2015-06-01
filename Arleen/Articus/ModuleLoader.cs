using Arleen;
using Arleen.Game;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Articus
{
    public static class ModuleLoader
    {
        public const string STR_Module_Extension = "mod";

        private readonly static Type[] _targetTypes = {
            typeof(Realm)
        };

        private static Dictionary<Type, List<Component>> _components;
        private static int _status;

        public static IEnumerable<Component> Discover(string assemblyFile)
        {
            var assembly = Assembly.LoadFile(assemblyFile);
            if (assembly != null)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var targetType in _targetTypes)
                    {
                        if (type.IsSubclassOf(targetType) && !type.IsAbstract)
                        {
                            Logbook.Instance.Trace(TraceEventType.Information, "Found component {0} of type {1} in module {2}", type, targetType, assembly);
                            yield return new Component(targetType, assemblyFile, type.FullName);
                        }
                    }
                }
            }
        }

        public static IEnumerable<Component> GetComponents(Type targetType)
        {
            LoadModules();
            List<Component> components;
            if (_components.TryGetValue(targetType, out components))
            {
                return components;
            }
            return new Component[] { };
        }

        public static object Load(Component component)
        {
            Initialize();
            var result = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(component.AssamblyFile, component.TypeName);
            Logbook.Instance.Trace(TraceEventType.Information, "Created Instance of {0}", component.TypeName);
            return result;
        }

        private static void Initialize()
        {
            var status = Interlocked.CompareExchange(ref _status, 1, 0);
            if (status == 0)
            {
                _components = new Dictionary<Type, List<Component>>();
                Thread.VolatileWrite(ref _status, 2);
            }
            else if (status < 2)
            {
                while (Thread.VolatileRead(ref _status) < 2)
                {
                    Thread.Sleep(0);
                }
            }
        }

        private static void LoadComponentsFromModule(string assemblyFile)
        {
            Logbook.Instance.Trace(TraceEventType.Information, "Trying to load module {0}", assemblyFile);
            var module = assemblyFile.Substring(0, assemblyFile.Length - 3) + STR_Module_Extension;
            if (!File.Exists(module))
            {
                Logbook.Instance.Trace(TraceEventType.Information, "Discovering types for module {0}", assemblyFile);
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath,
                        Arguments = "discover \"" + assemblyFile + "\""
                    }
                };
                process.Start();
                process.WaitForExit();
            }
            try
            {
                string data = File.ReadAllText(module);
                var found = JsonConvert.DeserializeObject<IEnumerable<Component>>(data);
                foreach (var component in found)
                {
                    if (_components.ContainsKey(component.TargetType))
                    {
                        _components[component.TargetType].Add(component);
                    }
                    else
                    {
                        _components[component.TargetType] = new List<Component> { component };
                    }
                }
            }
            catch (IOException)
            {
                Logbook.Instance.Trace(TraceEventType.Warning, "Discovering failed for module {0}", assemblyFile);
            }
        }

        private static void LoadModules()
        {
            Initialize();
            var status = Interlocked.CompareExchange(ref _status, 3, 2);
            if (status == 2)
            {
                foreach (var folder in Resources.Instance.GetFolders(new[] { "Modules" }))
                {
                    string[] files;
                    try
                    {
                        files = Directory.GetFiles(folder, "*.dll");
                        Logbook.Instance.Trace(TraceEventType.Information, "Trying to load modules from {0}", folder);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        Logbook.Instance.Trace(TraceEventType.Warning, " - Unable to access folder {0}", folder);
                        continue;
                    }
                    foreach (var file in files)
                    {
                        LoadComponentsFromModule(file);
                    }
                }
                Thread.VolatileWrite(ref _status, 4);
            }
            else if (status < 4)
            {
                while (Thread.VolatileRead(ref _status) < 4)
                {
                    Thread.Sleep(0);
                }
            }
        }
    }
}