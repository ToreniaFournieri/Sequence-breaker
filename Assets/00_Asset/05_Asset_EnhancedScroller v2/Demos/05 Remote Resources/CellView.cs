using System.Collections;
using _00_Asset._05_Asset_EnhancedScroller_v2.Plugins;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
#if UNITY_2017_4_OR_NEWER

#endif

namespace _00_Asset._05_Asset_EnhancedScroller_v2.Demos._05_Remote_Resources
{
    public class CellView : EnhancedScrollerCellView
    {
        public Image cellImage;
        public Sprite defaultSprite;

        private Coroutine _loadImageCoroutine;

        public void SetData(Data data)
        {
            _loadImageCoroutine = StartCoroutine(LoadRemoteImage(data));
        }

        public IEnumerator LoadRemoteImage(Data data)
        {
            string path = data.ImageUrl;

            Texture2D texture = null;

            // Get the remote texture

#if UNITY_2017_4_OR_NEWER
            var webRequest = UnityWebRequestTexture.GetTexture(path);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Failed to download image [" + path + "]: " + webRequest.error);
            }
            else
            {
                texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
            }
#else
            WWW www = new WWW(path);
            yield return www;
            texture = www.texture;
#endif

            if (texture != null)
            {
                cellImage.sprite = Sprite.Create(texture, new Rect(0, 0, data.ImageDimensions.x, data.ImageDimensions.y), new Vector2(0, 0), data.ImageDimensions.x);
            }
            else
            {
                ClearImage();
            }
        }

        public void ClearImage()
        {
            cellImage.sprite = defaultSprite;
        }

        /// <summary>
        /// Stop the coroutine if the cell is going to be recycled
        /// </summary>
        public void WillRecycle()
        {
            if (_loadImageCoroutine != null)
            {
                StopCoroutine(_loadImageCoroutine);
            }
        }
    }
}