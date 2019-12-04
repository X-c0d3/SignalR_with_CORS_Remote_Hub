using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Ninject;

namespace SignalR_webapi.Hubs
{
    public class NinjectSignalRDependencyResolver : DefaultDependencyResolver
    {
        private readonly IKernel _kernel;
        public NinjectSignalRDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType) ?? base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType).Concat(base.GetServices(serviceType));
        }
    }

    public class NinjectDependencyResolver : Microsoft.AspNet.SignalR.DefaultDependencyResolver, System.Web.Http.Dependencies.IDependencyResolver
    {
        public readonly IKernel Kernel;

        public NinjectDependencyResolver(string moduleFilePattern)
            : base()
        {
            Kernel = new StandardKernel();
            Kernel.Load(moduleFilePattern);

        }
        public override object GetService(Type serviceType)
        {
            var service = Kernel.TryGet(serviceType) ?? base.GetService(serviceType);
            return service;
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            IEnumerable<object> services = Kernel.GetAll(serviceType).ToList();
            return base.GetServices(serviceType) ?? services;
        }

        public System.Web.Http.Dependencies.IDependencyScope BeginScope()
        {
            return this;
        }

        public new void Dispose() { }
    }
}