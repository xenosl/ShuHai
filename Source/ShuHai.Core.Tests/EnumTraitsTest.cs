using System;
using NUnit.Framework;

namespace ShuHai.Tests
{
    public class EnumTraitsTest
    {
        public enum TestEnum
        {
            Add = 1, // 0
            [Obsolete] Delete = 2,
            Remove = 2, // 1
            remove = 2, // 2
            Update = 3, // 3
            update = 3, // 4
            Clear = 4, // 5
        }

        [TestCase(0, ExpectedResult = "Add")]
        [TestCase(1, ExpectedResult = "Remove")]
        [TestCase(2, ExpectedResult = "remove")]
        [TestCase(3, ExpectedResult = "Update")]
        [TestCase(4, ExpectedResult = "update")]
        [TestCase(5, ExpectedResult = "Clear")]
        public string Names(int index) { return EnumTraits<TestEnum>.Names[index]; }

        [TestCase(0, ExpectedResult = TestEnum.Add)]
        [TestCase(1, ExpectedResult = TestEnum.Remove)]
        [TestCase(2, ExpectedResult = TestEnum.remove)]
        [TestCase(3, ExpectedResult = TestEnum.Update)]
        [TestCase(4, ExpectedResult = TestEnum.update)]
        [TestCase(5, ExpectedResult = TestEnum.Clear)]
        public TestEnum Values(int index) { return EnumTraits<TestEnum>.Values[index]; }

        [TestCase(TestEnum.Add, 0)]
        [TestCase(TestEnum.Remove, 1, 2)]
        [TestCase(TestEnum.remove, 1, 2)]
        [TestCase(TestEnum.Update, 3, 4)]
        [TestCase(TestEnum.update, 3, 4)]
        [TestCase(TestEnum.Clear, 5)]
        public void ValueToIndices(TestEnum value, params int[] expectedIndices)
        {
            var indices = EnumTraits<TestEnum>.ValueToIndices[value];
            CollectionAssert.AreEquivalent(expectedIndices, indices);
        }

        [TestCase("Add", ExpectedResult = 0)]
        [TestCase("Remove", ExpectedResult = 1)]
        [TestCase("remove", ExpectedResult = 2)]
        [TestCase("Update", ExpectedResult = 3)]
        [TestCase("update", ExpectedResult = 4)]
        [TestCase("Clear", ExpectedResult = 5)]
        public static int NameToIndex(string name) { return EnumTraits<TestEnum>.NameToIndex[name]; }

        [TestCase("Add", ExpectedResult = TestEnum.Add)]
        [TestCase("Remove", ExpectedResult = TestEnum.Remove)]
        [TestCase("remove", ExpectedResult = TestEnum.remove)]
        [TestCase("Update", ExpectedResult = TestEnum.Update)]
        [TestCase("update", ExpectedResult = TestEnum.update)]
        [TestCase("Clear", ExpectedResult = TestEnum.Clear)]
        public static TestEnum NameToValue(string name) { return EnumTraits<TestEnum>.NameToValue[name]; }

        [TestCase(TestEnum.Add, "Add")]
        [TestCase(TestEnum.Remove, "Remove", "remove")]
        [TestCase(TestEnum.remove, "Remove", "remove")]
        [TestCase(TestEnum.Update, "Update", "update")]
        [TestCase(TestEnum.update, "Update", "update")]
        [TestCase(TestEnum.Clear, "Clear")]
        public static void ValueToNames(TestEnum value, params string[] expectedNames)
        {
            var names = EnumTraits<TestEnum>.ValueToNames[value];
            CollectionAssert.AreEquivalent(expectedNames, names);
        }
    }
}