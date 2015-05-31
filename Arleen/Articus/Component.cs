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
		private readonly Type _targetType;
		private readonly string _assamblyFile;
		private readonly string _typeName;

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
	
}
