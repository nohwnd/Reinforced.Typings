﻿using System;
using System.IO;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;
using Reinforced.Typings.Generators;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Extensions for fluent configuration
    /// </summary>
    public static class TypingsConfigurationExtensions
    {
        /// <summary>
        ///     Ignores specified mambers during exporting
        /// </summary>
        public static T Ignore<T>(this T conf) where T : IIgnorable
        {
            conf.Ignore = true;
            return conf;
        }

        #region CodeGenerator extensions

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsClassAttribute> WithCodeGenerator<T>(
            this IAttributed<TsClassAttribute> conf)
            where T : ITsCodeGenerator<Type>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsInterfaceAttribute> WithCodeGenerator<T>(
            this IAttributed<TsInterfaceAttribute> conf)
            where T : ITsCodeGenerator<Type>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsEnumAttribute> WithCodeGenerator<T>(
            this IAttributed<TsEnumAttribute> conf)
            where T : ITsCodeGenerator<Type>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsPropertyAttribute> WithCodeGenerator<T>(
            this IAttributed<TsPropertyAttribute> conf)
            where T : ITsCodeGenerator<MemberInfo>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsFunctionAttribute> WithCodeGenerator<T>(
            this IAttributed<TsFunctionAttribute> conf)
            where T : ITsCodeGenerator<MethodInfo>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsParameterAttribute> WithCodeGenerator<T>(
            this IAttributed<TsParameterAttribute> conf)
            where T : TsCodeGeneratorBase<ParameterInfo, RtArgument>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }

        #endregion

        #region Reference extensions

        /// <summary>
        ///     Adds reference directive to file containing typing for current type
        ///     This method is only used while splitting generated types to different files
        /// </summary>
        /// <param name="configuration">Configurator</param>
        /// <param name="referenceFile">Path to referenced file</param>
        public static T AddReference<T>(this T configuration, string referenceFile)
            where T : IReferenceConfigurationBuilder
        {
            configuration.References.Add(new TsAddTypeReferenceAttribute(referenceFile));
            return configuration;
        }

        /// <summary>
        ///     Adds reference directive to file containing typing for current type
        ///     This method is only used while splitting generated types to different files
        /// </summary>
        /// <param name="configuration">Configurator</param>
        /// <param name="referencedType">Another generated type that should be referenced</param>
        public static T AddReference<T>(this T configuration, Type referencedType)
            where T : IReferenceConfigurationBuilder
        {
            configuration.References.Add(new TsAddTypeReferenceAttribute(referencedType));
            return configuration;
        }

        /// <summary>
        ///     Adds import directive to file containing typing for current type
        ///     This method is only used while splitting generated types to different files
        /// </summary>
        /// <param name="configuration">Configurator</param>
        /// <param name="target">
        /// What we are importing from module.
        /// Everything that is placed after "import" keyword and before "from" or "= require(..)"
        /// Examples: 
        /// - "import * as shape from './Shapes'" -> "* as shape" is target <br/>
        /// - "import { Foo } from 'Bar'" -> "{ Foo }" is target <br/>
        /// - "import { Bar2 as bar } from 'Baz'" -> "{ Bar2 as bar }" is target <br/>
        /// If ImportTarget is null then side-effect import will be generated. 
        /// </param>
        /// <param name="from">
        /// Import source is everything that follows after "from" keyword. 
        /// Please not the you do not have to specify quotes here! Quotes will be added automatically
        /// </param>
        /// <param name="isRequire">When true, import will be generated as "import ImportTarget = require('ImportSource')"</param>
        public static T AddImport<T>(this T configuration, string target, string from, bool isRequire = false)
            where T : IReferenceConfigurationBuilder
        {
            configuration.Imports.Add(new TsAddTypeImportAttribute(target, from, isRequire));
            return configuration;
        }

        /// <summary>
        ///     Adds global reference to another typescript library
        /// </summary>
        /// <param name="conf">Table configurator</param>
        /// <param name="path">Full path to .d.ts or .ts file</param>
        /// <returns>Fluent</returns>
        public static ConfigurationBuilder AddReference(this ConfigurationBuilder conf, string path)
        {
            conf.References.Add(new RtReference { Path = path });
            return conf;
        }

        /// <summary>
        ///     Adds import directive to file containing typing for current type
        ///     This method is only used while splitting generated types to different files
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="target">
        /// What we are importing from module.
        /// Everything that is placed after "import" keyword and before "from" or "= require(..)"
        /// Examples: 
        /// - "import * as shape from './Shapes'" -> "* as shape" is target <br/>
        /// - "import { Foo } from 'Bar'" -> "{ Foo }" is target <br/>
        /// - "import { Bar2 as bar } from 'Baz'" -> "{ Bar2 as bar }" is target <br/>
        /// If ImportTarget is null then side-effect import will be generated. 
        /// </param>
        /// <param name="from">
        /// Import source is everything that follows after "from" keyword. 
        /// Please not the you do not have to specify quotes here! Quotes will be added automatically
        /// </param>
        /// <param name="isRequire">When true, import will be generated as "import ImportTarget = require('ImportSource')"</param>
        public static ConfigurationBuilder AddImport(this ConfigurationBuilder conf, string target, string from, bool isRequire = false)
        {
            conf.Imports.Add(new RtImport() { Target = target, From = from, IsRequire = isRequire });
            return conf;
        }

        /// <summary>
        ///     Overrides target file name where specified name will be exported.
        ///     This option will only be processed when RtDivideTypesAmongFiles is true.
        /// </summary>
        /// <param name="configuration">Configurator</param>
        /// <param name="fileName">Target file path override. Related to RtTargetDirectory</param>
        public static T ExportTo<T>(this T configuration, string fileName) where T : IReferenceConfigurationBuilder
        {
            configuration.PathToFile = fileName;
            return configuration;
        }

        #endregion

        #region Auto export control - temporary disabled

        ///// <summary>
        ///// Makes generator automatically look up for type methods and export them. 
        ///// Please note that this configuration setting will also export static and private members with corresponding modifiers.
        ///// </summary>
        ///// <param name="conf">Configuration</param>
        ///// <param name="export">Export or not</param>
        //public static T AutomaticallyExportMethods<T>(this T conf, bool export = true) where T : IExportConfiguration<IAutoexportSwitchAttribute>
        //{
        //    conf.AttributePrototype.AutoExportMethods = export;
        //    return conf;
        //}

        ///// <summary>
        ///// Makes generator automatically look up for type properties and export them. 
        ///// Please note that this configuration setting will also export static and private members with corresponding modifiers.
        ///// </summary>
        ///// <param name="conf">Configuration</param>
        ///// <param name="export">Export or not</param>
        //public static T AutomaticallyExportProperties<T>(this T conf, bool export = true) where T : IExportConfiguration<IAutoexportSwitchAttribute>
        //{
        //    conf.AttributePrototype.AutoExportProperties = export;
        //    return conf;
        //}

        ///// <summary>
        ///// Makes generator automatically look up for type fields and export them as TS fields. 
        ///// Please note that this configuration setting will also export static and private members with corresponding modifiers.
        ///// </summary>
        ///// <param name="conf">Configuration</param>
        ///// <param name="export">Export or not</param>
        //public static T AutomaticallyExportFields<T>(this T conf, bool export = false) where T : IExportConfiguration<TsClassAttribute>
        //{
        //    conf.AttributePrototype.AutoExportFields = export;
        //    return conf;
        //}

        ///// <summary>
        ///// Makes generator automatically look up for type constructors and export them as empty constructors. 
        ///// </summary>
        ///// <param name="conf">Configuration</param>
        ///// <param name="export">Export or not</param>
        //public static T AutomaticallyExportConstructors<T>(this T conf, bool export = false) where T : IExportConfiguration<TsClassAttribute>
        //{
        //    conf.AttributePrototype.AutoExportConstructors = export;
        //    return conf;
        //}

        ///// <summary>
        ///// Sets default code generator for each method among class is being exported
        ///// </summary>
        ///// <param name="conf">Configuration</param>
        //public static IExportConfiguration<TsClassAttribute> DefaultMethodCodeGenerator<T>(this IExportConfiguration<TsClassAttribute> conf) 
        //    where T : ITsCodeGenerator<MethodInfo>
        //{
        //    conf.AttributePrototype.DefaultMethodCodeGenerator = typeof(T);
        //    return conf;
        //}

        #endregion

        #region Names and namespaces

        /// <summary>
        ///     Overrides name of specified member
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="name">Custom name to be used</param>
        public static T OverrideName<T>(this T conf, string name) where T : IAttributed<INameOverrideAttribute>
        {
            conf.AttributePrototype.Name = name;
            return conf;
        }

        /// <summary>
        ///     Forces member name to be camelCase
        /// </summary>
        /// <param name="conf">Configuration</param>
        public static T CamelCase<T>(this T conf) where T : IAttributed<ICamelCaseableAttribute>
        {
            conf.AttributePrototype.ShouldBeCamelCased = true;
            return conf;
        }

        /// <summary>
        ///     Forces member name to be PascalCase
        /// </summary>
        /// <param name="conf">Configuration</param>
        public static T PascalCase<T>(this T conf) where T : IAttributed<IPascalCasableAttribute>
        {
            conf.AttributePrototype.ShouldBePascalCased = true;
            return conf;
        }

        /// <summary>
        ///     Configures exporter dont to export member to corresponding namespace
        /// </summary>
        public static T DontIncludeToNamespace<T>(this T conf, bool include = false)
            where T : IAttributed<TsDeclarationAttributeBase>
        {
            conf.AttributePrototype.IncludeNamespace = include;
            return conf;
        }

        /// <summary>
        ///     Configures exporter to export type to specified namespace
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="nameSpace">Namespace name</param>
        public static T OverrideNamespace<T>(this T conf, string nameSpace)
            where T : IAttributed<TsDeclarationAttributeBase>
        {
            conf.AttributePrototype.Namespace = nameSpace;
            return conf;
        }

        #endregion

        #region Types and strong types

        /// <summary>
        ///     Overrides member type name on export with textual string.
        ///     Beware of using this setting because specified type may not present in your TypeScript code and
        ///     this will lead to TypeScript compilation errors
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="typeName">TS-friendly type name</param>
        /// <returns></returns>
        public static T Type<T>(this T conf, string typeName) where T : IAttributed<TsTypedAttributeBase>
        {
            conf.AttributePrototype.Type = typeName;
            return conf;
        }

        /// <summary>
        ///     Overrides member type on export with strong type.
        ///     Feel free to use delegates here. It is very comfortable instead of regular TS functions syntax.
        /// </summary>
        /// <param name="conf">Configurator</param>
        public static IAttributed<TsTypedAttributeBase> Type<T>(
            this IAttributed<TsTypedAttributeBase> conf)
        {
            conf.AttributePrototype.StrongType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Overrides member type on export with strong type.
        ///     Feel free to use delegates here. It is very comfortable instead of regular TS functions syntax.
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="type">Type to override with</param>
        /// <returns></returns>
        public static T Type<T>(this T conf, Type type) where T : IAttributed<TsTypedAttributeBase>
        {
            conf.AttributePrototype.StrongType = type;
            return conf;
        }

        #region The same for methods

        /// <summary>
        ///     Overrides member type name on export with textual string.
        ///     Beware of using this setting because specified type may not present in your TypeScript code and
        ///     this will lead to TypeScript compilation errors.
        ///     Actually this method does the same as .Type call. Just for your convinence
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="typeName">TS-friendly type name</param>
        /// <returns></returns>
        public static T Returns<T>(this T conf, string typeName) where T : IAttributed<TsFunctionAttribute>
        {
            conf.AttributePrototype.Type = typeName;
            return conf;
        }

        /// <summary>
        ///     Overrides member type on export with strong type.
        ///     Feel free to use delegates here. It is very comfortable instead of regular TS functions syntax.
        ///     Actually this method does the same as .Type call. Just for your convinence
        /// </summary>
        /// <param name="conf">Configurator</param>
        public static IAttributed<TsTypedAttributeBase> Returns<T>(
            this IAttributed<TsFunctionAttribute> conf)
        {
            conf.AttributePrototype.StrongType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Overrides member type on export with strong type.
        ///     Feel free to use delegates here. It is very comfortable instead of regular TS functions syntax.
        ///     Actually this method does the same as .Type call. Just for your convinence
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="type">Type to override with</param>
        /// <returns></returns>
        public static T Returns<T>(this T conf, Type type) where T : IAttributed<TsFunctionAttribute>
        {
            conf.AttributePrototype.StrongType = type;
            return conf;
        }

        #endregion

        #endregion

        #region Various and specific

        /// <summary>
        ///     Forces property to be a nullable.
        ///     When set to true then property will be generated as [property]? : [type] with
        ///     forcibly added question mark denoting nullable field.
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="force">Force nullable or not</param>
        public static T ForceNullable<T>(this T conf, bool? force = true)
            where T : IAttributed<TsPropertyAttribute>
        {
            conf.AttributePrototype.NilForceNullable = force;
            return conf;
        }

        /// <summary>
        ///     Sets parameter default value.
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="value">Default value for parameter</param>
        public static T DefaultValue<T>(this T conf, object value) where T : IAttributed<TsParameterAttribute>
        {
            conf.AttributePrototype.DefaultValue = value;
            return conf;
        }

        /// <summary>
        ///     Forces exporter to add I letter as interface prefix.
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="auto">Add I automatically or not</param>
        public static T AutoI<T>(this T conf, bool auto = true) where T : IAttributed<TsInterfaceAttribute>
        {
            conf.AttributePrototype.AutoI = auto;
            return conf;
        }



        /// <summary>
        ///     Tries to find documentation .xml file for specified assembly and take it in account when generating documentaion
        /// </summary>
        /// <param name="conf">Table configurator</param>
        /// <param name="assmbly">Assembly which documentation should be included</param>
        /// <param name="documentationFileName">Override XMLDOC file name if differs (please include .xml extension)</param>
        /// <returns>Fluent</returns>
        public static ConfigurationBuilder TryLookupDocumentationForAssembly(this ConfigurationBuilder conf,
            Assembly assmbly, string documentationFileName = null)
        {
            if (!string.IsNullOrEmpty(documentationFileName)
                && Path.IsPathRooted(documentationFileName))
            {
                conf.AdditionalDocumentationPathes.Add(documentationFileName);
                return conf;
            }

            var assemblyDir = Path.GetDirectoryName(assmbly.Location);
            var file = string.IsNullOrEmpty(documentationFileName)
                ? Path.GetFileNameWithoutExtension(assmbly.Location) + ".xml"
                : documentationFileName;
            var filePath = Path.Combine(assemblyDir, file);
            if (File.Exists(filePath))
            {
                conf.AdditionalDocumentationPathes.Add(filePath);
            }
            return conf;
        }

        /// <summary>
        /// Sets order this membter will be written to output file in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf">Configurator</param>
        /// <param name="order">Order of member</param>
        /// <returns>Fluent</returns>
        public static T Order<T>(this T conf, double order) where T : IOrderableMember
        {
            conf.MemberOrder = order;
            return conf;
        }

        /// <summary>
        /// Defines global type substitution. Substituted type will be strictly replaced with substitution during export
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="substitute">Type to substitute</param>
        /// <param name="substitution">Substitution for type</param>
        /// <returns>Fluent</returns>
        public static ConfigurationBuilder Substitute(this ConfigurationBuilder builder, Type substitute,
            RtTypeName substitution)
        {
            builder.GlobalSubstitutions[substitute] = substitution;
            return builder;
        }


        /// <summary>
        /// Defines local type substitution that will work only when exporting current class. 
        /// Substituted type will be strictly replaced with substitution during export but this option will take effect only when 
        /// exporting currently configurable type
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="substitute">Type to substitute</param>
        /// <param name="substitution">Substitution for type</param>
        /// <returns>Fluent</returns>
        public static T Substitute<T>(this T builder, Type substitute, RtTypeName substitution)
            where T : ITypeConfigurationBuilder
        {
            builder.Substitutions[substitute] = substitution;
            return builder;
        }

        /// <summary>
        /// Sets function body (works in case of class export) that will be converted to RtRaw and inserted as code block
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="functionCode">Function code</param>
        /// <returns></returns>
        public static T Implement<T>(this T builder, string functionCode)
            where T : IAttributed<TsFunctionAttribute>
        {
            builder.AttributePrototype.Implementation = functionCode;
            return builder;
        }

        #endregion
    }
}