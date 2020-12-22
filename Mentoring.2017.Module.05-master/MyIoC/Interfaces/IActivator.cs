using System;

namespace MyIoC.Interfaces
{
    public interface IActivator
    {
        object CreateInstance(Type type, params object[] parameters);
    }
}
