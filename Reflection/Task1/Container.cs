using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Task1.DoNotChange;

namespace Task1
{
    public class Container
    {
        private Dictionary<Type, Type> _dictionary;
        private Assembly _assembly;
        private Type[] _assemblyTypes;
        private bool withAttributes;

        public Container()
        {
            _dictionary = new Dictionary<Type, Type>();
            withAttributes = false;
        }
        public Assembly AddAssembly(Assembly assembly)
        {
            if (_assembly != null)
            {
                throw new Exception($"Assembly already added to this container");
            }
            _assembly = assembly;
            _assemblyTypes = _assembly.GetExportedTypes();
            CreateInstanceByAttributes();
            return _assembly;
        }

        private void LoadAssembly()
        {
            _assembly ??= AddAssembly(Assembly.GetExecutingAssembly());
            _assemblyTypes ??= _assembly.GetExportedTypes();
        }

        public void AddType(Type type)
        {
            if (CheckIfTypesExists(type))
            {
                throw new Exception($"Dependency of {type.Name} already exists");
            }
            _dictionary.Add(type, type);
        }

        public void AddType(Type type, Type baseType) //baseType is a key, type is value
        {
            if (CheckIfTypesExists(baseType))
            {
                throw new Exception($"Dependency of {baseType.Name} already exists");
            }
            _dictionary.Add(baseType,type);
            AddType(type);
        }
        private bool CheckIfTypesExists(Type type)
        {
            bool itemExists = false;
            foreach (var item in _dictionary)
            {
                itemExists = item.Key == type;
                if (itemExists)
                {
                    return itemExists;
                }
            }
            return itemExists;
        }

        private object CreateInstance(Type type)
        {
            if (type.IsInterface)
            {
                String ErrMes = $"Dependency of {type.Name} is not provided";
                var typeOfInterface = AddInterface(type);

                if (!CheckIfTypesExists(_dictionary[typeOfInterface]))
                    throw new Exception(ErrMes);
                type = typeOfInterface;

                if (type.IsInterface && withAttributes)
                    throw new Exception(ErrMes);
            }
            if (!type.IsInterface)
            {
                object _object;
                object[] args = GetConstructorParameters(type);
                _object = args == null ? Activator.CreateInstance(type) : Activator.CreateInstance(type, args);
                InitializeProperties(type);
                return _object;
            }
            else return null;
        }

        private void CreateInstanceByAttributes()
        {
            LoadAssembly();
            withAttributes = true;
            foreach (var type in _assemblyTypes)
            {
                var constructorImportAttribute = type.GetCustomAttribute<ImportConstructorAttribute>();
                var importPropertiesAttributes = type.GetProperties().Where(p => p.GetCustomAttribute<ImportAttribute>() != null).Any();
                var exportAttributes = type.GetCustomAttribute<ExportAttribute>();
                if (constructorImportAttribute != null || importPropertiesAttributes || exportAttributes != null)
                {
                    CreateInstance(type);
                }
            }
        }

        private Type LoadTypeForInterface(Type type)
        {
            Type returnType;
            returnType = _dictionary[type];
            if (returnType.IsInterface)
            {
                foreach (var tempType in _assemblyTypes)
                {
                    if (type.IsAssignableFrom(tempType) && tempType.IsPublic && !tempType.IsInterface)
                    {
                        if (!CheckIfTypesExists(type))
                        {
                            AddType(tempType, returnType);
                        }
                        returnType = tempType;
                        break;
                    }
                }
            }
            return returnType;
        }
        private Type AddInterface(Type type)
        {
            var typeOfInterface = type;
            LoadAssembly();
            if (type.IsInterface)
            {
                typeOfInterface = LoadTypeForInterface(typeOfInterface);
            }
            if (!CheckIfTypesExists(type))
            {
                AddType(typeOfInterface, type);
            }
            if (!CheckIfTypesExists(typeOfInterface))
            {
                AddType(typeOfInterface);
            }
            return typeOfInterface;
        }

        private object[] GetConstructorParameters(Type type)
        {
            var constructorInfo = type.GetConstructors().Where(p => p.GetParameters().Length > 0).FirstOrDefault();
            if (constructorInfo != null)
            {
                ParameterInfo[] parameters = constructorInfo.GetParameters();
                object[] args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    var _paramType = parameters[i].ParameterType;

                    //_paramType = parameters[i].ParameterType.IsInterface ? CheckandAddType(parameters[i].ParameterType) : parameters[i].ParameterType;
                    if (!CheckIfTypesExists(_paramType))
                        AddType(_paramType);

                    var instance = CreateInstance(_paramType);  
                    args[i] = instance;
                }
                return args.Length > 0 ? args : null;
            }
            return null;
        }
        private void InitializeProperties(Type type)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var _paramType = propertyInfos[i].PropertyType;
                //_paramType = propertyInfos[i].PropertyType.IsInterface ? CheckandAddType(propertyInfos[i].PropertyType) : propertyInfos[i].PropertyType;
                if (!CheckIfTypesExists(_paramType))
                    AddType(_paramType);
                CreateInstance(_paramType);
            }
        }

        public T Get<T>()
        {
            if (_assembly == null && _dictionary.Count == 0)
                throw new Exception("Nothing is loaded");

            var type = typeof(T);
            var instance = CreateInstance(type);

            if (typeof(T) != instance.GetType() && !type.IsInterface && !withAttributes)
                throw new Exception("Wrong instance");
            return (T)instance;
        }

    }
}