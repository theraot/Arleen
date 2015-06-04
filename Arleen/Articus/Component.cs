using System;

namespace Articus
{
    [Serializable]
    public sealed class Component
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