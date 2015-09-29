using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace towerDefense
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new FilteredCamelCasePropertyNamesContractResolver
                {
                    TypesToInclude =
                {
                    typeof(Hubs.GameBroadcaster),
                    typeof(Hubs.GameHub),
                }
                }
            };
            var jsonNetSerializer = new JsonNetSerializer(serializerSettings);
            GlobalHost.DependencyResolver.Register(typeof(IJsonSerializer), () => jsonNetSerializer);
        }
    }

    public class FilteredCamelCasePropertyNamesContractResolver : DefaultContractResolver
    {
        public HashSet<Assembly> AssembliesToInclude { get; set; }
        public HashSet<Type> TypesToInclude { get; set; }

        public FilteredCamelCasePropertyNamesContractResolver()
        {
            AssembliesToInclude = new HashSet<Assembly>();
            TypesToInclude = new HashSet<Type>();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);

            Type declaringType = member.DeclaringType;
            if (TypesToInclude.Contains(declaringType) || AssembliesToInclude.Contains(declaringType.Assembly))
                jsonProperty.PropertyName = ToCamelCase(jsonProperty.PropertyName);

            return jsonProperty;
        }

        string ToCamelCase(string value)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            var firstChar = value[0];
            if (char.IsLower(firstChar))
                return value;

            firstChar = char.ToLowerInvariant(firstChar);
            return firstChar + value.Substring(1);
        }
    }
}
