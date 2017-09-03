using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PalringoApi.Subprofile.Types
{
    /// <summary>
    /// What code is the type of Response Code given
    /// </summary>
    public enum What
    {
        /// <summary>
        /// User attempted to Add a contact
        /// </summary>
        ADD_CONTACT = 0,
        /// <summary>
        /// User attempted to subscribe to a group
        /// </summary>
        SUBSCRIBE_TO_GROUP = 1,
        /// <summary>
        /// User attempted to create a group
        /// </summary>
        CREATE_GROUP = 2,
        /// <summary>
        /// User attempted to modified a user on their contact list
        /// </summary>
        UPDATE_CONTACT = 3,
        /// <summary>
        /// User attempted to unsubscribe from a group
        /// </summary>
        UNSUB_GROUP = 4,
        /// <summary>
        /// User attempted to update their status (online/offline)
        /// </summary>
        UPDATE_PRESENCE = 5,
        /// <summary>
        /// User attempted to update their nickname
        /// </summary>
        UPDATE_NICKNAME = 6,
        /// <summary>
        /// User attempted to update their status message
        /// </summary>
        UPDATE_STATUS_MESG = 7,
        /// <summary>
        /// User attempted to authorize a contact (not sure what that means..)
        /// </summary>
        CONTACT_AUTHORISATION = 8,
        /// <summary>
        /// User attempted a destination query (User location)
        /// </summary>
        DEST_QUERY = 9,
        /// <summary>
        /// User attempted a group invite
        /// </summary>
        GROUP_INVITE = 10,
        /// <summary>
        /// User attempted to send a message
        /// </summary>
        MESG = 11,
        /// <summary>
        /// User stored a message in history
        /// </summary>
        MESG_STORE = 12,
        /// <summary>
        /// User stats attempted to be stored on server
        /// </summary>
        STATS_LOGGING = 13,
        /// <summary>
        /// User attempted to bridge to a different server type (facebook ect)
        /// </summary>
        BRIDGING = 14,
        /// <summary>
        /// User attempted a bridge message
        /// </summary>
        BRIDGING_MESG = 15,
        /// <summary>
        /// Not sure
        /// </summary>
        BRI = 16,
        /// <summary>
        /// Again, not sure
        /// </summary>
        PLS = 17,
        /// <summary>
        /// user attempted a subprofile query
        /// </summary>
        SUB_QUERY = 18,
        /// <summary>
        /// User attempted an admin action
        /// </summary>
        GROUP_ADMIN = 19,
        /// <summary>
        /// User attempted a request from resources on the website
        /// </summary>
        URL_REQUEST = 20,
        /// <summary>
        /// User obtained an error form a URL request
        /// </summary>
        URL_ERROR = 21,
        /// <summary>
        /// Ignore the packet at hand
        /// </summary>
        IGNORE = 22,
        /// <summary>
        /// User attempted an icon (avatar) fetch
        /// </summary>
        ICON = 23,
        /// <summary>
        /// User attempted to login
        /// </summary>
        LOGON = 24,
        /// <summary>
        /// Old admin action thing
        /// </summary>
        AD_ACTION = 25,
        /// <summary>
        /// User attempted a group update (join/leave/info)
        /// </summary>
        UPDATE_GROUP = 26,
        /// <summary>
        /// User is being asked to prove something
        /// </summary>
        PROV_IF = 27,
        /// <summary>
        /// User attempted a Balance Query
        /// </summary>
        BALANCE_QUERY = 28,
        /// <summary>
        /// User attempted something that gives a call back.
        /// </summary>
        CALLBACK = 29
    }
}
