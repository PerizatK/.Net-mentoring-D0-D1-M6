using MyIoC.Attributes;

namespace MyIoC.Test.InjectTestEntities
{
    public interface ICustomerDAL
    {
    }

    [Export(typeof(ICustomerDAL))]
    public class CustomerDAL : ICustomerDAL
    {
        public CustomerDAL() { }
    }
}