using Arleen;
using Arleen.Game;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

namespace Articus
{
    public class ModuleLoader : MarshalByRefObject
    {
        public const string STR_Module_Extension = "mod";
        public const string STR_Module_Folder = "Modules";

        private static readonly Type[] TargetTypes = {
            typeof(Realm)
        };

        private static Dictionary<Type, List<Component>> _components;
        private static ModuleLoader _instance;
        private static int _status;
        private readonly AppDomain _targetDomain;

        private ModuleLoader(AppDomain targetDomain)
        {
            _targetDomain = targetDomain;
        }

        public static ModuleLoader Instance
        {
            get
            {
                var found = _instance;
                if (found == null)
                {
                    throw new InvalidOperationException("ModuleLoader not initialized");
                }
                // if found an instance just return it
                return found;
            }
            set
            {
                Interlocked.CompareExchange(ref _instance, value, null);
            }
        }

        public static void Initialize(AppDomain targetDomain)
        {
            Interlocked.CompareExchange(ref _instance, new ModuleLoader(targetDomain), null);
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)] // OK
        public static void LoadModules()
        {
            Initialize();
            var status = Interlocked.CompareExchange(ref _status, 3, 2);
            if (status == 2)
            {
                foreach (var folder in Facade.Resources.GetFolders(new[] { STR_Module_Folder }))
                {
                    string[] files;
                    try
                    {
                        files = Directory.GetFiles(folder, "*.dll");
                        Facade.Logbook.Trace(TraceEventType.Information, "Trying to load modules from {0}", folder);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        Facade.Logbook.Trace(TraceEventType.Warning, " - Unable to access folder {0}", folder);
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

        public IEnumerable<Component> Discover(string assemblyFile)
        {
            var assembly = Assembly.LoadFile(assemblyFile);
            if (assembly != null)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var targetType in TargetTypes)
                    {
                        if (type.IsSubclassOf(targetType) && !type.IsAbstract)
                        {
                            Facade.Logbook.Trace(TraceEventType.Information, "Found component {0} of type {1} in module {2}", type, targetType, assembly);
                            yield return new Component(targetType, assemblyFile, type.FullName);
                        }
                    }
                }
            }
        }

        public IEnumerable<Component> GetComponents(Type targetType)
        {
            List<Component> components;
            // Do not compact if to return (type conversion problem)
            if (_components.TryGetValue(targetType, out components))
            {
                return components;
            }
            return new Component[] { };
        }

        public object Load(Component component)
        {
            Initialize();
            var result = _targetDomain.CreateInstanceFromAndUnwrap(component.AssamblyFile, component.TypeName);
            Facade.Logbook.Trace(TraceEventType.Information, "Created Instance of {0}", component.TypeName);
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

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)] // OK
        private static void LoadComponentsFromModule(string assemblyFile)
        {
            Facade.Logbook.Trace(TraceEventType.Information, "Trying to load module {0}", assemblyFile);
            var module = assemblyFile.Substring(0, assemblyFile.Length - 3) + STR_Module_Extension;
            if (!File.Exists(module))
            {
                Facade.Logbook.Trace(TraceEventType.Information, "Discovering types for module {0}", assemblyFile);
                var process = new Process {
                    StartInfo = {
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
                Facade.Logbook.Trace(TraceEventType.Warning, "Discovering failed for module {0}", assemblyFile);
            }
        }
    }
}