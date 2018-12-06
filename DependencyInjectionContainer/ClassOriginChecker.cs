using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public static class ClassOriginChecker
    {
        private static bool GenericsParamsEquality(Type baseType, Type toCheck)
        {
            var baseGenericArgs = baseType.GetGenericArguments();
            var toCheckGenericArgs = toCheck.GetGenericArguments();
            bool genericArgsEquality = true;

            if (baseGenericArgs.Length != toCheckGenericArgs.Length)
            {
                return false;
            }

            for (int i = 0; i < baseGenericArgs.Length; i++)
            {
                if (baseGenericArgs[i].Name != toCheckGenericArgs[i].Name || baseGenericArgs[i].FullName != toCheckGenericArgs[i].FullName)
                {
                    genericArgsEquality = false;
                }
            }

            if (!genericArgsEquality)
            {
                return false;
            }
            return true;
        }

        public static bool IsDerivedFrom(Type toCheck, Type baseType)
        {
            //Normal class inheritance check
            if (toCheck.IsClass && baseType.IsClass)
            {
                if (toCheck == baseType)
                {
                    return true;
                }

                if (toCheck.IsSubclassOf(baseType))
                {
                    return true;
                }

                //Generic class check
                if (baseType.IsGenericType)
                {
                    baseType = baseType.GetGenericTypeDefinition();
                    while (toCheck != null && toCheck != typeof(object))
                    {
                        var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                        if (baseType == cur)
                        {
                            if (GenericsParamsEquality(baseType, cur))
                            {
                                return true;
                            }
                            return false;
                        }
                        toCheck = toCheck.BaseType;
                    }
                    return false;
                }

            }

            //Generic Interface checker
            if (toCheck.IsGenericType)
            {
                bool isImplement = toCheck.GetInterfaces().Any(x => x.IsGenericType
                && x.GetGenericTypeDefinition() == baseType.GetGenericTypeDefinition());
                if (isImplement)
                {
                    if (GenericsParamsEquality(baseType, toCheck))
                    {
                        return true;
                    }
                    return false;
                }
            }

            //Normal Interface checker
            if (!baseType.IsAssignableFrom(toCheck))
            {
                return false;
            }

            return true;
        }
    }
}
