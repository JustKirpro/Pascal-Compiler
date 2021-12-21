using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalCompiler
{
    class Program
    {
        static void Main()
        {
            AssemblyName assemblyName = new AssemblyName("Program");

            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Program.exe", true);

            TypeBuilder typeBuilder = moduleBuilder.DefineType("Program", TypeAttributes.Public);

            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Main", MethodAttributes.Static | MethodAttributes.Public, typeof(void), new System.Type[] { });

            ILGenerator ILGenerator = methodBuilder.GetILGenerator();

            Compiler compiler = new Compiler(Path.Combine(Environment.CurrentDirectory, "input.pas"), Path.Combine(Environment.CurrentDirectory, "output.txt"), ILGenerator);

            compiler.Start();

            assemblyBuilder.SetEntryPoint(methodBuilder);

            ILGenerator.Emit(OpCodes.Ret);

            System.Type type = typeBuilder.CreateType();

            assemblyBuilder.Save("Program.exe");
        }
    }
}