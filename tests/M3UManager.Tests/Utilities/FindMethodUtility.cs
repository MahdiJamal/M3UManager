using System.Reflection;

namespace M3UManager.Tests.Utilities;

public static class FindMethodUtility
{
    public static TResult CallPrivateStaticMethod<TResult>(Type classType, string methodName, params object[] methodCallArguments)
    {
        MethodInfo? detectChannelFromExtinfItemMethod = classType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        if (detectChannelFromExtinfItemMethod == null)
            throw new NullReferenceException($"'{nameof(detectChannelFromExtinfItemMethod)}' variable value is null.");

        TResult? result = (TResult?)detectChannelFromExtinfItemMethod.Invoke(null, methodCallArguments);
        return result ?? throw new NullReferenceException($"'{nameof(result)}' variable value is null.");
    }
}
