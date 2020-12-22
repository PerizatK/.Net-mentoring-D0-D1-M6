using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyIoC.Test.InjectTestEntities;

namespace MyIoC.Test
{
    [TestClass]
    public class ContainerTests
    {
        private Container _container;

        [TestInitialize]
        public void Init()
        {
            _container = new Container(new SimpleActivator());
        }

        [TestMethod]
        public void GenericCreateInstance_AssemblyAttributes_ConstructorInjectionTest()
        {
            _container.AddAssembly(Assembly.GetExecutingAssembly());

            var customerBll = _container.CreateInstance<CustomerBLL>();

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL));
        }

        [TestMethod]
        public void Not_GenericCreateInstance_AssemblyAttributes_ConstructorInjectionTest()
        {
            _container.AddAssembly(Assembly.GetExecutingAssembly());

            var customerBll = _container.CreateInstance<CustomerBLL>();

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL));
        }

        [TestMethod]
        public void Not_GenericCreateInstance_ExplicitSet_ConstructorInjectionTest()
        {
            _container.AddType(typeof(CustomerBLL));
            _container.AddType(typeof(Logger));
            _container.AddType(typeof(CustomerDAL), typeof(ICustomerDAL));

            var customerBll = (CustomerBLL)_container.CreateInstance(typeof(CustomerBLL));

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL));
        }

        [TestMethod]
        public void GenericCreateInstance_ExplicitSet_ConstructorInjectionTest()
        {
            _container.AddType(typeof(CustomerBLL));
            _container.AddType(typeof(Logger));
            _container.AddType(typeof(CustomerDAL), typeof(ICustomerDAL));

            var customerBll = _container.CreateInstance<CustomerBLL>();

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL));
        }

        [TestMethod]
        public void GenericCreateInstance_AssemblyAttributes_PropertiesInjectionTest()
        {
            _container.AddAssembly(Assembly.GetExecutingAssembly());

            var customerBll = _container.CreateInstance<CustomerBLL2>();

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL2));
            Assert.IsNotNull(customerBll.CustomerDAL);
            Assert.IsNotNull(customerBll.CustomerDAL.GetType() == typeof(CustomerDAL));
            Assert.IsNotNull(customerBll.Logger);
            Assert.IsNotNull(customerBll.Logger.GetType() == typeof(Logger));
        }

        [TestMethod]
        public void Not_GenericCreateInstance_AssemblyAttributes_PropertiesInjectionTest()
        {
            _container.AddAssembly(Assembly.GetExecutingAssembly());

            var customerBll = _container.CreateInstance<CustomerBLL2>();

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL2));
            Assert.IsNotNull(customerBll.CustomerDAL);
            Assert.IsNotNull(customerBll.CustomerDAL.GetType() == typeof(CustomerDAL));
            Assert.IsNotNull(customerBll.Logger);
            Assert.IsNotNull(customerBll.Logger.GetType() == typeof(Logger));
        }

        [TestMethod]
        public void Not_GenericCreateInstance_ExplicitSet_PropertiesInjectionTest()
        {
            _container.AddType(typeof(CustomerBLL2));
            _container.AddType(typeof(Logger));
            _container.AddType(typeof(CustomerDAL), typeof(ICustomerDAL));

            var customerBll = (CustomerBLL2)_container.CreateInstance(typeof(CustomerBLL2));

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL2));
            Assert.IsNotNull(customerBll.CustomerDAL);
            Assert.IsNotNull(customerBll.CustomerDAL.GetType() == typeof(CustomerDAL));
            Assert.IsNotNull(customerBll.Logger);
            Assert.IsNotNull(customerBll.Logger.GetType() == typeof(Logger));
        }

        [TestMethod]
        public void GenericCreateInstance_ExplicitSet_PropertiesInjectionTest()
        {
            _container.AddType(typeof(CustomerBLL2));
            _container.AddType(typeof(Logger));
            _container.AddType(typeof(CustomerDAL), typeof(ICustomerDAL));

            var customerBll = _container.CreateInstance<CustomerBLL2>();

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL2));
            Assert.IsNotNull(customerBll.CustomerDAL);
            Assert.IsNotNull(customerBll.CustomerDAL.GetType() == typeof(CustomerDAL));
            Assert.IsNotNull(customerBll.Logger);
            Assert.IsNotNull(customerBll.Logger.GetType() == typeof(Logger));
        }

        [TestMethod]
        public void EmitActivator_ConstructorInjectionTest()
        {
            _container = new Container(new EmitActivator());
            _container.AddType(typeof(CustomerBLL));
            _container.AddType(typeof(Logger));
            _container.AddType(typeof(CustomerDAL), typeof(ICustomerDAL));

            var customerBll = _container.CreateInstance<CustomerBLL>();

            Assert.IsNotNull(customerBll);
            Assert.IsTrue(customerBll.GetType() == typeof(CustomerBLL));
        }
    }
}