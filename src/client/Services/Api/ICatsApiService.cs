namespace EndlessCatsApp.Services.Api
{
    public interface ICatsApiService
    {
        ICatsApi Background { get; }
        ICatsApi Speculative { get; }
        ICatsApi UserInitiated { get; }
    }
}
