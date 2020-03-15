﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ShuHai
{
    public static class TypeExtensions
    {
        /// <summary>
        ///     Get a value indicating how many hierarchy levels the specified type is derived or parented from specified type.
        /// </summary>
        /// <param name="self">From which the depth comes.</param>
        /// <param name="type">The type from which the depth is calculated.</param>
        /// <returns>
        ///     A value indicating number of types in hierarchy between <paramref name="self" /> and <paramref name="type" />
        ///     if they have relationship of inheriting; otherwise, <see cref="ArgumentException" /> is thrown.
        ///     A positive value means the specified <paramref name="type" /> is child of <see cref="self" />; otherwise,
        ///     a negative value means the specified <paramref name="type" /> is parent of <see cref="self" />.
        /// </returns>
        /// <remarks>
        ///     There might be multiple inheriting path for interfaces, thus multiple hierarchy levels existed. The
        ///     shortest path is chosen in such case.
        /// </remarks>
        public static int GetDeriveDepth(this Type self, Type type = null)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            type = type ?? typeof(object);

            if (self == type)
                return 0;
            if (type.IsAssignableFrom(self))
                return GetDerivedDepthImpl(self, type);
            if (self.IsAssignableFrom(type))
                return -GetDerivedDepthImpl(type, self);

            throw new ArgumentException("Type is not in the hierarchy of current instance.", nameof(type));
        }

        private static int GetDerivedDepthImpl(Type derived, Type @base)
        {
            if (@base.IsInterface)
            {
                int depth = 1;
                var interfaces = derived.GetMostDerivedInterfaces();
                while (!interfaces.Contains(@base))
                {
                    depth++;
                    interfaces = interfaces.SelectMany(i => i.GetMostDerivedInterfaces()).Distinct().ToArray();
                }
                return depth;
            }
            else
            {
                Debug.Assert(!derived.IsInterface);

                int depth = 1;
                var t = derived.BaseType;
                while (t != @base && t != null)
                {
                    depth++;
                    t = t.BaseType;
                }
                return depth;
            }
        }

        /// <summary>
        ///     Get all interfaces of specified type and filter them by removing types those derived from any other.
        /// </summary>
        /// <param name="self">The type instance that derived or implemented the interfaces.</param>
        /// <returns>
        ///     An array of <see cref="Type" /> instances that contains only most derived interfaces of <paramref name="self" />;
        ///     or an empty array if no interfaces are implemented or inherited by <paramref name="self" />.
        /// </returns>
        public static Type[] GetMostDerivedInterfaces(this Type self)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            var interfaces = self.GetInterfaces();
            //return interfaces.Where(i => !interfaces.Any(ii => i != ii && i.IsAssignableFrom(ii))).ToArray();
            return interfaces.Except(interfaces.SelectMany(t => t.GetInterfaces())).ToArray();
        }

        #region Member Getters

        /// <summary>
        ///     Shortcut for <see cref="FindConstructor(Type,bool,Type[])" />(<paramref name="self" />, <see langword="false" />,
        ///     <see cref="Type.EmptyTypes" />).
        /// </summary>
        public static ConstructorInfo FindDefaultConstructor(this Type self)
        {
            return FindConstructor(self, Type.EmptyTypes);
        }

        /// <summary>
        ///     Shortcut for <see cref="FindConstructor(Type,bool,Type[])" />(<paramref name="self" />, <see langword="false" />,
        ///     <paramref name="parameterTypes" />).
        /// </summary>
        public static ConstructorInfo FindConstructor(this Type self, params Type[] parameterTypes)
        {
            return FindConstructor(self, false, parameterTypes);
        }

        /// <summary>
        ///     Searches for constructor with specified parameter types.
        /// </summary>
        /// <param name="self">The <see cref="Type" /> instance to search.</param>
        /// <param name="isStatic">Determines which constructor search, the static one or instance one.</param>
        /// <param name="parameterTypes">
        ///     An array of Type objects representing the number, order, and type of the parameters for the desired
        ///     constructor.
        /// </param>
        /// <returns>
        ///     A <see cref="ConstructorInfo" /> object representing the default constructor, if found; otherwise,
        ///     <see langword="null" />.
        /// </returns>
        /// <remarks>
        ///     Note that the underlying <see cref="BindingFlags" /> for search contains <see cref="BindingFlags.DeclaredOnly " />,
        ///     this makes the search only occurs at the hierarchy level of the specified type.
        /// </remarks>
        public static ConstructorInfo FindConstructor(this Type self, bool isStatic, params Type[] parameterTypes)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            var bindingAttr = isStatic
                ? BindingAttrForMemberSearch & ~BindingFlags.Instance
                : BindingAttrForMemberSearch & ~BindingFlags.Static;
            return self.GetConstructor(bindingAttr, null, parameterTypes, null);
        }

        /// <summary>
        ///     Searches for field with specified name.
        /// </summary>
        /// <param name="self">The <see cref="Type" /> instance to search.</param>
        /// <param name="name">Name of the field.</param>
        /// <returns>
        ///     A <see cref="FieldInfo" /> object representing the field with specified name, if found; otherwise,
        ///     <see langword="null" />.
        /// </returns>
        /// <remarks>
        ///     Note that the underlying <see cref="BindingFlags" /> for search contains <see cref="BindingFlags.DeclaredOnly " />,
        ///     this makes the search only occurs at the hierarchy level of the specified type.
        /// </remarks>
        public static FieldInfo FindField(this Type self, string name)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.GetField(name, BindingAttrForMemberSearch);
        }

        /// <summary>
        ///     Searches for property with specified name.
        /// </summary>
        /// <param name="self">The <see cref="Type" /> instance to search.</param>
        /// <param name="name">Name of the property.</param>
        /// <returns>
        ///     A <see cref="FieldInfo" /> object representing the property with specified name, if found; otherwise,
        ///     <see langword="null" />.
        /// </returns>
        /// <remarks>
        ///     Note that the underlying <see cref="BindingFlags" /> for search contains <see cref="BindingFlags.DeclaredOnly " />,
        ///     this makes the search only occurs at the hierarchy level of the specified type.
        /// </remarks>
        public static PropertyInfo FindProperty(this Type self, string name)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.GetProperty(name, BindingAttrForMemberSearch);
        }

        /// <summary>
        ///     Shortcut for <see cref="FindMethod(Type,string,Type[])" />(<paramref name="self" />, <paramref name="name" />,
        ///     <see cref="Type.EmptyTypes" />).
        /// </summary>
        public static MethodInfo FindMethod(this Type self, string name)
        {
            return FindMethod(self, name, Type.EmptyTypes);
        }

        /// <summary>
        ///     Searches for method with specified name.
        /// </summary>
        /// <param name="self"> The <see cref="Type" /> instance to search. </param>
        /// <param name="name"> Name of the method. </param>
        /// <param name="parameterTypes">
        ///     An array of Type objects representing the number, order, and type of the parameters for the desired
        ///     method.
        /// </param>
        /// <returns>
        ///     An object representing the method that matches the specified requirements, if found; otherwise,
        ///     <see langword="null" />.
        /// </returns>
        /// <remarks>
        ///     The method searches both public and non-public members, and performs exact matching on specified type
        ///     only. (The underlying <see cref="BindingFlags" /> for search contains <see cref="BindingFlags.DeclaredOnly " />,
        ///     this makes the search only occurs at the hierarchy level of the specified type.)
        /// </remarks>
        public static MethodInfo FindMethod(this Type self, string name, params Type[] parameterTypes)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.GetMethod(name, BindingAttrForMemberSearch, null, parameterTypes, null);
        }

        private const BindingFlags BindingAttrForMemberSearch = BindingFlags.DeclaredOnly |
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        #endregion Member Getters

        #region Related Type Enumeration

        /// <summary>
        ///     Enumerate all base types of <paramref name="self" />.
        /// </summary>
        /// <param name="self">Base type of which to enumerate.</param>
        /// <remarks>
        ///     Order of the enumeration is from <paramref name="self" /> to <see cref="object" />.
        /// </remarks>
        public static IEnumerable<Type> EnumerateBaseTypes(this Type self)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            var t = self.BaseType;
            while (t != null)
            {
                yield return t;
                t = t.BaseType;
            }
        }

        /// <summary>
        ///     Enumerate all base types of specified type and the type itself.
        /// </summary>
        /// <param name="self"> Most derived type in the class hierarchy of the enumeration. </param>
        /// <remarks>
        ///     Order of the enumeration is from <paramref name="self" /> to <see cref="object" />.
        /// </remarks>
        public static IEnumerable<Type> EnumerateSelfAndBaseTypes(this Type self)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            var t = self;
            while (t != null)
            {
                yield return t;
                t = t.BaseType;
            }
        }

        /// <summary>
        ///     Similar with <see cref="EnumerateDerivedTypes(System.Type,System.Collections.Generic.IEnumerable{System.Type})" />,
        ///     but search types from
        ///     assemblies.
        /// </summary>
        /// <param name="self"> Base type to enumerate. </param>
        /// <param name="searchAssemblies">
        ///     Where derived types come from. Assembly of <paramref name="self" /> is used if null.
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="self" /> is null. </exception>
        public static IEnumerable<Type> EnumerateDerivedTypes(this Type self, IEnumerable<Assembly> searchAssemblies)
        {
            return EnumerateTypes(EnumerateDerivedTypes, self, searchAssemblies);
        }

        /// <summary>
        ///     Enumerate derived types of <paramref name="self" /> in <paramref name="searchTypes" />.
        /// </summary>
        /// <param name="self"> Base type to enumerate. </param>
        /// <param name="searchTypes"> Where derived types come from. </param>
        /// <returns> Derived types of <paramref name="self" />. </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self" /> or <paramref name="searchTypes" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="searchTypes" /> is empty.
        /// </exception>
        public static IEnumerable<Type> EnumerateDerivedTypes(this Type self, IEnumerable<Type> searchTypes)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNullOrEmpty(searchTypes, "searchTypes");
            return searchTypes.Where(t => t.IsSubclassOf(self)).Distinct();
        }

        /// <summary>
        ///     Similar with <see cref="EnumerateDirectDerivedTypes(Type,IEnumerable{Type})" />, but search types
        ///     from assemblies.
        /// </summary>
        /// <param name="self"> Base type to enumerate. </param>
        /// <param name="searchAssemblies">
        ///     Where derived types come from.
        ///     Assembly of <paramref name="self" /> is used if null.
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="self" /> is null. </exception>
        public static IEnumerable<Type> EnumerateDirectDerivedTypes(
            this Type self, IEnumerable<Assembly> searchAssemblies)
        {
            return EnumerateTypes(EnumerateDirectDerivedTypes, self, searchAssemblies);
        }

        /// <summary>
        ///     Enumerate directly inherited derived types of <paramref name="self" />.
        /// </summary>
        /// <param name="self"> Base type to enumerate. </param>
        /// <param name="searchTypes"> Where derived types come from. </param>
        /// <returns> Directly inherited derived types of <paramref name="self" />. </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self" /> or <paramref name="searchTypes" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="searchTypes" /> is empty.
        /// </exception>
        public static IEnumerable<Type> EnumerateDirectDerivedTypes(this Type self, IEnumerable<Type> searchTypes)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNullOrEmpty(searchTypes, "searchTypes");
            return EnumerateDerivedTypes(self, searchTypes).Where(t => t.BaseType == self).Distinct();
        }

        /// <summary>
        ///     Similar with <see cref="EnumerateSelfAndDerivedTypes(Type,IEnumerable{Type})" />, but search types from specified
        ///     assembly list.
        /// </summary>
        /// <param name="self"> Root type for enumerating. </param>
        /// <param name="searchAssemblies">
        ///     Where derived types come from.
        ///     Assembly of <paramref name="self" /> is used if null.
        /// </param>
        public static IEnumerable<Type> EnumerateSelfAndDerivedTypes(
            this Type self, IEnumerable<Assembly> searchAssemblies)
        {
            return EnumerateTypes(EnumerateSelfAndDerivedTypes, self, searchAssemblies);
        }

        /// <summary>
        ///     Enumerate all derived types of specified <see cref="Type" /> and the <see cref="Type" /> itself.
        /// </summary>
        /// <param name="self"> Root type for enumerating. </param>
        /// <param name="searchTypes"> From where derived types come from. </param>
        /// <returns>
        ///     A enumerable collection that contains <paramref name="self" /> and derived types of
        ///     <paramref name="self" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self" /> or <paramref name="searchTypes" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="searchTypes" /> is empty.
        /// </exception>
        public static IEnumerable<Type> EnumerateSelfAndDerivedTypes(this Type self, IEnumerable<Type> searchTypes)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNullOrEmpty(searchTypes, "searchTypes");
            return self.Concat(EnumerateDerivedTypes(self, searchTypes)).Distinct();
        }

        /// <summary>
        ///     Similar with <see cref="EnumerateConstructedTypes(Type,IEnumerable{Type})" />, but search types from
        ///     assemblies.
        /// </summary>
        /// <param name="self"> Generic type from which the result types are constructed. </param>
        /// <param name="searchAssemblies">
        ///     Where constructed types come from.
        ///     Assembly of <paramref name="self" /> is used if null.
        /// </param>
        public static IEnumerable<Type> EnumerateConstructedTypes(
            this Type self, IEnumerable<Assembly> searchAssemblies)
        {
            return EnumerateTypes(EnumerateConstructedTypes, self, searchAssemblies);
        }

        /// <summary>
        ///     All constructed types of specified generic <see cref="Type" />.
        /// </summary>
        /// <param name="self"> Generic type from which the result types are constructed. </param>
        /// <param name="searchTypes"> Where constructed types come from. </param>
        /// <returns> Types constructed from <paramref name="self" />. </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self" /> or <paramref name="searchTypes" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="searchTypes" /> is empty.
        ///     <paramref name="self" /> is not generic type definition.
        /// </exception>
        public static IEnumerable<Type> EnumerateConstructedTypes(this Type self, IEnumerable<Type> searchTypes)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            if (!self.IsGenericTypeDefinition)
                throw new ArgumentException("Generic type definition expected.", nameof(self));
            Ensure.Argument.NotNullOrEmpty(searchTypes, "searchTypes");

            return searchTypes.Where(t => t.IsClosedConstructedTypeOf(self)).Distinct();
        }

        private static IEnumerable<Type> EnumerateTypes(
            Func<Type, IEnumerable<Type>, IEnumerable<Type>> method, Type type, IEnumerable<Assembly> assemblies)
        {
            if (CollectionUtil.IsNullOrEmpty(assemblies))
                assemblies = new[] { type.Assembly };
            return method(type, assemblies.Where(a => a != null).Distinct().SelectMany(a => a.GetTypes()));
        }

        #endregion Related Type Enumeration

        /// <summary>
        ///     Mock keyword <see langword="is" /> for <see cref="Type" />.
        /// </summary>
        /// <param name="self">The type instance to check.</param>
        /// <typeparam name="T">Target type to compare.</typeparam>
        public static bool Is<T>(this Type self)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return typeof(T).IsAssignableFrom(self);
        }

        /// <summary>
        ///     Mock keyword <see langword="is" /> for <see cref="Type" />.
        /// </summary>
        /// <param name="self">The type instance to check.</param>
        /// <param name="type">The target type to compare.</param>
        public static bool Is(this Type self, Type type)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(type, nameof(type));
            return type.IsAssignableFrom(self);
        }

        /// <summary>
        ///     Indicates whether the current type is a numeric type.
        /// </summary>
        /// <param name="self">The current type instance.</param>
        /// <returns>
        ///     <see langword="true" /> if the current type is a numeric type; otherwise <see langword="false" />.
        /// </returns>
        public static bool IsNumeric(this Type self)
        {
            switch (Type.GetTypeCode(self))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Indicates whether the current type is instantiable.
        /// </summary>
        public static bool IsInstantiable(this Type self)
        {
            return !self.IsInterface && !self.IsAbstract
                && !self.IsGenericTypeDefinition
                && !self.IsPartialConstructedType();
        }

        /// <summary>
        ///     Get a value indicates whether the current type is closed constructed generic type.
        /// </summary>
        /// <param name="self"> Current type to check. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="self" /> is a closed constructed generic type; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="self" /> is null. </exception>
        /// <remarks>
        ///     Closed constructed generic type means that all type parameters of the generic type have been replaced by
        ///     specific types.
        /// </remarks>
        public static bool IsClosedConstructedType(this Type self)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.IsGenericType && !self.ContainsGenericParameters;
        }

        /// <summary>
        ///     Get a value indicates whether the current type is closed constructed generic type from a specified generic
        ///     type definition.
        /// </summary>
        /// <param name="self"> The current type to check. </param>
        /// <param name="type"> The generic type definition to check. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="self" /> is closed constructed from <paramref name="type" />;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self" /> or <paramref name="type" /> is null.
        /// </exception>
        public static bool IsClosedConstructedTypeOf(this Type self, Type type)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(type, nameof(type));
            return self.IsClosedConstructedType() && self.GetGenericTypeDefinition() == type;
        }

        /// <summary>
        ///     Get a value indicates whether the current type is a partial constructed generic type.
        /// </summary>
        /// <param name="self"> The type to check. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="self" /> is a partial constructed generic type; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="self" /> is null. </exception>
        /// <remarks>
        ///     Partial constructed generic type means that part of the type parameters of the generic type have been
        ///     replaced by specific types, but others remained as generic type parameter.
        /// </remarks>
        public static bool IsPartialConstructedType(this Type self)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.IsGenericType && !self.IsGenericTypeDefinition && self.ContainsGenericParameters;
        }

        /// <summary>
        ///     Get a value indicates whether the current type is a partial constructed generic type from a specified
        ///     generic type definition.
        /// </summary>
        /// <param name="self"> The current type to check. </param>
        /// <param name="type"> The generic type definition to compare. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="self" /> is partial constructed from <paramref name="type" />;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsPartialConstructedTypeOf(this Type self, Type type)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(type, nameof(type));
            return self.IsPartialConstructedType() && self.GetGenericTypeDefinition() == type;
        }

        /// <summary>
        ///     Similar to <see cref="Type.IsSubclassOf(Type)" /> but also available on generic types.
        /// </summary>
        /// <param name="self">Current type instance.</param>
        /// <param name="type">The type to compare with the current type.</param>
        /// <returns>
        ///     <see langword="true" /> if the current Type derives from <paramref name="type" /> or its generic type definition;
        ///     otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="type" />
        ///     and the current Type are equal.
        /// </returns>
        public static bool IsBroadlySubclassOf(this Type self, Type type)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            if (type == null || self == type)
                return false;

            if (!type.IsGenericType && !self.IsGenericType)
                return self.IsSubclassOf(type);

            type = type.GetGenericTypeDefinition();
            self = self.BaseType;
            while (self != typeof(object) && self != null)
            {
                var t = self.IsGenericType ? self.GetGenericTypeDefinition() : self;
                if (t == type)
                    return true;
                self = self.BaseType;
            }
            return false;
        }

        /// <summary>
        ///     Get a value indicating whether <paramref name="self" /> is anonymous type.
        /// </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="self" /> is null. </exception>
        public static bool IsAnonymous(this Type self)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            return Attribute.IsDefined(self, typeof(CompilerGeneratedAttribute), false)
                && self.IsGenericType && self.Name.Contains("AnonymousType")
                && (self.Name.StartsWith("<>") || self.Name.StartsWith("VB$"))
                && (self.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        public static string GetName(this Type self, bool fullName)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            return self.IsGenericParameter ? self.Name // Generic parameter doesn't have a full name.
                : fullName ? self.FullName : self.Name;
        }
    }
}