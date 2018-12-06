using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public static class ClassOriginChecker
    {
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
                            var baseGenericArgs = baseType.GetGenericArguments();
                            var curGenericArgs = toCheck.GetGenericArguments();
                            var genericArgsEquality = baseGenericArgs.SequenceEqual(curGenericArgs);
                            if (!genericArgsEquality)
                            {
                                return false;
                            }
                            return true;
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
                    var baseGenericArgs = baseType.GetGenericArguments();
                    var toCheckGenericArgs = toCheck.GetGenericArguments();
                    var genericArgsEquality = baseGenericArgs.SequenceEqual(toCheckGenericArgs);
                    if (!genericArgsEquality)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    //Normal Interface checker
                    if (!baseType.IsAssignableFrom(toCheck))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
