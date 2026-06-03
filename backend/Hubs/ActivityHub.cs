using Microsoft.AspNetCore.SignalR;

namespace HyruleArchives.Hubs;

// SignalR hubs are the real-time communication layer.
//
// This hub will be used for the live activity feed.
// The backend can broadcast activity events to all connected clients,
// and the React frontend will listen for those events later.
public class ActivityHub : Hub
{
    // For now, this hub does not need client-callable methods.
    //
    // The important part is that other backend services can use IHubContext<ActivityHub>
    // to send messages to all connected users.
    //
    // Later, if we want users to join specific groups like:
    // - "game-{gameId}"
    // - "user-{userId}"
    // we can add methods here.
}