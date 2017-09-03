namespace PalringoApi.Subprofile.Types
{
    /// <summary>
    /// Code is what did or didn't happen in a response
    /// </summary>
    public enum Code
    {
        /// <summary>
        /// Thing happened fine
        /// </summary>
        OK = 0,
        /// <summary>
        /// Used for palringo only stuff
        /// </summary>
        INTERNAL_CODE = 1,
        /// <summary>
        /// Thingy-ma-bob didn't exist
        /// </summary>
        NO_SUCH_WHATEVER = 2,
        /// <summary>
        /// User isn't a member of that group
        /// </summary>
        NOT_A_MEMBER = 3,
        /// <summary>
        /// Message was delivered
        /// </summary>
        DELIVERED = 4,
        /// <summary>
        /// Message wasn't delivered
        /// </summary>
        NOT_DELIVERED = 5,
        /// <summary>
        /// No clue
        /// </summary>
        SFE_NOT_AVAILABLE = 6,
        /// <summary>
        /// No clue
        /// </summary>
        STATS_IF_NOT_AVAILABLE = 7,
        /// <summary>
        /// No more message history
        /// </summary>
        END_OF_MESG_STORED = 8,
        /// <summary>
        /// Could not store message history
        /// </summary>
        UNABLE_TO_STORE_OFFLINE_MESG = 9,
        /// <summary>
        /// Wants user to resend message
        /// </summary>
        RESEND_CURRENT_MESG = 10,
        /// <summary>
        /// Group already exists... Self explainitory, why do I even bother...
        /// </summary>
        GROUP_ALREADY_EXISTS = 11,
        /// <summary>
        /// Contact is already in your contacts list
        /// </summary>
        CONTACT_ALREADY_EXISTS = 12,
        /// <summary>
        /// YOU SHALL NOT PASS
        /// </summary>
        NOT_ALLOWED = 13,
        /// <summary>
        /// Can't connect to other social networks (like facebook, ect)
        /// </summary>
        BRIDGING_NOT_AVAILABLE = 14,
        /// <summary>
        /// You be throttled.
        /// </summary>
        THROTTLED = 15,
        /// <summary>
        /// Sub profile update already exists server side
        /// </summary>
        SUB_ALREADY_EXIST = 16,
        /// <summary>
        /// No more room left for your sorry ass.
        /// </summary>
        GROUP_FULL = 17,
        /// <summary>
        /// YOU SHALL NOT ENTER AGAIN
        /// </summary>
        BANNED = 18,
        /// <summary>
        /// Have to pay to get in group?
        /// </summary>
        PAY_GROUP = 19,
        /// <summary>
        /// You're in too many groups
        /// </summary>
        TOO_MANY_GROUPS = 20,
        /// <summary>
        /// ... Do i really...
        /// </summary>
        LOGIN_INCORRECT = 21,
        /// <summary>
        /// No more contacts for you
        /// </summary>
        CONTACT_LIST_FULL = 22,
        /// <summary>
        /// Useless staff want to feel like they is loved! Contact them!
        /// </summary>
        CONTACT_SUPPORT = 23,
        /// <summary>
        /// Can't login until x date
        /// </summary>
        LOGON_SUSPENDED = 24,
        /// <summary>
        /// Your account is gone bro.
        /// </summary>
        LOGON_BARRED = 25,
        /// <summary>
        /// Groupo no-o existo (spanish. woot.)
        /// </summary>
        GROUP_NOT_FOUND = 26,
        /// <summary>
        /// Stop giving us issues ;~;
        /// </summary>
        TOO_MUCH_DATA_QUEUED = 27,
        /// <summary>
        /// No connection
        /// </summary>
        TP_SESSION_EXPIRED = 28,
        /// <summary>
        /// Someone else jacked your session
        /// </summary>
        TP_SESSION_GHOSTED = 29,
        /// <summary>
        /// Session going what?
        /// </summary>
        TP_SESSION_UNKNOWN = 30,
        /// <summary>
        /// Resume session on different server
        /// </summary>
        RESUME_OTHER_IP = 31,
        /// <summary>
        /// Reconnect to pal on a different server
        /// </summary>
        RECONNECT_OTHER_IP = 32,
        /// <summary>
        /// You ain't got permission son.
        /// </summary>
        INSUFFICIENT_PRIVILEGES = 33,
        /// <summary>
        /// Other person don't want you,.
        /// </summary>
        TARGET_CONTACT_LIST_FULL = 34
    }
}
