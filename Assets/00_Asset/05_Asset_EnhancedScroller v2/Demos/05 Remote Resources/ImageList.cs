using System;

namespace EnhancedScrollerDemos.RemoteResourcesDemo
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
