namespace EulynxLive.Messages.Deprecated.NeuPro {
    public abstract class NeuProMessage : EulynxMessage {
        protected NeuProMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override sealed int Size => 128;

        protected sealed override void WritePayloadToByteArray(byte[] bytes)
        {
            // Zero-initialize message buffer
            for (var i = 43; i < Size; i++) {
                bytes[i] = 0;
            }

            NeuProWritePayloadToByteArray(bytes);
        }

        protected abstract void NeuProWritePayloadToByteArray(byte[] bytes);
    }
}
