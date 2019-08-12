namespace backend.Services {
    public class LoggingEvents {
        public const int ProcessStarted = 1001;

        public const int GetItem = 2001;
        public const int AddItem = 2002;
        public const int UpdateItem = 2003;

        public const int SendItem = 2003;

        public const int ItemNotFound = 4001;
        public const int ValidationError = 4002;
        public const int UnknownError = 4003;
    }
}