using WorldMessengerLib.WorldMessages;

namespace CollisionEngineLib.Objects
{
    public class CollisionMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.Collision;
        public Collidable FirstObject { get; set; }
        public Collidable SecondObject { get; set; }
        public string WorldName { get; set; }
    }
}