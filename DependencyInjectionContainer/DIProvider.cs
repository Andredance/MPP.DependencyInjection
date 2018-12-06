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
        private List<Type> recursionControl;

        public DIProvider(DIConfiguration configuration)
        {
            ValidateConfiguration(configuration);
            this.configuration = configuration;
            iEnumerableControl = false;
            recursionControl = new List<Type>();
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
                        throw new ArgumentException("TImplementation must implement or be inherited from TDependency. If class is generic, generic arguments of TDependency and TImplementation must be the same.");
                    }
                }
            }
        }

        private bool GenericParamsCheck(Type type)
        {
            if (type.IsGenericType)
            {
                foreach (Type genericParam in type.GenericTypeArguments)
                {
                    if (!configuration.IsSingleton.ContainsKey(genericParam))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private object OpenGenericCreater(Type implementationType)
        {
            if (!GenericParamsCheck(implementationType))
            {
                throw new InvalidOperationException(string.Format("Can not create instance of type {0}. Generic arguments of type not registered in the container.", implementationType));
            }
            return CreateInstance(implementationType);
        }

        public TDependency Resolve<TDependency>()
        {
            iEnumerableControl = false;
            return (TDependency)Resolve(typeof(TDependency));
        }
        
        internal object Resolve(Type type)
        {
            Type typeToResolve = type;

            if (typeToResolve.IsGenericType 
                && typeToResolve.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) 
                && !iEnumerableControl)
            {
                typeToResolve = typeToResolve.GetGenericArguments()[0];
                iEnumerableControl = true;
                object implList = Activator.CreateInstance(typeof(List<>).MakeGenericType(typeToResolve));
                bool isGenericTypeDef = false;
                List<Type> implementations;
                if (typeToResolve.IsGenericType && configuration.IsSingleton.ContainsKey(typeToResolve.GetGenericTypeDefinition()))
                {
                    implementations = configuration.DContainer[typeToResolve.GetGenericTypeDefinition()];
                    isGenericTypeDef = true;
                } else
                {
                    implementations = configuration.DContainer[typeToResolve];
                }

                foreach (Type tImplementation in implementations)
                {
                    object instance;
                    if (isGenericTypeDef)
                    {
                        var implementation = tImplementation.MakeGenericType(typeToResolve.GetGenericArguments());
                        instance = OpenGenericCreater(implementation);
                    } else
                    {
                        instance = CreateInstance(tImplementation);
                    }
                    ((IList)implList).Add(instance);
                }

                return implList;
            }

            //Open generic handler
            if (typeToResolve.IsGenericType && configuration.IsSingleton.ContainsKey(typeToResolve.GetGenericTypeDefinition()))
            {
                if (configuration.DContainer[typeToResolve.GetGenericTypeDefinition()].Count > 1)
                {
                    throw new ArgumentException(string.Format("Can not create instance of type {0}. More than one realization of dependency exist.", typeToResolve));
                }
                var implementation = configuration.DContainer[typeToResolve.GetGenericTypeDefinition()][0];
                implementation = implementation.MakeGenericType(typeToResolve.GetGenericArguments());
                return OpenGenericCreater(implementation);
            }

            if (!configuration.IsSingleton.ContainsKey(typeToResolve))
            {
                throw new InvalidOperationException(string.Format("Can not create instance of type {0}. Not registered in the container.", typeToResolve));
            }

            if (configuration.IsSingleton[typeToResolve])
            {
                return configuration.SContainer[typeToResolve].GetInstance(this);
            }

            if (configuration.DContainer[typeToResolve].Count > 1)
            {
                throw new ArgumentException(string.Format("Can not create instance of type {0}. More than one realization of dependency exist.", typeToResolve));
            }

            if (!GenericParamsCheck(typeToResolve))
            {
                throw new InvalidOperationException(string.Format("Can not create instance of type {0}. Generic arguments of type not registered in the container.", typeToResolve));
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

        internal object CreateInstance(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            ConstructorInfo constructor = GetBestConstructor(constructors);

            if (constructor == null)
            {
                throw new InvalidOperationException(string.Format("Can not create instance of type {0}. Satisfying constructor does not exist.", type));
            }

            ParameterInfo[] parameters = constructor.GetParameters();
            object[] newParams = new object[parameters.Length];

            recursionControl.Add(type);

            for (int i = 0; i < parameters.Length; i++)
            {
                Type paramType = parameters[i].ParameterType;

                if (recursionControl.Contains(paramType))
                {
                    throw new InvalidOperationException(string.Format("Can not create instance of type {0}. Recursion control list contain this type.", type));
                }

                recursionControl.Add(paramType);
                
                if (paramType.IsGenericType)
                {
                    newParams[i] = Resolve(paramType.GetGenericTypeDefinition()
                        .MakeGenericType(paramType.GenericTypeArguments));
                } else
                {
                    newParams[i] = Resolve(paramType);
                }

                recursionControl.Remove(paramType);
            }

            recursionControl.Remove(type);

            return constructor.Invoke(newParams);
        }
    }
}
