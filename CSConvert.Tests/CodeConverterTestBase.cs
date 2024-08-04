using CSConvert.Abstractions;
using CSConvert.TypeScript;

namespace CSConvert.Tests
{
    public abstract class CodeConverterTestBase
    {
        protected abstract Type[] types { get; set; }
        protected abstract ICodeConverter converter { get; }

        protected void TestConverter(Type type, string expected)
        {
            var processedTypes = new HashSet<string>();

            string actual = converter.Convert(types.First(t => t.Name == type.Name)).Trim();

            Assert.Equal(expected.Trim(), actual);
        }

    }
}