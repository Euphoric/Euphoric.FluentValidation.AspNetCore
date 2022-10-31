using System.Runtime.CompilerServices;
using VerifyTests;

namespace Euphoric.FluentValidation.AspNetCore;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyHttp.Enable();
    }   
}