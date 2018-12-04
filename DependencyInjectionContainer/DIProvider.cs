using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DIProvider
    {
        private readonly DIConfiguration configuration;
        private bool iEnumerableControl;

        public DIProvider(DIConfiguration configuration)
        {
            ValidateConfiguration(configuration);
            this.configuration = configuration;
            iEnumerableControl = false;
        }

        public void ValidateConfiguration(DIConfiguration config)
        {
            foreach (Type dependencyType in config.IsSingleton.Keys)
            {
                if (dependencyType.IsValueType)
                {
                    throw new ArgumentException("TDependency must be a reference type");
                }

                foreach (Type implementationType in config.DContainer[dependencyType])
                {
                    if (implementationType.IsAbstract || !implementationType.IsClass)
                    {
                        throw new ArgumentException("TImplementation must be class(not abstract)");
                    }

                    if (!ClassOriginChecker.IsDerivedFrom(implementationType, dependencyType))
                    {
                        throw new ArgumentException("TImplementation must implement or be inherited from TDependency");
                    }
                }
            }
        }

        public TDependency Resolve<TDependency>()
        {
            iEnumerableControl = false;
            return (TDependency)Resolve(typeof(TDependency));
        }
        
        public object Resolve(Type type)
        {
            Type typeToResolve = type;
            if (typeToResolve.IsGenericType 
                && typeToResolve.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) 
                && !iEnumerableControl)
            {
                typeToResolve = typeToResolve.GetGenericArguments()[0];
                iEnumerableControl = true;
                object implList = Activator.CreateInstance(typeof(List<>).MakeGenericType(typeToResolve));
                foreach (Type tImplementation in configuration.DContainer[typeToResolve])
                {
                    ((IList)implList).Add(CreateInstance(tImplementation));
                }

                return implList;
            }

            if (!configuration.IsSingleton.ContainsKey(typeToResolve))
            {
                throw new InvalidOperationException(string.Format("Can not create instance of type {0}. Not registered in the container.", typeToResolve));
            }

            if (typeToResolve.IsGenericType)
            {
                foreach (Type genericParam in typeToResolve.GenericTypeArguments)
                {
                    if (!configuration.IsSingleton.ContainsKey(genericParam))
                    {
                        throw new InvalidOperationException(string.Format("Can not create instance of type {0}. Not registered in the container.", genericParam));
                    }
                }
            }

            if (configuration.DContainer[typeToResolve].Count > 1)
            {
                throw new ArgumentException(string.Format("Can not create instance of type {0}. More than one realization of dependency exist.", typeToResolve));
            }

            return CreateInstance(configuration.DContainer[typeToResolve][0]);
        }

        private ConstructorInfo GetBestConstructor(ConstructorInfo[] constructors)
        {
            ConstructorInfo bestConstructor = null;

            var sortedConstructors = constructors.OrderBy(x => -x.GetParameters().Length);

            foreach (ConstructorInfo constructor in sortedConstructors)
            {
                if (CheckConstructorParameters(constructor.GetParameters()))
                {
                    bestConstructor = constructor;
                    break;
                }
            }
            return bestConstructor;
        }

        private bool CheckConstructorParameters(ParameterInfo[] parameters)
        {
            foreach (ParameterInfo parameter in parameters)
            {
                if (!configuration.IsSingleton.ContainsKey(parameter.ParameterType))
                {
                    return false;
                }
            }
            return true;
        }

        private object CreateInstance(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            ConstructorInfo constructor = GetBestConstructor(constructors);

            if (constructor == null)
            {
                throw new InvalidOperationException(string.Format("Can not create instance of type {0}. Satisfying constructor does not exist.", type));
            }

            ParameterInfo[] parameters = constructor.GetParameters();
            object[] newParams = new object[parameters.Length];
            
            for (int i = 0; i < parameters.Length; i++)
            {
                Type paramType = parameters[i].ParameterType;

                if (configuration.DContainer[paramType].Contains(type))
                {
                    throw new InvalidOperationException(string.Format("Can not create instance of type {0}. Implementation class constructor's parameter refers to this implementation class.", type));
                }

                if (paramType.IsGenericType)
                {
                    newParams[i] = Resolve(paramType.GetGenericTypeDefinition()
                        .MakeGenericType(paramType.GenericTypeArguments));
                } else
                {
                    if (configuration.DContainer.ContainsKey(paramType))
                    {
                        newParams[i] = Resolve(paramType);
                    }
                }
            }

            return constructor.Invoke(newParams);
        }
    }
}
