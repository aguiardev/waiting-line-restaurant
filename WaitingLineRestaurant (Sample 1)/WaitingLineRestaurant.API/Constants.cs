namespace WaitingLineRestaurant.API
{
    public static class Constants
    {
        public const string QUERY_PARAM_PHONE = "phone";
        public const string URL_SSE = "/v1/account-summary";
        
        /// <summary>
        /// When the position is updated
        /// </summary>
        public const string MSG_TYPE_UPDATE_POSITION = "update";
        
        /// <summary>
        /// When your turn has came
        /// </summary>
        public const string MSG_TYPE_NEXT = "next";
        
        public const string MSG_NEXT = "next";
    }
}