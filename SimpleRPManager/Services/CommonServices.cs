using shortid;
using shortid.Configuration;

namespace SimpleRPManager.Services;

public class CommonServices
{
    private static GenerationOptions genOpts = new GenerationOptions(true, false);
    public static string GenerateSimpleUid()
    {
        return ShortId.Generate(genOpts);
    }
}