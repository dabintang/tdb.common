using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace tdb.common
{
    /// <summary>
    /// emit帮助类
    /// </summary>
    public static class EmitHelper<T> where T: class
    {
        #region EmitSetter (object)

        /// <summary>
        /// Emit赋值动态方法
        /// </summary>
        private static readonly Dictionary<string, Action<T, object?>> dicObjSetter = new();

        /// <summary>
        /// Emit给对象属性赋值
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static Action<T, object?> EmitSetter(string propertyName)
        {
            if (dicObjSetter.ContainsKey(propertyName))
            {
                return dicObjSetter[propertyName];
            }

            lock(dicObjSetter)
            {
                if (dicObjSetter.ContainsKey(propertyName))
                {
                    return dicObjSetter[propertyName];
                }

                var type = typeof(T);
                var dynamicMethod = new DynamicMethod("EmitCallable", null, new[] { type, typeof(object) }, type.Module);
                var iLGenerator = dynamicMethod.GetILGenerator();

                var callMethod = type.GetMethod("set_" + propertyName, BindingFlags.Instance | BindingFlags.Public);
                if (callMethod == null)
                {
                    throw new Exception($"未找到类型[{typeof(T)}]的可赋值属性[{propertyName}]。（[EmitHelper]）");
                }
                var parameterInfo = callMethod.GetParameters()[0];
                var local = iLGenerator.DeclareLocal(parameterInfo.ParameterType, true);

                iLGenerator.Emit(OpCodes.Ldarg_1);
                if (parameterInfo.ParameterType.IsValueType)
                {
                    // 如果是值类型，拆箱
                    iLGenerator.Emit(OpCodes.Unbox_Any, parameterInfo.ParameterType);
                }
                else
                {
                    // 如果是引用类型，转换
                    iLGenerator.Emit(OpCodes.Castclass, parameterInfo.ParameterType);
                }

                iLGenerator.Emit(OpCodes.Stloc, local);
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldloc, local);

                iLGenerator.EmitCall(OpCodes.Callvirt, callMethod, null);
                iLGenerator.Emit(OpCodes.Ret);

                if (dynamicMethod.CreateDelegate(typeof(Action<T, object?>)) is not Action<T, object?> method)
                {
                    throw new Exception($"为类型[{typeof(T)}]的属性[{propertyName}]生成动态赋值方式失败。（[EmitHelper]）");
                }
                dicObjSetter[propertyName] = method;

                return method;
            }
        }

        #endregion

        #region EmitGetter (object)

        /// <summary>
        /// Emit取值动态方法
        /// </summary>
        private static readonly Dictionary<string, Func<T, object?>> dicObjGetter = new();

        /// <summary>
        /// Emit获取对象的属性值
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static Func<T, object?> EmitGetter(string propertyName)
        {
            if (dicObjGetter.ContainsKey(propertyName))
            {
                return dicObjGetter[propertyName];
            }

            lock(dicObjGetter)
            {
                if (dicObjGetter.ContainsKey(propertyName))
                {
                    return dicObjGetter[propertyName];
                }

                var type = typeof(T);

                var dynamicMethod = new DynamicMethod("get_" + propertyName, typeof(object), new[] { type }, type);
                var iLGenerator = dynamicMethod.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);

                var property = type.GetProperty(propertyName);
                if (property == null || property.GetMethod == null)
                {
                    throw new Exception($"未找到类型[{typeof(T)}]的属性[{propertyName}]。（[EmitHelper]）");
                }
                iLGenerator.Emit(OpCodes.Callvirt, property.GetMethod);

                if (property.PropertyType.IsValueType)
                {
                    // 如果是值类型，装箱
                    iLGenerator.Emit(OpCodes.Box, property.PropertyType);
                }
                else
                {
                    // 如果是引用类型，转换
                    iLGenerator.Emit(OpCodes.Castclass, property.PropertyType);
                }

                iLGenerator.Emit(OpCodes.Ret);

                if (dynamicMethod.CreateDelegate(typeof(Func<T, object?>)) is not Func<T, object?> method)
                {
                    throw new Exception($"为类型[{typeof(T)}]的属性[{propertyName}]生成动态取值方式失败。（[EmitHelper]）");
                }
                dicObjGetter[propertyName] = method;

                return method;
            }
        }

        #endregion

        #region EmitGetter (泛型)

        /// <summary>
        /// Emit取值动态方法
        /// </summary>
        private static readonly Dictionary<string, Delegate> dicGenericTypeGetter = new();

        /// <summary>
        /// Emit获取对象的属性值
        /// </summary>
        /// <typeparam name="ReturnT">返回类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static Func<T, ReturnT?> EmitGetter<ReturnT>(string propertyName)
        {
            if (dicGenericTypeGetter.ContainsKey(propertyName))
            {
#pragma warning disable CS8603 // 可能返回 null 引用。
                return dicGenericTypeGetter[propertyName] as Func<T, ReturnT?>;
#pragma warning restore CS8603 // 可能返回 null 引用。
            }

            lock(dicGenericTypeGetter)
            {
                if (dicGenericTypeGetter.ContainsKey(propertyName))
                {
#pragma warning disable CS8603 // 可能返回 null 引用。
                    return dicGenericTypeGetter[propertyName] as Func<T, ReturnT?>;
#pragma warning restore CS8603 // 可能返回 null 引用。
                }

                var type = typeof(T);

                var dynamicMethod = new DynamicMethod("get_" + propertyName, typeof(ReturnT?), new[] { type }, type);
                var iLGenerator = dynamicMethod.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);

                var property = type.GetProperty(propertyName);
                if (property == null || property.GetMethod == null)
                {
                    throw new Exception($"未找到类型[{typeof(T)}]的属性[{propertyName}]。（[EmitHelper]）");
                }
                iLGenerator.Emit(OpCodes.Callvirt, property.GetMethod);
                iLGenerator.Emit(OpCodes.Ret);

                if (dynamicMethod.CreateDelegate(typeof(Func<T, ReturnT?>)) is not Func<T, ReturnT?> method)
                {
                    throw new Exception($"为类型[{typeof(T)}]的属性[{propertyName}]生成动态取值方式失败。（[EmitHelper]）");
                }
                dicGenericTypeGetter[propertyName] = method;

                return method;
            }
        }

        #endregion
    }
}
