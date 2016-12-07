using System;
using System.Reflection;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Newtonsoft.Json.Serialization;

namespace Roulette.Hubs
{
    public class SignalRContractResolver : IContractResolver
    {
        private readonly Assembly _assembly;
        private readonly IContractResolver _camelCaseContractResolver;
        private readonly IContractResolver _defaultContractSerializer;

        public SignalRContractResolver()
        {
            _defaultContractSerializer = new DefaultContractResolver();
            _camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
            _assembly = typeof(Connection).GetTypeInfo().Assembly;
        }

        public JsonContract ResolveContract(Type type)
        {
            return type.GetTypeInfo().Assembly.Equals(_assembly)
                ? _defaultContractSerializer.ResolveContract(type)
                : _camelCaseContractResolver.ResolveContract(type);
        }
    }
}
