using System;

namespace _00_Asset._05_Asset_EnhancedScroller_v2.Demos._05_Remote_Resources
{
    [Serializable]
    public class RemoteImageList
    {
        public RemoteImage[] images;
    }

    [Serializable]
    public class RemoteImage
    {
        public string url;
        public RemoteImageSize size;
    }

    [Serializable]
    public class RemoteImageSize
    {
        public float x;
        public float y;
    }
}
