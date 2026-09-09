// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.Generic;
using System.Fabric.Description;
using System.Fabric.Management.ServiceModel;
using System.Linq;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport;

[WindowsOnly("Can't load libFabricCommon.so on Linux.")]
public abstract class FabricServiceConfigSectionTest: FabricServiceConfigAccessor
{
    readonly FabricServiceConfigSection sut;

    // Constructor parameters
    readonly string sectionName = fuzzy.String().LettersOrDigits();
    readonly Mock<Action> onInitialize = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    FabricServiceConfigSectionTest() => sut = new FabricServiceConfigSection(sectionName, onInitialize.Object);

    public sealed class Constructor : FabricServiceConfigSectionTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate sectionName.
        public void ThrowsArgumentNullExceptionWhenSectionNameIsNull()
        {
            // The constructor stores sectionName unchecked and Initialize calls this.sectionName.Trim()
            // while iterating exe settings sections, throwing NullReferenceException.
            var exception = Assert.Throws<ArgumentNullException>(
                () => new FabricServiceConfigSection(sectionName: null, onInitialize.Object));
            Assert.Equal(nameof(sectionName), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor does not validate onInitialize.
        public void ThrowsArgumentNullExceptionWhenOnInitializeIsNull()
        {
            // The constructor stores onInitialize unchecked and Initialize invokes it once a matching
            // section is found, throwing NullReferenceException.
            var exception = Assert.Throws<ArgumentNullException>(
                () => new FabricServiceConfigSection(sectionName, onInitialize: null));
            Assert.Equal(nameof(onInitialize), exception.ParamName);
        }
    }

    public sealed class GetSetting : FabricServiceConfigSectionTest
    {
        readonly string settingName = fuzzy.String().LettersOrDigits();

        [Fact]
        public void ReturnsParsedInt32ValueFromConfigurationSection()
        {
            int expected = fuzzy.Int32();
            InitializeWithConfigSection(MakeConfigParameter(settingName, expected.ToString()));

            int actual = sut.GetSetting(settingName, defaultValue: expected + fuzzy.SByte().Between(1, 5));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsParsedStringValueFromConfigurationSection()
        {
            string expected = fuzzy.String();
            InitializeWithConfigSection(MakeConfigParameter(settingName, expected));

            string actual = sut.GetSetting<string>(settingName, defaultValue: null);
            Assert.Same(expected, actual);
        }

        [Theory, InlineData(SampleEnum.First, SampleEnum.Second), InlineData(SampleEnum.Second, SampleEnum.First)]
        public void ReturnsParsedEnumValueFromConfigurationSection(SampleEnum expected, SampleEnum defaultValue)
        {
            InitializeWithConfigSection(MakeConfigParameter(settingName, expected.ToString()));

            SampleEnum actual = sut.GetSetting(settingName, defaultValue);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsParsedInt32ValueFromExeSection()
        {
            int expected = fuzzy.Int32();
            InitializeWithExeSection(MakeExeParameter(settingName, expected.ToString()));

            int actual = sut.GetSetting(settingName, defaultValue: expected + fuzzy.SByte().Between(1, 5));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsParsedValueFromExeSectionWhenNonMatchingParameterPrecedesIt()
        {
            // Exercises the loop-continuation branch over exeSection.Parameter: the first parameter does
            // not match and GetSetting must keep iterating to find the matching parameter.
            string expected = fuzzy.String();
            InitializeWithExeSection(
                MakeExeParameter(settingName + "_" + fuzzy.String().LettersOrDigits(), fuzzy.String()),
                MakeExeParameter(settingName, expected));

            string actual = sut.GetSetting<string>(settingName, defaultValue: null);
            Assert.Same(expected, actual);
        }

        [Fact]
        public void ReturnsDefaultValueWhenParameterIsAbsentFromConfigurationSection()
        {
            InitializeWithConfigSection(MakeConfigParameter(settingName + "_" + fuzzy.String().LettersOrDigits(), fuzzy.String()));
            int expected = fuzzy.Int32();

            int actual = sut.GetSetting(settingName, expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsDefaultValueWhenParameterIsAbsentFromExeSection()
        {
            InitializeWithExeSection(MakeExeParameter(settingName + "_" + fuzzy.String().LettersOrDigits(), fuzzy.String()));
            int expected = fuzzy.Int32();

            int actual = sut.GetSetting(settingName, expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PropagatesFormatExceptionWhenValueCannotBeConvertedToRequestedType()
        {
            // Documents that GetSetting does not catch Convert.ChangeType failures.
            InitializeWithConfigSection(MakeConfigParameter(settingName, "not-an-int"));

            _ = Assert.Throws<FormatException>(() => sut.GetSetting(settingName, defaultValue: fuzzy.Int32()));
        }

        [Fact]
        public void PropagatesArgumentExceptionWhenValueCannotBeParsedAsEnum()
        {
            // Documents that GetSetting does not catch Enum.Parse failures.
            InitializeWithConfigSection(MakeConfigParameter(settingName, "NotAMember"));

            _ = Assert.Throws<ArgumentException>(() => sut.GetSetting(settingName, defaultValue: fuzzy.Enum<SampleEnum>()));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSetting does not validate settingName.
        public void ThrowsArgumentNullExceptionWhenSettingNameIsNull()
        {
            // GetSetting calls settingName.Trim() without validating settingName, throwing NullReferenceException.
            InitializeWithConfigSection(MakeConfigParameter(fuzzy.String().LettersOrDigits(), fuzzy.String()));

            var exception = Assert.Throws<ArgumentNullException>(() => sut.GetSetting<string>(settingName: null, defaultValue: null));
            Assert.Equal(nameof(settingName), exception.ParamName);
        }

        [Fact]
        public void ReturnsParameterValueWhenSettingNameIsPaddedWithWhitespace()
        {
            string value = fuzzy.String();
            InitializeWithConfigSection(MakeConfigParameter(settingName, value));

            string actual = sut.GetSetting<string>($"  {settingName}  ", defaultValue: null);
            Assert.Same(value, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSetting does not validate settingName in exe-section branch.
        public void ThrowsArgumentNullExceptionWhenSettingNameIsNullAndExeSectionIsInitialized()
        {
            // In the exe-section branch, GetSetting calls settingName.Trim() without validating
            // settingName, throwing NullReferenceException.
            InitializeWithExeSection(MakeExeParameter(fuzzy.String().LettersOrDigits(), fuzzy.String()));

            var exception = Assert.Throws<ArgumentNullException>(() => sut.GetSetting<string>(settingName: null, defaultValue: null));
            Assert.Equal(nameof(settingName), exception.ParamName);
        }

        [Fact]
        public void ReturnsExeSectionParameterValueWhenSettingNameIsPaddedWithWhitespace()
        {
            string value = fuzzy.String();
            InitializeWithExeSection(MakeExeParameter(settingName, value));

            string actual = sut.GetSetting<string>($"  {settingName}  ", defaultValue: null);
            Assert.Same(value, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSetting does not check whether Initialize succeeded.
        public void ThrowsInvalidOperationExceptionWhenInitializeWasNotCalled() =>
            // GetSetting dereferences exeSection.Parameter without checking whether Initialize has
            // been called and succeeded, throwing NullReferenceException.
            _ = Assert.Throws<InvalidOperationException>(() => sut.GetSetting<string>(settingName, defaultValue: null));
    }

    public sealed class GetSettingsList : FabricServiceConfigSectionTest
    {
        readonly string settingName = fuzzy.String().LettersOrDigits();

        [Fact]
        public void ReturnsCommaSeparatedInt32ValuesFromConfigurationSection()
        {
            int[] expected = fuzzy.Array(fuzzy.Int32);
            InitializeWithConfigSection(MakeConfigParameter(settingName, string.Join(",", expected)));

            IList<int> actual = sut.GetSettingsList<int>(settingName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsCommaSeparatedInt32ValuesFromExeSection()
        {
            int[] expected = fuzzy.Array(fuzzy.Int32);
            InitializeWithExeSection(MakeExeParameter(settingName, string.Join(",", expected)));

            IList<int> actual = sut.GetSettingsList<int>(settingName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsCommaSeparatedValuesFromExeSectionWhenNonMatchingParameterPrecedesIt()
        {
            // Exercises the loop-continuation branch over exeSection.Parameter: the first parameter does
            // not match and GetSettingsList must keep iterating to find the matching parameter.
            int[] expected = fuzzy.Array(fuzzy.Int32);
            InitializeWithExeSection(
                MakeExeParameter(settingName + "_" + fuzzy.String().LettersOrDigits(), fuzzy.String()),
                MakeExeParameter(settingName, string.Join(",", expected)));

            IList<int> actual = sut.GetSettingsList<int>(settingName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsEmptyListWhenParameterIsAbsentFromConfigurationSection()
        {
            InitializeWithConfigSection(MakeConfigParameter(settingName + "_" + fuzzy.String().LettersOrDigits(), fuzzy.String()));

            IList<int> actual = sut.GetSettingsList<int>(settingName);
            Assert.Empty(actual);
        }

        [Fact]
        public void ReturnsEmptyListWhenParameterIsAbsentFromExeSection()
        {
            InitializeWithExeSection(MakeExeParameter(settingName + "_" + fuzzy.String().LettersOrDigits(), fuzzy.String()));

            IList<int> actual = sut.GetSettingsList<int>(settingName);
            Assert.Empty(actual);
        }

        [Fact]
        public void PropagatesFormatExceptionWhenElementCannotBeConvertedToRequestedType()
        {
            // Documents that GetSettingsList does not catch per-element Convert.ChangeType failures.
            InitializeWithConfigSection(MakeConfigParameter(settingName, "1,not-an-int,3"));

            _ = Assert.Throws<FormatException>(() => sut.GetSettingsList<int>(settingName));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSettingsList<T> does not support enums.
        public void ReturnsParsedEnumValuesFromConfigurationSection()
        {
            // CastParameterAsList<T> calls Convert.ChangeType which throws InvalidCastException for enum target
            // types, unlike GetSetting<T>.
            SampleEnum[] expected = fuzzy.Array(fuzzy.Enum<SampleEnum>);
            InitializeWithConfigSection(MakeConfigParameter(settingName, string.Join(",", expected)));

            IList<SampleEnum> actual = sut.GetSettingsList<SampleEnum>(settingName);
            Assert.Equal(expected, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSettingsList does not validate settingName.
        public void ThrowsArgumentNullExceptionWhenSettingNameIsNull()
        {
            // GetSettingsList calls settingName.Trim() without validating settingName, throwing NullReferenceException.
            InitializeWithConfigSection(MakeConfigParameter(fuzzy.String().LettersOrDigits(), fuzzy.String()));

            var exception = Assert.Throws<ArgumentNullException>(() => sut.GetSettingsList<string>(settingName: null));
            Assert.Equal(nameof(settingName), exception.ParamName);
        }

        [Fact]
        public void ReturnsParameterValuesWhenSettingNameIsPaddedWithWhitespace()
        {
            int[] expected = fuzzy.Array(fuzzy.Int32);
            InitializeWithConfigSection(MakeConfigParameter(settingName, string.Join(",", expected)));

            IList<int> actual = sut.GetSettingsList<int>($"  {settingName}  ");
            Assert.Equal(expected, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSettingsList does not validate settingName in exe-section branch.
        public void ThrowsArgumentNullExceptionWhenSettingNameIsNullAndExeSectionIsInitialized()
        {
            // In the exe-section branch, GetSettingsList calls settingName.Trim() without validating
            // settingName, throwing NullReferenceException.
            InitializeWithExeSection(MakeExeParameter(fuzzy.String().LettersOrDigits(), fuzzy.String()));

            var exception = Assert.Throws<ArgumentNullException>(() => sut.GetSettingsList<string>(settingName: null));
            Assert.Equal(nameof(settingName), exception.ParamName);
        }

        [Fact]
        public void ReturnsExeSectionParameterValuesWhenSettingNameIsPaddedWithWhitespace()
        {
            int[] expected = fuzzy.Array(fuzzy.Int32);
            InitializeWithExeSection(MakeExeParameter(settingName, string.Join(",", expected)));

            IList<int> actual = sut.GetSettingsList<int>($"  {settingName}  ");
            Assert.Equal(expected, actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSettingsList does not check whether Initialize succeeded.
        public void ThrowsInvalidOperationExceptionWhenInitializeWasNotCalled() =>
            // GetSettingsList dereferences exeSection.Parameter without checking whether Initialize
            // has been called and succeeded, throwing NullReferenceException.
            _ = Assert.Throws<InvalidOperationException>(() => sut.GetSettingsList<string>(settingName));
    }

    public sealed class GetSettingsMapFromPrefix : FabricServiceConfigSectionTest
    {
        readonly string settingPrefix = fuzzy.String().LettersOrDigits() + "_";

        [Fact]
        public void ReturnsMatchingParametersKeyedBySuffixFromConfigurationSection()
        {
            Dictionary<string, string> expected = fuzzy.Dictionary(() => fuzzy.String().LettersOrDigits(), fuzzy.String);
            // settingPrefix ends with '_' and LettersOrDigits cannot contain '_', so this name cannot start with settingPrefix.
            string nonMatchingName = settingPrefix.TrimEnd('_') + fuzzy.String().LettersOrDigits();
            InitializeWithConfigSection([
                MakeConfigParameter(nonMatchingName, fuzzy.String()),
                .. expected.Select(_ => MakeConfigParameter(settingPrefix + _.Key, _.Value))]);

            Dictionary<string, string> actual = sut.GetSettingsMapFromPrefix(settingPrefix);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsMatchingParametersKeyedBySuffixFromExeSection()
        {
            Dictionary<string, string> expected = fuzzy.Dictionary(() => fuzzy.String().LettersOrDigits(), fuzzy.String);
            // settingPrefix ends with '_' and LettersOrDigits cannot contain '_', so this name cannot start with settingPrefix.
            string nonMatchingName = settingPrefix.TrimEnd('_') + fuzzy.String().LettersOrDigits();
            InitializeWithExeSection([
                MakeExeParameter(nonMatchingName, fuzzy.String()),
                .. expected.Select(_ => MakeExeParameter(settingPrefix + _.Key, _.Value))]);

            Dictionary<string, string> actual = sut.GetSettingsMapFromPrefix(settingPrefix);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsEmptyMapWhenNoParameterMatchesPrefixInConfigurationSection()
        {
            // settingPrefix ends with '_' and LettersOrDigits cannot contain '_', so this name cannot start with settingPrefix.
            InitializeWithConfigSection(MakeConfigParameter(settingPrefix.TrimEnd('_') + fuzzy.String().LettersOrDigits(), fuzzy.String()));

            Dictionary<string, string> actual = sut.GetSettingsMapFromPrefix(settingPrefix);
            Assert.Empty(actual);
        }

        [Fact]
        public void ReturnsEmptyMapWhenNoParameterMatchesPrefixInExeSection()
        {
            // settingPrefix ends with '_' and LettersOrDigits cannot contain '_', so this name cannot start with settingPrefix.
            InitializeWithExeSection(MakeExeParameter(settingPrefix.TrimEnd('_') + fuzzy.String().LettersOrDigits(), fuzzy.String()));

            Dictionary<string, string> actual = sut.GetSettingsMapFromPrefix(settingPrefix);
            Assert.Empty(actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. An exact-equal name produces an empty-string key that no natural caller can look up.
        public void ReturnsEmptyMapWhenOnlyMatchingParameterNameEqualsPrefixExactly()
        {
            // The only product caller (FabricTransportSettings) treats the suffix as a certificate issuer name,
            // so the parameter should only match when its name has a non-empty suffix after the prefix.
            InitializeWithConfigSection(MakeConfigParameter(settingPrefix, fuzzy.String()));

            Dictionary<string, string> actual = sut.GetSettingsMapFromPrefix(settingPrefix);
            Assert.Empty(actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSettingsMapFromPrefix does not validate settingPrefix.
        public void ThrowsArgumentNullExceptionWhenSettingPrefixIsNull()
        {
            // GetSettingsMapFromPrefix does not validate settingPrefix and forwards it to
            // param.Name.StartsWith(settingPrefix), which throws ArgumentNullException with ParamName="value".
            InitializeWithConfigSection(MakeConfigParameter(fuzzy.String().LettersOrDigits(), fuzzy.String()));

            var exception = Assert.Throws<ArgumentNullException>(() => sut.GetSettingsMapFromPrefix(settingPrefix: null));
            Assert.Equal(nameof(settingPrefix), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSettingsMapFromPrefix does not trim settingPrefix, unlike GetSetting/GetSettingsList/Initialize.
        public void ReturnsMatchingParametersWhenSettingPrefixIsPaddedWithWhitespace()
        {
            // Mirrors the trimming behavior of GetSetting, GetSettingsList, and Initialize.
            string suffix = fuzzy.String().LettersOrDigits();
            string value = fuzzy.String();
            InitializeWithConfigSection(MakeConfigParameter(settingPrefix + suffix, value));

            Dictionary<string, string> actual = sut.GetSettingsMapFromPrefix($"  {settingPrefix}  ");

            KeyValuePair<string, string> entry = Assert.Single(actual);
            Assert.Equal(suffix, entry.Key);
            Assert.Same(value, entry.Value);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. An exact-equal name produces an empty-string key in the exe-section branch.
        public void ReturnsEmptyMapWhenOnlyMatchingExeSectionParameterNameEqualsPrefixExactly()
        {
            // The only product caller (FabricTransportSettings) treats the suffix as a certificate issuer name,
            // so the parameter should only match when its name has a non-empty suffix after the prefix.
            InitializeWithExeSection(MakeExeParameter(settingPrefix, fuzzy.String()));

            Dictionary<string, string> actual = sut.GetSettingsMapFromPrefix(settingPrefix);
            Assert.Empty(actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSettingsMapFromPrefix does not validate settingPrefix in exe-section branch.
        public void ThrowsArgumentNullExceptionWhenSettingPrefixIsNullAndExeSectionIsInitialized()
        {
            // In the exe-section branch, GetSettingsMapFromPrefix does not validate settingPrefix and
            // forwards it to param.Name.StartsWith(settingPrefix), which throws ArgumentNullException with ParamName="value".
            InitializeWithExeSection(MakeExeParameter(fuzzy.String().LettersOrDigits(), fuzzy.String()));

            var exception = Assert.Throws<ArgumentNullException>(() => sut.GetSettingsMapFromPrefix(settingPrefix: null));
            Assert.Equal(nameof(settingPrefix), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSettingsMapFromPrefix does not trim settingPrefix in the exe-section branch.
        public void ReturnsMatchingExeSectionParametersWhenSettingPrefixIsPaddedWithWhitespace()
        {
            // Mirrors the trimming behavior of GetSetting, GetSettingsList, and Initialize.
            string suffix = fuzzy.String().LettersOrDigits();
            string value = fuzzy.String();
            InitializeWithExeSection(MakeExeParameter(settingPrefix + suffix, value));

            Dictionary<string, string> actual = sut.GetSettingsMapFromPrefix($"  {settingPrefix}  ");

            KeyValuePair<string, string> entry = Assert.Single(actual);
            Assert.Equal(suffix, entry.Key);
            Assert.Same(value, entry.Value);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. GetSettingsMapFromPrefix does not check whether Initialize succeeded.
        public void ThrowsInvalidOperationExceptionWhenInitializeWasNotCalled() =>
            // GetSettingsMapFromPrefix dereferences exeSection.Parameter without checking whether
            // Initialize has been called and succeeded, throwing NullReferenceException.
            _ = Assert.Throws<InvalidOperationException>(() => sut.GetSettingsMapFromPrefix(settingPrefix));
    }

    public sealed class Initialize : FabricServiceConfigSectionTest
    {
        [Fact]
        public void ReturnsTrueAndInvokesOnInitializeWhenConfigurationSettingsContainsSection()
        {
            ConfigurationSection section = MakeConfigSection(sectionName);
            SetSingletonWithConfigurationSettings(section);

            Assert.True(sut.Initialize());
            onInitialize.Verify(_ => _(), Times.Once);
        }

        [Fact]
        public void ReturnsTrueAndInvokesOnInitializeWhenExeSettingsContainSection()
        {
            SetSingletonWithExeSettings(MakeExeSection(sectionName));

            Assert.True(sut.Initialize());
            onInitialize.Verify(_ => _(), Times.Once);
        }

        [Fact]
        public void ReturnsTrueWhenExeSettingsContainNonMatchingSectionBeforeMatchingSection()
        {
            // Exercises the loop-continuation branch over Settings.Section: the first section does not
            // match and Initialize must keep iterating to find the matching section. Distinguishable
            // parameter values pin that the matching (not the non-matching) section is stored in exeSection.
            string settingName = fuzzy.String().LettersOrDigits();
            string expected = fuzzy.String();
            SettingsTypeSection nonMatching = MakeExeSection(
                sectionName + "_" + fuzzy.String().LettersOrDigits(),
                MakeExeParameter(settingName, fuzzy.String()));
            SettingsTypeSection matching = MakeExeSection(sectionName, MakeExeParameter(settingName, expected));
            SetSingletonWithExeSettings(nonMatching, matching);

            Assert.True(sut.Initialize());
            onInitialize.Verify(_ => _(), Times.Once);
            Assert.Same(expected, sut.GetSetting<string>(settingName, defaultValue: null));
        }

        [Fact]
        public void ReturnsFalseWhenFabricServiceConfigIsUnavailable()
        {
            // The base ctor reset the singleton to null, and outside a Service Fabric host with no
            // entry-assembly settings file staged, GetConfig() also returns null.
            EntrySettingsFile.AssertAbsent();

            Assert.False(sut.Initialize());
            onInitialize.Verify(_ => _(), Times.Never);
        }

        [Fact]
        public void ReturnsFalseWhenBothConfigurationSettingsAndExeSettingsAreAbsent()
        {
            // Configure singleton with neither configurationSettings nor Settings populated.
            SetSingleton(Type<FabricServiceConfig>.New());

            Assert.False(sut.Initialize());
            onInitialize.Verify(_ => _(), Times.Never);
        }

        [Fact]
        public void ReturnsFalseWhenExeSettingsIsPopulatedButContainsNoSections()
        {
            // Exercises the Settings != null && Settings.Section == null branch of IsExeSettingsFileEmpty.
            var config = Type<FabricServiceConfig>.New();
            config.Settings = new SettingsType();
            SetSingleton(config);

            Assert.False(sut.Initialize());
            onInitialize.Verify(_ => _(), Times.Never);
        }

        [Fact]
        public void ReturnsFalseWhenConfigurationSettingsDoesNotContainSection()
        {
            ConfigurationSection otherSection = MakeConfigSection(sectionName + "_" + fuzzy.String().LettersOrDigits());
            SetSingletonWithConfigurationSettings(otherSection);

            Assert.False(sut.Initialize());
            onInitialize.Verify(_ => _(), Times.Never);
        }

        [Fact]
        public void ReturnsFalseWhenExeSettingsDoNotContainSection()
        {
            SetSingletonWithExeSettings(MakeExeSection(sectionName + "_" + fuzzy.String().LettersOrDigits()));

            Assert.False(sut.Initialize());
            onInitialize.Verify(_ => _(), Times.Never);
        }

        [Fact]
        public void ReturnsFalseAndIgnoresExeSettingsWhenConfigurationSettingsArePopulatedButLackSection()
        {
            // Pins the precedence: when configurationSettings is non-null, Initialize short-circuits
            // without inspecting Settings, even if Settings would have matched.
            var configSettings = Type<ConfigurationSettings>.New();
            configSettings.Sections.Add(MakeConfigSection(sectionName + "_" + fuzzy.String().LettersOrDigits()));
            var config = Type<FabricServiceConfig>.New();
            config.configurationSettings = configSettings;
            config.Settings = new SettingsType { Section = [MakeExeSection(sectionName)] };
            SetSingleton(config);

            Assert.False(sut.Initialize());
            onInitialize.Verify(_ => _(), Times.Never);
        }

        [Fact]
        public void ReturnsTrueWhenSectionNameIsPaddedWithWhitespaceAndMatchesExeSettingsSection()
        {
            SetSingletonWithExeSettings(MakeExeSection(sectionName));
            FabricServiceConfigSection paddedSectionNameSut = new($"  {sectionName}  ", onInitialize.Object);

            Assert.True(paddedSectionNameSut.Initialize());
            onInitialize.Verify(_ => _(), Times.Once);
        }

        [Fact]
        public void ReturnsFalseWhenPaddedSectionNameDoesNotExactlyMatchConfigurationSettingsSection()
        {
            // Pins the asymmetry: on the configuration-settings path, Initialize calls Sections.Contains(this.sectionName)
            // without trimming, so a padded sectionName does NOT match an un-padded section.
            SetSingletonWithConfigurationSettings(MakeConfigSection(sectionName));
            FabricServiceConfigSection paddedSectionNameSut = new($"  {sectionName}  ", onInitialize.Object);

            Assert.False(paddedSectionNameSut.Initialize());
            onInitialize.Verify(_ => _(), Times.Never);
        }
    }

    void InitializeWithConfigSection(params ConfigurationProperty[] parameters)
    {
        ConfigurationSection section = MakeConfigSection(sectionName, parameters);
        SetSingletonWithConfigurationSettings(section);
        Assert.True(sut.Initialize());
    }

    void InitializeWithExeSection(params SettingsTypeSectionParameter[] parameters)
    {
        SettingsTypeSection section = MakeExeSection(sectionName, parameters);
        SetSingletonWithExeSettings(section);
        Assert.True(sut.Initialize());
    }

    void SetSingletonWithConfigurationSettings(ConfigurationSection section)
    {
        var settings = Type<ConfigurationSettings>.New();
        settings.Sections.Add(section);
        var config = Type<FabricServiceConfig>.New();
        config.configurationSettings = settings;
        SetSingleton(config);
    }

    void SetSingletonWithExeSettings(params SettingsTypeSection[] sections)
    {
        var config = Type<FabricServiceConfig>.New();
        config.Settings = new SettingsType { Section = sections };
        SetSingleton(config);
    }

    static ConfigurationSection MakeConfigSection(string name, params ConfigurationProperty[] parameters)
    {
        var section = Type<ConfigurationSection>.New();
        section.Property<string>().Set(name);
        foreach (ConfigurationProperty parameter in parameters)
            section.Parameters.Add(parameter);
        return section;
    }

    static ConfigurationProperty MakeConfigParameter(string name, string value)
    {
        var parameter = Type<ConfigurationProperty>.New();
        parameter.Property<string>(nameof(ConfigurationProperty.Name)).Set(name);
        parameter.Property<string>(nameof(ConfigurationProperty.Value)).Set(value);
        return parameter;
    }

    static SettingsTypeSection MakeExeSection(string name, params SettingsTypeSectionParameter[] parameters) =>
        new() { Name = name, Parameter = parameters };

    static SettingsTypeSectionParameter MakeExeParameter(string name, string value) =>
        new() { Name = name, Value = value };

    public enum SampleEnum
    {
        First,
        Second,
    }
}
