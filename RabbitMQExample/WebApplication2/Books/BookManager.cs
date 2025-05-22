namespace ZteCommom.Book
{
    public class BookManager
    {
        public object lck = new object();
        public List<BookInfo> yuyueInfos = new List<BookInfo>();

        public BookManager()
        {
        }
         
        public int Push(string token, int yuyueDeviceNum, YuyueCallback callback)
        {
            lock (lck)
            {
                var idx = yuyueInfos.FindIndex(x => x.token == token);
                if (idx >= 0)
                {
                    if (yuyueInfos[idx].yuyueDeviceNum != yuyueDeviceNum)
                        yuyueInfos[idx].yuyueDeviceNum = yuyueDeviceNum;
                    return idx;
                }
                else
                {
                    BookInfo yuyueInfo1 = new BookInfo();
                    yuyueInfo1.token = token;
                    yuyueInfo1.yuyueDeviceNum = yuyueDeviceNum;
                    yuyueInfo1.callback = callback;
                    yuyueInfos.Add(yuyueInfo1);
                    return yuyueInfos.Count - 1;
                }
            }
        }

        public void Pop(string token)
        {
            lock (lck)
            {
                var idx = yuyueInfos.FindIndex(x => x.token == token);
                if (idx >= 0)
                {
                    yuyueInfos.RemoveAt(idx);
                }
            }
        }
    }

    public delegate void YuyueCallback(int reqNum, string resourceId);
    public class BookInfo
    {
        public string token;
        public int yuyueDeviceNum;
        public YuyueCallback callback;
    }
}
