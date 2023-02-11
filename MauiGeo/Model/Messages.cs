using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MauiGeo;

public class GeoServiceStartedMessage : ValueChangedMessage<bool>
{
    public GeoServiceStartedMessage(bool value) : base(value)
    {
    }
}

public class GeoServiceStoppedMessage : ValueChangedMessage<bool>
{
    public GeoServiceStoppedMessage(bool value) : base(value)
    {
    }
}

public class LocationUpdatedMessage : ValueChangedMessage<Location>
{
    public LocationUpdatedMessage(Location value) : base(value)
    {
    }
}

public class LoginStatusMessage : ValueChangedMessage<bool>
{
    public LoginStatusMessage(bool value) : base(value)
    {
    }
}
