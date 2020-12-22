using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Task1.DoNotChange;

namespace Task1
{
    public class Container
    {
        private Assembly _assembly;
        private IDictionary<Type, Type> _dictionary;

        public Container()
        {
            _dictionary = new Dictionary<Type, Type>();
        }

        public void AddAssembly(Assembly assembly)
        {
            if (_assembly != null)
            {
                throw new Exception($"Assembly already added to this container");
            }
            _assembly = assembly;
            AddAttributedTypes(_assembly);
        }

        private bool HasConstructorWithParameters(Type type)
        {
            var constructorInfo = type.GetConstructors().Where(p => p.GetParameters().Length > 0).FirstOrDefault();
            return constructorInfo != null;
        }

        private object[] GetConstructorParameters(Type type)
        {
            var constructorInfo = type.GetConstructors().Where(p => p.GetParameters().Length > 0).FirstOrDefault();
            ParameterInfo[] parameters = constructorInfo.GetParameters();
            object[] args = new object[parameters.Length];
            for (int i=0; i < parameters.Length; i++)
            {
                var _paramType = parameters[i].ParameterType;

                _paramType = parameters[i].ParameterType.IsInterface ? GetTypeForInterface(parameters[i].ParameterType) : parameters[i].ParameterType;
                if (!_dictionary.ContainsKey(_paramType))
                {
                    AddType(_paramType, type);
                }
                var instance = Activator.CreateInstance(_paramType);
                args[i] = instance;
            }
            return args;
        }

        public void AddType(Type type)
        {
            bool itemExists = false;
            foreach (var item in _dictionary)
            {
                itemExists = item.Key == type ? true : false;
                if (itemExists)
                {
                    throw new Exception($"Dependency of {type.Name} already exists");
                }
            }

            _dictionary.Add(type, type);
            CreateInstance(type);
        }

        public void AddType(Type type, Type baseType)
        {
            bool itemExists = false;
            foreach (var item in _dictionary)
            {
                itemExists = item.Key == type ? true : false;
                if (itemExists)
                {
                    throw new Exception($"Dependency of {type.Name} already exists");
                }
            }

            _dictionary.Add(type, baseType);
            CreateInstance(type);
        }

        private object CreateInstance(Type type)
        {
            object _object;
            object[] args = HasConstructorWithParameters(type) ? GetConstructorParameters(type) : null;
            _object = Activator.CreateInstance(type, args);
            InitializeProperties(type);
            return _object;
        }

        private void AddAttributedTypes(Assembly assembly)
        {
            foreach (var type in _assembly.GetExportedTypes())
            {
                var constructorImportAttribute = type.GetCustomAttribute<ImportConstructorAttribute>();
                var importPropertiesAttributes = type.GetProperties().Where(p => p.GetCustomAttribute<ImportAttribute>() != null).Any();
                if (constructorImportAttribute != null || importPropertiesAttributes)
                {
                    GetTypeForInterface(type);
                }

                var exportAttributes = type.GetCustomAttributes<ExportAttribute>();
                foreach (var exportAttribute in exportAttributes)
                {
                    GetTypeForInterface(exportAttribute.Contract ?? type);
                }
            }
        }

        private Type GetTypeForInterface(Type type)
        {
            if (type.IsInterface)
            {
                if (!_dictionary.ContainsKey(type))
                {
                    _assembly = _assembly == null ? Assembly.GetExecutingAssembly() : _assembly;
                    foreach (var tempType in _assembly.GetTypes())
                    {
                        if (type.IsAssignableFrom(tempType) && tempType.IsPublic && !tempType.IsInterface)
                        {
                            if (!_dictionary.ContainsKey(tempType))
                            {
                                AddType(tempType, type);
                            }
                        }
                    }
                }
                type = _dictionary.Where(x => x.Value == type).FirstOrDefault().Key;
            }
            return type;
        }

        private void InitializeProperties(Type type)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var _paramType = propertyInfos[i].PropertyType;
                _paramType = propertyInfos[i].PropertyType.IsInterface ? GetTypeForInterface(propertyInfos[i].PropertyType) : propertyInfos[i].PropertyType;
                if (!_dictionary.ContainsKey(_paramType))
                {
                    AddType(_paramType, type);
                }
            }
        }

        public T Get<T>()
        {
            var _localType = typeof(T);
            if (!_dictionary.ContainsKey(_localType) && (!_localType.IsInterface) && (_assembly == null))
            {
                throw new Exception($"Dependency of {_localType.Name} is not provided");
            }

            _localType = GetTypeForInterface(_localType);
            return (T)CreateInstance(_localType);
        }
    }
}