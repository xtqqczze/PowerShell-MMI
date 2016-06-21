﻿/*============================================================================
* Copyright (C) Microsoft Corporation, All rights reserved.
*=============================================================================
*/

namespace Microsoft.Management.Infrastructure.UnitTests
{
    using Microsoft.Management.Infrastructure;
    using Microsoft.Management.Infrastructure.Native;
    using Microsoft.Management.Infrastructure.Serialization;
    using MMI.Tests;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Xunit;

    public class CimMofDeserializerTest : IDisposable
    {
        # region pre-test
        CimMofDeserializer deserializer;

        public CimMofDeserializerTest()
        {
            this.deserializer = CimMofDeserializer.Create();
        }

        public void Dispose()
        {
            if (this.deserializer != null)
            {
                this.deserializer.Dispose();
            }
        }
        #endregion pre-test

        #region Deserialization tests
        [Fact]
        public void Deserialization_CimClass_Basic0()
        {
            string classmof = "class A{string p;}; class B:A{uint8 p1;};";
            uint offset = 0;
            byte[] buffer = Helpers.GetBytesFromString(classmof);

            IEnumerable<CimClass> classes = this.deserializer.DeserializeClasses(buffer, ref offset);
            MMI.Tests.Assert.NotNull(classes, "Class got deserialized");
            MMI.Tests.Assert.Equal((uint)buffer.Length, offset, "Offset got increased");
            MMI.Tests.Assert.Equal(2, classes.Count(), "class count should be 2");
            IEnumerator<CimClass> ce = classes.GetEnumerator();
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                MMI.Tests.Assert.Equal(1, ce.Current.CimClassProperties.Count, "A class should have 1 property");
                CimPropertyDeclaration p = ce.Current.CimClassProperties["p"];
                MMI.Tests.Assert.NotNull(p, "A class property p should not be null");
                MMI.Tests.Assert.Equal("p", p.Name, "property name should be p");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
            }
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("B", ce.Current.CimSystemProperties.ClassName, "first class should be 'B'");
                MMI.Tests.Assert.Equal(2, ce.Current.CimClassProperties.Count, "B class should have 2 properties");
                CimPropertyDeclaration p1 = ce.Current.CimClassProperties["p1"];
                MMI.Tests.Assert.NotNull(p1, "B class property p1 should not be null");
                MMI.Tests.Assert.Equal("p1", p1.Name, "property name should be p");
                MMI.Tests.Assert.Equal(CimType.UInt8, p1.CimType, "property should be Uint8 type");
                MMI.Tests.Assert.Equal("A", ce.Current.CimSuperClass.CimSystemProperties.ClassName, "B should have parent class A");
            }
            MMI.Tests.Assert.False(ce.MoveNext(), "movenext should be false");
        }

        [Fact]
        public void Deserialization_Instance_Basic()
        {
            string instancemof = "class A{string p;}; instance of A{p=\"a\";};instance of A{p=\"b\";};instance of A{p=\"c\";};instance of A{p=\"d\";};";

            uint offset = 0;
            byte[] buffer = Helpers.GetBytesFromString(instancemof);
            IEnumerable<CimInstance> instances = this.deserializer.DeserializeInstances(buffer, ref offset);
            MMI.Tests.Assert.NotNull(instances, "Instance got deserialized");
            MMI.Tests.Assert.Equal((uint)buffer.Length, offset, "Offset got increased");
            MMI.Tests.Assert.Equal(4, instances.Count(), "instance count should be 4");
            IEnumerator<CimInstance> ce = instances.GetEnumerator();
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                MMI.Tests.Assert.NotNull(p, "property p should not be null");
                MMI.Tests.Assert.Equal("p", p.Name, "property name is not p");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                MMI.Tests.Assert.Equal("a", p.Value.ToString(), "property value should be a");
            }
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                MMI.Tests.Assert.NotNull(p, "property p should not be null");
                MMI.Tests.Assert.Equal("p", p.Name, "property name is p");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                MMI.Tests.Assert.Equal("b", p.Value.ToString(), "property value should be b");
            }
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance should have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                MMI.Tests.Assert.NotNull(p, "property p should not be null");
                MMI.Tests.Assert.Equal("p", p.Name, "property name should be p");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                MMI.Tests.Assert.Equal("c", p.Value.ToString(), "property value should be  c");
            }
            MMI.Tests.Assert.True(ce.MoveNext(), "movenext should be true");
            {
                MMI.Tests.Assert.Equal("A", ce.Current.CimSystemProperties.ClassName, "first class should be 'A'");
                MMI.Tests.Assert.Equal(1, ce.Current.CimInstanceProperties.Count, "instance sould have 1 property");
                CimProperty p = ce.Current.CimInstanceProperties["p"];
                MMI.Tests.Assert.NotNull(p, "property p should not be null");
                MMI.Tests.Assert.Equal("p", p.Name, "property name is p");
                MMI.Tests.Assert.Equal(CimType.String, p.CimType, "property should be String type");
                MMI.Tests.Assert.Equal("d", p.Value.ToString(), "property value should be d");
            }
            MMI.Tests.Assert.False(ce.MoveNext(), "move next should be false.");

        }

        [Fact]
        public void Deserialization_CimClass_NullBuffer()
        {
            MMI.Tests.Assert.Throws<ArgumentNullException>(() =>
            {
                uint offset = 0;
                byte[] buffer = null;
                return this.deserializer.DeserializeClasses(buffer, ref offset);

            });
        }

        [Fact]
        public void Deserialization_CimClass_ToolSmallBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                uint offset = 0;
                byte[] buffer = new byte[1];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_CimClass_JustLittleLargeBuffer()
        {
            MMI.Tests.Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                byte[] buffer = new byte[82];
                uint offset = (uint)buffer.Length;
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }

        // This test case maybe is invalid, will confirm with John and Ben, what is max size of Mof file we support
        [Fact]
        public void Deserialization_CimClass_ToolLargeBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                const int size = 50 * 1024 * 1024 + 1;
                uint offset = 0;
                byte[] buffer = new byte[size];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_CimClass_GarbageBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                const int size = 50 * 1024 * 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_CimClasse_InvalidMofBuffer()
        {
            MMI.Tests.Assert.Throws<ArgumentNullException>(() =>
            {
                const int size = 50 * 1024 * 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                byte[] b2 = Helpers.GetBytesFromString("abcd");
                b2.CopyTo(buffer, 0);
                return this.deserializer.DeserializeClasses(buffer, ref offset);
            });
        }

        [TDDFact]
        public void Deserialization_CimClasse_NotNullOnClassNeededCallback()
        {
            MMI.Tests.Assert.Throws<NotImplementedException>(() =>
            {
                uint offset = 0;
                byte[] buffer = new byte[82];
                CimMofDeserializer.OnClassNeeded onClassNeede = this.GetClass;
                onClassNeede("Servername", @"root\TestNamespace", "MyClassName");
                return this.deserializer.DeserializeClasses(buffer, ref offset, null, onClassNeede, null);
            });
        }

        [Fact]
        public void Deserialization_CimClasse_NotNullGetIncludedFileContent()
        {
            MMI.Tests.Assert.Throws<NotImplementedException>(() =>
            {
                uint offset = 0;
                byte[] buffer = new byte[82];
                CimMofDeserializer.GetIncludedFileContent getIncludedFileContent = this.GetFileContent;
                getIncludedFileContent("I am a faked file");
                return this.deserializer.DeserializeClasses(buffer, ref offset, null, null, getIncludedFileContent);
            });
        }

        [Fact]
        public void Deserialization_Instance_NullBuffer()
        {
            MMI.Tests.Assert.Throws<ArgumentNullException>(() =>
            {
                uint offset = 0;
                byte[] buffer = null;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_Instance_ToolSmallBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                uint offset = 0;
                byte[] buffer = new byte[1];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_Instance_JustLittleLargeBuffer()
        {
            MMI.Tests.Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                byte[] buffer = new byte[82];
                uint offset = (uint)buffer.Length;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        // This test case maybe is invalid, will confirm with John and Ben, what is max size of Mof file we support
        [Fact]
        public void Deserialization_Instance_ToolLargeBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                const int size = 50 * 1024 * 1024 + 1;
                uint offset = 0;
                byte[] buffer = new byte[size];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        [Fact]
        public void Deserialization_Instance_GarbageBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                const int size = 50 * 1024 * 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                buffer[0] = byte.MinValue;
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        [TDDFact]
        public void Deserialization_Instance_InvalidMofBuffer()
        {
            MMI.Tests.Assert.Throws<CimException>(() =>
            {
                const int size = 50 * 1024 * 1024;
                uint offset = 0;
                byte[] buffer = new byte[size];
                byte[] b2 = Helpers.GetBytesFromString("abcd");
                b2.CopyTo(buffer, 0);
                return this.deserializer.DeserializeInstances(buffer, ref offset);
            });
        }

        [TDDFact]
        public void Deserialization_Instance_NotNullOnClassNeededCallback()
        {
            string instancemof = "class A{string p;}; instance of A{p=\"a\";};instance of A{p=\"b\";};instance of A{p=\"c\";};instance of A{p=\"d\";};";
            uint offset = 0;
            byte[] buffer = Helpers.GetBytesFromString(instancemof);
            CimMofDeserializer.OnClassNeeded onClassNeede = this.GetClass;
            onClassNeede("Servername", @"root\TestNamespace", "MyClassName");
            IEnumerable<CimInstance> instances = this.deserializer.DeserializeInstances(buffer, ref offset, null, onClassNeede, null);
            MMI.Tests.Assert.NotNull(instances, "Instance got deserialized");
        }

        [Fact]
        public void Deserialization_Instance_NotNullGetIncludedFileContent()
        {
            MMI.Tests.Assert.Throws<NotImplementedException>(() =>
            {
                uint offset = 0;
                byte[] buffer = new byte[82];
                CimMofDeserializer.GetIncludedFileContent getIncludedFileContent = this.GetFileContent;
                getIncludedFileContent("I am a faked file");
                return this.deserializer.DeserializeInstances(buffer, ref offset, null, null, getIncludedFileContent);
            });
        }

        //TODO: Fake different kinds of Mof file to deserialize
        #endregion Deserialization tests

        #region Fake
        private CimClass GetClass(string serverName, string namespaceName, string className)
        {
            CimInstance cimInstance = new CimInstance(className, namespaceName);
            return cimInstance.CimClass;
        }

        private byte[] GetFileContent(string fileName)
        {
            return Helpers.GetBytesFromString(fileName);
        }
        #endregion Fake
    }
}