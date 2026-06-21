// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using Fuzzy;
using Inspector;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using ConfigurationSection = System.Fabric.Description.ConfigurationSection;

namespace Microsoft.ServiceFabric.AspNetCore.Configuration;

public abstract class ServiceFabricConfigurationOptionsTest
{
    readonly ServiceFabricConfigurationOptions sut;

    // Constructor parameters
    readonly string packageName = fuzzy.String();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ServiceFabricConfigurationOptionsTest() =>
        sut = new ServiceFabricConfigurationOptions(packageName);

    public sealed class ConfigAction : ServiceFabricConfigurationOptionsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            Action<ConfigurationPackage, IDictionary<string, string>> expected = (_, _) => { };
            sut.ConfigAction = expected;
            Assert.Same(expected, sut.ConfigAction);
        }
    }

    public sealed class Constructor : ServiceFabricConfigurationOptionsTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Equal(packageName, sut.PackageName);
            Assert.True(sut.IncludePackageName);
            Assert.False(sut.DecryptValue);
            Action<ConfigurationPackage, IDictionary<string, string>> expectedConfigAction = sut.DefaultConfigAction;
            Func<ConfigurationSection, ConfigurationProperty, string> expectedExtractKeyFunc = sut.DefaultExtractKeyFunc;
            Func<ConfigurationSection, ConfigurationProperty, string> expectedExtractValueFunc = sut.DefaultExtractValueFunc;
            Assert.Equal(expectedConfigAction, sut.ConfigAction);
            Assert.Equal(expectedExtractKeyFunc, sut.ExtractKeyFunc);
            Assert.Equal(expectedExtractValueFunc, sut.ExtractValueFunc);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Incorrect packageName argument exception ParamName.
        public void ThrowsArgumentNullExceptionWhenPackageNameIsNull()
        {
            // The constructor throws `new ArgumentNullException(packageName)`, passing the (null) value as the
            // paramName argument instead of `nameof(packageName)`. As a result, the resulting exception's ParamName is
            // null and gives callers no indication which argument was invalid. This test asserts the correct paramName
            // and will fail until the SUT is fixed. Fixing the SUT is out of scope for the current change.
            var exception = Assert.Throws<ArgumentNullException>(() => new ServiceFabricConfigurationOptions(null));
            Assert.Equal(nameof(packageName), exception.ParamName);
        }
    }

    public sealed class DecryptValue : ServiceFabricConfigurationOptionsTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsSetToGivenValue(bool expected)
        {
            sut.DecryptValue = expected;
            Assert.Equal(expected, sut.DecryptValue);
        }
    }

    public sealed class DefaultConfigAction : ServiceFabricConfigurationOptionsTest
    {
        // Method parameters
        readonly ConfigurationPackage config;
        readonly IDictionary<string, string> data = new Dictionary<string, string>();

        readonly string section1 = fuzzy.String().LettersOrDigits();
        readonly string param1a = fuzzy.String().LettersOrDigits();
        readonly string param1b;
        readonly string section2;
        readonly string param2 = fuzzy.String().LettersOrDigits();

        // MockConfigurationPackage.CreateDefaultPackage dereferences each value via item.Value.Contains(...),
        // so it must be non-null. The contents are unused because ExtractValueFunc is mocked.
        const string unused = "";

        public DefaultConfigAction()
        {
            param1b = param1a + fuzzy.String().LettersOrDigits();
            section2 = section1 + fuzzy.String().LettersOrDigits();

            IConfigurationRoot root = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { $"{section1}:{param1a}", unused },
                { $"{section1}:{param1b}", unused },
                { $"{section2}:{param2}", unused },
            }).Build();
            config = MockConfigurationPackage.CreateDefaultPackage(root, packageName);
        }

        [Fact]
        public void ExecutesExtractKeyFuncAndExtractValueFuncToPopulateData()
        {
            // Arrange
            ConfigurationSection sec1 = config.Settings.Sections[section1];
            ConfigurationSection sec2 = config.Settings.Sections[section2];
            ConfigurationProperty p1a = sec1.Parameters[param1a];
            ConfigurationProperty p1b = sec1.Parameters[param1b];
            ConfigurationProperty p2 = sec2.Parameters[param2];

            string key1a = fuzzy.String();
            string key1b = key1a + fuzzy.String();
            string key2 = key1b + fuzzy.String();
            string val1a = fuzzy.String();
            string val1b = fuzzy.String();
            string val2 = fuzzy.String();

            Mock<Func<ConfigurationSection, ConfigurationProperty, string>> extractKey = new();
            _ = extractKey.Setup(_ => _(sec1, p1a)).Returns(key1a);
            _ = extractKey.Setup(_ => _(sec1, p1b)).Returns(key1b);
            _ = extractKey.Setup(_ => _(sec2, p2)).Returns(key2);
            sut.ExtractKeyFunc = extractKey.Object;

            Mock<Func<ConfigurationSection, ConfigurationProperty, string>> extractValue = new();
            _ = extractValue.Setup(_ => _(sec1, p1a)).Returns(val1a);
            _ = extractValue.Setup(_ => _(sec1, p1b)).Returns(val1b);
            _ = extractValue.Setup(_ => _(sec2, p2)).Returns(val2);
            sut.ExtractValueFunc = extractValue.Object;

            // Act
            sut.DefaultConfigAction(config, data);

            // Assert
            Dictionary<string, string> expected = new()
            {
                [key1a] = val1a,
                [key1b] = val1b,
                [key2] = val2,
            };
            Assert.Equal(expected, data);

            extractKey.Verify(_ => _(It.IsAny<ConfigurationSection>(), It.IsAny<ConfigurationProperty>()), Times.Exactly(3));
            extractValue.Verify(_ => _(It.IsAny<ConfigurationSection>(), It.IsAny<ConfigurationProperty>()), Times.Exactly(3));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing config argument validation.
        public void ThrowsArgumentNullExceptionWhenConfigIsNull()
        {
            // DefaultConfigAction dereferences its `config` parameter without a null check, so passing null produces a
            // NullReferenceException instead of the ArgumentNullException expected for a public-facing delegate. This
            // test asserts the correct behavior and will fail until the SUT validates the argument. Fixing the SUT is
            // out of scope for the current change.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.DefaultConfigAction(null, data));
            Assert.Equal(nameof(config), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing data argument validation.
        public void ThrowsArgumentNullExceptionWhenDataIsNull()
        {
            // DefaultConfigAction dereferences its `data` parameter without a null check, so passing null produces a
            // NullReferenceException instead of the ArgumentNullException expected for a public-facing delegate. This
            // test asserts the correct behavior and will fail until the SUT validates the argument. Fixing the SUT is
            // out of scope for the current change.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.DefaultConfigAction(config, null));
            Assert.Equal(nameof(data), exception.ParamName);
        }
    }

    public sealed class DefaultExtractKeyFunc : ServiceFabricConfigurationOptionsTest
    {
        readonly ConfigurationSection section = Section();
        readonly ConfigurationProperty property = Property();

        [Fact]
        public void IncludesPackageNameWhenIncludePackageNameIsTrue()
        {
            string actual = sut.DefaultExtractKeyFunc(section, property);

            string delimiter = ConfigurationPath.KeyDelimiter;
            Assert.Equal($"{packageName}{delimiter}{section.Name}{delimiter}{property.Name}", actual);
        }

        [Fact]
        public void ExcludesPackageNameWhenIncludePackageNameIsFalse()
        {
            sut.IncludePackageName = false;

            string actual = sut.DefaultExtractKeyFunc(section, property);

            string delimiter = ConfigurationPath.KeyDelimiter;
            Assert.Equal($"{section.Name}{delimiter}{property.Name}", actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing section argument validation.
        public void ThrowsArgumentNullExceptionWhenSectionIsNull()
        {
            // DefaultExtractKeyFunc dereferences its `section` parameter without a null check, so passing null produces
            // a NullReferenceException instead of the ArgumentNullException expected for a public-facing delegate. This
            // test asserts the correct behavior and will fail until the SUT validates the argument. Fixing the SUT is
            // out of scope for the current change.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.DefaultExtractKeyFunc(null, property));
            Assert.Equal(nameof(section), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing property argument validation.
        public void ThrowsArgumentNullExceptionWhenPropertyIsNull()
        {
            // DefaultExtractKeyFunc dereferences its `property` parameter without a null check, so passing null
            // produces a NullReferenceException instead of the ArgumentNullException expected for a public-facing
            // delegate. This test asserts the correct behavior and will fail until the SUT validates the argument.
            // Fixing the SUT is out of scope for the current change.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.DefaultExtractKeyFunc(section, null));
            Assert.Equal(nameof(property), exception.ParamName);
        }
    }

    public sealed class DefaultExtractValueFunc : ServiceFabricConfigurationOptionsTest
    {
        readonly ConfigurationSection section = Section();
        readonly ConfigurationProperty property = Property();

        [Fact]
        public void ReturnsPropertyValueWhenPropertyIsNotEncrypted()
        {
            string actual = sut.DefaultExtractValueFunc(section, property);
            Assert.Same(property.Value, actual);
        }

        [Fact]
        public void ReturnsPropertyValueWhenPropertyIsEncryptedAndDecryptValueIsFalse()
        {
            ConfigurationProperty encryptedProperty = Property(isEncrypted: true);
            sut.DecryptValue = false;
            string actual = sut.DefaultExtractValueFunc(section, encryptedProperty);
            Assert.Same(encryptedProperty.Value, actual);
        }

        [Fact]
        public void ReturnsPropertyValueWhenPropertyIsNotEncryptedAndDecryptValueIsTrue()
        {
            sut.DecryptValue = true;
            string actual = sut.DefaultExtractValueFunc(section, property);
            Assert.Same(property.Value, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT testability limitation. Decryption branch cannot be substituted.
        public void ReturnsDecryptedValueWhenPropertyIsEncryptedAndDecryptValueIsTrue() =>
            // The decryption branch of DefaultExtractValueFunc calls the non-virtual instance method
            // ConfigurationProperty.DecryptValue(), which cannot be substituted in unit tests. Exercising this branch
            // is not possible without refactoring the SUT to accept an injectable decryptor. Fixing the underlying
            // testability limitation is out of scope for the current change.
            throw new NotImplementedException();

        [Fact(Explicit = true)] // TODO: SUT bug. Missing property argument validation.
        public void ThrowsArgumentNullExceptionWhenPropertyIsNull()
        {
            // DefaultExtractValueFunc dereferences its `property` parameter without a null check, so passing null
            // produces a NullReferenceException instead of the ArgumentNullException expected for a public-facing
            // delegate. This test asserts the correct behavior and will fail until the SUT validates the argument.
            // Fixing the SUT is out of scope for the current change.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.DefaultExtractValueFunc(section, null));
            Assert.Equal(nameof(property), exception.ParamName);
        }
    }

    public sealed class ExtractKeyFunc : ServiceFabricConfigurationOptionsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            Func<ConfigurationSection, ConfigurationProperty, string> expected = (_, _) => null;
            sut.ExtractKeyFunc = expected;
            Assert.Same(expected, sut.ExtractKeyFunc);
        }
    }

    public sealed class ExtractValueFunc : ServiceFabricConfigurationOptionsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            Func<ConfigurationSection, ConfigurationProperty, string> expected = (_, _) => null;
            sut.ExtractValueFunc = expected;
            Assert.Same(expected, sut.ExtractValueFunc);
        }
    }

    public sealed class IncludePackageName : ServiceFabricConfigurationOptionsTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsSetToGivenValue(bool expected)
        {
            sut.IncludePackageName = expected;
            Assert.Equal(expected, sut.IncludePackageName);
        }
    }

    static ConfigurationSection Section()
    {
        var section = Type<ConfigurationSection>.New();
        section.Property<string>().Set(fuzzy.String());
        return section;
    }

    static ConfigurationProperty Property(bool isEncrypted = false)
    {
        var property = Type<ConfigurationProperty>.New();
        property.Property<string>(nameof(ConfigurationProperty.Name)).Set(fuzzy.String());
        property.Property<string>(nameof(ConfigurationProperty.Value)).Set(fuzzy.String());
        property.Property<bool>(nameof(ConfigurationProperty.IsEncrypted)).Set(isEncrypted);
        return property;
    }
}
