namespace ProxyParser.Interfaces
{
    public enum ResponceCode
    {
        OK = 100,
        BadRequest = 300,
        Forbidden = 403,
        Notfound = 404,
        NotAcceptable = 406,
        RequestTimeout = 408,
        RequiredCredentials = 415,
        DataError = 416,
        Conflict = 409,
        BadGateway = 502,
        Dead,
        Unknow
    };

    public class RequestResponce
    {
        public RequestResponce(object responceData, ResponceCode requestCode) 
        {
            Data = responceData;
            Code = requestCode;
        }

        public object Data;
        public ResponceCode Code;

    }
}
