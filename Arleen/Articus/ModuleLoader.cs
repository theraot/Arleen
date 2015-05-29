using Arleen.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;
using Newtonsoft.Json;
using Arleen;

namespace Articus
{
	public class Component
	{
		private Type _targetType;
		private string _assamblyFile;
		private string _typeName;

		public Component(Type targetType, string assamblyFile, string typeName)
		{
			_targetType = targetType;
			_assamblyFile = assamblyFile;
			_typeName = typeName;
		}

		public Type TargetType
		{
			get
			{
				return _targetType;
			}
		}

		public string AssamblyFile
		{
			get
			{
				return _assamblyFile;
			}
		}

		public string TypeName
		{
			get
			{
				return _typeName;
			}
		}
	}

	[Serializable]
	public class StaticCaller
	{
		private string _path;
		private string _type;
		private string _method;

		public StaticCaller(string path, string type, string method)
		{
			_path = path;
			_type = type;
			_method = method;
		}

		public void Call()
		{
			var assembly = Assembly.LoadFrom (_path);
			assembly.GetType (_type).InvokeMember (_method, BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
			Logbook.Instance.Trace (TraceEventType.Information, "Called {0}.{1}", _type, _method);
		}
	}

	public static class ModuleLoader
	{
		private static AppDomain _sandbox;
		private static Dictionary<Type, List<Component>> _components;

		private readonly static Type[] _targetTypes = {
			typeof(Realm)
		};

		private static int _status;

		static void Initialize ()
		{
			var status = Interlocked.CompareExchange (ref _status, 1, 0);
			if (status == 0)
			{
				PermissionSet permissionSet = new PermissionSet (PermissionState.Unrestricted);
				permissionSet.AddPermission (new SecurityPermission (SecurityPermissionFlag.Execution));
				permissionSet.AddPermission (new FileIOPermission (PermissionState.Unrestricted));
				permissionSet.AddPermission (new UIPermission (PermissionState.Unrestricted));
				permissionSet.AddPermission (new ReflectionPermission (PermissionState.Unrestricted));
				//Create a new sandboxed domain
				AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
				Evidence adevidence = AppDomain.CurrentDomain.Evidence;
				_sandbox = AppDomain.CreateDomain ("Sandbox Domain", adevidence, setup, permissionSet);
				var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
				var articus = new Uri (Assembly.GetExecutingAssembly().CodeBase).LocalPath;
				_sandbox.DoCallBack ((new StaticCaller (articus, "Articus.Program", "Initialize")).Call);
				_components = new Dictionary<Type, List<Component>> ();
				Thread.VolatileWrite (ref _status, 2);
			}
			else if (status < 2)
			{
				while (Thread.VolatileRead (ref _status) < 2)
				{
					Thread.Sleep (0);
				}
			}
		}

		public static object Load(Component component)
		{
			Initialize ();
			var result = _sandbox.CreateInstanceFromAndUnwrap (component.AssamblyFile, component.TypeName);
			Logbook.Instance.Trace (TraceEventType.Information, "Created sandboxed Instance of {0}", component.TypeName);
			return result;
		}

		public static IEnumerable<Component> GetComponents (Type targetType)
		{
			LoadModules ();
			if (_components.ContainsKey (targetType))
			{
				return _components [targetType];
			}
			return new Component[] { };
		}

		static void LoadComponentsFromModule (string assemblyFile)
		{
			Logbook.Instance.Trace (TraceEventType.Information, " - Trying to load module {0}", assemblyFile);
			var module = assemblyFile.Substring(0, assemblyFile.Length - 3) + "mod";
			if (!System.IO.File.Exists (module))
			{
				Logbook.Instance.Trace (TraceEventType.Information, "Discovering types for module {0}", assemblyFile);
				var process = new Process ();
				process.StartInfo.FileName = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
				process.StartInfo.Arguments = "discover \"" + assemblyFile  + "\"";
				process.Start();
				process.WaitForExit();
			}
			try
			{
				string data = File.ReadAllText(module);
				var found = JsonConvert.DeserializeObject<IEnumerable<Component>>(data);
				foreach (var component in found)
				{
					if (_components.ContainsKey (component.TargetType))
					{
						_components [component.TargetType].Add (component);
					}
					else
					{
						_components [component.TargetType] = new List<Component> { component };
					}
				}

			}
			catch (IOException)
			{
				Logbook.Instance.Trace (TraceEventType.Warning, "Discovering failed for module {0}", assemblyFile);
				return;
			}
		}

		public static IEnumerable<Component> Discover(string assemblyFile)
		{
			var assembly = Assembly.LoadFile (assemblyFile);
			if (assembly != null)
			{
				foreach (var type in assembly.GetTypes ())
				{
					foreach (var targetType in _targetTypes)
					{
						if (type.IsSubclassOf (targetType) && type.IsAbstract == false)
						{
							Logbook.Instance.Trace (TraceEventType.Information, "Found component {0} of type {1} in module {2}", type, targetType, assembly);
							yield return new Component (targetType, assemblyFile, type.FullName);
						}
					}
				}
			}
		}

		private static void LoadModules ()
		{
			Initialize ();
			var status = Interlocked.CompareExchange (ref _status, 3, 2);
			if (status == 2)
			{
				foreach (var folder in ResourceLoader.GetFolders(new[] {"Modules"}))
				{
					string[] files;
					try
					{
						files = Directory.GetFiles (folder, "*.dll");
						Logbook.Instance.Trace (TraceEventType.Information, " - Trying to load modules from {0}", folder);
					}
					catch (DirectoryNotFoundException)
					{
						Logbook.Instance.Trace (TraceEventType.Warning, " - Unable to access folder {0}", folder);
						continue;
					}
					foreach (var file in files)
					{
						LoadComponentsFromModule (file);
					}
				}
				Thread.VolatileWrite (ref _status, 4);
			}
			else if (status < 4)
			{
				while (Thread.VolatileRead (ref _status) < 4)
				{
					Thread.Sleep (0);
				}
			}
		}
	}
}
