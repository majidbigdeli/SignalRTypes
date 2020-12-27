using Microsoft.AspNetCore.SignalR;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Septa.AspNetCore.SignalRTypes
{
    public class SignalrTypeGenerator
    {
        private readonly SignalrTypeGeneratorSettings _settings;

        public SignalrTypeGenerator(SignalrTypeGeneratorSettings settings)
        {
            _settings = settings;
        }

        public async Task<SignalrTypeDocument> GenerateForHubsAsync(IReadOnlyDictionary<string, Type> hubs, ISignalRTypesBuilder signalRTypesBuilder)
        {
            var document = new SignalrTypeDocument();
            return await GenerateForHubsAsync(hubs, document);
        }

        public async Task<SignalrTypeDocument> GenerateForHubsAsync(IReadOnlyDictionary<string, Type> hubs, SignalrTypeDocument template)
        {

            var document = template;
            var resolver = new SignalrTypeSchemaResolver(document, _settings);
            var generator = new JsonSchemaGenerator(_settings);

            foreach (var h in hubs)
            {
                var type = h.Value;

                var hub = new SignalrTypeHub();
                hub.Name = type.Name.EndsWith("Hub") ? type.Name.Substring(0, type.Name.Length - 3) : type.Name;
                hub.Description = type.GetXmlDocsSummary();

                foreach (var method in GetOperationMethods(type))
                {
                    var operation = await GenerateOperationAsync(type, method, generator, resolver, SignalrTypeOperationType.Sync);
                    hub.Operations[method.Name] = operation;
                }

                foreach (var method in GetChannelMethods(type))
                {
                    hub.Operations[method.Name] = await GenerateOperationAsync(type, method, generator, resolver, SignalrTypeOperationType.Observable);
                }


                //  handle with service this comment

                //var ctors = type.GetConstructors();

                //if (ctors != null && ctors.Length > 0)
                //{
                //    var allTypes = ctors.Select(
                //        x => x.GetParameters()
                //        ?.Where(
                //            z => z.ParameterType.IsGenericType && z.ParameterType.GetGenericTypeDefinition() == typeof(IHubContext<,>))
                //        ).Select(e => e.Select(s => s.ParameterType.GetGenericArguments()[1]))
                //        ?.ToList();

                //    foreach (var itemTypes in allTypes)
                //    {
                //        foreach (var itemType in itemTypes)
                //        {
                //            foreach (var callbackMethod in GetOperationMethods(itemType))
                //            {
                //                var callback = await GenerateOperationAsync(type, callbackMethod, generator, resolver, SignalrTypeOperationType.Sync);

                //                var methodName = callbackMethod.Name;
                //                var hubMethodName = callbackMethod.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(HubMethodNameAttribute));

                //                if (hubMethodName != null && hubMethodName.ConstructorArguments.Count > 0)
                //                {
                //                    var argName = hubMethodName.ConstructorArguments[0].Value.ToString();
                //                    if (!string.IsNullOrEmpty(argName))
                //                    {
                //                        methodName = argName;
                //                    }
                //                }

                //                hub.Callbacks[methodName] = callback;
                //            }

                //        }
                //    }

                //}

                var baseTypeGenericArguments = type.BaseType.GetGenericArguments();
                if (baseTypeGenericArguments.Length == 1)
                {
                    var callbackType = baseTypeGenericArguments[0];
                    foreach (var callbackMethod in GetOperationMethods(callbackType))
                    {
                        var callback = await GenerateOperationAsync(type, callbackMethod, generator, resolver, SignalrTypeOperationType.Sync);

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

                        hub.Callbacks[methodName] = callback;
                    }
                }

                document.Hubs[h.Key] = hub;
            }

            return document;
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

        private IEnumerable<MethodInfo> GetChannelMethods(Type type)
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
                    returnsChannelReader == true;
            });
        }

        private async Task<SignalrTypeOperation> GenerateOperationAsync(Type type, MethodInfo method, JsonSchemaGenerator generator, SignalrTypeSchemaResolver resolver, SignalrTypeOperationType operationType)
        {
            var operation = new SignalrTypeOperation
            {
                Description = method.GetXmlDocsSummary(),
                Type = operationType
            };

            foreach (var arg in method.GetParameters())
            {
                var parameter = generator.GenerateWithReferenceAndNullability<SignalrTypeParameter>(
                    arg.ParameterType.ToContextualType(), arg.ParameterType.ToContextualType().IsNullableType, resolver, (p, s) =>
                    {
                        p.Description = arg.GetXmlDocs();
                    });

                operation.Parameters[arg.Name] = parameter;
            }

            var returnType =
                operationType == SignalrTypeOperationType.Observable
                    ? method.ReturnType.GetGenericArguments().First()
                : method.ReturnType == typeof(Task)
                    ? null
                : method.ReturnType.IsGenericType && method.ReturnType.BaseType == typeof(Task)
                    ? method.ReturnType.GetGenericArguments().First()
                    : method.ReturnType;

            operation.ReturnType = returnType == null ? null : generator.GenerateWithReferenceAndNullability<JsonSchema>(
                returnType.ToContextualType(), returnType.ToContextualType().IsNullableType, resolver, async (p, s) =>
                {
                    p.Description = method.ReturnType.GetXmlDocsSummary();
                });

            return operation;
        }
    }

}
