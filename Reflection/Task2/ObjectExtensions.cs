using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Task2
{
    struct ParamStruct
    {
        public int typeNum;
        public Type type;
        public string name;
        public object newValue;
    }

    public static class ObjectExtensions
    {
        private static ParamStruct paramStruct;
        private static Assembly assembly;

        private static void Init(int typeNum, Type type, string name, object newValue)
        {
            paramStruct.typeNum = typeNum;
            paramStruct.type = type;
            paramStruct.name = name;
            paramStruct.newValue = newValue;

            assembly = type.Assembly;
        }

        public static void SetReadOnlyProperty(this object obj, string propertyName, object newValue)
        {
            var type = obj.GetType();
            Init(1, type, propertyName, newValue);
            CreateInstances(type);
        }

        public static void SetReadOnlyField(this object obj, string filedName, object newValue)
        {
            var type = obj.GetType();
            Init(0, type, filedName, newValue);
            CreateInstances(type);
        }

        private static void SetFieldOrProperty(object instance)
        {
            if (paramStruct.typeNum == 0)
            {
                var field = paramStruct.type
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(f => f.Name == paramStruct.name);
                if (field != null)
                {
                    field.SetValueDirect(__makeref(instance), paramStruct.newValue);
                    field.SetValue(instance, paramStruct.newValue);
                }
            }
            else if (paramStruct.typeNum == 1)
            {
                var property = paramStruct.type
                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(f => f.Name == paramStruct.name);
                if (property != null)
                {
                    property.SetValue(instance, paramStruct.newValue);
                }
            }
        }

        private static void CreateInstances(Type type)
        {
            foreach (var tempType in assembly.GetTypes())
            {
              if (tempType.IsAssignableFrom(type) || type.IsAssignableFrom(tempType) || type == tempType)
                {
                    var instance = Activator.CreateInstance(type);
                    SetFieldOrProperty(instance);
                }
            }
        }

    }
}


//namespace Task2.Tests.Entities
//{
//    internal class Child : Parent
//    {
//        public int ChildProperty { get; } = 3;

//        public readonly string ChildField = "321";
//    }
//}

//namespace Task2.Tests.Entities
//{
//    internal class Parent
//    {
//        public int Property { get; } = 1;

//        public readonly string Filed = "123";
//    }
//}