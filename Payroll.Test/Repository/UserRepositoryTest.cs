using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using Payroll.Entities.Users;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;

namespace Payroll.Test.Repository
{
    [TestFixture]
    public class UserRepositoryTest
    {
        private readonly MoqMockingKernel _kernel;

        public UserRepositoryTest()
        {
            _kernel = new MoqMockingKernel();
            _kernel.Bind<IUserRepository>().To<UserRepository>();
        }


        [SetUp]
        public void SetUp()
        {
            _kernel.Reset();
        }

        [Test]
        public void GetUserById()
        {
            //Arrange
            var databaseFactory = new DatabaseFactory();
            var userRepository = new Repository<User>(databaseFactory);
            
            //Act
            var user = userRepository.GetById(1);

            //Asset
            Assert.NotNull(user);
        }
    }
}
