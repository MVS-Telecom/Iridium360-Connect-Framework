namespace Rock.Iridium360.Messaging
{
    public abstract class MessageMO : Message
    {
        protected MessageMO()
        {
        }

        public sealed override Rock.Iridium360.Messaging.Direction Direction =>
            Rock.Iridium360.Messaging.Direction.MO;
    }

}

