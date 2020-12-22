using System;
using MyIoC.Interfaces;

namespace MyIoC
{
    public class SimpleActivator : IActivator
    {
        public object CreateInstance(Type type, params object[] parameters)
        {
            return Activator.CreateInstance(type, parameters);
        }
    }
}
