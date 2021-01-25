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
        private static Dictionary<string, Action<T, object>> dicObjSetter = new Dictionary<string, Action<T, object>>();

        /// <summary>
        /// Emit给对象属性赋值
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static Action<T, object> EmitSetter(string propertyName)
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

                var callMethod = type.GetMethod("set_" + propertyName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
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

                var method = dynamicMethod.CreateDelegate(typeof(Action<T, object>)) as Action<T, object>;
                dicObjSetter[propertyName] = method;

                return method;
            }
        }

        #endregion

        #region EmitGetter (object)

        /// <summary>
        /// Emit取值动态方法
        /// </summary>
        private static Dictionary<string, Func<T, object>> dicObjGetter = new Dictionary<string, Func<T, object>>();

        /// <summary>
        /// Emit获取对象的属性值
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static Func<T, object> EmitGetter(string propertyName)
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

                var method = dynamicMethod.CreateDelegate(typeof(Func<T, object>)) as Func<T, object>;
                dicObjGetter[propertyName] = method;

                return method;
            }
        }

        #endregion

        #region EmitGetter (泛型)

        /// <summary>
        /// Emit取值动态方法
        /// </summary>
        private static Dictionary<string, Delegate> dicGenericTypeGetter = new Dictionary<string, Delegate>();

        /// <summary>
        /// Emit获取对象的属性值
        /// </summary>
        /// <typeparam name="ReturnT">返回类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static Func<T, ReturnT> EmitGetter<ReturnT>(string propertyName)
        {
            if (dicGenericTypeGetter.ContainsKey(propertyName))
            {
                return dicGenericTypeGetter[propertyName] as Func<T, ReturnT>;
            }

            lock(dicGenericTypeGetter)
            {
                if (dicGenericTypeGetter.ContainsKey(propertyName))
                {
                    return dicGenericTypeGetter[propertyName] as Func<T, ReturnT>;
                }

                var type = typeof(T);

                var dynamicMethod = new DynamicMethod("get_" + propertyName, typeof(ReturnT), new[] { type }, type);
                var iLGenerator = dynamicMethod.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);

                var property = type.GetProperty(propertyName);
                iLGenerator.Emit(OpCodes.Callvirt, property.GetMethod);
                iLGenerator.Emit(OpCodes.Ret);

                var method = dynamicMethod.CreateDelegate(typeof(Func<T, ReturnT>)) as Func<T, ReturnT>;
                dicGenericTypeGetter[propertyName] = method;

                return method;
            }
        }

        #endregion
    }
}
