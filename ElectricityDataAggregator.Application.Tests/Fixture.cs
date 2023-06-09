using ElectricityDataAggregator.Common.Application.Tools.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

namespace EMS.Application.Tests
{
    public class Fixture
    {
        public IServiceCollection ServiceCollection = new ServiceCollection();
        public IServiceProvider ServiceProvider => ServiceCollection.BuildServiceProvider();
        public IMediator Mediator => ServiceProvider.GetService<IMediator>();

        public Fixture()
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            ServiceCollection.AddHttpContextAccessor();
            ServiceCollection.AddCommonApplication(Assembly.Load("ElectricityDataAggregator.Application"));
        }
    }
}
