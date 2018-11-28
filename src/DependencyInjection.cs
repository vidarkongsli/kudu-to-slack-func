using System;
using Microsoft.Extensions.DependencyInjection;

namespace v2
{
    public interface IModule
    {
        void Load(IServiceCollection services);
    }

    public class Module : IModule
    {
        public virtual void Load(IServiceCollection services)
        {
            return;
        }
    }

    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IServiceCollection _services;

        public ContainerBuilder()
        {
            this._services = new ServiceCollection();
        }

        public IContainerBuilder RegisterModule(IModule module = null)
        {
            if (module == null)
            {
                module = new Module();
            }
            module.Load(_services);
            return this;
        }

        public IContainerBuilder Register(Action<IServiceCollection> action)
        {
            action(_services);
            return this;
        }

        public IServiceProvider Build()
        {
            var provider = this._services.BuildServiceProvider();

            return provider;
        }
    }

    public interface IContainerBuilder
    {
        IContainerBuilder RegisterModule(IModule module = null);

        IServiceProvider Build();
    }
}