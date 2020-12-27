using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Channels;

namespace Septa.AspNetCore.SignalRTypes
{
    public class SignalrTypeSchemaResolver : JsonSchemaResolver
    {
        private readonly SignalrTypeDocument _document;
        private readonly JsonSchemaGeneratorSettings _settings;

        public SignalrTypeSchemaResolver(SignalrTypeDocument document, SignalrTypeGeneratorSettings settings)
            : base(document, settings)
        {
            _document = document;
            _settings = settings;
        }

        public override void AppendSchema(JsonSchema schema, string typeNameHint)
        {
            // TODO: JsonSchemaResolver should use new IDefinitionsObject interface and not JsonSchema


            if (schema == null)
                throw new ArgumentNullException(nameof(schema));
            if (schema == RootObject)
                throw new ArgumentException("The root schema cannot be appended.");

            if (string.IsNullOrWhiteSpace(schema.Description))
            {
                schema.Description = null;
            }

            if (!_document.Definitions.Values.Contains(schema))
            {
                var typeName = _settings.TypeNameGenerator.Generate(schema, typeNameHint, _document.Definitions.Keys);
                if (!string.IsNullOrEmpty(typeName) && !_document.Definitions.ContainsKey(typeName))
                    _document.Definitions[typeName] = schema;
                else
                    _document.Definitions["ref_" + Guid.NewGuid().ToString().Replace("-", "_")] = schema;
            }
        }
    }

    public static class SignalRTypesDependencyInjectionExtensions
    {
        public static void AddSignalRTypes<T>(this IServiceCollection services) where T : class, ISignalRTypesBuilder
        {
            services.AddScoped<ISignalRTypesBuilder, T>();
        }

        public static void AddSignalRTypes(this IServiceCollection services)
        {
           services.AddSignalRTypes<SignalRTypesBuilder>();
        }

    }

    public interface ISignalRTypesBuilder
    {
        List<SignalrTypesCallBack> GetCallBack();
    }

    public class SignalRTypesBuilder : ISignalRTypesBuilder
    {
        public List<SignalrTypesCallBack> GetCallBack()
        {
            var type = typeof(Hub);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract).ToList();

            var signalrTypesCallBackTypes = new List<SignalrTypesCallBack>();

            foreach (var item in types)
            {
                var baseTypeGenericArguments = item.BaseType.GetGenericArguments();

                if (baseTypeGenericArguments.Length == 1)
                {
                    var callbackType = baseTypeGenericArguments[0];

                    foreach (var callbackMethod in GetOperationMethods(callbackType))
                    {
                        var methodName = callbackMethod.Name;
                        var hubMethodName = callbackMethod.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(HubMethodNameAttribute));

                        if (hubMethodName != null && hubMethodName.ConstructorArguments.Count > 0)
                        {
                            var argName = hubMethodName.ConstructorArguments[0].Value.ToString();
                            if (!string.IsNullOrEmpty(argName))
                            {
                                methodName = argName;
                            }
                        }


                        var parametersType = new List<SignalrTypesCallBackParameter>();

                        foreach (var arg in callbackMethod.GetParameters())
                        {
                            parametersType.Add(new SignalrTypesCallBackParameter()
                            {
                                Description = arg.GetXmlDocs(),
                                Type = arg.ParameterType
                            });
                        }

                        signalrTypesCallBackTypes.Add(new SignalrTypesCallBack()
                        {
                            Description = callbackMethod.GetXmlDocsSummary(),
                            MethodName = methodName,
                            SignalrTypesCallBackParameters = parametersType
                        });

                    }

                }

            }

            return signalrTypesCallBackTypes;
        }

        private static IEnumerable<string> _forbiddenOperations { get; } = typeof(Hub).GetRuntimeMethods().Concat(typeof(Hub<>).GetRuntimeMethods()).Select(x => x.Name).Distinct();
        private IEnumerable<MethodInfo> GetOperationMethods(Type type)
        {
            return type.GetTypeInfo().GetRuntimeMethods().Where(m =>
            {
                var returnsChannelReader = m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == typeof(ChannelReader<>);
                return
                    m.IsPublic &&
                    m.IsSpecialName == false &&
                    m.DeclaringType != typeof(Hub) &&
                    m.DeclaringType != typeof(Hub<>) &&
                    m.DeclaringType != typeof(object) &&
                    !_forbiddenOperations.Contains(m.Name) &&
                    returnsChannelReader == false;
            });
        }
    }







    public class SignalrTypesCallBack
    {
        public string Description { get; set; }
        public string MethodName { get; set; }
        public List<SignalrTypesCallBackParameter> SignalrTypesCallBackParameters { get; set; }
    }

    public class SignalrTypesCallBackParameter
    {
        public string Description { get; set; }
        public Type Type { get; set; }
    }

}
