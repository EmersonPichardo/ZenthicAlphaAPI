using System.Reflection;

namespace Identity.Presentation;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}