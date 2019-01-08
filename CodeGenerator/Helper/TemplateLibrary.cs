namespace CodeGenerator.Helper
{
    /// <summary>
    /// The TemplateLibrary contains the definitions for output code files.
    /// </summary>
    internal static class TemplateLibrary
    {
        internal static class CSharp
        {
            internal static string ClassFile = "{0}\n\nnamespace {1}\n{{\n\tpublic class {2}\n\t{{\n\t\t{3}\n\t}}\n}}";
        }

        internal static class TSql
        {
            internal static string QueryFile = "--GetByKey\n{0}\n\n--GetAll\n{1}\n\n--Insert\n{2}\n\n--Update\n{3}\n\n--Delete\n{4}";

            internal static string GetByKey = "SELECT {0} FROM {1}.{2} WHERE {3} = @{4};";

            internal static string GetAll = "SELECT {0} FROM {1}.{2};";

            internal static string Insert = "INSERT {0}.{1}({2}) VALUES ({3});";

            internal static string Update = "UPDATE {0}.{1} SET {2} WHERE {3} = @{4};";

            internal static string Delete = "DELETE {0}.{1} WHERE {2} = @{3};";
        }
    }
}
