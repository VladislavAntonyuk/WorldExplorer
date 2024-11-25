namespace Client.Services.Navigation;

internal record NavigationState(Uri State, IDictionary<string, object> Parameters);